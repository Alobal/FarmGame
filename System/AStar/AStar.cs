using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Map;

namespace AStar
{
    public class AStar : MonoBehaviour
    {
        private Grid grid;
        private List<Node> open_nodes;
        private HashSet<Node> close_nodes;
        private bool path_found;
        private Vector2Int[] neighbor_dir = { new Vector2Int(0, 1), new Vector2Int(0, -1), 
                                              new Vector2Int(1, 0), new Vector2Int(-1, 0), };
        private const int unit_cost = 10;

        private void Start()
        {
            Init();
        }
        private void Init()
        {
            var (grid_shape,grid_bottomleft) =TilemapManager.instance.GetGridDimensions();
            grid = new Grid(grid_shape.x,grid_shape.y,grid_bottomleft);
            open_nodes = new List<Node>();
            close_nodes = new HashSet<Node>();
        }

        public Stack<Node> BuildPath(Vector2Int start_pos, Vector2Int end_pos)
        {
            path_found = false;
            Node start_node = grid[start_pos];
            Node end_node = grid[end_pos];
            start_node.to_cost = unit_cost * UtilityMethods.ManhattanDist(end_node.grid_pos, start_node.grid_pos);
            open_nodes.Add(start_node);
            //构建路径
            while(open_nodes.Count>0)
            {   
                //选取最小cost的可行点
                open_nodes.Sort();
                Node current_node = open_nodes[0];
                open_nodes.RemoveAt(0);
                //添加路径节点
                close_nodes.Add(current_node);
                if(current_node==end_node)
                {
                    path_found=true;
                    break;
                }
                else//添加邻居点作为路径搜索 
                {
                    for(int i=0;i<neighbor_dir.Length;i++)
                    {
                        if (grid[current_node.grid_pos+neighbor_dir[i]] is Node neighbor)//null则false
                        {
                            TryAddOpenNode(neighbor, current_node, end_node);
                        }
                    }
                }
            }
            open_nodes.Clear();
            close_nodes.Clear();

            if(path_found)//从end节点寻找parent反向构建出path
            {
                Stack<Node> path_stack = new Stack<Node>();
                Node current = end_node;
                while(current != start_node)
                {
                    path_stack.Push(current);
                    current = current.parent;
                }
                return path_stack;
            }
            
            return null;

        }


        private bool TryAddOpenNode(Node new_node,Node parent_node,Node end_node)
        {
            if (new_node.is_obstacle || close_nodes.Contains(new_node) || open_nodes.Contains(new_node))
                return false;
            //计算cost并加入列表
            new_node.from_cost = parent_node.from_cost + unit_cost;
            new_node.to_cost = unit_cost * UtilityMethods.ManhattanDist(end_node.grid_pos,new_node.grid_pos);
            new_node.parent= parent_node;
            open_nodes.Add(new_node);

            return true;
        }
    }
}