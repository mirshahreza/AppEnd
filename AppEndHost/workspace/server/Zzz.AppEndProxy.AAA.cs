namespace Zzz
{
    public static partial class AppEndProxy
    {
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
            string sqlUserRecord = "SELECT Id,RoleId,AttributeId FROM BaseRolesAttributes WHERE RoleId=" + RoleId;
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            DataTable dtAttributes = dbIO.ToDataTable(sqlUserRecord)["Master"];
            return dtAttributes;
        }
        public static object? SetAttributesByRoleId(string AttributeId, string RoleId, bool Flag)
        {
            string sql = Flag == true
                ? $"INSERT INTO BaseRolesAttributes (RoleId,AttributeId) VALUES ({RoleId},{AttributeId})"
                : $"DELETE BaseRolesAttributes WHERE RoleId={RoleId} AND AttributeId={AttributeId}";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            dbIO.ToNoneQuery(sql);
            return true;
        }
        public static object? GetAttributesByUserId(string UserId)
        {
            string sqlUserRecord = "SELECT Id,UserId,AttributeId FROM BaseUsersAttributes WHERE UserId=" + UserId;
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            DataTable dtAttributes = dbIO.ToDataTable(sqlUserRecord)["Master"];
            return dtAttributes;
        }
        public static object? SetAttributesByUserId(string AttributeId, string UserId, bool Flag)
        {
            string sql = Flag == true
                ? $"INSERT INTO BaseUsersAttributes (UserId,AttributeId) VALUES ({UserId},{AttributeId})"
                : $"DELETE BaseUsersAttributes WHERE UserId={UserId} AND AttributeId={AttributeId}";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            dbIO.ToNoneQuery(sql);
            return true;
        }

        public static object? SaveUserSettings(AppEndUser? Actor, string Settings)
        {
            if (Actor == null) return false;
            string sqlUpdateUserSettings = "UPDATE BaseUsers SET Settings=N'" + Settings + "' WHERE Id=" + Actor.Id;
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
                    string pass = drUser["Password"].ToStringEmpty().Trim();
                    bool passwordMatch = pass.Equals(Password.GetMD4Hash(), StringComparison.OrdinalIgnoreCase) || pass.Equals(Password.GetMD5Hash(), StringComparison.OrdinalIgnoreCase);

                    if (passwordMatch && drUser["LoginLocked"].ToBooleanSafe() == false && drUser["IsActive"].ToBooleanSafe(true) == true)
                    {
                        AppEndUser appEndUser = CreateAppEndUserByIdAndUserName(drUser["Id"].ToIntSafe(), drUser["UserName"].ToStringEmpty());
                        UpdateLoginTry(drUser["Id"].ToIntSafe(), true, -1);
                        string accessToken = appEndUser.CreateAccessToken(15);
                        string refreshToken = GenerateRefreshToken();
                        StoreRefreshToken(drUser["Id"].ToIntSafe(), refreshToken, 7);
                        kvp.Add("access_token", accessToken);
                        kvp.Add("refresh_token", refreshToken);
                        kvp.Add("Result", true);
                    }
                    else
                    {
                        if (drUser["LoginTryFailsCount"].ToIntSafe(0) < 4)
                        {
                            UpdateLoginTry(drUser["Id"].ToIntSafe(), false, drUser["LoginTryFailsCount"].ToIntSafe(0));
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
                    AppEndUser appEndUser = CreateAppEndUserByIdAndUserName(drUser["Id"].ToIntSafe(), drUser["UserName"].ToStringEmpty());
                    string accessToken = appEndUser.CreateAccessToken(15);
                    string refreshToken = GenerateRefreshToken();
                    StoreRefreshToken(drUser["Id"].ToIntSafe(), refreshToken, 7);
                    kvp.Add("access_token", accessToken);
                    kvp.Add("refresh_token", refreshToken);
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
            string storedPass = drUser["Password"].ToStringEmpty().Trim();
            if (!storedPass.Equals(OldPassword.GetMD4Hash(), StringComparison.OrdinalIgnoreCase) && !storedPass.Equals(OldPassword.GetMD5Hash(), StringComparison.OrdinalIgnoreCase)) return false;
            string sql = "UPDATE BaseUsers SET PasswordUpdatedBy=" + Actor.ContextInfo?["UserId"] + ",PasswordUpdatedOn=GETDATE(),Password='" + NewPassword.GetMD5Hash() + "' WHERE Id=" + drUser["Id"];
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            dbIO.ToNoneQuery(sql);
            return true;
        }
        public static object? Logout(AppEndUser? Actor)
        {
            if (Actor == null) return false;
            RevokeRefreshTokenForUser(Actor.Id);
            AppEndCache.Remove(Actor.ContextCacheKey());
            return true;
        }

        public static object? RefreshToken(string? refreshToken)
        {
            Dictionary<string, object> kvp = [];
            if (string.IsNullOrEmpty(refreshToken))
            {
                LogMan.LogActivity("Zzz", "AppEndProxy", "RefreshToken", "", false, false, "", "Token rotation failed: no refresh token", 0, "", "", -1, "");
                kvp.Add("Result", false);
                return kvp;
            }
            int? userId = ValidateAndRevokeRefreshToken(refreshToken);
            if (userId == null)
            {
                LogMan.LogActivity("Zzz", "AppEndProxy", "RefreshToken", "", false, false, "", "Token rotation failed: invalid or expired refresh token", 0, "", "", -1, "");
                kvp.Add("Result", false);
                return kvp;
            }
            DataRow? drUser = GetUserRowById(userId.Value);
            if (drUser == null || drUser["IsActive"].ToBooleanSafe(true) == false)
            {
                LogMan.LogActivity("Zzz", "AppEndProxy", "RefreshToken", "", false, false, "", "Token rotation failed: user not found or inactive", 0, "", "", userId.Value, "");
                kvp.Add("Result", false);
                return kvp;
            }
            AppEndUser appEndUser = CreateAppEndUserByIdAndUserName(drUser["Id"].ToIntSafe(), drUser["UserName"].ToStringEmpty());
            string newAccessToken = appEndUser.CreateAccessToken(15);
            string newRefreshToken = GenerateRefreshToken();
            StoreRefreshToken(drUser["Id"].ToIntSafe(), newRefreshToken, 7);
            kvp.Add("access_token", newAccessToken);
            kvp.Add("refresh_token", newRefreshToken);
            kvp.Add("Result", true);
            LogMan.LogActivity("Zzz", "AppEndProxy", "RefreshToken", "", true, false, "", "Token rotation succeeded",
                0, "", "", drUser["Id"].ToIntSafe(), drUser["UserName"].ToStringEmpty());
            return kvp;
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

            r.Add("UserName", Actor.UserName ?? "");
            r.Add("Roles", newActor.Roles);
            r.Add("RoleNames", newActor.RoleNames);
            r.Add("IsPublicKey", AppEndSettings.PublicKeyUser.EqualsIgnoreCase(Actor.UserName));
            r.Add("HasPublicKeyRole", newActor.RoleNames.ContainsIgnoreCase(AppEndSettings.PublicKeyRole.ToLower()));

            string sqlUserRecord = @$"
SELECT 
    Id,UserName,Email,Mobile
    ,(SELECT TOP 1 BP.Picture_FileBody_xs FROM BasePersons BP WHERE BP.UserId=BaseUsers.Id) Picture_FileBody_xs
    ,Settings 
FROM BaseUsers 
WHERE UserName='{Actor.UserName}'";

            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            DataRow drUser = dbIO.ToDataTable(sqlUserRecord)["Master"].Rows[0];
            r.Add("Email", drUser["Email"] is System.DBNull ? "" : drUser["Email"].ToStringEmpty());
            r.Add("Mobile", drUser["Mobile"] is System.DBNull ? "" : drUser["Mobile"].ToStringEmpty());
            r.Add("Picture_FileBody_xs", drUser["Picture_FileBody_xs"] is System.DBNull ? "" : drUser["Picture_FileBody_xs"]);
            r.Add("Settings", drUser["Settings"] is System.DBNull || drUser["Settings"].ToStringEmpty() == "" ? "{}" : (string)drUser["Settings"]);
            // NewToken removed: auth uses httpOnly cookies; refresh via Zzz.AppEndProxy.RefreshToken

            return r;
        }
        public static Hashtable CreateUserServerContext(AppEndUser? Actor)
        {
            // Dont remove Roles , Just add your own keys if needed
            Tuple<List<string>, List<string>> rr = GetAppEndUserRoles(Actor?.Id);
            Hashtable r = new()
            {
                { "Roles", rr.Item1 },
                { "IsPublicKey", AppEndSettings.PublicKeyUser.EqualsIgnoreCase(Actor?.UserName) },
                { "HasPublicKeyRole", rr.Item1.ContainsIgnoreCase(AppEndSettings.PublicKeyRole.ToLower()) }
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
            string sqlUserRecord = $"SELECT Id,UserName,Email,Mobile,Password,IsActive,LoginLocked,LoginTryFailsCount,LoginTryFailLastOn,LoginTrySuccessesCount,LoginTrySuccessLastOn FROM BaseUsers WHERE UserName='{un}'";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            DataTable dtUser = dbIO.ToDataTable(sqlUserRecord)["Master"];
            if (dtUser.Rows.Count > 0) return dtUser.Rows[0];
            return null;
        }

        private static DataRow? GetUserRowById(int userId)
        {
            string sqlUserRecord = $"SELECT Id,UserName,Email,Mobile,Password,IsActive,LoginLocked FROM BaseUsers WHERE Id={userId}";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            DataTable dtUser = dbIO.ToDataTable(sqlUserRecord)["Master"];
            if (dtUser.Rows.Count > 0) return dtUser.Rows[0];
            return null;
        }

        private static string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static void StoreRefreshToken(int userId, string refreshToken, int validDays)
        {
            string tokenHash = refreshToken.GetSHA256Hash();
            DateTime createdOn = DateTime.UtcNow;
            DateTime expiryDate = createdOn.AddDays(validDays);
            string sql = $"UPDATE BaseUsers SET RefreshTokenHash=N'{tokenHash.Replace("'", "''")}',RefreshTokenExpiryDate='{expiryDate:yyyy-MM-dd HH:mm:ss}',RefreshTokenCreatedOn='{createdOn:yyyy-MM-dd HH:mm:ss}' WHERE Id={userId}";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            dbIO.ToNoneQuery(sql);
        }

        private static int? ValidateAndRevokeRefreshToken(string refreshToken)
        {
            string tokenHash = refreshToken.GetSHA256Hash();
            string sql = $"SELECT Id,RefreshTokenExpiryDate FROM BaseUsers WHERE RefreshTokenHash=N'{tokenHash.Replace("'", "''")}' AND RefreshTokenHash IS NOT NULL";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            DataTable dt = dbIO.ToDataTable(sql)["Master"];
            if (dt.Rows.Count == 0) return null;
            DataRow dr = dt.Rows[0];
            if (dr["RefreshTokenExpiryDate"] is DBNull) return null;
            if (DateTime.UtcNow > ((DateTime)dr["RefreshTokenExpiryDate"])) return null;
            int userId = dr["Id"].ToIntSafe();
            RevokeRefreshTokenForUser(userId);
            return userId;
        }

        /// <summary>Revokes refresh token for user (e.g. on Logout). Clears token so it cannot be reused.</summary>
        private static void RevokeRefreshTokenForUser(int userId)
        {
            string sql = $"UPDATE BaseUsers SET RefreshTokenHash=NULL,RefreshTokenExpiryDate=NULL,RefreshTokenCreatedOn=NULL WHERE Id={userId}";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            dbIO.ToNoneQuery(sql);
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
            string sqlRoles = $"SELECT RoleId,RoleName FROM BaseUsersRoles UsRs LEFT OUTER JOIN BaseRoles ON UsRs.RoleId=BaseRoles.Id WHERE UserId={userId}";
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
            string sql = (res == false
                ?
                $"UPDATE BaseUsers SET LoginTryFailLastOn=GETDATE(),LoginTryFailsCount=ISNULL(LoginTryFailsCount,0)+1"
                :
                $"UPDATE BaseUsers SET LoginTrySuccessLastOn=GETDATE(),LoginTrySuccessesCount=ISNULL(LoginTryFailsCount,0)+1,LoginTryFailsCount=0,LoginTryFailLastOn=NULL"
                );
            sql += $" WHERE Id={userId}";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            dbIO.ToNoneQuery(sql);
        }
        private static void UpdateLoginLocked(int userId, bool lockState)
        {
            string sql = $"UPDATE BaseUsers SET LoginLockedUpdatedOn=GETDATE(),LoginLocked={(lockState == true ? "1" : "0")} WHERE Id={userId}";
            DbIO dbIO = DbIO.Instance(DbConf.FromSettings(AppEndSettings.LoginDbConfName));
            dbIO.ToNoneQuery(sql);
        }
        #endregion
    }
}

