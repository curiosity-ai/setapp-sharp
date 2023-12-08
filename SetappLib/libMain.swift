import Cocoa
import Foundation
import Setapp

public struct SetAppSubscription : Codable  {
    var isActive: Bool?
    var expirationDate: Double?
    var description: String?
}


public struct RequestAuthorizationCodeResult : Codable  {
    
    var error: String?
    var code: String?
    
}


public typealias RequestAuthorizationCodeCallback = @convention(c)  (_ result: UnsafePointer<CChar>) -> Void;


@_cdecl("requestAuthorizationCode")
public func requestAuthorizationCode(clientID: UnsafePointer<CChar>, scope: UnsafePointer<CChar>, callback: RequestAuthorizationCodeCallback)  -> Void {
    
    let clientIDStr = String(cString: clientID)
    let scopeStr  = String(cString: scope)
    
    do {
        let scopeStrArray: [String] = try! JSONDecoder().decode( [String].self, from: scopeStr.data(using: .utf8)!)
        
        let scopeArray :[Setapp.VendorAuthorizationScope] = scopeStrArray.map { s in
            Setapp.VendorAuthorizationScope(rawValue:s)
        };
        
        // Make sure an active Setapp subscription is present.
        // See subscription monitoring examples on this page for more info.
        SetappManager.shared.requestAuthorizationCode(       clientID:  clientIDStr,        scope: scopeArray    ) { result in
            switch result {
            case let .success(code):
                // Authentication code obtained successfully.
                // Use the code to authorize your app or server for Setapp: exchange the auth code for the access token and the refresh token using the Setapp API.
                
                let s = RequestAuthorizationCodeResult(error: nil, code:code)
                
                do {
                    let jsonData =  try JSONEncoder().encode(s)
                    let jsonString = String(data: jsonData, encoding: .utf8)!
                    jsonString.withCString{jsonCStr in
                        callback( jsonCStr);}
                } catch     {
                    
                    "".withCString{jsonCStr in
                        callback( jsonCStr);}
                }
                
            case let .failure(error):
                // The request has failed.
                // See the error message for details.
                let s = RequestAuthorizationCodeResult(error: error.localizedDescription, code:nil)
                
                do {
                    let jsonData =  try JSONEncoder().encode(s)
                    let jsonString = String(data: jsonData, encoding: .utf8)!
                    jsonString.withCString{jsonCStr in
                        callback( jsonCStr);
                    }
                    
                    
                } catch     {
                    "".withCString {jsonCStr in
                        callback( jsonCStr);}
                    
                }
            }
        }
    } catch     {
        
        "".withCString {jsonCStr in
            callback( jsonCStr);}
    }
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

@_cdecl("setLogLevel")
public func setLogLevel( logLevel: Setapp.SetappLogLevel) {
    SetappManager.logLevel = logLevel;
}

@_cdecl("getLogLevel")
public func setLogLevel() -> Setapp.SetappLogLevel {
  return  SetappManager.logLevel;
}


public typealias LogHanldeCallback = @convention(c)  (_ message: UnsafePointer<CChar>, _ logLevel: Setapp.SetappLogLevel) -> Void;

@_cdecl("setLogHandle")
public func setLogHandle( cb:LogHanldeCallback) {
    SetappManager.setLogHandle({ ( message: String,  logLevel: Setapp.SetappLogLevel) -> Void in
        message.withCString { cStringPtr in
            cb(cStringPtr, .verbose)
        }
    })
    
    "Setapp logging initialized!".withCString { cStringPtr in
        cb(cStringPtr, .verbose)
    }
}

@_cdecl("reportUsageEvent")
public func reportUsageEvent(usageEvent: Setapp.SetappUsageEvent) {
    SetappManager.shared.reportUsageEvent(usageEvent);
}
