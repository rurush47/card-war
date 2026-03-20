using System;
using System.Collections.Generic;
using System.Linq;
using Cards;

namespace CardWar
{
    public class CardWar
    {
        private List<Card> _player1Deck;
        private List<Card> _player1SidePile;
        private List<Card> _player2Deck;
        private List<Card> _player2SidePile;
        private List<Card> _tableCards;
        private Random _random;

        public CardWar(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _player1Deck = new List<Card>();
            _player1SidePile = new List<Card>();
            _player2Deck = new List<Card>();
            _player2SidePile = new List<Card>();
            _tableCards = new List<Card>();
        }

        public void InitializeGame()
        {
            var deck = CreateShuffledDeck();
            DealCards(deck);
        }

        private List<Card> CreateShuffledDeck()
        {
            var deck = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    deck.Add(new Card(suit, rank));
                }
            }
            return Shuffle(deck);
        }

        private List<Card> Shuffle(List<Card> cards)
        {
            return cards.OrderBy(x => _random.Next()).ToList();
        }

        private void DealCards(List<Card> deck)
        {
            _player1Deck.Clear();
            _player2Deck.Clear();
            _player1SidePile.Clear();
            _player2SidePile.Clear();

            for (int i = 0; i < deck.Count; i++)
            {
                if (i % 2 == 0)
                    _player1Deck.Add(deck[i]);
                else
                    _player2Deck.Add(deck[i]);
            }
        }

        public RoundResult PlayRound()
        {
            _tableCards.Clear();

            RefillDeckIfNeeded(ref _player1Deck, _player1SidePile);
            RefillDeckIfNeeded(ref _player2Deck, _player2SidePile);

            if (_player1Deck.Count == 0 && _player1SidePile.Count == 0)
                return new RoundResult { Winner = 2, IsGameOver = true };
            if (_player2Deck.Count == 0 && _player2SidePile.Count == 0)
                return new RoundResult { Winner = 1, IsGameOver = true };

            var player1Card = DrawCard(_player1Deck);
            var player2Card = DrawCard(_player2Deck);
            _tableCards.Add(player1Card);
            _tableCards.Add(player2Card);

            var result = new RoundResult
            {
                Player1Card = player1Card,
                Player2Card = player2Card,
                IsWar = false
            };

            return ResolveCards(result);
        }

        private RoundResult ResolveCards(RoundResult result)
        {
            var comparison = result.Player1Card.CompareTo(result.Player2Card);

            if (comparison > 0)
            {
                _player1SidePile.AddRange(_tableCards);
                result.Winner = 1;
                return result;
            }
            else if (comparison < 0)
            {
                _player2SidePile.AddRange(_tableCards);
                result.Winner = 2;
                return result;
            }
            else
            {
                return HandleWar(result);
            }
        }

        private RoundResult HandleWar(RoundResult result)
        {
            result.IsWar = true;
            result.WarCards = new List<(Card player1, Card player2)>();

            while (true)
            {
                RefillDeckIfNeeded(ref _player1Deck, _player1SidePile);
                RefillDeckIfNeeded(ref _player2Deck, _player2SidePile);

                if (!CanPlayWar(_player1Deck, _player1SidePile))
                {
                    _player2SidePile.AddRange(_tableCards);
                    result.Winner = 2;
                    result.IsGameOver = true;
                    return result;
                }

                if (!CanPlayWar(_player2Deck, _player2SidePile))
                {
                    _player1SidePile.AddRange(_tableCards);
                    result.Winner = 1;
                    result.IsGameOver = true;
                    return result;
                }

                for (int i = 0; i < 3; i++)
                {
                    var p1Card = DrawCard(_player1Deck);
                    var p2Card = DrawCard(_player2Deck);
                    _tableCards.Add(p1Card);
                    _tableCards.Add(p2Card);
                }

                var p1FaceUp = DrawCard(_player1Deck);
                var p2FaceUp = DrawCard(_player2Deck);
                _tableCards.Add(p1FaceUp);
                _tableCards.Add(p2FaceUp);

                result.WarCards.Add((p1FaceUp, p2FaceUp));

                var comparison = p1FaceUp.CompareTo(p2FaceUp);

                if (comparison > 0)
                {
                    _player1SidePile.AddRange(_tableCards);
                    result.Winner = 1;
                    return result;
                }
                else if (comparison < 0)
                {
                    _player2SidePile.AddRange(_tableCards);
                    result.Winner = 2;
                    return result;
                }
            }
        }

        private bool CanPlayWar(List<Card> deck, List<Card> sidePile)
        {
            int totalCards = deck.Count + sidePile.Count;
            return totalCards >= 4;
        }

        private void RefillDeckIfNeeded(ref List<Card> deck, List<Card> sidePile)
        {
            if (deck.Count == 0 && sidePile.Count > 0)
            {
                deck.AddRange(Shuffle(sidePile));
                sidePile.Clear();
            }
        }

        private Card DrawCard(List<Card> deck)
        {
            var card = deck[0];
            deck.RemoveAt(0);
            return card;
        }

        public GameState GetGameState()
        {
            return new GameState
            {
                Player1DeckCount = _player1Deck.Count,
                Player1SidePileCount = _player1SidePile.Count,
                Player2DeckCount = _player2Deck.Count,
                Player2SidePileCount = _player2SidePile.Count
            };
        }

        public bool IsGameOver()
        {
            var p1Total = _player1Deck.Count + _player1SidePile.Count;
            var p2Total = _player2Deck.Count + _player2SidePile.Count;
            return p1Total == 0 || p2Total == 0;
        }

        public int? GetWinner()
        {
            if (!IsGameOver())
                return null;

            var p1Total = _player1Deck.Count + _player1SidePile.Count;
            var p2Total = _player2Deck.Count + _player2SidePile.Count;

            if (p1Total == 0 && p2Total == 0)
                return 0;
            if (p1Total == 0)
                return 2;
            return 1;
        }
    }

    public class RoundResult
    {
        public Card Player1Card { get; set; }
        public Card Player2Card { get; set; }
        public int Winner { get; set; }
        public bool IsWar { get; set; }
        public List<(Card player1, Card player2)> WarCards { get; set; }
        public bool IsGameOver { get; set; }
    }

    public class GameState
    {
        public int Player1DeckCount { get; set; }
        public int Player1SidePileCount { get; set; }
        public int Player2DeckCount { get; set; }
        public int Player2SidePileCount { get; set; }
    }
}
