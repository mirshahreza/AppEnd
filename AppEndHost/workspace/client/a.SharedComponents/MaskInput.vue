<template>
    <input
        :ref="inputRef"
        type="text"
        class="form-control form-control-sm"
        :class="[inputClass, isLtr ? 'ltr text-start' : '']"
        :id="id"
        :name="name"
        :placeholder="effectivePlaceholder"
        :disabled="disabled"
        :readonly="readonly"
        :autocomplete="autocomplete"
        @input="onInput"
        @blur="onBlur"
        @focus="onFocus"
        :data-ae-validation-required="validationRequired"
        :data-ae-validation-rule="validationRule"
    />
</template>

<script>
    // MaskInput.vue - کامپوننت ماسک ورودی با پشتیبانی از انواع مختلف
    // استفاده: <mask-input v-model="row.Mobile" type="mobile" />
    // وابستگی: jQuery Mask Plugin (jquery.mask.min.js)
    const MASK_PRESETS = {
        mobile: { mask: '0999-999-9999', placeholder: '0912-345-6789', ltr: true },
        landline: { mask: '099-99999999', placeholder: '021-12345678', ltr: true },
        nationalCode: { mask: '0000000000', placeholder: '1234567890', ltr: true },
        date: { mask: '0000/00/00', placeholder: '1403/01/01', ltr: true },
        dateEn: { mask: '00/00/0000', placeholder: '01/01/2024', ltr: true },
        time: { mask: '00:00', placeholder: '12:30', ltr: true },
        timeFull: { mask: '00:00:00', placeholder: '12:30:45', ltr: true },
        dateTime: { mask: '0000/00/00 00:00', placeholder: '1403/01/01 12:30', ltr: true },
        creditCard: { mask: '0000-0000-0000-0000', placeholder: '1234-5678-9012-3456', ltr: true },
        postalCode: { mask: '0000000000', placeholder: '1234567890', ltr: true },
        iban: { mask: 'AA00 0000 0000 0000 0000 0000 00', placeholder: 'IR00 0000 0000 0000 0000 0000 00', ltr: true },
        decimal: { mask: '9999999999.99', placeholder: '1234567.00', ltr: true },
        year: { mask: '0000', placeholder: '1403', ltr: true },
        month: { mask: '00', placeholder: '01', ltr: true },
        day: { mask: '00', placeholder: '01', ltr: true }
    };

    let _this = { cid: "", c: null, inputRef: "maskInput_" + Math.random().toString(36).substr(2, 9), maskInstance: null };
    export default {
        emits: ['update:modelValue', 'blur', 'focus', 'complete'],
        props: {
            modelValue: { type: [String, Number], default: "" },
            type: { type: String, default: "mobile" },
            customMask: { type: String, default: "" },
            customPlaceholder: { type: String, default: "" },
            id: { type: String, default: "" },
            name: { type: String, default: "" },
            placeholder: { type: String, default: "" },
            disabled: { type: Boolean, default: false },
            readonly: { type: Boolean, default: false },
            autocomplete: { type: String, default: "off" },
            validationRequired: { type: [Boolean, String], default: false },
            validationRule: { type: String, default: "" },
            returnRaw: { type: Boolean, default: true },
            inputClass: { type: [String, Object], default: "" }
        },
        setup(props) {
            _this.cid = (props.cid || props.id || _this.inputRef);
            return { inputRef: _this.inputRef };
        },
        data() {
            return _this;
        },
        computed: {
            effectivePlaceholder() {
                if (this.placeholder) return this.placeholder;
                if (this.type === 'custom' && this.customPlaceholder) return this.customPlaceholder;
                const preset = MASK_PRESETS[this.type];
                return preset ? preset.placeholder : "";
            },
            effectiveMask() {
                if (this.type === 'custom' && this.customMask) return this.customMask;
                const preset = MASK_PRESETS[this.type];
                return preset ? preset.mask : "0999-999-9999";
            },
            isLtr() {
                if (this.type === 'custom') return true;
                const preset = MASK_PRESETS[this.type];
                return preset && preset.ltr;
            }
        },
        created() { _this.c = this; },
        mounted() {
            this.$nextTick(() => this.applyMask());
        },
        beforeUnmount() {
            this.removeMask();
        },
        watch: {
            modelValue(nv) {
                this.$nextTick(() => this.syncFromModel(nv));
            },
            type() { this.$nextTick(() => this.reapplyMask()); },
            customMask() { this.$nextTick(() => this.reapplyMask()); },
            disabled() { this.reapplyMask(); }
        },
        methods: {
            applyMask() {
                const el = this.$refs[this.inputRef];
                if (!el || typeof $ === 'undefined' || !$.fn.mask) return;
                const mask = this.effectiveMask;
                this.removeMask();
                $(el).mask(mask, {
                    clearIfNotMatch: false,
                    placeholder: this.effectivePlaceholder,
                    onChange: (val) => this.onMaskChange(val),
                    onComplete: () => this.$emit('complete', this.getCleanValue())
                });
                _this.maskInstance = $(el).data("mask");
                this.syncFromModel(this.modelValue);
            },
            removeMask() {
                const el = this.$refs[this.inputRef];
                if (el && typeof $ !== 'undefined' && $(el).data("mask")) {
                    $(el).unmask();
                    _this.maskInstance = null;
                }
            },
            reapplyMask() {
                this.removeMask();
                this.$nextTick(() => this.applyMask());
            },
            syncFromModel(val) {
                const el = this.$refs[this.inputRef];
                if (!el || typeof $ === 'undefined') return;
                let str = (val !== undefined && val !== null) ? String(val).trim() : "";
                if (this.type === 'iban') str = str.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
                else if (this.type !== 'decimal') str = str.replace(/\D/g, '');
                const maskInst = $(el).data("mask");
                if (maskInst) {
                    try {
                        const masked = $(el).masked(str) || str;
                        $(el).val(masked);
                    } catch (e) { $(el).val(str); }
                } else {
                    $(el).val(str);
                }
            },
            getCleanValue() {
                const el = this.$refs[this.inputRef];
                if (!el || typeof $ === 'undefined') return "";
                const v = $(el).val() || "";
                if (this.type === 'decimal') return v.replace(/[^\d.]/g, '');
                if (this.type === 'iban') return v.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
                const maskInst = $(el).data("mask");
                return (maskInst && maskInst.getCleanVal) ? maskInst.getCleanVal() : v.replace(/\D/g, '');
            },
            getMaskedValue() {
                const el = this.$refs[this.inputRef];
                if (!el || typeof $ === 'undefined') return "";
                return $(el).val() || "";
            },
            onMaskChange(val) {
                const clean = this.getCleanValue();
                const out = this.returnRaw ? clean : (this.getMaskedValue() || "");
                this.$emit('update:modelValue', out);
            },
            onInput(e) {
                const el = e.target;
                if (typeof $ !== 'undefined' && $(el).data("mask")) {
                    this.onMaskChange($(el).val());
                } else {
                    this.$emit('update:modelValue', el.value);
                }
            },
            onBlur(e) {
                this.$emit('blur', e);
            },
            onFocus(e) {
                this.$emit('focus', e);
            }
        }
    };
</script>

<style scoped>
    input.ltr, input[data-ltr="true"] { direction: ltr; text-align: left; }
</style>
