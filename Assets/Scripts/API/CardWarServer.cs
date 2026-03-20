using System.Threading.Tasks;
using UnityEngine;
using CardWar.Game;

namespace CardWar.API
{
    public class CardWarServer : MonoBehaviour
    {
        [SerializeField] private int _responseDelayMs = 300;

        private WarGame _game = new WarGame();

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
