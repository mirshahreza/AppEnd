<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0" v-if="ismodal!=='true'">
			<div class="hstack">
				<button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="ok" data-ae-key="ok">
					<i class="fa-solid fa-save pe-1"></i>
					<span>{{shared.translate("Save")}}</span>
				</button>
				<div class="p-0 ms-auto"></div>
			</div>
		</div>
		<div class="card-body bg-primary-subtle-light scrollable">
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='Title'">
							<label class="fs-d8 text-muted ms-2" for="input_Title">{{shared.translate('Title')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Title" v-model="row.Title" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,128)">
						</div>
					</div>
				</div>
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='ParentId'">
							<label class="fs-d8 text-muted ms-2" for="input_ParentId">{{shared.translate('ParentId')}}</label>
							<div class="form-control form-control-sm py-0 data-ae-validation ">
								<div class="input-group input-group-sm p-0 pt-1" data-ae-widget="objectPicker">
									<input type="hidden" v-model="row.ParentId" data-ae-validation-required="false">
									<input type="hidden" v-model="row.ParentId_Title">
									<input type="text" class="form-control bg-transparent p-0 m-0 border-0" :value="shared.fixNull(row.ParentId+' '+row.ParentId_Title,'',true)" :placeholder="shared.translate('ParentId')" disabled="">
									<span></span>
									<button class="mx-1 text-hover-danger ae-objectpicker-search" type="button" @click="openPicker({colName:'ParentId'})">
										<i class="fa-solid fa-hand-pointer"></i>
									</button>
									<button class="mx-1 text-hover-danger ae-objectpicker-clear" type="button">
										<i class="fa-solid fa-times"></i>
									</button>
								</div>
							</div>
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='ShortName'">
							<label class="fs-d8 text-muted ms-2" for="input_ShortName">{{shared.translate('ShortName')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_ShortName" v-model="row.ShortName" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,16)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='ViewOrder'">
							<label class="fs-d8 text-muted ms-2" for="input_ViewOrder">{{shared.translate('ViewOrder')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_ViewOrder" v-model="row.ViewOrder" data-ae-validation-required="false" data-ae-validation-rule="">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='Value'">
							<label class="fs-d8 text-muted ms-2" for="input_Value">{{shared.translate('Value')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Value" v-model="row.Value" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,2147483647)">
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="card-footer p-0" v-if="ismodal==='true'">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>{{shared.translate("Save")}}</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>{{shared.translate("Cancel")}}</span>
                        </button>
                    </div>
                </div>
            </div>
		</div>
	</div>
</template>
<script>
let _this = { cid: "", ismodal:"", c: null, templateType:"Create", inputs: {}, dbConfName: "", objectName: "", submitMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], row: {}, Relations: {}, RelationsMetaData: {}, regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "BaseInfo";
_this.submitMethod = "Create";

_this.row = {"ParentId":"","Title":null,"ShortName":null,"ViewOrder":null,"Value":null,"Note":null,"Metadata":null,"IsActive":null,"UiColor":null,"UiIcon":null};

_this.pickerRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});

_this.pickerHumanIds.push({Id:'ParentId_HumanIds',Items:["Title"]});
export default {
	methods: {
	},
	setup(props) { 
		_this.cid = props['cid'];
		_this.ismodal = props['ismodal'];
		_this.inputs = shared["params_" + _this.cid];
	},
	data() { return _this; },
	created() { _this.c = this; assignDefaultMethods(_this); },
	mounted() { initVueComponent(_this); _this.c.componentFinalization(); },
	props: { cid: String, ismodal: String }
}

</script>