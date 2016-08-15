using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace ObjectWalker
{
    public class ParseResult : IParseItem
    {
        public ParseResult(string typeName, string fieldName, string value)
        {
            TypeName = typeName;
            FieldName = fieldName;
            Value = value;
        }

        public string TypeName { get; }
        public string FieldName { get; }
        public string Value { get; }
    }

    public class ObjectWalker
    {
        public static void WalkObject(object obj, IObjectWalker objectWalker)
        {
            WalkObject(obj, objectWalker, Int32.MaxValue);
        }

        public static void WalkObject(object obj, IObjectWalker objectWalker, int depth)
        {
            Action<IObjectWalker, object, string, int> parse = null;
            objectWalker.OnStart();
            objectWalker.WalkLevel(new ParseResult(obj.GetType().Name, "", ""));
            parse = (walker, o, txt, dep) =>
            {
                if (dep > 0)
                {
                    dep--;
                    var properties = o.GetType().GetProperties().Where(p => p.CanRead);
                    foreach (PropertyInfo property in properties)
                    {
                        walker.StepDown();
                        object val = property.GetValue(o, null);
                        string ifprimitive = null;
                        if (property.PropertyType == typeof(string))
                        {
                            ifprimitive = string.Format("{0} = \"{1}\"", property.Name, val == null ? "" : val.ToString().Trim());
                        }
                        else
                        {
                            ifprimitive = string.Format("{0} = {1}", property.Name, val);
                        }
                        ParseObject(val, property.Name, parse, walker, ifprimitive, dep);
                        walker.StepUp();
                    }
                }
            };
            parse(objectWalker, obj, null, depth);
            objectWalker.OnFinish();
        }

        static void ParseObject(object o, string name, Action<IObjectWalker, object, string, int> parse, IObjectWalker walker, string ifPrimitive, int depth)
        {
            if (o == null)
            {
                walker.WalkLevel(new ParseResult("", name, "null"));
                return;
            }
            var enu = o as IEnumerable;
            if (enu != null && !(enu is string) && !(o is IDictionary))
            {
                walker.WalkLevel(new ParseResult(o.GetType().Name, name, ""));
                foreach (object o2 in enu)
                {
                    Type et = o2.GetType();
                    walker.StepDown();
                    ParseObject(o2, et.Name, parse, walker, string.Concat("[", o2.ToString(), "]"), depth);
                    walker.StepUp();
                }
                return;
            }
            var dict = o as IDictionary;
            if (dict != null)
            {
                walker.WalkLevel(name);
                foreach (var key in dict.Keys)
                {
                    var dValue = dict[key];
                    Type dt = dValue.GetType();
                    walker.StepDown();
                    ParseObject(dValue, dt.Name, parse, walker, string.Concat("{", key, " : ", dValue.ToString(), "}"), depth);
                    walker.StepUp();
                }
                return;
            }
            Type t = o.GetType();
            if (!t.IsPrimitive && !t.IsValueType && t != typeof(string))
            {
                walker.WalkLevel(name);
                parse(walker, o, null, depth);
            }
            else
            {
                walker.WalkLevel(ifPrimitive);
            }
        }
    }
}