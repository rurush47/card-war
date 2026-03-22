using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CardWar.API
{
    public class ResilientServerProxy
    {
        public event Action<bool> ConnectionTroubleChanged;

        private readonly CardWarServer _server;
        private readonly ResilienceConfig _config;
        private readonly GameLogger _logger = new("Resilience");
        private bool _hasConnectionTrouble;

        public ResilientServerProxy(CardWarServer server, ResilienceConfig config)
        {
            _server = server;
            _config = config;
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
            var maxAttempts = _config.MaxRetries + 1;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var timeoutCts = new CancellationTokenSource(_config.TimeoutMs);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                try
                {
                    var result = await action(linkedCts.Token);
                    SetConnectionTrouble(false);
                    return result;
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning($"{operationName} timed out (attempt {attempt + 1}/{maxAttempts})");
                }
                catch (ServerException e)
                {
                    _logger.LogWarning($"{operationName} server error {e.StatusCode}: {e.Message} (attempt {attempt + 1}/{maxAttempts})");
                }

                SetConnectionTrouble(true);

                if (attempt < _config.MaxRetries)
                {
                    var delay = _config.BaseRetryDelayMs * (1 << attempt);
                    _logger.Log($"{operationName} retrying in {delay}ms...");
                    await Task.Delay(delay, cancellationToken);
                }
            }

            throw new ServerException(0, $"{operationName} failed after {maxAttempts} attempts.");
        }

        private void SetConnectionTrouble(bool value)
        {
            if (_hasConnectionTrouble == value)
                return;

            _hasConnectionTrouble = value;
            ConnectionTroubleChanged?.Invoke(value);
        }
    }
}
