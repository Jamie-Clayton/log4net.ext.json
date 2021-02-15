using System;
using System.Linq;
using log4net.Ext.Json.Xunit.General;
using Assert=NUnit.Framework.Assert;
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
            var data = new {A = 1, B = new {X = "Y"}};
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

            Assert.AreEqual(1, events.Length, "events Count");

            var le = JObject.Parse(events.Single());
            
            Assert.AreEqual(data.A, le["properties"][dataKey][nameof(data.A)].Value<long>());
            Assert.AreEqual(data.B.X, le["properties"][dataKey][nameof(data.B)][nameof(data.B.X)].Value<String>());
            Assert.AreEqual($"{testStackValue1} {testStackValue2}", le["properties"][testStackKey].Value<String>());
            Assert.AreEqual(message, le["messageobject"].Value<String>());
        }
    }
}

