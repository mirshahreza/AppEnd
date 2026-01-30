using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppEndCommon;
using Newtonsoft.Json.Linq;

namespace AppEndDynaCode
{
    internal static class DynaCodeSecurity
    {
        #region Access Control
        public static bool HasAccess(string methodFullPath, AppEndUser? actor)
        {
            MethodInfo methodInfo = DynaCodeMapping.GetMethodInfo(methodFullPath);
            string methodFilePath = DynaCodeMapping.GetMethodFilePath(methodFullPath);
            MethodSettings methodSettings = DynaCodeSettings.ReadMethodSettings(methodFullPath, methodFilePath);
            return HasAccess(methodInfo, methodSettings, actor);
        }

        internal static bool HasAccess(MethodInfo methodInfo, MethodSettings methodSettings, AppEndUser? actor)
        {
            var invokeOptions = DynaCodeCore.InvokeOptions;
            if (invokeOptions.PublicMethods is not null && invokeOptions.PublicMethods.ContainsIgnoreCase(methodInfo.GetFullName())) return true;
            if (actor is null) return false;
            if (actor.UserName.EqualsIgnoreCase(invokeOptions.PublicKeyUser)) return true;
            if (actor.RoleNames.ContainsIgnoreCase(invokeOptions.PublicKeyRole)) return true;
            if (actor.Roles.ToList().HasIntersect(methodSettings.AccessRules.AllowedRoles)) return true;
            if (methodSettings.AccessRules.AllowedRoles.Contains("*")) return true;
            if (methodSettings.AccessRules.AllowedUsers.ContainsIgnoreCase(actor.UserName)) return true;
            if (methodSettings.AccessRules.AllowedUsers.Contains("*")) return true;
            return false;
        }
        #endregion

        #region Permission Management
        public static Dictionary<string, object> GetAllAllowdAndDeniedActions(AppEndUser? actor)
        {
            if (actor == null) return new Dictionary<string, object> { { "AllowedActions", "".Split(',') } };

            List<DynaClass> dynaClasses = DynaCodeHelpers.GetDynaClasses();
            List<string> alloweds = [];
            List<string> denieds = [];

            var invokeOptions = DynaCodeCore.InvokeOptions;

            foreach (var dynaC in dynaClasses)
            {
                foreach (DynaMethod dynaM in dynaC.DynaMethods)
                {
                    MethodSettings ms = dynaM.MethodSettings;
                    string mFullName = dynaC.Namespace + "." + dynaC.Name + "." + dynaM.Name;
                    if (
                        invokeOptions.PublicMethods.ContainsIgnoreCase(mFullName) ||
                        invokeOptions.PublicKeyUser.EqualsIgnoreCase(actor.UserName) ||
                        actor.RoleNames.ContainsIgnoreCase(invokeOptions.PublicKeyRole) ||
                        ms.AccessRules.AllowedUsers.ContainsIgnoreCase(actor.UserName) ||
                        ms.AccessRules.AllowedRoles.HasIntersect(actor.Roles)
                        )
                        alloweds.Add(mFullName);

                    if (ms.AccessRules.DeniedUsers.ContainsIgnoreCase(actor.UserName)) denieds.Add(mFullName);
                }
            }

            foreach (string s in denieds) if (alloweds.ContainsIgnoreCase(s)) alloweds.Remove(s);
            return new Dictionary<string, object> { { "AllowedActions", alloweds.ToArray() } };
        }

        public static JArray GetDynaClassesAccessSettingsByRoleId(string roleId)
        {
            List<DynaClass> dynaClasses = DynaCodeHelpers.GetDynaClasses();
            var methodsPlus = new JArray();
            foreach (var dynaC in dynaClasses)
            {
                var jArrayC = new JArray();
                foreach (DynaMethod dynaM in dynaC.DynaMethods)
                {
                    MethodSettings ms = dynaM.MethodSettings;
                    var joM = new JObject();
                    joM["MethodName"] = dynaM.Name;
                    joM["HasAccess"] = ms.AccessRules.AllowedRoles.Contains(roleId) ? true : false;
                    jArrayC.Add(joM);
                }
                var joM2 = new JObject { ["Controller"] = dynaC.Namespace + "." + dynaC.Name, ["Methods"] = jArrayC };
                methodsPlus.Add(joM2);
            }
            return methodsPlus;
        }

        public static void SetAccessSettingsByRoleId(string methodFullName, string roleId, bool access)
        {
            MethodSettings ms = DynaCodeSettings.ReadMethodSettings(methodFullName);
            List<string> curRoles = [.. ms.AccessRules.AllowedRoles];
            if (access == true)
            {
                if (!curRoles.Contains(roleId)) { curRoles.Add(roleId); }
            }
            else
            {
                curRoles.Remove(roleId);
            }
            ms.AccessRules.AllowedRoles = curRoles.ToArray();
            DynaCodeSettings.WriteMethodSettings(methodFullName, ms);
        }
        #endregion
    }
}
