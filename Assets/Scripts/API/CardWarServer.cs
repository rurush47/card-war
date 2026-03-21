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
            _config = new Dictionary<string, string> { { "max_cards", $"{_game.PlayerCardCount}" }}; 
        }

        public async ValueTask<StateResponse> PostMove(int playerId, int cardIndex)
        {
            await Task.Delay(_responseDelayMs);
            _game.PlayRound();
            return BuildResponse();
        }

        public async ValueTask<StateResponse> GetState()
        {
            await Task.Delay(_responseDelayMs / 2);
            return BuildResponse();
        }
        
        public async ValueTask<Dictionary<string, string>> GetConfig()
        {
            await Task.Delay(_responseDelayMs / 2);
            return _config;
        }

        private StateResponse BuildResponse() => new StateResponse
        {
            GameState = _game.State,
            PlayerCard = _game.LastPlayerCard,
            OpponentCard = _game.LastOpponentCard,
            PlayerCardCount = _game.PlayerCardCount,
            OpponentCardCount = _game.OpponentCardCount,
            PotCount = _game.PotCount,
        };
    }
}
