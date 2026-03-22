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
        [SerializeField] private AnimationController _animationController;
        private CardWarServer _server;
        private CancellationTokenSource _gameCancellationTokenSource = new();
        private CancellationToken _gameCancellationToken => _gameCancellationTokenSource.Token;
        
        private async void Start()
        {
            _server = new CardWarServer();
            var config = await _server.GetConfig(_gameCancellationToken);
            _animationController.Init(config, _gameCancellationToken);
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
            
            var result = await _server.PostMove(1, _gameCancellationToken);
            foreach (var (action, value) in result)
                await ResolveAction(action, value, _gameCancellationToken);

            result = await _server.PostMove(2, _gameCancellationToken);
            foreach (var (action, value) in result)
                await ResolveAction(action, value, _gameCancellationToken);
            _animationsRunning = false;
        }
        
        private async ValueTask ResolveAction(string action, string value, CancellationToken cancellationToken)
        {
            int playerIndex;
            switch (action)
            {
                case "CardPlayed":
                    var move = ParsePlayedCard(value);
                    await _animationController.PlayCard(move.PlayerInxed, move.Card, cancellationToken);
                    break;
                case "ShuffleDeck":
                    playerIndex = int.Parse(value);
                    await _animationController.ShuffleDeck(playerIndex, cancellationToken);
                    break;
                case "RefillDeck":
                    playerIndex = int.Parse(value);
                    await _animationController.RefillDeck(playerIndex, cancellationToken);
                    break;
                case "GameOver":
                    playerIndex = int.Parse(value);
                    await _animationController.GameOver(playerIndex, cancellationToken);
                    break;
                case "WarResolved":
                    playerIndex = int.Parse(value);
                    await _animationController.WarResolved(playerIndex, cancellationToken);
                    break;
                case "Draw":
                    playerIndex = int.Parse(value);
                    await _animationController.Draw(playerIndex, cancellationToken);
                    break;
                case "BigPot":
                    playerIndex = int.Parse(value);
                    await _animationController.BigPot(playerIndex, cancellationToken);
                    break;
                case "SmallPot":
                    var cards = ParsePotCards(value);
                    await _animationController.SmallPot(cards.Card1, cards.Card2, cancellationToken);
                    break;
            }
        }

        private (int PlayerInxed, Card Card) ParsePlayedCard(string value)
        {
            var parts = value.Split(':');
            int.TryParse(parts[0], out var playerIndex);
            return (playerIndex, new Card(Enum.Parse<Suit>(parts[1]), Enum.Parse<Rank>(parts[2])));
        }
        
        private (Card Card1, Card Card2) ParsePotCards(string value)
        {
            var cardParts = value.Split('|');
            var p1Parts = cardParts[0].Split(':');
            var p2Parts = cardParts[1].Split(':');
            var card1 = new Card(Enum.Parse<Suit>(p1Parts[0]), Enum.Parse<Rank>(p1Parts[1]));
            var card2 = new Card(Enum.Parse<Suit>(p2Parts[0]), Enum.Parse<Rank>(p2Parts[1]));
            return (card1, card2);
        }

        private void OnDestroy()
        {
            _gameCancellationTokenSource.Cancel();
            _gameCancellationTokenSource.Dispose();
        }
    }
}