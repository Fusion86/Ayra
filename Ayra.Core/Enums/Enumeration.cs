﻿using System.Collections.Generic;
using System.Reflection;

namespace Ayra.Core.Enums
{
    // Based on https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
    public abstract class Enumeration
    {
        public readonly byte[] Identifier;
        public readonly string Name;

        public Enumeration() { }

        protected Enumeration(byte[] identifier, string name)
        {
            Identifier = identifier;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Identifier.Equals(otherValue.Identifier);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;

                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }
    }
}
