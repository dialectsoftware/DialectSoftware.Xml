using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Xml
{
    public static class DynamicParser
    {
        public static dynamic Load(Uri uri)
        {
            var doc = XDocument.Load(uri.ToString());
            var element = ((XElement)doc.FirstNode);
            if (String.IsNullOrEmpty(element.Name.NamespaceName))
                return doc.CreateDynamic(element.Name.LocalName);
            else
                return doc.CreateDynamic(element.Name.LocalName, element.Name.NamespaceName);
        }
       
        public static dynamic Parse(string xml)
        {
            var doc = XDocument.Parse(xml);
            var element = ((XElement)doc.FirstNode);
            if (String.IsNullOrEmpty(element.Name.NamespaceName))
                return doc.CreateDynamic(element.Name.LocalName);
            else
                return doc.CreateDynamic(element.Name.LocalName, element.Name.NamespaceName);
        }

        public static dynamic Load(Stream stream)
        {
            var xdoc = XDocument.Load(stream);
            return Load(xdoc);
        }

        public static dynamic Load(XDocument doc)
        {
            var element = ((XElement)doc.FirstNode);
            if (String.IsNullOrEmpty(element.Name.NamespaceName))
                return doc.CreateDynamic(element.Name.LocalName);
            else
                return doc.CreateDynamic(element.Name.LocalName, element.Name.NamespaceName);
        }

        public static dynamic Load(System.Xml.XPath.IXPathNavigable doc)
        {
            var xdoc = XDocument.Load(doc.CreateNavigator().ReadSubtree());
            return Load(xdoc);
        }
    }
}
