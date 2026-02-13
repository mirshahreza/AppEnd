(function ($) {
    $.fn.csvToTable = function (csvString, tableOptions) {
        let _this = $(this);

        if (!csvString || csvString.trim() === '') {
            _this.html('<div class="p-3 text-muted">No results</div>');
            return _this;
        }

        let options = $.extend({
            tableClass: 'table table-striped table-bordered table-hover table-sm mb-3',
            headerClass: 'table-light',
            cellPadding: 'px-2 py-1',
            delimiter: ',',
            lineBreak: '\r\n',
            showNull: true,
            nullText: '<span class="text-muted fst-italic">NULL</span>'
        }, tableOptions);

        let lines = csvString.trim().split(options.lineBreak);
        if (lines.length === 0) {
            _this.html('<div class="p-3 text-muted">No results</div>');
            return _this;
        }

        let html = `<div class="p-3"><table class="${options.tableClass}" style="width: auto;">`;

        // First line is header
        let headers = lines[0].split(options.delimiter);
        html += `<thead class="${options.headerClass}"><tr>`;
        headers.forEach(header => {
            html += `<th class="${options.cellPadding}">${escapeHtml(header.trim())}</th>`;
        });
        html += '</tr></thead>';

        // Rest are data rows
        html += '<tbody>';
        for (let i = 1; i < lines.length; i++) {
            if (lines[i].trim() === '') continue;

            let cells = lines[i].split(options.delimiter);
            html += '<tr>';
            cells.forEach(cell => {
                let value = cell.trim();
                if (options.showNull && (value === '' || value === 'null' || value === 'NULL')) {
                    value = options.nullText;
                } else {
                    value = escapeHtml(value);
                }
                html += `<td class="${options.cellPadding}">${value}</td>`;
            });
            html += '</tr>';
        }
        html += '</tbody>';
        html += '</table></div>';

        _this.html(html);
        return _this;

        function escapeHtml(text) {
            let map = {
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#039;'
            };
            return text.replace(/[&<>"']/g, function (m) { return map[m]; });
        }
    };
}(jQuery));
