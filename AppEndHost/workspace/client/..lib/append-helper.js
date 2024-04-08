var vApp;
var vInstance;

var shared = {
    ld() { return _; },
    debug: true,
    talkPoint: "/talk-to-me/",
    heavyWorkingCover: `<span></span>`,
    notHeavyWorkingCover: `<span></span>`,
    miniHeavyWorkingCover: `<span></span>`,
    defaultDb: 'DefaultRepo',
    biClass: 'Common_BaseInfo',
    biCacheTime: 10,
    translate(k) { return translate(k); },
    getImageURI(imageBytes) { return getImageURI(imageBytes); },
    openComponent(src, options) { return openComponent(src, options); },
    openComponentByEl(evt) { return openComponentByEl(evt); },
    closeComponent(cid) { closeComponent(cid); },
    getAppConfig() { return getAppConfig(); },
    getAppNav() { return getAppNav() },
    setAppTitle(title) { setAppTitle(title); },
    setAppSubTitle(title) { setAppSubTitle(title); },
    setAppMessage(msg, dur, cssClasses) { setAppMessage(msg, dur, cssClasses); },

    getQueryString(paramName) { return getQueryString(paramName); },

    isLogedIn() { return isLogedIn(); },
    setAsLogedOut() { return setAsLogedOut(); },
    getUserToken() { return getUserToken(); },
    getUserObject() { return getUserObject(); },
    isInRole(allowedRoles) { return isInRole(allowedRoles); },
    isDenied(deniedUsers) { return isDenied(deniedUsers); },
    isAllowed(allowedUsers) { return isAllowed(allowedUsers); },
    getUserRoles() { return getUserRoles(); },
    getUserAlloweds() { return getUserAlloweds(); },
    hasPublicKeyRole() { return hasPublicKeyRole(); },
    isPublicKey() { return isPublicKey(); },
    getLogedInUserContext() { return getLogedInUserContext(); },
    isAdmin() { return isAdmin(); },
    logout(after) { return logout(after); },
    login(loginInfo) { return login(loginInfo); },
    showConfirm(options) { showConfirm(options); },
    showPrompt(options) { showPrompt(options); },
    fixNull(val, isNullVal, checkUndefinedOrNullText) { return fixNull(val, isNullVal, checkUndefinedOrNullText); },
    fixNullOrEmpty(o1, o2) { return fixNullOrEmpty(o1, o2); },

    getResponseObjectById(o, id) { return getResponseObjectById(o, id); },
    getObjectById(o, id) { return getObjectById(o, id); },

    formatDate(d) { return formatDate(d); },
    formatDateTime(d) { return formatDateTime(d); },
    convertBoolToIcon(v, trueClasses, falseClasses, nullClasses) { return convertBoolToIcon(v, trueClasses, falseClasses, nullClasses); },
    convertBoolToIconWithOptions(v, options) { return convertBoolToIconWithOptions(v, options); },

    toSimpleArrayOf(arr, colName) { return toSimpleArrayOf(arr, colName); },
    usableSubmits(submits, templateName) { return usableSubmits(submits, templateName); },
    usableLoads(loads, templateName) { return usableLoads(loads, templateName); },
    bytesToSize(bytes) { return bytesToSize(bytes); },

    getEditorMode(str) { return getEditorMode(str); },
    getEditorName(mode) { return getEditorName(mode); },

    getThemeName() { return getPath(); },

    getUserSettings() { return getUserSettings(); },


    prevTab(evt) { prevTab(evt); },
    nextTab(evt) { nextTab(evt); },


};

function prevTab(evt) {
    let clickedBtn = $(evt.currentTarget);
}
function nextTab(evt) {
    let clickedBtn = $(evt.currentTarget);
}

function getResponseObjectById(arr, id) {
    let r = _.filter(arr, function (i) { return i.Id === id; });
    if (r.length === 0) return [];
    r = r[0];
    if (fixNull(r, '') === '' || fixNull(r.Result, '') === '' || fixNull(r.Result.Master, '') === '') return [];
    return r.Result.Master;
}
function getObjectById(arr, id) {
    let r = _.filter(arr, function (i) { return i.Id === id; })[0];
    if (fixNull(r,'') === '') return [];
    return r;
}
function setAppTitle(title) {
    $(".app-title").html(title);
    document.title = getAppConfig().title + " :: " + title;
    setAppMessage(translate("Ready"), 5000, 'text-success');
}
function setAppSubTitle(title) {
    $(".app-subtitle").html(title);
    document.title = document.title + " :: " + title;
    setAppMessage("Ready", 5000, 'text-success');
}
function setAppMessage(msg, dur, cssClasses) {
    dur = fixNull(dur, 5000);
    $("#appMessage").html(msg);
    if (fixNull(cssClasses) !== '') $("#appMessage").addClass(cssClasses);
    setTimeout(function () {
        $("#appMessage").html("");
        if (fixNull(cssClasses) !== '') $("#appMessage").removeClass(cssClasses);
    }, dur);
}



