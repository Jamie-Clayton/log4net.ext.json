using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

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

		public static TestCaseData[] testCases =
		{
			new TestCaseData(true, "true", "true"){TestName="SerializeBoolTrue"},
			new TestCaseData(false, "false", "false"){TestName="SerializeBoolFalse"},
			new TestCaseData(0, "0", "0"){TestName="SerializeInt"},
			new TestCaseData(0L, "0", "0"){TestName="SerializeLong"},
			new TestCaseData(0D, "0", "0"){TestName="SerializeDouble"},
			new TestCaseData(int.MinValue, "-2147483648", "-2147483648"){TestName="SerializeIntMin"},
			new TestCaseData(int.MaxValue, "2147483647", "2147483647"){TestName="SerializeIntMax"},
#if NET481
			new TestCaseData(double.MinValue, "-1.7976931348623157E+308", "-1.7976931348623157E+308"){TestName="SerializeDoubleMin"},
			new TestCaseData(double.MaxValue, "1.7976931348623157E+308", "1.7976931348623157E+308"){TestName="SerializeDoubleMax"},
			new TestCaseData(new double? (double.MinValue), "-1.7976931348623157E+308", "-1.7976931348623157E+308"){TestName="SerializeNullableDouble"},
#else
			new TestCaseData(double.MinValue, "-1.7976931348623157E+308", "-1.7976931348623157e+308"){TestName="SerializeDoubleMin"},
			new TestCaseData(double.MaxValue, "1.7976931348623157E+308", "1.7976931348623157e+308"){TestName="SerializeDoubleMax"},
			new TestCaseData(new double? (double.MinValue), "-1.7976931348623157E+308", "-1.7976931348623157e+308"){TestName="SerializeNullableDouble"},
#endif
			new TestCaseData('*', @"""*""", @"""*"""){TestName="SerializeChar"},
			new TestCaseData((byte)168, @"168", @"""\uFFFD"""){TestName="SerializeByte"},

			new TestCaseData("xxx &;", @"""xxx &;""", @"""xxx &;"""),
			new TestCaseData("ěščřžýáíé■", // JsonDotNet leaves unicode chars as is, homemade escapes them producing ASCII
				@"""ěščřžýáíé■""",
				@"""\u011B\u0161\u010D\u0159\u017E\u00FD\u00E1\u00ED\u00E9\u25A0"""),
			new TestCaseData("\"\\\b\f\n\r\t", @"""\""\\\b\f\n\r\t""", @"""\""\\\b\f\n\r\t"""),
			new TestCaseData("</>", @"""</>""", @"""</>"""),
			new TestCaseData(new int?(), "null", "null"){TestName="SerializeNullableNull"},
			new TestCaseData(new int? (int.MinValue), "-2147483648", "-2147483648"){TestName="SerializeNullableInt"},
			new TestCaseData(DBNull.Value, "null", "null"){TestName="SerializeDBNull"},
			new TestCaseData(decimal.MinValue, @"-79228162514264337593543950335", @"-79228162514264337593543950335"){TestName="SerializeDecimal"}
		};

		[TestCaseSource(nameof(testCases))]
		public void Serialize(object value, string expectedJsonDotNet, string expectedHomeMade)
		{
			var wrJsonDotNet = new StringWriter();
			_serializerJsonDotNet.RenderObject(null, value, wrJsonDotNet);
			var resultJsonDotNet = wrJsonDotNet.ToString();
			StringAssert.StartsWith(expectedJsonDotNet, resultJsonDotNet, String.Format ("JsonDotNet serialized {0}", value));

			var wrHomeMade = new StringWriter();
			_serializerHomeMade.RenderObject(null, value, wrHomeMade);
			var resultHomeMade = wrHomeMade.ToString();
			StringAssert.StartsWith(expectedHomeMade, resultHomeMade, String.Format("HomeMade serialized {0}", value));
		}

		[Test]
		public void SerializeEnumFlags()
		{
			// JsonDotNet works enums into numbers, homemade into names
			Serialize(System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline,
				@"10",
				@"""Multiline, Compiled""");
		}

		[Test]
		public void SerializeEnumSingle()
		{
			// JsonDotNet works enums into numbers, homemade into names
			Serialize(System.Text.RegularExpressions.RegexOptions.Compiled,
				@"8",
				@"""Compiled""");
		}

		[Test]
		public void SerializeGuid()
		{
			Serialize(Guid.Empty, @"""00000000-0000-0000-0000-000000000000""", @"""00000000-0000-0000-0000-000000000000""");
		}

		[Test]
		public void SerializeUri()
		{
			Serialize(new Uri("irc://xxx/yyy"), @"""irc://xxx/yyy""", @"""irc://xxx/yyy""");
		}

		/// <summary>
		/// We differ here on purpose?
		/// </summary>
		[Test]
		public void SerializeBytes()
		{
			Serialize(new byte[] { 65, 255, 0, 128 }, @"""Qf8AgA==""", @"""A\uFFFD\u0000\uFFFD""");
		}

		/// <summary>
		/// We differ here on purpose?
		/// </summary>
		[Test]
		public void SerializeChars()
		{
			Serialize(new char[] { 'A', '¿', char.MinValue, '├' }, @"[""A"",""¿"",""\u0000"",""├""]", @"""A\u00BF\u0000\u251C""");
		}

		/// <summary>
		/// We differ here on purpose with ISO standard date? Greater precision and better support.
		/// </summary>
		[Test]
		public void SerializeDateTime()
		{
			Serialize(DateTime.Parse("2014-01-01 00:00:01"), @"""2014-01-01T00:00:01""", @"""2014-01-01T00:00:01.0000000""");
		}

		/// <summary>
		/// We differ here on purpose?
		/// </summary>
		[Test]
		public void SerializeTimeSpan()
		{
			// JsonDotNet handles TimeSpan as any object serializing it's fields and props, homemeade ony does seconds
			Serialize(TimeSpan.Parse("3.00:00:01.1234567"),
				@"""3.00:00:01.1234567""",
				@"259201.12345");
		}

		[Test]
		public void SerializeObject()
		{
			Serialize(new object(), @"{}", @"{}");
		}

		[Test]
		public void SerializeCustomPrivateObject()
		{
			Serialize(new CustomPrivateObject(), @"{""X"":""Y""}", @"{""X"":""Y""}");
		}

		[Test]
		public void SerializeCustomPublicObject()
		{
			Serialize(new CustomPublicObject(), @"{""X"":""Y""}", @"{""X"":""Y""}");
		}

		[Test]
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
