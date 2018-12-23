using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendGridEmailApplication.Entities
{
    /// <summary>
    /// Enumeration of AccessControl operations
    /// </summary>
    public enum Operations
    {
        //Access Control - Cache
        RemoveAllAccessControlCacheItems,
        
        // Access Control - Operation 
        InsertAccessControlOperation,
        UpdateAccessControlOperation,
        DeleteAccessControlOperation,
        GetAccessControlOperation,
        GetAccessControlOperations,

        // Access Control - Role
        InsertAccessControlRole,
        UpdateAccessControlRole,
        DeleteAccessControlRole,
        GetAccessControlRole,
        GetAccessControlRoles,
		GetUsersForRole,

		// Access Control - User Profile
		InsertAccessControlUserProfile,
        UpdateAccessControlUserProfile,
        DeleteAccessControlUserProfile,
        GetAccessControlUserProfile,
        GetOperationsForUser,
        GetLoggedInUserDetails,

        // Access Control - Resource
        InsertAccessControlResources,
        UpdateAccessControlResources,
        DeleteAccessControlResources,
        GetAccessControlResources,

        // Adopter
        InsertAdopter,
        UpdateAdopter,
        DeleteAdopter,
        GetAdopter,
        GetAdopters,
        GetAdopterSites,
        GetAdopterUsers,
        GetAdopterApplications,

        // Application
        RegisterApplication,
        UpdateApplication,
        DeleteApplication,
        GetApplication,
        UpdateAdopterApplicationMapping,

        //Blob
        InsertBlob,
        UpdateBlob,
        DeletetBlob,
        GetBlobContent,
        GetBlobList,
        GetConfigBlobMetadata,

        // Command
        SetChannelValue,
        GetChannelValues, //Command to get channel values directly from device through IoT Hub
        SetChannelValues, //Command to set channel values directly on device through IoT Hub

        // Config
        InsertConfigBlob,
        UpdateConfigBlob,
        DeleteConfigBlob,
        GetConfigBlobContent,
        GetConfigBlobList,

        // Device
        RegisterDevice,
        UnRegisterDevice,
        GetDevices,
        GetChannels,
        GetDeviceConnection,
        GetDeviceMessages,
        GetPaths,
        GetMetricIds,
        EnableDevice,
        DisableDevice,

        // Device Profile
        InsertDeviceProfile,
        UpdateDeviceProfile,
        DeleteDeviceProfile,
        GetDeviceProfile,
        GetAllDeviceProfiles,
        
        // Events
        GetEvents,

        // Master Channel

        UpdateMasterChannelsInfo,
        GetMasterChannelsInfo,
        ValidateMasterChannelsInfo,

        // Partition
        InsertPartition,
        UpdatePartition,
        DeletePartition,
        GetPartition,
        GetSitesInPartition,

        // Security
        DecryptData,
        GetTokenDetails,
        GetSecurityToken,
        DeleteSecretKey,
        GetSecretKey,
        RefreshSecurityToken,
        CheckAuthorization,
        Logout,

        // Site
        RegisterSite,
        UpdateSite,
        DeleteSite,
        GetSite,
        GetDevicesInSite,
        GenerateActivationCode,
        GetActivationCode,
        GetUsersForSite,

        // System
        SystemPing,
        SystemConnectionStrings,
        SystemComponentInfo,
        SystemInfo,
        SystemMsgCounts,
        SystemLogPublisherList,
        SystemLogPublisher,
        SystemMetricId,
        SystemInvalidateCache,
        DumpMclandDeviceprofileCache,
        SystemWebAPILogReset,
        SystemWebAPILoggingInfo,

        // Time Series
        GetChannelRealtimes,
        GetTrends,

        // User
        InsertUser,
        UpdateUser,
        DeleteUser,
        GetUser,
        GetUserAdopters,
        UpdateUserAdopterMapping,
        GetSitesForUser,
        SendVerificationEmail,
        GetVerificationCode,
        GetVerificationCodeForSignUp,
        CreateVerificationForAccountRecovery,
        SetUserPassword,
		ChangeUserPassword,
        SignUpUser,
        ValidateVerificationCode,
		CheckUserExists,
		CreateVerificationCode,
		VerifyVerificationCode,
		UpdateLoggedInUserDetails,

		// DLM
		ScheduleDlmJob,
        GetDlmJob,
        CancelDlmJob,
        GetDlmJobsInSite,

        //DMS
        GetCCISChannelList,
        GetAdopterChannelLists,
        GetAdopterChannelListChannels,
        UpdateAdopterChannelList,
        UpdateAdopterChannelListChannels,
        GetDmsDataTypes,
        GetDmsUnits,
        ExportSCLData,

        // Scheduled Jobs
        ScheduleJob,
        GetScheduledJob,
        CancelScheduledJob,
        GetScheduledJobsInSite,
        ExecuteScheduledJob
    }
}

