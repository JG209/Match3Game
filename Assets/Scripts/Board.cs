using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


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

        private bool _isSwapping = false;
        private bool isSwapping {
            get => _isSwapping;

            set
            {
                _isSwapping = value;

                if(_isSwapping) AudioManager.Instance.PlayOneShot(AudioManager.Instance.swapSound);//Plays the swap sound
            }    
        }



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

            Pop();//to avoid initialize with matchs
        }

        /// <summary>
        /// Used to Select the tiles, need to be called two times so it makes a valid selection 
        /// </summary>
        /// <param name="tile">Tile to be selected</param>
        /// <returns></returns>
        public async void Select(Tile tile)
        {
            if(isSwapping) return; //Avoid to allow player to select another tile while a swapping is running

            if(!_selection.Contains(tile))
            {
                Debug.Log($"Is Swapping: {isSwapping}");
                //Check if its a valid selection to move
                if(_selection.Count > 0)
                {
                    if(Array.IndexOf(_selection[0].Neighbours, tile) != -1) AddElementeToSelectionList(tile);
                }
                else
                    AddElementeToSelectionList(tile);

            }


            if(_selection.Count < 2) return;

            Debug.Log($"Selected tiles at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

            isSwapping = true;

            await Swap(_selection[0], _selection[1]);

            if(CanPop())
                Pop();
            else
                await Swap(_selection[0], _selection[1]);

            // isSwapping = false;

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
            isSwapping = true;

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

            isSwapping = false;
        }

        private bool CanPop()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if(Tiles[x,y].GetConnectedTiles().Skip(1).Count() >= 2) return true;

            return false;
        }

        private async void Pop()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tile tile = Tiles[x, y];

                    List<Tile> connectedTiles = tile.GetConnectedTiles();

                    if(connectedTiles.Skip(1).Count() < 2) continue;

                    Sequence deflateSequence = DOTween.Sequence();

                    foreach (Tile connectedTile in connectedTiles)
                        deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, tweenDuration));

                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.clearSound);//Plays the clear sound

                    Score.Instance.score += tile.item.value * connectedTiles.Count; //adds pontuation to score

                    await deflateSequence.Play().AsyncWaitForCompletion();

                    Sequence inflateSequence = DOTween.Sequence();

                    foreach (Tile connectedTile in connectedTiles)
                    {
                        connectedTile.item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                        inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, tweenDuration));
                    }

                    await inflateSequence.Play().AsyncWaitForCompletion();

                    //Force to check the board again
                    x=0;
                    y=0;
                }
            }
        }
        /// <summary>
        /// Used to add a element to the _selection list and trigger the method to play the select sound
        /// </summary>
        /// <param name="element"></param>
        private void AddElementeToSelectionList(Tile element)
        {
            _selection.Add(element);
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.selectSound);//Plays the select sound
        }
    }
}
