(function ($) {
    $.fn.inputsRegulator = function (options) {
        let _this = $(this);
        let invalidItems = [];

        $(document).ready(function () {
            setTimeout(function () {
                initWidget();
            }, 250);
        });

        var output = {
            validateArea: function () { validateArea(); },
            isValid: function () { validateArea(); return invalidItems.length === 0; },
            getInvalidItems: function () { validateArea(); return invalidItems; }
        };
        return output;
        function initWidget() {
            if (options === undefined || options === null) options = { };
            options = _.defaults(options, { onStart: true, invalidClass: "is-invalid" });
            if (options.onStart === true) validateArea();
            attachOnChangeToInputs();
        }
        function validateArea() {
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
