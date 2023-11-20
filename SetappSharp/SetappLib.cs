using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SetappSharp;

public static class Setapp
{
    private class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(
            ref Utf8JsonReader    reader,
            Type                  typeToConvert,
            JsonSerializerOptions options) => DateTimeOffset.FromUnixTimeMilliseconds((long)(reader.GetDouble() * 1000));

        public override void Write(
            Utf8JsonWriter        writer,
            DateTimeOffset        dateTimeValue,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(dateTimeValue.ToString("O"));
    }

    public class SetAppSubscription
    {
        [JsonExtensionData] public Dictionary<string, JsonElement> ExtensionData { get; set; }

        [JsonPropertyName("isActive")] public bool? IsActive { get; set; }
        [JsonConverter(typeof(DateTimeOffsetJsonConverter))]
        [JsonPropertyName("expirationDate")] public DateTimeOffset? ExpirationDate { get; set; }
        [JsonPropertyName("description")] public string? Description { get;               set; }
    }

    public static SetAppSubscription? GetSubscription()
    {
        var jsonStr = Marshal.PtrToStringUTF8(get_subscription());
        if (string.IsNullOrWhiteSpace(jsonStr)) return null;

        return JsonSerializer.Deserialize<SetAppSubscription>(jsonStr);

    }
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

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
   private static extern IntPtr get_subscription();

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void showReleaseNotesWindowIfNeeded();

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private  static extern void showReleaseNotesWindow();

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private   static extern void askUserToShareEmail();

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private  static extern void reportUsageEvent(SetappUsageEvent usageEvent);

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private  static extern void setLogHandle(LogHandleCallback logHandleCallback);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogHandleCallback(string message, SetappLogLevel logLevel);
}