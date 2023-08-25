using Crop;
using Save;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{

    /// <summary>
    /// 管理所有具有特殊功能(Flag)的tile,所有FlagTilemap的管理器，存储于SO文件。Enbale加载，Disable更新保存。
    /// </summary>
    public class TilemapManager : Singleton<TilemapManager>, Save.ISavable
    {

        public MapPropertySO map_property_so;
        public Dictionary<Vector2Int, TileDetail> tile_dict;
        public Grid grid;//用来计算grid坐标
        [SerializeField] private RuleTile dig_ruletile;
        [SerializeField] private RuleTile water_ruletile;
        [SerializeField] private Tilemap dig_tilemap;
        [SerializeField] private Tilemap water_tilemap;
        [SerializeField] private List<FlagTilemap> flag_tilemaps;

        public string GUID => GetComponent<Save.Guid>().guid;

        private new void Awake()
        {
            base.Awake();
            if (grid == null) 
                grid = GetComponent<Grid>();
            flag_tilemaps = GetComponentsInChildren<FlagTilemap>().ToList();
            //如果有保存地图数据，则加载
            if (map_property_so != null)
            {
                ReadSO();
            }

            //注册为保存对象
            ISavable savable = this;
            savable.RegisterSaveObject();
        }


        /// <summary>
        /// 用于在卸载时将临时修改写入SO。
        /// NOTE 注意[ExecuteInEditMode]特性使得脚本在编辑器模式中可运行。
        /// 并且由于Unity在进入Play mode时会自动卸载当前常场景并重新加载,因此进入Play mode会调用该脚本的OnDisabl，退出会调用OnEnable。
        /// </summary>
        private void OnDisable()
        {
            if (map_property_so != null)
                WriteSO();
        }

        private void ReadSO()
        {
            //加载保存的tile数据
            tile_dict = map_property_so.ListToDict();
            //恢复特殊tile的贴图
            foreach (var tile_detail in tile_dict.Values)
            {
                if (tile_detail.is_digged)
                    SetTileDig(tile_detail, tile_detail.day_from_dig);
                if (tile_detail.is_watered)
                    SetTileWater(tile_detail, tile_detail.day_from_water);
            }
        }

        //保存TileDetail数据，注意不会保存TileBase的贴图修改，这与TileDetail无关。
        private void WriteSO()
        {
            map_property_so.DictToList(tile_dict);
        }

        #region Tile功能状态设置
        public void SetTileDig(TileDetail tile_detail, float day_from_dig = 0)
        {
            if (dig_tilemap != null && tile_detail != null)
            {
                dig_tilemap.SetTile((Vector3Int)tile_detail.cell_pos, dig_ruletile);
                tile_detail.day_from_dig = day_from_dig;
                if (!tile_detail.is_timing)
                    tile_detail.StartTimeEvent();
                WriteSO();
            }
        }

        public void SetTileWater(TileDetail tile_detail, float day_from_water = 0)
        {
            if (water_tilemap != null && tile_detail != null)
            {
                water_tilemap.SetTile((Vector3Int)tile_detail.cell_pos, water_ruletile);
                tile_detail.day_from_water = day_from_water;
                if (!tile_detail.is_timing)
                    tile_detail.StartTimeEvent();
                WriteSO();
            }
        }

        public void SetTileSeed(TileDetail seed_tile, int seed_id,int day_from_seed = 0)
        {
            Vector2 seed_pos = grid.CellToWorld((Vector3Int)seed_tile.cell_pos);
            seed_pos += new Vector2(0.5f, 0.5f);
            seed_tile.seeded = true;
            CropManager.instance.MakeCrop(seed_id, seed_pos,seed_tile.cell_pos,day_from_seed);
        }
        #endregion

        public TileDetail GetTileDetail(Vector3 world_pos)
        {
            Vector3Int grid_pos = grid.WorldToCell(world_pos);
            TileDetail tile_detail;
            tile_dict.TryGetValue((Vector2Int)grid_pos, out tile_detail);
            return tile_detail;
        }

        /// <summary>
        /// 根据网格坐标得到TileDetail
        /// </summary>
        /// <param name="grid_pos"></param>
        /// <returns>查找失败则返回null</returns>
        public TileDetail GetTileDetail(Vector2Int grid_pos)
        {
            TileDetail tile_detail;
            tile_dict.TryGetValue(grid_pos, out tile_detail);
            return tile_detail;
        }

        public (Vector2Int,Vector2Int) GetGridDimensions()
        {
            return (map_property_so.grid_shape, map_property_so.bottom_left);
        }
#if UNITY_EDITOR
        /// <summary>
        /// 检查所有flag_tilemap，从左下角到右上角更新每个tile的Flag。每次覆盖更新。
        /// </summary>
        [InspectorButton("复原耕种Tilemap")]
        [Conditional("UNITY_EDITOR")]
        private void ResetCropTile()
        {
            tile_dict.Clear();
            //遍历每一种flag tilemap处理
            foreach (var flag_tilemap in flag_tilemaps)
            {
                Tilemap tilemap = flag_tilemap.tilemap;
                Map.TileFlag property = flag_tilemap.flag;
                //压缩边界
                tilemap.CompressBounds();
                Vector3Int min_cell = tilemap.cellBounds.min;
                Vector3Int max_cell = tilemap.cellBounds.max;

                //循环该flag tilemap中所有tile进行重建
                for (int x = min_cell.x; x < max_cell.x; x++)
                {
                    for (int y = min_cell.y; y < max_cell.y; y++)
                    {
                        Vector3Int grid3_pos = new Vector3Int(x, y, 0);
                        if (tilemap.GetTile(grid3_pos) is TileBase tile)
                        {
                            var grid2_pos = new Vector2Int(x, y);
                            //第一次重建tile时 复原被修改的贴图
                            if (!tile_dict.ContainsKey(grid2_pos))
                            {
                                tile_dict.Add(grid2_pos, new TileDetail(grid2_pos, property));
                                dig_tilemap.SetTile(grid3_pos, null);
                                water_tilemap.SetTile(grid3_pos, null);
                            }
                            else
                                tile_dict[grid2_pos].AddProperty(property);
                        }
                    }
                }
            }
            CropManager.instance.Clear();

            EditorUtility.SetDirty(map_property_so);

            WriteSO();
        }
#endif

        #region 存档相关
        public void SaveProfile()
        {
            var list=map_property_so.DictToList(tile_dict);
            GameSaveData.instance.scene_tile_pos[gameObject.scene.name] = list.Item1;
            GameSaveData.instance.scene_tile_detail[gameObject.scene.name] = list.Item2;
        }

        public void LoadProfile()
        {
            map_property_so.cell_postions= GameSaveData.instance.scene_tile_pos[gameObject.scene.name];
           map_property_so.tile_details = GameSaveData.instance.scene_tile_detail[gameObject.scene.name];
            tile_dict = map_property_so.ListToDict();
            //恢复特殊tile的贴图
            foreach (var tile_detail in tile_dict.Values)
            {
                if (tile_detail.is_digged)
                    SetTileDig(tile_detail, tile_detail.day_from_dig);
                else if (tile_detail.is_watered)
                    SetTileWater(tile_detail, tile_detail.day_from_water);
                else
                {
                    dig_tilemap.SetTile((Vector3Int)tile_detail.cell_pos, null);
                    water_tilemap.SetTile((Vector3Int)tile_detail.cell_pos, null);
                }
            }
        }

        #endregion
    }
}
