// ============================================
// Window and Modal Management
// ============================================
// This module handles all window and modal-related operations including:
// - Opening and closing components
// - Window size switching
// - Modal dialog management
// - Floating window support with drag and resize

// ============================================
// Component Window Management
// ============================================

let _aeFloatingZIndex = 10000;

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
        resizable: false,
        draggable: false,
        modalMargin: "p-lg-5 p-md-3 p-sm-1",
        params: {},
        width: 500,
        height: 450,
        top: null,
        left: null
    });
    if (options.modalSize === 'modal-fullscreen') options.windowSizeSwitchable = false;
    if (fixNull(options.title, '') === '') options.title = src;

    options.animation = options.animation.replaceAll("$dir$", getLayoutDir()).replaceAll("$DirHand$", getLayoutDir() === 'rtl' ? "Right" : "Left");

    shared["params_" + id] = options.params;

    // Reuse a single modal container if possible to reduce DOM churn
    let containerId = 'ae-modal-container';
    let container = document.getElementById(containerId);
    if (!container) {
        container = document.createElement('div');
        container.id = containerId;
        document.body.appendChild(container);
    }

    if (options.modal === false) {
        createFloatingWindow(container);
    } else {
        createWindow(container);
    }

    function createWindow(root) {
        let dBody = $(getDialogHtml());
        root.appendChild(dBody.get(0));
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
                // Avoid re-initializing overlays globally; only for modal body
                $(m).find('.scrollable').overlayScrollbars({});
                m.focus();
                runWidgets();
                if (options.draggable && options.showHeader) {
                    _initModalDraggable(m);
                }
                if (options.resizable) {
                    _initModalResizable(m);
                }
            });
            m.addEventListener('hidden.bs.modal', event => {
                setTimeout(function () {
                    // Recycle by clearing content instead of removing container node
                    m.remove();
                    setTimeout(function () {
                        $(".modal:last").focus();
                    }, 50);
                }, 100);
            });
            mdl.show();
        });
    }

    function createFloatingWindow(root) {
        let dBody = $(getFloatingHtml());
        root.appendChild(dBody.get(0));
        $(document).ready(function () {
            let app = Vue.createApp();
            app.config.globalProperties.shared = shared;
            app.config.warnHandler = () => null;
            app.component('comp-loader', loadVM("/a.SharedComponents/BaseComponentLoader.vue"));
            app.mount(options.sharpId);

            let floatWin = document.getElementById(options.id);

            _aeFloatingZIndex++;
            floatWin.style.zIndex = _aeFloatingZIndex;

            floatWin.addEventListener('mousedown', () => {
                _aeFloatingZIndex++;
                floatWin.style.zIndex = _aeFloatingZIndex;
            });

            if (options.draggable && options.showHeader) {
                _initFloatingDrag(floatWin);
            }
            if (options.resizable) {
                _initFloatingResize(floatWin);
            }

            setTimeout(() => {
                $("#c_" + options.id).attr("data-ae-ready", "true");
                runWidgets();
            }, 100);
        });
    }

    function getDialogHtml() {
        let comp = `<comp-loader src="` + src + `" uid="c_` + options.id + `" cid="` + options.id + `" ismodal="true" />`;
        let modalClose = options.showCloseButton !== true ? "" : `<button type="button" class="btn btn-sm p-0 border-0" style="border-width:0px !important;" data-bs-dismiss="modal" aria-label="Close"><i class="fa-solid fa-times fa-fw text-secondary text-hover-dark fs-1d2"></i></button>`;
        let modalMaxiBtn = options.windowSizeSwitchable !== true ? "" : `<button type="button" class="btn btn-sm p-0 me-2 border-0" style="border-width:0px !important;" onclick="switchWindowSize(this);"><i class="fa-solid fa-expand fa-fw text-secondary text-hover-dark fs-1d2"></i></button>`;
        let modalHeader = options.showHeader ? `<div ondblclick="alert('${src}');" class="modal-header input-group input-group-sm p-2 pb-1 ${options.headerCSS}" style="border-top-left-radius:10px;border-top-right-radius:10px;"><div class="modal-title fw-bold fs-d8">${shared.translate(options.title)}</div><input class="form-control bg-transparent border-0" disabled />${modalMaxiBtn}${modalClose}<div>&nbsp;</div></div>` : "";
        let modalBody = `<div class="modal-body p-0" style="${options.showHeader ? '' : 'border-top-left-radius:10px;border-top-right-radius:10px;'}border-bottom-left-radius:10px;border-bottom-right-radius:10px;overflow:hidden;"><div class="h-100 ${options.modalBodyCSS}" data-ae-overlaycontainer="${id}">${comp}</div></div>`;
        let modalContent = `<div class="modal-content ${options.border}" style="border-radius:10px;overflow:hidden;box-shadow:0 10px 40px rgba(0,0,0,0.3),0 2px 10px rgba(0,0,0,0.2);">${modalHeader}${modalBody}</div>`;
        let backdrop = options.backdrop === false ? 'data-bs-backdrop="false"' : (options.closeByOverlay === false ? 'data-bs-backdrop="static"' : '');
        let modalCss = `modal-dialog border-0 ${options.modalSize} ${options.placement} modal-fullscreen-lg-down ${(options.modalSize === 'modal-fullscreen' ? options.modalMargin : '')}`; // modal-dialog-scrollable
        return `<div class="modal ${options.animation}" id="${id}" tabindex="-1" aria-hidden="true" ${backdrop}><div class="${modalCss}">${modalContent}</div></div>`;
    }

    function getFloatingHtml() {
        let comp = `<comp-loader src="${src}" uid="c_${id}" cid="${id}" ismodal="true" />`;
        let closeBtn = options.showCloseButton
            ? `<button type="button" class="btn btn-sm p-0 border-0" style="border-width:0px !important;" onclick="closeFloatingWindow('${id}')"><i class="fa-solid fa-times fa-fw text-secondary text-hover-dark fs-1d2"></i></button>`
            : '';
        let maxiBtn = options.windowSizeSwitchable
            ? `<button type="button" class="btn btn-sm p-0 me-2 border-0" style="border-width:0px !important;" onclick="toggleFloatingMaximize('${id}')"><i class="fa-solid fa-expand fa-fw text-secondary text-hover-dark fs-1d2"></i></button>`
            : '';

        let header = options.showHeader
            ? `<div class="ae-float-header ${options.headerCSS}"><div class="ae-float-title fw-bold fs-d8">${shared.translate(options.title)}</div><div style="flex:1;"></div>${maxiBtn}${closeBtn}</div>`
            : '';

        let resizeHandles = options.resizable
            ? `<div class="ae-resize-handle ae-resize-n"></div><div class="ae-resize-handle ae-resize-s"></div><div class="ae-resize-handle ae-resize-e"></div><div class="ae-resize-handle ae-resize-w"></div><div class="ae-resize-handle ae-resize-nw"></div><div class="ae-resize-handle ae-resize-ne"></div><div class="ae-resize-handle ae-resize-sw"></div><div class="ae-resize-handle ae-resize-se"></div>`
            : '';

        let top = options.top !== null ? options.top : Math.max(60, (window.innerHeight - options.height) / 2);
        let left = options.left !== null ? options.left : Math.max(50, (window.innerWidth - options.width) / 2);

        return `<div class="ae-floating-window ${options.border}" id="${id}" style="top:${top}px;left:${left}px;width:${options.width}px;height:${options.height}px;">${header}<div class="ae-float-body ${options.modalBodyCSS}" data-ae-overlaycontainer="${id}">${comp}</div>${resizeHandles}</div>`;
    }
}

