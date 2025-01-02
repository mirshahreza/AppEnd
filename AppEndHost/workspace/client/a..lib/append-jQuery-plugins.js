(function ($) {
    $.fn.aeFileField = function (options) {
        let _this = $(this);
        initWidget(_this);
        setTimeout(function () {
            setFile(options.valInput_FileBody.value, options.valInput_FileName.value, options.valInput_Size.value, options.valInput_MimeType.value, false);
        }, 100);

        function initWidget(elm) {
            elm.css("position", 'relative');
            options = options || {};
            options = _.defaults(options, {
                readonly: false,
                accept: '*',
                maxSize: (3 * 1024 * 1024),
                resize: false,
                resizeMaxWidth: 1200,
                resizeMaxHeight: 1200,
                emptyIcon: "fa-plus"
            });
            options.isEmpty = true;
            options.dataType = (options.accept.indexOf('image/') > -1 ? "data:image/png;base64" : "data:application/octet-stream;base64");
            options.inputFile = $('<input type="file" accept="' + options.accept + '" style="visibility:hidden;display:none;" />');
            options.clickArea = $('<div class="w-100 h-100 click-area" style="background-size:contain;background-position:center center;background-repeat:no-repeat;">&nbsp;</div>');
            options.clearButton = $('<span class="btn btn-sm btn-light p-1 py-0 pointer" style="position:absolute;top:7px;left:4px;"><i class="fas fa-times hover-danger"></i></span>');
            options.downloadButton = $('<span class="btn btn-sm btn-light p-1 py-0 pointer" style="position:absolute;top:7px;left:26px;"><i class="fas fa-download hover-primary"></i></span>');
            options.editButton = $('<span class="btn btn-sm btn-light p-1 py-0 pointer" style="position:absolute;top:7px;left:52px;"><i class="fas fa-edit hover-primary"></i></span>');

            options.valInput_FileBody = elm.find("input.FileBody").get(0);
            options.valInput_FileName = elm.find("input.FileName").get(0);
            options.valInput_Size = elm.find("input.FileSize").get(0);
            options.valInput_MimeType = elm.find("input.FileMime").get(0);


            let previousValue = options.valInput_FileBody.value;

            setInterval(function () {
                if (options.valInput_FileBody.value !== previousValue) {
                    previousValue = options.valInput_FileBody.value;
                    setFile(options.valInput_FileBody.value, options.valInput_FileName.value, options.valInput_Size.value, options.valInput_MimeType.value, false);
                }
            }, 500);

            elm.append(options.inputFile);
            elm.append(options.clickArea);
            elm.append(options.clearButton);
            elm.append(options.downloadButton);
            elm.append(options.editButton);

            options.clickArea.parent().css("background-size", 'contain').css("background-position", "center center").css("background-repeat", "no-repeat").css("cursor", "pointer");

            elm.mouseover(function () {
                if (!options.isEmpty && !options.readonly) {
                    options.clearButton.show();
                    options.downloadButton.show();
                    if (options.dataType === "data:image/png;base64") options.editButton.show();
                }
            }).mouseout(function () {
                options.clearButton.hide();
                options.downloadButton.hide();
                options.editButton.hide();
            });

            options.clearButton.on("click", function () {
                setFile(null, null, null, null, true);
            });

            options.inputFile.on("change", function () {
                let thisInput = this;
                if (thisInput.files.length === 0) return;
                if (thisInput.files[0].size > options.maxSize) {
                    if (options.resize !== true || options.dataType !== "data:image/png;base64") {
                        alert("Max size :" + " : " + (parseInt(options.maxSize) / 1024) + "KB");
                        return;
                    }
                }

                let fileReader = new FileReader();
                fileReader.onload = function () {
                    let fileBody = getB64Str(fileReader.result);
                    let fileName = thisInput.files[0].name;
                    let fileSize = thisInput.files[0].size;
                    let fileMime = thisInput.files[0].type;

                    if (options.resize === true && options.dataType === "data:image/png;base64") {
                        resizebase64(fileName, fileBody, options.resizeMaxWidth, options.resizeMaxHeight, function (resized) {
                            fileBody = resized;
                            fileSize = fileBody.length;
                            setFile(fileBody, fileName, fileSize, fileMime, true);
                            options.inputFile.val('');
                        });
                    } else {
                        setFile(fileBody, fileName, fileSize, fileMime, true);
                        options.inputFile.val('');
                    }
                }
                fileReader.readAsArrayBuffer(thisInput.files[0]);
            });

            // set initial
        }
        function setFile(FileBody, FileName, FileSize, FileMime, notifyModel) {
            options.clearButton.hide();
            options.downloadButton.hide();
            options.editButton.hide();
            options.clickArea.off("click").html("&nbsp;");
            options.clickArea.parent().off("click").css("background-image", '');
            if (fixV(FileBody, null) === null) {
                options.isEmpty = true;
                options.clickArea.append(getHtmlIcon(options.emptyIcon));

                //handle click for open file
                options.clickArea.on("click", function (e) {
                    options.inputFile.click();
                    e.stopPropagation();
                });
            } else {
                options.isEmpty = false;

                if (isImageFromNameFromType(FileMime)) {
                    options.clickArea.parent().css("background-image", 'url("' + options.dataType + "," + FileBody + '")');
                } else {
                    options.clickArea.append(getHtmlIcon(getIconFromName(FileName)));
                }

                // handle click for download the file
                options.downloadButton.off("click").on("click", function () {
                    var a = document.createElement("a");
                    a.href = "data:application/octet-stream;base64, " + FileBody;
                    a.download = FileName;
                    a.click();
                });

                options.editButton.off("click").on("click", function () {
                    openComponent("/a.SharedComponents/ImageEditor", {
                        "modalSize": "modal-fullscreen",
                        "title": "Image Editor",
                        resizable: false,
                        draggable: false,
                        params: {
                            image: FileBody,
                            callback: function (ret) {
                                if (ret.rr === true) setFile(ret.rv, FileName, FileSize, FileMime, true);
                            }
                        }
                    });
                });
            }

            if (notifyModel === true) {
                let event = new Event('input', { bubbles: true });

                options.valInput_FileBody.value = FileBody;
                options.valInput_FileBody.dispatchEvent(event);

                options.valInput_FileName.value = FileName;
                options.valInput_FileName.dispatchEvent(event);

                options.valInput_Size.value = FileSize;
                options.valInput_Size.dispatchEvent(event);

                options.valInput_MimeType.value = FileMime;
                options.valInput_MimeType.dispatchEvent(event);

                $(options.valInput_FileBody).keyup();

            }
        }

        function getHtmlIcon(ico) {
            return '<i class="fas ' + ico + ' fa-2x vertical-center"></i>';
        }

    };
}(jQuery));

