using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ObjectUtils
{
    public class TreeViewWalker : IObjectWalker
    {
        private Stack<TreeNode> m_stack = new Stack<TreeNode>();
        private TreeNode m_curNode;
        private TreeView _tv;

        public TreeViewWalker(TreeView tv)
        {
            _tv = tv;
        }

        public void OnStart(string rootType)
        {
            m_curNode = new TreeNode(rootType);
        }

        public void OnBeginContainer(IParseItem item)
        {
            m_stack.Push(m_curNode);
            TreeNode n = new TreeNode("");
            m_curNode.Nodes.Add(n);
            m_curNode = n;
            m_curNode.Text = string.Format("{0} ({1})", item.FieldName, item.TypeName);
        }

        public void OnContainer(IParseItem item)
        {
            
        }

        public void OnField(IParseItem item)
        {
            TreeNode n = new TreeNode("");
            m_curNode.Nodes.Add(n);
            n.Text = string.IsNullOrEmpty(item.FieldName)
                ? string.Format("[{0}]", item.Value)
                : string.Format("{0} = {1}", item.FieldName, item.Value);
        }

        public void OnEndContainer()
        {
            m_curNode = m_stack.Pop();
        }

        public void OnFinish()
        {
            _tv.Nodes.Add(m_curNode.FirstNode);
        }
    }
}
