using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    /// <summary>
    /// 用于标识Flag属性的特殊Tilemap
    /// </summary>
    public class FlagTilemap : MonoBehaviour
    {
        public Tilemap tilemap;
        public TileFlag flag;

        private void Awake()
        {
            if (tilemap == null)
            {
                tilemap = GetComponent<Tilemap>();
            }
        }

    }
}