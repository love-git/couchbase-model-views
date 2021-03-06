﻿#region [ License information          ]
/* ************************************************************
 * 
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2012 Couchbase, Inc.
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CouchbaseModelViews.Framework.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class CouchbaseCollatedViewKeyAttribute : CouchbaseViewKeyAttributeBase
	{
		public string RelationName { get; set; }

		public string RelationPropertyName { get; set; }

		public CouchbaseCollatedViewKeyAttribute(string viewName, string relationName, string relationPropertyName, string propertyName = "", int order = 0)
		{
			RelationName = relationName;
			ViewName = viewName;
			PropertyName = propertyName;
			RelationPropertyName = relationPropertyName;
			Order = order;
		}
	}
}
