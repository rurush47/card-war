namespace CardWar.Game
{
    public static class DeckDefinitions
    {
        public const string FullDeck =
            "Clubs:Two,Clubs:Three,Clubs:Four,Clubs:Five,Clubs:Six,Clubs:Seven,Clubs:Eight,Clubs:Nine,Clubs:Ten,Clubs:Jack,Clubs:Queen,Clubs:King,Clubs:Ace," +
            "Diamonds:Two,Diamonds:Three,Diamonds:Four,Diamonds:Five,Diamonds:Six,Diamonds:Seven,Diamonds:Eight,Diamonds:Nine,Diamonds:Ten,Diamonds:Jack,Diamonds:Queen,Diamonds:King,Diamonds:Ace," +
            "Hearts:Two,Hearts:Three,Hearts:Four,Hearts:Five,Hearts:Six,Hearts:Seven,Hearts:Eight,Hearts:Nine,Hearts:Ten,Hearts:Jack,Hearts:Queen,Hearts:King,Hearts:Ace," +
            "Spades:Two,Spades:Three,Spades:Four,Spades:Five,Spades:Six,Spades:Seven,Spades:Eight,Spades:Nine,Spades:Ten,Spades:Jack,Spades:Queen,Spades:King,Spades:Ace";

        // 8 cards (4 per player) — game ends in a few rounds; useful for testing GameOver and Draw
        public const string MiniDeck =
            "Hearts:Ace,Hearts:King,Hearts:Queen,Hearts:Jack," +
            "Clubs:Two,Clubs:Three,Clubs:Four,Clubs:Five";
    }
}