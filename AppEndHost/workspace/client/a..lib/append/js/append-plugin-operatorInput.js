(function ($) {
    $.fn.operatorInput = function (options) {
        let _this = $(this);

        if (_this.attr("data-ae-operator-inited") === "true") return;

        initOptions();
        initWidget();
        _this.attr("data-ae-operator-inited", "true");

        function initOptions() {
            options = options || {};
            options.dbType = fixNullOrEmpty(options.dbType, 'NVARCHAR');
            options.operators = shared.getOperatorsForDbType(options.dbType);
            options.fieldName = _this.attr('id').replace('input_', '');
            
            if (options.operators.length > 0) {
                options.defaultOperator = options.operators[0].operator;
            }
        }

        function initWidget() {
            if (options.operators.length === 0) return;

            let hiddenInputId = _this.attr('id') + '_Operator';
            let hiddenInput = $('#' + hiddenInputId);
            let dropdownBtn = _this.siblings('.operator-btn');
            let dropdownMenu = _this.siblings('.operator-menu');
            
            if (dropdownBtn.length === 0 || dropdownMenu.length === 0) {
                console.error('operatorInput: Required DOM elements not found. Button and dropdown menu must exist in markup.');
                return;
            }

            if (hiddenInput.length > 0 && !hiddenInput.val()) {
                hiddenInput.val(options.defaultOperator);
            }

            let defaultIcon = getOperatorIcon(options.defaultOperator);
            dropdownBtn.find('i').attr('class', `fa-solid fa-fw ${defaultIcon}`);

            dropdownMenu.find('.dropdown-item').each(function() {
                let $item = $(this);
                let operator = $item.attr('data-operator');
                
                if (operator === options.defaultOperator) {
                    $item.find('.fa-check').removeClass('invisible').addClass('text-success');
                } else {
                    $item.find('.fa-check').addClass('invisible');
                }
                
                $item.on('click', function() {
                    let selectedOperator = $(this).attr('data-operator');
                    setOperator(selectedOperator);
                });
            });
        }

        function setOperator(operator) {
            let hiddenInputId = _this.attr('id') + '_Operator';
            let hiddenInput = $('#' + hiddenInputId);
            hiddenInput.val(operator);
            
            hiddenInput.get(0).dispatchEvent(new Event('input', { bubbles: true }));

            let newIcon = getOperatorIcon(operator);
            _this.siblings('.operator-btn').find('i').attr('class', `fa-solid fa-fw ${newIcon}`);

            _this.siblings('.operator-menu').find('.fa-check').addClass('invisible').removeClass('text-success');
            _this.siblings('.operator-menu').find(`[data-operator="${operator}"]`).find('.fa-check').removeClass('invisible').addClass('text-success');
        }

        function getOperatorIcon(operator) {
            let op = options.operators.find(o => o.operator === operator);
            return op ? op.icon : 'fa-filter';
        }

        function fixNullOrEmpty(v1, v2) {
            if (v1 === undefined || v1 === null || v1 === '') return v2;
            return v1;
        }
    };
}(jQuery));
