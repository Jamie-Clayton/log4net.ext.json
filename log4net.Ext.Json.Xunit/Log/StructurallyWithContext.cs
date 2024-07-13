using System;
using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace log4net.Ext.Json.Xunit.Log
{
    public class StructurallyWithContext : RepoTest
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
                             <member value='properties' />
                             <remove value='message' />
                              <member value='messageobject' />
                          </layout>
                        </appender>
                      </log4net>";
        }

        protected override void RunTestLog(log4net.ILog log)
        {

            var dataKey = "data";
            var data = new { A = 1, B = new { X = "Y" } };
            var testStackKey = "Test";
            var testStackValue1 = "TestStack";
            var testStackValue2 = "and Inner";
            var message = "OK";

            using (ThreadContext.Stacks[testStackKey].Push(testStackValue1))
            {
                ThreadContext.Properties[dataKey] = data;

                using (ThreadContext.Stacks[testStackKey].Push(testStackValue2))
                {
                    log.Info(message);
                }
            }

            var events = GetEventStrings(log.Logger);

            events.Length.Should().Be(1, "events Count");

            var le = JObject.Parse(events.Single());
            le["properties"][dataKey][nameof(data.A)].Value<long>().Should().Be(data.A);
            le["properties"][dataKey][nameof(data.B)][nameof(data.B.X)].Value<String>().Should().Be(data.B.X);
            le["properties"][testStackKey].Value<String>().Should().Be($"{testStackValue1} {testStackValue2}");
            le["messageobject"].Value<String>().Should().Be(message);
        }
    }
}

