using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using AppEndCommon;
using AppEndDynaCode;
using AppEndDbIO;
using AppEndServer;
using Microsoft.AspNetCore.Routing.Matching;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections;
using AngleSharp.Text;
using Microsoft.Extensions.Caching.Memory;
using AngleSharp.Common;
using System.Reflection;
using static System.Net.WebRequestMethods;



namespace Zzz
{
    public static partial class AppEndProxy
    {
		#region Log
		public static void AppEndSuccessLogger(MethodInfo methodInfo, int actorId, string methodFullPath, string clientInfo, CodeInvokeResult codeInvokeResult, object[]? inputParams)
		{
			JObject joLogContent = AppEndServer.HostingUtils.CreateStandardLogContent(methodInfo, actorId, methodFullPath, clientInfo, codeInvokeResult, inputParams);
			AppEndEventLogger.Add(joLogContent);
		}
		public static void AppEndErrorLogger(MethodInfo methodInfo, int actorId, string methodFullPath, string clientInfo, CodeInvokeResult codeInvokeResult, object[]? inputParams)
		{
			JObject joLogContent = AppEndServer.HostingUtils.CreateStandardLogContent(methodInfo, actorId, methodFullPath, clientInfo, codeInvokeResult, inputParams);
			AppEndEventLogger.Add(joLogContent);
			//JObject joLogContent = AppEndServer.HostingUtils.CreateStandardLogContent(methodInfo, actorId, methodFullPath, clientInfo, codeInvokeResult, inputParams);
			//StaticMethods.LogImmed(joLogContent.ToJsonStringByNewtonsoft(), "log", "", $"{methodFullPath}-{actorId}-{codeInvokeResult.IsSucceeded}-");
		}
		public static void AppEndStartWritingLogItems()
		{
			List<JObject> logRecords = AppEndEventLogger.GetAndCleanupEvents();
			string cmd = "";
			foreach (JObject logRecord in logRecords)
			{
				cmd += $"INSERT INTO Common_ActivityLog (Method,IsSucceeded,FromCache,RecordId,EventBy,EventOn,Duration,ClientInfo) VALUES ('{logRecord["Method"]}',{logRecord["IsSucceeded"].ToBooleanSafe().To01Safe()},{logRecord["FromCache"].ToBooleanSafe().To01Safe()},'{logRecord["RecordId"]}','{logRecord["EventBy"]}','{logRecord["EventOn"]}',{logRecord["Duration"]},'{logRecord["ClientInfo"]}');" + SV.NL;
			}
			DbSchemaUtils dbSchemaUtils = new(AppEndSettings.LogDbConfName);
			dbSchemaUtils.DbIOInstance.ToNoneQuery(cmd);
		}
		#endregion

		#region AAA
		public static object? GetDynaClassesAccessSettingsByRoleId(string RoleId)
		{
			return DynaCode.GetDynaClassesAccessSettingsByRoleId(RoleId);
		}
		public static object? SetAccessSettingsByRoleId(string MethodFullName, string RoleId, bool Access)
		{
			DynaCode.SetAccessSettingsByRoleId(MethodFullName, RoleId, Access);
			return true;
		}
		public static object? GetAttributesByRoleId(string RoleId)
		{
			string sqlUserRecord = "SELECT Id,RoleId,AttributeId FROM AAA_Roles_Attributes WHERE RoleId=" + RoleId;
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			DataTable dtAttributes = dbIO.ToDataTable(sqlUserRecord)["Master"];
			return dtAttributes;
		}
		public static object? SetAttributesByRoleId(string AttributeId, string RoleId, bool Flag)
		{
			string sql = Flag == true
				? $"INSERT INTO AAA_Roles_Attributes (RoleId,AttributeId) VALUES ({RoleId},{AttributeId})"
				: $"DELETE AAA_Roles_Attributes WHERE RoleId={RoleId} AND AttributeId={AttributeId}";
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			dbIO.ToNoneQuery(sql);
			return true;
		}
		public static object? GetAttributesByUserId(string UserId)
		{
			string sqlUserRecord = "SELECT Id,UserId,AttributeId FROM AAA_Users_Attributes WHERE UserId=" + UserId;
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			DataTable dtAttributes = dbIO.ToDataTable(sqlUserRecord)["Master"];
			return dtAttributes;
		}
		public static object? SetAttributesByUserId(string AttributeId, string UserId, bool Flag)
		{
			string sql = Flag == true
				? $"INSERT INTO AAA_Users_Attributes (UserId,AttributeId) VALUES ({UserId},{AttributeId})"
				: $"DELETE AAA_Users_Attributes WHERE UserId={UserId} AND AttributeId={AttributeId}";
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			dbIO.ToNoneQuery(sql);
			return true;
		}

