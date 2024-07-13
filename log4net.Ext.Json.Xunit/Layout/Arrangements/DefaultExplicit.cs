using FluentAssertions;
using log4net.Ext.Json.Xunit.General;
using Xunit;

namespace log4net.Ext.Json.Xunit.Layout.Arrangements
{
    public class DefaultExplicit : RepoTest
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
                            <default />
                          </layout>
                        </appender>
                      </log4net>";
		}

		protected override void RunTestLog(log4net.ILog log)
		{
			log.Info(4);

			var events = GetEventStrings(log.Logger);

			Assert.Collection(events, (le) =>
			{
				Assert.NotNull(le);
                le.Should().Contain(@"""date"":", because: "log line had date");
                le.Should().Contain(@"""message"":", because: "log line has message");
                le.Should().Contain(@"""logger"":", because: "log line has logger");
                le.Should().Contain(@"""level"":", because: "log line has level");
            });
		}
	}
}

