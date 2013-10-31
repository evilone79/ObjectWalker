using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectWalker
{
    public class Walker
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

        public static void FillObject(object obj, int collectionItemsCount)
        {
            Action<object> fillObject = null;
            fillObject = o =>
            {
                var props = o.GetType().GetProperties();
                foreach (var prop in props)
                {
                    Type t = prop.PropertyType;
                    if (t.GetInterfaces().Contains(typeof(IList)))
                    {
                        if (t.IsArray)
                        {
                            Type itemType = t.GetElementType();
                            Array arr = Array.CreateInstance(itemType, 3);
                            for (int i = 0; i < collectionItemsCount; i++)
                            {
                                if (itemType.IsClass)
                                {
                                    if (itemType == typeof(string))
                                    {
                                        arr.SetValue(("array_item_" + i),i);
                                    }
                                    else
                                    {
                                        if (itemType != o.GetType())
                                        {
                                            object val = Activator.CreateInstance(itemType);
                                            fillObject(val);
                                            arr.SetValue(val, i); 
                                        }
                                    }
                                }
                            }
                            prop.SetValue(o, arr, null);
                        }
                        else
                        {
                            Type itemType = t.GetGenericArguments()[0];
                            var constructedListType = typeof(List<>).MakeGenericType(itemType);
                            var list = (IList)Activator.CreateInstance(constructedListType);
                            for (int i = 0; i < collectionItemsCount; i++)
                            {
                                if (itemType.IsClass)
                                {
                                    if (itemType == typeof(string))
                                    {
                                        list.Add("list_item_" + i);
                                    }
                                    else
                                    {
                                        if (itemType != o.GetType())
                                        {
                                            object val = Activator.CreateInstance(itemType);
                                            fillObject(val);
                                            list.Add(val); 
                                        }
                                    }
                                }
                            }
                            prop.SetValue(o, list, null);
                        }
                        continue;
                    }
                    if (t.GetInterfaces().Contains(typeof(IDictionary)))
                    {
                        Type keyType = t.GetGenericArguments()[0];
                        Type valType = t.GetGenericArguments()[1];
                        var constructedDict = typeof (Dictionary<,>).MakeGenericType(keyType, valType);
                        var dict = (IDictionary) Activator.CreateInstance(constructedDict);
                        for (int i = 0; i < collectionItemsCount; i++)
                        {
                            object key;
                            object value = null;
                            if (keyType.IsPrimitive)
                            {
                                key = i;
                            }
                            else if (keyType == typeof(string))
                            {
                                key = "key_" + i;
                            }
                            else continue;

                            if (valType.IsClass)
                            {
                                if (valType == typeof(string))
                                {
                                    value = ("value_" + i);
                                }
                                else
                                {
                                    if (valType != o.GetType())
                                    {
                                        value = Activator.CreateInstance(valType);
                                        fillObject(value); 
                                    }
                                }
                            }
                            dict.Add(key, value);
                        }
                        prop.SetValue(o, dict, null);
                        continue;
                    }
                    if (t.IsPrimitive)
                    {
                        if (t == typeof(Boolean))
                        {
                            prop.SetValue(o, true, null);
                        }
                        else
                        {
                            prop.SetValue(o, new Random().Next(20), null);
                        }
                        continue;
                    }
                    if (t.IsClass)
                    {
                        if (t == typeof(string))
                        {
                            var sb = new StringBuilder();
                            char[] chars = prop.Name.ToCharArray();
                            sb.Append(chars[0]);
                            for (int i = 1; i < chars.Length; i++)
                            {
                                char c = chars[i];
                                if (Char.IsUpper(c))
                                {
                                    sb.Append('_');
                                }
                                sb.Append(c);
                            }
                            sb.Append('_');
                            prop.SetValue(o, sb.ToString() + new Random().Next(20), null);
                        }
                        else
                        {
                            object val = Activator.CreateInstance(t);
                            fillObject(val);
                            prop.SetValue(o, val, null);
                        }
                        continue;
                    }
                    if (t.IsValueType)
                    {
                        prop.SetValue(o, Activator.CreateInstance(t), null);	
                    }
                }
            };
            fillObject(obj);

        }

    }
}