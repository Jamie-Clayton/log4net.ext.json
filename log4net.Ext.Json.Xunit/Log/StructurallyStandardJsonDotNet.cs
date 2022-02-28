﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Ext.Json.Xunit.General;
using NUnit.Framework;
using log4net.Core;
using System.Collections;

namespace log4net.Ext.Json.Xunit.Log
{
	public class StructurallyStandardJsonDotNet : RepoTest
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
                            <renderer type='log4net.ObjectRenderer.JsonDotNetRenderer, log4net.Ext.Json.Net'>
                            </renderer>
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

            Assert.AreEqual(1, events.Length, "events Count");

            var le = events.Single();

            Assert.IsNotNull(le, "loggingevent");
            Assert.Multiple(() =>
            {
                StringAssert.Contains(@"""A"":1", le, "le has structured message");
                StringAssert.Contains(@"""X"":""2014-01-01T00:00:00.0000000""", le, "le has structured message");
            });
        }
    }
}

