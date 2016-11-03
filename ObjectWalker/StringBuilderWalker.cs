using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectUtils
{
    public class StringBuilderWalker : IObjectWalker
    {
        private Stack<Leaf> m_leafs = new Stack<Leaf>();
        private StringBuilder m_builder;
        private Leaf m_curLeaf;
        public StringBuilderWalker(StringBuilder builder)
        {
            m_curLeaf = new Leaf(string.Empty);
            m_builder = builder;
        }

        public void OnStart(string rootType)
        {
            m_curLeaf.Text = rootType;
        }

        public void OnBeginContainer(IParseItem item)
        {
            m_leafs.Push(m_curLeaf);
            var newLeaf = new Leaf("");
            m_curLeaf.AddLeaf(newLeaf);
            m_curLeaf = newLeaf;
            m_curLeaf.Text = string.Format("{0} ({1})", item.FieldName, item.TypeName);
        }

        
        public void OnField(IParseItem item)
        {
            var leaf = new Leaf("");
            m_curLeaf.AddLeaf(leaf);
            leaf.Text = string.IsNullOrEmpty(item.FieldName)
                ? string.Format("[{0}]", item.Value)
                : string.Format("{0} = {1}", item.FieldName, item.Value);
        }


        public void OnEndContainer()
        {
            m_curLeaf = m_leafs.Pop();
        }

        public void OnFinish()
        {
            m_curLeaf = m_curLeaf.Leafs.First();
            m_curLeaf.DrawTree(m_builder, new List<string>(), true);
            //m_builder.Remove(0, 2);
        }

        class Leaf
        {
            private const string EMPTY_TOKEN = "      ";
            private const string BRANCH_TOKEN = "    |   ";
            private const string LEAF_TOKEN = "    |__";

            public Leaf(string text)
            {
                Text = text;
                Leafs = new List<Leaf>();
            }

            public List<Leaf> Leafs { get; set; }
            public string Text { get; set; }
            public void AddLeaf(Leaf l)
            {
                Leafs.Add(l);
            }

           
            public void DrawTree(StringBuilder builder, List<string> prefix, bool isLast)
            {
                builder.Append(string.Concat(prefix)).Append(LEAF_TOKEN).AppendLine(Text);
                int cnt = Leafs.Count;
                if (isLast)
                {
                    prefix.Add(EMPTY_TOKEN);
                }
                else
                {
                    prefix.Add(BRANCH_TOKEN);
                }
                foreach (var leaf in Leafs)
                {
                    if (--cnt == 0)
                    {
                        leaf.DrawTree(builder, prefix,true);
                    }
                    else
                    {
                        leaf.DrawTree(builder, prefix, false);
                    }
                }
                prefix.RemoveAt(prefix.Count - 1);
            }
        }
    }
}
