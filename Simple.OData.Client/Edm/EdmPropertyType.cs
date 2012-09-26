﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace Simple.OData.Client
{

    public abstract class EdmPropertyType
    {
        public abstract string Name { get; }

        public static EdmPropertyType Parse(string s, IEnumerable<EdmComplexType> complexTypes, IEnumerable<EdmEntityType> entityTypes)
        {
            var result1 = TryParseCollectionType(s, complexTypes, entityTypes);
            if (result1.Item1)
            {
                return result1.Item2;
            }

            var result2 = TryParsePrimitiveType(s);
            if (result2.Item1)
            {
                return result2.Item2;
            }

            var result3 = TryParseComplexType(s, complexTypes);
            if (result3.Item1)
            {
                return result3.Item2;
            }

            var result4 = TryParseEntityType(s, entityTypes);
            if (result4.Item1)
            {
                return result4.Item2;
            }

            throw new ArgumentException(string.Format("Unrecognized type {0}", s));
        }

        private static Tuple<bool, EdmCollectionPropertyType> TryParseCollectionType(string s, IEnumerable<EdmComplexType> complexTypes, IEnumerable<EdmEntityType> entityTypes)
        {
            if (s.StartsWith("Collection(") && s.EndsWith(")"))
            {
                int start = s.IndexOf("(");
                int end = s.LastIndexOf(")");
                var baseType = EdmPropertyType.Parse(s.Substring(start + 1, end - start - 1), complexTypes, entityTypes);
                return new Tuple<bool, EdmCollectionPropertyType>(true, new EdmCollectionPropertyType() { BaseType = baseType });
            }
            else
            {
                return new Tuple<bool, EdmCollectionPropertyType>(false, null);
            }
        }

        private static Tuple<bool, EdmPrimitivePropertyType> TryParsePrimitiveType(string s)
        {
            var result = EdmType.TryParse(s);
            if (result.Item1)
            {
                return new Tuple<bool, EdmPrimitivePropertyType>(true, new EdmPrimitivePropertyType { Type = result.Item2 });
            }
            else
            {
                return new Tuple<bool, EdmPrimitivePropertyType>(false, null);
            }
        }

        private static Tuple<bool, EdmComplexPropertyType> TryParseComplexType(string s, IEnumerable<EdmComplexType> complexTypes)
        {
            var result = EdmComplexType.TryParse(s, complexTypes);
            if (!result.Item1)
                result = EdmComplexType.TryParse(s.Split('.').Last(), complexTypes);
            if (result.Item1)
            {
                return new Tuple<bool, EdmComplexPropertyType>(true, new EdmComplexPropertyType { Type = result.Item2 });
            }
            else
            {
                return new Tuple<bool, EdmComplexPropertyType>(false, null);
            }
        }

        private static Tuple<bool, EdmEntityPropertyType> TryParseEntityType(string s, IEnumerable<EdmEntityType> entityTypes)
        {
            var result = EdmEntityType.TryParse(s, entityTypes);
            if (!result.Item1)
                result = EdmEntityType.TryParse(s.Split('.').Last(), entityTypes);
            if (result.Item1)
            {
                return new Tuple<bool, EdmEntityPropertyType>(true, new EdmEntityPropertyType { Type = result.Item2 });
            }
            else
            {
                return new Tuple<bool, EdmEntityPropertyType>(false, null);
            }
        }
    }

    public class EdmPrimitivePropertyType : EdmPropertyType
    {
        public EdmType Type { get; set; }
        public override string Name { get { return Type == null ? null : Type.Name; } }
    }

    public class EdmComplexPropertyType : EdmPropertyType
    {
        public EdmComplexType Type { get; set; }
        public override string Name { get { return Type == null ? null : Type.Name; } }
    }

    public class EdmEntityPropertyType : EdmPropertyType
    {
        public EdmEntityType Type { get; set; }
        public override string Name { get { return Type == null ? null : Type.Name; } }
    }

    public class EdmCollectionPropertyType : EdmPropertyType
    {
        public EdmPropertyType BaseType { get; set; }
        public override string Name { get { return BaseType == null ? null : string.Format("Collection({0})", BaseType.Name); } }
    }
}
