using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser.Core
{
    public interface ITimer
    {
        long GetNow();
        long GetFuture(long seconds);
    }
    public class MockTimer : ITimer
    {
        private long _fakeTime;
        public MockTimer()
        {
            _fakeTime = 0; //DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
