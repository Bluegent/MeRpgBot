using System;
using RPGEngine.Game;

namespace RPGEngine.Core
{
    public interface ITimer
    {
        long GetNow();
        long GetFuture(long seconds);
    }

    public class RealTimer : ITimer
    {
        private long nowTime;

        public RealTimer()
        {
            nowTime = 0;
        }
        public long GetNow()
        {
            nowTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return nowTime;
        }

        public long GetFuture(long seconds)
        {
            return GetNow() + seconds * GameConstants.TickTime;
        }
    }
    public class MockTimer : ITimer
    {
        private long _fakeTime;
        public MockTimer()
        {
            _fakeTime = 0;
        }

        public void ForceTick()
        {
            _fakeTime+=1000;
        }
        public long GetNow()
        {
            return _fakeTime;
        }

        public long GetFuture(long seconds)
        {
            return _fakeTime + seconds * 1000;
        }
    }
}
