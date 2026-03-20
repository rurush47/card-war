using System;
using System.Collections.Generic;
using Cards;

namespace CardWar.Game
{
    public enum GameState { Ongoing, PlayerWon, OpponentWon, Draw }

    public class WarGame
    {
        private readonly Queue<Card> _playerDeck;
        private readonly List<Card> _playerSidePile = new List<Card>();
        private readonly Queue<Card> _opponentDeck;
        private readonly List<Card> _opponentSidePile = new List<Card>();
        private readonly List<Card> _pot = new List<Card>();

        public GameState State { get; private set; } = GameState.Ongoing;
        public Card LastPlayerCard { get; private set; }
        public Card LastOpponentCard { get; private set; }
        public int PlayerCardCount => _playerDeck.Count + _playerSidePile.Count;
        public int OpponentCardCount => _opponentDeck.Count + _opponentSidePile.Count;
        public int PotCount => _pot.Count;

        public WarGame()
        {
            var deck = CreateShuffledDeck();
            _playerDeck = new Queue<Card>();
            _opponentDeck = new Queue<Card>();
            for (int i = 0; i < 26; i++) _playerDeck.Enqueue(deck[i]);
            for (int i = 26; i < 52; i++) _opponentDeck.Enqueue(deck[i]);
        }

        private static List<Card> CreateShuffledDeck()
        {
            var deck = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    deck.Add(new Card(suit, rank));
            Shuffle(deck);
            return deck;
        }

        public void PlayRound()
        {
            if (State != GameState.Ongoing)
                throw new InvalidOperationException("Game is over.");

            RefillIfEmpty(_playerDeck, _playerSidePile);
            RefillIfEmpty(_opponentDeck, _opponentSidePile);

            var playerCard = _playerDeck.Dequeue();
            var opponentCard = _opponentDeck.Dequeue();
            LastPlayerCard = playerCard;
            LastOpponentCard = opponentCard;
            _pot.Add(playerCard);
            _pot.Add(opponentCard);

            ResolveCards(playerCard, opponentCard);
            UpdateGameState();
        }

        private void ResolveCards(Card playerCard, Card opponentCard)
        {
            int cmp = playerCard.Rank.CompareTo(opponentCard.Rank);
            if (cmp > 0)
                AwardPotTo(_playerSidePile);
            else if (cmp < 0)
                AwardPotTo(_opponentSidePile);
            else
                ResolveWar();
        }

        private void ResolveWar()
        {
            if (State != GameState.Ongoing) return;

            RefillIfEmpty(_playerDeck, _playerSidePile);
            RefillIfEmpty(_opponentDeck, _opponentSidePile);

            // Need 4 cards to continue war: 3 face-down + 1 face-up
            bool playerCanContinue = _playerDeck.Count >= 4;
            bool opponentCanContinue = _opponentDeck.Count >= 4;

            if (!playerCanContinue && !opponentCanContinue) { State = GameState.Draw; return; }
            if (!playerCanContinue) { State = GameState.OpponentWon; return; }
            if (!opponentCanContinue) { State = GameState.PlayerWon; return; }

            // 3 face-down cards each
            for (int i = 0; i < 3; i++)
            {
                _pot.Add(_playerDeck.Dequeue());
                _pot.Add(_opponentDeck.Dequeue());
            }

            // 1 face-up card each
            var playerFaceUp = _playerDeck.Dequeue();
            var opponentFaceUp = _opponentDeck.Dequeue();
            _pot.Add(playerFaceUp);
            _pot.Add(opponentFaceUp);

            ResolveCards(playerFaceUp, opponentFaceUp);
        }

        private void AwardPotTo(List<Card> sidePile)
        {
            sidePile.AddRange(_pot);
            _pot.Clear();
        }

        private void UpdateGameState()
        {
            if (State != GameState.Ongoing) return;

            RefillIfEmpty(_playerDeck, _playerSidePile);
            RefillIfEmpty(_opponentDeck, _opponentSidePile);

            bool playerOut = PlayerCardCount == 0;
            bool opponentOut = OpponentCardCount == 0;

            if (playerOut && opponentOut) State = GameState.Draw;
            else if (playerOut) State = GameState.OpponentWon;
            else if (opponentOut) State = GameState.PlayerWon;
        }

        private static void RefillIfEmpty(Queue<Card> deck, List<Card> sidePile)
        {
            if (deck.Count > 0 || sidePile.Count == 0) return;
            Shuffle(sidePile);
            foreach (var card in sidePile) deck.Enqueue(card);
            sidePile.Clear();
        }

        private static void Shuffle<T>(List<T> list)
        {
            var rng = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