(function ($) {
    $.fn.bsPagination = function (options) {

        var renderPlace = this;
        initOptions();
        initWidget();

        function initOptions() {
            options = options || {};
            options["next-text"] = fixNullOrUndefined(options["next-text"], 'Next');
            options["previous-text"] = fixNullOrUndefined(options["previous-text"], 'Previous');
            options["pages"] = fixNullOrUndefined(options["pages"], 0);
            options["page"] = fixNullOrUndefined(options["page"], 1);
            options["css"] = fixNullOrUndefined(options["css"], 'pagination-sm m-0');
        }

        function initWidget() {
            renderPagination(options["pages"], options["page"]);
        }

        function renderPagination(pages, page) {
            let str = '<nav class="Page navigation p-0 m-0"><ul class="pagination ' + options["css"] + '">';
            let active;
            let pageCutLow = page - 1;
            let pageCutHigh = page + 1;
            // Show the Previous button only if you are on a page other than the first
            if (page > 1) {
                str += '<li class="page-item"><a class="page-link page-previous pointer p-0 px-2 border-0">' + options["previous-text"] + '</a></li>';
            } else {
                str += '<li class="page-item disabled"><a class="page-link p-0 px-2 border-0">' + options["previous-text"] + '</a></li>';
            }

            // Show all the pagination elements if there are less than 6 pages total
            if (pages < 6) {
                for (let p = 1; p <= pages; p++) {
                    active = page == p ? "active" : "";
                    str += '<li class="page-item ' + active + '"><a class="page-link page-p pointer p-0 px-2 border-0">' + p + '</a></li>';
                }
            }
            // Use "..." to collapse pages outside of a certain range
            else {
                // Show the very first page followed by a "..." at the beginning of the
                // pagination section (after the Previous button)
                if (page > 2) {
                    str += '<li class="page-item"><a class="page-link pointer page-p p-0 px-2 border-0">1</a></li>';
                    if (page > 3) {
                        str += '<li class="page-item"><a class="page-link pointer page-afterprevios p-0 px-2 border-0">...</a></li>';
                    }
                }
                // Determine how many pages to show after the current page index
                if (page === 1) {
                    pageCutHigh += 2;
                } else if (page === 2) {
                    pageCutHigh += 1;
                }
                // Determine how many pages to show before the current page index
                if (page === pages) {
                    pageCutLow -= 2;
                } else if (page === pages - 1) {
                    pageCutLow -= 1;
                }
                // Output the indexes for pages that fall inside the range of pageCutLow
                // and pageCutHigh
                for (let p = pageCutLow; p <= pageCutHigh; p++) {
                    if (p === 0) {
                        p += 1;
                    }
                    if (p > pages) {
                        continue
                    }
                    active = page == p ? "active" : "no";
                    str += '<li class="page-item ' + active + '"><a class="page-link pointer page-p p-0 px-2 border-0">' + p + '</a></li>';
                }
                // Show the very last page preceded by a "..." at the end of the pagination
                // section (before the Next button)
                if (page < pages - 1) {
                    if (page < pages - 2) {
                        str += '<li class="page-item"><a class="page-link pointer page-beforenext p-0 px-2 border-0">...</a></li>';
                    }
                    str += '<li class="page-item"><a class="page-link page-p pointer p-0 px-2 border-0">' + pages + '</a></li>';
                }
            }
            if (page < pages) {
                str += '<li class="page-item"><a class="page-link page-next pointer p-0 px-2 border-0">' + options["next-text"] + '</a></li>';
            } else {
                str += '<li class="page-item disabled"><a class="page-link p-0 px-2 border-0">' + options["next-text"] + '</a></li>';
            }

            str += '</ul></nav>';

            renderPlace.html(str);

            renderPlace.find("a.page-p").off("click").on("click", function () {
                let p = parseInt($(this).text());
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

            renderPlace.find("a.page-previous").off("click").on("click", function () {
                let p = page - 1;
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

            renderPlace.find("a.page-next").off("click").on("click", function () {
                let p = page + 1;
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

            renderPlace.find("a.page-afterprevios").off("click").on("click", function () {
                let p = page - 2;
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

            renderPlace.find("a.page-beforenext").off("click").on("click", function () {
                let p = page + 2;
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

        }

        function fixNullOrUndefined(v1, v2) {
            if (v1 === undefined || v1 === null) return v2;
            return v1;
        }

    }
}(jQuery));

(function ($) {
    $.fn.bsTabsAutoNav = function (options) {
        let _this = $(this);
        $(document).ready(function () { setTimeout(function () { initWidget(); }, 250); });
        function initWidget() {
            options = options || {};
            options = _.defaults(options, { mode: "nav-items", nextTitle: "Next", prevTitle: "Previous", justAllowByBackNext: false, dir: 'ltr', navStyle:"nav-underline nav-justified" });
            if (options["tabsContentsId"] === null || options["tabsContentsId"] === undefined || options["tabsContentsId"] === '') return;

            if (options["mode"] !== "back-next") _this.addClass("d-none d-sm-block");

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
                _this.parent().append(`<div class="mobile-title text-center d-block d-sm-none fw-bold p-2 fs-1d1"></div>`)
                //_this.parent().append(`<div class="mobile-title text-center fw-bold p-2 fs-1d1"></div>`)
                _this.find(".nav-link").off("click").on("click", function () { setBackNextBarState(); });
            }

            setNavTabsAbility();
            activateTabByIndex(0);
        }

        function activate(bn) {
            let tabsContentsId = options["tabsContentsId"];

            let regulator = $("#" + tabsContentsId).find(".tab-pane.active").inputsRegulator();
            if (!regulator.isValid()) return;

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

            if (navBar.length > 0 && options.justAllowByBackNext === false) {
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

(function ($) {
    $.fn.dtPicker = function (options) {
        let _this = $(this);
        initOptions();
        initWidget();

        function initOptions() {
            options = options || {};

            if (options.isGregorian === null || options.isGregorian === undefined) {
                if (getAppConfig()["calendar"] === "Gregorian") {
                    options.isGregorian = true;
                } else {
                    options.isGregorian = false;
                }
            }

            if ($(options.targetDateSelector).val().trim() !== "") {
                options.selectedDate = new Date($(options.targetDateSelector).val());
            }
        }

        function initWidget() {
            $(document).ready(function () {
                setTimeout(function () {
                    let elmDTP = document.getElementById(_this.attr("id"));
                    if (_this.attr("disabled") === "disabled") options.disabled = "true";
                    options.onDayClick = function (e) {
                        setTimeout(function () {

                            let obj = $(options.targetDateSelector);
                            obj.get(0).dispatchEvent(new Event('input', { bubbles: true }));
                            obj.get(0).dispatchEvent(new KeyboardEvent('keyup', { 'key': '' }));


                        }, 100);
                    };
                    let dtComp = new mds.MdsPersianDateTimePicker(elmDTP, options);


                    //$(options.targetDateSelector).off("change").on("change", function () {
                    //    let obj = $(this);
                    //    console.log(obj.val());
                    //    obj.get(0).dispatchEvent(new Event('input', { bubbles: true }));
                    //    obj.get(0).dispatchEvent(new KeyboardEvent('keyup', { 'key': '' }));
                    //});
                }, 100);
            });
        }
    }
}(jQuery));

(function ($) {
    $.fn.editorBox = function (options) {
        let _this = $(this);
        $(document).ready(function () { setTimeout(function () { initWidget(); }, 250); });
        function initWidget() {
            options = options || {};
            let retTo = _this.parent().find("input");
            options = _.defaults(options, { mode: "ace/mode/csharp" });
            options["value"] = retTo.val() === null || retTo.val() === undefined || retTo.val() === '' ? '' : retTo.val();
            shared.editors[_this.attr("id")] = ace.edit(_this.attr("id"), options);
            if (retTo.attr("disabled")) shared.editors[_this.attr("id")].setReadOnly(true);
            shared.editors[_this.attr("id")].getSession().on('change', function () {
                retTo.val(shared.editors[_this.attr("id")].getValue().trim());
                retTo.get(0).dispatchEvent(new Event('input', { bubbles: true }));
                retTo.get(0).dispatchEvent(new KeyboardEvent('keyup', { 'key': '' }));
            });
        }
    };
}(jQuery));

(function ($) {
    $.fn.inputsRegulator = function (options) {
        let _this = $(this);
        let isFirstTime = true;
        let invalidItems = [];
        initWidget();
        var output = {
            validateArea: function () { validateArea(); },
            isValid: function () { validateArea(); return invalidItems.length === 0; },
            getInvalidItems: function () { validateArea(); return invalidItems; }
        };
        return output;
        function initWidget() {
            options = options || {};
            options = _.defaults(options, { onStart: true, invalidClass: "is-invalid" });
            if (options.onStart === true) validateArea();
            attachOnChangeToInputs();
            isFirstTime = false;
        }
        function validateArea() {
            //console.log("validateArea");
            let flag = true;
            invalidItems = [];
            _this.find(`[data-ae-validation-required]`).each(function () {
                let inputO = $(this);
                inputO.attr("data-ae-validation-required", inputIsRequired(inputO).toString().toLowerCase());
                let vRes = validateInput(inputO);
                if (vRes === false) {
                    invalidItems.push(inputO.attr("id"));
                    flag = false;
                }
            });
            _this.attr("data-ae-validation-flag", flag.toString().toLowerCase());
        }
        function attachOnChangeToInputs() {
            _this.find("[data-ae-validation-required]").each(function () {
                let inputO = $(this);
                $(this).off("keypress").on("keypress", function (e) {
                    let r = inputO.attr("data-ae-validation-rule");
                    if (r !== undefined && r !== null && r.startsWith(":=i") && !isNumberString(e.key)) e.preventDefault();
                });

                inputO.off("keyup").on("keyup", function (e) {
                    validateInput(inputO);
                    setAreaValidationState();
                });
                inputO.off("change").on("change", function (e) {
                    validateInput(inputO);
                    setAreaValidationState();
                });
            });
        }
        function setAreaValidationState() {
            let n = _this.find('[data-ae-isvalid="0"]').length;
            if (n === 0) _this.attr("data-ae-validation-flag", "true");
            else _this.attr("data-ae-validation-flag", "false");
        }
        function validateInput(inputO) {
            let vRes = inputIsValid(inputO);
            setInputUiView(inputO, vRes);
            return vRes;
        }
        function setInputUiView(inputO, validationState) {
            let tagName = inputO.get(0).tagName.toLowerCase();
            if (tagName === 'input' || tagName === 'textarea' || tagName === 'select') {
                if (validationState === true) {
                    inputO.parents(".data-ae-validation").removeClass("border-danger");
                    inputO.removeClass(options.invalidClass);
                    inputO.attr("data-ae-isvalid", "1");
                }
                else {
                    inputO.parents(".data-ae-validation").addClass("border-danger");
                    inputO.addClass(options.invalidClass);
                    setupShaking(inputO.parents(".data-ae-validation"));
                    setupShaking(inputO);
                    inputO.attr("data-ae-isvalid", "0");
                };
            } else {
                if (inputO.hasClass("data-ae-filearea")) {
                    if (validationState === true) {
                        inputO.removeClass("bg-danger-subtle");
                        inputO.attr("data-ae-isvalid", "1");
                    }
                    else {
                        inputO.addClass("bg-danger-subtle")
                        setupShaking(inputO.parents(".data-ae-validation"));
                        setupShaking(inputO);
                        inputO.attr("data-ae-isvalid", "0");
                    };
                } else {
                    if (validationState === true) {
                        inputO.removeClass("border-danger");
                        inputO.attr("data-ae-isvalid", "1");
                    }
                    else {
                        inputO.addClass("border-danger");
                        setupShaking(inputO.parents(".data-ae-validation"));
                        setupShaking(inputO);
                        inputO.attr("data-ae-isvalid", "0");
                    };
                }
            }
        }
        function setupShaking(elm) {
            elm.addClass("animate__animated animate__headShake").one("webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend",
                function () {
                    $(this).removeClass("animate__animated animate__headShake");
                }
            );
        }
        function inputIsValid(inputO) {
            let _v = v(inputO);
            if (isDisabled(inputO)) return true;
            let _isRequired = inputIsRequired(inputO);
            if (_isRequired === true && !inputHasValue(_v)) return false;
            let regRule = inputO.attr("data-ae-validation-rule");
            if (regRule === undefined || regRule === null || regRule === '') regRule = ":=s(1)";
            if (_isRequired === false && (_v === undefined || _v === null || _v === '')) return true;

            if (regRule.startsWith(":=")) {
                let rrMin = null;
                let rrMax = null;
                let rr = regRule.substring(4, regRule.length - 1).split(",");
                if (regRule.startsWith(":=i(")) { // is a number range
                    if (!isStrInt(_v)) return false;
                    if (!isStrInt(rr[0])) return false;
                    rrMin = parseInt(rr[0]);
                    if (rr.length > 1) {
                        if (!isStrInt(rr[1])) return false;
                        rrMax = parseInt(rr[1]);
                    }
                    let vv = parseInt(_v);
                    if (vv < rrMin) return false;
                    if (rrMax !== null && vv > rrMax) return false;
                    return true;
                }
                else if (regRule.startsWith(":=f(")) { // is a float
                    if (!isStrFloat(_v)) return false;
                    if (!isStrFloat(rr[0])) return false;
                    rrMin = parseFloat(rr[0]);
                    if (rr.length > 1) {
                        if (!isStrFloat(rr[1])) return false;
                        rrMax = parseFloat(rr[1]);
                    }
                    let vv = parseFloat(_v);
                    if (vv < rrMin) return false;
                    if (rrMax !== null && vv > rrMax) return false;
                    return true;
                }
                else if (regRule.startsWith(":=d(")) { // is a date
                    if (!isStrDate(_v)) return false;
                    if (!isStrDate(rr[0])) return false;
                    rrMin = parseDate(rr[0]);
                    if (rr.length > 1) {
                        if (!isStrDate(rr[1])) return false;
                        rrMax = parseDate(rr[1]);
                    }

                    _v = normalizeDate(_v);
                    let vv = parseDate(_v);

                    if (vv < rrMin) return false;
                    if (rrMax !== null && vv > rrMax) return false;
                    return true;
                }
                else if (regRule.startsWith(":=dt(")) { // is a datetime
                    if (!isStrDateTime(_v)) return false;
                    if (!isStrDateTime(rr[0])) return false;
                    rrMin = parseDate(rr[0]);
                    if (rr.length > 1) {
                        if (!isStrDate(rr[1])) return false;
                        rrMax = parseDate(rr[1]);
                    }

                    _v = normalizeDate(_v);
                    let vv = parseDate(_v);

                    if (vv < rrMin) return false;
                    if (rrMax !== null && vv > rrMax) return false;

                    //inputO.val(moment(inputO.val(), 'YYYY-MM-DD HH:mm:ss.SSS', true));

                    return true;
                }
                else if (regRule.startsWith(":=s(")) { // is a string
                    if (!isStrInt(rr[0])) return false;
                    rrMin = parseInt(rr[0]);
                    if (rr.length > 1) {
                        if (!isStrInt(rr[1])) return false;
                        rrMax = parseInt(rr[1]);
                    }
                    let vv = _v.length;
                    if (vv < rrMin) return false;
                    if (rrMax !== null && vv > rrMax) return false;
                    return true;
                }
                else if (regRule.startsWith(":=n(")) { // is a checkboxlist
                    rrMin = parseInt(rr[0]);
                    if (rr.length > 1) rrMax = parseInt(rr[1]);
                    let vv = _v;
                    if (vv < rrMin) return false;
                    if (rrMax !== null && vv > rrMax) return false;
                    return true;
                }
                else {
                    return false;
                }
            } else {
                return isRegMatchedOnce(_v, regRule);
            }
        }
        function isDisabled(inputO) {
            let tagName = inputO.get(0).tagName.toLowerCase();
            if (tagName === 'input' || tagName === 'textarea' || tagName === 'select') {
                return inputO.prop("disabled");
            } else {
                if (inputO.hasClass("data-ae-filearea")) {
                    return inputO.find(".ae-file-field:first").prop("disabled");
                } else {
                    return inputO.find("input:first").prop("disabled");
                }
            }
        }
        function inputHasValue(v) {
            if (v === undefined || v === null || v === "") return false;
            if (v.length > 0 || v > -1) return true;
            return false;
        }
        function inputIsRequired(inputO) {
            let r = inputO.attr("data-ae-validation-required");
            if (r === "true") return true;
            return false;
        }
        function v(inputO) {
            let tagName = inputO.get(0).tagName.toLowerCase();
            if (tagName === 'input' || tagName === 'textarea' || tagName === 'select') {
                return inputO.val();
            } else {
                let _v = '';

                if (inputO.hasClass("data-ae-filearea")) {
                    let fileItems = inputO.find(".ae-file-field");
                    return fileItems.length;
                } else {
                    let checkItems = inputO.find("input");
                    if (checkItems.length === 1) {
                        checkItems.each(function () {
                            if ($(this).prop("checked").toString().toLowerCase() === "true") _v = $(this).val();
                        });
                    } else {
                        _v = 0;
                        checkItems.each(function () {
                            if ($(this).prop("checked").toString().toLowerCase() === "true") _v = _v + 1;
                        });
                    }
                    return _v;
                }
            }
        }
        function isAreaValid() {
            if (_this.attr("data-ae-validation-flag") === 'true') return true; else return false;
        }
        function isRegMatchedOnce(str, regexp) {
            const regex = new RegExp(regexp);
            return regex.test(str);
        }
        function isStrInt(n) {
            return !isNaN(parseInt(n)) && isFinite(n);
        }
        function isStrFloat(n) {
            return !isNaN(parseFloat(n)) && isFinite(n);
        }
        function isStrDateTime(str) {
            //let m = moment(str, 'YYYY-MM-DD HH:mm:ss.SSS');
            //return m.isValid();
            return true;
        }
        function isStrDate(str) {
            return !isNaN(Date.parse(str));
        }
        function parseDate(str) {
            return new Date(str);
        }
        function normalizeDate(str) {
            let nD = str.split('-');
            return nD[0] + '-' + fix2Char(nD[1] === undefined ? '01' : nD[1]) + '-' + fix2Char(nD[2] === undefined ? '01' : nD[2]);
        }
        function fix2Char(str) {
            if (str.length === 1) return '0' + str;
            return str;
        }
    };
}(jQuery));

(function ($) {
    $.fn.nullableCheckbox = function (options) {
        var el = $(this);
        let data;
        initOptions();
        initWidget();

        function initOptions() {
            options = options || {};
            if (options.shownull === undefined || options.shownull === null) options.shownull = true;
            if (options.nullClasses === undefined || options.nullClasses === null) options.nullClasses = "fa-minus text-secondary";
            if (options.trueClasses === undefined || options.trueClasses === null) options.trueClasses = "fa-check text-success";
            if (options.falseClasses === undefined || options.falseClasses === null) options.falseClasses = "fa-xmark text-danger";
        }

        function initWidget() {
            data = el.find("input:first").val();
            setVisualState(data);
            if (!el.hasClass("disabled")) {
                el.off("click").on("click", function () {
                    setVisualState(nextState(data));
                });
            }
        }

        function setVisualState(dIn) {
            let d = toStateStr(dIn);
            let chkEl = el.find("i:first");
            //chkEl.removeClass("fa-check").removeClass("fa-xmark").removeClass("fa-minus").removeClass("text-success").removeClass("text-danger");

            chkEl.removeClass("fa-check").removeClass(options.nullClasses).removeClass(options.trueClasses).removeClass(options.falseClasses);

            if (d === 'true') {
                //chkEl.addClass("fa-check").addClass("text-success");
                chkEl.addClass(options.trueClasses);
                data = true;
            } else if (d === 'false') {
                //chkEl.addClass("fa-xmark").addClass("text-danger");
                chkEl.addClass(options.falseClasses);
                data = false;
            } else {
                if (options.shownull === true) {
                    //chkEl.addClass("fa-minus");
                    chkEl.addClass(options.nullClasses);
                    data = null;
                }
                else {
                    //chkEl.addClass("fa-xmark").addClass("text-danger");
                    chkEl.addClass(options.falseClasses);
                    data = false;
                }
            }

            let event = new Event('input', { bubbles: true });
            let hV = el.find("input:first").get(0);
            hV.value = data === '' ? null : data;
            hV.dispatchEvent(event);
            el.find("input:first").keyup();
        }

        function nextState(dIn) {
            let d = toStateStr(dIn);

            if (options.shownull === true) {
                if (d === 'true') return false;
                if (d === 'false') return null;
                if (d === '') return true;
            }
            else {
                if (d === 'true') return false;
                if (d === 'false') return true;
                return false;
            }
        }

        function toStateStr(dIn) {
            if (dIn === null || dIn === undefined) return '';
            return dIn.toString().toLowerCase().trim();
        }

    }
}(jQuery));

(function ($) {
    $.fn.objectPicker = function (options) {
        let el = $(this);
        initOptions();
        initWidget();

        function initOptions() {
            options = options || {};
        }

        function initWidget() {
            if (!el.hasClass("disabled")) {
                el.find(".ae-objectpicker-clear").off("click").on("click", function () {
                    el.find("input").each(function () {
                        let obj = $(this);
                        obj.val("");
                        obj.get(0).dispatchEvent(new Event('input', { bubbles: true }));
                        obj.keyup();
                        setTimeout(function () {
                            obj.val("");
                        }, 100);
                    });
                });
            }
        }
    }
}(jQuery));