function initVueComponent(_this) {
    initComponent(_this);
}
function initComponent(_this) {
    setTimeout(function () {
        $(`#${_this.cid} .ae-focus`).focus();
        $(`.scrollable`).overlayScrollbars({});
        _this.regulator = $(`#${_this.cid}`).inputsRegulator();
        runWidgets();
    }, 200);
}
function initPage() {
    shared.heavyWorkingCover = $(".static-working-cover").get(0).outerHTML;
    shared.notHeavyWorkingCover = shared.heavyWorkingCover.replace("white", "transparent");
    shared.miniHeavyWorkingCover = shared.heavyWorkingCover.replace("background-color: white !important;", "background-color: white !important;opacity:.5;").replace("static-working-cover", "static-working-cover-busy");
    $(".static-working-cover").get(0).remove();
    $("body").append(`<div id="topEndToastContainer" class="position-fixed top-0 end-0 p-3" style="z-index:99999;"></div><div id="mytemp"></div>`);
}
function runWidgets() {
    $("[data-ae-widget]").each(function () {
        let elm = $(this);
        if (fixNullOrEmpty(elm.attr("data-ae-widget-done"), "0") === "0") {
            elm.attr("data-ae-widget-done", "1");
            let widgetFunc = elm.attr("data-ae-widget");
            let opts = fixNullOrEmpty(elm.attr("data-ae-widget-options"), '');
            try {
                let ev = `elm.${widgetFunc}(${opts})`;
                if (widgetFunc === "trumbowyg") {
                    ev = ev + `.on('tbwchange', function () { elm.get(0).dispatchEvent(new Event('input', { bubbles: true })); })`;
                }
                log(ev);
                let w = eval(ev + ";");
            } catch (ex) {
                elm.html(ex.message);
            }
        }
    });
}
function showConfirm(options) {
    options = _.defaults(options, {
        title: "",
        message1: "",
        message1Class: "text-secondary fw-bold fs-d9",
        message2: "",
        message2Class: "text-primary fw-bold fs-1d2",
        cancelText: "Cancel",
        cancelClass: "btn btn-sm btn-secondary w-100 py-2",
        okText: "Ok",
        okClass: "btn btn-sm btn-primary w-100 py-2",
        callback: null
    });

    openComponent("/.publiccomponents/baseConfirm", { title: options.title, resizable: false, draggable: false, params: options });
}
function showPrompt(options) {
    options = _.defaults(options, {
        title: "",
        message1: "",
        message1Class: "text-secondary fw-bold fs-d9",
        message2: "",
        message2Class: "text-primary fw-bold fs-1d2",
        cancelText: "Cancel",
        cancelClass: "btn btn-sm btn-secondary w-100 py-2",
        okText: "Ok",
        okClass: "btn btn-sm btn-primary w-100 py-2",
        validation: {
            "required": true,
            "rule": "^[^a-zA-Z0-9]?.{1,128}$"
        },
        callback: null
    });

    openComponent("/.publiccomponents/basePrompt", { title: options.title, resizable: false, draggable: false, params: options });
}
function openComponentByEl(evt) {
    let el = $(evt.currentTarget);
    openComponent(el.attr("data-ae-src"), JSON.parse(fixNull(el.attr("data-ae-options"), '{}')));
}
function openComponent(src, options) {
    let id = genUN('overlay_component_');
    options = _.defaults(options, {
        id: id,
        sharpId: "#" + id,
        modal: true,
        placement: '',
        showHeader: true,
        title: '&nbsp;',
        caller: null,
        callback: null,
        showCloseButton: true,
        modalSize: '',
        modalBodyCSS: 'bg-light bg-gradient',
        closeByOverlay: false,
        headerCSS: 'bg-light bg-gradient',
        backdrop: true,
        border: 'border-4 border-secondary',
        resizable: true,
        draggable: true,
        params: {}
    });

    createWindow();
    shared["params_" + id] = options.params;
    function createWindow() {
        let dBody = $(getDialogHtml());
        $("body").append(dBody);
        $(document).ready(function () {
            const mdl = new bootstrap.Modal(options.sharpId, {});
            let app = Vue.createApp();
            app.config.globalProperties.shared = shared;
            app.component('comp-loader', loadVM("/.PublicComponents/baseComponentLoader.vue"));
            app.mount(options.sharpId);
            let m = document.getElementById(options.id);
            m.addEventListener('shown.bs.modal', () => {
                $("#c_" + options.id).attr("data-ae-ready", "true");
                $(`.scrollable`).overlayScrollbars({});
                //if (options.resizable === true) $(options.sharpId).find('.modal-content').resizable(); 
                //if (options.draggable === true) $(options.sharpId).find('.modal-content').draggable(); 
            });
            m.addEventListener('hidden.bs.modal', event => {
                setTimeout(function () { m.remove(); }, 1000);
            });
            mdl.show();
        });
    }
    function getDialogHtml() {
        let comp = `<comp-loader src="` + src + `" uid="c_` + options.id + `" cid="` + options.id + `" ismodal="true" />`;
        let modalClose = options.showCloseButton !== true ? "" : `<button type="button" class="btn-close p-0 mx-0" data-bs-dismiss="modal" aria-label="Close"></button>`;
        let modalHeader = options.showHeader ? `<div ondblclick="alert('${src}');" class="modal-header p-2 pb-1 ${options.headerCSS}"><span class="modal-title fb fs-d7">${shared.translate(options.title)}</span>${modalClose}</div>` : "";
        let modalBody = `<div class="modal-body p-0"><div class="h-100 ${options.modalBodyCSS}" data-ae-overlaycontainer="${id}">${comp}</div></div>`;
        let modalContent = `<div class="modal-content rounded-3 ${options.border} shadow-lg">${modalHeader}${modalBody}</div>`;
        let backdrop = options.backdrop === false ? 'data-bs-backdrop="false"' : (options.closeByOverlay === false ? 'data-bs-backdrop="static"' : '');
        let modalCss = `modal-dialog rounded-3 border-0 ${options.modalSize} ${options.placement} modal-fullscreen-lg-down`; // modal-dialog-scrollable
        return `<div class="modal fade" id="${id}" tabindex="-1" aria-hidden="true" ${backdrop}><div class="${modalCss}">${modalContent}</div></div>`;
    }
}
function closeComponent(cid) {
    let mdl = $("#" + cid);
    mdl.modal("hide");
}
function showWorking(workingCover, containerId) {
    let id = _.uniqueId("busy_") + "_" + _.random(100000, 9000000);
    let loader = workingCover.replace('<div ', '<div id="' + id + '" ');
    if (fixNull(containerId, '') !== '') $("#" + containerId).append(loader);
    else $("body").append(loader);
    return id;
}
function hideWorking(id) {
    $("#" + id).fadeOut(500, function () {
        $("#" + id).remove();
    });
}
function switchVisibility(switchHandler, targetSelector, initialVisibleState, initialIcon, secondIcon) {
    let clicked = $(switchHandler);
    $(targetSelector).toggleClass(initialVisibleState);
    clicked.find(".fa-solid").toggleClass(initialIcon);
    clicked.find(".fa-solid").toggleClass(secondIcon);
}
function showMoreInfo(elm) {
    let popOver = new bootstrap.Popover(elm, { trigger: 'focus', html: true });
    popOver.setContent({ '.popover-header': 'Auditing info', '.popover-body': '...' });
    popOver.show();
    $(".popover-body").html($(elm).parent().find(".more-info").html())
}
function showInfo(message) {
    showMessage({ "title1": shared.translate("Info"), "message": message, "type": "Info" });
}
function showSuccess(message) {
    showMessage({ "title1": shared.translate("Success"), "message": message, "type": "Success" });
}
function showError(message) {
    showMessage({ "title1": shared.translate("Error") , "message": message, "type": "Error" });
}
function showWarning(message) {
    showMessage({ "title1": shared.translate("Warning"), "message": message, "type": "Warning" });
}
function showMessage(options) {
    let id = _.uniqueId("noty_") + "_" + _.random(100000, 9000000);
    options = _.defaults(options, {
        "title1": "", "title2": "",
        "message": fixNull(options.message, options.type),
        "showTime": 5000, "closeable": true,
        "type": fixNull(options.type, 'Info')
    });

    let _strong = fixNull(options.title1) === "" ? "" : `<strong class="me-auto">${options.title1}</strong>`;
    let _small = fixNull(options.title2) === "" ? "" : `<small>${options.title2}</small>`;
    let _close = options.closeable !== true ? "" : `<button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>`;
    let _typeBg = "bg-info";
    if (options.type === 'Error') _typeBg = "bg-danger";
    if (options.type === 'Success') _typeBg = "bg-success";
    if (options.type === 'Warning') _typeBg = "bg-warning";
    let _type = `<span class="${_typeBg} rounded fs-d8">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;`;

    $("#topEndToastContainer").append(`
<div id="${id}" class="toast" role="alert" aria-live="assertive" style="z-index:99999;">
  <div class="toast-header">${_type}${_strong}${_small}${_close}</div>
  <div class="toast-body">${options.message}</div>
</div>
`);
    try {
        $('#' + id).toast('show');
    }
    catch (ex) {
        log(options.message);
    }
}
function showJson(jsn) {
    if (jsn === null || jsn === undefined || jsn === '') return;
    let s = JSON.stringify(jsn).trim();
    if (s === '' || s === '{}') return;

    if (s.indexOf("AccessDenied") > -1) {
        showError(translate("AccessDenied"));
    } else {
        openComponent("/.publiccomponents/baseJsonView.vue", { title: "JsonView", modalSize: "modal-fullscreen", params: { jsonToView: jsn } });
    }
}

