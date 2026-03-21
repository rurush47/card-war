using System.Collections.Generic;
using System.Threading.Tasks;
using CardWar.Game;

namespace CardWar.API
{
    public class CardWarServer
    {
        private const int _responseDelayMs = 300;
        private readonly CardWarGame _game = new();
        private readonly Dictionary<string, string> _config;

        public CardWarServer()
        {
            _config = new Dictionary<string, string> { { "max_cards", $"{CardWarGame.MaxCards}" }}; 
        }

        public async ValueTask<List<(string Action, int PlayerIndex)>> PostMove(int playerId)
        {
            await Task.Delay(_responseDelayMs);
            return _game.PlayRound(playerId);
        }

        public async ValueTask<Dictionary<string, string>> GetConfig()
        {
            await Task.Delay(_responseDelayMs / 2);
            return _config;
        }
    }
}
