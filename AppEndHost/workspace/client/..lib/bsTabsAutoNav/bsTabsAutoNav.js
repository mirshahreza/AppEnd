(function ($) {
    $.fn.bsTabsAutoNav = function (options) {
        let _this = $(this);
        $(document).ready(function () { setTimeout(function () { initWidget(); }, 250); });
        function initWidget() {
            options = options || {};
            options = _.defaults(options, { mode: "nav-items", nextTitle: "Next", prevTitle: "Previous", justAllowByBackNext: false });
            if (options["tabsContentsId"] === null || options["tabsContentsId"] === undefined || options["tabsContentsId"] === '') return;
            if (translate) {
                options.prevTitle = translate(options.prevTitle);
                options.nextTitle = translate(options.nextTitle);
            }
            if (options.mode === 'back-next') {
                _this.html(getBackNext());
                _this.find(".btn-prev").off("click").on("click", function () { activate(-1); });
                _this.find(".btn-next").off("click").on("click", function () { activate(1); });
                setBackNextBarState();
            } else {
                _this.html(getTabsNav());
                _this.find(".nav-link").off("click").on("click", function () { setBackNextBarState(); });
            }

            setNavTabsAbility();

        }

        function activate(bn) {
            let tabsContentsId = options["tabsContentsId"];
            let navItemsCount = $("#" + tabsContentsId).find(".tab-pane").length;
            let indActive = getSelectedIndex();            
            if (indActive === -1) return;
            if (bn === -1 && indActive === 0) return;
            if (bn === 1 && indActive === navItemsCount - 1) return;
            let nextId = indActive + bn;
            activateTabById(nextId);
        }

        function activateTabById(toActivateIndex) {
            let tabsContentsId = options["tabsContentsId"];
            let toActivateTab = $("#" + tabsContentsId).find(".tab-pane").eq(toActivateIndex);
            let navBarId = `#tabsNavbar_${tabsContentsId}`;
            let navBar = $(navBarId);

            if (navBar.length > 0 && options.justAllowByBackNext === false) {
                navBar.find(".nav-link").eq(toActivateIndex).click();
            } else {
                navBar.find(".nav-link").removeClass("active");
                navBar.find(".nav-link").eq(toActivateIndex).addClass("active");
                $("#" + tabsContentsId).find(".tab-pane").removeClass("show active");
                toActivateTab.addClass("show active");
            }
            setBackNextBarState();
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

        function getBackNext() {
            let tabsContentsId = options["tabsContentsId"];
            return `
<table class="w-100 text-center" id="bn_${tabsContentsId}">
    <tr>
        <td style="width:100px">
            <button class="btn btn-sm btn-link fw-bold text-decoration-none bg-hover-light w-100 btn-prev"><i class="fa-solid fa-fw fa-chevron-left"></i> ${options.prevTitle}</button>
        </td>
        <td></td>
        <td style="width:100px">
            <button class="btn btn-sm btn-link fw-bold text-decoration-none bg-hover-light w-100 btn-next">${options.nextTitle} <i class="fa-solid fa-fw fa-chevron-right"></i></button>
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
<ul class="nav nav-underline nav-justified" id="tabsNavbar_${tabsContentsId}">
    ${navItems}
</ul>
            `;
        }
        function genNavItem(id, title, icon, active) {

            let ico = icon === null || icon === undefined || icon === '' ? '' : `<i class="fa-solid fa-fw ${icon}"></i>`;

            return `
    <li class="nav-item">
        <button class="nav-link ${active}" id="navItemBtn_${id}" data-bs-target="#${id}" data-bs-toggle="pill">${ico} ${title}</button>
    </li>
            `;
        }
    };
}(jQuery));
