using CardWar.API;
using UnityEngine;

namespace CardWar.View
{
    public class GameController : MonoBehaviour
    {
        private CardWarServer _server;
        [SerializeField] private AnimationController _animationController;
        
        private async void Start()
        {
            _server = new CardWarServer();
            var config = await _server.GetConfig();
            var initialGameState = await _server.GetState();
            _animationController.Init(config, initialGameState);
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