using System.Threading;
using System.Threading.Tasks;
using CardWar.View.Data;
using CardWar.View.Utils;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace CardWar.View
{
    public class MessageModal : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private VisualConfig _visualConfig;
        private TaskCompletionSource<bool> _clickTcs;

        private void Start()
        {
            _clickTcs = new TaskCompletionSource<bool>();
        }
        
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) 
                return;
            
            _clickTcs?.TrySetResult(true);
            _clickTcs = new TaskCompletionSource<bool>();
        }
        
        public async ValueTask ShowMessage(string message, CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            _messageText.text = message;
            
            await Tween.Scale(transform, Vector3.one, _visualConfig.ScaleDuration).ToValueTask(cancellationToken);
            await _clickTcs.Task;
            await HideMessage(cancellationToken);
        }

        private async ValueTask HideMessage(CancellationToken cancellationToken)
        {
            await Tween.Scale(transform, Vector3.zero, _visualConfig.ScaleDuration).ToValueTask(cancellationToken);
            gameObject.SetActive(false);
        }
    }
}