using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardWar.Game;

namespace CardWar.API
{
    public class CardWarServer
    {
        private const int _responseDelayMs = 300;
        private readonly CardWarGame _game;

        public CardWarServer(bool useMiniDeck = false)
        {
            var deckDefinition = useMiniDeck ? DeckDefinitions.MiniDeck : DeckDefinitions.FullDeck;
            _game = new CardWarGame(deckDefinition);
        }

        public async ValueTask<Dictionary<string, string>> PostMove(int playerId, CancellationToken cancellationToken)
        {
            await Task.Delay(_responseDelayMs, cancellationToken);
            return new Dictionary<string, string>(_game.PlayRound(playerId));
        }

        public async ValueTask<Dictionary<string, string>> GetConfig(CancellationToken cancellationToken)
        {
            await Task.Delay(_responseDelayMs / 2, cancellationToken);
            return new Dictionary<string, string>(_game.GetConfig());
        }

        public async ValueTask PostRestart(bool useMiniDeck, CancellationToken cancellationToken)
        {
            await Task.Delay(_responseDelayMs, cancellationToken);
            var deckDefinition = useMiniDeck ? DeckDefinitions.MiniDeck : DeckDefinitions.FullDeck;
            _game.Restart(deckDefinition);
        }
    }
}
