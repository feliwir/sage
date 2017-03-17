using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{

    struct Node
    {
        int m_probability;
        int m_left;
        int m_right;
        short m_symbol;

        public int Probability { get => m_probability; set => m_probability = value; }
        public short Symbol { get => m_symbol; set => m_symbol = value; }
        public int Left { get => m_left; set => m_left = value; }
        public int Right { get => m_right; set => m_right = value; }
    }

    class NodeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            Node a = (Node)x;
            Node b = (Node)y;
            return (a.Probability - b.Probability) * 16 + (b.Symbol - a.Symbol);
        }
    }

    class Huffman
    {

        public static readonly int MAX_SIZE = 12;
        private Node[] m_nodes;

        public Huffman(byte[] model, byte[] map,int size)
        {
            int a, b;
            m_nodes = new Node[2 * MAX_SIZE];
            //Convert to Huffman Probabilities
            m_nodes[12].Probability = 256;
            for (int i = 0; i < size-1; i++)
            {
                a = m_nodes[size + i].Probability * model[i] >> 8;
                b = m_nodes[size + i].Probability * (255- model[i]) >> 8;
                m_nodes[map[2 * i]].Probability = a + ((a==0) ?  1:0);
                m_nodes[map[2 * i+1]].Probability = b + ((b == 0) ? 1 : 0);
            }

            int sum = 0;

            for (short i = 0; i < size; i++)
            {
                m_nodes[i].Symbol = i;
                sum += m_nodes[i].Probability;
            }

            Array.Sort(m_nodes,0,size,new NodeComparer());
            m_nodes[size * 2 - 1].Probability = 0;
        }

        internal Node[] Nodes { get => m_nodes; set => m_nodes = value; }
    }
}
