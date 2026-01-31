// append-auth.js
// Authentication and Authorization functions

/**
 * Check if user is logged in
 */
function isLogedIn() {
    if (getUserToken() === "") return false;
    return true;
}

/**
 * Set user as logged in
 */
function setAsLogedIn(token, remember) {
    if (remember === true) {
        localStorage.setItem("token", token);
    } else {
        sessionStorage.setItem("token", token);
    }
}

/**
 * Set user as logged out
 */
function setAsLogedOut() {
    sessionStorage.clear();
    localStorage.clear();
    shared.fake = null;
}

/**
 * Get user token
 */
function getUserToken() {
    if (fixNull(localStorage.getItem("token"), '') !== '') return localStorage.getItem("token");
    if (fixNull(sessionStorage.getItem("token"), '') !== '') return sessionStorage.getItem("token");
    return "";
}

/**
 * Get user object from JWT token
 */
function getUserObject() {
    if (isLogedIn()) {
        return decodeJwt(getUserToken()).payload;
    } else {
        return 'nobody';
    }
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
 */
function getLogedInUserContext() {
    if (getUserObject() === "nobody") {
        return { "AllowedActions": [], "DeniedActions": [], "HasPublicKeyRole": false, "IsPublicKey": false, "Settings": {} };
    } else {
        if (isNaNOrEmpty(sessionStorage.getItem("userContext"))) {
            let res = rpcSync({ requests: [{ "Method": "Zzz.AppEndProxy.GetLogedInUserContext", "Inputs": {} }] });
            sessionStorage.setItem("userContext", JSON.stringify(R0R(res)));
        }
        
        let context = JSON.parse(sessionStorage.getItem("userContext"));
        
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
    sessionStorage.setItem("userContext", JSON.stringify(R0R(res)));
    return JSON.parse(sessionStorage.getItem("userContext"));
}

/**
 * Check if user is admin
 */
function isAdmin() {
    return (isPublicKey() === true) || (hasPublicKeyRole() === true);
}

/**
 * Logout user
 */
function logout(after) {
    rpcAEP("Logout", {}, function (res) {
        setAsLogedOut();
        if (after) after();
    });
}

/**
 * Login user
 */
function login(loginInfo) {
    let rqst = { requests: [{ "Method": "Zzz.AppEndProxy.Login", "Inputs": loginInfo }] };
    let r = rpcSync(rqst)[0];
    if (r.IsSucceeded === true && fixNull(r.Result, '') !== '' && r.Result.Result === true) {
        setAsLogedIn(r.Result.token, loginInfo.RememberMe);
        return true;
    } else {
        return false;
    }
}

/**
 * Login as another user (admin feature)
 */
function loginAs(loginAsUserName) {
    let rqst = { requests: [{ "Method": "Zzz.AppEndProxy.LoginAs", "Inputs": { "UserName": loginAsUserName } }] };
    let r = rpcSync(rqst)[0];
    if (r.IsSucceeded === true && fixNull(r.Result, '') !== '' && r.Result.Result === true) {
        setAsLogedOut();
        setAsLogedIn(r.Result.token, false);
        return true;
    } else {
        return false;
    }
}

/**
 * Start Google OAuth login flow
 */
function loginWithGoogle() {
    window.location.href = '/auth/google/start';
}

/**
 * Complete Google OAuth login with IdToken
 */
function loginWithGoogleToken(idToken) {
    let rqst = { requests: [{ "Method": "Zzz.AppEndProxy.LoginWithGoogle", "Inputs": { "IdToken": idToken } }] };
    let r = rpcSync(rqst)[0];
    if (r.IsSucceeded === true && fixNull(r.Result, '') !== '' && r.Result.Result === true) {
        setAsLogedIn(r.Result.token, false);
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
 * Refresh session with new token
 */
function refereshSession() {
    let cntx = reGetLogedInUserContext();
    setAsLogedOut();
    setAsLogedIn(cntx["NewToken"], false);
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