// ============================================
// Floating Window Drag
// ============================================
function _initFloatingDrag(windowEl) {
    let header = windowEl.querySelector('.ae-float-header');
    if (!header) return;

    let isDragging = false;
    let startX, startY, startLeft, startTop;
    header.style.cursor = 'move';

    const onMouseDown = (e) => {
        if (e.target.closest('button')) return;
        if (windowEl.dataset.aeMaximized === 'true') return;
        isDragging = true;
        startX = e.clientX;
        startY = e.clientY;
        startLeft = windowEl.offsetLeft;
        startTop = windowEl.offsetTop;
        document.body.style.userSelect = 'none';
        e.preventDefault();
    };
    const onMouseMove = (e) => {
        if (!isDragging) return;
        let newLeft = startLeft + (e.clientX - startX);
        let newTop = Math.max(0, startTop + (e.clientY - startY));
        windowEl.style.left = newLeft + 'px';
        windowEl.style.top = newTop + 'px';
    };
    const onMouseUp = () => {
        if (isDragging) {
            isDragging = false;
            document.body.style.userSelect = '';
        }
    };

    header.addEventListener('mousedown', onMouseDown);
    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);

    // Touch support
    header.addEventListener('touchstart', (e) => {
        if (e.target.closest('button')) return;
        if (windowEl.dataset.aeMaximized === 'true') return;
        let t = e.touches[0];
        isDragging = true;
        startX = t.clientX;
        startY = t.clientY;
        startLeft = windowEl.offsetLeft;
        startTop = windowEl.offsetTop;
    }, { passive: true });
    document.addEventListener('touchmove', (e) => {
        if (!isDragging) return;
        let t = e.touches[0];
        windowEl.style.left = (startLeft + (t.clientX - startX)) + 'px';
        windowEl.style.top = Math.max(0, startTop + (t.clientY - startY)) + 'px';
    }, { passive: true });
    document.addEventListener('touchend', () => { isDragging = false; });
}

