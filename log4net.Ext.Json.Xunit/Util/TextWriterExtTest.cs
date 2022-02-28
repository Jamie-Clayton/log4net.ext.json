using System;
using System.Globalization;
using System.IO;
using System.Text;
using log4net.Util;
using NUnit.Framework;

namespace log4net.Ext.Json.Xunit.Util
{
    public class TextWriterExtTest
    {
        [Test]
        public void WriteFormatDoubleInSpecificCulture()
        {
            var orignalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                // setting up not English culture globally
                System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
                // string writer setup for using English culture
                var tw = new StringWriter(new StringBuilder(500), CultureInfo.GetCultureInfo("en-US")) as TextWriter;
                double d = 1.25d;
                tw.WriteFormat("{0:r}", d);

                Assert.AreEqual("1.25", tw.ToString());
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = orignalCulture;
            }
        }
    }
}
