using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectWalker
{
    public class TextFormatter : IItemFormatter
    {
        public string GetItemText(string name, string value, ItemType type)
        {
            switch (type)
            {
                case ItemType.Collection:
                    return string.Concat("[", value, "]");
                case ItemType.Dictionary:
                    return string.Concat("{", name, " : ", value, "}");
                case ItemType.NameValue:
                    return string.Concat(name, " = ", value);
                default:
                    return name;
            }
        }
    }
}