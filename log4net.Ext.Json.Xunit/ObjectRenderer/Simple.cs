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

		[Test]
		[TestCase(true, "true", "true")]
		[TestCase(false, "false", "false")]
		[TestCase(0, "0", "0")]
		[TestCase(0L, "0", "0")]
		[TestCase(0D, "0", "0")]
		[TestCase(int.MinValue, "-2147483648", "-2147483648")]
		[TestCase(int.MaxValue, "2147483647", "2147483647")]
		[TestCase(double.MinValue, "-1.7976931348623157E+308", "-1.7976931348623157E+308")]
		[TestCase(double.MaxValue, "1.7976931348623157E+308", "1.7976931348623157E+308")]

		[TestCase('*', @"""*""", @"""*""")]
		[TestCase((byte)168, @"168", @"""\uFFFD""")]

		[TestCase("xxx &;", @"""xxx &;""", @"""xxx &;""")]
		[TestCase("ěščřžýáíé■", // JsonDotNet leaves unicode chars as is, homemade escapes them producing ASCII
			@"""ěščřžýáíé■""",
			@"""\u011B\u0161\u010D\u0159\u017E\u00FD\u00E1\u00ED\u00E9\u25A0""")]
		[TestCase("\"\\\b\f\n\r\t", @"""\""\\\b\f\n\r\t""", @"""\""\\\b\f\n\r\t""")]
		[TestCase("</>", @"""</>""", @"""</>""")]
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
		public void SerializeNullableNull()
		{
			Serialize(new int?(), "null", "null");
		}

		[Test]
		public void SerializeNullableInt()
		{
			Serialize(new int?(int.MinValue), "-2147483648", "-2147483648");
		}

		[Test]
		public void SerializeNullableDouble()
		{
			Serialize(new double?(double.MinValue), "-1.7976931348623157E+308", "-1.7976931348623157E+308");
		}

		[Test]
		public void SerializeDBNull()
		{
			Serialize(DBNull.Value, "null", "null");
		}

		[Test]
		public void SerializeDecimal()
		{
			Serialize(decimal.MinValue, @"-79228162514264337593543950335", @"-79228162514264337593543950335");
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
