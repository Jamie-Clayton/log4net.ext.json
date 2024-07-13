using System;
using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;

namespace log4net.Ext.Json.Xunit.Log
{
    public class StructurallyStandard : RepoTest
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
                            <decorator type='log4net.Layout.Decorators.StandardTypesDecorator, log4net.Ext.Json' />
                            <default />
                            <remove value='message' />
                            <member value='data:messageobject' />
                          </layout>
                        </appender>
                      </log4net>";
        }

		protected override void RunTestLog(log4net.ILog log)
        {
            log.Info(new { A = 1, B = new { X = DateTime.Parse("2014-01-01") } });

            var events = GetEventStrings(log.Logger);

            events.Length.Should().Be(1, "events Count");

            var le = events.Single();

            le.Should().NotBeNull(because: "loggingevent");

            le.Should().Contain(@"""A"":1", because: "le has structured message");
            le.Should().Contain(@"""X"":""2014-01-01T00:00:00.0000000""", because: "le has structured message");

        }
    }
}

