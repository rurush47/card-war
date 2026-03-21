using UnityEngine;

namespace CardWar.View.Data
{
    [CreateAssetMenu(fileName = "VisualConfig", menuName = "CardWar/VisualConfig")]
    public class VisualConfig : ScriptableObject
    {
        public Vector2 DeckOffset = new Vector2(0.5f, 0);
        public Vector2 StackOffset = new Vector2(0.5f, 0.5f);
        public float AddToDeckDuration = 0.2f;
        public float CardPlayDuration = 0.35f;
        public float CardToStackDuration = 0.25f;
        public float CardDelayBetweenSends = 0.05f;
        public float ShuffleMoveDuration = 0.15f;
        public float ShuffleRotationAmount = 10f;
        public float ShuffleOffsetAmount = 2f;
        public int ShuffleIterations = 3;
    }
}