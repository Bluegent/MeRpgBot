using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser.Core
{
    public interface ITimer
    {
        long GetNow();
    }
    public class MockTimer : ITimer
    {
        private long _fakeTime;
        public MockTimer()
        {
            _fakeTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public void ForceTick()
        {
            _fakeTime+=1000;
        }
        public long GetNow()
        {
            return _fakeTime;
        }
    }
}
