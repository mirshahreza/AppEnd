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
    biCacheTime: 30,
    editors:[],
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
    fixEndBy(str, endFix) { return fixEndBy(str, endFix); },
    fixStartBy(str, preFix) { return fixStartBy(str, preFix); },

    getResponseObjectById(initialRequests, initialResponses, row, colName) { return getResponseObjectById(initialRequests, initialResponses, row, colName); },
    getObjectById(o, id) { return getObjectById(o, id); },

    formatDate(d) { return formatDate(d); },
    formatDateTime(d) { return formatDateTime(d); },
    formatDateL(d) { return formatDateL(d, getAppConfig()["calendar"]); },
    formatDateTimeL(d) { return formatDateTimeL(d, getAppConfig()["calendar"]); },
    formatNumber(n) { return formatNumber(n) },

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

    removeProp(obj, propName) { return removeProp(obj, propName); },

    enum(parentId) { return getBiItemsByParentId(parentId); },
    getBiItemsByParentId(parentId) { return getBiItemsByParentId(parentId); },
    getBiItemsByParentShortName(parentShortName) { return getBiItemsByParentShortName(parentShortName); },

    truncateString(str, maxLength) { return truncateString(str, maxLength); }

};

function getResponseObjectById(initialRequests, initialResponses, row, colName) {
    let finalResult = [];
    let theKey = colName;
    let rqst = _.filter(initialRequests, function (i) { return i.Id === colName; })[0];
    let rqstStr = JSON.stringify(rqst);
    let params = fixNull(rqstStr,'')==='' ? [] : rqstStr.getParameters();
    let paramsArePerfect = true;
    if (params.length > 0) {
        _.forEach(params, function (p) {
            if (fixNull(row[p], "") !== "") theKey = theKey + "_" + p + "_" + row[p];
            else paramsArePerfect = false;
        });
    }

    let r = _.filter(initialResponses, function (i) { return i.Id === theKey; });
    if (r.length === 0 && params.length > 0) {
        let flagReturn = true;
        _.forEach(params, function (p) {
            if (fixNull(row[p], "") !== "") {
                rqstStr = rqstStr.replaceAll("&[" + p + "]", row[p]);
                flagReturn = false;
            }
        });

        if (flagReturn === false) {
            let rqstById = JSON.parse(rqstStr);
            rqstById.Id = theKey;
            let res = rpcSync({ requests: [rqstById] });
            initialResponses.push(res[0]);
            finalResult = _.filter(initialResponses, function (i) { return i.Id === theKey; })[0].Result.Master;
        }
    } else {
        r = r[0];
        if (fixNull(r, '') !== '' && fixNull(r.Result, '') !== '' && fixNull(r.Result.Master, '') !== '') finalResult = r.Result.Master;
    }

    let testToSetEmpty = _.filter(finalResult, function (i) { return i.Id === row[colName.replace("_Lookup", "")]; }).length === 0;
    if (testToSetEmpty === true && paramsArePerfect === false) row[colName.replace("_Lookup", "")] = "";

    return finalResult;
}
function getObjectById(arr, id) {
    let r = _.filter(arr, function (i) { return i.Id === id; })[0];
    if (fixNull(r,'') === '') return [];
    return r;
}
function setAppTitle(title) {
    let tHtml = "";
    let tText = "";
    if (fixNullOrEmpty(title, '$auto$') === "$auto$") {
        let ci = getCurrentAppNavItem();
        tText = translate(ci.itemTitle);
        tHtml = `<span class="text-secondary title-first-part"><i class="${ci.parentIcon} me-1 d-none d-md-inline-block d-lg-inline-block"></i><span class="d-none d-md-inline-block d-lg-inline-block">${ci.parentTitle}</span></span> <span class="d-none d-md-inline-block d-lg-inline-block">&nbsp;&nbsp;/&nbsp;&nbsp;</span> <span class="text-success title-second-part"><i class="${ci.itemIcon} me-1"></i><span>${ci.itemTitle}</span></span>`;
    } else {
        tText = translate($(`<div>${title}</div>`).text());
        tHtml = title;
    }
    $(".app-title").html(tHtml);
    document.title = getAppConfig().title + " :: " + tText;
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
function getCurrentAppNavItem() {
    let urlC = getQueryString("c");
    let res = {};
    _.forEach(getAppNav(), function (navG) {
        _.forEach(navG.items, function (navItem) {
            if (navItem.component === urlC) {
                res["parentTitle"] = navG.title;
                res["parentIcon"] = navG.icon;
                res["itemTitle"] = navItem.title;
                res["itemIcon"] = navItem.icon;
            }
        });
    });
    return res;
}


function initVueComponent(_this) {
    $(document).ready(function () {
        setTimeout(function () {
            $(`#${_this.cid} .ae-focus`).focus();
            $(`.scrollable`).overlayScrollbars({});
            setTimeout(function () {
                _this.regulator = $(`#${_this.cid}`).inputsRegulator();
            }, 300);
            runWidgets();
        }, 200);
    });
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

    openComponent("/a.SharedComponents/BaseConfirm", { title: options.title, resizable: false, draggable: false, windowSizeSwitchable: false, params: options });
}
function showPrompt(options) {
    options = _.defaults(options, {
        title: "",
        message1: "",
        message1Class: "text-secondary fw-bold fs-d9",
        message2: "",
        message2Class: "text-primary fw-bold fs-1d2",
        okText: "Ok",
        okClass: "btn btn-sm btn-primary w-100 py-2",
        validation: {
            "required": true,
            "rule": "^[^a-zA-Z0-9]?.{1,128}$"
        },
        callback: null
    });

    openComponent("/a.SharedComponents/BasePrompt", { title: options.title, windowSizeSwitchable: false, params: options });
}
function showPromptEx(options) {
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
        reasonTitle: "", reasonRequired: true,
        noteTitle: "", noteRequired: true, noteRule: ":=s(8,4000)",
        callback: null
    });
    openComponent("/a.SharedComponents/BasePromptEx", { title: options.title, windowSizeSwitchable: false, params: options });
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
        animation: 'fade',
        modalSize: '',
        modalBodyCSS: 'bg-light bg-gradient',
        closeByOverlay: false,
        headerCSS: 'bg-light bg-gradient',
        backdrop: true,
        border: 'border-4 border-secondary',
        windowSizeSwitchable: true,
        resizable: true,
        draggable: true,
        modalMargin: "p-lg-5 p-md-3 p-sm-1",
        params: {}
    });

    if (options.modalSize === 'modal-fullscreen') options.windowSizeSwitchable = false;
    if (fixNull(options.title, '') === '') options.title = src;

    options.animation = options.animation.replaceAll("$dir$", getLayoutDir()).replaceAll("$DirHand$", getLayoutDir() === 'rtl' ? "Right" : "Left");

    createWindow();
    shared["params_" + id] = options.params;
    function createWindow() {
        let dBody = $(getDialogHtml());
        $("body").append(dBody);
        $(document).ready(function () {
            const mdl = new bootstrap.Modal(options.sharpId, {});
            let app = Vue.createApp();
            app.config.globalProperties.shared = shared;
            app.config.warnHandler = () => null;
            app.component('comp-loader', loadVM("/a.SharedComponents/BaseComponentLoader.vue"));
            app.mount(options.sharpId);
            let m = document.getElementById(options.id);
            m.addEventListener('shown.bs.modal', () => {
                $("#c_" + options.id).attr("data-ae-ready", "true");
                $(`.scrollable`).overlayScrollbars({});
                m.focus();
            });
            m.addEventListener('hidden.bs.modal', event => {
                setTimeout(function () {
                    m.remove();
                    setTimeout(function () {
                        $(".modal:last").focus();
                    }, 50);
                }, 200);
            });
            mdl.show();
        });
    }
    function getDialogHtml() {
        let comp = `<comp-loader src="` + src + `" uid="c_` + options.id + `" cid="` + options.id + `" ismodal="true" />`;
        let modalClose = options.showCloseButton !== true ? "" : `<button type="button" class="btn btn-sm p-0" data-bs-dismiss="modal" aria-label="Close"><i class="fa-solid fa-times fa-fw text-secondary text-hover-dark fs-1d2"></i></button>`;
        let modalMaxiBtn = options.windowSizeSwitchable !== true ? "" : `<button type="button" class="btn btn-sm p-0 me-2" onclick="switchWindowSize(this);"><i class="fa-solid fa-expand fa-fw text-secondary text-hover-dark fs-1d2"></i></button>`;
        let modalHeader = options.showHeader ? `<div ondblclick="alert('${src}');" class="modal-header input-group input-group-sm p-2 pb-1 ${options.headerCSS}"><div class="modal-title fb fs-d8">${shared.translate(options.title)}</div><input class="form-control bg-transparent border-0" disabled />${modalMaxiBtn}${modalClose}<div>&nbsp;</div></div>` : "";
        let modalBody = `<div class="modal-body p-0"><div class="h-100 ${options.modalBodyCSS}" data-ae-overlaycontainer="${id}">${comp}</div></div>`;
        let modalContent = `<div class="modal-content rounded-3 ${options.border} shadow-lg">${modalHeader}${modalBody}</div>`;
        let backdrop = options.backdrop === false ? 'data-bs-backdrop="false"' : (options.closeByOverlay === false ? 'data-bs-backdrop="static"' : '');
        let modalCss = `modal-dialog rounded-3 border-0 ${options.modalSize} ${options.placement} modal-fullscreen-lg-down ${(options.modalSize === 'modal-fullscreen' ? options.modalMargin : '')}`; // modal-dialog-scrollable
        return `<div class="modal ${options.animation}" id="${id}" tabindex="-1" aria-hidden="true" ${backdrop}><div class="${modalCss}">${modalContent}</div></div>`;
    }
}
function switchWindowSize(elm) {
    if($(elm).find(".fa-solid").attr("class").indexOf("fa-expand")>-1){
        $(elm).parents(".modal:first").find(".modal-dialog").addClass("modal-fullscreen p-lg-5 p-md-3 p-sm-1");
        $(elm).find(".fa-solid").removeClass("fa-expand").addClass("fa-compress");
    }else{
        $(elm).parents(".modal:first").find(".modal-dialog").removeClass("modal-fullscreen p-lg-5 p-md-3 p-sm-1");
        $(elm).find(".fa-solid").removeClass("fa-compress").addClass("fa-expand");
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
        openComponent("/a.SharedComponents/BaseJsonView.vue", { title: "JsonView", modalSize: "modal-fullscreen", params: { jsonToView: jsn } });
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
    shared.fake = null;
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
        userRoles.push(i)
    });
    return userRoles;
}
function getUserAlloweds() {
    return getLogedInUserContext()["AllowedActions"];
}
function hasPublicKeyRole() {
    return fixNull(getLogedInUserContext()["HasPublicKeyRole"], false);
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
    return (isPublicKey() === true) || (hasPublicKeyRole() === true);
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

    options.requests = _.filter(options.requests, function (rq) {
        let tR = JSON.stringify(rq);
        return (tR.indexOf('"&[') === -1 || tR.indexOf("SaveDbObjectBody") > -1);
    });

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
    let RRs = analyzeRequests(optionsOrig.requests);
    let options = _.cloneDeep(optionsOrig);
    options.requests = _.cloneDeep(RRs.todoRequests);
    let res = [];
    if (options.requests.length > 0) {
        let workingObject = showWorking(optionsOrig.loadingModel);
        res = $.ajax(getRpcConf(options.requests, false)).responseText;
        hideWorking(workingObject);
    }
    try {
        let resps = !_.isObject(res) ? JSON.parse(res) : res;
        resps = !_.isObject(resps) ? JSON.parse(resps) : resps;
        cacheResponses(options.requests, resps);
        showUnHandledErrors(resps);
        RRs.cachedResponses.push(...resps); 
        return RRs.cachedResponses;
    } catch (ex) {
        handleError(options.requests, res);
        return [];
    }
}
function rpcAEP(method, inputs, onDone, onFail) {
    rpc({ requests: [{ "Method": "Zzz.AppEndProxy." + method, "Inputs": fixNull(inputs, {}) }], onDone: onDone, onFail: onFail });
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
    if (requests.length === 0 || responses.length === 0) return;
    _.forEach(responses, function (resp) {
        let rqst = _.filter(requests, function (r) { return r.Id.toString().toLowerCase() === resp.Id.toString().toLowerCase(); })[0];
        if (resp["IsSucceeded"].toString().toLowerCase() === 'true' && rqst.cacheTime > 0) {
            sessionSet(rqst.cacheKey, resp, rqst.cacheTime);
        }
    });
}
function showUnHandledErrors(responses) {
    let i = 0;
    _.forEach(responses, function (resp) {
        if (resp["IsSucceeded"].toString().toLowerCase() !== 'true') {
            resp["Index"] = i;
            if (resp["Result"]) {
                let r = resp["Result"];
                let title = r["Message"];
                let content = "";
                if (r["Data"]) {
                    _.forEach(r["Data"], function (v,p) {
                        content += '<span class="text-secondary fs-d9">' + p + '</span > : <span class="text-dark fw-bold fs-d9">' + v + "</span>" + "<br />";
                    });
                }
                if (r["StackTraceString"]) {
                    content += '<hr class="my-2 mb-0" />';
                    content += '<span class="text-dark fs-d7">' + r["StackTraceString"].replace('at ','').replaceAll('at ', '<br />') + '</span>';
                }
                openComponent("/a.SharedComponents/BaseContent", {
                    title: "Error", windowSizeSwitchable: false, modalSize: "modal-fullscreen",
                    params: {
                        content: {
                            Title: `<div style="direction:ltr">${title}</div>`,
                            ContentBody: `<div style="direction:ltr">${content.trim()}</div>`
                        }
                    }
                });
                //showJson(resp);

            } else {
                showJson(resp);
            }
        }
        i++;
    });
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
    if (fixNull(id,'')==='') return {};
    let options = getBiReadByKeyOptions(id);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'][0];
    } else {
        return {};
    }
}
function getBiItemsByParentId(parentId) {
    if (fixNull(parentId,'')==='') return {};
    let options = getBiReadListOptions("ParentId", parentId, 500);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'];
    } else {
        return {};
    }
}
function getBiByName(shortName) {
    if (fixNull(shortName,'')==='') return {};
    let options = getBiReadListOptions("ShortName", shortName, 1);
    let r = rpcSync(options)[0];
    if (r.IsSucceeded === true) {
        return r['Result']['Master'][0];
    } else {
        return {};
    }
}
function getBiItemsByParentShortName(parentShortName) {
    if (fixNull(parentShortName,'')==='') return {};
    let parObj = getBiByName(parentShortName);
    if (fixNull(parent, '') !== '') {
        let parentId = parObj["Id"];
        return getBiItemsByParentId(parentId);
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
function getLayoutDir() {
    return $(document).attr("dir");
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
        return _.filter(submits, function (i) { return i.Type.toLowerCase().indexOf('UpdateByKey') > -1; });

    return [];
}
function usableLoads(loads, templateName) {
    if (templateName.toLowerCase().indexOf("readbykey") > -1 || templateName.toLowerCase().indexOf("updatebykey") > -1)
        return _.filter(loads, function (i) { return i.Type.toLowerCase().indexOf('readbykey') > -1; });

    if (templateName.toLowerCase().indexOf("aggregatedreadlist") > -1)
        return _.filter(loads, function (i) { return i.Type.toLowerCase().indexOf('aggregatedreadlist') > -1; });

    if ((templateName.toLowerCase().indexOf("readlist") > -1 || templateName.toLowerCase().indexOf("readtreelist") > -1))
        return _.filter(loads, function (i) { return i.Type.toLowerCase().indexOf('readlist') > -1 && i.Type.toLowerCase().indexOf('aggregatedreadlist') === -1; });

    return [];
}

function assignDefaultMethods(_this) {
    if (!_this.c.resetSearchOptions) _this.c.resetSearchOptions = function () {
        _this.c.filter = _.cloneDeep(_this.initialSearchOptions);
    };

    if (!_this.c.openPicker) _this.c.openPicker = function (options) {
        options = fixNullOptions(options);
        options.row = (fixNull(_this.c.filter, '') !== '' ? _this.c.filter : _this.c.row);
        if (fixNull(options.dialog.title, '') === '') options.dialog.title = options.colName;
        let rqst = getObjectById(_this.c.pickerRequests, options.colName + '_Lookup');
        let targetHumanIds = getObjectById(_this.c.pickerHumanIds, options.colName + '_HumanIds')["Items"];
        openComponent('/a.SharedComponents/DbObjectPicker.vue', {
            placement: options.dialog.modalPlacement,
            title: options.dialog.title,
            modalSize: options.dialog.modalSize,
            params: {
                api: rqst,
                humanIds: targetHumanIds,
                callback: function (ret) {
                    options.row[options.colName] = ret["Id"];
                    _.forEach(targetHumanIds, function (i) {
                        options.row[options.colName + "_" + i] = ret[i];
                    });
                }
            }
        });
    };

    if (!_this.c.loadRecords) _this.c.loadRecords = function (after) {
        let compiled = compileWhere(_this.c.filter, _this.c.clientQueryMetadata);
        _this.c.initialRequests[0]['Inputs']['ClientQueryJE']['Where'] = compiled.where;
        _this.c.initialRequests[0]['Inputs']['ClientQueryJE']["Params"] = _.cloneDeep((fixNull(_this.c.params, '') !== '' ? _this.c.params : []));
        _this.c.initialRequests[0]['Inputs']['ClientQueryJE']["Params"].push(..._.cloneDeep(compiled.params));
        rpc({
            requests: _this.c.initialRequests,
            onDone: function (res) {
                setupList(_this, res);
                if (after) after();
            }
        });
    };

    if (!_this.c.exportExcel) _this.c.exportExcel = function () {
        let _exceptColumns = [];
        let _columns = _this.c.clientQueryMetadata["ParentObjectColumns"];

        let _master = _this.c.initialRequests[0];
        let compiled = compileWhere(_this.c.filter, _this.c.clientQueryMetadata);
        _master['Inputs']['ClientQueryJE']['Where'] = compiled.where;
        _master['Inputs']['ClientQueryJE']["Params"] = (fixNull(_this.c.params, '') !== '' ? _this.c.params : []);
        _master['Inputs']['ClientQueryJE']["Params"].push(...compiled.params);
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
    };

    if (!_this.c.openById) _this.c.openById = function (options) {
        options = fixNullOptions(options);
        if (fixNull(options.dialog.title, '') === '') options.dialog.title = "Update";
        if (options.actionsAllowed.trim() !== '' && !isPublicKey() && !hasPublicKeyRole()) {
            let tagAllowed = options.actionsAllowed.split(',');
            let userAllowed = getUserAlloweds();
            let intersect = _.intersection(tagAllowed, userAllowed);
            if (intersect.length === 0) {
                showError(translate("AccessDenied"));
                return;
            }
        }
        openComponent(options.compPath, {
            placement: options.dialog.modalPlacement,
            title: options.dialog.title,
            modalSize: options.dialog.modalSize,
            windowSizeSwitchable: options.dialog.windowSizeSwitchable,
            params: {
                key: options.recordKey,
                callback: function () {
                    if (options.refereshOnCallback === true) {
                        if (_this.c.templateType==="ReadList" || _this.c.templateType==="ReadTreeList") _this.c.loadRecords();
                        else _this.c.loadMasterRecord();
                    } 
                }
            }
        });
    };

    if (!_this.c.deleteById) _this.c.deleteById = function (options) {
        options.pkName = "Id";
        showConfirm({
            title: shared.translate("DeleteRecord"), message1: shared.translate("AreYouSureYouWantToDeleteThisRecord"), message2: shared.translate("RecordId") + " : " + options.pkValue,
            callback: function () {
                let r = genDeleteRequest(_this.deleteMethod, options.pkName, options.pkValue);
                rpc({
                    requests: [r],
                    onDone: function (res) {
                        _this.c.loadRecords();
                    }
                });
            }
        });
    };

    if (!_this.c.openCreate) _this.c.openCreate = function (options) {
        options = fixNullOptions(options);
        if (fixNull(options.compPath, '') === '') options.compPath = `/a.Components/${_this.filePrefix}${_this.objectName}_Create`;
        if (fixNull(options.dialog.title, '') === '') options.dialog.title = "Create";
        openComponent(options.compPath, {
            placement: options.dialog.modalPlacement,
            title: options.dialog.title,
            modalSize: options.dialog.modalSize,
            windowSizeSwitchable: options.dialog.windowSizeSwitchable,
            params: {
                callback: function () {
                    _this.c.loadRecords();
                }
            }
        });
    };


    if (!_this.c.addRelation) _this.c.addRelation = function (options) {
        options = fixNullOptions(options);
        if (!options.action) options.action = _this.c.templateType !== "Create" ? "SaveAndReturn" : "Return";
        let mData = getRelationMetadata(_this.RelationsMetaData, options.relName);
        if (mData.RelationType === 'OneToMany' && mData.IsFileCentric === true) {
            if (fixNull(options.filesArray, '') !== '') {
                $.each(options.filesArray, function (index, f) {
                    _this.c.Relations[options.relName].push(f);
                });
            } else {
                _this.c.Relations[options.relName].push({});
            }
            initVueComponent(_this);
        } else {
            openComponent(mData.createComponent, {
                title: (fixNull(options.title, '') === '' ? options.relName : options.title),
                params: {
                    okAction: options.action,
                    fkColumn: mData.RelationFkColumn,
                    fkValue: _this.c.row["Id"],
                    callback: function (ret) {
                        if (options.action === "SaveAndReturn") _this.c.loadMasterRecord();
                        else _this.c.Relations[options.relName].push(ret);
                    }
                }
            });
        }
    };
    if (!_this.c.deleteRelation) _this.c.deleteRelation = function (options) {
        if (!options.action) options.action = _this.c.templateType !== "Create" ? "SaveAndReturn" : "Return";
        _this.c.Relations[options.relationTable].splice(options.ind, 1);
        let arr = _.cloneDeep(_this.c.Relations[options.relationTable]);
        _this.c.Relations[options.relationTable] = [];
        setTimeout(function () {
            _this.c.Relations[options.relationTable] = arr;
            initVueComponent(_this);
        }, 0);
    };
    if (!_this.c.updateRelation) _this.c.updateRelation = function (options) {
        options = fixNullOptions(options);
        if (fixNull(options.dialog.title, '') === '') options.dialog.title = options.compPath.replace('_', ', ');
        if (!options.action) options.action = _this.c.templateType !== "Create" ? "SaveAndReturn" : "Return";
        openComponent(options.compPath, {
            title: options.compPath.replace('_', ', '),
            modalSize: options.modalSize,
            params: {
                row: _this.c.Relations[options.relName][options.ind],
                okAction: options.action,
                fkColumn: options.fkColumn,
                callback: function (row) {
                    _this.c.Relations[options.relName][options.ind] = row;
                }
            }
        });
    };

    if (!_this.c.loadMasterRecord) _this.c.loadMasterRecord = function (after) {
        if (_this.c.inputs.okAction !== 'Return') {
            _this.c.masterRequest["Inputs"]["ClientQueryJE"]["Params"][0]["Value"] = _this.c.inputs["key"];
            rpc({
                requests: [_this.c.masterRequest],
                onDone: function (res) {
                    _this.c.row = res[0]['Result']['Master'][0];
                    _this.c.Relations = extracRelations(_this);
                    if (after) after();
                    if (_this.c.afterLoadMasterRecord) _this.c.afterLoadMasterRecord();
                }
            });
        }
        else {
            _this.c.row = _this.c.inputs.row;
            if (_this.c.afterLoadMasterRecord) _this.c.afterLoadMasterRecord();
        }
    };
    if (!_this.c.componentFinalization) _this.c.componentFinalization = function () {
        if (_this.c.ismodal !== "true") {
            if (fixNull(_this.submitApi, '') !== '') setAppTitle(translate(_this.ObjectName + _this.submitApi.Replace(_this.dbConfName + ".", "").Replace(".", ", ")));
            else setAppTitle(translate(_this.ObjectName) + " :: " + _this.inputs["key"]);
        }
        if (_this.c.inputs.fkColumn) {
            _this.c.row[_this.c.inputs.fkColumn] = _this.c.inputs.fkValue;
        }
    };

    if (!_this.c.selectFiles) _this.c.selectFiles = function (relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType) {
        let elm = $('#' + parentId);
        let btnInputFiles = elm.parent().find('input[type="file"]:first');
        btnInputFiles.click();
        btnInputFiles.on("change", function () {
            let options = { accept: '*', maxSize: (3 * 1024 * 1024), resizeMaxWidth: 1200, resizeMaxHeight: 1200 };
            let btnInputFilesDOM = this;
            if (btnInputFilesDOM.files.length === 0) return;
            let filesArray = [];
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
                        filesArray.push(newItem);
                        if (filesArray.length === btnInputFilesDOM.files.length) {
                            $(btnInputFilesDOM).val("");
                            _this.c.addRelation({ relName: relName, filesArray: filesArray });
                        }
                    });
                }
                fileReader.readAsArrayBuffer(value);
            });
        });
    };

    if (!_this.c.loadBaseInfo) _this.c.loadBaseInfo = function () {
        if (_this.c.initialRequests.length > 0) {
            rpc({
                requests: _this.c.initialRequests,
                onDone: function (res) {
                    _this.c.initialResponses = res;
                }
            });
        }
    };

    if (!_this.c.ok) _this.c.ok = function (e, after) {
        if (!_this.regulator.isValid()) return;
        if (_this.c.inputs.okAction === "Return") {
            if (_this.inputs.callback) _this.inputs.callback(_this.row);
            _this.c.close();
        } else {

            let request = genCreateUpdateRequest(_this, `${_this.dbConfName}.${_this.objectName}.${_this.submitMethod}`, turnKeyValuesToParams(_this.c.row), _this.c.Relations, _this.c.RelationsMetaData);
            rpc({
                requests: [request],
                onDone: function (res) {
                    if (res[0].IsSucceeded === true) {
                        showSuccess(translate("RecordSaved"));
                        if (_this.inputs.callback) _this.c.inputs.callback(_this.c.row);
                        if (after) after(res);
                        _this.c.close();
                    }
                }
            });
        }
    };
    if (!_this.c.cancel) _this.c.cancel = function () {
        _this.c.close();
    };
    if (!_this.c.close) _this.c.close = function () {
        closeComponent(_this.cid);
    };
}


