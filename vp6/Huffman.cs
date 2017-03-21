using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{

    class Node
    {
        int m_probability;
        Node m_left;
        Node m_right;
        int m_symbol;

        public int Probability { get => m_probability; set => m_probability = value; }
        public int Symbol { get => m_symbol; set => m_symbol = value; }
        public Node Left { get => m_left; set => m_left = value; }
        public Node Right { get => m_right; set => m_right = value; }
    }

    class NodeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            Node a = (Node)x;
            Node b = (Node)y;
            return (a.Probability - b.Probability)*16+(b.Symbol-a.Symbol);
        }
    }

    class Huffman
    {
        private int m_bits;
        private Node m_root;

        public Huffman(byte[] model, byte[] map,int size)
        {
            int a, b;
            m_bits = (int)Math.Log((double)size, 2.0) +1;
            Node[] nodes = new Node[2 * size];

            for (int i = 0; i < nodes.Length; ++i)
                nodes[i] = new Node();

            //Convert to Huffman Probabilities
            nodes[size].Probability = 256;

            for (int i = 0; i < size-1; i++)
            {
                a = nodes[size + i].Probability * model[i] >> 8;
                b = nodes[size + i].Probability * (255- model[i]) >> 8;
                nodes[map[2 * i]].Probability = a + ((a==0) ?  1:0);
                nodes[map[2 * i+1]].Probability = b + ((b == 0) ? 1 : 0);
                nodes[i].Symbol = i;
            }

            nodes[size-1].Symbol = size-1;
            //Sort depending on probabilities
            Array.Sort(nodes, 0,size,new NodeComparer());

            //Always pack 2 together
            int cur_node = size,j;
            nodes[size * 2 - 1].Probability = 0;

            for (int i = 0; i < size * 2 - 1; i += 2)
            {
                int cur_count = nodes[i].Probability + nodes[i + 1].Probability;
                // find correct place to insert new node, and
                // make space for the new node while at it
                for (j = cur_node; j > i + 2; j--)
                {
                    if (cur_count > nodes[j - 1].Probability)
                        break;
                    nodes[j] = nodes[j - 1];
                }
                nodes[j].Symbol = -1;
                nodes[j].Probability = cur_count;
                cur_node++;
            }

            //Right here my nodes list is somehow different from the list in ffmpeg
        }

        //Read the according element
        public int DecodeSymbol(BitReader br)
        {
            return 0;
        }
    }   
}