function getUserSettings() {
    let settings = {};
    let uContext = getLogedInUserContext();
    if (fixNull(uContext["Settings"], '') !== '') settings = JSON.parse(uContext["Settings"]);
    return settings;
}
function setUserSettings(settings) {
    let uContext = getLogedInUserContext();
    uContext["Settings"] = settings;
    sessionStorage.setItem("userContext", JSON.stringify(uContext));
    rpcAEP("SaveUserSettings", { "Settings": JSON.stringify(settings) }, function (res) { });
    refereshSession();
}
function setUserShortcuts(shortcuts) {
    let uSettings = getUserSettings();
    uSettings["Shortcuts"] = shortcuts;
    setUserSettings(uSettings);
}
function getUserShortcuts() {
    let shortCuts = [];
    let uSettings = getUserSettings();
    if (fixNull(uSettings["Shortcuts"], '') !== '') shortCuts = uSettings["Shortcuts"];
    return shortCuts;
}
function isLogedIn() {
    if (getUserToken() === "") return false;
    return true;
}
function setAsLogedIn(token, remember) {
    if (remember === true) {
        localStorage.setItem("token", token);
    } else {
        sessionStorage.setItem("token", token);
    }
}
function setAsLogedOut() {
    sessionStorage.clear();
    localStorage.clear();
}
function getUserToken() {
    if (fixNull(localStorage.getItem("token"), '') !== '') return localStorage.getItem("token");
    if (fixNull(sessionStorage.getItem("token"), '') !== '') return sessionStorage.getItem("token");
    return "";
}
function getUserObject() {
    if (isLogedIn()) {
        return decodeJwt(getUserToken()).payload;
    } else {
        return 'nobody';
    }
}
function isInRole(allowedRoles) {
    let userRoles = getUserRoles();
    let intersect = _.intersection(allowedRoles, userRoles)
    return intersect.length > 0;
}
function isDenied(deniedUsers) {
    userDenieds = getUserDeniedes();
    let intersect = _.intersection(deniedUsers, userDenieds)
    return intersect.length > 0;
}
function isAllowed(allowedUsers) {
    userAlloweds = getUserAlloweds();
    let intersect = _.intersection(allowedUsers, userAlloweds)
    return intersect.length > 0;
}
function getUserRoles() {
    let userRoles = [];
    _.forEach(getUserObject().Roles, function (i) {
        userRoles.push(i.RoleName)
    });
    return userRoles;
}
function getUserAlloweds() {
    return getLogedInUserContext()["AllowedActions"];
}
function hasPublicKeyRole() {
    return getLogedInUserContext()["HasPublicKeyRole"];
}
function isPublicKey() {
    return getLogedInUserContext()["IsPublicKey"];
}
function getLogedInUserContext() {
    if (getUserObject() === "nobody") {
        return { "AllowedActions": [], "DeniedActions": [], "HasPublicKeyRole": false, "IsPublicKey": false, "Settings": {} };
    } else {
        if (isNaNOrEmpty(sessionStorage.getItem("userContext"))) {
            let res = rpcSync({ requests: [{ "Method": "Zzz.AppEndProxy.GetLogedInUserContext", "Inputs": {} }] });
            sessionStorage.setItem("userContext", JSON.stringify(R0R(res)));
        }
        return JSON.parse(sessionStorage.getItem("userContext"));
    }
}

