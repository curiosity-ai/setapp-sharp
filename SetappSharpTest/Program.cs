using SetappSharp;
Console.WriteLine("Testing Calls");


Setapp.SetLogHandle((message, level) =>
{
    Console.WriteLine($"[{level}] {message}");
});

Setapp.ReportUsageEvent(Setapp.SetappUsageEvent.SIGN_IN);
Setapp.ShowReleaseNotesWindowIfNeeded();
Setapp.ShowReleaseNotesWindow();
Setapp.AskUserToShareEmail();


Console.WriteLine("Done!");