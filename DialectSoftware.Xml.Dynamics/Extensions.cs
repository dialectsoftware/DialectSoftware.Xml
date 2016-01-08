using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Xml
{
    public static class Extensions
    {
        public static dynamic CreateDynamic(this XDocument doc, string node)
        {
            return
            new DynamicConverter(doc.Descendants().Where(n =>
            {
                return n is XElement && ((XElement)n).Name == XName.Get(node);
            })
            .Select(e => new DynamicElement(e)));
        }
        public static dynamic CreateDynamic(this XDocument doc, string node, string namespaceName)
        {
            return
            new DynamicConverter(doc.Descendants().Where(n =>
            {
                return n is XElement && ((XElement)n).Name == XName.Get(node, namespaceName);
            })
            .Select(e => new DynamicElement(e)));
        }
        
    }
}
