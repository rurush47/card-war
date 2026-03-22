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
        private readonly CardWarGame _game;
        private readonly ServerConfig _config;
        private readonly Random _random = new();

        public CardWarServer(bool useMiniDeck, ServerConfig config)
        {
            _config = config;
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

            if (roll < _config.ErrorRate)
            {
                await Task.Delay(_config.ResponseDelayMs / 2, cancellationToken);
                throw new ServerException(500, "Internal Server Error: something went wrong on the server.");
            }

            if (roll < _config.ErrorRate + _config.SlowResponseRate)
            {
                await Task.Delay(_config.SlowResponseDelayMs, cancellationToken);
                return;
            }

            await Task.Delay(_config.ResponseDelayMs, cancellationToken);
        }
    }
}
