﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Ext.Json.Xunit.General;
using NUnit.Framework;

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
			
			Assert.Multiple(() =>
			{
				foreach (var le in events)
				{
					Assert.NotNull(le);
					StringAssert.Contains(@"""date"":", le);
					StringAssert.Contains(@"""message"":", le);
					StringAssert.Contains(@"""logger"":", le);
					StringAssert.Contains(@"""level"":", le);
				}
			});
		}
	}
}