		public static object? SaveUserSettings(AppEndUser? Actor, string Settings)
		{
			if (Actor == null) return false;
			string sqlUpdateUserSettings = "UPDATE AAA_Users SET Settings=N'" + Settings + "' WHERE Id=" + Actor.Id;
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			dbIO.ToNoneQuery(sqlUpdateUserSettings);
			return true;
		}
		public static object? Login(string UserName, string Password)
		{
			Dictionary<string, object> kvp = [];
			if (UserName.ToStringEmpty() == "" || Password.ToStringEmpty() == "")
			{
				kvp.Add("Result", false);
			}
			else
			{
				DataRow? drUser = GetUserRow(UserName);
				if (drUser is not null)
				{
					string pass = drUser["Password"].ToStringEmpty();

					if ((pass == Password.GetMD4Hash() || pass == Password.GetMD5Hash()) && drUser["LoginLocked"].ToBooleanSafe() == false && drUser["IsActive"].ToBooleanSafe(true) == true)
					{
						AppEndUser appEndUser = CreateAppEndUserByIdAndUserName(drUser["Id"].ToIntSafe(), drUser["UserName"].ToStringEmpty());
						UpdateLoginTry(drUser["Id"].ToIntSafe(), true, -1);
						kvp.Add("token", appEndUser.CreateToken());
						kvp.Add("Result", true);
					}
					else
					{
						if (drUser["LoginTryFails"].ToIntSafe(0) < 4)
						{
							UpdateLoginTry(drUser["Id"].ToIntSafe(), false, drUser["LoginTryFails"].ToIntSafe(0));
						}
						else
						{
							UpdateLoginLocked(drUser["Id"].ToIntSafe(), true);
						}
						kvp.Add("Result", false);
					}
				}
				else
				{
					kvp.Add("Result", false);
				}
			}

