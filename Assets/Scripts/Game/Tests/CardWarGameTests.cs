using System.Collections.Generic;
using System.Linq;
using Cards;
using NUnit.Framework;

namespace CardWar.Game.Tests
{
    public class CardWarGameTests
    {
        // Helper: play a full round (player 1 then player 2) and return player 2's commands
        private static Dictionary<string, string> PlayFullRound(CardWarGame game)
        {
            game.PlayRound(1);
            return game.PlayRound(2);
        }

        private static List<Card> Deck(params (Suit suit, Rank rank)[] cards) =>
            cards.Select(c => new Card(c.suit, c.rank)).ToList();

        [Test]
        public void InitialState_EachPlayerHas26Cards()
        {
            var game = new CardWarGame();
            Assert.AreEqual(26, game.PlayerCardCount(1));
            Assert.AreEqual(26, game.PlayerCardCount(2));
        }

        [Test]
        public void InitialState_PotIsEmpty()
        {
            var game = new CardWarGame();
            Assert.AreEqual(0, game.PotCount);
        }

        [Test]
        public void PlayRound_WrongPlayerFirst_ThrowsArgumentException()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Ace)),
                Deck((Suit.Clubs, Rank.Two))
            );

            Assert.Throws<System.ArgumentException>(() => game.PlayRound(2));
        }

        [Test]
        public void PlayRound_Player1First_ReturnsCardPlayedForPlayer1()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Ace)),
                Deck((Suit.Clubs, Rank.Two))
            );

            var commands = game.PlayRound(1);

            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("1:Hearts:Ace", commands["CardPlayed"]);
        }

        [Test]
        public void PlayRound_Player2Second_ReturnsCardPlayedForPlayer2()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Ace)),
                Deck((Suit.Clubs, Rank.Two))
            );
            game.PlayRound(1);

            var commands = game.PlayRound(2);

            Assert.IsTrue(commands["CardPlayed"] == "2:Clubs:Two");
        }

        [Test]
        public void PlayRound_Player1HigherRank_WarResolvedForPlayer1()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Ace)),
                Deck((Suit.Clubs, Rank.Two))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands["WarResolved"] == "1");
        }

        [Test]
        public void PlayRound_Player2HigherRank_WarResolvedForPlayer2()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Two)),
                Deck((Suit.Clubs, Rank.Ace))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands["WarResolved"] == "2");
        }

        [Test]
        public void PlayRound_Player1Wins_GetsAllPotCards()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Ace)),
                Deck((Suit.Clubs, Rank.Two))
            );
            var totalBefore = game.PlayerCardCount(1) + game.PlayerCardCount(2);

            PlayFullRound(game);

            Assert.AreEqual(0, game.PotCount);
            var totalAfter = game.PlayerCardCount(1) + game.PlayerCardCount(2);
            Assert.AreEqual(totalBefore, totalAfter);
        }

        [Test]
        public void PlayRound_EqualRanks_TriggersBigPotAndSmallPot()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.King), (Suit.Clubs, Rank.Two), (Suit.Diamonds, Rank.Two), (Suit.Spades, Rank.Two), (Suit.Hearts, Rank.Ace)),
                Deck((Suit.Clubs, Rank.King), (Suit.Hearts, Rank.Three), (Suit.Diamonds, Rank.Three), (Suit.Spades, Rank.Three), (Suit.Clubs, Rank.Two))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands.Any(c => c.Key == "BigPot"));
            Assert.IsTrue(commands.Any(c => c.Key == "SmallPot"));
        }

        [Test]
        public void PlayRound_War_Player1HigherFaceUp_Player1WinsAll()
        {
            // Tied on Kings; face-up: Ace vs Two → player 1 wins
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.King), (Suit.Clubs, Rank.Two), (Suit.Diamonds, Rank.Two), (Suit.Spades, Rank.Two), (Suit.Hearts, Rank.Ace)),
                Deck((Suit.Clubs, Rank.King), (Suit.Hearts, Rank.Three), (Suit.Diamonds, Rank.Three), (Suit.Spades, Rank.Three), (Suit.Clubs, Rank.Two))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands["WarResolved"] == "1");
            Assert.AreEqual(0, game.PotCount);
        }

        [Test]
        public void PlayRound_War_Player1TooFewCards_GameOverPlayer2()
        {
            // Player 1 has only 1 card (the tied card), can't supply 4 for war
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.King)),
                Deck((Suit.Clubs, Rank.King), (Suit.Hearts, Rank.Two), (Suit.Diamonds, Rank.Two), (Suit.Spades, Rank.Two), (Suit.Clubs, Rank.Two))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands["GameOver"] == "2");
        }

        [Test]
        public void PlayRound_War_Player2TooFewCards_GameOverPlayer1()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.King), (Suit.Clubs, Rank.Two), (Suit.Diamonds, Rank.Two), (Suit.Spades, Rank.Two), (Suit.Clubs, Rank.Three)),
                Deck((Suit.Clubs, Rank.King))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands["GameOver"] == "1");
        }

        [Test]
        public void PlayRound_War_BothTooFewCards_Draw()
        {
            // Both players have only 1 card (the tied card), can't supply 4 for war
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.King)),
                Deck((Suit.Clubs, Rank.King))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands["Draw"] == "0");
        }

        [Test]
        public void PlayRound_Player1OutOfCards_GameOverPlayer2()
        {
            // Player 1 has 1 card, player 2 wins it, then player 1 has none
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Two)),
                Deck((Suit.Clubs, Rank.Ace))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands["GameOver"] == "2");
        }

        [Test]
        public void PlayRound_Player2OutOfCards_GameOverPlayer1()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Ace)),
                Deck((Suit.Clubs, Rank.Two))
            );

            var commands = PlayFullRound(game);

            Assert.IsTrue(commands["GameOver"] == "1");
        }

        [Test]
        public void TotalCardCount_AlwaysConserved_AfterMultipleRounds()
        {
            var game = new CardWarGame();
            const int total = CardWarGame.MaxCards;

            for (int i = 0; i < 10; i++)
            {
                game.PlayRound(1);
                game.PlayRound(2);
                Assert.AreEqual(total, game.PlayerCardCount(1) + game.PlayerCardCount(2) + game.PotCount);
            }
        }

        [Test]
        public void PlayRound_AfterResolution_Player1GoesFirst()
        {
            var game = new CardWarGame(
                Deck((Suit.Hearts, Rank.Ace), (Suit.Hearts, Rank.King)),
                Deck((Suit.Clubs, Rank.Two), (Suit.Clubs, Rank.Three))
            );

            PlayFullRound(game);

            // After resolution, player 1 must go first again (not player 2)
            Assert.Throws<System.ArgumentException>(() => game.PlayRound(2));
            Assert.DoesNotThrow(() => game.PlayRound(1));
        }
    }
}
