<template>
    <div class="col-48 h-100">

        <div class="card h-100 border-0 bg-transparent">
            <div class="card-body p-3 pb-4 bg-transparent fs-d8">

                <div class="fw-bold fs-d9">
                    {{inputs.oJson.ObjectName}} - {{inputs.oJson.ObjectType}}
                </div>

                <hr class="my-2 mt-1" />

                <div class="input-group input-group-sm border-0">
                    <span class="input-group-text border-0 rounded-0 bg-transparent col-12">Developer Note</span>
                    <textarea class="form-control form-control-sm" v-model="inputs.oJson.DevNote" rows="2"
                              data-ae-validation-required="false" data-ae-validation-rule=":=s(0,512)"></textarea>
                </div>

                <hr class="my-2 mt-1" />

                <!--<div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>OpenCreatePlace</span></div>
                    <div class="col">
                        <select class="form-select form-select-sm" v-model="inputs.oJson.OpenCreatePlace">
                            <option value="InlineDialog">InlineDialog</option>
                            <option value="NewWindow">NewWindow</option>
                            <option value="Both">Both</option>
                        </select>
                    </div>
                </div>-->
                <div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>OpenUpdatePlace</span></div>
                    <div class="col">
                        <select class="form-select form-select-sm" v-model="inputs.oJson.OpenUpdatePlace">
                            <option value="InlineDialog">InlineDialog</option>
                            <option value="NewWindow">NewWindow</option>
                            <option value="Both">Both</option>
                        </select>
                    </div>
                </div>

                <hr class="my-2 mt-1" />

                <div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>ObjectIcon</span></div>
                    <div class="col">
                        <input type="text" class="form-control form-control-sm" v-model="inputs.oJson.ObjectIcon" />
                    </div>
                </div>
                <div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>ObjectColor</span></div>
                    <div class="col">
                        <input type="text" class="form-control form-control-sm" v-model="inputs.oJson.ObjectColor" />
                    </div>
                </div>
                <div class="my-2">&nbsp;</div>
                <div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>Parent column</span></div>
                    <div class="col">
                        <select class="form-select form-select-sm" v-model="inputs.oJson.ParentColumn">
                            <option></option>
                            <option v-for="i in inputs.oJson.Columns" :value="i.Name">{{i.Name}}</option>
                        </select>
                    </div>
                </div>
                <div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>Note column</span></div>
                    <div class="col">
                        <select class="form-select form-select-sm" v-model="inputs.oJson.NoteColumn">
                            <option></option>
                            <option v-for="i in inputs.oJson.Columns" :value="i.Name">{{i.Name}}</option>
                        </select>
                    </div>
                </div>
                <div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>ViewOrder column</span></div>
                    <div class="col">
                        <select class="form-select form-select-sm" v-model="inputs.oJson.ViewOrderColumn">
                            <option></option>
                            <option v-for="i in inputs.oJson.Columns" :value="i.Name">{{i.Name}}</option>
                        </select>
                    </div>
                </div>
                <div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>UiIcon column</span></div>
                    <div class="col">
                        <select class="form-select form-select-sm" v-model="inputs.oJson.UiIconColumn">
                            <option></option>
                            <option v-for="i in inputs.oJson.Columns" :value="i.Name">{{i.Name}}</option>
                        </select>
                    </div>
                </div>
                <div class="row my-2">
                    <div class="col-16 pt-2 px-2"><span>UiColor column</span></div>
                    <div class="col">
                        <select class="form-select form-select-sm" v-model="inputs.oJson.UiColorColumn">
                            <option></option>
                            <option v-for="i in inputs.oJson.Columns" :value="i.Name">{{i.Name}}</option>
                        </select>
                    </div>
                </div>


            </div>
            <div class="card-footer p-0">
                <div class="container-fluid pt-2 pb-1">
                    <div class="row p-0">
                        <div class="col-36 px-2">
                            <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                                <i class="fa-solid fa-check me-1"></i>
                                <span>Ok</span>
                            </button>
                        </div>
                        <div class="col-12 px-2">
                            <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                                <i class="fa-solid fa-xmark me-1"></i>
                                <span>Cancel</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {} };
    export default {
        methods: {
            ok(e) {
                if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.inputs.oJson);
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        created() {
            _this.c = this;
        },
        mounted() {
            
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        props: { cid: String }
    }

</script>