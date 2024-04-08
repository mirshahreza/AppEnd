(function ($) {
    $.fn.bsTabsAutoNav = function (options) {
        let _this = $(this);
        $(document).ready(function () { setTimeout(function () { initWidget(); }, 250); });
        function initWidget() {
            options = options || {};
            //options = _.defaults(options, { theme: "ace/theme/cloud9_day", mode: "ace/mode/csharp" });

            if (options["tabsContentsId"] === null || options["tabsContentsId"] === undefined || options["tabsContentsId"] === '') return;

            let nav = getTabsNav();

            _this.html(nav);

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
