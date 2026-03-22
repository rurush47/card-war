using System;

namespace CardWar.View.Utils.CustomExceptions
{
    public class CardsCountNotValidException : Exception
    {
        public int CardsCount { get; }

        public CardsCountNotValidException(int cardsCount) : base($"Cards count {cardsCount} is not valid. Should be greater than 0.")
        {
            CardsCount = cardsCount;
        }
    }
}