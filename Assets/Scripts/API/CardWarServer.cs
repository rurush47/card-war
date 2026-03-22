using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardWar.Game;

namespace CardWar.API
{
    public class ServerException : Exception
    {
        public int StatusCode { get; }

        public ServerException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class CardWarServer
    {
        private const int _responseDelayMs = 300;
        private const int _slowResponseDelayMs = 3000;
        private const double _errorRate = 0.1;
        private const double _slowResponseRate = 0.15;
        private readonly CardWarGame _game;
        private readonly Random _random = new();

        public CardWarServer(bool useMiniDeck = false)
        {
            var deckDefinition = useMiniDeck ? DeckDefinitions.MiniDeck : DeckDefinitions.FullDeck;
            _game = new CardWarGame(deckDefinition);
        }

        public async ValueTask<Dictionary<string, string>> PostMove(int playerId, CancellationToken cancellationToken)
        {
            await SimulateNetworkConditions(cancellationToken);
            return new Dictionary<string, string>(_game.PlayRound(playerId));
        }

        public async ValueTask<Dictionary<string, string>> GetConfig(CancellationToken cancellationToken)
        {
            await SimulateNetworkConditions(cancellationToken);
            return new Dictionary<string, string>(_game.GetConfig());
        }

        public async ValueTask PostRestart(bool useMiniDeck, CancellationToken cancellationToken)
        {
            await SimulateNetworkConditions(cancellationToken);
            var deckDefinition = useMiniDeck ? DeckDefinitions.MiniDeck : DeckDefinitions.FullDeck;
            _game.Restart(deckDefinition);
        }

        private async ValueTask SimulateNetworkConditions(CancellationToken cancellationToken)
        {
            var roll = _random.NextDouble();

            if (roll < _errorRate)
            {
                await Task.Delay(_responseDelayMs / 2, cancellationToken);
                throw new ServerException(500, "Internal Server Error: something went wrong on the server.");
            }

            if (roll < _errorRate + _slowResponseRate)
            {
                await Task.Delay(_slowResponseDelayMs, cancellationToken);
                return;
            }

            await Task.Delay(_responseDelayMs, cancellationToken);
        }
    }
}
