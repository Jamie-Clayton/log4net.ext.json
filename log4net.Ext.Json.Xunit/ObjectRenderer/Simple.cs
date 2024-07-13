using System;
using System.IO;
using Xunit;
using FluentAssertions;
using System.Globalization;

namespace log4net.Ext.Json.Xunit.ObjectRenderer
{
    public class Simple
    {
        log4net.ObjectRenderer.IJsonRenderer _serializerHomeMade;
        log4net.ObjectRenderer.IJsonRenderer _serializerJsonDotNet;

        public Simple()
        {
            _serializerHomeMade = new log4net.ObjectRenderer.JsonRenderer();
            _serializerJsonDotNet = new log4net.ObjectRenderer.JsonDotNetRenderer();
        }

        [Theory]
        [InlineData(true, "true", "true")]
        [InlineData(false, "false", "false")]
        [InlineData(0, "0", "0")]
        [InlineData(0L, "0", "0")]
        [InlineData(0D, "0.0", "0")] // TODO: Why different decimal behaviour
        [InlineData(int.MinValue, "-2147483648", "-2147483648")]
        [InlineData(int.MaxValue, "2147483647", "2147483647")]
        [InlineData('*', @"""*""", @"""*""")]
        [InlineData((byte)168, @"168", @"""\uFFFD""")]
        [InlineData("xxx &;", @"""xxx &;""", @"""xxx &;""")]
        [InlineData("ěščřžýáíé■", // JsonDotNet leaves unicode chars as is, homemade escapes them producing ASCII
            @"""ěščřžýáíé■""",
            @"""\u011B\u0161\u010D\u0159\u017E\u00FD\u00E1\u00ED\u00E9\u25A0""")]
        [InlineData("\"\\\b\f\n\r\t", @"""\""\\\b\f\n\r\t""", @"""\""\\\b\f\n\r\t""")]
        [InlineData("</>", @"""</>""", @"""</>""")]
        public void Serialize(object value, string expectedJsonDotNet, string expectedHomeMade)
        {
            
            string culturalValue = value?.ToString().ToString(CultureInfo.CurrentCulture);
            var wrJsonDotNet = new StringWriter();
            _serializerJsonDotNet.RenderObject(null, value, wrJsonDotNet);
            var resultJsonDotNet = wrJsonDotNet.ToString().ToString(CultureInfo.CurrentCulture);
            resultJsonDotNet.Should().Be(expectedJsonDotNet.ToString(CultureInfo.CurrentCulture), because: $"JsonDotNet serialized {culturalValue}");

            var wrHomeMade = new StringWriter();
            _serializerHomeMade.RenderObject(null, value, wrHomeMade);
            var resultHomeMade = wrHomeMade.ToString();
            resultHomeMade.Should().Be(expectedHomeMade.ToString(CultureInfo.CurrentCulture), because: $"HomeMade serialized {culturalValue}");
        }

        [Fact]
        public void SerializeNullableNull()
        {
            Serialize(new int?(), "null", "null");
        }

        [Fact]
        public void SerializeNullableInt()
        {
            Serialize(new int?(int.MinValue), "-2147483648", "-2147483648");
        }

        [Fact]
        public void SerializeNullableDoubleMin()
        {

#if NETCOREAPP3_0_OR_GREATER
            {
                // IEEE 754-2008 compliance changes from Core v3.0 onwards.
                // See: https://devblogs.microsoft.com/dotnet/floating-point-parsing-and-formatting-improvements-in-net-core-3-0/
                Serialize(new double?(double.MinValue), "-1.7976931348623157E+308", "-1.7976931348623157e+308");
            }
#else
            {
            Serialize(new double?(double.MaxValue), "1.7976931348623157E+308", "1.7976931348623157E+308");
            }
#endif
        }

