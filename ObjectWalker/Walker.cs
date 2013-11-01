using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
                        string ifprimitive = o is string
                                                 ? string.Format("{0} = \"{1}\"", property.Name, val)
                                                 : string.Format("{0} = {1}", property.Name, val);
                        ParseObject(val, property.Name, parse, walker, ifprimitive);
                        walker.WalkUp();
                    }
                };
            parse(objectWalker, obj, null);
        }

        static void ParseObject(object o, string name, Action<IObjectWalker, object, string> parse, IObjectWalker walker, string ifPrimitive)
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
                    ParseObject(o2, et.Name, parse, walker, string.Concat("[", o2.ToString(), "]"));
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
                    ParseObject(dValue, dt.Name, parse, walker, string.Concat("{", key, " : ", dValue.ToString(), "}"));
                    walker.WalkUp();
                }
                return;
            }
            Type t = o.GetType();
            if (!t.IsPrimitive && !t.IsValueType && t != typeof(string))
            {
                walker.WalkLevel(name);
                parse(walker, o, null);
            }
            else
            {
                walker.WalkLevel(ifPrimitive);
            }
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
                    if (t == o.GetType())
                    {
                        continue;
                    }
                    
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
                    string s = sb.Append('_').Append(new Random().Next(20)).ToString().ToLower();
                        
                    object propVal = CreateObject(t, s, fillObject, collectionItemsCount);
                    prop.SetValue(o, propVal, null);
                }
            };
            fillObject(obj);
        }

        static object CreateObject(Type t, string forString, Action<object> fill, int collCnt)
        {
            if (t.IsPrimitive)
            {
                if (t == typeof(Boolean))
                {
                    return true;
                }
                return new Random().Next(20);
            }
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            if (t.IsClass)
            {
                if (t == typeof(string))
                {
                    return forString;
                }
                if (t.GetInterfaces().Contains(typeof(IDictionary)))
                {
                    Type keyType = t.GetGenericArguments()[0];
                    Type valType = t.GetGenericArguments()[1];
                    var constructedDict = t.GetGenericTypeDefinition().MakeGenericType(keyType, valType);
                    var dict = (IDictionary)Activator.CreateInstance(constructedDict);
                    for (int i = 0; i < collCnt; i++)
                    {
                        object key;
                        if (keyType.IsPrimitive)
                        {
                            key = i;
                        }
                        else if (keyType == typeof(string))
                        {
                            key = "key_" + i;
                        }
                        else continue;

                        object value = CreateObject(valType, "value_" + i, fill, collCnt);
                        dict.Add(key, value);
                    }
                    return dict;
                }
                if (t.GetInterfaces().Contains(typeof(IEnumerable)) && t != typeof(string))
                {
                    if (t.IsArray)
                    {
                        Type itemType = t.GetElementType();
                        Array arr = Array.CreateInstance(itemType, 3);
                        for (int i = 0; i < collCnt; i++)
                        {
                            var arrItem = CreateObject(itemType, "array_item_" + i, fill, collCnt);
                            arr.SetValue(arrItem, i);
                        }
                        return arr;
                    }
                    else
                    {
                        Type itemType = t.GetGenericArguments()[0];
                        Type genType = t.GetGenericTypeDefinition();
                        var genericCollType = genType.MakeGenericType(itemType);
                        var collection = Activator.CreateInstance(genericCollType);
                        for (int i = 0; i < collCnt; i++)
                        {
                            var listItem = CreateObject(itemType, "list_item_" + i, fill, collCnt);
                            if (genType == typeof(LinkedList<>))
                            {
                                t.GetMethod("AddLast", new[] { itemType }).Invoke(collection, new[] { listItem });
                            }
                            else
                            {
                                t.GetMethod("Add").Invoke(collection, new[] { listItem });
                            }
                        }
                        return collection;
                    }
                }
                //else treat as non collection object
                object val = Activator.CreateInstance(t);
                fill(val);
                return val;
            }
            return null;
        }
    }
}