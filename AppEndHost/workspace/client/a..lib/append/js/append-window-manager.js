// ============================================
// Window and Modal Management
// ============================================
// This module handles all window and modal-related operations including:
// - Opening and closing components
// - Window size switching
// - Modal dialog management

// ============================================
// Component Window Management
// ============================================

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

    shared["params_" + id] = options.params;

    // Reuse a single modal container if possible to reduce DOM churn
    let containerId = 'ae-modal-container';
    let container = document.getElementById(containerId);
    if (!container) {
        container = document.createElement('div');
        container.id = containerId;
        document.body.appendChild(container);
    }

    createWindow(container);

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
    function getDialogHtml() {
        let comp = `<comp-loader src="` + src + `" uid="c_` + options.id + `" cid="` + options.id + `" ismodal="true" />`;
        let modalClose = options.showCloseButton !== true ? "" : `<button type="button" class="btn btn-sm p-0" data-bs-dismiss="modal" aria-label="Close"><i class="fa-solid fa-times fa-fw text-secondary text-hover-dark fs-1d2"></i></button>`;
        let modalMaxiBtn = options.windowSizeSwitchable !== true ? "" : `<button type="button" class="btn btn-sm p-0 me-2" onclick="switchWindowSize(this);"><i class="fa-solid fa-expand fa-fw text-secondary text-hover-dark fs-1d2"></i></button>`;
        let modalHeader = options.showHeader ? `<div ondblclick="alert('${src}');" class="modal-header input-group input-group-sm p-2 pb-1 ${options.headerCSS}" style="border-top-left-radius:10px;border-top-right-radius:10px;"><div class="modal-title fw-bold fs-d8">${shared.translate(options.title)}</div><input class="form-control bg-transparent border-0" disabled />${modalMaxiBtn}${modalClose}<div>&nbsp;</div></div>` : "";
        let modalBody = `<div class="modal-body p-0" style="${options.showHeader ? '' : 'border-top-left-radius:10px;border-top-right-radius:10px;'}border-bottom-left-radius:10px;border-bottom-right-radius:10px;overflow:hidden;"><div class="h-100 ${options.modalBodyCSS}" data-ae-overlaycontainer="${id}">${comp}</div></div>`;
        let modalContent = `<div class="modal-content ${options.border}" style="border-radius:10px;overflow:hidden;box-shadow:0 10px 40px rgba(0,0,0,0.3),0 2px 10px rgba(0,0,0,0.2);">${modalHeader}${modalBody}</div>`;
        let backdrop = options.backdrop === false ? 'data-bs-backdrop="false"' : (options.closeByOverlay === false ? 'data-bs-backdrop="static"' : '');
        let modalCss = `modal-dialog border-0 ${options.modalSize} ${options.placement} modal-fullscreen-lg-down ${(options.modalSize === 'modal-fullscreen' ? options.modalMargin : '')}`; // modal-dialog-scrollable
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
