using System.Threading;
using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CardWar.View
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _flipDuration = 0.3f;
        [SerializeField] private Ease _flipEase = Ease.OutCubic;

        private Sprite _backSprite;
        private Sprite _cardSprite;
        private bool _isFaceUp;

        public void Initialize(Sprite cardSprite, Sprite backSprite)
        {
            if (_image == null)
                _image = GetComponent<Image>();

            _cardSprite = cardSprite;
            _backSprite = backSprite;
            _isFaceUp = false;
            _image.sprite = _backSprite;
        }

        /// <summary>
        /// Flips the card with a smooth rotation animation
        /// </summary>
        /// <param name="showFace">True to show card face, false to show back</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        public async ValueTask FlipAsync(bool showFace, CancellationToken cancellationToken = default)
        {
            if (_isFaceUp == showFace)
                return;

            _isFaceUp = showFace;

            var startRotation = transform.localEulerAngles;
            var endRotation = startRotation;
            endRotation.y = showFace ? 180f : 0f;

            // Create sequence: shrink -> change sprite -> grow
            await Sequence.Create()
                .Chain(Tween.LocalRotation(transform, endRotation, _flipDuration / 2, _flipEase))
                .Group(Tween.Scale(transform, new Vector3(0f, 1f, 1f), _flipDuration / 2, _flipEase));

            _image.sprite = showFace ? _cardSprite : _backSprite;
            
            await Tween.Scale(transform, Vector3.one, _flipDuration / 2, _flipEase);
        }

        /// <summary>
        /// Flips the card to show its face
        /// </summary>
        public ValueTask FlipToFaceAsync(CancellationToken cancellationToken = default)
            => FlipAsync(true, cancellationToken);

        /// <summary>
        /// Flips the card to show its back
        /// </summary>
        public ValueTask FlipToBackAsync(CancellationToken cancellationToken = default)
            => FlipAsync(false, cancellationToken);

        /// <summary>
        /// Moves the card to a target position with animation
        /// </summary>
        public async Task MoveToPositionAsync(Vector3 targetPosition, float duration = 0.5f,
            Ease ease = Ease.OutQuad, CancellationToken cancellationToken = default)
        {
            await Tween.Position(transform, targetPosition, duration, ease);
        }

        /// <summary>
        /// Moves the card to a target with animation
        /// </summary>
        public async Task MoveToTargetAsync(Transform target, float duration = 0.5f,
            Ease ease = Ease.OutQuad, CancellationToken cancellationToken = default)
        {
            await Tween.Position(transform, target.position, duration, ease);
        }

        /// <summary>
        /// Scales the card with animation
        /// </summary>
        public async Task ScaleToAsync(Vector3 scale, float duration = 0.3f,
            Ease ease = Ease.OutBack, CancellationToken cancellationToken = default)
        {
            await Tween.Scale(transform, scale, duration, ease);
        }

        /// <summary>
        /// Plays a "pop in" animation when the card appears
        /// </summary>
        public async Task PopInAsync(CancellationToken cancellationToken = default)
        {
            transform.localScale = Vector3.zero;
            await Tween.Scale(transform, Vector3.one, 0.3f, Ease.OutBack);
        }

        /// <summary>
        /// Plays a "pop out" animation before the card disappears
        /// </summary>
        public async Task PopOutAsync(CancellationToken cancellationToken = default)
        {
            await Tween.Scale(transform, Vector3.zero, 0.2f, Ease.InBack);
        }

        /// <summary>
        /// Plays a subtle hover animation
        /// </summary>
        public async Task PlayHoverEffectAsync(CancellationToken cancellationToken = default)
        {
            await Tween.PunchScale(transform, Vector3.one * 0.1f, 0.3f);
        }

        /// <summary>
        /// Highlights the card (scales up slightly)
        /// </summary>
        public async Task HighlightAsync(float scale = 1.1f, CancellationToken cancellationToken = default)
        {
            await Tween.Scale(transform, Vector3.one * scale, 0.2f, Ease.OutQuad);
        }

        /// <summary>
        /// Removes highlight (returns to normal scale)
        /// </summary>
        public async Task UnhighlightAsync(CancellationToken cancellationToken = default)
        {
            await Tween.Scale(transform, Vector3.one, 0.2f, Ease.OutQuad);
        }

        /// <summary>
        /// Shakes the card (useful for "war" declaration or invalid action)
        /// </summary>
        public async Task ShakeAsync(float strength = 10f, float duration = 0.3f,
            CancellationToken cancellationToken = default)
        {
            await Tween.PunchLocalPosition(transform, Vector3.right * strength, duration, frequency: 10);
        }

        /// <summary>
        /// Combines move and flip animations in sequence
        /// </summary>
        public async Task MoveAndFlipAsync(Vector3 targetPosition, bool showFace,
            float moveDuration = 0.5f, CancellationToken cancellationToken = default)
        {
            await MoveToPositionAsync(targetPosition, moveDuration, cancellationToken: cancellationToken);
            await FlipAsync(showFace, cancellationToken);
        }

        /// <summary>
        /// Combines move and flip animations in parallel
        /// </summary>
        public async Task MoveAndFlipParallelAsync(Vector3 targetPosition, bool showFace,
            float moveDuration = 0.5f, CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(
                MoveToPositionAsync(targetPosition, moveDuration, cancellationToken: cancellationToken),
                FlipAsync(showFace, cancellationToken).AsTask()
            );
        }
    }
}