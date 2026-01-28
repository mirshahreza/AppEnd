// ============================================
// FILE UTILITIES MODULE
// ============================================
// File operations, image processing, and file type detection
// Extracted from append-helpers.js for better organization

/**
 * Check if file is an image by name
 */
function isImageFromName(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".jpg") || fn.endsWith(".jpeg") || fn.endsWith(".gif") || fn.endsWith(".png") || fn.endsWith(".plist") || fn.endsWith(".jfif") || fn.endsWith(".ico")) return true;
    return false;
}

/**
 * Check if file is a video by name
 */
function isVideoFromName(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".mkv") || fn.endsWith(".avi") || fn.endsWith(".flv") || fn.endsWith(".mpeg") || fn.endsWith(".mp4") || fn.endsWith(".mov")) return true;
    return false;
}

/**
 * Check if file is audio by name
 */
function isAudioFromName(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".mp3") || fn.endsWith(".wav") || fn.endsWith(".3gp") || fn.endsWith(".au") || fn.endsWith(".m4a") || fn.endsWith(".m4b") || fn.endsWith(".ogg") || fn.endsWith(".wma")) return true;
    return false;
}

/**
 * Check if file is plain text
 */
function isPlainText(file) {
    if (file.endsWith('.txt') || file.endsWith('.html') || file.endsWith('.html') || file.endsWith('.js') ||
        file.endsWith('.json') || file.endsWith('.xml') || file.endsWith('.conf') || file.endsWith('.config') ||
        file.endsWith('.cs') || file.endsWith('.css')) return true;
    return false;
}

/**
 * Check if file is AppEnd package
 */
function isAppEndPackage(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".aepkg")) return true;
    return false;
}

/**
 * Check if file is a zip file
 */
function isZipFile(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".zip")) return true;
    return false;
}

/**
 * Check if file is image from MIME type
 */
function isImageFromNameFromType(fileMime) {
    if (fileMime === undefined || fileMime === null) return true;
    if (fileMime.startsWith("image")) return true;
    return false;
}

/**
 * Get language from file name for syntax highlighting
 */
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

/**
 * Get icon class from file name
 */
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

/**
 * Get content type from file name
 */
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

/**
 * Convert bytes to size string (KB, MB, etc.)
 */
function bytesToSize(bytes) {
    var sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    if (bytes == 0) return '0B';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
    return Math.round(bytes / Math.pow(1024, i), 2) + '' + sizes[i];
}

/**
 * Get base64 string from ArrayBuffer
 */
function getB64Str(buffer) {
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

/**
 * Download file from base64
 */
function downloadFile(byteArray, fileName) {
    var a = document.createElement("a");
    a.href = "data:application/octet-stream;base64, " + byteArray;
    a.download = fileName;
    a.click();
}

/**
 * Download CSV file
 */
function downloadCSV(str, fileName) {
    var exportedFilenmae = fileName;
    var blob = new Blob([str], { type: 'text/csv;charset=utf-8;' });
    var link = document.createElement("a");
    if (link.download !== undefined) {
        var url = URL.createObjectURL(blob);
        link.setAttribute("href", url);
        link.setAttribute("download", exportedFilenmae);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}

/**
 * Export array to CSV
 */
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

/**
 * Resize base64 image
 */
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

/**
 * Final processing for image resize
 */
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

/**
 * Convert base64 to image source URI
 */
function getImageURI(imageBytes) {
    if (fixNull(imageBytes, '') === '') return "/a..lib/images/avatar.png";
    return 'data:image/png;base64, ' + imageBytes;
}
