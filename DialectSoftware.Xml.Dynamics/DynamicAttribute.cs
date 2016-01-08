using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Xml.Linq;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Xml
{
    public class DynamicAttribute : DynamicObject
    {
        XAttribute xAttribute;
      
        public DynamicAttribute(XAttribute attribute)
        {
            xAttribute = attribute;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new DynamicConverter();
            return true; 
        }
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType.IsValueType)
                result = Activator.CreateInstance(binder.ReturnType);
            else
                result = null;
            try
            {
                result = System.Convert.ChangeType(ToString(), binder.ReturnType);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        public override string ToString()
        {
            return xAttribute.Value;
        }

        public static implicit operator DynamicAttribute(XAttribute attribute)
        {
            return new DynamicAttribute(attribute);
        }

        public static implicit operator XAttribute(DynamicAttribute node)
        {
            return node.xAttribute;
        }
    }
}
