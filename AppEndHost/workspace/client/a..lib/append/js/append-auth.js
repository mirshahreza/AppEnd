// append-auth.js
// Authentication and Authorization functions

// Clear legacy token from storage on load (cookie-based auth no longer uses it)
(function () { try { localStorage.removeItem("token"); sessionStorage.removeItem("token"); } catch (e) { } })();

/**
 * Check if user is logged in
 * Relies on appendauth flag (set on successful login, cleared on logout). Token is httpOnly and not readable by client.
 */
function isLogedIn() {
    return fixNull(sessionStorage.getItem("appendauth"), "") === "1";
}

/**
 * Restore session from refresh_token cookie when appendauth is empty (e.g. tab refresh, new tab).
 * Call before deciding login vs app. Uses rpcSync(RefreshToken) with withCredentials to send cookie.
 * If refresh succeeds, sets appendauth so isLogedIn() becomes true.
 */
function tryRestoreSessionFromCookie() {
    if (isLogedIn()) return;
    try {
        var r = rpcSync({ requests: [{ Method: "Zzz.AppEndProxy.RefreshToken", Inputs: {} }] })[0];
        if (r && r.IsSucceeded === true && r.Result && r.Result.Result === true) {
            setAsLogedIn();
        }
    } catch (e) { }
}

/**
 * Set user as logged in
 * Server sets httpOnly cookies. Token expiry is backend-only; frontend reacts to 401 by calling RefreshToken and retrying.
 */
function setAsLogedIn() {
    sessionStorage.setItem("appendauth", "1");
}

/**
 * Set user as logged out
 * Clears client state and any legacy token from storage. Server response from Logout expires the cookies.
 */
function setAsLogedOut() {
    sessionStorage.removeItem("appendauth");
    sessionStorage.removeItem("userContext");
    sessionStorage.removeItem("token");
    localStorage.removeItem("token");
    shared.fake = null;
}

/**
 * Get user token - always empty with cookie auth (JWT is httpOnly, not readable by client)
 * Kept for API compatibility.
 */
function getUserToken() {
    return "";
}

/** Safe nobody object - prevents "Cannot read properties of undefined" in templates */
var _nobodyObject = { UserName: "nobody", RoleNames: [], Roles: [] };

/**
 * Get user object from server context (cookie auth)
 * JWT is httpOnly; user info comes from GetLogedInUserContext.
 * Always returns an object so templates can safely access .UserName and .RoleNames.
 */
function getUserObject() {
    try {
        if (isLogedIn()) {
            var ctx = getLogedInUserContext();
            if (ctx && typeof ctx === 'object' && (ctx.UserName || ctx.AllowedActions)) return ctx;
        }
    } catch (e) { }
    return _nobodyObject;
}

/**
 * Check if user is in specified roles
 */
function isInRole(allowedRoles) {
    let userRoles = getUserRoles();
    let intersect = _.intersection(allowedRoles, userRoles)
    return intersect.length > 0;
}

/**
 * Check if user is denied
 */
function isDenied(deniedUsers) {
    userDenieds = getUserDeniedes();
    let intersect = _.intersection(deniedUsers, userDenieds)
    return intersect.length > 0;
}

/**
 * Check if user is allowed
 */
function isAllowed(allowedUsers) {
    userAlloweds = getUserAlloweds();
    let intersect = _.intersection(allowedUsers, userAlloweds)
    return intersect.length > 0;
}

/**
 * Get user roles
 */
function getUserRoles() {
    let userRoles = [];
    _.forEach(getUserObject().Roles, function (i) {
        userRoles.push(i)
    });
    return userRoles;
}

