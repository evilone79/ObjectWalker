using System;
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
            Configuration c = new Configuration();
            //Button c=new Button();
            ObjectUtils.FillObject(c,2);
            var sb = new StringBuilder();
            //ObjectUtils.WalkObject(c, new StringBuilderWalker(sb));
            ObjectUtils.WalkObject(c, new TreeViewWalker(treeView1));
            richTextBox1.Text = sb.ToString();
            treeView1.ExpandAll();
        }
       
    }
}
