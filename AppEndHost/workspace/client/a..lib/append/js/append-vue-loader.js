// append-vue-loader.js
// Dynamic Vue SFC loading without build

/**
 * Load Vue component dynamically
 */
function loadVM(componentPath) {
    const { loadModule } = window["vue3-sfc-loader"];
    const options = {
        moduleCache: { vue: Vue },
        getFile(url) {
            return fetch(url + "?fake=" + shared.fake, { credentials: "include" }).then(resp => resp.ok ? resp.text().then(i => trimUi(i)) : Promise.reject(resp));
        },
        addStyle(styleStr) {
            const style = document.createElement("style");
            style.textContent = styleStr;
            const ref = document.head.getElementsByTagName("style")[0] || null;
            document.head.insertBefore(style, ref);
        }
    };
    return Vue.defineAsyncComponent(() => loadModule(componentPath, options));
}

/**
 * Load JSON file
 */
function loadJSON(filePath) {
    return JSON.parse(loadTextFileAjaxSync(filePath, "application/json"));
}

/**
 * Load text file synchronously via AJAX
 */
function loadTextFileAjaxSync(filePath, mimeType) {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open("GET", filePath, false);
    if (mimeType != null) {
        if (xmlhttp.overrideMimeType) {
            xmlhttp.overrideMimeType(mimeType);
        }
    }
    xmlhttp.send();
    if (xmlhttp.status == 200 && xmlhttp.readyState == 4) {
        return xmlhttp.responseText;
    }
    else {
        return null;
    }
}

/**
 * Get app configuration
 */
function getAppConfig() {
    let appName = getThemeName();
    let appConfPath = `/${appName}/app.json?fake=${shared.fake}`;
    return getSessionItemSync("appsettings_" + appName, true, function () { return loadJSON(appConfPath); });
}

/**
 * Get app navigation
 */
function getAppNav() {
    let appName = getThemeName();
    return getSessionItemSync("appnav_" + appName, true, function () { return trimNav(_.cloneDeep(getAppConfig().navigation)); });
}

/**
 * Trim navigation based on user roles
 */
function trimNav(nav) {
    if (fixNull(nav, '') !== '') {
        let finalArr = [];
        _.forEach(nav, function (cat) {
            let newCat = null;
            if (isInRolesOrActions(cat["actions"], cat["roles"]) === true) {
                newCat = _.cloneDeep(cat);
                newCat["items"] = [];
                _.forEach(cat["items"], function (navItem) {
                    if (isInRolesOrActions(navItem["actions"], navItem["roles"]) === true) {
                        newCat["items"].push(_.cloneDeep(navItem));
                    }
                });
                finalArr.push(newCat);
            }
        });
        return finalArr;
    }
    return [];
}

/**
 * Trim UI elements based on user roles and actions
 */
function trimUi(str) {
    if (isPublicKey() === true || hasPublicKeyRole() === true) return str;
    let o = $("<div>" + str.replace("<template>", "<myhtmltag>").replace("</template>", "</myhtmltag>") + "</div>");

    o.find("[data-ae-actions]").each(function () {
        let elm = $(this);
        let strActions = elm.attr("data-ae-actions").trim();
        if (strActions !== "") {
            let tagAllowed = strActions.split(',');
            let userAllowed = getUserAlloweds();
            let intersect = _.intersection(tagAllowed, userAllowed);
            if (intersect.length === 0) elm.remove();
        }
    });

    o.find("[data-ae-roles]").each(function () {
        let elm = $(this);
        let strRoles = elm.attr("data-ae-roles").trim();
        if (strRoles !== "") {
            let tagRoles = strRoles.split(',');
            let userRoles = getUserRoles();
            let intersect = _.intersection(tagRoles, userRoles);
            if (intersect.length === 0) elm.remove();
        }
    });

    return o.html().replace("<myhtmltag>", "<template>").replace("</myhtmltag>", "</template>");
}

/**
 * Get theme name from URL
 */
function getThemeName() {
    return new URL(window.location).pathname.replaceAll(`/`,'');
}

/**
 * Get path
 */
function getPath() {
    return document.location.pathname.replaceAll('/', '');
}

/**
 * Get query string parameter
 */
function getQueryString(queryStringName) {
    let urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(queryStringName);
}

/**
 * Set query string parameter
 */
function setQueryString(queryStringName, val) {
    if (history.pushState) {
        var params = new URLSearchParams(window.location.search);
        params.set(queryStringName, val);
        var newUrl = window.location.origin + window.location.pathname + '?' + params.toString();
        window.history.pushState({ path: newUrl }, '', newUrl);
    }
}

/**
 * Remove query string parameter
 */
function removeQueryString(queryStringName) {
    if (history.pushState) {
        var params = new URLSearchParams(window.location.search);
        params.delete(queryStringName);
        var newUrl = window.origin + window.location.pathname + '?' + params.toString();
        window.history.pushState({ path: newUrl }, '', newUrl);
    }
}

/**
 * Translate key
 */
function translate(k) {
    if (shared.getAppConfig()["translation"] !== undefined && fixNull(shared.getAppConfig()["translation"][k], '') !== '') {
        return shared.getAppConfig()["translation"][k];
    } else {
        return k;
    }
}

/**
 * Get current language
 */
function getCurrentLang(k) {
    return fixNull(shared.getAppConfig()["lang"], 'En');
}