function hasColumnViewAccess(columnAccess, columnName) {
    if (!columnAccess || !columnAccess[columnName]) return true;
    if (isAdmin() || isPublicKey()) return true;
    let access = columnAccess[columnName];
    let deniedUsers = fixNull(access.DeniedUsers, []);
    let deniedRoles = fixNull(access.DeniedRoles, []);
    let userObj = getUserObject();
    let userName = (typeof userObj === 'string') ? '' : fixNull(userObj.UserName, '');
    if (deniedUsers.includes("*") || deniedRoles.includes("*")) return false;
    if (userName !== '' && _.intersection([userName], deniedUsers).length > 0) return false;
    if (_.intersection(getUserRoles(), deniedRoles).length > 0) return false;
    return true;
}

function hasColumnEditAccess(columnAccess, columnName) {
    if (!columnAccess || !columnAccess[columnName]) return true;
    if (isAdmin() || isPublicKey()) return true;
    let access = columnAccess[columnName];
    let deniedUsers = fixNull(access.DeniedUsers, []);
    let deniedRoles = fixNull(access.DeniedRoles, []);
    let userObj = getUserObject();
    let userName = (typeof userObj === 'string') ? '' : fixNull(userObj.UserName, '');
    if (deniedUsers.includes("*") || deniedRoles.includes("*")) return false;
    if (userName !== '' && _.intersection([userName], deniedUsers).length > 0) return false;
    if (_.intersection(getUserRoles(), deniedRoles).length > 0) return false;
    return true;
}

/**
 * Get user allowed actions
 */
function getUserAlloweds() {
    return getLogedInUserContext()["AllowedActions"];
}

/**
 * Check if user has public key role
 */
function hasPublicKeyRole() {
    return fixNull(getLogedInUserContext()["HasPublicKeyRole"], false);
}

/**
 * Check if user is public key
 */
function isPublicKey() {
    return getLogedInUserContext()["IsPublicKey"];
}

/**
 * Get logged in user context
 * Uses isLogedIn() to avoid recursion with getUserObject when using cookie auth
 */
function getLogedInUserContext() {
    if (!isLogedIn()) {
        return { "AllowedActions": [], "DeniedActions": [], "HasPublicKeyRole": false, "IsPublicKey": false, "Settings": {} };
    } else {
        if (isNaNOrEmpty(sessionStorage.getItem("userContext"))) {
            let res = rpcSync({ requests: [{ "Method": "Zzz.AppEndProxy.GetLogedInUserContext", "Inputs": {} }] });
            let ctx = R0R(res) || {};
            delete ctx.NewToken;
            sessionStorage.setItem("userContext", JSON.stringify(ctx));
        }
        
        let context = JSON.parse(sessionStorage.getItem("userContext"));
        if (!context || typeof context !== 'object') context = {};
        if (context.NewToken) {
            delete context.NewToken;
            sessionStorage.setItem("userContext", JSON.stringify(context));
        }
        
        // Apply saved theme from settings
        if (context.Settings && typeof ThemeManager !== 'undefined') {
            try {
                let settings = JSON.parse(context.Settings);
                if (settings.Theme) {
                    ThemeManager.setTheme(settings.Theme);
                }
            } catch (ex) {
                console.warn('Failed to parse user settings:', ex);
            }
        }
        
        return context;
    }
}

/**
 * Re-get logged in user context
 */
function reGetLogedInUserContext() {
    let res = rpcSync({ requests: [{ "Method": "Zzz.AppEndProxy.GetLogedInUserContext", "Inputs": {} }] });
    let ctx = R0R(res) || {};
    delete ctx.NewToken;
    sessionStorage.setItem("userContext", JSON.stringify(ctx));
    return ctx;
}

/**
 * Check if user is admin
 */
function isAdmin() {
    return (isPublicKey() === true) || (hasPublicKeyRole() === true);
}

/**
 * Logout user
 * Calls server to expire cookies; clears client state on response.
 */
function logout(after) {
    rpcAEP("Logout", {}, function (res) {
        setAsLogedOut();
        if (after) after();
    });
}

/**
 * Login user
 * Server sets httpOnly cookies on success.
 */
