using System.Runtime.InteropServices;

namespace SetappSharp;

public static class Setapp
{
    public static void ShowReleaseNotesWindowIfNeeded()                     => showReleaseNotesWindowIfNeeded();
    public static void ShowReleaseNotesWindow()                             => showReleaseNotesWindow();
    public static void AskUserToShareEmail()                                => askUserToShareEmail();
    public static void ReportUsageEvent(SetappUsageEvent usageEvent)        => reportUsageEvent(usageEvent);
    public static void SetLogHandle(LogHandleCallback    logHandleCallback) => setLogHandle(logHandleCallback);

    public enum SetappUsageEvent
    {
        SIGN_IN          = 0,
        SIGN_OUT         = 1,
        USER_INTERACTION = 2
    }

    public enum SetappLogLevel
    {
        VERBOSE = 0,
        DEBUG   = 1,
        INFO    = 2,
        WARNING = 3,
        ERROR   = 4,
        OFF     = 5
    }


    [DllImport("Resources/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    static extern void showReleaseNotesWindowIfNeeded();

    [DllImport("Resources/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    static extern void showReleaseNotesWindow();

    [DllImport("Resources/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    static extern void askUserToShareEmail();

    [DllImport("Resources/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    static extern void reportUsageEvent(SetappUsageEvent usageEvent);

    [DllImport("Resources/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    static extern void setLogHandle(LogHandleCallback logHandleCallback);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogHandleCallback(string message, SetappLogLevel logLevel);
}