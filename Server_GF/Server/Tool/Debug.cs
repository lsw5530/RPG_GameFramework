using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class Debug
{
    private static ILog m_Log;

    static Debug()
    {
        XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
        m_Log = LogManager.GetLogger(typeof(Debug));
    }

    public static void Log(object message)
    {
        m_Log.Debug(message);
    }

    public static void Log(string format, params object[] args)
    {
        m_Log.DebugFormat(format, args);
    }

    public static void LogInfo(object message)
    {
        m_Log.Info(message);
    }

    public static void LogInfo(string format, params object[] args)
    {
        m_Log.InfoFormat(format, args);
    }

    public static void LogWarn(object message)
    {
        m_Log.Warn(message);
    }

    public static void LogWarn(string format, params object[] args)
    {
        m_Log.WarnFormat(format, args);
    }

    public static void LogError(object message)
    {
        m_Log.Error(message);
    }

    public static void LogError(string format, params object[] args)
    {
        m_Log.ErrorFormat(format, args);
    }

    public static void LogFatle(object message)
    {
        m_Log.Fatal(message);
    }

    public static void LogFatle(string format, params object[] args)
    {
        m_Log.FatalFormat(format, args);
    }
}
