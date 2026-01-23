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
    //return encodeURIComponent(str.replaceAll(`'`, `&apos;`));
}
function deS(str) {
    return decodeURIComponent(str);
    //return decodeURIComponent(str).replaceAll(`&apos;`, `\'`);
}


function formatDate(date) {
    if (date) {
        var d = new Date(date), month = '' + (d.getMonth() + 1), day = '' + d.getDate(), year = d.getFullYear();
        return [year, format2Char(month), format2Char(day)].join('-');
    } else {
        return "";
    }
}
function formatDateTime(date) {
    if (date) {
        return formatDate(date) + " " + format2Char((new Date(date)).getHours()) + ":" + format2Char((new Date(date)).getMinutes());
    } else {
        return "";
    }
}

function formatDateL(date, calendarType) {
    let res = "";
    if (date) {
        let d = new Date(date), month = '' + (d.getMonth() + 1), day = '' + d.getDate(), year = d.getFullYear();

        if (calendarType.toLowerCase() === 'jalali') {
            var dateFormat = new Intl.DateTimeFormat("fa", { year: "numeric", month: "2-digit", day: "2-digit" });
            res = dateFormat.format(d);
        } else {
            res = [year, format2Char(month), format2Char(day)].join('-');
        }
    }
    return traverseByCalendarType(res, calendarType);
}
function formatDateTimeL(date, calendarType) {
    let res = "";
    if (date) {
        res =  formatDateL(date, calendarType) + " " + format2Char((new Date(date)).getHours()) + ":" + format2Char((new Date(date)).getMinutes());
    }
    return traverseByCalendarType(res, calendarType);
}

function traverseByCalendarType(res, calendarType) {
    if (calendarType.toLowerCase() === 'jalali') {
        return traverseFa(res);
    } else if (calendarType.toLowerCase() === 'hijri') {
        return traverseAr(res);
    }
    else {
        return traverseEn(res);
    }
}

function AddDay(strDate, intNum) {
    let date = new Date(strDate);
    const copy = new Date(Number(date));
    copy.setDate(date.getDate() + intNum);
    return copy;
}

function AddWeek(strDate, intNum) {
    let date = new Date(strDate);
    const copy = new Date(Number(date));
    copy.setDate(date.getDate() + intNum * 7);
    return copy;
}

function AddMonth(strDate, intNum) {
    let sdate = new Date(strDate);
    sdate.setMonth(sdate.getMonth() + intNum);
    return new Date(sdate.getFullYear() + "-" + (sdate.getMonth() + 1) + "-" + sdate.getDate());
}

function compileKnownDateString(s) {
    let ss = s.replace("eval:", "");

    if (ss == "currentDay") return formatDate(new Date());
    if (ss == "lastDay") return AddDay(formatDate(new Date()), -1);
    if (ss == "lastWeek") return AddWeek(formatDate(new Date()), -1);
    if (ss == "last2Week") return AddWeek(formatDate(new Date()), -2);
    if (ss == "next3Week") return AddWeek(formatDate(new Date()), -3);
    if (ss == "last4Week") return AddWeek(formatDate(new Date()), -4);
    if (ss == "lastMonth") return AddMonth(formatDate(new Date()), -1);
    if (ss == "last3Month") return AddMonth(formatDate(new Date()), -3);
    if (ss == "last6Month") return AddMonth(formatDate(new Date()), -6);
    if (ss == "lastYear") return AddMonth(formatDate(new Date()), -12);
    if (ss == "last2Year") return AddMonth(formatDate(new Date()), -24);
    if (ss == "last3Year") return AddMonth(formatDate(new Date()), -36);
    if (ss == "last4Year") return AddMonth(formatDate(new Date()), -48);
    if (ss == "last5Year") return AddMonth(formatDate(new Date()), -60);

    if (ss == "nextDay") return AddDay(formatDate(new Date()), 1);
    if (ss == "nextWeek") return AddWeek(formatDate(new Date()), 1);
    if (ss == "next2Week") return AddWeek(formatDate(new Date()), 2);
    if (ss == "next3Week") return AddWeek(formatDate(new Date()), 3);
    if (ss == "next4Week") return AddWeek(formatDate(new Date()), 4);
    if (ss == "nextMonth") return AddMonth(formatDate(new Date()), 1);
    if (ss == "next3Month") return AddMonth(formatDate(new Date()), 3);
    if (ss == "next6Month") return AddMonth(formatDate(new Date()), 6);
    if (ss == "nextYear") return AddMonth(formatDate(new Date()), 12);
    if (ss == "next2Year") return AddMonth(formatDate(new Date()), 24);
    if (ss == "next3Year") return AddMonth(formatDate(new Date()), 36);
    if (ss == "last4Year") return AddMonth(formatDate(new Date()), 48);
    if (ss == "next5Year") return AddMonth(formatDate(new Date()), 60);

}

