// append-core.js
// Core initialization and shared object

var vApp;
var vInstance;

var shared = {
    ld() { return _; },
    widgets: {},
    debug: true,
    talkPoint: "/talk-to-me/",
    heavyWorkingCover: `<span></span>`,
    notHeavyWorkingCover: `<span></span>`,
    miniHeavyWorkingCover: `<span></span>`,
    defaultDb: 'DefaultRepo',
    biClass: 'BaseInfo',
    biCacheTime: 30,
    editors: [],
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
    formatNumber(n) { return formatNumber(n); },
    format2Char(s) { return format2Char(s); },

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
    setUserSettings(settings) { return setUserSettings(settings); },

    removeProp(obj, propName) { return removeProp(obj, propName); },

    enum(parentId) { return getBiItemsByParentId(parentId); },
    getBiItemsByParentId(parentId) { return getBiItemsByParentId(parentId); },
    getBiItemsByParentShortName(parentShortName) { return getBiItemsByParentShortName(parentShortName); },

    truncateString(str, maxLength) { return truncateString(str, maxLength); },

    getIconFromName(fileName) { return getIconFromName(fileName); },
    
    getOperatorsForDbType(dbType) { return getOperatorsForDbType(dbType); }
};

/**
 * Initialize page - called on page load
 */
function initPage() {
    shared.heavyWorkingCover = $(".static-working-cover").get(0).outerHTML;
    shared.notHeavyWorkingCover = shared.heavyWorkingCover.replace("bg-elevated", "bg-transparent");
    shared.miniHeavyWorkingCover = shared.heavyWorkingCover
        .replace("static-working-cover", "static-working-cover-busy")
        .replace("\"position: fixed;", "\"position: fixed;opacity:.5;");
    $(".static-working-cover").get(0).remove();
    $("body").append(`<div id="topEndToastContainer" class="position-fixed top-0 end-0 p-3" style="z-index:99999;"></div><div id="mytemp"></div>`);
}

/**
 * Initialize Vue component
 */
function initVueComponent(_this) {
    $(`.scrollable`).overlayScrollbars({});
    $(document).ready(function () {
        setTimeout(function () {
            $(`#${_this.cid} .ae-focus`).focus();
            $(`.scrollable`).overlayScrollbars({});
            runWidgets();
        }, 100);
    });
}

/**
 * Run widgets on page
 */
function runWidgets() {
    $("[data-ae-widget]").each(function () {
        let elm = $(this);
        let isWidgetExecuted = elm.attr("data-ae-widget-executed");
        if (isWidgetExecuted !== '1') {
            let widgetFunc = elm.attr("data-ae-widget");
            let opts = fixNullOrEmpty(elm.attr("data-ae-widget-options"), '');
            try {
                let ev = `elm.${widgetFunc}(${opts})`;
                if (widgetFunc === "trumbowyg") {
                    ev = ev + `.on('tbwchange', function () { elm.get(0).dispatchEvent(new Event('input', { bubbles: true })); })`;
                }
                let w = eval(ev + ";");

                if (elm.attr("id")) shared.widgets[elm.attr("id")] = w;
                if (widgetFunc !== 'nullableCheckbox' && widgetFunc !== 'trumbowyg') elm.attr("data-ae-widget-executed", '1');
            } catch (ex) {
                elm.html(ex.message);
            }
        }
    });
}

/**
 * Get current app navigation item based on query string
 */
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

function setAppTitle(title) {
    let tHtml = "";
    let tText = "";
    if (fixNullOrEmpty(title, '$auto$') === "$auto$") {
        let ci = getCurrentAppNavItem();
        tText = translate($(`<div>${ci.itemTitle}</div>`).text());
        tParentTitle = translate($(`<div>${ci.parentTitle}</div>`).text());
        tHtml = `<span class="text-dark title-first-part"><i class="${ci.parentIcon} me-1 d-none d-md-inline-block d-lg-inline-block"></i><span class="d-none d-md-inline-block d-lg-inline-block">${tParentTitle}</span></span> <span class="d-none d-md-inline-block d-lg-inline-block">&nbsp;&nbsp;/&nbsp;&nbsp;</span> <span class="text-dark fw-bold title-second-part"><i class="${ci.itemIcon} me-1"></i><span>${tText}</span></span>`;
    } else {
        tText = translate($(`<div>${title}</div>`).text());
        tHtml = title;
    }
    $(".app-title").html(tHtml);
    document.title = getAppConfig().title + " :: " + tText;
    setAppMessage(translate("Ready"), 5000, 'text-dark');
}

