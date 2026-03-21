using System;
using System.Threading;
using System.Threading.Tasks;
using CardWar.API;
using Cards;
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

        private bool _animationsRunning;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_animationsRunning)
            {
                Play();
            }
        }

        private async void Play()
        {
            // TODO
            // if(!result.Success)
            //     handle error

            _animationsRunning = true;
            var cancellationToken = new CancellationToken();
            
            var result = await _server.PostMove(1);
            foreach (var (action, value) in result)
                await ResolveAction(action, value, cancellationToken);

            result = await _server.PostMove(2);
            foreach (var (action, value) in result)
                await ResolveAction(action, value, cancellationToken);
            _animationsRunning = false;
        }
        
        private async ValueTask ResolveAction(string action, string value, CancellationToken cancellationToken)
        {
            if (action == "CardPlayed")
            {
                var parts = value.Split(':');
                int.TryParse(parts[0], out var playerIndex);
                var card = new Card(Enum.Parse<Suit>(parts[1]), Enum.Parse<Rank>(parts[2]));
                await _animationController.PlayCard(playerIndex, card, cancellationToken);
                return;
            }

            int.TryParse(value, out var pi);
            switch (action)
            {
                case "ShuffleDeck":
                    await _animationController.ShuffleDeck(pi, cancellationToken);
                    break;
                case "RefillDeck":
                    await _animationController.RefillDeck(pi, cancellationToken);
                    break;
                case "GameOver":
                    await _animationController.GameOver(pi, cancellationToken);
                    break;
                case "WarResolved":
                    await _animationController.WarResolved(pi, cancellationToken);
                    break;
                case "Draw":
                    await _animationController.Draw(pi, cancellationToken);
                    break;
                case "BigPot":
                    await _animationController.BigPot(pi, cancellationToken);
                    break;
                case "SmallPot":
                    var cardParts = value.Split('|');
                    var p1Parts = cardParts[0].Split(':');
                    var p2Parts = cardParts[1].Split(':');
                    var card1 = new Card(Enum.Parse<Suit>(p1Parts[0]), Enum.Parse<Rank>(p1Parts[1]));
                    var card2 = new Card(Enum.Parse<Suit>(p2Parts[0]), Enum.Parse<Rank>(p2Parts[1]));
                    await _animationController.SmallPot(card1, card2, cancellationToken);
                    break;
            }
        }
    }
}