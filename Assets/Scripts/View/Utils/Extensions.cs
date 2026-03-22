using System.Threading;
using System.Threading.Tasks;
using PrimeTween;

namespace CardWar.View.Utils
{
    public static class Extensions
    {
        public static async Task ToTask(this Tween tween, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                tween.Stop();
                cancellationToken.ThrowIfCancellationRequested();
            }

            await using (cancellationToken.Register(tween.Stop))
            {
                await tween;
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        public static async ValueTask ToValueTask(this Tween tween, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                tween.Stop();
                cancellationToken.ThrowIfCancellationRequested();
            }

            await using (cancellationToken.Register(tween.Stop))
            {
                await tween;
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
