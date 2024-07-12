using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;

namespace log4net.Ext.Json.Xunit.Layout.Arrangements
{
    public class DefaultNXLog : RepoTest
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
                            <default value=""nxlog"" />
                          </layout>
                        </appender>
                      </log4net>";
        }

		protected override void RunTestLog(log4net.ILog log)
        {
            log.Info(4);

            var events = GetEventStrings(log.Logger);
            events.Should().NotBeNullOrEmpty();
            events.Should().HaveCount(1);

            var le = events.Single();
            le.Should().NotBeNullOrEmpty(because: "loggingevent");
            le.Should().Contain(@"""EventTime"":", because: "log line has EventTime");
            le.Should().Contain(@"""Message"":", because: "log line has Message");
            le.Should().Contain(@"""Logger"":", because: "log line has Logger");
            le.Should().Contain(@"""Severity"":", because: "log line has Severity");
        }
    }
}

