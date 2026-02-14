(function ($) {
    $.fn.dtPicker = function (options) {
        let _this = $(this);

        if (_this.attr("data-ae-inited") !== "true") {
            initOptions();
            initWidget();
            _this.attr("data-ae-inited", "true");
        }

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
