(function ($) {
    $.fn.bsTabsAutoNav = function (options) {
        let _this = $(this);
        initWidget();
        function initWidget() {
            options = options || {};
            options = _.defaults(options, { mode: "nav-items", stepperStyle: "default", nextTitle: "Next", prevTitle: "Previous", justAllowByBackNext: false, dir: 'ltr', navStyle:"nav-underline nav-justified", completeTitle: "Complete" });
            if (options["tabsContentsId"] === null || options["tabsContentsId"] === undefined || options["tabsContentsId"] === '') return;

            if (options["mode"] !== "back-next") _this.addClass("d-none d-sm-block");

            if (translate) {
                options.prevTitle = translate(options.prevTitle);
                options.nextTitle = translate(options.nextTitle);
                options.completeTitle = translate(options.completeTitle);
            }
            if (options.mode === 'back-next') {
                _this.html(getBackNext());
                _this.find(".btn-prev").off("click").on("click", function () { activate(-1); });
                _this.find(".btn-next").off("click").on("click", function () { activate(1); });
                setBackNextBarState();
            } else {
                if (options.stepperStyle === 'connected-circles') {
                    _this.html(getConnectedCirclesStepper());
                    _this.find(".stepper-item").off("click").on("click", function () { 
                        let index = $(this).data("step-index");
                        activateTabByIndex(index);
                    });
                } else if (options.stepperStyle === 'arrow-steps') {
                    _this.html(getArrowStepsStepper());
                    _this.find(".arrow-step-item").off("click").on("click", function () {
                        let index = $(this).data("step-index");
                        activateTabByIndex(index);
                    });
                } else if (options.stepperStyle === 'progress-bar') {
                    _this.html(getProgressBarStepper());
                    _this.find(".progress-bar-step").off("click").on("click", function () {
                        let index = $(this).data("step-index");
                        activateTabByIndex(index);
                    });
                } else {
                    _this.html(getTabsNav());
                }
                _this.parent().append(`<div class="mobile-title text-center d-block d-sm-none fw-bold p-2 fs-1d1"></div>`)
                _this.find(".nav-link").off("click").on("click", function () { setBackNextBarState(); });
            }

            setNavTabsAbility();
            activateTabByIndex(0);
        }

        function activate(bn) {
            let tabsContentsId = options["tabsContentsId"];

            let isAreaValid = $("#" + tabsContentsId).find("[data-ae-widget='inputsRegulator']").attr("data-ae-validation-flag");
            if (isAreaValid === 'false') return;

            let navItemsCount = $("#" + tabsContentsId).find(".tab-pane").length;
            let indActive = getSelectedIndex();
            if (indActive === -1) return;
            if (bn === -1 && indActive === 0) return;
            if (bn === 1 && indActive === navItemsCount - 1) return;
            let nextId = indActive + bn;
            activateTabByIndex(nextId);
        }

        function activateTabByIndex(toActivateIndex) {
            let tabsContentsId = options["tabsContentsId"];
            let toActivateTab = $("#" + tabsContentsId).find(".tab-pane").eq(toActivateIndex);
            let navBarId = `#tabsNavbar_${tabsContentsId}`;
            let navBar = $(navBarId);

            if (options.stepperStyle === 'connected-circles') {
                // Update stepper circles
                let stepperId = `#stepper_${tabsContentsId}`;
                $(stepperId).find(".stepper-item").each(function (index) {
                    $(this).removeClass("active completed");
                    if (index === toActivateIndex) {
                        $(this).addClass("active");
                    } else if (index < toActivateIndex) {
                        $(this).addClass("completed");
                    }
                });
                $("#" + tabsContentsId).find(".tab-pane").removeClass("show active");
                toActivateTab.addClass("show active");
            } else if (options.stepperStyle === 'arrow-steps') {
                let stepperId = `#stepper_${tabsContentsId}`;
                $(stepperId).find(".arrow-step-item").each(function (index) {
                    $(this).removeClass("active completed");
                    if (index === toActivateIndex) {
                        $(this).addClass("active");
                    } else if (index < toActivateIndex) {
                        $(this).addClass("completed");
                    }
                });
                $("#" + tabsContentsId).find(".tab-pane").removeClass("show active");
                toActivateTab.addClass("show active");
            } else if (options.stepperStyle === 'progress-bar') {
                let stepperId = `#stepper_${tabsContentsId}`;
                $(stepperId).find(".progress-bar-step").each(function (index) {
                    $(this).removeClass("active completed");
                    if (index === toActivateIndex) {
                        $(this).addClass("active");
                    } else if (index < toActivateIndex) {
                        $(this).addClass("completed");
                    }
                });
                $("#" + tabsContentsId).find(".tab-pane").removeClass("show active");
                toActivateTab.addClass("show active");
            } else if (navBar.length > 0 && options.justAllowByBackNext === false) {
                navBar.find(".nav-link").eq(toActivateIndex).click();
            } else {
                navBar.find(".nav-link").removeClass("active");
                navBar.find(".nav-link").eq(toActivateIndex).addClass("active");
                $("#" + tabsContentsId).find(".tab-pane").removeClass("show active");
                toActivateTab.addClass("show active");
            }
            setBackNextBarState();
            setMobileTitle(toActivateTab.attr("data-ae-tab-title"), toActivateTab.attr("data-ae-tab-icon"));
        }

        function setMobileTitle(title, icon) {
            let ico = icon === null || icon === undefined || icon === '' ? '' : `<i class="fa-solid fa-fw ${icon} me-1"></i>`;
            $(".mobile-title").html(ico + title);
        }

        function setBackNextBarState() {
            $(document).ready(function () {
                setTimeout(function () {
                    let tabsContentsId = options["tabsContentsId"];
                    let navItemsCount = $("#" + tabsContentsId).find(".tab-pane").length;
                    let activeTabIndex = getSelectedIndex();
                    let bnPane = $("#bn_" + tabsContentsId);
                    if (bnPane.length > 0) {
                        bnPane.find(".btn-prev").removeClass("disabled");
                        bnPane.find(".btn-next").removeClass("disabled");
                        if (activeTabIndex === 0) {
                            bnPane.find(".btn-prev").addClass("disabled");
                        }
                        if (activeTabIndex === navItemsCount - 1) {
                            bnPane.find(".btn-next").addClass("disabled");
                        }
                    }
                }, 100);
            });
        }
        function setNavTabsAbility() {
            if (options.justAllowByBackNext === true) {
                let tabsContentsId = options["tabsContentsId"];
                let navBarId = `#tabsNavbar_${tabsContentsId}`;
                $(navBarId).find(".nav-link").addClass("disabled");
            }
        }
        function getSelectedIndex() {
            let tabsContentsId = options["tabsContentsId"];
            let ind = 0;
            let indActive = -1;
            $("#" + tabsContentsId).find(".tab-pane").each(function () {
                let tabItem = $(this);
                if (tabItem.hasClass("active")) {
                    indActive = ind;
                }
                ind++;
            });
            return indActive;
        }
        function getConnectedCirclesStepper() {
            let tabsContentsId = options["tabsContentsId"];
            let stepItems = '';
            let stepIndex = 0;
            let tabsCount = $("#" + tabsContentsId).find(".tab-pane").length;

            $('#' + tabsContentsId + ' .tab-pane').each(function () {
                let tTitle = $(this).attr("data-ae-tab-title");
                let isCompleted = stepIndex < tabsCount;
                stepItems += genConnectedCircleItem(tTitle, stepIndex, false, tabsCount);
                stepIndex++;
            });

            // Add complete step
            stepItems += genConnectedCircleItem(options.completeTitle, tabsCount, true, tabsCount);

            return `
<div class="stepper-connected-circles" id="stepper_${tabsContentsId}">
    <div class="stepper-container">
        ${stepItems}
    </div>
</div>
            `;
        }
        function genConnectedCircleItem(title, stepIndex, isComplete, totalSteps) {
            let isActive = stepIndex === getSelectedIndex();
            let isCompleted = stepIndex < getSelectedIndex();
            let circleClass = isActive ? 'active' : (isCompleted ? 'completed' : '');
            let isLastStep = stepIndex === totalSteps;

            return `
    <div class="stepper-item ${circleClass}" data-step-index="${stepIndex}">
        <div class="stepper-circle">
            ${isCompleted ? '<i class="fa-solid fa-check"></i>' : (stepIndex + 1)}
        </div>
        <div class="stepper-label">${title}</div>
        ${!isLastStep ? '<div class="stepper-line"></div>' : ''}
    </div>
            `;
        }

        function getArrowStepsStepper() {
            let tabsContentsId = options["tabsContentsId"];
            let stepItems = '';
            let stepIndex = 0;
            let tabsCount = $("#" + tabsContentsId).find(".tab-pane").length;

            $('#' + tabsContentsId + ' .tab-pane').each(function () {
                let tTitle = $(this).attr("data-ae-tab-title");
                stepItems += genArrowStepItem(tTitle, stepIndex, tabsCount);
                stepIndex++;
            });

            return `
<div class="stepper-arrow-steps" id="stepper_${tabsContentsId}">
    <div class="arrow-steps-container">
        ${stepItems}
    </div>
</div>
            `;
        }
        function genArrowStepItem(title, stepIndex, totalSteps) {
            let selectedIndex = getSelectedIndex();
            let isActive = stepIndex === selectedIndex;
            let isCompleted = stepIndex < selectedIndex;
            let stepClass = isActive ? 'active' : (isCompleted ? 'completed' : '');
            let isFirst = stepIndex === 0;
            let isLast = stepIndex === totalSteps - 1;
            let posClass = isFirst ? 'first' : (isLast ? 'last' : '');

            return `
    <div class="arrow-step-item ${stepClass} ${posClass}" data-step-index="${stepIndex}">
        <span class="arrow-step-title">${title}</span>
        <span class="arrow-step-badge">${stepIndex + 1}</span>
    </div>
            `;
        }

        function getProgressBarStepper() {
            let tabsContentsId = options["tabsContentsId"];
            let stepItems = '';
            let stepIndex = 0;
            let tabsCount = $("#" + tabsContentsId).find(".tab-pane").length;

            $('#' + tabsContentsId + ' .tab-pane').each(function () {
                let tTitle = $(this).attr("data-ae-tab-title");
                let tIcon = $(this).attr("data-ae-tab-icon");
                stepItems += genProgressBarStepItem(tTitle, tIcon, stepIndex, tabsCount);
                stepIndex++;
            });

            return `
<div class="stepper-progress-bar" id="stepper_${tabsContentsId}">
    <div class="progress-bar-container">
        ${stepItems}
    </div>
</div>
            `;
        }
        function genProgressBarStepItem(title, icon, stepIndex, totalSteps) {
            let selectedIndex = getSelectedIndex();
            let isActive = stepIndex === selectedIndex;
            let isCompleted = stepIndex < selectedIndex;
            let stepClass = isActive ? 'active' : (isCompleted ? 'completed' : '');
            let ico = icon ? `<i class="fa-solid fa-fw ${icon}"></i>` : '';
            let nodeContent = isCompleted ? '<i class="fa-solid fa-check"></i>' : (ico !== '' ? ico : (stepIndex + 1));

            return `
    <div class="progress-bar-step ${stepClass}" data-step-index="${stepIndex}">
        <div class="progress-bar-node">
            ${nodeContent}
        </div>
        <div class="progress-bar-subline"></div>
    </div>
            `;
        }

        function getBackNext() {
            let tabsContentsId = options["tabsContentsId"];
            let next = options["dir"] === 'ltr' ? "fa-chevron-right" : "fa-chevron-left";
            let prev = options["dir"] === 'ltr' ? "fa-chevron-left" : "fa-chevron-right";

            return `
<table class="w-100 text-center" id="bn_${tabsContentsId}">
    <tr>
        <td style="width:100px">
            <button class="btn btn-sm btn-link fw-bold text-decoration-none bg-hover-light w-100 btn-prev"><i class="fa-solid fa-fw ${prev}"></i> ${options.prevTitle}</button>
        </td>
        <td></td>
        <td style="width:100px">
            <button class="btn btn-sm btn-link fw-bold text-decoration-none bg-hover-light w-100 btn-next">${options.nextTitle} <i class="fa-solid fa-fw ${next}"></i></button>
        </td>
    </tr>
</table>
            `;
        }
        function getTabsNav() {

            let tabsContentsId = options["tabsContentsId"];
            let navItems = '';

            let active = "active";
            $('#' + tabsContentsId + ' .tab-pane').each(function () {
                let tId = $(this).attr("id");
                let tTitle = $(this).attr("data-ae-tab-title");
                let tIcon = $(this).attr("data-ae-tab-icon");
                navItems += genNavItem(tId, tTitle, tIcon, active);
                active = "";
            });

            return `
<ul class="nav ${options["navStyle"]}" id="tabsNavbar_${tabsContentsId}">
    ${navItems}
</ul>
            `;
        }
        function genNavItem(id, title, icon, active) {

            let ico = icon === null || icon === undefined || icon === '' ? '' : `<i class="fa-solid fa-fw ${icon}"></i>`;

            return `
    <li class="nav-item text-nowrap">
        <button class="nav-link ${active}" id="navItemBtn_${id}" data-bs-target="#${id}" data-bs-toggle="pill">${ico} ${title}</button>
    </li>
            `;
        }
    };
}(jQuery));
