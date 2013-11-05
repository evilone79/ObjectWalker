using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ObjectWalker
{
    public class TreeViewWalker : TextWalker
    {
        private readonly Stack<TreeNode> m_stack = new Stack<TreeNode>();
        private TreeNode m_curNode;

        public TreeViewWalker(TreeView tv) : base(new ToTextFormatter())
        {
            m_curNode = new TreeNode();
            tv.Nodes.Add(m_curNode);
        }

        public override void OnStart()
        {
        }

        public override void WalkDown(string text)
        {
            m_stack.Push(m_curNode);
            TreeNode n = new TreeNode(text);
            m_curNode.Nodes.Add(n);
            m_curNode = n;
        }

        public override void WalkLevel(string text)
        {
            m_curNode.Text = text;
        }

        public override void WalkUp()
        {
            m_curNode = m_stack.Pop();
        }

        public override void OnFinish()
        {
        }
    }
}
