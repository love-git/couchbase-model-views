﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CouchbaseModelViews.Framework.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CouchbaseModelViewsGenerator
{
    public class ViewBuilder
    {
        private IList<Assembly> _assemblies = new List<Assembly>();
        private IDictionary<string, string> _designDocs = new Dictionary<string, string>();

        public void AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
        }

        public void AddAssemblies(IList<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public IDictionary<string, string> Build()
        {
            buildTypes();
            return _designDocs;
        }

        private void buildTypes()
        {
            foreach (var assembly in _assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var typesAndViews = new Dictionary<string, Dictionary<string, List<string>>>();

                    var designDoc = "";
                    foreach (CouchbaseDesignDocAttribute attribute in type.GetCustomAttributes(true).Where(a => a is CouchbaseDesignDocAttribute))
                    {
                        designDoc = string.IsNullOrEmpty(attribute.Name) ? type.Name.ToLower() : attribute.Name;
                        typesAndViews[designDoc] = new Dictionary<string, List<string>>();
                    }

                    foreach (var prop in type.GetProperties())
                    {                        
                        foreach (CouchbaseViewKeyAttribute attr in prop.GetCustomAttributes(typeof(CouchbaseViewKeyAttribute), true))
                        {
                            var viewName = attr.ViewName;
                            if (!typesAndViews[designDoc].ContainsKey(viewName))
                            {
                                typesAndViews[designDoc][viewName] = new List<string>();
                            }

                            var propName = string.IsNullOrEmpty(attr.PropertyName) ? prop.Name : attr.PropertyName;
                            typesAndViews[designDoc][viewName].Add(propName);
                        }
                    }

                    buildJson(typesAndViews);   
                }
        }

        private void buildJson(Dictionary<string, Dictionary<string, List<string>>> typesAndViews)
        {
            foreach (var type in typesAndViews.Keys)
            {
                var jObject = new JObject();
                jObject["views"] = new JObject();

                foreach (var value in typesAndViews.Values)
                {
                    foreach (var key in value.Keys)
                    {
                        var map = new JObject();
                        map["map"] = getFunction(value[key]);
                        jObject["views"][key] = map;
                    }
                }
                
                Console.WriteLine(jObject.ToString());
            }
        }

        private string getFunction(List<string> values)
        {
            var template = "function(doc, meta) {{ emit({0}, null); }}";

            var retval = "";
            if (values.Count == 1)
            {
                retval = "doc." + values[0];
            }
            else
            {
                var keys = values.Select(s => "doc." + s);

                retval = "[";
                foreach (var key in keys)
                {
                    retval += key + ",";
                }
                retval = retval.Remove(retval.Length-1);
                retval += "]";
            }

            return string.Format(template, retval);
        }
    }
}