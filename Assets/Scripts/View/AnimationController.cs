using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardWar.View.Data;
using CardWar.View.Utils;
using Cards;
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
        [SerializeField] private CardGraphicsData _cardGraphicsData;
        [SerializeField] private Sprite _cardBackSprite;
        
        private GameObjectPool<CardView> _cardPool;
        private Dictionary<int, Stack<CardView>> _stacks;
        private Dictionary<int, Stack<CardView>> _decks;
        private Dictionary<Card, Sprite> _cardSprites;
        private readonly List<CardView> _pot = new();
        
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
            _cardSprites = _cardGraphicsData.CardGraphics.ToDictionary(p => p.Card, p => p.Sprite);
            
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
            await RefillDeck(2);
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

                var task = MoveCardToDeckAsync(cardView, deckPosition, cardIndex, playerIndex == 1 ? 1 : -1, cancellationToken);
                tasks.Add(task);

                deck.Push(cardView);

                await Task.Delay(TimeSpan.FromSeconds(_visualConfig.CardDelayBetweenSends), cancellationToken);
            }

            await Task.WhenAll(tasks);
        }

        private async Task MoveCardToDeckAsync(CardView cardView, Transform deckPosition, int deckIndex, int dir, CancellationToken cancellationToken)
        {
            await Tween
                .Position(cardView.transform, deckPosition.position, _visualConfig.AddToDeckDuration, Ease.OutQuad)
                .ToTask(cancellationToken);

            cardView.transform.SetParent(deckPosition, false);
            cardView.transform.localPosition = Vector2.zero + dir*deckIndex * _visualConfig.DeckOffset;
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

        public async Task PlayCard(int playerIndex, Card card, CancellationToken cancellationToken)
        {
            if (_decks[playerIndex].Count == 0) return;

            var cardView = _decks[playerIndex].Pop();
            var sprite = _cardSprites[card];
            cardView.Initialize(sprite, _cardBackSprite);

            var targetTransform = _playerTargetCardsPositions.Find(t => t.Player == playerIndex).Target;

            await Task.WhenAll(
                cardView.MoveToPositionAsync(targetTransform.position, _visualConfig.CardPlayDuration, cancellationToken: cancellationToken),
                cardView.FlipToFaceAsync(cancellationToken).AsTask()
            );

            cardView.transform.SetParent(targetTransform, false);
            cardView.transform.localPosition = Vector3.zero;
            _pot.Add(cardView);
        }

        public async Task GameOver(int playerIndex, CancellationToken cancellationToken)
        {
            if (playerIndex == 0) return;
            var stack = _stacks[playerIndex];
            if (stack.Count > 0)
                await stack.Peek().HighlightAsync(1.3f, cancellationToken);
        }

        public async Task WarResolved(int playerIndex, CancellationToken cancellationToken)
        {
            var stackParent = _playerStackPositions.Find(t => t.Player == playerIndex).Target;
            var stack = _stacks[playerIndex];

            var potCards = _pot.ToList();
            _pot.Clear();

            var tasks = new List<Task>();
            for (int i = 0; i < potCards.Count; i++)
            {
                var stackIndex = stack.Count + i;
                tasks.Add(MovePotCardToStack(potCards[i], stackIndex, stackParent, cancellationToken));
            }

            foreach (var card in potCards)
                stack.Push(card);

            await Task.WhenAll(tasks);
        }

        private async Task MovePotCardToStack(CardView card, int stackIndex, Transform stackParent, CancellationToken cancellationToken)
        {
            var worldPos = stackParent.TransformPoint((Vector3)(stackIndex * _visualConfig.StackOffset));

            await Task.WhenAll(
                card.MoveToPositionAsync(worldPos, _visualConfig.CardToStackDuration, cancellationToken: cancellationToken),
                card.FlipToBackAsync(cancellationToken).AsTask()
            );

            card.transform.SetParent(stackParent, false);
            card.transform.localPosition = (Vector3)(stackIndex * _visualConfig.StackOffset);
        }

        public async Task Draw(int playerIndex, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            for (int p = 1; p <= 2; p++)
            {
                if (_decks[p].Count > 0)
                    tasks.Add(_decks[p].Peek().ShakeAsync(cancellationToken: cancellationToken));
                if (_stacks[p].Count > 0)
                    tasks.Add(_stacks[p].Peek().ShakeAsync(cancellationToken: cancellationToken));
            }
            await Task.WhenAll(tasks);
        }

        public async Task BigPot(int _, CancellationToken cancellationToken)
        {
            for (int i = 0; i < 3; i++)
            {
                var tasks = new List<Task>();
                for (int p = 1; p <= 2; p++)
                {
                    if (_decks[p].Count == 0) continue;

                    var card = _decks[p].Pop();
                    var targetTransform = _playerTargetCardsPositions.Find(t => t.Player == p).Target;
                    var potIndex = _pot.Count;
                    var dir = p % 2 == 0 ? -1 : 1;
                    var worldPos = targetTransform.TransformPoint(dir*potIndex * _visualConfig.DeckOffset/5);

                    _pot.Add(card);
                    tasks.Add(MoveCardToPotAsync(card, worldPos, targetTransform, potIndex, cancellationToken));
                }

                await Task.WhenAll(tasks);
                await Task.Delay(TimeSpan.FromSeconds(_visualConfig.CardDelayBetweenSends), cancellationToken);
            }
        }

        public async Task SmallPot(int _, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            for (var p = 1; p <= 2; p++)
            {
                if (_decks[p].Count == 0) continue;

                var card = _decks[p].Pop();
                var targetTransform = _playerTargetCardsPositions.Find(t => t.Player == p).Target;
                var potIndex = _pot.Count;
                var worldPos = targetTransform.TransformPoint((Vector3)(potIndex * _visualConfig.DeckOffset));

                _pot.Add(card);
                tasks.Add(Task.WhenAll(
                    card.MoveToPositionAsync(worldPos, _visualConfig.CardPlayDuration, cancellationToken: cancellationToken),
                    card.FlipToFaceAsync(cancellationToken).AsTask()
                ).ContinueWith(_ =>
                {
                    card.transform.SetParent(targetTransform, false);
                    card.transform.localPosition = potIndex * _visualConfig.DeckOffset;
                }, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.FromCurrentSynchronizationContext()));
            }
            await Task.WhenAll(tasks);
        }

        private async Task MoveCardToPotAsync(CardView card, Vector3 worldPos, Transform parent, int localIndex, CancellationToken cancellationToken)
        {
            await card.MoveToPositionAsync(worldPos, _visualConfig.CardPlayDuration, cancellationToken: cancellationToken);
            card.transform.SetParent(parent, false);
            card.transform.localPosition = localIndex * _visualConfig.DeckOffset;
        }
    }
}