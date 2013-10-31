using System;
using System.Collections;
using System.Reflection;

namespace ObjectWalker
{
    public class Dumper
    {
        public static void WalkObject(object obj, IObjectWalker objectWalker)
        {
            Action<IObjectWalker, object, string> parse = null;
            objectWalker.WalkLevel(obj.GetType().Name);
            parse = (walker, o, txt) =>
                {
                    PropertyInfo[] propreties = o.GetType().GetProperties();
                    foreach (PropertyInfo property in propreties)
                    {
                        walker.WalkDown(null);
                        object val = property.GetValue(o, null);
                        if (val == null)
                        {
                            walker.WalkLevel(string.Format("{0} = null", property.Name));
                            walker.WalkUp();
                            continue;
                        }
                        var enu = val as IEnumerable;
                        if (enu != null && !(enu is string) && !(val is IDictionary))
                        {
                            walker.WalkLevel(string.Format("{0}", property.Name));
                            foreach (object o2 in enu)
                            {
                                Type et = o2.GetType();
                                if (!et.IsPrimitive && !et.IsValueType && et != typeof(string))
                                {
                                    walker.WalkDown(et.Name);
                                    parse(walker, o2, et.Name);
                                    walker.WalkUp();
                                }
                                else
                                {
                                    walker.WalkDown(string.Concat("[", o2.ToString(), "]"));
                                    walker.WalkUp();
                                }
                            }
                            walker.WalkUp();
                            continue;
                        }
                        var dict = val as IDictionary;
                        if (dict != null)
                        {
                            walker.WalkLevel(string.Format("{0}", property.Name));
                            foreach (var key in dict.Keys)
                            {
                                var dValue = dict[key];
                                Type dt = dValue.GetType();
                                if (!dt.IsPrimitive && !dt.IsValueType && dt != typeof(string))
                                {
                                    walker.WalkDown(dt.Name);
                                    parse(walker, dValue, dt.Name);
                                    walker.WalkUp();
                                }
                                else
                                {
                                    walker.WalkDown(string.Concat("{", key, " : ", dValue.ToString(), "}"));
                                    walker.WalkUp();
                                }
                            }
                            walker.WalkUp();
                            continue;
                        }
                        Type t = property.PropertyType;
                        if (!t.IsPrimitive && !t.IsValueType && t != typeof(string))
                        {
                            walker.WalkLevel(string.Format("{0}", property.Name));
                            parse(walker, val, null);
                        }
                        else
                        {
                            walker.WalkLevel(val is string
                                                 ? string.Format("{0} = \"{1}\"", property.Name, val)
                                                 : string.Format("{0} = {1}", property.Name, val));
                        }
                        walker.WalkUp();
                    }
                };

            parse(objectWalker, obj, null);
        }


    }
}