using System.Runtime.InteropServices;
using System;
using SteamKit2;
using SteamKit2.Authentication;

public class Exports
{
    // [UnmanagedCallersOnly(EntryPoint = "CreateSteamClient")]
    // public static IntPtr CreateSteamClient()
    // {
    //     var client = new SteamClient();
    //     return GCHandle.ToIntPtr(GCHandle.Alloc(client));
    // }

    [UnmanagedCallersOnly(EntryPoint = "GetVersion")]
    public static unsafe int GetVersion()
    {
        return 1;
    }

    [UnmanagedCallersOnly(EntryPoint = "CreateSteamClient")]
    public static unsafe IntPtr CreateSteamClient()
    {
        SteamKit2.DebugLog.Enabled = true;
        SteamKit2.DebugLog.AddListener((category, message) =>
        {
            Console.WriteLine($"[SteamKit2][{category}] {message}");
        });
        var client = new SteamClient();
        return GCHandle.ToIntPtr(GCHandle.Alloc(client));
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeSteamClient")]
    public static unsafe void FreeSteamClient(IntPtr clientPtr)
    {
        var handle = GCHandle.FromIntPtr(clientPtr);
        handle.Free();
    }

    [UnmanagedCallersOnly(EntryPoint = "CreateCallbackManager")]
    public static unsafe IntPtr CreateCallbackManager(IntPtr steamClientPtr)
    {
        var steamClient = (SteamClient)GCHandle.FromIntPtr(steamClientPtr).Target;
        var callbackManager = new CallbackManager(steamClient);
        return GCHandle.ToIntPtr(GCHandle.Alloc(callbackManager));
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeCallbackManager")]
    public static unsafe void FreeCallbackManager(IntPtr callbackManagerPtr)
    {
        var handle = GCHandle.FromIntPtr(callbackManagerPtr);
        handle.Free();
    }

    [UnmanagedCallersOnly(EntryPoint = "GetSteamUserHandler")]
    public static unsafe IntPtr GetSteamUserHandler(IntPtr steamClientPtr)
    {
        var steamClient = (SteamClient)GCHandle.FromIntPtr(steamClientPtr).Target;
        var steamUser = steamClient.GetHandler<SteamUser>();
        return GCHandle.ToIntPtr(GCHandle.Alloc(steamUser));
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeSteamUserHandler")]
    public static unsafe void FreeSteamUserHandler(IntPtr steamUserHandlerPtr)
    {
        var handle = GCHandle.FromIntPtr(steamUserHandlerPtr);
        handle.Free();
    }

    [UnmanagedCallersOnly(EntryPoint = "CallbackManagerSubscribeConnectedCallback")]
    public static unsafe void CallbackManagerSubscribeConnectedCallback(IntPtr callbackManagerPtr, IntPtr callback)
    {
        var callbackManager = (CallbackManager)GCHandle.FromIntPtr(callbackManagerPtr).Target;
        delegate* unmanaged[Cdecl]<IntPtr, void> unmanagedCallback = (delegate* unmanaged[Cdecl]<IntPtr, void>)callback;
        Action<SteamClient.ConnectedCallback> managedCallback = (cb) =>
        {
            IntPtr cbPtr = GCHandle.ToIntPtr(GCHandle.Alloc(cb));
            unmanagedCallback(cbPtr);
        };
        callbackManager.Subscribe<SteamClient.ConnectedCallback>(managedCallback);
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeConnectedCallback")]
    public static unsafe void FreeConnectedCallback(IntPtr callbackPtr)
    {
        var handle = GCHandle.FromIntPtr(callbackPtr);
        handle.Free();
    }

    [UnmanagedCallersOnly(EntryPoint = "CallbackManagerSubscribeDisconnectedCallback")]
    public static unsafe void CallbackManagerSubscribeDisconnectedCallback(IntPtr callbackManagerPtr, IntPtr callback)
    {
        var callbackManager = (CallbackManager)GCHandle.FromIntPtr(callbackManagerPtr).Target;
        delegate* unmanaged[Cdecl]<IntPtr, void> unmanagedCallback = (delegate* unmanaged[Cdecl]<IntPtr, void>)callback;
        Action<SteamClient.DisconnectedCallback> managedCallback = (cb) =>
        {
            IntPtr cbPtr = GCHandle.ToIntPtr(GCHandle.Alloc(cb));
            unmanagedCallback(cbPtr);
        };
        callbackManager.Subscribe<SteamClient.DisconnectedCallback>(managedCallback);
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeDisconnectedCallback")]
    public static unsafe void FreeDisconnectedCallback(IntPtr callbackPtr)
    {
        var handle = GCHandle.FromIntPtr(callbackPtr);
        handle.Free();
    }

    [UnmanagedCallersOnly(EntryPoint = "CallbackManagerSubscribeLoggedOnCallback")]
    public static unsafe void CallbackManagerSubscribeLoggedOnCallback(IntPtr callbackManagerPtr, IntPtr callback)
    {
        var callbackManager = (CallbackManager)GCHandle.FromIntPtr(callbackManagerPtr).Target;
        delegate* unmanaged[Cdecl]<IntPtr, void> unmanagedCallback = (delegate* unmanaged[Cdecl]<IntPtr, void>)callback;
        Action<SteamUser.LoggedOnCallback> managedCallback = (cb) =>
        {
            IntPtr cbPtr = GCHandle.ToIntPtr(GCHandle.Alloc(cb));
            unmanagedCallback(cbPtr);
        };
        callbackManager.Subscribe<SteamUser.LoggedOnCallback>(managedCallback);
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeLoggedOnCallback")]
    public static unsafe void FreeLoggedOnCallback(IntPtr callbackPtr)
    {
        var handle = GCHandle.FromIntPtr(callbackPtr);
        handle.Free();
    }

    [UnmanagedCallersOnly(EntryPoint = "CallbackManagerSubscribeLoggedOffCallback")]
    public static unsafe void CallbackManagerSubscribeLoggedOffCallback(IntPtr callbackManagerPtr, IntPtr callback)
    {
        var callbackManager = (CallbackManager)GCHandle.FromIntPtr(callbackManagerPtr).Target;
        delegate* unmanaged[Cdecl]<IntPtr, void> unmanagedCallback = (delegate* unmanaged[Cdecl]<IntPtr, void>)callback;
        Action<SteamUser.LoggedOffCallback> managedCallback = (cb) =>
        {
            IntPtr cbPtr = GCHandle.ToIntPtr(GCHandle.Alloc(cb));
            unmanagedCallback(cbPtr);
        };
        callbackManager.Subscribe<SteamUser.LoggedOffCallback>(managedCallback);
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeLoggedOffCallback")]
    public static unsafe void FreeLoggedOffCallback(IntPtr callbackPtr)
    {
        var handle = GCHandle.FromIntPtr(callbackPtr);
        handle.Free();
    }

    [UnmanagedCallersOnly(EntryPoint = "SteamClientConnect")]
    public static unsafe void SteamClientConnect(IntPtr steamClientPtr)
    {
        var steamClient = (SteamClient)GCHandle.FromIntPtr(steamClientPtr).Target;
        steamClient.Connect();
    }

    [UnmanagedCallersOnly(EntryPoint = "CallbackManagerRunCallbacks")]
    public static unsafe byte CallbackManagerRunCallbacks(IntPtr callbackManagerPtr)
    {
        var callbackManager = (CallbackManager)GCHandle.FromIntPtr(callbackManagerPtr).Target;
        return (byte)(callbackManager.RunCallbacks() ? 1 : 0);
    }

    [UnmanagedCallersOnly(EntryPoint = "SteamClient_Authentication_BeginAuthSessionViaQRAsync")]
    public static unsafe IntPtr SteamClient_Authentication_BeginAuthSessionViaQRAsync(IntPtr steamClientPtr)
    {
        var steamClient = (SteamClient)GCHandle.FromIntPtr(steamClientPtr).Target;
        var qrAuthSession = steamClient.Authentication.BeginAuthSessionViaQRAsync(new AuthSessionDetails()).GetAwaiter().GetResult();
        return GCHandle.ToIntPtr(GCHandle.Alloc(qrAuthSession));
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeQrAuthSession")]
    public static unsafe void FreeQrAuthSession(IntPtr qrAuthSessionPtr)
    {
        var handle = GCHandle.FromIntPtr(qrAuthSessionPtr);
        handle.Free();
    }

    [UnmanagedCallersOnly(EntryPoint = "AuthSession_SetChallengeURLChangedCallback")]
    public static unsafe void AuthSession_SetChallengeURLChangedCallback(IntPtr QrAuthSessionPtr, IntPtr callback)
    {
        var qrAuthSession = (QrAuthSession)GCHandle.FromIntPtr(QrAuthSessionPtr).Target;
        delegate* unmanaged[Cdecl]<void> unmanagedCallback = (delegate* unmanaged[Cdecl]<void>)callback;
        qrAuthSession.ChallengeURLChanged = () =>
        {
            unmanagedCallback();
        };
    }

    [UnmanagedCallersOnly(EntryPoint = "AuthSession_PollingWaitForResultAsync")]
    [return: DNNE.C99Type("struct AuthPollResultNative")]
    [DNNE.C99DeclCode(@"
struct AuthPollResultNative
{
    const char* account_name;
    const char* refresh_token;
    const char* access_token;
    const char* new_guard_data;
};
")]
    public static unsafe AuthPollResultNative AuthSession_PollingWaitForResultAsync(IntPtr authSessionPtr)
    {
        var authSession = (AuthSession)GCHandle.FromIntPtr(authSessionPtr).Target;
        var managedPollResponse = authSession.PollingWaitForResultAsync().GetAwaiter().GetResult();
        return new AuthPollResultNative
        {
            AccountName = Utf8Alloc(managedPollResponse.AccountName),
            RefreshToken = Utf8Alloc(managedPollResponse.RefreshToken),
            AccessToken = Utf8Alloc(managedPollResponse.AccessToken),
            NewGuardData = Utf8Alloc(managedPollResponse.NewGuardData)
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct AuthPollResultNative
    {
        public byte* AccountName;
        public byte* RefreshToken;
        public byte* AccessToken;
        public byte* NewGuardData;
    }

    private static unsafe byte* Utf8Alloc(string? value)
    {
        if (value is null)
            return null;

        return (byte*)Marshal.StringToCoTaskMemUTF8(value);
    }

    [UnmanagedCallersOnly(EntryPoint = "FreeAuthPollResult")]
    public static unsafe void FreeAuthPollResult([DNNE.C99Type("struct AuthPollResultNative")] AuthPollResultNative result)
    {
        Marshal.FreeCoTaskMem((nint)result.AccountName);
        Marshal.FreeCoTaskMem((nint)result.RefreshToken);
        Marshal.FreeCoTaskMem((nint)result.AccessToken);

        if (result.NewGuardData != null)
            Marshal.FreeCoTaskMem((nint)result.NewGuardData);
    }

    [UnmanagedCallersOnly(EntryPoint = "SteamUser_LogOn")]
    public static unsafe void SteamUser_LogOn(IntPtr steamUserHandlerPtr, IntPtr logOnDetailsPtr)
    {
        var steamUser = (SteamUser)GCHandle.FromIntPtr(steamUserHandlerPtr).Target;
        var logOnDetails = (SteamUser.LogOnDetails)GCHandle.FromIntPtr(logOnDetailsPtr).Target;
        steamUser.LogOn(logOnDetails);
    }

    [UnmanagedCallersOnly(EntryPoint = "CreateLogOnDetails")]
    public static unsafe IntPtr CreateLogOnDetails([DNNE.C99Type("const char*")] byte* username, [DNNE.C99Type("const char*")] byte* accessToken)
    {
        var logOnDetails = new SteamUser.LogOnDetails
        {
            Username = Marshal.PtrToStringUTF8((nint)username),
            AccessToken = Marshal.PtrToStringUTF8((nint)accessToken)
        };
        return GCHandle.ToIntPtr(GCHandle.Alloc(logOnDetails));
    }

    [UnmanagedCallersOnly(EntryPoint = "LoggedOnCallbackData_isOk")]
    public static unsafe byte LoggedOnCallbackData_isOk(IntPtr loggedOnCallbackDataPtr)
    {
        var loggedOnCallbackData = (SteamUser.LoggedOnCallback)GCHandle.FromIntPtr(loggedOnCallbackDataPtr).Target;
        return (byte)(loggedOnCallbackData.Result == EResult.OK ? 1 : 0);
    }

    [UnmanagedCallersOnly(EntryPoint = "LoggedOnCallbackData_GetResult")]
    [return: DNNE.C99Type("EResult")]
    [DNNE.C99DeclCode(@"
typedef enum EResult 
{
    EResult_AccessDenied = 15,
    EResult_AccountActivityLimitExceeded = 96,
    EResult_AccountAssociatedToMultiplePartners = 90,
    EResult_AccountDeleted = 114,
    EResult_AccountDisabled = 43,
    EResult_AccountLimitExceeded = 95,
    EResult_AccountLockedDown = 73,
    EResult_AccountLoginDeniedNeedTwoFactor = 85,
    EResult_AccountLoginDeniedThrottle = 87,
    EResult_AccountLogonDenied = 63,
    EResult_AccountLogonDeniedNoMail = 66,
    EResult_AccountLogonDeniedVerifiedEmailRequired = 74,
    EResult_AccountNotFeatured = 45,
    EResult_AccountNotFound = 18,
    EResult_AccountNotFriends = 111,
    EResult_AdministratorOK = 46,
    EResult_AlreadyLoggedInElsewhere = 50,
    EResult_AlreadyOwned = 30,
    EResult_AlreadyRedeemed = 28,
    EResult_BadResponse = 76,
    EResult_Banned = 17,
    EResult_Blocked = 40,
    EResult_Busy = 10,
    EResult_CachedCredentialInvalid = 126,
    EResult_Cancelled = 52,
    EResult_CannotUseOldPassword = 64,
    EResult_CantRemoveItem = 113,
    EResult_ChargerRequired = 125,
    EResult_CommunityCooldown = 116,
    EResult_ConnectFailed = 35,
    EResult_ContentVersion = 47,
    EResult_DataCorruption = 53,
    EResult_Disabled = 80,
    EResult_DiskFull = 54,
    EResult_DuplicateName = 14,
    EResult_DuplicateRequest = 29,
    EResult_EmailSendFailure = 99,
    EResult_EncryptionFailure = 23,
    EResult_ExistingUserCancelledLicense = 115,
    EResult_Expired = 27,
    EResult_ExpiredLoginAuthCode = 71,
    EResult_ExternalAccountAlreadyLinked = 59,
    EResult_ExternalAccountUnlinked = 57,
    EResult_FacebookQueryError = 70,
    EResult_Fail = 2,
    EResult_FamilySizeLimitExceeded = 129,
    EResult_FileNotFound = 9,
    EResult_GSLTDenied = 102,
    EResult_GSLTExpired = 106,
    EResult_GSOwnerDenied = 103,
    EResult_HandshakeFailed = 36,
    EResult_HardwareNotCapableOfIPT = 67,
    EResult_IOFailure = 37,
    EResult_IPBanned = 105,
    EResult_IPLoginRestrictionFailed = 72,
    EResult_IPNotFound = 31,
    EResult_IPTInitError = 68,
    EResult_Ignored = 41,
    EResult_IllegalPassword = 61,
    EResult_InsufficientBattery = 124,
    EResult_InsufficientFunds = 107,
    EResult_InsufficientPrivilege = 24,
    EResult_Invalid = 0,
    EResult_InvalidCEGSubmission = 81,
    EResult_InvalidEmail = 13,
    EResult_InvalidItemType = 104,
    EResult_InvalidLoginAuthCode = 65,
    EResult_InvalidName = 12,
    EResult_InvalidParam = 8,
    EResult_InvalidPassword = 5,
    EResult_InvalidProtocolVer = 7,
    EResult_InvalidSignature = 121,
    EResult_InvalidState = 11,
    EResult_InvalidSteamID = 19,
    EResult_ItemDeleted = 86,
    EResult_LauncherMigrated = 119,
    EResult_LimitExceeded = 25,
    EResult_LimitedUserAccount = 112,
    EResult_LockingFailed = 33,
    EResult_LoggedInElsewhere = 6,
    EResult_LogonSessionReplaced = 34,
    EResult_MustAgreeToSSA = 118,
    EResult_NeedCaptcha = 101,
    EResult_NoConnection = 3,
    EResult_NoLauncherSpecified = 117,
    EResult_NoMatch = 42,
    EResult_NoMatchingURL = 75,
    EResult_NoMobileDevice = 92,
    EResult_NoSiteLicensesFound = 109,
    EResult_NoVerifiedPhone = 123,
    EResult_NotLoggedOn = 21,
    EResult_NotModified = 91,
    EResult_NotSettled = 100,
    EResult_NotSupported = 128,
    EResult_OK = 1,
    EResult_OfflineAppCacheInvalid = 130,
    EResult_PSNTicketInvalid = 58,
    EResult_ParentalControlRestricted = 69,
    EResult_ParseFailure = 122,
    EResult_PasswordRequiredToKickSession = 49,
    EResult_PasswordUnset = 56,
    EResult_Pending = 22,
    EResult_PersistFailed = 32,
    EResult_PhoneActivityLimitExceeded = 97,
    EResult_PhoneNumberIsVOIP = 127,
    EResult_RateLimitExceeded = 84,
    EResult_RefundToWallet = 98,
    EResult_RegionLocked = 83,
    EResult_RemoteCallFailed = 55,
    EResult_RemoteDisconnect = 38,
    EResult_RemoteFileConflict = 60,
    EResult_RequirePasswordReEntry = 77,
    EResult_RestrictedDevice = 82,
    EResult_Revoked = 26,
    EResult_SMSCodeFailed = 94,
    EResult_SameAsPreviousValue = 62,
    EResult_ServiceReadOnly = 44,
    EResult_ServiceUnavailable = 20,
    EResult_ShoppingCartNotFound = 39,
    EResult_SteamRealmMismatch = 120,
    EResult_Suspended = 51,
    EResult_TimeNotSynced = 93,
    EResult_Timeout = 16,
    EResult_TooManyPending = 108,
    EResult_TryAnotherCM = 48,
    EResult_TryLater = 131,
    EResult_TwoFactorActivationCodeMismatch = 89,
    EResult_TwoFactorCodeMismatch = 88,
    EResult_UnexpectedError = 79,
    EResult_ValueOutOfRange = 78,
    EResult_WGNetworkSendExceeded = 110
} EResult;
")]
    public static unsafe EResult LoggedOnCallbackData_GetResult(IntPtr loggedOnCallbackDataPtr)
    {
        var loggedOnCallbackData = (SteamUser.LoggedOnCallback)GCHandle.FromIntPtr(loggedOnCallbackDataPtr).Target;
        return loggedOnCallbackData.Result;
    }
}