function reGetLogedInUserContext() {
    let res = rpcSync({ requests: [{ "Method": "Zzz.AppEndProxy.GetLogedInUserContext", "Inputs": {} }] });
    sessionStorage.setItem("userContext", JSON.stringify(R0R(res)));
    return JSON.parse(sessionStorage.getItem("userContext"));
}


function isAdmin() {
    return (isPublicKey() === true) || (HasPublicKeyRole() === true);
}
function logout(after) {
    rpcAEP("Logout", {}, function (res) {
        setAsLogedOut();
        if (after) after();
    });
}
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
function refereshSession() {
    let cntx = reGetLogedInUserContext();
    setAsLogedOut();
    setAsLogedIn(cntx["NewToken"], false);
}

function rpc(optionsOrig) {
    optionsOrig = normalizeOptions(optionsOrig);
    let workingObject = showWorking(optionsOrig.loadingModel);
    let RRs = analyzeRequests(optionsOrig.requests);
    let options = _.cloneDeep(optionsOrig);
    options.requests = _.cloneDeep(RRs.todoRequests);

    if (options.requests.length > 0) {
        $.ajax(getRpcConf(options.requests, true)).done(function (res) {
            try {
                let resps = _.isObject(res) ? res : JSON.parse(res);
                cacheResponses(options.requests, resps);
                if (!options.onFail) showUnHandledErrors(resps);
                RRs.cachedResponses.push(...resps); 
                if (options.onDone) options.onDone(RRs.cachedResponses);
            } catch (ex) {
                if (options.onFail) options.onFail(ex);
                else showJson({ ex: ex });
            }
            hideWorking(workingObject);
        }).fail(function (jqXhr, textStatus) {
            if (options.onFail) options.onFail({ "jqXhr": jqXhr, "textStatus": textStatus });
            else showJson({ "jqXhr": jqXhr, "textStatus": textStatus });
            hideWorking(workingObject);
        });
    } else {
        if (options.onDone) options.onDone(RRs.cachedResponses);
        hideWorking(workingObject);
    }
}
function rpcSync(optionsOrig) {
    optionsOrig = normalizeOptions(optionsOrig);
    let workingObject = showWorking(optionsOrig.loadingModel);
    let RRs = analyzeRequests(optionsOrig.requests);
    let options = _.cloneDeep(optionsOrig);
    options.requests = _.cloneDeep(RRs.todoRequests);
    let res = $.ajax(getRpcConf(options.requests, false)).responseText;
    hideWorking(workingObject);
    try {
        let resps = JSON.parse(res);
        if (!_.isObject(resps)) resps = JSON.parse(resps);
        cacheResponses(options.requests, resps);
        showUnHandledErrors(resps);
        RRs.cachedResponses.push(...resps); 
        return RRs.cachedResponses;
    } catch (ex) {
        handleError(options.requests, res);
        return [];
    }
}
function analyzeRequests(requests) {
    let cachedResps = [];
    let cachedRqsts = [];
    let todoRqsts = [];
    _.forEach(requests, function (rqst) {
        let r = sessionGet(rqst.cacheKey);
        if (r === null) todoRqsts.push(_.cloneDeep(rqst));
        else {
            cachedResps.push(r);
            cachedRqsts.push(_.cloneDeep(rqst));
        }
    });
    return { cachedRequests: cachedRqsts, cachedResponses: cachedResps, todoRequests: todoRqsts };
}
function cacheResponses(requests, responses) {
    _.forEach(responses, function (resp) {
        if (resp["IsSucceeded"].toString().toLowerCase() === 'true') {
            let rqst = _.filter(requests, function (r) { return r.Id.toString().toLowerCase() === resp.Id.toString().toLowerCase(); })[0];
            if (rqst.cacheTime > 0) {
                sessionSet(rqst.cacheKey, resp, rqst.cacheTime);
            }
        }
    });
}
function showUnHandledErrors(responses) {
    _.forEach(responses, function (resp) {
        if (resp["IsSucceeded"].toString().toLowerCase() !== 'true') {
            showJson(resp);
        }
    });
}

function rpcAEP(method, inputs, onDone, onFail) {
    rpc({ requests: [{ "Method": "Zzz.AppEndProxy." + method, "Inputs": fixNull(inputs, {}) }], onDone: onDone, onFail: onFail });
}
function handleError(requests, responses) {
    try {
        log({ responses: responses, requests: requests });
    } catch (ex) {
        log(ex);
    }
}
function getRpcConf(request, async) {
    return {
        type: 'POST', async: async, Accept: "application/json", dataType: "json", url: shared.talkPoint,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        headers: { "token": getUserToken() },
        data: (_.isObject(request) ? JSON.stringify(request) : request).toString()
    };
}
function normalizeOptions(options) {
    if (!_.isArray(options.requests)) options.requests = [options.requests];
    options.loadingModel = options.loadingModel || shared.notHeavyWorkingCover;
    options.silent = fixNull(options.silent, false);
    _.forEach(options.requests, function (rqst) {
        rqst.Id = fixNull(rqst.Id, genUN("id_"));
        rqst.cacheTime = fixNull(rqst.cacheTime, 0).toString().toInt();
        rqst.cacheKey = rqst.cacheTime > 0 ? genCacheKey(rqst) : '';
    });
    return options;
}
function R0R(r) {
    return r[0]["Result"];
}
function genCacheKey(rqst) {
    let r = _.cloneDeep(rqst);
    if (fixNull(r["Id"], '') !== '') delete r["Id"];
    if (fixNull(r["cacheTime"], '') !== '') delete r["cacheTime"];
    return "ck_" + JSON.stringify(r).hashCode();
}


