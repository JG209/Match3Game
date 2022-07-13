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

        void Start()
        {
            button.onClick.AddListener(() => Board.Instance.Select(this));
        }
    }
}
