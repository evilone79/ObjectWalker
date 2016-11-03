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

        public TreeViewWalker(TreeView tv)
        {
            m_curNode = new TreeNode();
            tv.Nodes.Add(m_curNode);
        }

        public void OnStart()
        {
        }

        public void WalkDown(string text)
        {
            m_stack.Push(m_curNode);
            TreeNode n = new TreeNode(text);
            m_curNode.Nodes.Add(n);
            m_curNode = n;
        }

        public void WalkLevel(string text)
        {
            m_curNode.Text = text;
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
