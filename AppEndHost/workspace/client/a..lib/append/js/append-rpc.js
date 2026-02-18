// append-rpc.js
// RPC and Server Communication functions

var _isRefreshing = false;
var _pendingRequests = [];

/**
 * 401 interceptor: on Unauthorized, try RefreshToken then retry or redirect to login
 */
function handle401AndRetry(requests, options, RRs, workingObject) {
    if (_isRefreshing) {
        _pendingRequests.push({ requests: requests, options: options, RRs: RRs, workingObject: workingObject });
        return;
    }

    _isRefreshing = true;
    var refreshOpts = normalizeOptions({ requests: [{ Method: "Zzz.AppEndProxy.RefreshToken", Inputs: {} }] });
    $.ajax(getRpcConf(refreshOpts.requests, true)).done(function (refreshRes) {
        try {
            var resp = Array.isArray(refreshRes) ? refreshRes[0] : refreshRes;
            if (resp && resp.IsSucceeded === true && resp.Result && resp.Result.Result === true) {
                if (typeof setAsLogedIn === "function") setAsLogedIn();
                executeRpcAjax(requests, options, RRs, workingObject, true);

                while (_pendingRequests.length > 0) {
                    var pending = _pendingRequests.shift();
                    executeRpcAjax(pending.requests, pending.options, pending.RRs, pending.workingObject, true);
                }
            } else {
                redirectToLogin();
                _pendingRequests = [];
            }
        } catch (e) {
            redirectToLogin();
            _pendingRequests = [];
        }
        _isRefreshing = false;
    }).fail(function (jqXhr) {
        _isRefreshing = false;
        _pendingRequests = [];
        if (jqXhr.status === 401 || jqXhr.status === 403) {
            redirectToLogin();
        } else {
            redirectToLogin();
        }
    });
}

function redirectToLogin() {
    if (typeof setAsLogedOut === "function") setAsLogedOut();
}

function executeRpcAjax(requests, options, RRs, workingObject, isRetry) {
    $.ajax(getRpcConf(requests, true)).done(function (res) {
        try {
            var resps = _.isObject(res) ? res : JSON.parse(res);
            cacheResponses(options.requests, resps);
            if (!options.onFail) showUnHandledErrors(resps);
            RRs.cachedResponses.push.apply(RRs.cachedResponses, resps);
            if (options.onDone) options.onDone(RRs.cachedResponses);
        } catch (ex) {
            if (options.onFail) options.onFail(ex);
            else showJson({ ex: ex });
        }
        hideWorking(workingObject);
    }).fail(function (jqXhr, textStatus) {
        if (jqXhr.status === 401) {
            var isRefreshOnly = requests.length === 1 && requests[0].Method === "Zzz.AppEndProxy.RefreshToken";
            if (isRefreshOnly || isRetry) {
                redirectToLogin();
                hideWorking(workingObject);
            } else {
                handle401AndRetry(requests, options, RRs, workingObject);
            }
        } else {
            if (options.onFail) options.onFail({ jqXhr: jqXhr, textStatus: textStatus });
            else showJson({ jqXhr: jqXhr, textStatus: textStatus });
            hideWorking(workingObject);
        }
    });
}

/**
 * Async RPC call
 */
function rpc(optionsOrig) {
    optionsOrig = normalizeOptions(optionsOrig);
    var workingObject = optionsOrig.silent === true ? null : showWorking(optionsOrig.loadingModel);
    var RRs = analyzeRequests(optionsOrig.requests);
    var options = _.cloneDeep(optionsOrig);
    options.requests = _.cloneDeep(RRs.todoRequests);

    options.requests = _.filter(options.requests, function (rq) {
        var tR = JSON.stringify(rq);
        return (tR.indexOf('"&[') === -1 || tR.indexOf("SaveDbObjectBody") > -1);
    });

    if (options.requests.length > 0) {
        executeRpcAjax(options.requests, options, RRs, workingObject);
    } else {
        if (options.onDone) options.onDone(RRs.cachedResponses);
        hideWorking(workingObject);
    }
}

/**
 * Try RefreshToken synchronously; returns true if successful
 */
function tryRefreshTokenSync() {
    try {
        var refreshOpts = normalizeOptions({ requests: [{ Method: "Zzz.AppEndProxy.RefreshToken", Inputs: {} }] });
        var refreshJq = $.ajax(getRpcConf(refreshOpts.requests, false));
        if (refreshJq.status !== 200) return false;
        var refreshRes = JSON.parse(refreshJq.responseText);
        var resp = Array.isArray(refreshRes) ? refreshRes[0] : refreshRes;
        if (resp && resp.IsSucceeded === true && resp.Result && resp.Result.Result === true) {
            if (typeof setAsLogedIn === "function") setAsLogedIn();
            return true;
        }
    } catch (e) { }
    return false;
}

/**
 * Sync RPC call (handles 401 by trying RefreshToken then retry)
 */
