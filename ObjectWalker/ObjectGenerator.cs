using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ObjectUtils
{
    

    public class ObjectGenerator
    {
        private static readonly List<string> m_fillExcludedAssemblies = new List<string>
        {
            "mscorlib",
            "System",
            "Microsoft"
        };

        
        public static void FillObject(object obj, int collectionItemsCount)
        {
            Action<object> fillObject = null;
            fillObject = o =>
            {
                var props = o.GetType().GetProperties().Where(p=>p.CanWrite);
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
                        if (char.IsUpper(c))
                        {
                            sb.Append('_');
                        }
                        sb.Append(c);
                    }
                    string s = sb.Append('_').Append(new Random().Next(20)).ToString().ToLower();
                        
                    object propVal = CreateObject(t, s, fillObject, collectionItemsCount);
                    try
                    {
                        prop.SetValue(o, propVal, null);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
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
            if (t.IsEnum)
            {
                return t.GetEnumValues().GetValue(0);
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
                        Array arr = Array.CreateInstance(itemType, collCnt);
                        for (int i = 0; i < collCnt; i++)
                        {
                            var arrItem = CreateObject(itemType, "array_item_" + i, fill, collCnt);
                            arr.SetValue(Convert.ChangeType(arrItem, itemType), i);
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
                object val = null;
                if (!t.Assembly.FullName.Split(',','.').Intersect(m_fillExcludedAssemblies).Any())
                {
                    if (t.GetConstructors().Count(c => c.GetParameters().Length == 0) > 0)
                    {
                        val = Activator.CreateInstance(t);
                        fill(val);
                    }
                    else
                    {
                        var constructors = t.GetConstructors();
                        if (constructors.Length > 0)
                        {
                            var parameters = constructors[0].GetParameters();
                            var objects =
                                parameters.Select(pi => CreateObject(pi.ParameterType, "from_constructor", fill, collCnt)).ToArray();
                            val = Activator.CreateInstance(t, objects);
                            fill(val);
                        }
                    }
                }
                return val;
            }
            return null;
        }
    }
}