        [Fact]
        public void SerializeNullableDoubleMax()
        {

#if NETCOREAPP3_0_OR_GREATER
            {
                // IEEE 754-2008 compliance changes from Core v3.0 onwards.
                // See: https://devblogs.microsoft.com/dotnet/floating-point-parsing-and-formatting-improvements-in-net-core-3-0/
                Serialize(new double?(double.MaxValue), "1.7976931348623157E+308", "1.7976931348623157e+308");
            }
#else
            {
            Serialize(new double?(double.MaxValue), "1.7976931348623157E+308", "1.7976931348623157E+308");
            }
#endif
        }

        [Fact]
        public void SerializeDBNull()
        {
            Serialize(DBNull.Value, "null", "null");
        }

        [Fact]
        public void SerializeDecimal()
        {
            // TODO: Why the different decimal behaviour
            Serialize(decimal.MinValue, @"-79228162514264337593543950335.0", @"-79228162514264337593543950335");
        }

        [Fact]
        public void SerializeEnumFlags()
        {
            // JsonDotNet works enums into numbers, homemade into names
            Serialize(System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline,
                @"10",
                @"""Multiline, Compiled""");
        }

        [Fact]
        public void SerializeEnumSingle()
        {
            // JsonDotNet works enums into numbers, homemade into names
            Serialize(System.Text.RegularExpressions.RegexOptions.Compiled,
                @"8",
                @"""Compiled""");
        }

        [Fact]
        public void SerializeGuid()
        {
            Serialize(Guid.Empty, @"""00000000-0000-0000-0000-000000000000""", @"""00000000-0000-0000-0000-000000000000""");
        }

        [Fact]
        public void SerializeUri()
        {
            Serialize(new Uri("irc://xxx/yyy"), @"""irc://xxx/yyy""", @"""irc://xxx/yyy""");
        }

        /// <summary>
        /// We differ here on purpose?
        /// </summary>
        [Fact]
        public void SerializeBytes()
        {
            Serialize(new byte[] { 65, 255, 0, 128 }, @"""Qf8AgA==""", @"""A\uFFFD\u0000\uFFFD""");
        }

        /// <summary>
        /// We differ here on purpose?
        /// </summary>
        [Fact]
        public void SerializeChars()
        {
            Serialize(new char[] { 'A', '¿', char.MinValue, '├' }, @"[""A"",""¿"",""\u0000"",""├""]", @"""A\u00BF\u0000\u251C""");
        }

        /// <summary>
        /// We differ here on purpose with ISO standard date? Greater precision and better support.
        /// </summary>
        [Fact]
        public void SerializeDateTime()
        {
            Serialize(DateTime.Parse("2014-01-01 00:00:01"), @"""2014-01-01T00:00:01""", @"""2014-01-01T00:00:01.0000000""");
        }

        /// <summary>
        /// We differ here on purpose?
        /// </summary>
        [Fact]
        public void SerializeTimeSpan()
        {
#if NETCOREAPP3_0_OR_GREATER
            {
                Serialize(TimeSpan.Parse("3.00:00:01.1234567"), @"""3.00:00:01.1234567""", @"259201.1234567");
            }
#else
            {
                // JsonDotNet handles TimeSpan as any object serializing it's fields and props, homemeade ony does seconds
                Serialize(TimeSpan.Parse("3.00:00:01.1234567"), @"""3.00:00:01.1234567""", @"259201.12345669998");
            }
#endif
        }

        [Fact]
        public void SerializeObject()
        {
            Serialize(new object(), @"{}", @"{}");
        }

        [Fact]
        public void SerializeCustomPrivateObject()
        {
            Serialize(new CustomPrivateObject(), @"{""X"":""Y""}", @"{""X"":""Y""}");
        }

        [Fact]
        public void SerializeCustomPublicObject()
        {
            Serialize(new CustomPublicObject(), @"{""X"":""Y""}", @"{""X"":""Y""}");
        }

        [Fact]
        public void SerializeAnonymous()
        {
            Serialize(new { PROP = 1 }, @"{""PROP"":1}", @"{""PROP"":1}");
        }

        public class CustomPublicObject
        {
            public string X = "Y";
            protected string Z = "Y";
        }

        private class CustomPrivateObject : CustomPublicObject
        {

        }
    }
}
