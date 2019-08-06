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
        private long fakeTime;
        public MockTimer()
        {
            fakeTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public void forceTick()
        {
            ++fakeTime;
        }
        public long GetNow()
        {
            return fakeTime;
        }
    }
}
