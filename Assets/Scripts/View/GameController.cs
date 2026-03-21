using System.Threading;
using System.Threading.Tasks;
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
            _animationController.Init(config);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Play();
            }
        }

        private async void Play()
        {
            var result = await _server.PostMove(1);
            // TODO
            // if(!result.Success)
            //     handle error

            var cancellationToken = new CancellationToken();
            foreach (var (action, playerIndex) in result)
            {
                await ResolveAction(action, playerIndex, cancellationToken);
            }
        }
        
        private async ValueTask ResolveAction(string action, int playerIndex, CancellationToken cancellationToken)
        {
            switch (action)
            {
                case "ShuffleDeck":
                    await _animationController.ShuffleDeck(playerIndex, cancellationToken);
                    break;
                case "RefillDeck":
                    await _animationController.RefillDeck(playerIndex, cancellationToken);
                    break;
                case "GameOver":
                    await _animationController.GameOver(playerIndex, cancellationToken);
                    break;
                case "CardPlayed":
                    await _animationController.CardPlayed(playerIndex, cancellationToken);
                    break;
                case "WarResolved":
                    await _animationController.WarResolved(playerIndex, cancellationToken);
                    break;
                case "Draw":
                    await _animationController.Draw(playerIndex, cancellationToken);
                    break;
                case "BigPot":
                    await _animationController.BigPot(playerIndex, cancellationToken);
                    break;
                case "SmallPot":
                    await _animationController.SmallPot(playerIndex, cancellationToken);
                    break;
            }
        }
    }
}