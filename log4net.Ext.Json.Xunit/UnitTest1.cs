using System;
using NUnit.Framework;

namespace log4net.Ext.Json.Xunit
{
    public class UnitTest1
    {
        [Test]
        public void Test1()
        {
			var t = Type.GetType("log4net.Layout.SerializedLayout");
        }
    }
}
