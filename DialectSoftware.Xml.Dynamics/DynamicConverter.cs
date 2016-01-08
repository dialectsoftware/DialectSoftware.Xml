using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Xml
{
    public class DynamicConverter : DynamicObject,IEnumerable 
    {
        IEnumerable<DynamicElement> dynamicNodes;

        internal DynamicConverter()
        {
            dynamicNodes = new DynamicElement[] { };
        }
        internal DynamicConverter(IEnumerable<DynamicElement> element)
        {
            dynamicNodes = element;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this;
            bool success = true;
            if(dynamicNodes.Count() == 1)
            {
                success = success && dynamicNodes.Single().TryGetMember(binder, out result);
            }
            return success;
        }
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType.IsValueType)
                result = Activator.CreateInstance(binder.ReturnType);
            else
                result = null;

            if (binder.ReturnType.IsValueType || binder.ReturnType == typeof(string))
            {
                try
                {
                    result = System.Convert.ChangeType(dynamicNodes.Single().ToString(), binder.ReturnType);
                }
                catch
                {
                    return false;
                }
            }
            else if (binder.ReturnType.IsArray || typeof(System.Array).IsAssignableFrom(binder.ReturnType))
            {
                if (binder.ReturnType.GetElementType() == null)
                    result = dynamicNodes.ToArray();
                else
                {
                    var temp = dynamicNodes
                    .Select(node =>
                    {
                        object value = null;
                        if (binder.ReturnType.IsValueType)
                            value = Activator.CreateInstance(binder.ReturnType);
                        else
                            value = null;
                        try
                        {
                            value = System.Convert.ChangeType(node.ToString(), binder.ReturnType.GetElementType());
                        }
                        catch { }

                        return value;
                    })
                    .Where(value => value != null)
                    .ToArray();

                    var array = Array.CreateInstance(binder.ReturnType.GetElementType(), temp.Count());
                    temp.CopyTo(array, 0);
                    result = array;
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(binder.ReturnType))
            {
                result = this;
            }
            else
            {
                return false;
            }

            return true;
        }
        public override string ToString()
        {
            return (String)((dynamic)this);
        }
        public IEnumerator GetEnumerator()
        {
            return new Enumerate(dynamicNodes.GetEnumerator());
        }
        public IEnumerable Select(string xpath, bool group = false)
        {
            var nodes = dynamicNodes.SelectMany(node => ((XElement)node).XPathSelectElements(xpath).Select(element => (DynamicElement)element));
            if (group)
                return nodes.GroupBy(node => ((XElement)node).Name).Select(grouping => new DynamicConverter(grouping.Select(node=>node)));
            return new DynamicConverter(nodes);
        }
        internal class Enumerate : IEnumerator
        {
            IEnumerator enumeration;
            internal Enumerate(IEnumerator enumerator)
            {
                enumeration = enumerator;
            }
            public object Current
            {
                get
                {
                   return new DynamicConverter(new[] { (DynamicElement)enumeration.Current });
                }
            }
            public bool MoveNext()
            {
                return enumeration.MoveNext();
            }
            public void Reset()
            {
                enumeration.Reset();
            }
        }
       
    }
}
