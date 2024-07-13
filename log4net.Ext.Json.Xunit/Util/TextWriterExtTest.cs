using System.Globalization;
using System.IO;
using System.Text;
using log4net.Util;
using Xunit;

namespace log4net.Ext.Json.Xunit.Util
{
    public class TextWriterExtTest
    {
        [Fact]
        public void WriteFormatDoubleInSpecificCulture()
        {
            // setting up not English culture globally
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");

            // string writer setup for using English culture
            var tw = new StringWriter(new StringBuilder(500), CultureInfo.GetCultureInfo("en-US")) as TextWriter;
            double d = 1.25d;
            tw.WriteFormat("{0:r}", d);
            
            Assert.Equal("1.25", tw.ToString());
        }
    }
}
