(function ($) {
    $.fn.aeFileField = function (options) {
        let _this = $(this);
        initWidget($(this));
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

            elm.append(options.inputFile);
            elm.append(options.clickArea);
            elm.append(options.clearButton);
            elm.append(options.downloadButton);
            elm.append(options.editButton);

            options.clickArea.parent().css("background-size", 'contain').css("background-position", "center center").css("background-repeat", "no-repeat").css("cursor","pointer");

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
            setFile(options.valInput_FileBody.value, options.valInput_FileName.value, options.valInput_Size.value, options.valInput_MimeType.value, false);
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
                    openComponent("/.PublicComponents/imageEditor", {
                        "modalSize": "modal-fullscreen",
                        "title": "Image Editor",
                        resizable: false,
                        draggable:false,
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
