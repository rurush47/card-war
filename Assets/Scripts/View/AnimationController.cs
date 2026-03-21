using System;
using System.Collections.Generic;
using CardWar.API;
using CardWar.View.Utils;
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
        [Header("UI Positions")]
        [SerializeField] private List<AnimationTarget> _playerDeckPositions;
        [SerializeField] private List<AnimationTarget> _playerTargetCardsPositions;
        [SerializeField] private List<AnimationTarget> _playerStackPositions;
        [Header("Refs")]
        [SerializeField] private CardView _cardViewPrefab;
        
        private GameObjectPool<CardView> _cardPool;
        private Dictionary<int, Stack<CardView>> _stacks;
        private Dictionary<int, Stack<CardView>> _decks;
        
        public void Init(Dictionary<string, string> config, StateResponse initialGameState)
        {
            int.TryParse(config["max_cards"], out var maxCards);
            
            _cardPool = new GameObjectPool<CardView>(_cardViewPrefab, maxCards);
            _stacks = new Dictionary<int, Stack<CardView>>
            {
                { 1, new Stack<CardView>(maxCards) },
                { 2, new Stack<CardView>(maxCards) },
            };
            _decks = new Dictionary<int, Stack<CardView>>
            {
                { 1, new Stack<CardView>(maxCards) },
                { 2, new Stack<CardView>(maxCards) },
            };
            
            var offset = new Vector2(1, 1);
            
            for (int p = 1; p <= 2; p++)
            {
                var parent = _playerStackPositions.Find(t => t.Player == p).Target;
                for (int c = 0; c < maxCards; c++)
                {
                    var newCard = _cardPool.Get();
                    newCard.transform.SetParent(parent, false);
                    newCard.transform.localScale = Vector3.one;
                    newCard.transform.localPosition = Vector2.zero + c*offset/2; 
                    
                    _stacks[p].Push(newCard);
                }
            }
        }

        public void AddCardToDeck()
        {
            
        }
    }
}