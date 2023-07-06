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
        [SerializeField] private List<Vector2Int> cell_postions;//dict无法序列化，转为双List进行存储，运行加载后重建dict
        [SerializeField] private List<TileDetail> tile_details;

        //将字典转化为可序列化存储的双List
        public void DictToList(Dictionary<Vector2Int, TileDetail> dict)
        {
            cell_postions = dict.Keys.ToList();
            tile_details = dict.Values.ToList();
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
    }
}
