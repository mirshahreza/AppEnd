
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
