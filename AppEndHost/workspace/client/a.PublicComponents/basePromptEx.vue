<template>
    <div class="card h-100 border-0 shadow-lg rounded-0 bg-dark-subtle">
        <div class="card-body p-3 pb-4 fs-d8 scrollable">

            <div class="card">
                <div class="card-body">

                    <div class="fw-bold" v-if="shared.fixNull(inputs.message1,'')!==''">{{inputs.message1}}</div>
                    <div class="px-2 fs-d9 text-secondary" v-if="shared.fixNull(inputs.message2,'')!==''">{{inputs.message2}}</div>

                    <hr class="my-3 border-4 border-primary-subtle" v-if="shared.fixNull(inputs.message1,'')!=='' || shared.fixNull(inputs.message2,'')!==''" />

                    <div class="row">
                        <div class="col-48 p-2">

                            <lable>{{inputs.reasonTitle}}</lable>
                            <select class="form-select form-select-sm mt-1 ae-focus" v-model="reasonId" :data-ae-validation-required="inputs.reasonRequired">
                                <option value="">----</option>
                                <option v-for="i in shared.enum(inputs.reasonsParentId)" :value="i.Id">{{i.Title}}</option>
                            </select>

                            <div>&nbsp;</div>

                            <lable>{{inputs.noteTitle}}</lable>
                            <textarea class="form-control form-control-sm mt-1" v-model="note" @keyup.enter="ok"
                                      :data-ae-validation-required="inputs.noteRequired" :data-ae-validation-rule="inputs.noteRule"></textarea>

                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div class="card-footer p-0 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <button class="btn btn-link text-decoration-none bg-hover-light w-100 py-3 rounded-0" @click="ok">
                <i class="fa-solid fa-check me-2"></i><span>{{inputs.okText}}</span>
            </button>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, regulator: null, reasonId: '', note: '' };
    export default {
        methods: {
            ok() {
                if (!_this.regulator.isValid()) return;
                if (_this.c.inputs.callback) _this.inputs.callback({ reasonId: _this.c.reasonId, note: _this.c.note });
                closeComponent(_this.cid);
            }
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