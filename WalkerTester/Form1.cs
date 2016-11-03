using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ObjectUtils;


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
            ObjectGenerator.FillObject(c, 2);

            TheTester nc = new TheTester("aaa", DateTime.Now, SomeeNum.EnumValue2);
            ObjectGenerator.FillObject(nc, 2);
            
            var sb = new StringBuilder();
            ObjectWalker.WalkObject(nc, new StringBuilderWalker(sb), 6);
            richTextBox1.Text = sb.ToString();

            ObjectWalker.WalkObject(nc, new TreeViewWalker(treeView1), 5);
            treeView1.ExpandAll();

            richTextBox2.Text = SerializeToXml(c);
        }
       
        string SerializeToXml(Configuration c)
        {
            //XDocument xdoc = new XDocument();
            //ObjectGenerator.WalkObject(c, new XmlWalker(xdoc));
            //using (var writer=new StringWriter())
            //{
            //    xdoc.Save(writer);
            //    return writer.ToString();
            //}
            return "";
        }
    }
}
