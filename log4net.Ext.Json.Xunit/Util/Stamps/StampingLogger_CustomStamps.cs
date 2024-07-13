using System.Linq;
using FluentAssertions;
using log4net.Ext.Json.Xunit.General;
using Newtonsoft.Json.Serialization;

namespace log4net.Ext.Json.Xunit.Util.Stamps
{
    public class StampingLogger_CustomStamps : RepoTest
    {
        protected override string GetConfig()
        {
            return @"
                <log4net>                    
                  <loggerFactory type=""log4net.Util.Stamps.StampingLoggerFactory, log4net.Ext.Json"">
                    <stamp type=""log4net.Util.Stamps.ValueStamp, log4net.Ext.Json"">
                      <name>stamp.value</name>
                      <value>CustomValue</value>
                    </stamp>
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
            le.Properties.Count.Should().Be(1, "loggingevent Properties count");
            le.Properties["stamp.value"].Should().NotBeNull("loggingevent Properties has stamp");
            le.Properties["stamp.value"].Should().Be("CustomValue", "loggingevent Properties has stamp.value");            
        }
    }
}
