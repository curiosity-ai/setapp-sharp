import Cocoa
import Foundation
import Setapp

public struct SetAppSubscription : Codable  {
    var isActive: Bool?
    var expirationDate: Double?
    var description: String?
}

@_cdecl("get_subscription")
public func get_subscription() -> UnsafePointer<CChar> {
      let sub =   SetappManager.shared.subscription;
    let s = SetAppSubscription(isActive:sub?.isActive, expirationDate:  sub?.expirationDate?.timeIntervalSince1970,description:sub?.description)
    
    do {
        let jsonData =  try JSONEncoder().encode(s)
        let jsonString = String(data: jsonData, encoding: .utf8)!
        return toCString(jsonString);
    } catch     {
        
        return toCString("");
    }
}

@_cdecl("showReleaseNotesWindowIfNeeded")
public func showReleaseNotesWindowIfNeeded() {
    SetappManager.shared.showReleaseNotesWindowIfNeeded();
}

@_cdecl("showReleaseNotesWindow")
public func showReleaseNotesWindow() {
    SetappManager.shared.showReleaseNotesWindow();
}

@_cdecl("askUserToShareEmail")
public func askUserToShareEmail() {
    SetappManager.shared.askUserToShareEmail();
}

private func toCString(_ str : String) -> UnsafePointer<CChar    >{
    return UnsafePointer(strdup(str))
}

public typealias LogHanldeCallback = @convention(c)  (_ message: UnsafePointer<CChar>, _ logLevel: Setapp.SetappLogLevel) -> Void;

@_cdecl("setLogHandle")
public func setLogHandle( cb:LogHanldeCallback) {
    SetappManager.setLogHandle({ ( message: String,  logLevel: Setapp.SetappLogLevel) -> Void in
        cb(toCString(message), logLevel);
    })
}

@_cdecl("reportUsageEvent")
public func reportUsageEvent(usageEvent: Setapp.SetappUsageEvent) {
    SetappManager.shared.reportUsageEvent(usageEvent);
}
