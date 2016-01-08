using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Xml.Linq;
using System.Xml.XPath;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Xml
{
    public class DynamicElement : DynamicObject
    {
        XElement xElement;
        
        public DynamicElement(XElement element)
        {
            xElement = element;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var elements = xElement.Elements()
                                    .Where(e => e.Name == XName.Get(binder.Name, xElement.Name.NamespaceName))
                                    .Select(e => new DynamicElement(e));
            if (elements.Count() == 0)
            {
                var attributes = xElement.Attributes().Where(a=>a.Name == XName.Get(binder.Name))
                                 .DefaultIfEmpty(null)
                                 .SingleOrDefault();
                if (attributes == null)
                { 
                    result = new DynamicConverter();
                }
                else
                {  
                    result = new DynamicAttribute(attributes);
                }
            }
            else
            {
                result = new DynamicConverter(elements);
            }
            return true;
        }
        public override string ToString()
        {
            return xElement.Value;
        }

        public static implicit operator DynamicElement(XElement element)
        {
            return new DynamicElement(element);
        }

        public static implicit  operator XElement(DynamicElement node)
        {
            return node.xElement;
        }
    }
}
