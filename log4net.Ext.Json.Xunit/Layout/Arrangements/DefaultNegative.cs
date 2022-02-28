﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Ext.Json.Xunit.General;
using NUnit.Framework;

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

            Assert.AreEqual(1, events.Length, "events Count");

            var le = events.Single();

            Assert.IsNotNull(le, "loggingevent");

			Assert.AreEqual(@"{""message"":""Hola!""}" + Environment.NewLine, le, "log line has no members");
        }
    }
}

