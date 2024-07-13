using System;
using System.Linq;
using log4net.Ext.Json.Xunit.General;
using Xunit.Abstractions;
using FluentAssertions;
using System.Diagnostics;

namespace log4net.Ext.Json.Xunit.Layout.Arrangements
{
    public class Members : RepoTest
    {
        private readonly ITestOutputHelper output;

        protected override string GetConfig()
        {
            return @"<log4net>
                        <root>
                          <level value='DEBUG'/>
                          <appender-ref ref='TestAppender'/>
                        </root>

                        <appender name='TestAppender' type='log4net.Ext.Json.Xunit.General.TestAppender, log4net.Ext.Json.Xunit'>
                          <layout type='log4net.Layout.SerializedLayout, log4net.Ext.Json'>
                            <member value='OurCompany.ApplicationName' /> <!-- ref to property -->
                            <member value='A|L-%p-%c' /> <!-- (|) arbitrary pattern layout format -->
                            <member value='B%date:yyyy' /> <!-- (%:) one pattern layout conversion pattern with optional option -->
                            <member value='Host=ProcessId\;HostName\;UserName' /> <!-- (=) nested structure, escape ; -->
                            <member value='App:appname' /> <!-- named member -->
                            <member value='empty1' /> <!-- empty member -->
                            <member value='empty2:EmPty2' /> <!-- empty named member -->
                          </layout>
                        </appender>
                      </log4net>";
        }

        public Members(ITestOutputHelper output)
        {
            this.output = output;
        }

        protected override void RunTestLog(log4net.ILog log)
        {
            log4net.GlobalContext.Properties["OurCompany.ApplicationName"] = "fubar";

            log.Info(4);

            var events = GetEventStrings(log.Logger);

            events.Length.Should().Be(1, "events Count");

            var le = events.Single();
            le.Should().NotBeNull(because: "loggingevent");

            var procid = Process.GetCurrentProcess().Id;
            output.WriteLine(Environment.OSVersion.VersionString);
            var userName =
                Environment.OSVersion.VersionString.StartsWith("Microsoft Windows")?
                  Environment.UserDomainName + @"\\" + Environment.UserName
                : Environment.UserName;

            le.Should().StartWith(@"{""OurCompany.ApplicationName"":""fubar""", because: "log line starts with");
            le.Should().Contain(@",""Host"":{", because: "log line contains host");
            le.Should().Contain(@"""ProcessId"":" + procid, because: "log line contains process id");
            le.Should().Contain(@"""HostName"":""" + Environment.MachineName + @"""", because: "log line contains host name");
            le.Should().Contain(@"""UserName"":""" + userName + @"""", because: "log line contains user name");
            le.Should().Contain(@"""A"":""L-INFO-log4net.Ext.Json.Xunit.Layout.Arrangements.Members""", because: "log line contains A");
            le.Should().Contain(@"""B"":""" + DateTime.Now.Year + @"""",because: "log line contains B");
            le.Should().Contain(@"""App"":""", because: "log line contains App");

            // fix #3, do not use member name as a default value
            le.Should().NotContain("emty1", because: "log line does not contain empty1");
            le.Should().NotContain("empty2", because: "log line does not contain empty2");
        }
    }
}