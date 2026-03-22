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
        [SerializeField] private MessageModal _messageModal;
        [SerializeField] private bool _useMiniDeck;
        
        private ResilientServerProxy _server;
        private CancellationTokenSource _gameCancellationTokenSource = new();
        private CancellationToken _gameCancellationToken => _gameCancellationTokenSource.Token;
        private int _playerIndex = 1;
        private int _cpuIndex = 2;
        private bool _actionOngoing;
        
        private async void Start()
        {
            _server = new ResilientServerProxy(new CardWarServer(_useMiniDeck));
            
            _actionOngoing = true;
            try
            {
                var config = await _server.GetConfig(_gameCancellationToken);
                await _animationController.Init(config, _gameCancellationToken);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            _actionOngoing = false;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_actionOngoing)
            {
                HandlePlay();
            }
        }

        private async void HandlePlay()
        {
            try
            {
                _actionOngoing = true;
                await HandleMove();
            }
            catch (OperationCanceledException)
            {
                //operation canceled, normal behavior
            }
            catch (ArgumentException e)
            {
                Debug.LogError("Wrong player index played!\n" + e);
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError("Game trying to be played while already finished!\n" + e);
            }
            catch (ServerException e)
            {
                Debug.LogError($"Server unreachable after retries: {e.Message}");
            }
            catch (Exception e)
            {
                //general exception, should not happen
                Debug.LogException(e);
            }
            finally
            {
                _actionOngoing = false;
            }
        }

        private async ValueTask HandleMove()
        {
            await MovePlayer(_playerIndex);
            //auto move cpu
            await MovePlayer(_cpuIndex);
        }

        private async ValueTask MovePlayer(int index)
        {
            var result = await _server.PostMove(index, _gameCancellationToken);
            foreach (var (action, value) in result)
                await ResolveAction(action, value, _gameCancellationToken);
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
                case "WarResolved":
                    playerIndex = int.Parse(value);
                    await _animationController.WarResolved(playerIndex, cancellationToken);
                    break;
                case "BigPot":
                    playerIndex = int.Parse(value);
                    await _animationController.BigPot(playerIndex, cancellationToken);
                    break;
                case "SmallPot":
                    var cards = ParsePotCards(value);
                    await _animationController.SmallPot(cards.Card1, cards.Card2, cancellationToken);
                    break;
                case "Draw":
                case "GameOver":
                    await HandleGameOver(value, cancellationToken);
                    await RestartGame();
                    break;
            }
        }

        private async ValueTask RestartGame()
        {
            await _server.PostRestart(_useMiniDeck, _gameCancellationToken);
            var config = await _server.GetConfig(_gameCancellationToken);
            await _animationController.Init(config, _gameCancellationToken);
        }

        private async Task HandleGameOver(string value, CancellationToken cancellationToken)
        {
            var playerIndex = int.Parse(value);
            if (playerIndex == 0)
            {
                await _messageModal.ShowMessage("Draw!", cancellationToken);
                return;
            }
            await _messageModal.ShowMessage($"Player {playerIndex} wins!", cancellationToken);
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