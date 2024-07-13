using System.Linq;
using log4net.Ext.Json.Xunit.General;
using FluentAssertions;
using log4net.Layout;
using log4net.Layout.Pattern;
using log4net.ObjectRenderer;

namespace log4net.Ext.Json.Xunit.Log
{
    public class WithJsonDotNetRenderer1 : RepoTest
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
                             <serializingconverter type='log4net.Layout.Pattern.JsonPatternConverter, log4net.Ext.Json'>
                                <renderer type='log4net.ObjectRenderer.JsonDotNetRenderer, log4net.Ext.Json.Net'>
                                </renderer>
                             </serializingconverter>
                             <decorator type='log4net.Layout.Decorators.StandardTypesDecorator, log4net.Ext.Json' />
                             <member value='message:messageobject' />
                          </layout>
                        </appender>
                      </log4net>";
        }

		protected override void RunTestLog(log4net.ILog log)
        {   
            var appenders = GetAppenders<TestAppender>(log.Logger);
            appenders.Length.Should().Be(1, "appenders Count");            

            var tapp = appenders.Single();
            tapp.Layout.Should().BeAssignableTo<SerializedLayout>("layout type");
            
            var layout = ((SerializedLayout)tapp.Layout);
            layout.SerializingConverter.Should().BeAssignableTo<JsonPatternConverter>("converter type");
            
            var converter = ((JsonPatternConverter)layout.SerializingConverter);
            converter.Renderer.Should().BeAssignableTo<JsonDotNetRenderer>("renderer type");
            var renderer = ((JsonDotNetRenderer)converter.Renderer);

            log.Info(new { A = 1, B = new { X = "Y" } });

            var events = GetEventStrings(log.Logger);
            events.Length.Should().Be(1, "events Count");

            var le = events.Single();
            le.Should().NotBeNull(because: "loggingevent");
            le.Should().Contain(@"{""message"":{""A"":1,""B"":{""X"":""Y""}}}", "le has structured message");
            
        }
    }
}
