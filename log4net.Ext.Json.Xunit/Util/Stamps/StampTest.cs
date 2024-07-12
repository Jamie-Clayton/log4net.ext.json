using System;
using Xunit;
using FluentAssertions;
using log4net.Core;
using System.Diagnostics;

namespace log4net.Ext.Json.Xunit.Util.Stamps
{
    public class StampTest
    {
        protected virtual LoggingEvent CreateLoggingEvent(Type logger = null, Level level = null, object message = null, Exception exception = null)
        {
            logger = logger ?? GetType();
            level = level ?? Level.Info;
            var le = new LoggingEvent(logger, null, logger.FullName, level, message, exception);
            return le;
        }

        [Fact]
        public void RegularStamp()
        {
            var le = CreateLoggingEvent();
            var stamp = new log4net.Util.Stamps.Stamp();
            var stamp2 = new log4net.Util.Stamps.Stamp() { Name = "stamp2" };
            stamp.StampEvent(le);
            stamp2.StampEvent(le);

            le.Properties["stamp"].Should().NotBeNull(@"Properties[""stamp""]");
            le.Properties["stamp2"].Should().NotBeNull(@"Properties[""stamp2""]");
            le.Properties["stamp"].Should().NotBe(le.Properties["stamp2"], @"stamp!=stamp2");
        }

        [Fact]
        public void TimeStamp()
        {
            var le = CreateLoggingEvent();
            var stamp = new log4net.Util.Stamps.TimeStamp();
            var stamp2 = new log4net.Util.Stamps.TimeStamp() { Name = "stamp2" };
            stamp.StampEvent(le);
            stamp2.StampEvent(le);
            le.Properties["stamp"].Should().NotBeNull(@"Properties[""stamp""]");
            le.Properties["stamp2"].Should().NotBeNull(@"Properties[""stamp2""]");
            ((double)le.Properties["stamp2"]).Should().BeGreaterThan((double)le.Properties["stamp"], because: "stamp2 > stamp");
        }

        [Fact]
        public void TimeStampRound()
        {
            var le = CreateLoggingEvent();
            var stamp = new log4net.Util.Stamps.TimeStamp() { Round = true };
            stamp.StampEvent(le);
            var value = Convert.ToString(le.Properties["stamp"]);
            var time = 0L;
            long.TryParse(value, out time).Should().BeTrue("{0} must be a long when Round=true", value);
        }

        [Fact]
        public void SequenceStamp()
        {
            var le = CreateLoggingEvent();
            var stamp = new log4net.Util.Stamps.SequenceStamp();
            var stamp2 = new log4net.Util.Stamps.SequenceStamp() { Name = "stamp2" };
            stamp.StampEvent(le);
            stamp2.StampEvent(le);

            le.Properties["stamp"].Should().NotBeNull(@"Properties[""stamp""]");
            le.Properties["stamp2"].Should().NotBeNull(@"Properties[""stamp2""]");
            ((long)le.Properties["stamp2"]).Should().BeGreaterThan((long)le.Properties["stamp"], because: "stamp2 > stamp");
        }

        [Fact]
        public void ProcessIdStamp()
        {
            var le = CreateLoggingEvent();
            var stamp = new log4net.Util.Stamps.ProcessIdStamp();
            var stamp2 = new log4net.Util.Stamps.ProcessIdStamp() { Name = "stamp2" };
            stamp.StampEvent(le);
            stamp2.StampEvent(le);

            le.Properties["stamp"].Should().NotBeNull(@"Properties[""stamp""]");
            le.Properties["stamp2"].Should().NotBeNull(@"Properties[""stamp2""]");
            ((int)le.Properties["stamp2"]).Should().Be((int)le.Properties["stamp"], because: "stamp2 == stamp");
            ((int)le.Properties["stamp"]).Should().Be(Process.GetCurrentProcess().Id, because: "stamp2 == stamp");
        }

        [Fact]
        public void ValueStamp()
        {
            var le = CreateLoggingEvent();
            var stamp = new log4net.Util.Stamps.ValueStamp() { Value = "A" };
            var stamp2 = new log4net.Util.Stamps.ValueStamp() { Name = "stamp2", Value = "B" };
            stamp.StampEvent(le);
            stamp2.StampEvent(le);

            le.Properties["stamp"].Should().NotBeNull(@"Properties[""stamp""]");
            le.Properties["stamp2"].Should().NotBeNull(@"Properties[""stamp2""]");
            le.Properties["stamp"].Should().Be("A", @"stamp A");
            le.Properties["stamp2"].Should().Be("B", @"stamp2 B");
        }
    }
}
