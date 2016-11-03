using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ObjectUtils
{
    public class ParseResult : IParseItem
    {
        private ParseResult(string typeName, string fieldName, string value)
        {
            TypeName = typeName;
            FieldName = fieldName;
            Value = value;
            Trace.WriteLine(ToString());
        }

        public static IParseItem CreateForField(string type, string name, string value)
        {
            return new ParseResult(type, name, value);
        }

        public static IParseItem CreateForContainer(string type, string name)
        {
            return new ParseResult(type, name, string.Empty);
        }

        public string TypeName { get; }
        public string FieldName { get; }
        public string Value { get; }

        public override string ToString()
        {
            return $"{TypeName} {FieldName} {Value}";
        }
    }

    public class ObjectWalker
    {
        private static HashSet<Type> _genericDefinitions = new HashSet<Type>
        {
            typeof(Dictionary<,>),
            typeof(List<>),
            typeof(LinkedList<>),
            typeof(HashSet<>),
            typeof(Queue<>),
            typeof(Stack<>)
        };

        class KeyValue
        {
            public object Key { get; set; }
            public object Value { get; set; }
        }
        public static void WalkObject(object obj, IObjectWalker objectWalker)
        {
            WalkObject(obj, objectWalker, Int32.MaxValue);
        }

        public static void WalkObject(object obj, IObjectWalker objectWalker, int depth)
        {
            Action<IObjectWalker, object, string, int> parseObject = null;
            objectWalker.OnStart(obj.GetType().Name);
            parseObject = (walker, o, txt, dep) =>
            {
                if (dep-- == 0) return;

                if (o == null)
                {
                    walker.OnField(ParseResult.CreateForField("Unknown", txt, "null"));
                    return;
                }
                var t = o.GetType();
                if (t.IsValueType)
                {
                    walker.OnField(ParseResult.CreateForField(t.Name, txt, o.ToString()));
                }
                else
                {
                    if (t == typeof(string))
                    {
                        walker.OnField(ParseResult.CreateForField(t.Name, txt, (string)o));
                    }
                    else
                    {
                       
                        walker.OnBeginContainer(ParseResult.CreateForContainer(t.Name, txt));
                        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            var dict = (IDictionary)o;
                            foreach (DictionaryEntry entry in dict)
                            {
                                parseObject(walker, new KeyValue{Key = entry.Key, Value = entry.Value }, "", dep);
                            }
                        }
                        else if (t.IsArray || t.IsGenericType && _genericDefinitions.Contains(t.GetGenericTypeDefinition()))
                        {
                            foreach (var item in (IEnumerable)o)
                            {
                                parseObject(walker, item, "", dep);
                            }
                        }
                        else
                        {
                            var properties = o.GetType().GetProperties().Where(p => p.CanRead);
                            foreach (PropertyInfo property in properties)
                            {
                                object val = property.GetValue(o, null);
                                string name = property.Name;
                                parseObject(objectWalker, val, name, dep);
                            }
                        }
                        walker.OnEndContainer();
                    }
                    
                }
            };
            
            parseObject(objectWalker, obj, null, depth);
            objectWalker.OnFinish();
        }

        
    }
}