function getBiById(id) {
    let options = getBiReadByKeyOptions(id);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'][0];
    } else {
        return {};
    }
}
function getBiByParentId(parentId) {
    let options = getBiReadListOptions("ParentId", parentId, 500);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'];
    } else {
        return {};
    }
}
function getBiByName(shortName) {
    let options = getBiReadListOptions("ShortName", shortName, 1);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'][0];
    } else {
        return {};
    }
}
function getBiByParentName(parentName) {
    let parObj = getBiByName(parentName);
    if (fixNull(parent, '') !== '') {
        let parentId = parObj["Id"];
        return getBiByParentId(parentId);
    } else {
        return {};
    }
}
function getBiReadListOptions(fieldName, fieldValue, pageSize) {
    let mName = getBiReadListMethod();
    let options = {};
    options.requests = [{
        Method: mName,
        cacheTime: shared.biCacheTime,
        Inputs: {
            ClientQueryJE: {
                QueryFullName: mName,
                Where: {
                    ConjunctiveOperator: "AND",
                    CompareClauses: [{ Name: fieldName, Value: fieldValue, CompareOperator: "Equal" }]
                },
                OrderClauses: [{ Name: "ViewOrder", OrderDirection: "ASC" }],
                Pagination: { PageNumber: 1, PageSize: pageSize },
                ExceptAggregations: ["Count"],
                IncludeSubQueries: false
            }
        }
    }];
    return options;
}
function getBiReadByKeyOptions(id) {
    let mName = getBiReadByKeyMethod();
    return {
        requests: [{
            Method: mName,
            cacheTime: shared.biCacheTime,
            Inputs: {
                ClientQueryJE: {
                    QueryFullName: mName,
                    Params: [{ Name: "Id", Value: id }]
                }
            }
        }]
    };
}
function getBiReadListMethod() {
    let m = `${shared.defaultDb}.${shared.biClass}.ReadList`;
    return m;
}
function getBiReadByKeyMethod() {
    let m = `${shared.defaultDb}.${shared.biClass}.ReadByKey`;
    return m;
}


