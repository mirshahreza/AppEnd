<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 h-100">
        <div class="card-header p-2 fw-bold fs-d9">
            {{inputs.RoleName}} [ {{inputs.RoleId}} ]
        </div>
        <div class="card-body scrollable" style="background-color:#fffbfb">
            
            <div class="card h-100 shadow-sm" id="accMain">
                <div class="card-header border-bottom border-bottom-1 border-top border-top-1 pointer fs-d9 text-primary text-hover-success fw-bold" @click="openMe">
                    Gender
                </div>
                <div class="card-body collapse-area">
                    <div class="btn btn-sm me-1 my-1 p-1 fs-d7" :class="isAssigned(attItem.Id) ? 'btn-success' : 'btn-light'"
                         v-for="attItem in shared.enum(102)" @click="switchAttribute(attItem.Id)">
                        <i class="fa-solid fa-fw me-1" :class="isAssigned(attItem.Id) ? 'fa-check' : 'fa-minus'"></i>
                        <span>{{attItem.Title}}</span>
                    </div>
                </div>
            </div>

        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, d: null, attributes: [] };

    export default {
        methods: {
            isAssigned(attItemId) {
                return _.filter(_this.c.attributes, function (i) { return i.AttributeId === attItemId; }).length > 0;
            },
            switchAttribute(attItemId) {
                let flag = _.filter(_this.c.attributes, function (i) { return i.AttributeId === attItemId; }).length === 0;
                rpcAEP("SetAttributesByRoleId", { RoleId: _this.c.inputs.RoleId, AttributeId: attItemId, Flag: flag }, function (res) {
                    if (flag === true) _this.c.attributes.push({ RoleId: _this.c.inputs.RoleId, AttributeId: attItemId });
                    else _this.c.attributes = _.filter(_this.c.attributes, function (i) { return i.AttributeId !== attItemId });
                });
            },
            openMe(elm) {
                $("#accMain .collapse-area").hide();
                $(elm.target).next().show();
            },
            loadRoleAttributes() {
                rpcAEP("GetAttributesByRoleId", { RoleId: _this.c.inputs.RoleId }, function (res) {
                    _this.c.attributes = R0R(res);
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.loadRoleAttributes(); },
        props: { cid: String }
    }
</script>