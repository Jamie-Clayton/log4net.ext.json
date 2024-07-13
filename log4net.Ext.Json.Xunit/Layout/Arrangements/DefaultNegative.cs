using System;
using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;

namespace log4net.Ext.Json.Xunit.Layout.Arrangements
{
    public class DefaultNegative : RepoTest
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
                            <arrangement value='DEFAULT!' />
                            <remove value='date' />
                            <remove value='level' />
                            <remove value='logger' />
                            <remove value='thread' />
                            <remove value='exception' />
                            <remove value='appname' />
                            <remove value='ndc' />
                          </layout>
                        </appender>
                      </log4net>";
        }

        protected override void RunTestLog(log4net.ILog log)
        {
            log.Info("Hola!");

            var events = GetEventStrings(log.Logger);
            events.Should().NotBeNull();
            events.Length.Should().Be(1, because: "events Count");

            var le = events.Single();
            le.Should().NotBeNullOrEmpty();
            le.Should().Be(@"{""message"":""Hola!""}" + Environment.NewLine, "log line has no members");
        }
    }
}