function setAppSubTitle(title) {
    $(".app-subtitle").html(title);
    document.title = document.title + " :: " + title;
    setAppMessage("Ready", 5000, 'text-dark');
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

function showConfirm(options) {
    options = _.defaults(options, {
        title: "",
        message1: "",
        message1Class: "text-primary fw-bold fs-dd",
        message2: "",
        message2Class: "text-secondary fs-7",
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
        message1Class: "text-primary fw-bold fs-dd",
        message2: "",
        message2Class: "text-secondary fs-7",
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
        message1Class: "text-primary fw-bold fs-dd",
        message2: "",
        message2Class: "text-secondary fs-7",
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

function showInfo(message) {
    showMessage({ "title1": shared.translate("Info"), "message": message, "type": "Info" });
}

function showSuccess(message) {
    showMessage({ "title1": shared.translate("Success"), "message": message, "type": "Success" });
}

function showError(message) {
    showMessage({ "title1": shared.translate("Error"), "message": message, "type": "Error" });
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
<div id="${id}" class="toast" role="alert" aria-live="assertive" style="z-index:99999;border-radius:10px;box-shadow:0 8px 24px rgba(0,0,0,0.25),0 2px 8px rgba(0,0,0,0.15);">
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

function showMoreInfo(elm) {
    let popOver = new bootstrap.Popover(elm, { trigger: 'focus', html: true });
    popOver.setContent({ '.popover-header': 'Auditing info', '.popover-body': '...' });
    popOver.show();
    $(".popover-body").html($(elm).parent().find(".more-info").html())
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

function getResponseObjectById(initialRequests, initialResponses, row, colName) {
    let finalResult = [];
    let theKey = colName;
    let rqst = _.filter(initialRequests, function (i) { return i.Id === colName; })[0];
    let rqstStr = JSON.stringify(rqst);
    let params = fixNull(rqstStr, '') === '' ? [] : rqstStr.getParameters();
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
    if (fixNull(r, '') === '') return [];
    return r;
}




function getKey(e) {
    return $(e.target).parents("tr:first").find("[data-ae-key]").attr("data-ae-key");
}
function getRow(ds, keyName, keyValue) {
    let r = _.filter(ds, function (i) { return i[keyName] === keyValue; });
    if (r.length === 0) return null;
    return r[0];
}

function assignDefaultMethods(_this) {
    if (!_this.c.resetSearchOptions) _this.c.resetSearchOptions = function () {
        _this.c.filter = _.cloneDeep(_this.initialSearchOptions);
        // Reinitialize widgets (including tri-state checkboxes) after resetting filter values
        setTimeout(function () {
            runWidgets();
        }, 100);
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
        let compiled = compileWhere(_this.c.filter, _this.c.columns);
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
        let _columns = _this.c.columns;

        let _master = _this.c.initialRequests[0];
        let compiled = compileWhere(_this.c.filter, _this.c.columns);
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
                fkColumn: options.fkColumn,
                callback: function () {
                    if (options.refereshOnCallback === true) {
                        if (_this.c.templateType === "ReadList" || _this.c.templateType === "ReadTreeList") _this.c.loadRecords();
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
                fkColumn: options.fkColumn,
                fkValue: options.fkValue,
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
                    runWidgets();
                    $(`#container_${_this.c.inputs.fkColumn}`).hide();
                }
            });
        }
        else {
            _this.c.row = _this.c.inputs.row;
            if (_this.c.afterLoadMasterRecord) _this.c.afterLoadMasterRecord();
            runWidgets();
            $(`#container_${_this.c.inputs.fkColumn}`).hide();
        }
    };
    if (!_this.c.componentFinalization) _this.c.componentFinalization = function () {
        if (_this.c.ismodal !== "true") {
            if (fixNull(_this.submitApi, '') !== '') setAppTitle(translate(_this.ObjectName + _this.submitApi.Replace(_this.dbConfName + ".", "").Replace(".", ", ")));
            else setAppTitle(translate(_this.ObjectName) + " :: " + _this.inputs["key"]);
        }
        if (_this.c.inputs.fkColumn) {
            _this.c.row[_this.c.inputs.fkColumn] = _this.c.inputs.fkValue;
            $(`#container_${_this.c.inputs.fkColumn}`).hide();
        }
        runWidgets(); // to ensure checkboxes and radios are rendered properly
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
        let validationAreaId = $("#" + _this.c.cid).find("[data-ae-widget='inputsRegulator']").attr("id");
        if (isAreaValidById(validationAreaId) === false) return;

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

function isAreaValidById(areaId) {
    let validationArea = shared.widgets[areaId];
    if (fixNull(validationArea, '') !== '') return validationArea.isValid();
    return true;
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
function genDeleteRequest(queryFullName, pkName, pkValue) {
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
            // Skip operator keys
            if (key.endsWith('_Operator')) continue;
            
            if (fixNull(filter[key], '') !== '') {
                // Get custom operator (if exists)
                let operatorKey = key + '_Operator';
                let customOperator = filter[operatorKey];
                
                let co = getCompareObject(filter, queryMetadata, key, key, customOperator);
                if (fixNull(co, '') !== '') clauses.push(co);
                else params.push({ Name: key, Value: filter[key] });
            }
        }
    }
    if (clauses.length > 0) where = { "ConjunctiveOperator": "AND", "CompareClauses": clauses };
    return { "where": where, "params": params };
}
function getCompareObject(filter, queryMetadata, key, compareName, customOperator) {
    let compareObject;
    if (fixNull(filter[key], '') === '') return compareObject;
    let colName = compareName.replace('__startof', '').replace('__endof', '');
    let col = _.filter(queryMetadata, function (c) { return c.Name === colName; });
    if (fixNull(col, '') === '' || col.length === 0 || fixNull(col[0].DbType, '') === '') return compareObject;
    let colDbType = col[0].DbType.toLowerCase();
    
    // Check for special IsNull/IsNotNull operators
    if (customOperator === 'IsNull' || customOperator === 'IsNotNull') {
        compareObject = { "Name": colName, "Value": null, "CompareOperator": customOperator };
        return compareObject;
    }
    
    if (colDbType === 'bit') {
        compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "Equal" };
    } else if (colDbType === 'date' || colDbType === 'datetime' || colDbType === 'datetime2') {
        let _format = colDbType === 'datetime' || colDbType === 'datetime2' ? 'YYYY-MM-DD HH:mm:ss.SSS' : 'YYYY-MM-DD';
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
                let operator = fixNull(customOperator, 'Equal');
                compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": operator };
            }
        }
    } else if (dbTypeIsNumerical(colDbType) === true) {
        try {
            if (_.isArray(filter[key])) {
                compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "In" };
            } else {
                let operator = fixNull(customOperator, 'Equal');
                compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": operator };
            }
        } catch (ex) { alert(ex); }
    } else {
        // String types
        if (_.isArray(filter[key])) {
            compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": "In" };
        } else {
            let operator = fixNull(customOperator, 'Contains');
            compareObject = { "Name": colName, "Value": filter[key], "CompareOperator": operator };
        }
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

/**
 * Get list of allowed operators for a specific data type
 */
function getOperatorsForDbType(dbType) {
    dbType = dbType.toLowerCase();
    
    if (dbType === 'nvarchar' || dbType === 'varchar' || dbType === 'ntext' || dbType === 'text') {
        return [
            { operator: 'Contains', icon: 'fa-magnifying-glass', text: 'Contains' },
            { operator: 'Equal', icon: 'fa-equals', text: 'Equal' },
            { operator: 'NotEqual', icon: 'fa-not-equal', text: 'NotEqual' },
            { operator: 'StartsWith', icon: 'fa-right-from-bracket', text: 'StartsWith' },
            { operator: 'EndsWith', icon: 'fa-right-to-bracket', text: 'EndsWith' },
            { operator: 'IsNull', icon: 'fa-circle', text: 'IsNull' },
            { operator: 'IsNotNull', icon: 'fa-circle-check', text: 'IsNotNull' }
        ];
    }
    else if (dbType === 'int' || dbType === 'bigint' || dbType === 'decimal' || dbType === 'float' || dbType === 'money' || dbType === 'smallint' || dbType === 'tinyint') {
        return [
            { operator: 'Equal', icon: 'fa-equals', text: 'Equal' },
            { operator: 'NotEqual', icon: 'fa-not-equal', text: 'NotEqual' },
            { operator: 'MoreThan', icon: 'fa-greater-than', text: 'MoreThan' },
            { operator: 'MoreThanOrEqual', icon: 'fa-greater-than-equal', text: 'MoreThanOrEqual' },
            { operator: 'LessThan', icon: 'fa-less-than', text: 'LessThan' },
            { operator: 'LessThanOrEqual', icon: 'fa-less-than-equal', text: 'LessThanOrEqual' },
            { operator: 'IsNull', icon: 'fa-circle', text: 'IsNull' },
            { operator: 'IsNotNull', icon: 'fa-circle-check', text: 'IsNotNull' }
        ];
    }
    else if (dbType === 'date' || dbType === 'datetime' || dbType === 'datetime2') {
        return [
            { operator: 'Equal', icon: 'fa-equals', text: 'Equal' },
            { operator: 'NotEqual', icon: 'fa-not-equal', text: 'NotEqual' },
            { operator: 'MoreThan', icon: 'fa-greater-than', text: 'After' },
            { operator: 'MoreThanOrEqual', icon: 'fa-greater-than-equal', text: 'AfterOrEqual' },
            { operator: 'LessThan', icon: 'fa-less-than', text: 'Before' },
            { operator: 'LessThanOrEqual', icon: 'fa-less-than-equal', text: 'BeforeOrEqual' },
            { operator: 'IsNull', icon: 'fa-circle', text: 'IsNull' },
            { operator: 'IsNotNull', icon: 'fa-circle-check', text: 'IsNotNull' }
        ];
    }
    else if (dbType === 'bit') {
        // No operator - always Equal
        return [];
    }
    
    return [];
}