// ============================================
// Floating Window Resize
// ============================================
function _initFloatingResize(windowEl) {
    let handles = windowEl.querySelectorAll('.ae-resize-handle');
    let isResizing = false;
    let curHandle = null;
    let startX, startY, startW, startH, startL, startT;

    handles.forEach(h => {
        h.addEventListener('mousedown', (e) => {
            if (windowEl.dataset.aeMaximized === 'true') return;
            isResizing = true;
            curHandle = h;
            startX = e.clientX;
            startY = e.clientY;
            let rect = windowEl.getBoundingClientRect();
            startW = rect.width;
            startH = rect.height;
            startL = windowEl.offsetLeft;
            startT = windowEl.offsetTop;
            document.body.style.userSelect = 'none';
            e.preventDefault();
            e.stopPropagation();
        });
    });

    document.addEventListener('mousemove', (e) => {
        if (!isResizing || !curHandle) return;
        let dx = e.clientX - startX;
        let dy = e.clientY - startY;
        let minW = 250, minH = 200;
        let cls = curHandle.className;

        if (cls.includes('ae-resize-e') || cls.includes('ae-resize-ne') || cls.includes('ae-resize-se')) {
            windowEl.style.width = Math.max(minW, startW + dx) + 'px';
        }
        if (cls.includes('ae-resize-w') || cls.includes('ae-resize-nw') || cls.includes('ae-resize-sw')) {
            let nw = Math.max(minW, startW - dx);
            if (nw > minW) { windowEl.style.width = nw + 'px'; windowEl.style.left = (startL + dx) + 'px'; }
        }
        if (cls.includes('ae-resize-s') || cls.includes('ae-resize-se') || cls.includes('ae-resize-sw')) {
            windowEl.style.height = Math.max(minH, startH + dy) + 'px';
        }
        if (cls.includes('ae-resize-n') || cls.includes('ae-resize-ne') || cls.includes('ae-resize-nw')) {
            let nh = Math.max(minH, startH - dy);
            if (nh > minH) { windowEl.style.height = nh + 'px'; windowEl.style.top = (startT + dy) + 'px'; }
        }
    });

    document.addEventListener('mouseup', () => {
        if (isResizing) { isResizing = false; curHandle = null; document.body.style.userSelect = ''; }
    });
}

