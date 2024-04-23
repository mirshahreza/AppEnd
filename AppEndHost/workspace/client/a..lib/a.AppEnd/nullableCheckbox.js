
(function ($) {
    $.fn.nullableCheckbox = function (options) {
        var el = $(this);
        let data;
        initOptions();
        initWidget();

        function initOptions() {
            options = options || {};
            if (options.shownull === undefined || options.shownull === null) options.shownull = true;
            if(options.nullClasses === undefined || options.nullClasses === null) options.nullClasses = "fa-minus text-secondary";
            if(options.trueClasses === undefined || options.trueClasses === null) options.trueClasses = "fa-check text-success";
            if(options.falseClasses === undefined || options.falseClasses === null) options.falseClasses = "fa-xmark text-danger";
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