function login(loginInfo) {
    let rqst = { requests: [{ "Method": "Zzz.AppEndProxy.Login", "Inputs": loginInfo }] };
    let r = rpcSync(rqst)[0];
    if (r.IsSucceeded === true && fixNull(r.Result, '') !== '' && r.Result.Result === true) {
        setAsLogedIn();
        return true;
    } else {
        return false;
    }
}

/**
 * Login as another user (admin feature)
 * Server sets httpOnly cookies on success.
 */
function loginAs(loginAsUserName) {
    let rqst = { requests: [{ "Method": "Zzz.AppEndProxy.LoginAs", "Inputs": { "UserName": loginAsUserName } }] };
    let r = rpcSync(rqst)[0];
    if (r.IsSucceeded === true && fixNull(r.Result, '') !== '' && r.Result.Result === true) {
        setAsLogedOut();
        setAsLogedIn();
        return true;
    } else {
        return false;
    }
}

/**
 * Check if user is in roles or has actions
 */
function isInRolesOrActions(arrActionsStr, arrRolesStr) {
    if (isAdmin() || isPublicKey()) return true;
    arrActionsStr = fixNull(arrActionsStr, '');
    arrRolesStr = fixNull(arrRolesStr, '');
    if (arrActionsStr === '' && arrRolesStr === '') return true;
    let allowedByRoles = false;
    let allowedByActions = false;
    let arrActions = arrActionsStr === '' ? [] : arrActionsStr.split(',');
    let arrRoles = arrRolesStr === '' ? [] : arrRolesStr.split(',');
    let userRoles = getUserRoles();
    let userActions = getUserAlloweds();

    if (arrActions.length > 0) {
        if (_.intersection(arrActions, userActions).length > 0) allowedByActions = true;
    } else {
        allowedByActions = true;
    }

    if (arrRoles.length > 0) {
        if (_.intersection(arrRoles, userRoles).length > 0) allowedByRoles = true;
    } else {
        allowedByRoles = true;
    }

    if (allowedByActions === true && allowedByRoles === true) return true;
    return false;
}

/**
 * Refresh session
 * With httpOnly cookies, calls RefreshToken which sets new cookies via server response.
 */
function refereshSession() {
    let r = rpcSync({ requests: [{ "Method": "Zzz.AppEndProxy.RefreshToken", "Inputs": {} }] })[0];
    if (r && r.IsSucceeded === true && fixNull(r.Result, '') !== '' && r.Result.Result === true) {
        setAsLogedIn();
        reGetLogedInUserContext();
        return;
    }
    reGetLogedInUserContext();
}

/**
 * Get user settings
 */
function getUserSettings() {
    let settings = {};
    let uContext = getLogedInUserContext();
    if (fixNull(uContext["Settings"], '') !== '') settings = JSON.parse(uContext["Settings"]);
    
    // If theme not in settings, get current theme from ThemeManager
    if (!settings.Theme && typeof ThemeManager !== 'undefined') {
        settings.Theme = ThemeManager.getCurrentTheme();
    }
    
    return settings;
}

/**
 * Set user settings
 */
function setUserSettings(settings) {
    let uContext = getLogedInUserContext();
    uContext["Settings"] = settings;
    delete uContext.NewToken;
    sessionStorage.setItem("userContext", JSON.stringify(uContext));
    
    // Apply theme if it changed
    if (settings.Theme && typeof ThemeManager !== 'undefined') {
        ThemeManager.setTheme(settings.Theme);
    }
    
    rpcAEP("SaveUserSettings", { "Settings": JSON.stringify(settings) }, function (res) { });
    refereshSession();
}

/**
 * Set user shortcuts
 */
function setUserShortcuts(shortcuts) {
    let uSettings = getUserSettings();
    uSettings["Shortcuts"] = shortcuts;
    setUserSettings(uSettings);
}

/**
 * Get user shortcuts
 */
function getUserShortcuts() {
    let shortCuts = [];
    let uSettings = getUserSettings();
    if (fixNull(uSettings["Shortcuts"], '') !== '') shortCuts = uSettings["Shortcuts"];
    return shortCuts;
}
