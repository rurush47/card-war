using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CardWar.API
{
    public class ResilientServerProxy
    {
        private const int _maxRetries = 3;
        private const int _timeoutMs = 2000;
        private const int _baseRetryDelayMs = 500;
        private readonly CardWarServer _server;

        public ResilientServerProxy(CardWarServer server)
        {
            _server = server;
        }

        public ValueTask<Dictionary<string, string>> PostMove(int playerId, CancellationToken cancellationToken)
        {
            return ExecuteWithResilience(
                ct => _server.PostMove(playerId, ct),
                nameof(PostMove),
                cancellationToken);
        }

        public ValueTask<Dictionary<string, string>> GetConfig(CancellationToken cancellationToken)
        {
            return ExecuteWithResilience(
                ct => _server.GetConfig(ct),
                nameof(GetConfig),
                cancellationToken);
        }

        public async ValueTask PostRestart(bool useMiniDeck, CancellationToken cancellationToken)
        {
            await ExecuteWithResilience(
                async ct =>
                {
                    await _server.PostRestart(useMiniDeck, ct);
                    return (Dictionary<string, string>)null;
                },
                nameof(PostRestart),
                cancellationToken);
        }

        private async ValueTask<Dictionary<string, string>> ExecuteWithResilience(
            Func<CancellationToken, ValueTask<Dictionary<string, string>>> action,
            string operationName,
            CancellationToken cancellationToken)
        {
            for (var attempt = 0; attempt <= _maxRetries; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var timeoutCts = new CancellationTokenSource(_timeoutMs);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                try
                {
                    return await action(linkedCts.Token);
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    Debug.LogWarning($"[{operationName}] Request timed out (attempt {attempt + 1}/{_maxRetries + 1})");
                }
                catch (ServerException e)
                {
                    Debug.LogWarning($"[{operationName}] Server error {e.StatusCode}: {e.Message} (attempt {attempt + 1}/{_maxRetries + 1})");
                }

                if (attempt < _maxRetries)
                {
                    var delay = _baseRetryDelayMs * (1 << attempt); // exponential backoff
                    Debug.Log($"[{operationName}] Retrying in {delay}ms...");
                    await Task.Delay(delay, cancellationToken);
                }
            }

            throw new ServerException(0, $"[{operationName}] All {_maxRetries + 1} attempts failed.");
        }
    }
}
