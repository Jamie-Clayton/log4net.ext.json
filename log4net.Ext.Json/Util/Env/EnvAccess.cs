using System;
using System.Reflection;
using log4net.Util;

namespace log4net.Ext.Json.Util.Env
{
    /// <remarks>
    /// Values are cached statically on first access
    /// </remarks>
    public class EnvAccess : IEnvAccess
    {
        public string GetMachineName() => Environment.MachineName;
        public string GetCommandLine() => Environment.CommandLine;
        public string GetUserName() => Environment.UserName;
        public string GetUserDomain() => Environment.UserDomainName;
        public virtual int GetProcessId() => ProcessIdAccess.Value;
        public virtual string GetAppName() => WebAppNameAccess.Value ?? AppNameAccess.Value;
        public virtual string GetAppPath() => WebAppNameAccess.Value == null ? AppDirAccess.Value : AppNameAccess.Value;
        public long GetWorkingSet() => Environment.WorkingSet;

        #region Private lazy static factories and caches
        private static class ProcessIdAccess
        {
            public static int Value { get; }
            static ProcessIdAccess()
            {
                Value = System.Diagnostics.Process.GetCurrentProcess().Id;
            }
        }

        private static class WebAppNameAccess
        {
            public static string Value { get; }
            static WebAppNameAccess()
            {
                // use reflection to avoid deps on web
                var hostingEnvType = Type.GetType("System.Web.Hosting.HostingEnvironment", false);
                if(hostingEnvType == null)
                    return;

                var t = hostingEnvType.GetTypeInfo();
                Value = t.GetDeclaredProperty("SiteName").GetValue(null) as string;
            }
        }
        private static class AppNameAccess
        {
            public static string Value { get; }
            static AppNameAccess()
            {
                Value = AppDomain.CurrentDomain.FriendlyName;
            }
        }
        private static class AppDirAccess
        {
            public static string Value { get; }
            static AppDirAccess()
            {
                Value = System.IO.Directory.GetCurrentDirectory();
                //Value = AppContext.BaseDirectory;
                //Value = AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        #endregion
    }
}