// ============================================
// Modal Drag & Resize
// ============================================
function _initModalDraggable(modalEl) {
    let header = modalEl.querySelector('.modal-header');
    let dialog = modalEl.querySelector('.modal-dialog');
    if (!header || !dialog) return;

    let isDragging = false;
    let startX, startY, startLeft, startTop;
    header.style.cursor = 'move';

    header.addEventListener('mousedown', (e) => {
        if (e.target.closest('button')) return;
        isDragging = true;
        startX = e.clientX;
        startY = e.clientY;
        startLeft = parseInt(dialog.style.left) || 0;
        startTop = parseInt(dialog.style.top) || 0;
        dialog.style.position = 'relative';
        document.body.style.userSelect = 'none';
        e.preventDefault();
    });

    document.addEventListener('mousemove', (e) => {
        if (!isDragging) return;
        dialog.style.left = (startLeft + (e.clientX - startX)) + 'px';
        dialog.style.top = (startTop + (e.clientY - startY)) + 'px';
    });

    document.addEventListener('mouseup', () => {
        if (isDragging) { isDragging = false; document.body.style.userSelect = ''; }
    });
}

function _initModalResizable(modalEl) {
    let dialog = modalEl.querySelector('.modal-dialog');
    let content = modalEl.querySelector('.modal-content');
    if (!dialog || !content) return;

    let handle = document.createElement('div');
    handle.className = 'ae-modal-resize-handle';
    content.appendChild(handle);

    let isResizing = false;
    let startX, startY, startWidth, startHeight;

    handle.addEventListener('mousedown', (e) => {
        isResizing = true;
        startX = e.clientX;
        startY = e.clientY;
        let rect = content.getBoundingClientRect();
        startWidth = rect.width;
        startHeight = rect.height;
        dialog.style.maxWidth = 'none';
        dialog.style.width = startWidth + 'px';
        content.style.height = startHeight + 'px';
        document.body.style.userSelect = 'none';
        e.preventDefault();
        e.stopPropagation();
    });

    document.addEventListener('mousemove', (e) => {
        if (!isResizing) return;
        dialog.style.maxWidth = 'none';
        dialog.style.width = Math.max(250, startWidth + (e.clientX - startX)) + 'px';
        content.style.height = Math.max(200, startHeight + (e.clientY - startY)) + 'px';
    });

    document.addEventListener('mouseup', () => {
        if (isResizing) { isResizing = false; document.body.style.userSelect = ''; }
    });
}

// ============================================
// Floating Window Close & Maximize
// ============================================
function closeFloatingWindow(id) {
    let el = document.getElementById(id);
    if (el) {
        el.style.transition = 'opacity 0.2s, transform 0.2s';
        el.style.opacity = '0';
        el.style.transform = 'scale(0.95)';
        setTimeout(() => el.remove(), 200);
    }
}

function toggleFloatingMaximize(id) {
    let el = document.getElementById(id);
    if (!el) return;
    let btn = el.querySelector('.ae-float-header .fa-expand, .ae-float-header .fa-compress');

    if (el.dataset.aeMaximized === 'true') {
        el.style.left = el.dataset.aeRestoreLeft;
        el.style.top = el.dataset.aeRestoreTop;
        el.style.width = el.dataset.aeRestoreWidth;
        el.style.height = el.dataset.aeRestoreHeight;
        el.style.borderRadius = '10px';
        el.dataset.aeMaximized = 'false';
        if (btn) { btn.classList.remove('fa-compress'); btn.classList.add('fa-expand'); }
    } else {
        el.dataset.aeRestoreLeft = el.style.left;
        el.dataset.aeRestoreTop = el.style.top;
        el.dataset.aeRestoreWidth = el.style.width;
        el.dataset.aeRestoreHeight = el.style.height;
        el.style.left = '0px';
        el.style.top = '0px';
        el.style.width = '100vw';
        el.style.height = '100vh';
        el.style.borderRadius = '0';
        el.dataset.aeMaximized = 'true';
        if (btn) { btn.classList.remove('fa-expand'); btn.classList.add('fa-compress'); }
    }
}

// ============================================
// Window Size Switch & Close
// ============================================
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
    let el = document.getElementById(cid);
    if (el && el.classList.contains('ae-floating-window')) {
        closeFloatingWindow(cid);
    } else {
        let mdl = $("#" + cid);
        mdl.modal("hide");
    }
}
