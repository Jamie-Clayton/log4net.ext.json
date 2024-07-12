using System;
using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;

namespace log4net.Ext.Json.Xunit.Layout.Arrangements
{
    public class MessageObject : RepoTest
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
                            <member value='MessageObject' />
                          </layout>
                        </appender>
                      </log4net>";
        }

		protected override void RunTestLog(log4net.ILog log)
        {
            log.Info(4);

            var events = GetEventStrings(log.Logger);
            events.Length.Should().Be(1, "events Count");

            var le = events.Single();
            le.Should().NotBeNull(because: "loggingevent");
			le.Should().Be(@"{""MessageObject"":4}" + Environment.NewLine, because: "log line");
        }
    }
}