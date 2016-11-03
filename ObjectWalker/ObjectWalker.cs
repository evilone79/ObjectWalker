using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace ObjectUtils
{
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
            objectWalker.WalkLevel(obj.GetType().Name);
            parse = (walker, o, txt, dep) =>
            {
                if (dep > 0)
                {
                    dep--;
                    var properties = o.GetType().GetProperties().Where(p => p.CanRead);
                    foreach (PropertyInfo property in properties)
                    {
                        walker.WalkDown(null);
                        object val = property.GetValue(o, null);
                        string ifprimitive = property.PropertyType == typeof(string)
                            ? string.Format("{0} = \"{1}\"", property.Name, val==null?"":((string)val).Trim())
                            : string.Format("{0} = {1}", property.Name, val);
                        ParseObject(val, property.Name, parse, walker, ifprimitive, dep);
                        walker.WalkUp();
                    }
                }
            };
            parse(objectWalker, obj, null, depth);
            objectWalker.OnFinish();
        }

        private static void ParseObject(object o, string name, Action<IObjectWalker, object, string, int> parse, IObjectWalker walker, string ifPrimitive, int depth)
        {
            if (o == null)
            {
                walker.WalkLevel(string.Format("{0} = null", name));
                return;
            }
            var enu = o as IEnumerable;
            if (enu != null && !(enu is string) && !(o is IDictionary))
            {
                walker.WalkLevel(name);
                foreach (object o2 in enu)
                {
                    Type et = o2.GetType();
                    walker.WalkDown(null);
                    ParseObject(o2, et.Name, parse, walker, string.Concat("[", o2.ToString(), "]"),depth);
                    walker.WalkUp();
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
                    walker.WalkDown(null);
                    ParseObject(dValue, dt.Name, parse, walker, string.Concat("{", key, " : ", dValue.ToString(), "}"),depth);
                    walker.WalkUp();
                }
                return;
            }
            Type t = o.GetType();
            if (!t.IsPrimitive && !t.IsValueType && t != typeof(string))
            {
                walker.WalkLevel(name);
                parse(walker, o, null,depth);
            }
            else
            {
                walker.WalkLevel(ifPrimitive);
            }
        }
    }
}