using System;

namespace Cards
{
    public readonly struct Card : IEquatable<Card>, IComparable<Card>
    {
        public readonly Suit Suit;
        public readonly Rank Rank;

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public bool Equals(Card other)
        {
            return Suit == other.Suit && Rank == other.Rank;
        }

        public override bool Equals(object obj)
        {
            return obj is Card other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Suit, Rank);
        }

        public int CompareTo(Card other)
        {
            int rankComparison = Rank.CompareTo(other.Rank);
            if (rankComparison != 0)
                return rankComparison;
            return Suit.CompareTo(other.Suit);
        }

        public static bool operator ==(Card left, Card right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Card left, Card right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Card left, Card right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Card left, Card right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Card left, Card right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Card left, Card right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}