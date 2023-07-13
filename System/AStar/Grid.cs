using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class Grid
    {
        private int width;
        private int height;
        private Vector2Int node_offset;//索引偏移量，负数，untiy grid原点在中心，数组不能有负索引，因此进行偏移。
        private Node[,] nodes;

        public Grid(int width, int height,Vector2Int origin_offset)
        {
            this.width = width;
            this.height = height;
            this.node_offset = origin_offset;
            nodes = new Node[width, height];
            for(int i=0;i<width; i++)
            {
                for(int j=0;j<height;j++)
                {
                    nodes[i,j]=new Node(new Vector2Int(i,j)+node_offset);
                }
            }
        }

        public Node this[int pos_x,int pos_y]
        {
            get
            {
                pos_x -= node_offset.x;
                pos_y -= node_offset.y;
                if (pos_x < width && pos_x>=0  && pos_y < height && pos_y>=0)
                    return nodes[pos_x, pos_y];
                else
                {
                    Debug.Log("超出网格范围");
                    return null;
                }
            }
        }

        public Node this[Vector2Int pos]
        {
            get { return this[pos.x,pos.y]; }
        }
    }
}