			return kvp;
		}
		public static object? LoginAs(string UserName)
		{
			Dictionary<string, object> kvp = [];
			if (UserName.ToStringEmpty() == "")
			{
				kvp.Add("Result", false);
			}
			else
			{
				DataRow? drUser = GetUserRow(UserName);
				if (drUser != null)
				{
					kvp.Add("token", CreateAppEndUserByIdAndUserName(drUser["Id"].ToIntSafe(), drUser["UserName"].ToStringEmpty()).CreateToken());
					kvp.Add("Result", true);
				}
				else
				{
					kvp.Add("Result", false);
				}
			}
			return kvp;
		}
		public static object? ChangePassword(AppEndUser? Actor, string OldPassword, string NewPassword)
		{
			if (Actor is null) return false;
			DataRow? drUser = GetUserRow(Actor.UserName);
			if (drUser is null) return null;
			if (drUser["Password"].ToStringEmpty() != OldPassword.GetMD4Hash() && drUser["Password"].ToStringEmpty() != OldPassword.GetMD5Hash()) return false;
			string sql = "UPDATE AAA_Users SET PasswordUpdatedBy=" + Actor.ContextInfo?["UserId"] + ",PasswordUpdatedOn=GETDATE(),Password='" + NewPassword.GetMD5Hash() + "' WHERE Id=" + drUser["Id"];
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			dbIO.ToNoneQuery(sql);
			return true;
		}
		public static object? Logout(AppEndUser? Actor)
		{
			if (Actor == null) return false;
			SV.SharedMemoryCache.TryRemove(Actor.ContextCacheKey());
			return true;
		}
		public static object? Signup()
		{
			return true;
		}
		public static Dictionary<string, object> CreateUserClientContext(AppEndUser Actor)
		{
			if (Actor is null) return [];

			AppEndUser newActor = CreateAppEndUserByIdAndUserName(Actor.Id, Actor.UserName);

			Dictionary<string, object> r = DynaCode.GetAllAllowdAndDeniedActions(Actor);

			r.Add("IsPublicKey", AppEndSettings.PublicKeyUser.EqualsIgnoreCase(Actor.UserName));
			r.Add("HasPublicKeyRole", newActor.Roles.ContainsIgnoreCase(AppEndSettings.PublicKeyRole.ToLower()));

			string sqlUserRecord = "SELECT Id,UserName,Email,Mobile,Picture_FileBody,Picture_FileBody_xs,Settings FROM AAA_Users WHERE UserName='" + Actor.UserName + "'";
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			DataRow drUser = dbIO.ToDataTable(sqlUserRecord)["Master"].Rows[0];
			r.Add("Email", drUser["Email"] is System.DBNull ? "" : drUser["Email"].ToStringEmpty());
			r.Add("Mobile", drUser["Mobile"] is System.DBNull ? "" : drUser["Mobile"].ToStringEmpty());
			r.Add("Picture_FileBody", drUser["Picture_FileBody"] is System.DBNull ? "" : (byte[])drUser["Picture_FileBody"]);
			r.Add("Picture_FileBody_xs", drUser["Picture_FileBody_xs"] is System.DBNull ? "" : (byte[])drUser["Picture_FileBody_xs"]);

			r.Add("Settings", drUser["Settings"] is System.DBNull || drUser["Settings"].ToStringEmpty() == "" ? "{}" : (string)drUser["Settings"]);

			r.Add("NewToken", newActor.CreateToken());

			return r;
		}
		public static Hashtable CreateUserServerContext(AppEndUser? Actor)
		{
			// Dont remove Roles , Just add your own keys if needed
			Tuple<List<string>, List<string>> rr = GetAppEndUserRoles(Actor?.Id);
			Hashtable r = new()
			{
				{ "Roles",  rr.Item1 },
				{ "IsPublicKey",  AppEndSettings.PublicKeyUser.EqualsIgnoreCase(Actor?.UserName) },
				{ "HasPublicKeyRole",  rr.Item1.ContainsIgnoreCase(AppEndSettings.PublicKeyRole.ToLower()) }
			};
			return r;
		}
		public static object? GetLogedInUserContext(AppEndUser? Actor)
		{
			if (Actor == null) return null;
			Dictionary<string, object> r = CreateUserClientContext(Actor);
			return r;
		}
		private static DataRow? GetUserRow(string UserName)
		{
			string un = UserName.Replace("'", "").Replace(" ", "").Replace("=", "");
			string sqlUserRecord = "SELECT Id,UserName,Email,Mobile,Password,IsActive,LoginLocked,LoginTryFails,LoginTryFailLastOn,LoginTrySuccesses,LoginTrySuccessLastOn FROM AAA_Users WHERE UserName='" + un + "'";
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			DataTable dtUser = dbIO.ToDataTable(sqlUserRecord)["Master"];
			if (dtUser.Rows.Count > 0) return dtUser.Rows[0];
			return null;
		}
		private static AppEndUser CreateAppEndUserByIdAndUserName(int Id, string UserName)
		{
			Tuple<List<string>, List<string>> rr = GetAppEndUserRoles(Id);
			return new() { Id = Id, UserName = UserName, Roles = [.. rr.Item1], RoleNames = [.. rr.Item2] };
		}
		private static Tuple<List<string>, List<string>> GetAppEndUserRoles(int? userId)
		{
			List<string> roles = [];
			List<string> roleNames = [];

			if (userId is null) return new Tuple<List<string>, List<string>>([], []);
			string sqlRoles = "SELECT RoleId,RoleName FROM AAA_Users_Roles UsRs LEFT OUTER JOIN AAA_Roles ON UsRs.RoleId=AAA_Roles.Id WHERE UserId=" + userId;
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			DataTable dtRoles = dbIO.ToDataTable(sqlRoles)["Master"];
			if (dtRoles.Rows.Count > 0)
			{
				foreach (DataRow dr in dtRoles.Rows)
				{
					roles.Add(dr["RoleId"].ToStringEmpty());
					roleNames.Add(dr["RoleName"].ToStringEmpty());
				}
			}
			return new Tuple<List<string>, List<string>>(roles, roleNames);
		}
		private static void UpdateLoginTry(int userId, bool res, int count)
		{
			string sql = (res == true 
				?
				"UPDATE AAA_Users SET LoginTryFailLastOn=GETDATE(),LoginTryFails=" + (count == -1 ? "0" : "ISNULL(LoginTryFails,0)+1") + " WHERE Id=" + userId
				:
				"UPDATE AAA_Users SET LoginTrySuccessLastOn=GETDATE(),LoginTrySuccesses=" + (count == -1 ? "0" : "ISNULL(LoginTryFails,0)+1") + ",LoginTryFails=0,LoginTryFailLastOn=NULL WHERE Id=" + userId
				);
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			dbIO.ToNoneQuery(sql);
		}
		private static void UpdateLoginLocked(int userId, bool lockState)
		{
			string sql = "UPDATE AAA_Users SET LoginLockedUpdatedOn=GETDATE(),LoginLocked=" + (lockState == true ? "1" : "0") + " WHERE Id=" + userId;
			DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
			dbIO.ToNoneQuery(sql);
		}
		#endregion
	}
}

