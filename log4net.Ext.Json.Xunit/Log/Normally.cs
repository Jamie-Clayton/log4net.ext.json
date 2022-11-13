﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Ext.Json.Xunit.General;
using NUnit.Framework;

namespace log4net.Ext.Json.Xunit.Log
{
    public class Normally : RepoTest
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
                          </layout>
                        </appender>
                      </log4net>";
        }

        protected override void RunTestLog(log4net.ILog log)
        {
            log.Info(new { A = 1, B = new { X = "Y" } });

            var events = GetEventStrings(log.Logger);

            Assert.AreEqual(1, events.Length, "events Count");

            var le = events.Single();

            Assert.IsNotNull(le, "loggingevent");

            StringAssert.Contains(@"""message"":""{ A = 1, B = { X = Y } }""", le, "le has structured message");
        }
    }
}

