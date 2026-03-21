using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardWar.View.Data;
using CardWar.View.Utils;
using PrimeTween;
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
        [SerializeField] private VisualConfig _visualConfig;
        
        private GameObjectPool<CardView> _cardPool;
        private Dictionary<int, Stack<CardView>> _stacks;
        private Dictionary<int, Stack<CardView>> _decks;
        
        public async void Init(Dictionary<string, string> config)
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
            
            for (int p = 1; p <= 2; p++)
            {
                var parent = _playerStackPositions.Find(t => t.Player == p).Target;
                for (int c = 0; c < maxCards; c++)
                {
                    var newCard = _cardPool.Get();
                    newCard.transform.SetParent(parent, false);
                    newCard.transform.localScale = Vector3.one;
                    newCard.transform.localPosition = Vector2.zero + c*0.5f*_visualConfig.StackOffset; 
                    
                    _stacks[p].Push(newCard);
                }
            }
            
            await RefillDeck(1);
        }

        public async ValueTask RefillDeck(int playerIndex, CancellationToken cancellationToken = default)
        {
            var stack = _stacks[playerIndex];
            var deck = _decks[playerIndex];
            var deckPosition = _playerDeckPositions.Find(t => t.Player == playerIndex).Target;

            var tasks = new List<Task>();

            while (stack.Count > 0)
            {
                var cardView = stack.Pop();
                var cardIndex = deck.Count;

                var task = MoveCardToDeckAsync(cardView, deckPosition, cardIndex, cancellationToken);
                tasks.Add(task);

                deck.Push(cardView);

                await Task.Delay(TimeSpan.FromSeconds(_visualConfig.CardDelayBetweenSends), cancellationToken);
            }

            await Task.WhenAll(tasks);
        }

        private async Task MoveCardToDeckAsync(CardView cardView, Transform deckPosition, int deckIndex, CancellationToken cancellationToken)
        {
            await Tween
                .Position(cardView.transform, deckPosition.position, _visualConfig.AddToDeckDuration, Ease.OutQuad)
                .ToTask(cancellationToken);

            cardView.transform.SetParent(deckPosition, false);
            cardView.transform.localPosition = Vector2.zero + deckIndex * _visualConfig.DeckOffset;
        }

        public async Task ShuffleDeck(int playerIndex, CancellationToken cancellationToken)
        {
            var deck = _decks[playerIndex];
            if (deck.Count == 0) return;

            var deckPosition = _playerDeckPositions.Find(t => t.Player == playerIndex).Target;
            var cards = deck.ToArray();

            for (int iteration = 0; iteration < _visualConfig.ShuffleIterations; iteration++)
            {
                var tweens = new List<Sequence>();

                for (int i = 0; i < cards.Length; i++)
                {
                    var card = cards[i];
                    var randomOffset = new Vector3(
                        UnityEngine.Random.Range(-_visualConfig.ShuffleOffsetAmount, _visualConfig.ShuffleOffsetAmount),
                        UnityEngine.Random.Range(-_visualConfig.ShuffleOffsetAmount, _visualConfig.ShuffleOffsetAmount),
                        0
                    );
                    var randomRotation = UnityEngine.Random.Range(-_visualConfig.ShuffleRotationAmount, _visualConfig.ShuffleRotationAmount);

                    var targetPos = deckPosition.position + randomOffset;
                    var originalPos = deckPosition.position + (Vector3)(i * _visualConfig.DeckOffset);

                    var sequence = Sequence.Create()
                        .Group(Tween.Position(card.transform, targetPos, _visualConfig.ShuffleMoveDuration, Ease.OutQuad))
                        .Group(Tween.Rotation(card.transform, Quaternion.Euler(0, 0, randomRotation), _visualConfig.ShuffleMoveDuration, Ease.OutQuad))
                        .Chain(Tween.Position(card.transform, originalPos, _visualConfig.ShuffleMoveDuration, Ease.InQuad))
                        .Group(Tween.Rotation(card.transform, Quaternion.identity, _visualConfig.ShuffleMoveDuration, Ease.InQuad));

                    tweens.Add(sequence);
                }

                await Task.WhenAll(tweens.Select(t => t.ToTask(cancellationToken)));
            }
        }

        public async Task CardPlayed(int playerIndex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task GameOver(int playerIndex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task WarResolved(int playerIndex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Draw(int playerIndex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task BigPot(int playerIndex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SmallPot(int playerIndex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}