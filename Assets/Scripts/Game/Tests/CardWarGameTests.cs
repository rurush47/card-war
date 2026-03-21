using System;
using System.Reflection;
using Cards;
using NUnit.Framework;

namespace CardWar.Game.Tests
{
    [TestFixture]
    public class CardWarGameTests
    {
        [Test]
        public void Constructor_InitializesGameWithOngoingState()
        {
            var game = new CardWarGame();

            Assert.AreEqual(GameState.Ongoing, game.State);
        }

        [Test]
        public void Constructor_Distributes52CardsEvenlyBetweenPlayers()
        {
            var game = new CardWarGame();

            Assert.AreEqual(26, game.PlayerCardCount);
            Assert.AreEqual(26, game.OpponentCardCount);
        }

        [Test]
        public void Constructor_InitializesEmptyPot()
        {
            var game = new CardWarGame();

            Assert.AreEqual(0, game.PotCount);
        }

        [Test]
        public void PlayRound_UpdatesLastPlayedCards()
        {
            var game = new CardWarGame();

            game.PlayRound();

            Assert.IsNotNull(game.LastPlayerCard);
            Assert.IsNotNull(game.LastOpponentCard);
        }

        [Test]
        public void PlayRound_DecreasesTotalCardCountByTwo_WhenNoWar()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.King) },
                new[] { new Card(Suit.Clubs, Rank.Two) }
            );

            game.PlayRound();

            // Player wins both cards (goes to side pile), opponent has none
            Assert.AreEqual(2, game.PlayerCardCount);
            Assert.AreEqual(0, game.OpponentCardCount);
        }

        [Test]
        public void PlayRound_PlayerWinsWhenPlayerCardIsHigher()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.King) },
                new[] { new Card(Suit.Clubs, Rank.Two) }
            );

            game.PlayRound();

            Assert.AreEqual(2, game.PlayerCardCount);
            Assert.AreEqual(0, game.OpponentCardCount);
        }

        [Test]
        public void PlayRound_OpponentWinsWhenOpponentCardIsHigher()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.Two) },
                new[] { new Card(Suit.Clubs, Rank.King) }
            );

            game.PlayRound();

            Assert.AreEqual(0, game.PlayerCardCount);
            Assert.AreEqual(2, game.OpponentCardCount);
        }

        [Test]
        public void PlayRound_TriggersWarWhenCardsAreEqual()
        {
            var game = CreateGameWithKnownDecks(
                new[] {
                    new Card(Suit.Hearts, Rank.Five),
                    new Card(Suit.Hearts, Rank.Two),
                    new Card(Suit.Hearts, Rank.Three),
                    new Card(Suit.Hearts, Rank.Four),
                    new Card(Suit.Hearts, Rank.King)
                },
                new[] {
                    new Card(Suit.Clubs, Rank.Five),
                    new Card(Suit.Clubs, Rank.Two),
                    new Card(Suit.Clubs, Rank.Three),
                    new Card(Suit.Clubs, Rank.Four),
                    new Card(Suit.Clubs, Rank.Queen)
                }
            );

            game.PlayRound();

            // Player should win the war (King > Queen), getting all 10 cards
            Assert.AreEqual(10, game.PlayerCardCount);
            Assert.AreEqual(0, game.OpponentCardCount);
        }

        [Test]
        public void PlayRound_DrawWhenBothPlayersRunOutDuringWar()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.Five) },
                new[] { new Card(Suit.Clubs, Rank.Five) }
            );

            game.PlayRound();

            Assert.AreEqual(GameState.Draw, game.State);
        }

        [Test]
        public void PlayRound_OpponentWinsWhenPlayerCannotContinueWar()
        {
            var game = CreateGameWithKnownDecks(
                new[] {
                    new Card(Suit.Hearts, Rank.Five),
                    new Card(Suit.Hearts, Rank.Two),
                    new Card(Suit.Hearts, Rank.Three)
                },
                new[] {
                    new Card(Suit.Clubs, Rank.Five),
                    new Card(Suit.Clubs, Rank.Two),
                    new Card(Suit.Clubs, Rank.Three),
                    new Card(Suit.Clubs, Rank.Four),
                    new Card(Suit.Clubs, Rank.Six)
                }
            );

            game.PlayRound();

            Assert.AreEqual(GameState.OpponentWon, game.State);
        }

        [Test]
        public void PlayRound_PlayerWinsWhenOpponentCannotContinueWar()
        {
            var game = CreateGameWithKnownDecks(
                new[] {
                    new Card(Suit.Hearts, Rank.Five),
                    new Card(Suit.Hearts, Rank.Two),
                    new Card(Suit.Hearts, Rank.Three),
                    new Card(Suit.Hearts, Rank.Four),
                    new Card(Suit.Hearts, Rank.Six)
                },
                new[] {
                    new Card(Suit.Clubs, Rank.Five),
                    new Card(Suit.Clubs, Rank.Two),
                    new Card(Suit.Clubs, Rank.Three)
                }
            );

            game.PlayRound();

            Assert.AreEqual(GameState.PlayerWon, game.State);
        }

        [Test]
        public void PlayRound_ThrowsExceptionWhenGameIsOver()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.King) },
                new[] { new Card(Suit.Clubs, Rank.Two) }
            );

            game.PlayRound(); // Player wins

            Assert.Throws<InvalidOperationException>(() => game.PlayRound());
        }

        [Test]
        public void State_UpdatesToPlayerWonWhenOpponentRunsOut()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.King) },
                new[] { new Card(Suit.Clubs, Rank.Two) }
            );

            game.PlayRound();

            Assert.AreEqual(GameState.PlayerWon, game.State);
        }

        [Test]
        public void State_UpdatesToOpponentWonWhenPlayerRunsOut()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.Two) },
                new[] { new Card(Suit.Clubs, Rank.King) }
            );

            game.PlayRound();

            Assert.AreEqual(GameState.OpponentWon, game.State);
        }

        [Test]
        public void PotCount_ClearsAfterWinnerDetermined()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.King) },
                new[] { new Card(Suit.Clubs, Rank.Two) }
            );

            game.PlayRound();

            Assert.AreEqual(0, game.PotCount);
        }

        [Test]
        public void PlayRound_RefillsPlayerDeckFromSidePileWhenEmpty()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.Ace), new Card(Suit.Hearts, Rank.King) },
                new[] { new Card(Suit.Clubs, Rank.Two), new Card(Suit.Clubs, Rank.Three) }
            );

            // First round: player wins with Ace > Two
            game.PlayRound();
            Assert.AreEqual(3, game.PlayerCardCount); // 1 card left in deck + 2 won

            // Second round: player deck has 1 card (King), should win again with King > Three
            game.PlayRound();
            Assert.AreEqual(4, game.PlayerCardCount); // All 4 cards now belong to player
        }

        [Test]
        public void PlayRound_RefillsOpponentDeckFromSidePileWhenEmpty()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.Two), new Card(Suit.Hearts, Rank.Three) },
                new[] { new Card(Suit.Clubs, Rank.Ace), new Card(Suit.Clubs, Rank.King) }
            );

            // First round: opponent wins with Ace > Two
            game.PlayRound();
            Assert.AreEqual(3, game.OpponentCardCount); // 1 card left in deck + 2 won

            // Second round: opponent deck has 1 card (King), should win again with King > Three
            game.PlayRound();
            Assert.AreEqual(4, game.OpponentCardCount); // All 4 cards now belong to opponent
        }

        [Test]
        public void PlayRound_MultipleWarScenario()
        {
            var game = CreateGameWithKnownDecks(
                new[] {
                    new Card(Suit.Hearts, Rank.Five),
                    new Card(Suit.Hearts, Rank.Two),
                    new Card(Suit.Hearts, Rank.Three),
                    new Card(Suit.Hearts, Rank.Four),
                    new Card(Suit.Hearts, Rank.Six),
                    new Card(Suit.Hearts, Rank.Seven),
                    new Card(Suit.Hearts, Rank.Eight),
                    new Card(Suit.Hearts, Rank.Nine),
                    new Card(Suit.Hearts, Rank.King)
                },
                new[] {
                    new Card(Suit.Clubs, Rank.Five),
                    new Card(Suit.Clubs, Rank.Two),
                    new Card(Suit.Clubs, Rank.Three),
                    new Card(Suit.Clubs, Rank.Four),
                    new Card(Suit.Clubs, Rank.Six),
                    new Card(Suit.Clubs, Rank.Seven),
                    new Card(Suit.Clubs, Rank.Eight),
                    new Card(Suit.Clubs, Rank.Nine),
                    new Card(Suit.Clubs, Rank.Queen)
                }
            );

            game.PlayRound();

            // After two equal cards (5=5, then 6=6), player wins with K>Q
            Assert.AreEqual(18, game.PlayerCardCount);
            Assert.AreEqual(0, game.OpponentCardCount);
        }

        [Test]
        public void PlayerCardCount_ReturnsCorrectTotalFromDeckAndSidePile()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.Ace) },
                new[] { new Card(Suit.Clubs, Rank.Two), new Card(Suit.Clubs, Rank.Three) }
            );

            game.PlayRound();

            // Player should have won 2 cards, total 2 (0 in deck + 2 in side pile before refill)
            Assert.AreEqual(2, game.PlayerCardCount);
        }

        [Test]
        public void OpponentCardCount_ReturnsCorrectTotalFromDeckAndSidePile()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.Two), new Card(Suit.Hearts, Rank.Three) },
                new[] { new Card(Suit.Clubs, Rank.Ace) }
            );

            game.PlayRound();

            // Opponent should have won 2 cards, total 2 (0 in deck + 2 in side pile before refill)
            Assert.AreEqual(2, game.OpponentCardCount);
        }

        [Test]
        public void PlayRound_WarRequiresFourCards_ThreeDownOneFaceUp()
        {
            var game = CreateGameWithKnownDecks(
                new[] {
                    new Card(Suit.Hearts, Rank.Five),
                    new Card(Suit.Hearts, Rank.Two),
                    new Card(Suit.Hearts, Rank.Three),
                    new Card(Suit.Hearts, Rank.Four),
                    new Card(Suit.Hearts, Rank.King)
                },
                new[] {
                    new Card(Suit.Clubs, Rank.Five),
                    new Card(Suit.Clubs, Rank.Two),
                    new Card(Suit.Clubs, Rank.Three),
                    new Card(Suit.Clubs, Rank.Four),
                    new Card(Suit.Clubs, Rank.Two)
                }
            );

            int initialTotal = game.PlayerCardCount + game.OpponentCardCount;
            game.PlayRound();

            // After war, player wins all cards (2 initial + 8 war cards)
            Assert.AreEqual(initialTotal, game.PlayerCardCount);
        }

        [Test]
        public void GameState_RemainsOngoingDuringNormalPlay()
        {
            var game = CreateGameWithKnownDecks(
                new[] { new Card(Suit.Hearts, Rank.King), new Card(Suit.Hearts, Rank.Ace) },
                new[] { new Card(Suit.Clubs, Rank.Two), new Card(Suit.Clubs, Rank.Three) }
            );

            game.PlayRound();

            Assert.AreEqual(GameState.Ongoing, game.State);
        }

        // Helper method to create a game with known decks for testing
        private CardWarGame CreateGameWithKnownDecks(Card[] playerCards, Card[] opponentCards)
        {
            var game = new CardWarGame();

            var playerDeckField = typeof(CardWarGame).GetField("_playerDeck", BindingFlags.NonPublic | BindingFlags.Instance);
            var opponentDeckField = typeof(CardWarGame).GetField("_opponentDeck", BindingFlags.NonPublic | BindingFlags.Instance);

            var playerDeck = new System.Collections.Generic.Queue<Card>();
            var opponentDeck = new System.Collections.Generic.Queue<Card>();

            foreach (var card in playerCards)
                playerDeck.Enqueue(card);

            foreach (var card in opponentCards)
                opponentDeck.Enqueue(card);

            playerDeckField.SetValue(game, playerDeck);
            opponentDeckField.SetValue(game, opponentDeck);

            return game;
        }
    }
}
