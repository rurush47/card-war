using Cards;
using CardWar.Game;

namespace CardWar.API
{
    public class StateResponse
    {
        public GameState GameState;
        public Card PlayerCard;
        public Card OpponentCard;
        public int PlayerCardCount;
        public int OpponentCardCount;
        public int PotCount;
    }
}
