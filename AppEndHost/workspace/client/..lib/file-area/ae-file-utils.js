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

function getIconFromName(FileName) {
    let fn = FileName.toString().toLowerCase();
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

function getB64Str(buffer) {
    let binary = '';
    let bytes = new Uint8Array(buffer);
    let len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

function isImageFromName(fn) {
    fn = fn.toLowerCase();
    if (fn.endsWith(".jpg") || fn.endsWith(".jpeg") || fn.endsWith(".gif") || fn.endsWith(".png") || fn.endsWith(".plist") || fn.endsWith(".jfif")) return true;
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

function fixV(v, ifV) {
    if (v === undefined || v === null || v === '') return ifV;
    return v;
}
