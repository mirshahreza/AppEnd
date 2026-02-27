(function ($) {
    $.fn.objectPicker = function (options) {
        let _this = $(this);

        initOptions();
        initWidget();

        function initOptions() {
            options = options || {};
        }

        function initWidget() {
            if (!_this.hasClass("disabled")) {
                _this.find(".ae-objectpicker-clear").off("click").on("click", function () {
                    _this.find("input").each(function () {
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
