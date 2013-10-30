using System;
using System.Collections;
using System.Reflection;

namespace ObjectWalker
{
    public class Dumper
    {
        public static void WalkObject(object obj, IObjectWalker objectWalker, int depth)
        {
            Action<IObjectWalker, object, string, bool, int> parse = null;
            objectWalker.WalkLevel(obj.GetType().Name);
            parse = (walker, o, txt, firstTime, _depth) =>
                {
                    _depth--;
                    if (!firstTime) walker.WalkDown(txt);
                    PropertyInfo[] propreties = o.GetType().GetProperties();
                    foreach (PropertyInfo property in propreties)
                    {
                        walker.WalkDown(null);
                        object val = property.GetValue(o, null);
                        if (val == null)
                        {
                            walker.WalkLevel(string.Format("{0} = null", property.Name));
                            continue;
                        }
                        var enu = val as IEnumerable;
                        if (enu != null && !(enu is string) && !(val is IDictionary))
                        {
                            walker.WalkLevel(string.Format("{0}", property.Name));
                            foreach (object o2 in enu)
                            {
                                Type et = o2.GetType();
                                if (_depth > 0 && !et.IsPrimitive && !et.IsValueType && et != typeof(string))
                                {
                                    parse(walker, o2, et.Name, false, depth);
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
                                if (_depth > 0 && !dt.IsPrimitive && !dt.IsValueType && dt != typeof(string))
                                {
                                    parse(walker, dValue, dt.Name, false, _depth);
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
                        if (_depth > 0 && !t.IsPrimitive && !t.IsValueType && t != typeof(string))
                        {
                            //walker.WalkDown(string.Format("{0}", property.Name));
                            parse(walker, val, string.Format("{0}", property.Name), false, depth);
                        }
                        else
                        {
                            walker.WalkLevel(val is string
                                                 ? string.Format("{0} = \"{1}\"", property.Name, val)
                                                 : string.Format("{0} = {1}", property.Name, val));
                        }
                        walker.WalkUp();
                    }
                    if (!firstTime) walker.WalkUp();
                };

            parse(objectWalker, obj, null, true, depth);
        }


    }
}