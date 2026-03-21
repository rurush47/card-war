using System;

namespace CardWar.View.Utils
{
    public interface IPoolable
    {
        void OnPoolRelease();
        public event Action ReturnToPool;
    }
}