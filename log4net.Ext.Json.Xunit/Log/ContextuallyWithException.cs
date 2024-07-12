using System;
using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;

namespace log4net.Ext.Json.Xunit.Log
{
    public class ContextuallyWithException : RepoTest
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
                             <decorator type='log4net.Layout.Decorators.StandardTypesFlatDecorator, log4net.Ext.Json' />
                             <member value='NDC' />
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
                    try
                    {
                        throw new InvalidProgramException("test");
                    }
                    catch (InvalidProgramException x)
                    {
                        log.Error("Exception caught", x);
                    }
                }
            };

            var events = GetEventStrings(log.Logger);

            events.Length.Should().Be(1, "events Count");

            var le = events.Single();

            le.Should().NotBeNull(because: "loggingevent");
            le.Should().Contain(@"""data.A"":1", "le2 has structured message");
            le.Should().Contain(@"""data.B.X"":""Y""", "le2 has structured message");
            le.Should().Contain(@"""TestLog sub section""", "le1 has structured message");
            le.Should().Contain(@"""exception"":""System.InvalidProgramException: test", "le2 has structured message");
            
            // curiously, the properties set within the NDC are still here... not my fault.

            log.Info(null);

            events = GetEventStrings(log.Logger);
            events.Length.Should().Be(2, "events Count");

            le = events.Last();
            le.Should().NotBeNull(because: "loggingevent");
            le.Should().Be(@"{""data.A"":1,""data.B.X"":""Y""}" + Environment.NewLine, because: "le2 has structured message");

        }
    }
}

