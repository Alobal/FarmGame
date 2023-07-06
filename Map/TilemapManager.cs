using Crop;
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
    /// 所有TilemapDetail的管理器，存储于SO文件。Enbale加载，Disable更新保存。

    /// </summary>
    [ExecuteInEditMode]
    public class TilemapManager : Singleton<TilemapManager>
    {

        public MapPropertySO map_property_so;
        public Dictionary<Vector2Int, TileDetail> tile_dict;
        [SerializeField] private RuleTile dig_ruletile;
        [SerializeField] private RuleTile water_ruletile;
        [SerializeField] private Tilemap dig_tilemap;
        [SerializeField] private Tilemap water_tilemap;
        [SerializeField] private Grid grid;//用来计算grid坐标
        [SerializeField] private List<FlagTilemap> flag_tilemaps;

        private new void Awake()
        {
            base.Awake();
            if (grid == null) grid = GetComponent<Grid>();
            flag_tilemaps = GetComponentsInChildren<FlagTilemap>().ToList();

        }
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (map_property_so != null)
            {
                Load();
                EditorUtility.SetDirty(map_property_so);
            }
#endif
        }
        /// <summary>
        /// 注意[ExecuteInEditMode]特性使得脚本在编辑器模式中可运行。
        /// 并且由于Unity在进入Play mode时会自动卸载当前常场景并重新加载,因此进入Play mode会调用该脚本的OnDisabl，退出会调用OnEnable。
        /// </summary>
        private void OnDisable()
        {
            if (map_property_so != null)
                Save();
        }

        private void Load()
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

        private void Save()
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
                Save();
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
                Save();
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
            tile_dict.TryGetValue(grid_pos.XY(), out tile_detail);
            return tile_detail;
        }

        public TileDetail GetTileDetail(Vector2Int grid_pos)
        {
            TileDetail tile_detail;
            tile_dict.TryGetValue(grid_pos, out tile_detail);
            return tile_detail;
        }

        /// <summary>
        /// 压缩tilemap后，从左下角到右上角更新每个tile的property。每次覆盖更新。
        /// </summary>
        [InspectorButton("初始化Tilemap Flags")]
        [Conditional("UNITY_EDITOR")]
        private void InitMapProperty()
        {
            tile_dict.Clear();
            foreach (var flag_tilemap in flag_tilemaps)
            {
                Tilemap tilemap = flag_tilemap.tilemap;
                Map.TileFlag property = flag_tilemap.flag;

                tilemap.CompressBounds();
                Vector3Int min_cell = tilemap.cellBounds.min;
                Vector3Int max_cell = tilemap.cellBounds.max;

                for (int x = min_cell.x; x < max_cell.x; x++)
                {
                    for (int y = min_cell.y; y < max_cell.y; y++)
                    {
                        Vector3Int cell_pos = new Vector3Int(x, y, 0);
                        if (tilemap.GetTile(cell_pos) is TileBase tile)
                        {
                            var new_cell_pos = new Vector2Int(x, y);
                            if (!tile_dict.ContainsKey(new_cell_pos))
                            {
                                tile_dict.Add(new_cell_pos, new TileDetail(new_cell_pos, property));
                                dig_tilemap.SetTile(cell_pos, null);
                                water_tilemap.SetTile(cell_pos, null);
                            }
                            else
                                tile_dict[new_cell_pos].AddProperty(property);
                        }
                    }
                }
            }
            Save();
        }
    }
}
