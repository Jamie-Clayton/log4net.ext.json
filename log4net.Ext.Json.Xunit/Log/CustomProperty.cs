using System;
using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;

namespace log4net.Ext.Json.Xunit.Log
{
    /// <summary>
    /// http://sourceforge.net/p/log4net-json/support-tickets/9/
    /// </summary>
    public class CustomProperty : RepoTest
    {
        class Custom
        {
            public string Text { get { return "Number " + Number; } }
            public int Number { get { return counter++; } }
            protected int counter;
        }

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
                             <member value='message' />
                             <member value='custom' />
                          </layout>
                        </appender>
                      </log4net>";
        }

        protected override void RunTestLog(log4net.ILog log)
        {
            log4net.GlobalContext.Properties["custom"] = new Custom();
            try
            {
                log.Info("First");
                log.Info("Second");

                var events = GetEventStrings(log.Logger);

                events.Length.Should().Be(2, "events Count");

                var le1 = events.First();
                var le2 = events.Last();

                le1.Should().NotBeNull(because: "loggingevent 1");
                le2.Should().NotBeNull(because: "loggingevent 2");
                le1.Should().Be(@"{""message"":""First"",""custom"":{""Text"":""Number 0"",""Number"":1}}" + Environment.NewLine, because: "le1 has structured message");
                le2.Should().Be(@"{""message"":""Second"",""custom"":{""Text"":""Number 2"",""Number"":3}}" + Environment.NewLine, because: "le2 has structured message");
            }
            finally
            {
                log4net.GlobalContext.Properties.Remove("custom");
            }
        }
    }
}

