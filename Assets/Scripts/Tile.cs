using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public sealed class Tile : MonoBehaviour
    {
        public int x;
        public int y;

        private Item _item;
        public Item item {
            get => _item; 
            set
            {
                if(_item == value) return;

                _item = value;

                icon.sprite = _item.sprite;
            }
            }


        public Image icon;

        public Button button;

        public Tile Left => x > 0 ? Board.Instance.Tiles[x-1, y] : null; //Adjacent left tile
        public Tile Top => y > 0 ? Board.Instance.Tiles[x, y-1] : null; //Adjacent top tile
        public Tile Right => x < Board.Instance.Width-1 ? Board.Instance.Tiles[x+1, y] : null; //Adjacent right tile
        public Tile Bottom => y < Board.Instance.Height-1 ? Board.Instance.Tiles[x, y+1] : null; //Adjacent bottom tile

        /// <summary>
        /// Return a array with all adjacent tiles to this tile
        /// </summary>
        /// <value></value>
        public Tile[] Neighbours => new[]{Left, Top, Right, Bottom};

        void Start()
        {
            button.onClick.AddListener(() => Board.Instance.Select(this));
        }

        public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
        {
            List<Tile> result = new List<Tile> {this, };

            if(exclude == null)
            {
                exclude = new List<Tile>{this, };
            }
            else
            {
                exclude.Add(this);
            }

            foreach (Tile neighbour in Neighbours)
            {
                if(neighbour == null || exclude.Contains(neighbour) || neighbour.item != item) continue;
                result.AddRange(neighbour.GetConnectedTiles(exclude));
            }

            return result;
        }
    }
}
