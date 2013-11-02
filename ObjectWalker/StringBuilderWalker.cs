using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectWalker
{
    public class StringBuilderWalker : IObjectWalker
    {
        private Stack<Leaf> m_leafs = new Stack<Leaf>();
        private Leaf m_curLeaf;
        public StringBuilderWalker()
        {
            m_curLeaf = new Leaf(string.Empty);
        }

        public void WalkDown(string text)
        {
            m_leafs.Push(m_curLeaf);
            var newLeaf = new Leaf(text);
            m_curLeaf.AddLeaf(newLeaf);
            m_curLeaf = newLeaf;
        }

        public void WalkLevel(string text)
        {
            m_curLeaf.Text = text;
        }

        public void WalkUp()
        {
            m_curLeaf = m_leafs.Pop();
        }

        public StringBuilder GetBuilder()
        {
            var sb = new StringBuilder();
            m_curLeaf.DrawTree(sb);
            return sb;
        }

        class Leaf
        {
            private const string m_emptyToken = "      ";
            private const string m_branchToken = "  |   ";
            private const string m_leafToken = "  |--";

            public Leaf(string text)
            {
                Text = text;
                Leafs = new List<Leaf>();
            }

           
            List<Leaf> Leafs { get; set; }

            public string Text { get; set; }

            public bool Terminated { get; set; }

            public void AddLeaf(Leaf l)
            {
                Leafs.Add(l);
            }
            public void DrawTree(StringBuilder builder)
            {
                
            }
        }
    }
}
