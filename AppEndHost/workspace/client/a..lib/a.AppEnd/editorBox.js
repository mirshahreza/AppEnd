(function ($) {
    $.fn.editorBox = function (options) {
        let _this = $(this);
        $(document).ready(function () { setTimeout(function () { initWidget(); }, 250); });
        function initWidget() {
            options = options || {};
            let retTo = _this.parent().find("input");
            options = _.defaults(options, { theme: "ace/theme/cloud9_day", mode: "ace/mode/csharp" });
            options["value"] = retTo.val() === null || retTo.val() === undefined || retTo.val() === '' ? '' : retTo.val();
            shared.editors[_this.attr("id")] = ace.edit(_this.attr("id"), options);
            if (retTo.attr("disabled")) shared.editors[_this.attr("id")].setReadOnly(true);
            shared.editors[_this.attr("id")].getSession().on('change', function() {
                retTo.val(shared.editors[_this.attr("id")].getValue().trim());
                retTo.get(0).dispatchEvent(new Event('input', { bubbles: true }));
                retTo.get(0).dispatchEvent(new KeyboardEvent('keyup', { 'key': '' }));
            });
        }
    };
}(jQuery));
