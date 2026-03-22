using PrimeTween;
using UnityEngine;

namespace CardWar.View.Data
{
    [CreateAssetMenu(fileName = "VisualConfig", menuName = "CardWar/VisualConfig")]
    public class VisualConfig : ScriptableObject
    {
        [Tooltip("Per-card offset in the deck, as a fraction of screen width")]
        public Vector2 StackOffset = new Vector2(0.5f, 0.5f);
        public float AddToDeckDuration = 0.2f;
        public float CardPlayDuration = 0.35f;
        public float CardToStackDuration = 0.25f;
        public float CardDelayBetweenSends = 0.05f;
        public float WarResolutionDelay = 0.7f;
        public float ShuffleDuration = 0.3f;
        public float ScreenOffset = 300;
        public int PotScale = 150;

        [Header("Card")]
        public float FlipDuration = 0.3f;
        public Ease FlipEase = Ease.OutCubic;
        public float HighlightScale = 1.3f;
        public float HighlightDuration = 0.2f;
        public Ease HighlightEase = Ease.OutQuad;
        public float ShakeStrength = 10f;
        public int ShakeFrequency = 10;
        public Ease MoveEase = Ease.OutQuad;
    }
}