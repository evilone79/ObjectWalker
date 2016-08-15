using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ObjectWalker
{
    //public class XmlWalker : IObjectWalker
    //{
    //    private XDocument m_xml;
    //    private Stack<XElement> m_stack;
    //    private XElement m_curNode;
 
    //    public XmlWalker(XDocument xml)
    //    {
    //        m_xml = xml;
    //        m_curNode = new XElement("");
    //        m_xml.Add(m_curNode);
    //    }

    //    public void OnStart()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void StepDown()
    //    {
    //        m_stack.Push(m_curNode);
    //        var newNode = new XElement(text);
    //        m_curNode.Add(newNode);
    //        m_curNode = newNode;
    //    }

    //    public void WalkLevel(string text)
    //    {
    //        m_curNode.Value = text;
    //    }

    //    public void StepUp()
    //    {
    //        m_curNode = m_stack.Pop();
    //    }

    //    public void OnFinish()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
