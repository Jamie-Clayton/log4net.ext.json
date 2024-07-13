using System.Linq;
using FluentAssertions;
using log4net.Ext.Json.Xunit.General;

namespace log4net.Ext.Json.Xunit.Util.Stamps
{
    public class StampingLogger_Basic : RepoTest
    {
        protected override string GetConfig()
        {
            return @"
                <log4net>                    
                  <loggerFactory type=""log4net.Util.Stamps.StampingLoggerFactory, log4net.Ext.Json"">
                  </loggerFactory>
                  <root>
                    <level value=""ALL"" />
                  </root>
                </log4net>";
        }

		protected override void RunTestLog(log4net.ILog log)
        {
            log.Info("Hola!");

            var events = GetEvents(log.Logger);
            events.Length.Should().Be(1, "events Count");

            var le = events.Single();

            le.Should().NotBeNull(because: "loggingevent");
            le.Properties.Should().NotBeNull(because: "loggingevent Properties");
            le.Properties["stamp"].Should().NotBeNull(because: "loggingevent Properties has stamp");
        }
    }
}
