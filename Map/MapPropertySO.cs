using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Map
{

    /// <summary>
    /// 一张Map下所有Tilemap共用的Map Property数据
    /// <para>tile_properties : 以网格坐标为索引的tile属性</para>
    /// </summary>
    [CreateAssetMenu(fileName = "MapPropertySO", menuName = "Map/MapProperty")]
    public class MapPropertySO : ScriptableObject
    {
        [SceneName]
        public string scene_name;
        public Vector2Int grid_shape;//网格地图的宽高
        public Vector2Int bottom_left;//网格地图左下角的点，用于校正原点
        [SerializeField] private List<Vector2Int> cell_postions;//dict无法序列化，转为双List进行存储，运行加载后重建dict
        [SerializeField] private List<TileDetail> tile_details;

        //将字典转化为可序列化存储的双List
        public (List<Vector2Int>, List<TileDetail>) DictToList(Dictionary<Vector2Int, TileDetail> dict)
        {
            cell_postions = dict.Keys.ToList();
            tile_details = dict.Values.ToList();
            return (cell_postions, tile_details);
        }

        public Dictionary<Vector2Int, TileDetail> ListToDict()
        {
            Dictionary<Vector2Int, TileDetail> dict = new();
            for (int i = 0; i < cell_postions.Count; i++)
            {
                dict.Add(cell_postions[i], tile_details[i]);
            }
            return dict;
        }

        public static Dictionary<Vector2Int, TileDetail> ListToDict(List<Vector2Int> tile_pos, List<TileDetail> tile_detail)
        {
            Dictionary<Vector2Int, TileDetail> dict = new();
            for (int i = 0; i < tile_pos.Count; i++)
            {
                dict.Add(tile_pos[i], tile_detail[i]);
            }
            return dict;
        }
    }
}
