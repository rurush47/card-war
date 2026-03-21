using System;
using Cards;
using UnityEngine;

namespace CardWar.View.Data
{
    [Serializable]
    public struct CardSpritePair
    {
        public Card Card;
        public Sprite Sprite;
    }
}