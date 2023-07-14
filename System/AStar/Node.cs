using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
	public class Node : IComparable<Node>
	{
		public Vector2Int grid_pos;//中心点为原点
		public Vector3 world_pos { get => TilemapManager.instance.grid.CellToWorld((Vector3Int)grid_pos) + new Vector3(0.5f, 0.5f); }
		public int from_cost = 0;//起点到此的csot
		public int to_cost = 0;//此处到终点的cost
		public int cost => from_cost + to_cost;//最终cost
		public bool is_obstacle = false;
		public Node parent;
		public Node(Vector2Int pos)
		{
			grid_pos = pos;
			parent = null;
			var tile=TilemapManager.instance.GetTileDetail(grid_pos);
			if(tile!= null && tile.CheckProperty(TileFlag.NPCObstacle))
				is_obstacle=true;
		}

        public int CompareTo(Node other)
        {
            int result=cost.CompareTo(other.cost);
			if(result==0)
				result=to_cost.CompareTo(other.to_cost);
			return result;
        }
    } 
}
