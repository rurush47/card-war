using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cards;

[assembly: InternalsVisibleTo("CardWar.Game.Tests")]

namespace CardWar.Game
{
    public enum GameAction
    {
        ShuffleDeck,
        RefillDeck,
        GameOver,
        CardPlayed,
        WarResolved,
        Draw,
        BigPot,
        SmallPot
    }

    public class CardWarGame
    {
        public const int MaxCards = 52;

        private readonly Dictionary<int, Queue<Card>> _decks;
        private readonly Dictionary<int, List<Card>> _sidePiles;
        private readonly Dictionary<int, Card?> _pendingCards = new(2);
        private readonly List<Card> _pot = new();
        private readonly Dictionary<string, string> _gameCommands = new();

        public int PlayerCardCount(int playerIndex) => _decks[playerIndex].Count + _sidePiles[playerIndex].Count;
        public int PotCount => _pot.Count;
        private int _currentPlayerIndex = 1;

        public CardWarGame()
        {
            var deck = CreateShuffledDeck();
            _decks = new Dictionary<int, Queue<Card>>
            {
                { 1, new Queue<Card>() },
                { 2, new Queue<Card>() }
            };
            _sidePiles = new Dictionary<int, List<Card>>
            {
                { 1, new List<Card>() },
                { 2, new List<Card>() }
            };
            _pendingCards = new Dictionary<int, Card?>
            {
                { 1, null },
                { 2, null }
            };

            for (var i = 0; i < MaxCards/2; i++) _decks[1].Enqueue(deck[i]);
            for (var i = MaxCards/2; i < MaxCards; i++) _decks[2].Enqueue(deck[i]);
        }

        internal CardWarGame(IEnumerable<Card> player1Cards, IEnumerable<Card> player2Cards)
        {
            _decks = new Dictionary<int, Queue<Card>>
            {
                { 1, new Queue<Card>(player1Cards) },
                { 2, new Queue<Card>(player2Cards) }
            };
            _sidePiles = new Dictionary<int, List<Card>>
            {
                { 1, new List<Card>() },
                { 2, new List<Card>() }
            };
            _pendingCards = new Dictionary<int, Card?>
            {
                { 1, null },
                { 2, null }
            };
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

        private int OtherPlayerIndex => _currentPlayerIndex == 1 ? 2 : 1;

        public Dictionary<string, string> PlayRound(int playerIndex)
        {
            _gameCommands.Clear();

            if (playerIndex == _currentPlayerIndex)
            {
                RefillIfEmpty(playerIndex);
                if (_decks[playerIndex].Count == 0)
                {
                    _gameCommands.Add(nameof(GameAction.GameOver), OtherPlayerIndex.ToString());
                    return _gameCommands;
                }

                var card = _decks[playerIndex].Dequeue();
                _pendingCards[playerIndex] = card;
                _pot.Add(card);
                _gameCommands.Add(nameof(GameAction.CardPlayed), $"{playerIndex}:{card.Suit}:{card.Rank}");

                if (!_pendingCards[OtherPlayerIndex].HasValue)
                {
                    _currentPlayerIndex = OtherPlayerIndex;
                    return _gameCommands;
                }
            }
            else
            {
                throw new ArgumentException("Wrong player index: " + playerIndex + ", current player index: " + _currentPlayerIndex);
            }

            // Both cards played, resolve
            ResolveCards(_pendingCards[1].Value, _pendingCards[2].Value);
            _pendingCards[1] = null;
            _pendingCards[2] = null;
            _currentPlayerIndex = 1;
            UpdateGameState();

            return _gameCommands;
        }

        private void ResolveCards(Card card1, Card card2)
        {
            int cmp = card1.Rank.CompareTo(card2.Rank);
            if (cmp > 0)
            {
                AwardPotTo(_sidePiles[1]);
                _gameCommands.Add(nameof(GameAction.WarResolved), "1");
                return;
            }
            else if (cmp < 0)
            {
                AwardPotTo(_sidePiles[2]);
                _gameCommands.Add(nameof(GameAction.WarResolved), "2");
                return;
            }
            else
            {
                HandleWarSetup();
            }
        }

        private void HandleWarSetup()
        {
            RefillIfEmpty(1);
            RefillIfEmpty(2);

            // Need 4 cards to continue war: 3 face-down + 1 face-up
            var player1CanContinue = _decks[1].Count >= 4;
            var player2CanContinue = _decks[2].Count >= 4;

            if (!player1CanContinue && !player2CanContinue)
            {
                _gameCommands.Add(nameof(GameAction.Draw), "0");
                return;
            }
            if (!player1CanContinue)
            {
                _gameCommands.Add(nameof(GameAction.GameOver), "2");
                return;
            }
            if (!player2CanContinue)
            {
                _gameCommands.Add(nameof(GameAction.GameOver), "1");
                return;
            }

            // 3 face-down cards each
            _gameCommands.Add(nameof(GameAction.BigPot), "0");
            for (int i = 0; i < 3; i++)
            {
                _pot.Add(_decks[1].Dequeue());
                _pot.Add(_decks[2].Dequeue());
            }

            // 1 face-up card each
            var playerFaceUp = _decks[1].Dequeue();
            var opponentFaceUp = _decks[2].Dequeue();
            _gameCommands.Add(nameof(GameAction.SmallPot), $"{playerFaceUp.Suit}:{playerFaceUp.Rank}|{opponentFaceUp.Suit}:{opponentFaceUp.Rank}");
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
            var player1Empty = PlayerCardCount(1) == 0;
            var player2Empty = PlayerCardCount(2) == 0;

            if (player1Empty && player2Empty)
            {
                _gameCommands.Add(nameof(GameAction.Draw), "0");
            }
            else if (player1Empty)
            {
                _gameCommands.Add(nameof(GameAction.GameOver), "2");
            }
            else if (player2Empty)
            {
                _gameCommands.Add(nameof(GameAction.GameOver), "1");
            }
        }

        private void RefillIfEmpty(int playerIndex)
        {
            if (_decks[playerIndex].Count > 0 || _sidePiles[playerIndex].Count == 0) return;
            _gameCommands.Add(nameof(GameAction.ShuffleDeck), playerIndex.ToString());
            Shuffle(_sidePiles[playerIndex]);
            foreach (var card in _sidePiles[playerIndex]) _decks[playerIndex].Enqueue(card);
            _sidePiles[playerIndex].Clear();
            _gameCommands.Add(nameof(GameAction.RefillDeck), playerIndex.ToString());
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