function rpcSync(optionsOrig) {
    optionsOrig = normalizeOptions(optionsOrig);
    let RRs = analyzeRequests(optionsOrig.requests);
    let options = _.cloneDeep(optionsOrig);
    options.requests = _.cloneDeep(RRs.todoRequests);
    let res = "";
    if (options.requests.length > 0) {
        let workingObject = optionsOrig.silent === true ? null : showWorking(optionsOrig.loadingModel);
        let jqXHR = $.ajax(getRpcConf(options.requests, false));
        res = jqXHR.responseText || "";
        if (jqXHR.status === 401) {
            var isRefreshOnly = options.requests.length === 1 && options.requests[0].Method === "Zzz.AppEndProxy.RefreshToken";
            if (!isRefreshOnly && tryRefreshTokenSync()) {
                jqXHR = $.ajax(getRpcConf(options.requests, false));
                res = jqXHR.responseText || "";
            }
            if (jqXHR.status === 401) {
                hideWorking(workingObject);
                redirectToLogin();
                return [];
            }
        }
        hideWorking(workingObject);
    }
    try {
        let resps = !_.isObject(res) ? JSON.parse(res) : res;
        resps = !_.isObject(resps) ? JSON.parse(resps) : resps;
        cacheResponses(options.requests, resps);
        showUnHandledErrors(resps);
        RRs.cachedResponses.push.apply(RRs.cachedResponses, resps);
        return RRs.cachedResponses;
    } catch (ex) {
        handleError(options.requests, res);
        return [];
    }
}

/**
 * RPC call to AppEndProxy
 */
function rpcAEP(method, inputs, onDone, onFail, silent) {
    rpc({ requests: [{ "Method": "Zzz.AppEndProxy." + method, "Inputs": fixNull(inputs, {}) }], onDone: onDone, onFail: onFail, silent: silent });
}

/**
 * Analyze requests for caching
 */
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

/**
 * Cache responses
 */
function cacheResponses(requests, responses) {
    if (requests.length === 0 || responses.length === 0) return;
    _.forEach(responses, function (resp) {
        let rqst = _.filter(requests, function (r) { return r.Id.toString().toLowerCase() === resp.Id.toString().toLowerCase(); })[0];
        if (resp["IsSucceeded"].toString().toLowerCase() === 'true' && rqst.cacheTime > 0) {
            sessionSet(rqst.cacheKey, resp, rqst.cacheTime);
        }
    });
}

/**
 * Show unhandled errors
 */
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
                    _.forEach(r["Data"], function (v, p) {
                        let p1 = `<span class="text-secondary fs-d9">'${p}'</span > : `;
                        let p2 = p.toString().toLowerCase().indexOf('sql') === -1
                            ? `<span class="text-dark fw-bold fs-d9">${v}</span>`
                            : `<div class="text-dark fw-bold fs-d9">${v.replaceAll(';', '; <br /><br />')}</div>`;
                        content += `<div>${p1}${p2}</div>`;
                    });
                }
                if (r["StackTraceString"]) {
                    content += '<hr class="my-2 mb-0" />';
                    content += '<span class="text-dark fs-d7">' + r["StackTraceString"].replace('at ', '').replaceAll('at ', '<br />') + '</span>';
                }
                openComponent("/a.SharedComponents/BaseContent", {
                    title: "Error", windowSizeSwitchable: false, modalSize: "modal-fullscreen",
                    params: {
                        content: {
                            Title: `<div style="direction:ltr !important;text-align:left !important;width:100%;">${title}</div>`,
                            ContentBody: `<div style="direction:ltr !important;text-align:left !important;">${content.trim()}</div>`
                        }
                    }
                });
            } else {
                showJson(resp);
            }
        }
        i++;
    });
}

/**
 * Handle error
 */
function handleError(requests, responses) {
    try {
        log({ responses: responses, requests: requests });
    } catch (ex) {
        log(ex);
    }
}

/**
 * Get RPC configuration for AJAX
 * withCredentials: true is mandatory for cookies to be sent to /talk-to-me
 */
function getRpcConf(request, async) {
    return {
        type: 'POST', async: async, Accept: "application/json", dataType: "json", url: shared.talkPoint,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: (_.isObject(request) ? JSON.stringify(request) : request).toString(),
        xhrFields: { withCredentials: true }
    };
}

/**
 * Normalize options for RPC
 */
function normalizeOptions(options) {
    if (!_.isArray(options.requests)) options.requests = [options.requests];
    options.loadingModel = options.loadingModel || shared.notHeavyWorkingCover;
    options.silent = fixNull(options.silent, false);

    _.forEach(options.requests, function (rqst) {
        rqst.Id = fixNull(rqst.Id, genUN("id_"));
        rqst.cacheTime = fixNull(rqst.cacheTime, 0).toString().toInt();
        rqst.cacheKey = rqst.cacheTime > 0 ? genCacheKey(rqst) : '';
        rqst.Lang = fixNull(options.lang, getCurrentLang());
    });
    return options;
}

/**
 * Result of Response - Extract first result from response array
 */
function R0R(r) {
    try {
        if (!r) return {};
        // If server returned a single object instead of array, normalize
        if (!Array.isArray(r)) r = [r];
        if (r.length === 0) return {};
        const first = r[0];
        if (!first) return {};
        if (first.Result !== undefined && first.Result !== null) return first.Result;
        // fallback: if response shape is different, return empty object to avoid runtime exceptions
        return {};
    } catch (ex) {
        console.error('R0R parse error', ex);
        return {};
    }
}

/**
 * Generate cache key for request
 */
function genCacheKey(rqst) {
    let r = _.cloneDeep(rqst);
    if (fixNull(r["Id"], '') !== '') delete r["Id"];
    if (fixNull(r["cacheTime"], '') !== '') delete r["cacheTime"];
    return "ck_" + JSON.stringify(r).hashCode();
}
