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
