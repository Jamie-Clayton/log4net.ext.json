using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;

namespace log4net.Ext.Json.Xunit.Log
{
    public class Contextually : RepoTest
    {
        protected override string GetConfig()
        {
            return @"<log4net>
                        <root>
                          <level value='DEBUG'/>
                          <appender-ref ref='TestAppender'/>
                        </root>

                        <appender name='TestAppender' type='log4net.Ext.Json.Xunit.General.TestAppender, log4net.Ext.Json.Xunit'>
                          <layout type='log4net.Layout.SerializedLayout, log4net.Ext.Json'>
                             <member value='ndc|%ndc' />
                             <member value='data' />
                             <member value='exception' />
                          </layout>
                        </appender>
                      </log4net>";
        }

        protected override void RunTestLog(log4net.ILog log)
        {
            using (log4net.ThreadContext.Stacks["NDC"].Push("TestLog"))
            {
                log4net.ThreadContext.Properties["data"] = new { A = 1, B = new { X = "Y" } };

                using (log4net.ThreadContext.Stacks["NDC"].Push("sub section"))
                {
                    log.Info("OK");
                }
            };

            var events = GetEventStrings(log.Logger);
            events.Length.Should().Be(1, "events Count");

            var le = events.Single();
            le.Should().NotBeNull(because: "loggingevent");
            le.Should().Contain(@"""data"":{", because: "le has structured message");
            le.Should().Contain(@"""X"":""Y""", because: "le has structured message");
            le.Should().Contain(@"""A"":1", because: "le has structured message");
            le.Should().Contain(@"""TestLog sub section""", because: "le has structured message");
            le.Should().NotContain(@"""exception""", because: "le has no exception");
        }
    }
}

