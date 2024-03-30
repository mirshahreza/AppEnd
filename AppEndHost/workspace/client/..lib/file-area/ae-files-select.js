(function ($) {
    $.fn.aeFilesSelect = function (options) {
        let elm = $(this);
        let data = [];
        initWidget();

        function initWidget() {
            options = options || {};
            options = _.defaults(options, { maxSize: (3 * 1024 * 1024), accept: '*', resize: false, resizeMaxWidth: 1200, resizeMaxHeight: 1200 });

            let inputFile = '<input type="file" accept="' + options.accept + '" style="visibility:hidden;display:none;" multiple /><button class="btn btn-sm btn-primary">SelectFiles</button>';
            elm.html(inputFile);
            let btnSelectFiles = elm.find('button:first');
            let btnInputFiles = elm.find('input[type="file"]:first');
            btnSelectFiles.off("click").on("click", function () { btnInputFiles.click(); });

            btnInputFiles.on("change", function () {
                let btnInputFilesDOM = this;
                if (btnInputFilesDOM.files.length === 0) return;

                filesLoaded = false;
                let fileArray = [];

                $.each(btnInputFilesDOM.files, function (index, value) {
                    var fileReader = new FileReader();
                    fileReader.onload = function () {
                        if (!isImageFromName(value.name) && value.size > options.maxSize) return;
                        resizebase64(value.name, getB64Str(fileReader.result), options.resizeMaxWidth, options.resizeMaxHeight, function (resized) {
                            let newItem = { id: "", name: value.name, size: resized.length, type: value.type, content: resized, title: '', note: '', vieworder: 1 };
                            fileArray.push(newItem);
                            data.push(newItem);
                            if (fileArray.length === btnInputFilesDOM.files.length) {
                                filesLoaded = true;
                                $(btnInputFilesDOM).val("");
                                if (options.onFilesLoaded) options.onFilesLoaded(fileArray);
                            }
                        });
                    }
                    fileReader.readAsArrayBuffer(value);
                });

            });
        }

    };
}(jQuery));
