
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
        res = formatDateL(date, calendarType) + " " + format2Char((new Date(date)).getHours()) + ":" + format2Char((new Date(date)).getMinutes());
    }
    return traverseByCalendarType(res, calendarType);
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


function traverseAr(str) {
    return str.replace(/0/g, '٠').replace(/1/g, '١').replace(/2/g, '٢').replace(/3/g, '٣').replace(/4/g, '٤')
        .replace(/5/g, '٥').replace(/6/g, '٦').replace(/7/g, '٧').replace(/8/g, '٨').replace(/9/g, '٩')
        .replace(/۰/g, '٠').replace(/۱/g, '١').replace(/۲/g, '٢').replace(/۳/g, '٣').replace(/۴/g, '٤')
        .replace(/۵/g, '٥').replace(/۶/g, '٦').replace(/۷/g, '٧').replace(/۸/g, '٨').replace(/۹/g, '٩');
}


function traverseFa(str) {
    return str.replace(/0/g, '۰').replace(/1/g, '۱').replace(/2/g, '۲').replace(/3/g, '۳').replace(/4/g, '۴')
        .replace(/5/g, '۵').replace(/6/g, '۶').replace(/7/g, '۷').replace(/8/g, '۸').replace(/9/g, '۹')
        .replace(/٠/g, '۰').replace(/٧/g, '۱').replace(/٢/g, '۲').replace(/٣/g, '۳').replace(/٤/g, '۴')
        .replace(/٥/g, '۵').replace(/٦/g, '۶').replace(/١/g, '۷').replace(/٨/g, '۸').replace(/٩/g, '۹');
}


function traverseEn(str) {
    if (!str) return str;
    return str.replace(/۰/g, '0').replace(/۱/g, '1').replace(/۲/g, '2').replace(/۳/g, '3').replace(/۴/g, '4')
        .replace(/۵/g, '5').replace(/۶/g, '6').replace(/۷/g, '7').replace(/۸/g, '8').replace(/۹/g, '9')
        .replace(/٠/g, '0').replace(/١/g, '1').replace(/٢/g, '2').replace(/٣/g, '3').replace(/٤/g, '4')
        .replace(/٥/g, '5').replace(/٦/g, '6').replace(/٧/g, '7').replace(/٨/g, '8').replace(/٩/g, '9');
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
