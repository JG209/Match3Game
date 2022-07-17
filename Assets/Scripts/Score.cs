using UnityEngine;
using TMPro;

namespace Match3
{
    public sealed class Score : MonoBehaviour
    {
        public static Score Instance {get; private set;}

        private int _score;

        public int score
        {
            get => _score;

            set
            {
                if(_score == value) return;

                _score = value;

                scoreText.SetText($"{_score}");
            }
        }

        [SerializeField] private TextMeshProUGUI scoreText;
        void Awake()
        {
            Instance = this;
        }
    }
}
