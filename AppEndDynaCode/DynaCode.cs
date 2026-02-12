using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using AppEndCommon;
using Newtonsoft.Json.Linq;

namespace AppEndDynaCode
{
    /// <summary>
    /// DynaCode Facade - Public API for dynamic code compilation and execution.
    /// All implementation details are delegated to specialized internal classes.
    /// </summary>
    public static partial class DynaCode
    {
        #region Lifecycle Management
        public static void Init(CodeInvokeOptions? codeInvokeOptions = null) 
            => DynaCodeCore.Init(codeInvokeOptions);

        public static void Refresh() 
            => DynaCodeCore.Refresh();

        public static string ReBuild() 
            => DynaCodeCore.ReBuild();
        #endregion

        #region Assembly Access
        public static Assembly DynaAsm 
            => DynaCodeCore.DynaAsm;

        public static string[] ScriptFiles 
            => DynaCodeCore.ScriptFiles;

        public static List<CodeMap> CodeMaps 
            => DynaCodeMapping.CodeMaps;
        #endregion

        #region File Path Retrieval
        public static string GetMethodFilePath(string methodFullName) 
            => DynaCodeMapping.GetMethodFilePath(methodFullName);

        public static string TryGetMethodFilePath(string methodFullName) 
            => DynaCodeMapping.TryGetMethodFilePath(methodFullName);

        public static string GetClassFilePath(string typeFullName) 
            => DynaCodeMapping.GetClassFilePath(typeFullName);

        public static string TryGetClassFilePath(string typeFullName) 
            => DynaCodeMapping.TryGetClassFilePath(typeFullName);
        #endregion

        #region Method Invocation
        public static CodeInvokeResult InvokeByJsonInputs(string methodFullPath, JsonElement? inputParams = null, AppEndUser? dynaUser = null, string clientIp = "", string clientAgent = "", bool ignoreCaching = false) 
            => DynaCodeInvocation.InvokeByJsonInputs(methodFullPath, inputParams, dynaUser, clientIp, clientAgent, ignoreCaching);

        public static CodeInvokeResult InvokeByParamsInputs(string methodFullPath, object[]? inputParams = null, AppEndUser? dynaUser = null, bool ignoreCaching = false) 
            => DynaCodeInvocation.InvokeByParamsInputs(methodFullPath, inputParams, dynaUser, ignoreCaching);
        #endregion

        #region Access Control
        public static bool HasAccess(string methodFullPath, AppEndUser? actor) 
            => DynaCodeSecurity.HasAccess(methodFullPath, actor);

        public static Dictionary<string, object> GetAllAllowdAndDeniedActions(AppEndUser? actor) 
            => DynaCodeSecurity.GetAllAllowdAndDeniedActions(actor);

        public static JArray GetDynaClassesAccessSettingsByRoleId(string roleId) 
            => DynaCodeSecurity.GetDynaClassesAccessSettingsByRoleId(roleId);

        public static void SetAccessSettingsByRoleId(string methodFullName, string roleId, bool access) 
            => DynaCodeSecurity.SetAccessSettingsByRoleId(methodFullName, roleId, access);
        #endregion

        #region Settings Management
        public static MethodSettings ReadMethodSettings(string methodFullName) 
            => DynaCodeSettings.ReadMethodSettings(methodFullName);

        public static MethodSettings ReadMethodSettings(string methodFullName, string methodFilePath) 
            => DynaCodeSettings.ReadMethodSettings(methodFullName, methodFilePath);

        public static void WriteMethodSettings(string methodFullName, MethodSettings methodSettings) 
            => DynaCodeSettings.WriteMethodSettings(methodFullName, methodSettings);

        public static void WriteMethodSettings(string methodFullName, string methodFilePath, MethodSettings methodSettings) 
            => DynaCodeSettings.WriteMethodSettings(methodFullName, methodFilePath, methodSettings);

        public static void RemoveMethodSettings(string methodFullName) 
            => DynaCodeSettings.RemoveMethodSettings(methodFullName);
        #endregion

        #region Method Manipulation
        public static void DuplicateMethod(string methodFullName, string methodCopyName) 
            => DynaCodeManipulation.DuplicateMethod(methodFullName, methodCopyName);

        public static void CreateMethod(string methodFullName, string methodName, MethodTemplate methodTemplate = MethodTemplate.DbDialog) 
            => DynaCodeManipulation.CreateMethod(methodFullName, methodName, methodTemplate);

        public static void RemoveMethod(string methodFullName) 
            => DynaCodeManipulation.RemoveMethod(methodFullName);

        public static bool MethodExist(string methodFullName) 
            => DynaCodeManipulation.MethodExist(methodFullName);
        #endregion

        #region Helper Methods
        public static List<DynaClass> GetDynaClasses() 
            => DynaCodeHelpers.GetDynaClasses();
        #endregion

        #region Long-Running Tasks
        public static LongRunningTaskInfo? GetLongRunningTaskStatus(string taskToken, string requestingUser) 
            => LongRunningTaskManager.GetStatus(taskToken, requestingUser);

        public static object? GetLongRunningTaskResult(string taskToken, string requestingUser) 
            => LongRunningTaskManager.GetResult(taskToken, requestingUser);

        public static bool CancelLongRunningTask(string taskToken, string requestingUser) 
            => LongRunningTaskManager.Cancel(taskToken, requestingUser);
        #endregion
    }
}