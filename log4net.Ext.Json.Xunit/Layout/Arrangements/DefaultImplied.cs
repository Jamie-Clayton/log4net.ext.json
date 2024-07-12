using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;

namespace log4net.Ext.Json.Xunit.Layout.Arrangements
{
    public class DefaultImplied : RepoTest
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
                          </layout>
                        </appender>
                      </log4net>";
        }

        protected override void RunTestLog(log4net.ILog log)
        {
            log.Info(4);

            var events = GetEventStrings(log.Logger);
            events.Length.Should().Be(1, because: "events Count");
            events.Should().NotContainNulls(because: "events Not Null");
            events.Should().HaveCount(1, because: "events Not Empty");
            var le = events.Single();
            le.Should().NotBeNullOrEmpty(because: "loggingevent");
            le.Should().Contain(@"""date"":", because: "log line has date");
            le.Should().Contain(@"""message"":", because: "log line has message");
            le.Should().Contain(@"""logger"":", because: "log line has logger");
            le.Should().Contain(@"""level"":", because: "log line has level");
        }
    }
}

