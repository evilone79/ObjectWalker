using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

            var obj = CreateObject();
            Dumper.WalkObject(obj,new TreeViewWalker(treeView1));
            treeView1.ExpandAll();
            var sb = new StringBuilder();
            Dumper.WalkObject(button1, new StringBuilderWalker(sb));
            richTextBox1.Text = sb.ToString();
        }

        Configuration CreateObject()
        {
            Configuration obj = new Configuration();
            var ent1 = new Entity { AnotherStuff = "gbsdfghsdf", Date = DateTime.Now, Result = false };
            ent1.Dict.Add(1, "zxdfsdg");
            ent1.DictEnt.Add(2, new Entity { AnotherStuff = "sss", Date = DateTime.Now, Result = false });
            var ent2 = new Entity { AnotherStuff = "fghdfgjh", Date = DateTime.Now, Result = true };
            ent2.Dict.Add(34, "46785678tyui");
            obj.DefaultInstallerParams = new List<Entity>() { ent1, ent2 };
            obj.Listing = new List<string>() { "asdfasdf", "dfgsdfgh" };
            obj.ExitValueLocation = "fasdfasdfgasdfgsdfgs";
            return obj;
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
