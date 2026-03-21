using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardWar.View
{
    [Serializable]
    public struct AnimationTarget{
        public int Player;
        public Transform Target;
    }
    
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private List<AnimationTarget> _playerDeckPositions;
        [SerializeField] private List<AnimationTarget> _playerTargetCardsPositions;
        [SerializeField] private List<AnimationTarget> _playerStackPositions;
        
    }
}