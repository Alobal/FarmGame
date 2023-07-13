
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AStar
{
	public class Test : Singleton<Test>
	{
		private AStar a_star;
		public Vector2Int start_pos;
		public Vector2Int end_pos;
		public Tilemap display_tilemap;
		public TileBase display_tile;

        private void Start()
        {
            a_star= GetComponent<AStar>();
        }

		public void ShowNode(Node node)
		{
            display_tilemap.SetTile((Vector3Int)node.grid_pos, display_tile);
        }

        [InspectorButton("显示路径")]
		private void Show()
		{
            display_tilemap.SetTile((Vector3Int)start_pos, display_tile);
            display_tilemap.SetTile((Vector3Int)end_pos, display_tile);

            var path_stack =a_star.BuildPath(start_pos, end_pos);
			if(display_tilemap!=null && display_tile!=null)
			{
				while(path_stack.Count>0)
				{
					Node current= path_stack.Pop();
					display_tilemap.SetTile((Vector3Int)current.grid_pos, display_tile);
				}
                
			}
		}

    } 
}
