using System;
using System.Threading;
using System.Threading.Tasks;
using CardWar.View.Data;
using CardWar.View.Utils;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CardWar.View
{
    public class CardView : MonoBehaviour, IPoolable
    {
        public event Action ReturnToPool;

        [SerializeField] private Image _image;
        [SerializeField] private VisualConfig _visualConfig;

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

        public void OnPoolRelease()
        {
        }

        public async ValueTask FlipAsync(bool showFace, CancellationToken cancellationToken = default)
        {
            if (_isFaceUp == showFace)
                return;

            _isFaceUp = showFace;

            var midRotation = transform.localEulerAngles + new Vector3(0f, 90f, 0f);
            var endRotation = transform.localEulerAngles;

            await Tween
                .LocalRotation(transform, midRotation, _visualConfig.FlipDuration / 2, _visualConfig.FlipEase)
                .ToValueTask(cancellationToken);

            _image.sprite = showFace ? _cardSprite : _backSprite;

            await Tween
                .LocalRotation(transform, endRotation, _visualConfig.FlipDuration / 2, _visualConfig.FlipEase)
                .ToValueTask(cancellationToken);
        }

        public ValueTask FlipToFaceAsync(CancellationToken cancellationToken = default)
            => FlipAsync(true, cancellationToken);

        public ValueTask FlipToBackAsync(CancellationToken cancellationToken = default)
            => FlipAsync(false, cancellationToken);

        public async Task MoveToPositionAsync(Vector3 targetPosition, float duration,
            CancellationToken cancellationToken = default)
        {
            await Tween
                .Position(transform, targetPosition, duration, _visualConfig.MoveEase)
                .ToValueTask(cancellationToken);
        }

        public async Task HighlightAsync(CancellationToken cancellationToken = default)
        {
            await Tween
                .Scale(transform, Vector3.one * _visualConfig.HighlightScale, _visualConfig.HighlightDuration, _visualConfig.HighlightEase)
                .ToValueTask(cancellationToken);
        }

        public async Task ShakeAsync(float duration, CancellationToken cancellationToken = default)
        {
            await Tween
                .PunchLocalPosition(transform, Vector3.right * _visualConfig.ShakeStrength, duration, frequency: _visualConfig.ShakeFrequency)
                .ToValueTask(cancellationToken);
        }
    }
}
