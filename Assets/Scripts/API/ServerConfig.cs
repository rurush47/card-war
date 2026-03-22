using UnityEngine;

namespace CardWar.API
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "CardWar/ServerConfig")]
    public class ServerConfig : ScriptableObject
    {
        [Header("Response Delay")]
        public int ResponseDelayMs = 300;
        public int SlowResponseDelayMs = 3000;

        [Header("Error Simulation")]
        [Range(0f, 1f)] public float ErrorRate = 0.1f;
        [Range(0f, 1f)] public float SlowResponseRate = 0.15f;
    }
}
