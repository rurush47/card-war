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
    }
}