function setupList(_this, res) {
    _this.c.initialResponses = res;
    if (res[0].IsSucceeded.toString().toLowerCase() === 'true') {
        $(".pagination").bsPagination({
            pages: Math.ceil(_this.c.initialResponses[0]['Result']['Aggregations'][0]['Count'] / _this.c.initialRequests[0].Inputs.ClientQueryJE.Pagination.PageSize),
            page: _this.c.initialRequests[0].Inputs.ClientQueryJE.Pagination.PageNumber,
            "next-text": shared.translate("Next"),
            "previous-text": shared.translate("Previous"),
            afterPageChanged: function (p) {
                _this.c.initialRequests[0].Inputs.ClientQueryJE.Pagination.PageNumber = p;
                _this.c.loadRecords();
            }
        });
    }
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
        let mData = getRelationMetadata(relationsMetaData, relName);
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
function compileWhere(filter, queryMetadata) {
    let where = null;
    let clauses = [];
    let params = [];
    for (var key in filter) {
        if (filter.hasOwnProperty(key)) {
            if (fixNull(filter[key], '') !== '') {
                let co = getCompareObject(filter, queryMetadata, key, key);
                if (fixNull(co, '') !== '') clauses.push(co);
                else params.push({ Name: key, Value: filter[key] });
            }
        }
    }
    if (clauses.length > 0) where = { "ConjunctiveOperator": "AND", "CompareClauses": clauses };
    return { "where": where, "params": params };
}
function getCompareObject(filter, queryMetadata, key, compareName) {
    let compareObject;
    if (fixNull(filter[key], '') === '') return compareObject;
    let colName = compareName.replace('__startof', '').replace('__endof', '');
    let col = _.filter(queryMetadata["ParentObjectColumns"], function (c) { return c.Name === colName; });
    if (fixNull(col, '') === '' || col.length === 0 || fixNull(col[0].DbType, '') === '') return compareObject;
    let colDbType = col[0].DbType.toLowerCase();
    if (colDbType === 'bit') {
        compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "Equal" };
    } else if (colDbType === 'date' || colDbType === 'datetime') {
        let _format = colDbType === 'datetime' ? 'YYYY-MM-DD HH:mm:ss.SSS' : 'YYYY-MM-DD';
        let vv = moment(filter[key], _format);
        if (vv.toString().toLowerCase().startsWith('invalid')) {
            filter[key] = "";
        } else {
            if (key.endsWith('__startof')) {
                filter[key] = vv.startOf('day').format(_format);
                compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "MoreThanOrEqual" };
            } else if (key.endsWith('__endof')) {
                filter[key] = vv.endOf('day').format(_format);
                compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "LessThanOrEqual" };
            } else {
                filter[key] = vv.format(_format);
                compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "Equal" };
            }
        }
    } else if (dbTypeIsNumerical(colDbType) === true) {
        try {
            if (_.isArray(filter[key])) {
                compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "In" };
            } else {
                compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "Equal" };
            }
        } catch (ex) { alert(ex); }
    } else {
        compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "Contains" };
    }
    return compareObject;
}
function getRelationMetadata(relationsMetaData, tableName) {
    let res = null;
    for (let p in relationsMetaData) { if (relationsMetaData[p]["RelationTable"] === tableName) res = relationsMetaData[p]; };
    return res;
}
function extracRelations(_this) {
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
function turnKeyValuesToParams(keyVals) {
    let res = [];
    for (var key in keyVals) {
        if (keyVals.hasOwnProperty(key)) {
            res.push({ "Name": key, "Value": keyVals[key] });
        }
    }
    return res;
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
    if (fixNull(imageBytes, '') === '') return "/a..lib/images/avatar.png";
    return 'data:image/png;base64, ' + imageBytes;
}
function fixNullOptions(options) {
    if (fixNull(options, '') === '') options = {};
    if (fixNull(options.dialog, '') === '') options.dialog = {};
    return options;
}

String.prototype.getParameters = function () {
    if(this===null || this===undefined) return [];
    var re = /&\[(.*?)]/gm;
    var arr = this.matchAll(re);
    var inputs = [];
    $.each(arr, function (i, item) {
        inputs.push(item[1]);
    });
    return _.uniq(inputs);
};
function getPath() {
    return document.location.pathname.replaceAll('/', '');
}


function getFake(env) {
    if (env === 'dev') return "fake__" + (new Date()).getTime();
    if (fixNull(sessionStorage.getItem("fake"), '') === '') sessionStorage.setItem("fake", "fake__" + (new Date()).getTime());
    return sessionStorage.getItem("fake");
}