function formatNumber(x) {
    if (x === 0) return 0;
    if (fixNull(x, '') == '') return '?';
    x = x.toString();
    var pattern = /(-?\d+)(\d{3})/;
    while (pattern.test(x))
        x = x.replace(pattern, "$1,$2");
    return x;
}
function format2Char(s) {
    let ss = s.toString();
    if (ss.length == 0) return '00';
    if (ss.length == 1) return '0' + ss;
    return ss;
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
function isPlainText(file) {
    if (file.endsWith('.txt') || file.endsWith('.html') || file.endsWith('.html') || file.endsWith('.js') ||
        file.endsWith('.json') || file.endsWith('.xml') || file.endsWith('.conf') || file.endsWith('.config') ||
        file.endsWith('.cs') || file.endsWith('.css')) return true;
    return false;
}

function getLangFromFileName(filePath) {
    if (filePath.endsWith('.cs')) return 'csharp';
    if (filePath.endsWith('.js')) return 'javascript';
    if (filePath.endsWith('.json')) return 'json';
    if (filePath.endsWith('.xml')) return 'xml';
    if (filePath.endsWith('.html')) return 'html';
    if (filePath.endsWith('.htm')) return 'html';
    if (filePath.endsWith('.css')) return 'css';
    if (filePath.endsWith('.txt')) return 'text';
    return '';
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

function isNumberString(str) {
    var p = /[0-9]/g;
    str = traverseEn(str);
    return p.test(str);
}
function isCharsString(str) {
    var p = /\D/g;
    str = traverseEn(str);
    return p.test(str);
}
function isPersian(str) {
    var p = /^[\u0600-\u06FF\s]+$/;
    str = traverseFa(str);
    return p.test(str);
}
function isArabic(str) {
    var p = /^[\u0600-\u06FF\s]+$/;
    str = traverseAr(str);
    return p.test(str);
}
function isEnglish(str) {
    var p = /^[A-Za-z0-9]*$/;
    str = traverseEn(str);
    return p.test(str);
}
function isRtlString(str) {
    return str.trim().substring(0, 1).match(/[\u0590-\u083F]|[\u08A0-\u08FF]|[\uFB1D-\uFDFF]|[\uFE70-\uFEFF]/mg);
}
function isNaN(s) {
    if (s === undefined || s === null) return true;
    return false;
}
function isNaNOrEmpty(s) {
    if (s === undefined || s === null || s.toString().trim() === '') return true;
    return false;
}
function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
function isInteger(n) {
    return Number.isInteger(parseInt(n)) && n.toString().indexOf('.') === -1;
}
function isDateTime(str) {
    var p = /\d{4}-[01]\d-[0-3]\d [0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?/gm;
    return p.test(str);
}
function isDate(str) {
    var p = /^\d{4}-\d{2}-\d{2}$/gm;
    return p.test(str);
}
function isJalaliDateTime(str) {
    var p = /\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?/gm;
    return p.test(str);
}
function isJalaliDate(str) {
    var p = /^\d{4}-\d{2}-\d{2}$/gm;
    return p.test(str);
}
function isHijriDateTime(str) {
    var p = /\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?/gm;
    return p.test(str);
}
function isHijriDate(str) {
    var p = /^\d{4}-\d{2}-\d{2}$/gm;
    return p.test(str);
}

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}
function isValidProgName(fieldName) {
    var regex = /^[a-zA-Z_][a-zA-Z0-9_]*$/;
    return regex.test(fieldName);
}

function isNullOrUndefined(v) {
    if (v === null || v === undefined) return true;
    return false;
}

function traverseAr(str) {
    return str.replace(/0/g, '٠').replace(/1/g, '١').replace(/2/g, '٢').replace(/3/g, '٣').replace(/4/g, '٤')
        .replace(/5/g, '٥').replace(/6/g, '٦').replace(/7/g, '٧').replace(/8/g, '٨').replace(/9/g, '٩')
        .replace(/۰/g, '٠').replace(/۱/g, '١').replace(/۲/g, '٢').replace(/۳/g, '٣').replace(/۴/g, '٤')
        .replace(/۵/g, '٥').replace(/۶/g, '٦').replace(/۷/g, '٧').replace(/۸/g, '٨').replace(/۹/g, '٩');
}
function traverseFa(str) {
    return str.replace(/0/g, '۰').replace(/1/g, '۱').replace(/2/g, '۲').replace(/3/g, '۳').replace(/4/g, '۴')
        .replace(/5/g, '۵').replace(/6/g, '۶').replace(/7/g, '۷').replace(/8/g, '۸').replace(/9/g, '۹')
        .replace(/٠/g, '۰').replace(/١/g, '۱').replace(/٢/g, '۲').replace(/٣/g, '۳').replace(/٤/g, '۴')
        .replace(/٥/g, '۵').replace(/٦/g, '۶').replace(/٧/g, '۷').replace(/٨/g, '۸').replace(/٩/g, '۹');
}
function traverseEn(str) {
    if (!str) return str;
    return str.replace(/۰/g, '0').replace(/۱/g, '1').replace(/۲/g, '2').replace(/۳/g, '3').replace(/۴/g, '4')
        .replace(/۵/g, '5').replace(/۶/g, '6').replace(/۷/g, '7').replace(/۸/g, '8').replace(/۹/g, '9')
        .replace(/٠/g, '0').replace(/١/g, '1').replace(/٢/g, '2').replace(/٣/g, '3').replace(/٤/g, '4')
        .replace(/٥/g, '5').replace(/٦/g, '6').replace(/٧/g, '7').replace(/٨/g, '8').replace(/٩/g, '9');
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

function bytesToSize(fileSizeInBytes) {
    var i = -1;
    var byteUnits = [' kB', ' MB', ' GB', ' TB', 'PB', 'EB', 'ZB', 'YB'];
    fileSizeInBytes = fixNull(fileSizeInBytes, 0);
    do {
        fileSizeInBytes /= 1024;
        i++;
    } while (fileSizeInBytes > 1024);

    return Math.max(fileSizeInBytes, 0.1).toFixed(1) + byteUnits[i];
}

function exportCSV(objArray, transF) {
    var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
    var str = '\uFEFF';
    var fields = Object.keys(array[0])

    let l = '';
    for (var index in fields) {
        if (l != '') l += ',';
        let t = fields[index];
        if (transF) t = transF(t);
        l += t;
    }
    str = str + l + '\r\n';

    for (var i = 0; i < array.length; i++) {
        var line = '';
        for (var index in array[i]) {
            if (line != '') line += ','
            line += array[i][index];
        }
        str += line + '\r\n';
    }
    return str;
}
function downloadCSV(str, fileName) {
    var exportedFilenmae = fileName;
    var blob = new Blob([str], { type: 'text/csv;charset=utf-8;' });
    var link = document.createElement("a");
    if (link.download !== undefined) { // feature detection
        var url = URL.createObjectURL(blob);
        link.setAttribute("href", url);
        link.setAttribute("download", exportedFilenmae);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}

function downloadFile(byteArray, fileName) {
    var a = document.createElement("a");
    a.href = "data:application/octet-stream;base64, " + byteArray;
    a.download = fileName;
    a.click();
}

function resizebase64(fileName, base64, maxWidth, maxHeight, after) {
    if (!isImageFromName(fileName)) {
        if (after) after(base64);
        return;
    }
    var img = new Image();
    img.src = "data:image/png;base64, " + base64;
    img.onload = function () {
        resizeFinalProc(img, maxWidth, maxHeight, after);
    };
}

function resizeFinalProc(img, maxWidth, maxHeight, after) {

    // Max size for resize
    if (typeof (maxWidth) === 'undefined' || maxWidth === null) maxWidth = 850;
    if (typeof (maxHeight) === 'undefined' || maxHeight === null) maxHeight = 850;

    // Create and initialize two canvas
    var canvas = document.createElement("canvas");
    var ctx = canvas.getContext("2d");
    var canvasCopy = document.createElement("canvas");
    var copyContext = canvasCopy.getContext("2d");

    // Determine new ratio based on max size
    var ratio = 1;
    if (img.width > img.height) {
        if (img.width > maxWidth) ratio = maxWidth / img.width;
        else if (img.height > maxHeight) ratio = maxHeight / img.height;
    } else {
        if (img.height > maxHeight) ratio = maxHeight / img.height;
        else if (img.width > maxWidth) ratio = maxWidth / img.width;
    }

    // Draw original image in second canvas
    canvasCopy.width = img.width;
    canvasCopy.height = img.height;
    copyContext.drawImage(img, 0, 0);

    // Copy and resize second canvas to first canvas
    canvas.width = Math.ceil(img.width * ratio);
    canvas.height = Math.ceil(img.height * ratio);
    ctx.drawImage(canvasCopy, 0, 0, canvasCopy.width, canvasCopy.height, 0, 0, canvas.width, canvas.height);

    let a = canvas.toDataURL('image/jpeg', 0.7).replace("data:image/jpeg;base64,", "");

    if (after) after(a);

}

function getIconFromName(fileName) {
    let fn = fixNull(fileName, '').toString().toLowerCase();
    if (isImageFromName(fn)) return "fa-image";
    if (isVideoFromName(fn)) return "fa-file-video";
    if (isAudioFromName(fn)) return "fa-file-audio";
    if (fn.endsWith(".pdf")) return "fa-file-pdf";
    if (fn.endsWith(".txt")) return "fa-file-lines";
    if (fn.endsWith(".csv")) return "fa-file-csv";
    if (fn.endsWith(".xlsx") || fn.endsWith(".xls")) return "fa-file-excel";
    if (fn.endsWith(".docx") || fn.endsWith(".doc")) return "fa-file-word";
    if (fn.endsWith(".zip")) return "fa-file-zipper";

    return "fa-file";
}

function getContentType(fileName) {
    let fn = fileName.toString().toLowerCase();
    if (isPlainText(fn)) return "text";
    if (isImageFromName(fn)) return "image";
    if (isVideoFromName(fn)) return "video";
    if (isAudioFromName(fn)) return "audio";
    if (isAppEndPackage(fn)) return "aepkg";
    if (isZipFile(fn)) return "zip";

    return "other";
}

function getB64Str(buffer) {
    let binary = '';
    let bytes = new Uint8Array(buffer);
    let len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

function isZipFile(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".zip")) return true;
    return false;
}
function isImageFromName(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".jpg") || fn.endsWith(".jpeg") || fn.endsWith(".gif") || fn.endsWith(".png") || fn.endsWith(".plist") || fn.endsWith(".jfif") || fn.endsWith(".ico")) return true;
    return false;
}
function isVideoFromName(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".mkv") || fn.endsWith(".avi") || fn.endsWith(".flv") || fn.endsWith(".mpeg") || fn.endsWith(".mp4") || fn.endsWith(".mov")) return true;
    return false;
}
function isAudioFromName(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".mp3") || fn.endsWith(".wav") || fn.endsWith(".3gp") || fn.endsWith(".au") || fn.endsWith(".m4a") || fn.endsWith(".m4b") || fn.endsWith(".ogg") || fn.endsWith(".wma")) return true;
    return false;
}
function isPlainText(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".txt") || fn.endsWith(".htm") || fn.endsWith(".html") || fn.endsWith(".css") || fn.endsWith(".js") || fn.endsWith(".json") || fn.endsWith(".config") || fn.endsWith(".csv") || fn.endsWith(".cs") || fn.endsWith(".sql") || fn.endsWith(".vue")) return true;
    return false;
}
function isAppEndPackage(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".aepkg")) return true;
    return false;
}


function isImageFromNameFromType(fileMime) {
    if (fileMime === undefined || fileMime === null) return true;
    if (fileMime.startsWith("image")) return true;
    return false;
}

function bytesToSize(bytes) {
    var sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    if (bytes == 0) return '0B';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
    return Math.round(bytes / Math.pow(1024, i), 2) + '' + sizes[i];
}

function getB64Str(buffer) {
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

