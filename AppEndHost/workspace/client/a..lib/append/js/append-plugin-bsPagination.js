(function ($) {
    $.fn.bsPagination = function (options) {

        var _this = this;

        initOptions();
        initWidget();
        _this.attr("data-ae-inited", "true");

        function initOptions() {
            options = options || {};
            options["next-text"] = fixNullOrUndefined(options["next-text"], 'Next');
            options["previous-text"] = fixNullOrUndefined(options["previous-text"], 'Previous');
            options["pages"] = fixNullOrUndefined(options["pages"], 0);
            options["page"] = fixNullOrUndefined(options["page"], 1);
            options["css"] = fixNullOrUndefined(options["css"], 'pagination-sm m-0');
        }

        function initWidget() {
            renderPagination(options["pages"], options["page"]);
        }

        function renderPagination(pages, page) {
            let str = '<nav class="Page navigation p-0 m-0"><ul class="pagination ' + options["css"] + '">';
            let active;
            let pageCutLow = page - 1;
            let pageCutHigh = page + 1;
            // Show the Previous button only if you are on a page other than the first
            if (page > 1) {
                str += '<li class="page-item"><a class="page-link page-previous pointer p-0 px-2 border-0">' + options["previous-text"] + '</a></li>';
            } else {
                str += '<li class="page-item disabled"><a class="page-link p-0 px-2 border-0">' + options["previous-text"] + '</a></li>';
            }

            // Show all the pagination elements if there are less than 6 pages total
            if (pages < 6) {
                for (let p = 1; p <= pages; p++) {
                    active = page == p ? "active" : "";
                    str += '<li class="page-item ' + active + '"><a class="page-link page-p pointer p-0 px-2 border-0">' + p + '</a></li>';
                }
            }
            // Use "..." to collapse pages outside of a certain range
            else {
                // Show the very first page followed by a "..." at the beginning of the
                // pagination section (after the Previous button)
                if (page > 2) {
                    str += '<li class="page-item"><a class="page-link pointer page-p p-0 px-2 border-0">1</a></li>';
                    if (page > 3) {
                        str += '<li class="page-item"><a class="page-link pointer page-afterprevious p-0 px-2 border-0">...</a></li>';
                    }
                }
                // Determine how many pages to show after the current page index
                if (page === 1) {
                    pageCutHigh += 2;
                } else if (page === 2) {
                    pageCutHigh += 1;
                }
                // Determine how many pages to show before the current page index
                if (page === pages) {
                    pageCutLow -= 2;
                } else if (page === pages - 1) {
                    pageCutLow -= 1;
                }
                // Output the indexes for pages that fall inside the range of pageCutLow
                // and pageCutHigh
                for (let p = pageCutLow; p <= pageCutHigh; p++) {
                    if (p === 0) {
                        p += 1;
                    }
                    if (p > pages) {
                        continue
                    }
                    active = page == p ? "active" : "no";
                    str += '<li class="page-item ' + active + '"><a class="page-link pointer page-p p-0 px-2 border-0">' + p + '</a></li>';
                }
                // Show the very last page preceded by a "..." at the end of the pagination
                // section (before the Next button)
                if (page < pages - 1) {
                    if (page < pages - 2) {
                        str += '<li class="page-item"><a class="page-link pointer page-beforenext p-0 px-2 border-0">...</a></li>';
                    }
                    str += '<li class="page-item"><a class="page-link page-p pointer p-0 px-2 border-0">' + pages + '</a></li>';
                }
            }
            if (page < pages) {
                str += '<li class="page-item"><a class="page-link page-next pointer p-0 px-2 border-0">' + options["next-text"] + '</a></li>';
            } else {
                str += '<li class="page-item disabled"><a class="page-link p-0 px-2 border-0">' + options["next-text"] + '</a></li>';
            }

            str += '</ul></nav>';

            _this.html(str);

            _this.find("a.page-p").off("click").on("click", function () {
                let p = parseInt($(this).text());
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

            _this.find("a.page-previous").off("click").on("click", function () {
                let p = page - 1;
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

            _this.find("a.page-next").off("click").on("click", function () {
                let p = page + 1;
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

            _this.find("a.page-afterprevious").off("click").on("click", function () {
                let p = page - 2;
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

            _this.find("a.page-beforenext").off("click").on("click", function () {
                let p = page + 2;
                if (options.afterPageChanged) options.afterPageChanged(p);
                renderPagination(pages, p);
            });

        }

        function fixNullOrUndefined(v1, v2) {
            if (v1 === undefined || v1 === null) return v2;
            return v1;
        }

    }
}(jQuery));
