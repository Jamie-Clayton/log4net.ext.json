using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Ext.Json.Xunit.General;
using NUnit.Framework;
using System.Diagnostics;

namespace log4net.Ext.Json.Xunit.Layout.Arrangements
{
    public class Members : RepoTest
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

        protected override void RunTestLog(log4net.ILog log)
        {
            log4net.GlobalContext.Properties["OurCompany.ApplicationName"] = "fubar";

            log.Info(4);

            var events = GetEventStrings(log.Logger);

            Assert.AreEqual(1, events.Length, "events Count");

            var le = events.Single();

            Assert.IsNotNull(le, "loggingevent");

            var procid = Process.GetCurrentProcess().Id;
            Console.WriteLine(Environment.OSVersion.VersionString);
            var userName =
                Environment.OSVersion.VersionString.StartsWith("Microsoft Windows")?
                  Environment.UserDomainName + @"\\" + Environment.UserName
                : Environment.UserName;
            Assert.Multiple(() =>
            {
                StringAssert.StartsWith(@"{""OurCompany.ApplicationName"":""fubar""", le, "log line");
                StringAssert.Contains(@",""Host"":{", le, "log line");
                StringAssert.Contains(@"""ProcessId"":" + procid, le, "log line");
                StringAssert.Contains(@"""HostName"":""" + Environment.MachineName + @"""", le, "log line");
                StringAssert.Contains(@"""UserName"":""" + userName + @"""", le, "log line");
                StringAssert.Contains(@"""A"":""L-INFO-log4net.Ext.Json.Xunit.Layout.Arrangements.Members""", le, "log line");
                StringAssert.Contains(@"""B"":""" + DateTime.Now.Year + @"""", le, "log line");
                StringAssert.Contains(@"""App"":""", le, "log line");

                // fix #3, do not use member name as a default value
                StringAssert.DoesNotContain("empty1", le);
                StringAssert.DoesNotContain("empty2", le);
            });
        }
    }
}