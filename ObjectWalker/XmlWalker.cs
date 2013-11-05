using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ObjectWalker
{
    public class XmlWalker : IObjectWalker
    {
        private XDocument m_xml;
        private Stack<XElement> m_stack = new Stack<XElement>();
        private XElement m_curNode;

        public XmlWalker(XDocument xml)
        {
            m_xml = xml;
        }

        public void OnStart()
        {
            m_curNode = new XElement("_dummy");
            m_xml.Add(m_curNode);
        }

        public void WalkDown(string text)
        {
            m_stack.Push(m_curNode);
            var newNode = new XElement("_dummy");
            m_curNode.Add(newNode);
            m_curNode = newNode;
        }

        public void WalkLevel(string name, string value, ItemType type)
        {
            value = value != null ? value.Trim('"') : null;
            switch (type)
            {
                case ItemType.Collection:
                    m_curNode.Name = "item";
                    m_curNode.Add(new XAttribute("value", value));
                    break;
                case ItemType.Dictionary:
                    m_curNode.Name = "item";
                    m_curNode.Add(new XAttribute("key", name));
                    m_curNode.Add(new XAttribute("value", value));
                    break;
                case ItemType.NameValue:
                    m_curNode.Name = name;
                    m_curNode.Add(new XAttribute("value", value));
                    //m_curNode.Value = value;
                    break;
                default:
                    m_curNode.Name = name;
                    break;
            }
        }


        public void WalkUp()
        {
            m_curNode = m_stack.Pop();
        }

        public void OnFinish()
        {
        }
    }
}