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

    public class RequestAuthorizationCodeResult
    {
        public string error { get; set; }
        public string code  { get; set; }
    }


    public static async Task<string> RequestAuthorizationCode(string clientId, params VendorAuthorizationScope[] scope)
    {
        var requestAuthorizationCodeTask = new TaskCompletionSource<string>();

        var scopeStr = JsonSerializer.Serialize(scope.Select(s => s switch
        {
            VendorAuthorizationScope.openAI            => "ai.openai",
            VendorAuthorizationScope.applicationAccess => "application.access",
            _                                          => throw new ArgumentOutOfRangeException(nameof(s), s, null)
        }).ToArray());

        using var requestAuthorizationCodeRunner = Task.Run(() =>
        {
            try
            {
                requestAuthorizationCode(System.Text.Encoding.UTF8.GetBytes(clientId + "\0"), System.Text.Encoding.UTF8.GetBytes(scopeStr + "\0"), resultPtr =>
                {
                    try
                    {
                        var resultStr = Marshal.PtrToStringUTF8(resultPtr);

                        var parsedResult = JsonSerializer.Deserialize<RequestAuthorizationCodeResult>(resultStr);

                        if (!string.IsNullOrWhiteSpace(parsedResult?.code))
                        {
                            requestAuthorizationCodeTask.SetResult(parsedResult?.code);
                            return;
                        }
                        else if (!string.IsNullOrWhiteSpace(parsedResult?.error))
                        {
                            throw new Exception(parsedResult?.error);
                        }
                        else
                        {
                            throw new Exception("no result returned");
                        }
                    }
                    catch (Exception e)
                    {
                        requestAuthorizationCodeTask.SetException(e);
                    }
                });
            }
            catch (Exception e)
            {
                requestAuthorizationCodeTask.SetException(e);
            }
        });

        var delayTask = Task.Run(async () => await Task.Delay(TimeSpan.FromSeconds(30)));
        var task      = requestAuthorizationCodeTask.Task;

        await Task.WhenAny(task, delayTask);

        if (task.IsCompleted)
        {
            return await task;
        }
        else
        {
            throw new TimeoutException("Took longer than 30s.");
        }
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

    public enum VendorAuthorizationScope
    {
        openAI,
        applicationAccess,
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void RequestAuthorizationCodeCallback(IntPtr result);

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void requestAuthorizationCode(byte[] clientID, byte[] scope, RequestAuthorizationCodeCallback callback);

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_subscription();

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void showReleaseNotesWindowIfNeeded();

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void showReleaseNotesWindow();

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void askUserToShareEmail();

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void reportUsageEvent(SetappUsageEvent usageEvent);

    [DllImport("runtimes/osx/native/libSetappLib.dylib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void setLogHandle(LogHandleCallback logHandleCallback);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogHandleCallback(string message, SetappLogLevel logLevel);
}