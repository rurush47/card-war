using NUnit.Framework;

namespace Cards.Tests
{
    public class CardTests
    {
        [Test]
        public void Equality_SameCards_ReturnsTrue()
        {
            var card1 = new Card(Suit.Hearts, Rank.Ace);
            var card2 = new Card(Suit.Hearts, Rank.Ace);

            Assert.IsTrue(card1 == card2);
            Assert.IsTrue(card1.Equals(card2));
            Assert.IsFalse(card1 != card2);
        }

        [Test]
        public void Equality_DifferentRank_ReturnsFalse()
        {
            var card1 = new Card(Suit.Hearts, Rank.Ace);
            var card2 = new Card(Suit.Hearts, Rank.King);

            Assert.IsFalse(card1 == card2);
            Assert.IsFalse(card1.Equals(card2));
            Assert.IsTrue(card1 != card2);
        }

        [Test]
        public void Equality_DifferentSuit_ReturnsFalse()
        {
            var card1 = new Card(Suit.Hearts, Rank.Ace);
            var card2 = new Card(Suit.Spades, Rank.Ace);

            Assert.IsFalse(card1 == card2);
            Assert.IsFalse(card1.Equals(card2));
            Assert.IsTrue(card1 != card2);
        }

        [Test]
        public void Comparison_HigherRank_ReturnsGreater()
        {
            var lowerCard = new Card(Suit.Hearts, Rank.Two);
            var higherCard = new Card(Suit.Hearts, Rank.Ace);

            Assert.IsTrue(higherCard > lowerCard);
            Assert.IsTrue(lowerCard < higherCard);
            Assert.IsTrue(higherCard >= lowerCard);
            Assert.IsTrue(lowerCard <= higherCard);
        }

        [Test]
        public void Comparison_SameRankDifferentSuit_ComparesBySuit()
        {
            var clubs = new Card(Suit.Clubs, Rank.Ace);
            var diamonds = new Card(Suit.Diamonds, Rank.Ace);
            var hearts = new Card(Suit.Hearts, Rank.Ace);
            var spades = new Card(Suit.Spades, Rank.Ace);

            Assert.IsTrue(clubs < diamonds);
            Assert.IsTrue(diamonds < hearts);
            Assert.IsTrue(hearts < spades);
        }

        [Test]
        public void Comparison_SameCard_ReturnsEqual()
        {
            var card1 = new Card(Suit.Hearts, Rank.King);
            var card2 = new Card(Suit.Hearts, Rank.King);

            Assert.IsTrue(card1 <= card2);
            Assert.IsTrue(card1 >= card2);
            Assert.IsFalse(card1 < card2);
            Assert.IsFalse(card1 > card2);
        }

        [Test]
        public void CompareTo_HigherRank_ReturnsPositive()
        {
            var lowerCard = new Card(Suit.Hearts, Rank.Five);
            var higherCard = new Card(Suit.Hearts, Rank.Ten);

            Assert.Greater(higherCard.CompareTo(lowerCard), 0);
            Assert.Less(lowerCard.CompareTo(higherCard), 0);
        }

        [Test]
        public void CompareTo_SameCard_ReturnsZero()
        {
            var card1 = new Card(Suit.Diamonds, Rank.Queen);
            var card2 = new Card(Suit.Diamonds, Rank.Queen);

            Assert.AreEqual(0, card1.CompareTo(card2));
        }

        [Test]
        public void GetHashCode_SameCards_ReturnsSameHash()
        {
            var card1 = new Card(Suit.Spades, Rank.Jack);
            var card2 = new Card(Suit.Spades, Rank.Jack);

            Assert.AreEqual(card1.GetHashCode(), card2.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentCards_ReturnsDifferentHash()
        {
            var card1 = new Card(Suit.Spades, Rank.Jack);
            var card2 = new Card(Suit.Hearts, Rank.Jack);

            Assert.AreNotEqual(card1.GetHashCode(), card2.GetHashCode());
        }
    }
}
