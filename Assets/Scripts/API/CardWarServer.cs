using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardWar.Game;

namespace CardWar.API
{
    public class CardWarServer
    {
        private const int _responseDelayMs = 300;
        private readonly CardWarGame _game = new();
        private readonly Dictionary<string, string> _config = new() { { "max_cards", $"{CardWarGame.MaxCards}" }};

        public async ValueTask<Dictionary<string, string>> PostMove(int playerId, CancellationToken cancellationToken)
        {
            await Task.Delay(_responseDelayMs, cancellationToken);
            return _game.PlayRound(playerId);
        }

        public async ValueTask<Dictionary<string, string>> GetConfig(CancellationToken cancellationToken)
        {
            await Task.Delay(_responseDelayMs / 2, cancellationToken);
            return _config;
        }

        public async ValueTask PostRestart(CancellationToken cancellationToken)
        {
            await Task.Delay(_responseDelayMs, cancellationToken);
            _game.Restart();
        }
    }
}
