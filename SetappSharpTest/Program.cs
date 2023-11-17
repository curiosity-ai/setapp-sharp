using System.Runtime.InteropServices;
using System.Text.Json;
using SetappSharp;
Console.WriteLine("Testing Calls");


Setapp.SetLogHandle((message, level) =>
{
    Console.WriteLine($"[{level}] {message}");
});
var subscription = Setapp.GetSubscription();
Console.WriteLine($"subscription:");
Console.WriteLine(JsonSerializer.Serialize(subscription));
//Console.WriteLine(Marshal.PtrToStringAuto(subscription));
//Console.WriteLine(Marshal.PtrToStringAnsi(subscription));
//Console.WriteLine(Marshal.PtrToStringUTF8(subscription));
//Console.WriteLine(Marshal.PtrToStringUni(subscription));
//Console.WriteLine(Marshal.PtrToStringBSTR(subscription));
//Console.WriteLine(subscription);
//Console.WriteLine(subscription);
//Console.WriteLine(subscription);
//  Console.WriteLine($"isActive:\t\t {subscription.isActive}");
//  Console.WriteLine($"expirationDateTimestamp:\t {subscription.expirationDateTimestamp}");
//  Console.WriteLine($"description:\t {subscription.description}");


Setapp.ReportUsageEvent(Setapp.SetappUsageEvent.SIGN_IN);
Setapp.ShowReleaseNotesWindowIfNeeded();
Setapp.ShowReleaseNotesWindow();
Setapp.AskUserToShareEmail();


Console.WriteLine("Done!");