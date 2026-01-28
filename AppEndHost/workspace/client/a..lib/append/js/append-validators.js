/**
 * Check if string can be converted to an array
 */
function isArray(str) {
    try {
        var parsed = JSON.parse(str);
        return Array.isArray(parsed);
    } catch (e) {
        return false;
    }
}


function isNumberString(str) {
    var p = /[0-9]/g;
    str = traverseEn(str);
    return p.test(str);
}

/**
 * Check if string contains characters
 */
function isCharsString(str) {
    var p = /\D/g;
    str = traverseEn(str);
    return p.test(str);
}

/**
 * Check if string is Persian
 */
function isPersian(str) {
    var p = /^[\u0600-\u06FF\s]+$/;
    str = traverseFa(str);
    return p.test(str);
}

/**
 * Check if string is Arabic
 */
function isArabic(str) {
    var p = /^[\u0600-\u06FF\s]+$/;
    str = traverseAr(str);
    return p.test(str);
}

/**
 * Check if string is English
 */
function isEnglish(str) {
    var p = /^[A-Za-z0-9]*$/;
    str = traverseEn(str);
    return p.test(str);
}

/**
 * Check if string is RTL
 */
function isRtlString(str) {
    return str.trim().substring(0, 1).match(/[\u0590-\u083F]|[\u08A0-\u08FF]|[\uFB1D-\uFDFF]|[\uFE70-\uFEFF]/mg);
}

/**
 * Check if value is NaN (null or undefined)
 */
function isNaN(s) {
    if (s === undefined || s === null) return true;
    return false;
}

/**
 * Check if value is NaN or empty string
 */
function isNaNOrEmpty(s) {
    if (s === undefined || s === null || s.toString().trim() === '') return true;
    return false;
}

/**
 * Check if value is a number
 */
function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

/**
 * Check if value is an integer
 */
function isInteger(n) {
    return Number.isInteger(parseInt(n)) && n.toString().indexOf('.') === -1;
}

/**
 * Check if string is a datetime
 */
function isDateTime(str) {
    var p = /\d{4}-[01]\d-[0-3]\d [0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?/gm;
    return p.test(str);
}

/**
 * Check if string is a date
 */
function isDate(str) {
    var p = /^\d{4}-\d{2}-\d{2}$/gm;
    return p.test(str);
}

/**
 * Check if string is a Jalali datetime
 */
function isJalaliDateTime(str) {
    var p = /\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?/gm;
    return p.test(str);
}

/**
 * Check if string is a Jalali date
 */
function isJalaliDate(str) {
    var p = /^\d{4}-\d{2}-\d{2}$/gm;
    return p.test(str);
}

/**
 * Check if string is a Hijri datetime
 */
function isHijriDateTime(str) {
    var p = /\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?/gm;
    return p.test(str);
}

/**
 * Check if string is a Hijri date
 */
function isHijriDate(str) {
    var p = /^\d{4}-\d{2}-\d{2}$/gm;
    return p.test(str);
}

/**
 * Check if key pressed is a number key
 */
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

/**
 * Check if field name is a valid program name
 */
function isValidProgName(fieldName) {
    var regex = /^[a-zA-Z_][a-zA-Z0-9_]*$/;
    return regex.test(fieldName);
}

/**
 * Check if value is null or undefined
 */
function isNullOrUndefined(v) {
    if (v === null || v === undefined) return true;
    return false;
}
