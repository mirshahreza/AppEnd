<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

            <div class="fw-bold fs-d9">
                {{inputs.colProps.Name}} -
                <span class="">
                    {{inputs.colProps.DbType}}<span v-if="shared.fixNull(inputs.colProps.Size,'')!==''">({{inputs.colProps.Size}})</span>
                    <span class="ms-2 fw-light">AllowNull : </span> <span v-if="shared.fixNull(inputs.colProps.AllowNull,'false')==='false'">No</span><span v-else>Yes</span>
                </span>
            </div>

            <hr />

            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12">Developer Note</span>
                <textarea class="form-control form-control-sm" v-model="inputs.colProps.DevNote" rows="2"
                          data-ae-validation-required="false" data-ae-validation-rule=":=s(0,512)"></textarea>
            </div>

            <hr />

            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12">Group</span>
                <input type="text" class="form-control form-control-sm" v-model="inputs.uiProps.Group"
                       data-ae-validation-required="false" data-ae-validation-rule=":=s(0,128)" />
            </div>
            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12">Widget</span>
                <select class="form-select form-select-sm" v-model="inputs.uiProps.UiWidget">
                    <optgroup label="Single line">
                        <option value="Textbox">Textbox</option>
                        <option value="DisabledTextbox">DisabledTextbox</option>
                        <option value="Numberbox">Numberbox</option>
                    </optgroup>
                    <optgroup label="Multi line">
                        <option value="MultilineTextbox">MultilineTextbox</option>
                        <option value="Htmlbox">Htmlbox</option>
                        <option value="CodeEditorbox">CodeEditorbox</option>
                    </optgroup>
                    <optgroup label="Select">
                        <option value="Combo">Combo</option>
                        <option value="Radio">Radio</option>
                        <option value="ObjectPicker">ObjectPicker</option>
                    </optgroup>
                    <optgroup label="Date & Time">
                        <option value="DatePicker">DatePicker</option>
                        <option value="DateTimePicker">DateTimePicker</option>
                        <option value="TimePicker">TimePicker</option>
                    </optgroup>
                    <optgroup label="Binary">
                        <option value="ImageView">ImageView</option>
                        <option value="FileView">FileView</option>
                    </optgroup>
                    <optgroup label="Other">
                        <option value="Checkbox">Checkbox</option>
                        <option value="ColorPicker">ColorPicker</option>
                    </optgroup>
                    <option value="NoWidget">NoWidget</option>
                </select>
            </div>

            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12">Widget Options</span>
                <textarea type="text" class="form-control form-control-sm" rows="6" v-model="inputs.uiProps.UiWidgetOptions"
                          data-ae-validation-required="false" data-ae-validation-rule=":=s(0,1024)"></textarea>
            </div>
            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12">&nbsp;</span>
                <div class="input-group-text border-0 rounded-0 bg-transparent px-0" v-if="inputs.uiProps.UiWidget==='CodeEditorbox'">
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCodeEditor('csharp')">csharp</span>
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCodeEditor('sqlserver')">sqlserver</span>
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCodeEditor('html')">html(vue)</span>
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCodeEditor('javascript')">javascript</span>
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCodeEditor('css')">css</span>
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCodeEditor('json')">json</span>
                </div>
                <div class="input-group-text border-0 rounded-0 bg-transparent px-0" v-if="inputs.uiProps.UiWidget==='Htmlbox'">
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForHtmlbox('')">NoOption</span>
                </div>
                <div class="input-group-text border-0 rounded-0 bg-transparent px-0" v-if="inputs.uiProps.UiWidget==='Checkbox'">
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCheckbox('NoOption')">NoOption</span>
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCheckbox('Positive')">Positive</span>
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCheckbox('Negative')">Negative</span>
                    <span class="badge bg-light text-primary pointer me-1" @click="setWidgetOptionsForCheckbox('LockOptions')">LockOptions</span>
                </div>
            </div>

            <hr />

            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12"></span>
                <div>
                    <div class="form-check me-2">
                        <input class="form-check-input" type="checkbox" value="" id="chk_Required" v-model="inputs.uiProps.Required">
                        <label class="form-check-label" for="chk_Required">
                            Required
                        </label>
                    </div>
                </div>
            </div>
            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12">ValidationRule</span>
                <input type="text" class="form-control form-control-sm"
                       :disabled="inputs.uiProps.Required!==true"
                       v-model="inputs.uiProps.ValidationRule"
                       data-ae-validation-required="false" data-ae-validation-rule=":=s(0,128)" />
            </div>

            <div>&nbsp;</div>

            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12 pt-1">Include in</span>
                <select class="form-select form-select-sm" v-model="inputs.uiProps.SearchType">
                    <option value="None">None</option>
                    <option value="Fast">Fast</option>
                    <option value="Expandable">Expandable</option>
                </select>
            </div>

            <div>&nbsp;</div>

            <div class="input-group input-group-sm border-0">
                <span class="input-group-text border-0 rounded-0 bg-transparent col-12">Ui Note</span>
                <textarea class="form-control form-control-sm" v-model="inputs.uiProps.Note" rows="2"
                          data-ae-validation-required="false" data-ae-validation-rule=":=s(0,256)"></textarea>
            </div>
        </div>

        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-24">
                    <button class="btn btn-sm btn-secondary w-100 py-2" @click="cancel" data-ae-key="ok">
                        <i class="fa-solid fa-cancel"></i>
                        &nbsp;
                        <span>Cancel</span>
                    </button>
                </div>
                <div class="col-24">
                    <button class="btn btn-sm btn-primary w-100 py-2" @click="ok" data-ae-key="ok">
                        <i class="fa-solid fa-check"></i>
                        &nbsp;
                        <span>Ok</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, regulator: null };
    export default {
        methods: {
            setWidgetOptionsForCheckbox(str) {
                let opt = `{}`;
                if (str === 'Positive') opt = `{"shownull":true,"nullClasses":"fa-minus text-secondary","trueClasses":"fa-check text-success","falseClasses":"fa-xmark text-danger"}`;
                if (str === 'Negative') opt = `{"shownull":true,"nullClasses":"fa-minus text-secondary","trueClasses":"fa-xmark text-danger","falseClasses":"fa-check text-success"}`;
                if (str === 'LockOptions') opt = `{"shownull":true,"nullClasses":"fa-minus text-secondary","trueClasses":"fa-lock text-danger","falseClasses":"fa-lock-open text-success"}`;
                _this.c.inputs.uiProps.UiWidgetOptions = JSON.stringify(JSON.parse(opt), null, 4);
            },
            setWidgetOptionsForCodeEditor(str) {
                let opt = `{"mode":"ace/mode/${str}"}`;
                _this.c.inputs.uiProps.UiWidgetOptions = JSON.stringify(JSON.parse(opt), null, 4);;
            },
            setWidgetOptionsForHtmlbox(str) {
                let opt = `{}`;
                _this.c.inputs.uiProps.UiWidgetOptions = JSON.stringify(JSON.parse(opt), null, 4);;
            },
            ok(e) {
                if (!_this.regulator.isValid()) return;
                if (_this.inputs.callback) _this.inputs.callback(_this.c.inputs);
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }

</script>