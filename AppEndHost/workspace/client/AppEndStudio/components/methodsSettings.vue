<template>
    <div class="card border-0 rounded-0">
        <div class="card-body bg-primary-subtle bg-gradient">
            <div class="fb p-1 fs-d7 border-0">
                Access Rules
                <div class="badge bg-light text-dark fs-d9">
                    Type names and press enter ( Empty : none | * : all )
                </div>
            </div>
            <div class="card">
                <div class="card-body py-2 pb-3">
                    <div class="my-1 ae-addable-list">
                        <div class="input-group input-group-sm mt-3">
                            <span class="input-group-text text-secondary fs-d7">Allowed Roles</span>
                            <input type="text" class="form-control form-control-sm" @keyup.enter="addAllowedRoleToList" />
                        </div>
                        <div class="form-control form-control-lg rounded-top-0">
                            <span class="btn btn-sm bg-success-subtle text-success-emphasis me-1" @click="removeAllowedRoleFromList"
                                  v-for="i in d['AccessRules']['AllowedRoles']">
                                <i class="fa-solid fa-fw fa-user-group"></i> {{i}}
                            </span>
                        </div>
                    </div>
                    <div class="my-1 ae-addable-list">
                        <div class="input-group input-group-sm mt-3">
                            <span class="input-group-text text-secondary fs-d7">Allowed Users</span>
                            <input type="text" class="form-control form-control-sm" @keyup.enter="addAllowedUserToList" />
                        </div>
                        <div class="form-control form-control-lg rounded-top-0">
                            <span class="btn btn-sm bg-success-subtle text-success-emphasis me-1" @click="removeAllowedUserFromList"
                                  v-for="i in d['AccessRules']['AllowedUsers']">
                                <i class="fa-solid fa-fw fa-user"></i> {{i}}
                            </span>
                        </div>
                    </div>
                    <div class="my-1 ae-addable-list">
                        <div class="input-group input-group-sm mt-3">
                            <span class="input-group-text text-secondary fs-d7">Denied Users</span>
                            <input type="text" class="form-control form-control-sm" @keyup.enter="addDeniedUserToList" />
                        </div>
                        <div class="form-control form-control-lg rounded-top-0">
                            <span class="btn btn-sm bg-danger-subtle text-danger-emphasis me-1" @click="removeDeniedUserFromList"
                                  v-for="i in d['AccessRules']['DeniedUsers']">
                                <i class="fa-solid fa-fw fa-user"></i> {{i}}
                            </span>
                        </div>
                    </div>
                </div>
            </div>

            <div class="fs-d7">&nbsp;</div>
            <div class="fb p-1 fs-d7 border-0">
                Cache Policy
            </div>
            <div class="card">
                <div class="card-body py-2 pb-3">
                    <div class="row gx-2">
                        <div class="col-24">
                            <label class="form-label text-secondary fs-d7 my-0" for="select_CacheType">Cache Level</label>
                            <select class="form-select form-select-sm" aria-label="Default select example" v-model="d['CachePolicy']['CacheLevel']" id="select_CacheType">
                                <option value="0">None</option>
                                <option value="1">PerUser</option>
                                <option value="2">AllUsers</option>
                            </select>
                        </div>
                        <div class="col-24" v-if="d['CachePolicy']['CacheLevel'].toString()==='1' || d['CachePolicy']['CacheLevel'].toString()==='2'">
                            <label class="form-label text-secondary fs-d7 my-0" for="txt_AbsoluteExpirationSeconds">Between 10 and 999,999 Seconds</label>
                            <input type="text" class="form-control form-control-sm"
                                   v-model="d['CachePolicy']['AbsoluteExpirationSeconds']" id="txt_AbsoluteExpirationSeconds"
                                   data-ae-validation-item="this"
                                   data-ae-validation-required="true"
                                   data-ae-validation-onstart="true"
                                   data-ae-validation-onchange="true"
                                   data-ae-validation-min="10"
                                   data-ae-validation-max-len="6"
                                   data-ae-validation-type="number" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="fs-d7">&nbsp;</div>
            <div class="fb p-1 fs-d7 border-0">
                Log Policy
            </div>
            <div class="card">
                <div class="card-body py-2 pb-3">
                    <div class="row">
                        <div class="col-48">
                            <label class="form-label text-secondary fs-d7 my-0" for="txt_AbsoluteExpirationSeconds">OnErrorLogMethod</label>
                            <input type="text" class="form-control form-control-sm" v-model="d['LogPolicy']['OnErrorLogMethod']" />
                        </div>
                        <div class="col-48 pt-1">
                            <label class="form-label text-secondary fs-d7 my-0" for="txt_AbsoluteExpirationSeconds">OnSuccessLogMethod</label>
                            <input type="text" class="form-control form-control-sm" v-model="d['LogPolicy']['OnSuccessLogMethod']" />
                        </div>
                    </div>
                </div>
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
    shared.setAppTitle("Methods All");
    let _this = { cid: "", c: null, inputs: {} , d: null };
    export default {
        methods: {
            addAllowedRoleToList(event) {
                let v = $(event.target).val().trim();
                if (v === '') return;
                if (v === "*") {
                    _this.c.d['AccessRules']["AllowedRoles"] = [];
                    $(event.target).attr("disabled", "disabled");
                }
                _this.c.d['AccessRules']["AllowedRoles"].push(v);
                $(event.target).val("");
            },
            removeAllowedRoleFromList(event) {
                let v = $(event.target).text().trim();
                _this.c.d['AccessRules']["AllowedRoles"] = _.filter(_this.c.d['AccessRules']["AllowedRoles"], function (i) {
                    return i !== v;
                });
                if (v === "*") {
                    $(event.target).parents('.ae-addable-list').find("input").removeAttr("disabled");
                    $(event.target).parents('.ae-addable-list').find("input").focus();
                }
            },
            addAllowedUserToList(event) {
                let v = $(event.target).val().trim();
                if (v === '') return;
                if (v === "*") {
                    _this.c.d['AccessRules']["AllowedUsers"] = [];
                    $(event.target).attr("disabled", "disabled");
                }
                _this.c.d['AccessRules']["AllowedUsers"].push(v);
                $(event.target).val("");
            },
            removeAllowedUserFromList(event) {
                let v = $(event.target).text().trim();
                _this.c.d['AccessRules']["AllowedUsers"] = _.filter(_this.c.d['AccessRules']["AllowedUsers"], function (i) {
                    return i !== v;
                });
                if (v === "*") {
                    $(event.target).parents('.ae-addable-list').find("input").removeAttr("disabled");
                    $(event.target).parents('.ae-addable-list').find("input").focus();
                }
            },
            addDeniedUserToList(event) {
                let v = $(event.target).val().trim();
                if (v === '') return;
                if (v === "*") {
                    _this.c.d['AccessRules']["DeniedUsers"] = [];
                    $(event.target).attr("disabled", "disabled");
                }
                _this.c.d['AccessRules']["DeniedUsers"].push(v);
                $(event.target).val("");
            },
            removeDeniedUserFromList(event) {
                let v = $(event.target).text().trim();
                _this.c.d['AccessRules']["DeniedUsers"] = _.filter(_this.c.d['AccessRules']["DeniedUsers"], function (i) {
                    return i !== v;
                });
                if (v === "*") {
                    $(event.target).parents('.ae-addable-list').find("input").removeAttr("disabled");
                    $(event.target).parents('.ae-addable-list').find("input").focus();
                }
            },
            ok() {
                _this.c.d['CachePolicy']['AbsoluteExpirationSeconds'] = _this.c.d['CachePolicy']['AbsoluteExpirationSeconds'].toString().toInt();

                rpcAEP("WriteMethodSettings", { "NamespaceName": _this.inputs.ns, "ClassName": _this.inputs.cs, "MethodName": _this.inputs.mn, "NewMethodSettings": _this.c.d }, function (res) {
                    if (R0R(res) === true) {
                        shared.closeComponent(_this.cid);
                    } else {
                        showError(res.Message);
                    }
                });

            },
            cancel() {
                shared.closeComponent(_this.cid);
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() {
            _this.d = rpcSync({
                requests: [{
                    "Method": "Zzz.AppEndProxy.GetMethodSettings",
                    "Inputs": { "NamespaceName": _this.inputs.ns, "ClassName": _this.inputs.cs, "MethodName": _this.inputs.mn }
                }]
            })[0]["Result"];
            return _this;
        },
        created() {_this.c = this;},
        mounted() {},
        props: {cid: String}

    }

</script>