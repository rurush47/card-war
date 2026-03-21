using CardWar.API;
using UnityEngine;

namespace CardWar.View
{
    public class GameController : MonoBehaviour
    {
        private CardWarServer _server;
        [SerializeField] private CardView _cardView;
        [SerializeField] private Sprite _cardSprite;
        [SerializeField] private Sprite _backSprite;

        private void Start()
        {
            _server = new CardWarServer();
            var initialGameState = _server.GetState();
            _cardView.Initialize(_cardSprite, _backSprite);
        }

        private void InitGameView(StateResponse initialState)
        {
            
        }

        [ContextMenu("Play")]
        public void Play()
        {
            FindObjectOfType<CardView>().FlipAsync(true);
        }
    }
}