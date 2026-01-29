function getSessionItemSync(itemName, isObject, producer) {
    if (fixNull(sessionStorage.getItem(itemName), '') !== '') {
        let v = sessionStorage.getItem(itemName);
        return (isObject === true ? JSON.parse(v) : v);
    }
    if (producer) {
        let v = producer();
        sessionStorage.setItem(itemName, (isObject === true ? JSON.stringify(v) : v));
        return v;
    } else {
        return null;
    }
}

function removeProp(obj, propName) {
    let o = _.cloneDeep(obj);
    if (Array.isArray(o)) {
        _.forEach(o, (d) => {
            delete d[propName];
        });
        return o;
    } else {
        delete o[propName];
        return o;
    }
}

function decodeB64Unicode(str) {
    return decodeURIComponent(atob(str).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
}
function decodeString(str) {
    try {
        return b64DecodeUnicode(str);
    } catch (err) {
        return "Bad Section.\nError: " + err.message;
    }
}
function decodeJwt(jwt_token) {
    var Header, Payload, Signature;
    var tokens = jwt_token.split(".")
    if (tokens.length == 3) {
        Header = decodeString(tokens[0]);
        Payload = parseJwt(jwt_token);
        if (tokens[2].length > 0) { Signature = "[Signed Token]"; } else { Signature = "[Unsigned Token]"; }
        return { header: Header, payload: Payload, signature: Signature };
    } else {
        return {};
    }
}
function parseJwt(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
}
function parseJO(str) {
    try {
        if (str) {
            return JSON.parse(str);
        } else {
            return {};
        }
    } catch (ex) {
        throw "can not parse : " + str;
    }
}
function parseJA(str) {
    try {
        if (str) {
            str = str.replaceAll(`'`, `"`);
            return JSON.parse(str);
        } else {
            return [];
        }
    } catch (ex) {
        console.log("can not parse : " + str);
        return [];
    }
}

function enS(str) {
    return encodeURIComponent(str);
}
function deS(str) {
    return decodeURIComponent(str);
}




function getTextAlignCss() {
    return (getDir() === 'rtl' ? "text-right" : "text-left");
}
function getDir() {
    return $("body").css("direction");
}
function isRtl() {
    return ($("body").css("direction") === 'rtl' ? true : false);
}



function getQueryParamByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}



function fixNull(s, ifValue, checkUndefinedOrNullText) {
    if (s === undefined || s === null) return ifValue;
    if (checkUndefinedOrNullText === true) {
        s = s.toString().replaceAll("undefined", '').replaceAll("null", '').replaceAll(",", '').trim();
        if (s === '') return ifValue;
        else return s;
    }
    return s;
}
function fixV(v, ifV) {
    if (v === undefined || v === null || v === '') return ifV;
    return v;
}
function fixNullOrEmpty(s, ifValue) {
    if (s === undefined || s === null || s === [] || s === {} || s.trim() === '') return ifValue;
    return s;
}
function fix2Char(s) {
    if (fixNull(s, '') === '') return '00';
    s = s.toString();
    if (s.length === 1) return '0' + s;
    return s;
}
function fixEndBy(str, endFix) {
    if (str.endsWith(endFix) === false) return str + endFix;
    return str;
}
function fixStartBy(str, preFix) {
    if (str.startsWith(preFix) === false) return preFix + str;
    return str;
}
function truncateString(str, maxLength) {
    let s = fixNull(str, '');
    if (s.length === 0) return '';
    return s.length > maxLength ? `${s.substring(0, maxLength)}…` : str;
}

function sessionGet(key) {
    let stringValue = window.sessionStorage.getItem(key)
    if (stringValue !== null && stringValue !== "") {
        let value = JSON.parse(stringValue);
        let expirationDate = new Date(value.expirationDate);
        if (expirationDate > new Date()) {
            return value.value;
        } else {
            window.sessionStorage.removeItem(key);
        }
    }
    return null
}
function sessionSet(key, value, expirationInMinutes = 5) {
    let expirationDate = new Date(new Date().getTime() + (60000 * expirationInMinutes))
    let newValue = { value: value, expirationDate: expirationDate.toISOString() };
    window.sessionStorage.setItem(key, JSON.stringify(newValue))
}

function enterToBr(str) {
    return str.replaceAll('\n', '<br />')
}

function animateByCSS(element, animationName, callback) {
    element.on("animationend", function () {
        if (typeof callback === 'function') callback();
    });
    element.addClass('animate__animated').addClass(animationName);
}

function b64ToImageSrc(b64) {
    if (b64 === undefined || b64 === null) return "/a..lib/images/avatar.png";
    return 'data:image/png;base64, ' + b64;
}

function eachRecursive(obj, func) {
    for (var k in obj) {
        if (typeof obj[k] === "object" && obj[k] !== null)
            eachRecursive(obj[k], func);
        else {
            if (func) func(obj, k);
        }
    }
}

