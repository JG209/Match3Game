using System.Net.Mime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Match3
{
    public sealed class Board : MonoBehaviour
    {
        public static Board Instance {get ; private set;}

        public Row[] rows;

        public Tile[,] Tiles {get ; private set;}

        public int Width {get => Tiles.GetLength(0);}
        public int Height {get => Tiles.GetLength(1);}

        private readonly List<Tile> _selection = new List<Tile>();

        private const float tweenDuration = 0.25f;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            
            Tiles = new Tile[rows.Max(rows => rows.tiles.Length), rows.Length];

            for (int y = 0; y < Height; y++)//Populate the board
            {
                for (int x = 0; x < Width; x++)
                {
                    Tile tile  = rows[y].tiles[x];

                    tile.x = x;
                    tile.y = y;

                    tile.item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                    Tiles[x, y] = tile;
                }
            }
        }

        /// <summary>
        /// Used to Select the tiles, need to be called two times so it makes a valid selection 
        /// </summary>
        /// <param name="tile">Tile to be selected</param>
        /// <returns></returns>
        public async void Select(Tile tile)
        {
            if(!_selection.Contains(tile))
                _selection.Add(tile);

            if(_selection.Count < 2) return;

            Debug.Log($"Selected tiles at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

            await Swap(_selection[0], _selection[1]);

            _selection.Clear();
        }
        
        /// <summary>
        /// Swap the tiles selected, change sprites, items and parent
        /// </summary>
        /// <param name="tile1"></param>
        /// <param name="tile2"></param>
        /// <returns></returns>
        public async Task Swap(Tile tile1, Tile tile2)
        {
            var icon1 = tile1.icon;
            var icon2 = tile2.icon;

            Transform icon1Transform = icon1.transform;
            Transform icon2Transform = icon2.transform;

            var sequence = DOTween.Sequence();
            
            sequence.Join(icon1Transform.DOMove(icon2Transform.position, tweenDuration))
                    .Join(icon2Transform.DOMove(icon1Transform.position, tweenDuration));

            await sequence.Play().AsyncWaitForCompletion();

            icon1Transform.SetParent(tile2.transform);
            icon2Transform.SetParent(tile1.transform);

            tile1.icon = icon2;
            tile2.icon = icon1;

            var tempTile1Item = tile1.item;

            tile1.item = tile2.item;
            tile2.item = tempTile1Item;
        }
    }
}
