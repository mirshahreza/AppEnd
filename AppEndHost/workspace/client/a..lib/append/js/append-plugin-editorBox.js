(function ($) {
    $.fn.editorBox = function (options) {
        let _this = $(this);
        initWidget();
        function initWidget() {
            options = options || {};
            let retTo = _this.parent().find("input");
            options = _.defaults(options, { mode: "ace/mode/csharp" });
            options["value"] = retTo.val() === null || retTo.val() === undefined || retTo.val() === '' ? '' : retTo.val();
            
            // Configure Ace Editor to load additional files from the correct path
            ace.config.set('basePath', '/a..lib/ace/src-min');
            ace.config.set('modePath', '/a..lib/ace/src-min');
            ace.config.set('themePath', '/a..lib/ace/src-min');
            ace.config.set('workerPath', '/a..lib/ace/src-min');
            
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