function toSimpleArrayOf(arr, colName) {
    let res = [];
    _.each(arr, function (i) {
        res.push(i[colName]);
    });
    return res;
}

function prepend(element, obj) {
    return { ...obj, ...element }
}
function log(s) {
    if (shared.debug) console.log(s);
}


function refereshPage() {
    window.location.reload();
}

function goHome() {
    window.location = "/";
}


String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
};
String.prototype.matchAll = function (regexp) {
    var matches = [];
    this.replace(regexp, function () {
        var arr = ([]).slice.call(arguments, 0);
        var extras = arr.splice(-2);
        arr.index = extras[0];
        arr.input = extras[1];
        matches.push(arr);
    });
    return matches.length ? matches : null;
};
String.prototype.toBoolean = function () {
    if (this.toString().toLowerCase() === "true") return true;
    return false;
};
String.prototype.toInt = function () {
    return _.parseInt(this.toString());
};
String.prototype.hashCode = function () {
    var hash = 0;
    for (var i = 0; i < this.length; i++) {
        var character = this.charCodeAt(i);
        hash = ((hash << 5) - hash) + character;
        hash = hash & hash; // Convert to 32bit integer
    }
    return hash;
}


// matrix functions
function getMatrixColumn(matrix, colName) {
    let res = [];
    _.forEach(matrix, function (i) {
        res.push(i[colName]);
    });
    return res;
};

function concatObjects(o1, o2) {
    if (isNaN(o1) && isNaN(o2)) return {};
    if (isNaN(o1) && !isNaN(o2)) return o2;
    if (!isNaN(o1) && isNaN(o2)) return o1;

    let r = {};
    for (var prop in o1) {
        r[prop] = o1[prop];
    }

    for (var prop in o2) {
        r[prop] = o2[prop];
    }

    return r;
}

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

function genUN(prefix) {
    return _.uniqueId((fixNull(prefix, '') === '' ? "" : `${prefix}_`) + uuidv4().replaceAll("-", "") + "_");
}

function convertBoolToIcon(v, trueClasses, falseClasses, nullClasses) {
    if (v === true) return `<i class="fa-solid fa-fw ${trueClasses}"></i>`;
    if (v === false) return `<i class="fa-solid fa-fw ${falseClasses}"></i>`;
    return `<i class="fa-solid fa-fw ${nullClasses}"></i>`;
}
function convertBoolToIconWithOptions(v, options) {
    if (options.shownull === undefined || options.shownull === null) options.shownull = true;
    if (options.nullClasses === undefined || options.nullClasses === null) options.nullClasses = "fa-minus text-secondary";
    if (options.trueClasses === undefined || options.trueClasses === null) options.trueClasses = "fa-check text-success";
    if (options.falseClasses === undefined || options.falseClasses === null) options.falseClasses = "fa-xmark text-danger";


    if (v === true) return `<i class="fa-solid fa-fw ${options.trueClasses}"></i>`;
    if (v === false) return `<i class="fa-solid fa-fw ${options.falseClasses}"></i>`;
    return `<i class="fa-solid fa-fw ${options.nullClasses}"></i>`;
}

function format2Char(s) {
    let ss = s.toString();
    if (ss.length == 0) return '00';
    if (ss.length == 1) return '0' + ss;
    return ss;
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

function dbTypeIsNumerical(dbType) {
    let dbT = dbType.toLowerCase();
    if (dbT.indexOf("int") > -1) return true;
    if (dbT.indexOf("numeric") > -1) return true;
    if (dbT.indexOf("real") > -1) return true;
    if (dbT.indexOf("money") > -1) return true;
    if (dbT.indexOf("float") > -1) return true;

    return false;
}
function turnDotsToTree(items) {
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

function fixNullOptions(options) {
    if (fixNull(options, '') === '') options = {};
    if (fixNull(options.dialog, '') === '') options.dialog = {};
    return options;
}

String.prototype.getParameters = function () {
    if (this === null || this === undefined) return [];
    var re = /&\[(.*?)]/gm;
    var arr = this.matchAll(re);
    var inputs = [];
    $.each(arr, function (i, item) {
        inputs.push(item[1]);
    });
    return _.uniq(inputs);
};

/**
* Get fake parameter for cache busting
*/
function getFake(env) {
    if (env === 'dev') return "fake__" + (new Date()).getTime();
    if (fixNull(sessionStorage.getItem("fake"), '') === '') sessionStorage.setItem("fake", "fake__" + (new Date()).getTime());
    return sessionStorage.getItem("fake");
}

/**
 * Switch visibility of elements
 */
function switchVisibility(switchHandler, targetSelector, initialVisibleState, initialIcon, secondIcon) {
    let clicked = $(switchHandler);
    $(targetSelector).toggleClass(initialVisibleState);
    clicked.find(".fa-solid").toggleClass(initialIcon);
    clicked.find(".fa-solid").toggleClass(secondIcon);
}