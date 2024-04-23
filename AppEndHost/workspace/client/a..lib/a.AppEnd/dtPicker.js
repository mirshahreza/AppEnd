
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
            let a = new mds.MdsPersianDateTimePicker(document.getElementById(_this.attr("id")), options);

            $(options.targetDateSelector).off("change").on("change", function () {
                let obj = $(this);
                obj.get(0).dispatchEvent(new Event('input', { bubbles: true }));
                obj.get(0).dispatchEvent(new KeyboardEvent('keyup', { 'key': '' }));
            });
        }
    }
}(jQuery));
