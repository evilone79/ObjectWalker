using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ObjectWalker;

namespace WalkerTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuration c = new Configuration();
            //Button c=new Button();
            Walker.FillObject(c,2);
            var sb = new StringBuilder();
            Walker.WalkObject(label1, new StringBuilderWalker(sb));
            Walker.WalkObject(c, new TreeViewWalker(treeView1));
            richTextBox1.Text = sb.ToString();
        }
       
    }

    public class StringBuilderWalker : IObjectWalker
    {
        private StringBuilder m_sb;
        private int m_depth = 0;
        public StringBuilderWalker(StringBuilder sb)
        {
            m_sb = sb;
        }

        public void WalkDown(string text)
        {
            m_depth++;
            if (!string.IsNullOrEmpty(text))
            {
                m_sb.AppendLine(GetTabs() + text);
            }
        }

        public void WalkLevel(string text)
        {
            m_sb.AppendLine(GetTabs() + text);
        }

        public void WalkUp()
        {
            m_depth--;
        }

        string GetTabs()
        {
            return new string(' ', m_depth*6);
        }
    }


    public class TreeViewWalker : IObjectWalker
    {
        private Stack<TreeNode> m_stack = new Stack<TreeNode>();
        private TreeNode m_curNode;

        public TreeViewWalker(TreeView tv)
        {
            m_curNode = new TreeNode();
            tv.Nodes.Add(m_curNode);
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
    }
}
