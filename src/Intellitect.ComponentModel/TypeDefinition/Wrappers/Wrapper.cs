﻿using Intellitect.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intellitect.ComponentModel.TypeDefinition.Wrappers
{
    internal abstract class Wrapper
    {
        public abstract string Name { get; }

        public abstract object GetAttributeValue<TAttribute>(string valueName) where TAttribute : Attribute;
        public abstract bool HasAttribute<TAttribute>() where TAttribute : Attribute;
        public T? GetAttributeValue<TAttribute, T>(string valueName) where TAttribute : Attribute where T : struct
        {
            var result = GetAttributeValue<TAttribute>(valueName);
            if (result == null)
            {
                return null;
            }
            return new Nullable<T>((T)result);
        }
        public T GetAttributeObject<TAttribute, T>(string valueName) where TAttribute : Attribute where T : class
        {
            return GetAttributeValue<TAttribute>(valueName) as T;
        }



    }
}
