using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
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
            
            ObjectGenerator.FillObject(c,2);
            
            var walker = new StringBuilderWalker();
            ObjectGenerator.WalkObject(c, walker);
            richTextBox1.Text = walker.GetBuilder().ToString();

            ObjectGenerator.WalkObject(c, new TreeViewWalker(treeView1));
            treeView1.ExpandAll();

            richTextBox2.Text = SerializeToXml(c);
        }
       
        string SerializeToXml(Configuration c)
        {
            XDocument xdoc = new XDocument();
            ObjectGenerator.WalkObject(c, new XmlWalker(xdoc));
            using (var writer=new StringWriter())
            {
                xdoc.Save(writer);
                return writer.ToString();
            }
            
        }
    }
}
