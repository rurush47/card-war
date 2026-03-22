using UnityEngine;

namespace CardWar.API
{
    [CreateAssetMenu(fileName = "ResilienceConfig", menuName = "CardWar/ResilienceConfig")]
    public class ResilienceConfig : ScriptableObject
    {
        [Header("Retry")]
        public int MaxRetries = 3;
        public int BaseRetryDelayMs = 500;

        [Header("Timeout")]
        public int TimeoutMs = 2000;
    }
}
