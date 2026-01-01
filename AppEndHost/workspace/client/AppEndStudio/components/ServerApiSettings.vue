<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body bg-primary-subtle-light scrollable" v-if="d!==null">

            

            <div class="card">
                <div class="card-header p-2">
                    <div class="fw-bold fs-d7">
                        Access Rules
                        <div class="badge bg-light text-dark fs-d9">
                            Type names and press enter ( Empty : none | * : all )
                        </div>
                    </div>
                </div>
                <div class="card-body p-2 pt-0">
                    <div class="mb-1 ae-addable-list">
                        <div class="input-group input-group-sm mt-3">
                            <span class="input-group-text text-dark fs-d7 fw-bold">Allowed Roles</span>
                            <input type="text" class="form-control form-control-sm" @keyup.enter="addAllowedRoleToList" />
                        </div>
                        <div class="form-control form-control-lg rounded-top-0">
                            <span class="btn btn-sm bg-success-subtle text-success-emphasis me-1" @click="removeAllowedRoleFromList"
                                  v-for="i in d['AccessRules']['AllowedRoles']">
                                <i class="fa-solid fa-fw fa-user-group me-1"></i> {{i}}
                            </span>
                        </div>
                    </div>
                    <div class="mb-1 ae-addable-list">
                        <div class="input-group input-group-sm mt-3">
                            <span class="input-group-text text-dark fs-d7 fw-bold">Allowed Users</span>
                            <input type="text" class="form-control form-control-sm" @keyup.enter="addAllowedUserToList" />
                        </div>
                        <div class="form-control form-control-lg rounded-top-0">
                            <span class="btn btn-sm bg-success-subtle text-success-emphasis me-1" @click="removeAllowedUserFromList"
                                  v-for="i in d['AccessRules']['AllowedUsers']">
                                <i class="fa-solid fa-fw fa-user me-1"></i> {{i}}
                            </span>
                        </div>
                    </div>
                    <div class="mb-1 ae-addable-list">
                        <div class="input-group input-group-sm mt-3">
                            <span class="input-group-text text-dark fs-d7 fw-bold">Denied Users</span>
                            <input type="text" class="form-control form-control-sm" @keyup.enter="addDeniedUserToList" />
                        </div>
                        <div class="form-control form-control-lg rounded-top-0">
                            <span class="btn btn-sm bg-danger-subtle text-danger-emphasis me-1" @click="removeDeniedUserFromList"
                                  v-for="i in d['AccessRules']['DeniedUsers']">
                                <i class="fa-solid fa-fw fa-user me-1"></i> {{i}}
                            </span>
                        </div>
                    </div>
                </div>
            </div>



            <div class="card mt-2">
                <div class="card-header p-2">
                    <div class="fw-bold fs-d7">
                        Log Policy
                    </div>
                </div>
                <div class="card-body p-2">
                    <div class="row">
                        <div class="col-24">
                            <select class="form-select form-select-sm" v-model="d['LogPolicy']">
                                <option value="0">IgnoreLogging</option>
                                <option value="1">TrimInputs</option>
                                <option value="2">Full</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card mt-2">
                <div class="card-header p-2">
                    <div class="fw-bold fs-d7">
                        Cache Policy
                    </div>
                </div>
                <div class="card-body p-2 pt-0">
                    <div class="row gx-2">
                        <div class="col-24">
                            <label class="form-label text-secondary fs-d7 my-0" for="select_CacheType">Cache Level</label>
                            <select class="form-select form-select-sm" aria-label="Default select example" v-model="d['CachePolicy']['CacheLevel']" id="select_CacheType" @change="checkForCachability">
                                <option value="0">None</option>
                                <option value="1">PerUser</option>
                                <option value="2">AllUsers</option>
                            </select>
                        </div>
                        <div class="col-24">
                            <label class="form-label text-secondary fs-d7 my-0" for="txt_AbsoluteExpirationSeconds">Between 30 and 999,999 Seconds</label>
                            <input id="txt_AbsoluteExpirationSeconds" type="text" class="form-control form-control-sm"
                                   :disabled="d['CachePolicy']['CacheLevel'].toString()==='0'"
                                   v-model="d['CachePolicy']['AbsoluteExpirationSeconds']"
                                   data-ae-validation-item="this"
                                   data-ae-validation-required="true"
                                   data-ae-validation-onstart="true"
                                   data-ae-validation-onchange="true"
                                   data-ae-validation-min="30"
                                   data-ae-validation-max-len="6"
                                   data-ae-validation-type="number" />
                        </div>
                    </div>
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
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, d: null };
    export default {
        methods: {
            checkForCachability(event) {
                let readConcepts = ['read', 'get', 'select'];
                let methodName = (_this.inputs.mn || '').toLowerCase();
                let isReadLike = readConcepts.some(function (k) { return methodName.indexOf(k) !== -1; });
                if (!isReadLike) {
                    showWarning("Cache settings are only supported for methods with read behavior (e.g., those including 'get', 'read', or 'select'). Applying cache settings to this non-read method is likely to cause application performance issues.");
                }
            },
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
            rpcAEP("GetMethodSettings", { "NamespaceName": _this.inputs.ns, "ClassName": _this.inputs.cs, "MethodName": _this.inputs.mn }, function (res) {
                _this.c.d = R0R(res);
            });
            return _this;
        },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String }

    }

</script>