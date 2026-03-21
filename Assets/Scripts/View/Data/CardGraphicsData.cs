using System.Collections.Generic;
using UnityEngine;

namespace CardWar.View.Data
{
    [CreateAssetMenu(fileName = "CardGraphicsData", menuName = "CardWar/CardGraphicsData")]
    public class CardGraphicsData : ScriptableObject
    {
        public List<CardSpritePair> CardGraphics;
    }
}