function loadVM(componentPath) {
    const { loadModule } = window["vue3-sfc-loader"];
    const options = {
        moduleCache: { vue: Vue },
        getFile(url) {


            return fetch(url + "?fake=" + shared.fake).then(resp => resp.ok ? resp.text().then(i => trimUi(i)) : Promise.reject(resp));
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
function loadJSON(filePath) {
    return JSON.parse(loadTextFileAjaxSync(filePath, "application/json"));
}
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
function getAppConfig() {
    let appName = getThemeName();
    let appConfPath = `/${appName}/app.json?fake=${shared.fake}`;
    return getSessionItemSync("appsettings_" + appName, true, function () { return loadJSON(appConfPath); });
}
function getAppNav() {
    let appName = getThemeName();
    return getSessionItemSync("appnav_" + appName, true, function () { return trimNav(_.cloneDeep(getAppConfig().navigation)); });
}
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

function getThemeName() {
    return new URL(window.location).pathname.replaceAll(`/`,'');
}
function getQueryString(queryStringName) {
    let urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(queryStringName);
}
function setQueryString(queryStringName, val) {
    if (history.pushState) {
        var params = new URLSearchParams(window.location.search);
        params.set(queryStringName, val);
        var newUrl = window.location.origin + window.location.pathname + '?' + params.toString();
        window.history.pushState({ path: newUrl }, '', newUrl);
    }
}
function removeQueryString(queryStringName) {
    if (history.pushState) {
        var params = new URLSearchParams(window.location.search);
        params.delete(queryStringName);
        var newUrl = window.location.origin + window.location.pathname + '?' + params.toString();
        window.history.pushState({ path: newUrl }, '', newUrl);
    }
}

function translate(k) {
    if (shared.getAppConfig()["translation"] !== undefined && fixNull(shared.getAppConfig()["translation"][k], '') !== '') {
        return shared.getAppConfig()["translation"][k];
    } else {
        return k;
    }
}

function getKey(e) {
    return $(e.target).parents("tr:first").find("[data-ae-key]").attr("data-ae-key");
}
function getRow(ds, keyName, keyValue) {
    let r = _.filter(ds, function (i) { return i[keyName] === keyValue; });
    if (r.length === 0) return null;
    return r[0];
}


function getSqlScriptName(scriptBody) {
    let arr = getMatches(scriptBody, /\.[\ [a-zA-Z_0-9]+]/gm);
    if (arr === null || arr.length === 0) return "";
    return arr[0].toString().replace(".", "").replace("[", "").replace("]", "").trim();
}

function getMatches(str, regexp) {
    var matches = [];
    str.replace(regexp, function () {
        var arr = ([]).slice.call(arguments, 0);
        var extras = arr.splice(-2);
        arr.index = extras[0];
        arr.input = extras[1];
        matches.push(arr);
    });
    return matches.length ? matches : null;
}

function applyDirBasedOnUserType(elm) {
    if (elm.val().length > 0) {
        elm.css("direction", getDirBasedOnContent(elm.val()));
    }
}
function getDirBasedOnContent(content) {
    var x = new RegExp("[\x00-\x80]+"); // is ascii
    var isAscii = x.test(content);
    if (isAscii) return "ltr";
    return "rtl";
}
function getEditorMode(str) {
    if (str.toLowerCase().endsWith(".json")) return "ace/mode/json";
    if (str.toLowerCase().endsWith(".js")) return "ace/mode/javascript";
    if (str.toLowerCase().endsWith(".cs")) return "ace/mode/csharp";
    if (str.toLowerCase().endsWith(".css")) return "ace/mode/css";
    if (str.toLowerCase().endsWith(".html")) return "ace/mode/html";
    if (str.toLowerCase().endsWith(".htm")) return "ace/mode/html";
    if (str.toLowerCase().endsWith(".cshtml")) return "ace/mode/csharp";
    if (str.toLowerCase().endsWith(".vue")) return "ace/mode/html";
    if (str.toLowerCase().endsWith(".sql")) return "ace/mode/sqlserver";
    return "ace/mode/text";
}
function getEditorName(options) {
    return JSON.parse(options)["mode"].split('/')[2];
}

function usableSubmits(submits, templateName) {
    if (templateName.toLowerCase().indexOf("create") > -1)
        return _.filter(submits, function (i) { return i.Type.toLowerCase().indexOf('create') > -1; });

    if (templateName.toLowerCase().indexOf("updatebykey") > -1)
        return _.filter(submits, function (i) { return i.Type.toLowerCase().indexOf('updatebykey') > -1; });

    return [];
}
function usableLoads(loads, templateName) {
    if (templateName.toLowerCase().indexOf("readbykey") > -1 || templateName.toLowerCase().indexOf("updatebykey") > -1)
        return _.filter(loads, function (i) { return i.Type.toLowerCase().indexOf('readbykey') > -1; });

    if (templateName.toLowerCase().indexOf("aggregatedreadlist") > -1)
        return _.filter(loads, function (i) { return i.Type.toLowerCase().indexOf('aggregatedreadlist') > -1; });

    if ((templateName.toLowerCase().indexOf("readlist") > -1 || templateName.toLowerCase().indexOf("readtreelist") > -1) && templateName.toLowerCase().indexOf("aggregatedreadlist") === -1)
        return _.filter(loads, function (i) { return i.Type.toLowerCase().indexOf('readlist') > -1 && i.Type.toLowerCase().indexOf('aggregatedreadlist') === -1; });

    return [];
}

function crudSelectFiles(_this, relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType) {
    let elm = $('#' + parentId);
    let btnInputFiles = elm.parent().find('input[type="file"]:first');
    btnInputFiles.click();
    btnInputFiles.on("change", function () {
        let options = { accept: '*', maxSize: (3 * 1024 * 1024), resizeMaxWidth: 1200, resizeMaxHeight: 1200 };
        let btnInputFilesDOM = this;
        if (btnInputFilesDOM.files.length === 0) return;
        let fileArray = [];
        $.each(btnInputFilesDOM.files, function (index, value) {
            var fileReader = new FileReader();
            fileReader.onload = function () {
                if (!isImageFromName(value.name) && value.size > options.maxSize) return;
                resizebase64(value.name, getB64Str(fileReader.result), options.resizeMaxWidth, options.resizeMaxHeight, function (resized) {
                    let newItem = {};
                    newItem[fieldName_FileContent] = resized;
                    newItem[fieldName_FileName] = value.name;
                    newItem[fieldName_FileSize] = resized.length;
                    newItem[fieldName_FileType] = value.type;
                    fileArray.push(newItem);
                    if (fileArray.length === btnInputFilesDOM.files.length) {
                        $(btnInputFilesDOM).val("");
                        crudAddRelation(_this, relName, fileArray);
                    }
                });
            }
            fileReader.readAsArrayBuffer(value);
        });
    });
}
function crudLoadMasterRecord(_this) {
    _this.c.masterRequest["Inputs"]["ClientQueryJE"]["Params"][0]["Value"] = _this.c.inputs["key"];
    rpc({
        requests: [_this.c.masterRequest],
        onDone: function (res) {
            _this.c.row = res[0]['Result']['Master'][0];
            _this.c.Relations = crudExtracRelations(_this);
        }
    });
}
function crudLoadBaseInfo(_this) {
    if (_this.c.initialRequests.length > 0) {
        rpc({
            requests: _this.c.initialRequests,
            onDone: function (res) {
                _this.c.initialResponses = res;
            }
        });
    }
}
function crudOpenPicker(_this, ds, colName) {
    let rqst = getObjectById(_this.c.pickerRequests, colName + '_Lookup');
    let targetHumanIds = getObjectById(_this.c.pickerHumanIds, colName + '_HumanIds')["Items"];
    openComponent('/.publiccomponents/dbObjectPicker.vue', {
        placement: 'modal-dialog-centered',
        title: 'ObjectPicker',
        params: {
            api: rqst,
            humanIds: targetHumanIds,
            callback: function (ret) {
                ds[colName] = ret["Id"];
                _.forEach(targetHumanIds, function (i) {
                    ds[colName + "_" + i] = ret[i];
                });
            }
        }
    });
}
function crudAddRelation(_this, relName, filesArray) {
    let mData = findMetadataByRelationTableName(_this.RelationsMetaData, relName);
    if (mData.RelationType === 'OneToMany' && mData.IsFileCentric === true) {
        if (fixNull(filesArray, '') !== '') {
            $.each(filesArray, function (index, f) {
                _this.c.Relations[relName].push(f);
            });
        } else {
            _this.c.Relations[relName].push({});
        }
        initVueComponent(_this);
    } else {
        openComponent(mData.createComponent, {
            params: {
                okAction: "return",
                fkColumn: mData.RelationFkColumn,
                callback: function (ret) {
                    _this.c.Relations[relName].push(ret);
                }
            }
        });
    }
}
function crudUpdateRelation(_this, compPath, modalSize, recordKey,rowIndex, fkColumn, relName) {
    openComponent(compPath, {
        title: compPath.split(_this.dbConfName + '_')[1].replace('_', ', '),
        modalSize: modalSize,
        params: {
            row: _this.c.Relations[relName][rowIndex],
            fkColumn: fkColumn,
            okAction: "return",
            callback: function (row) {
                _this.c.Relations[relName][rowIndex]=row;
            }
        }
    });
}
function crudRemoveRelation(_this, relName, ind) {
    _this.c.Relations[relName].splice(ind, 1);
    let arr = _.cloneDeep(_this.c.Relations[relName]);
    _this.c.Relations[relName] = [];    
    setTimeout(function () {
        _this.c.Relations[relName] = arr;
        initVueComponent(_this);
    }, 0);
}
function crudExtracRelations(_this) {
    let res = {};
    for (let p in _this.c.RelationsMetaData) {
        let md = _this.c.RelationsMetaData[p];
        let arr = [];
        if (fixNull(_this.c.row[p], '') !== '') {
            let vItems = JSON.parse(_this.c.row[p]);
            if (md.RelationType === 'ManyToMany') {
                _.each(vItems, function (i) {
                    arr.push(i[md.LinkingColumnInManyToMany]);
                });
            } else {
                _.each(vItems, function (i) {
                    arr.push(i);
                });
            }
        }
        res[md.RelationTable] = arr;
    }
    return res;
}
function crudExportExcel(_this) {
    let _exceptColumns = [];
    let _columns = _this.c.clientQueryMetadata["ParentObjectColumns"];
    let _where = compileWhere(_this.c.searchOptions, _this.c.clientQueryMetadata);
    let _master = _this.c.initialRequests[0];
    _master['Inputs']['ClientQueryJE']['Where'] = _where;
    _master['Inputs']['ClientQueryJE']['Pagination'] = { PageNumber: 1, PageSize: 100000 };
    
    _.each(_columns, function (col) {
        if (col.DbType.toLowerCase() === "image") _exceptColumns.push(col.Name);
    });

    if (_exceptColumns.length > 0) {
        _master['Inputs']['ClientQueryJE']['ColumnsContainment'] = "ExcludeIndicatedItems";
        _master['Inputs']['ClientQueryJE']['ClientIndicatedColumns'] = _exceptColumns;
    }

    rpc({
        requests: [_master],
        onDone: function (res) {
            if (res[0].IsSucceeded.toString().toLowerCase() === 'true') {
                let records = res[0]['Result']['Master'];
                let csv = exportCSV(records, function (t) { return translate(t); });
                downloadCSV(csv, 'export.xls');
            }
        }
    });
}
function crudLoadRecords(_this) {
    let _where = compileWhere(_this.c.searchOptions, _this.c.clientQueryMetadata);
    _this.c.initialRequests[0]['Inputs']['ClientQueryJE']['Where'] = _where;
    rpc({
        requests: _this.c.initialRequests,
        onDone: function (res) {
            if (res[0].IsSucceeded.toString().toLowerCase() === 'true') {
                setupList(_this, res);
            }
        }
    });
}
function crudOpenById(_this, compPath, modalSize, recordKey, refereshOnCallback, actionsAllowed) {
    if (actionsAllowed.trim() !== '' && !isPublicKey() && !hasPublicKeyRole()) {
        let tagAllowed = actionsAllowed.split(',');
        let userAllowed = getUserAlloweds();
        let intersect = _.intersection(tagAllowed, userAllowed);
        if (intersect.length === 0) {
            showError(translate("AccessDenied"));
            return;
        }
    }
    openComponent(compPath, {
        title: compPath.split(_this.dbConfName + '_')[1].replace('_', ', '),
        modalSize: modalSize,
        params: {
            key: recordKey,
            callback: function () {
                if (refereshOnCallback === true) _this.c.localCrudLoadRecords();
            }
        }
    });
}
function crudDeleteRecord(_this, pkName, pkValue) {
    showConfirm({
        title: shared.translate("DeleteRecord"), message1: shared.translate("AreYouSureYouWantToDeleteThisRecord"), message2: shared.translate("RecordId") + " : " + pkValue,
        callback: function () {
            let r = genDeleteRequest(_this.deleteMethod, pkName, pkValue);
            rpc({
                requests: [r],
                onDone: function (res) {
                    _this.c.localCrudLoadRecords();
                }
            });
        }
    });
}
function crudSaveRecord(_this, after) {
    let request = genCreateUpdateRequest(_this, `${_this.dbConfName}.${_this.objectName}.${_this.submitMethod}`, turnKeyValuesToParams(_this.c.row), _this.c.Relations, _this.c.RelationsMetaData);
    rpc({
        requests: [request],
        onDone: function (res) {
            if (res[0].IsSucceeded === true) {
                showSuccess(translate("RecordSaved"));
                if (after) after();
            } else {
                showJson(res);
            }
        }
    });
}
function crudOpenCreate(_this, creaeControl, modalSize) {
    openComponent(creaeControl, {
        title: `Create`,
        modalSize: modalSize,
        params: {
            callback: function () {
                _this.c.localCrudLoadRecords();
            }
        }
    });
}
function setupList(_this, res) {
    _this.c.initialResponses = res;
    $(".pagination").bsPagination({
        pages: Math.ceil(_this.c.initialResponses[0]['Result']['Aggregations'][0]['Count'] / _this.c.initialRequests[0].Inputs.ClientQueryJE.Pagination.PageSize),
        page: _this.c.initialRequests[0].Inputs.ClientQueryJE.Pagination.PageNumber,
        "next-text": shared.translate("Next"),
        "previous-text": shared.translate("Previous"),
        afterPageChanged: function (p) {
            _this.c.initialRequests[0].Inputs.ClientQueryJE.Pagination.PageNumber = p;
            _this.c.localCrudLoadRecords();
        }
    });
}
function genListRequest(queryFullName, where, orderClauses, pagination) {
    return {
        "Id": queryFullName,
        "Method": queryFullName,
        "Inputs": {
            "ClientQueryJE": {
                "QueryFullName": queryFullName,
                "Where": where,
                "OrderClauses": orderClauses,
                "Pagination": pagination
            }
        }
    };
}
function genDeleteRequest(queryFullName,pkName,pkValue) {
    return {
        Id: queryFullName,
        Method: queryFullName,
        Inputs: {
            ClientQueryJE: {
                QueryFullName: queryFullName,
                Params: [{ Name: pkName, Value: pkValue }]
            }
        }
    };
}
function genCreateUpdateRequest(_this, apiName, params, relations, relationsMetaData) {
    let r = {
        "Method": apiName,
        "Inputs": {
            "ClientQueryJE": {
                "QueryFullName": apiName,
                "Params": params
            }
        }
    };

    r["Inputs"]["ClientQueryJE"]["Relations"] = {};
    for (let relName in relations) {
        let mData = findMetadataByRelationTableName(relationsMetaData, relName);
        r["Inputs"]["ClientQueryJE"]["Relations"][mData.RelationTable] = [];
        let finalItems = [];
        let existingNs = JSON.parse(fixNull(_this.c.row[mData.RelationName], '[]'));
        if (mData.RelationType === 'ManyToMany') {
            // flag for insert only
            _.each(relations[relName], function (i) {
                let row = {};
                row[mData.LinkingColumnInManyToMany] = i;
                let _item = _.findIndex(existingNs, function (j) { return j[mData.LinkingColumnInManyToMany] === i; });
                if (_item !== -1) row["_flag_"] = "i";
                finalItems.push(row);
            });

            // flag for delete
            _.each(existingNs, function (i) {
                let _itemIndex = _.findIndex(finalItems, function (j) { return j[mData.LinkingColumnInManyToMany] === i[mData.LinkingColumnInManyToMany]; });
                if (_itemIndex === -1) {
                    let row = {};
                    row["_flag_"] = "d";
                    row[mData.RelationPkColumn] = i[mData.RelationPkColumn];
                    row[mData.LinkingColumnInManyToMany] = i[mData.LinkingColumnInManyToMany];
                    finalItems.push(row);
                }
            });
        } else {
            // flag for insert or update
            _.each(relations[relName], function (i) {
                let row = i;
                if (fixNull(i[mData.RelationPkColumn], '') === '') { // insert
                    row["_flag_"] = "c";
                } else { // update
                    row["_flag_"] = "u"; 
                }
                finalItems.push(row);
            });

            _.each(existingNs, function (i) {
                let _itemIndex = _.findIndex(finalItems, function (j) { return fixNull(j[mData.RelationPkColumn], '').toString() === fixNull(i[mData.RelationPkColumn], '').toString(); });
                if (_itemIndex === -1) {
                    let row = {};
                    row["_flag_"] = "d";
                    row[mData.RelationPkColumn] = i[mData.RelationPkColumn];
                    finalItems.push(row);
                }
            });
        }
        _.each(finalItems, function (i) { r["Inputs"]["ClientQueryJE"]["Relations"][mData.RelationTable].push(turnKeyValuesToParams(i)); });
    }
    return r;
}
function turnKeyValuesToParams(keyVals) {
    let res = [];
    for (var key in keyVals) {
        if (keyVals.hasOwnProperty(key)) {
            res.push({ "Name": key, "Value": keyVals[key] });
        }
    }
    return res;
}
function compileWhere(searchInputs, queryMetadata) {
    let where = null;
    let clauses = [];

    for (var key in searchInputs) {
        if (searchInputs.hasOwnProperty(key)) {
            if (fixNull(searchInputs[key], '') !== '') {
                let co = getCompareObject(searchInputs, queryMetadata, key, key);
                if (fixNull(co, '') !== '') clauses.push(co);
            }
        }
    }

    if (clauses.length > 0) where = { "ConjunctiveOperator": "AND", "CompareClauses": clauses };
    return where;
}
function getCompareObject(searchInputs, queryMetadata, key, compareName) {
    let compareObject;
    if (fixNull(searchInputs[key], '') === '') return compareObject;
    let colName = compareName.replace('__startof', '').replace('__endof', '');
    let col = _.filter(queryMetadata["ParentObjectColumns"], function (c) { return c.Name === colName; });
    if (fixNull(col, '') === '' || col.length === 0 || fixNull(col[0].DbType, '') === '') return compareObject;
    let colDbType = col[0].DbType.toLowerCase();
    if (colDbType === 'bit') {
        compareObject = { "Name": colName, "Value": searchInputs[key], "CompareOperator": "Equal" };
    } else if (colDbType === 'date' || colDbType === 'datetime') {
        let _format = colDbType === 'datetime' ? 'YYYY-MM-DD HH:mm:ss.SSS' : 'YYYY-MM-DD';
        let vv = moment(searchInputs[key], _format);
        if (vv.toString().toLowerCase().startsWith('invalid')) {
            searchInputs[key] = "";
        } else {
            if (key.endsWith('__startof')) {
                searchInputs[key] = vv.startOf('day').format(_format);
                compareObject = { "Name": colName, "Value": searchInputs[key], "CompareOperator": "MoreThanOrEqual" };
            } else if (key.endsWith('__endof')) {
                searchInputs[key] = vv.endOf('day').format(_format);
                compareObject = { "Name": colName, "Value": searchInputs[key], "CompareOperator": "LessThanOrEqual" };
            } else {
                searchInputs[key] = vv.format(_format);
                compareObject = { "Name": colName, "Value": searchInputs[key], "CompareOperator": "Equal" };
            }
        }
    } else if (dbTypeIsNumerical(colDbType) === true) {
        try {
            compareObject = { "Name": colName, "Value": searchInputs[key], "CompareOperator": "Equal" };
        } catch (ex) { }
    } else {
        compareObject = { "Name": colName, "Value": searchInputs[key], "CompareOperator": "Contains" };
    }
    return compareObject;
}
function dbTypeIsNumerical(dbType) {
    let dbT = dbType.toLowerCase();
    if (dbT.indexOf("int") > -1) return true;
    if (dbT.indexOf("numeric") > -1) return true;
    if (dbT.indexOf("real") > -1) return true;
    if (dbT.indexOf("money") > -1) return true;
    if (dbT.indexOf("float") > -1) return true;

    return false;
}
function makeDotsToTree(items) {
    let tree = [];
    _.each(items, function (i) {
        let o = i.split('.');
        let itemCat = o[0] + ", " + o[1];
        let ind = _.findIndex(tree, function (tI) { return tI.cat === itemCat; });
        if (ind === -1) {
            let newCat = { cat: itemCat, ns: o[0], cs: o[1], methods: [] };
            newCat.methods.push(o[2]);
            tree.push(newCat);
        } else {
            tree[ind].methods.push(o[2]);
        }
    });
    return tree;
}
function getImageURI(imageBytes) {
    if (imageBytes === null || imageBytes === undefined) return "/.lib/images/avatar.png";
    return 'data:image/png;base64, ' + imageBytes;
}
function findMetadataByRelationTableName(relationsMetaData, tableName) {
    let res = null;
    for (let p in relationsMetaData) { if (relationsMetaData[p]["RelationTable"] === tableName) res = relationsMetaData[p]; };
    return res;
}


function getPath() {
    return document.location.pathname.replaceAll('/', '');
}
