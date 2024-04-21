<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-body bg-dark-subtle bg-opacity-75 scrollable">
			<div class="row">
				<div class="card rounded-1 border-light mb-1">
					<div class="card-body">
						<div class="row">
							<div class="col-48" v-if="inputs.fkColumn!=='Title'">
								<label class="fs-d9 text-muted ms-2" for="input_Title">{{shared.translate('Title')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_Title" v-model="row.Title" data-ae-validation-required="true" data-ae-validation-rule="">
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='ShortName'">
								<label class="fs-d9 text-muted ms-2" for="input_ShortName">{{shared.translate('ShortName')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_ShortName" v-model="row.ShortName" data-ae-validation-required="false" data-ae-validation-rule="">
							</div>
						</div>
					</div>
				</div>
				<div class="card rounded-1 border-light mb-1">
					<div class="card-header text-bg-light">
						{{shared.translate('More')}}
					</div>
					<div class="card-body">
						<div class="row">
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_ViewOrder">{{shared.translate('ViewOrder')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_ViewOrder" v-model="row.ViewOrder" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,10000)">
							</div>
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_Note">{{shared.translate('Note')}}</label>
								<textarea type="text" class="form-control form-control-sm " id="input_Note" v-model="row.Note" data-ae-validation-required="false" data-ae-validation-rule=""></textarea>
							</div>
							<div class="col-48">
								<div class="form-control mt-2 pointer text-nowrap " data-ae-widget="nullableCheckbox">
									<i class="fa-solid fa-fw me-1"></i>
									<span>{{shared.translate('IsActive')}}</span>
									<input type="hidden" v-model="row.IsActive">
								</div>
							</div>
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_UiColor">{{shared.translate('UiColor')}}</label>
								<div class="input-group input-group-sm border-0">
									<span class="form-control form-control-sm bg-transparent">{{row.UiColor}}</span>
									<input type="color" class="input-group-text p-3" :style="'background-color:'+row.UiColor" id="input_UiColor" v-model="row.UiColor" data-ae-validation-required="false" data-ae-validation-rule="">
								</div>
							</div>
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_UiIcon">{{shared.translate('UiIcon')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_UiIcon" v-model="row.UiIcon" data-ae-validation-required="false" data-ae-validation-rule="">
							</div>
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_Metadata">{{shared.translate('Metadata')}}</label>
								<textarea type="text" class="form-control form-control-sm " id="input_Metadata" v-model="row.Metadata" data-ae-validation-required="false" data-ae-validation-rule=""></textarea>
							</div>
						</div>
					</div>
				</div>
				<div class="card rounded-1 border-light mb-1">
					<div class="card-header text-bg-light">
						{{shared.translate('Parent')}}
					</div>
					<div class="card-body">
						<div class="row">
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_ParentId">{{shared.translate('ParentId')}}</label>
								<div class="form-control p-0 px-1 data-ae-validation ">
									<div class="input-group" data-ae-widget="objectPicker">
										<input type="hidden" v-model="row.ParentId" data-ae-validation-required="false">
										<input type="hidden" v-model="row.ParentId_Title">
										<input type="hidden" v-model="row.ParentId_ShortName">
										<input type="text" class="form-control bg-transparent border-0" :value="shared.fixNull(row.ParentId+' '+row.ParentId_Title+' '+row.ParentId_ShortName,'',true)" :placeholder="shared.translate('ParentId')" disabled="">
										<button class="btn btn-outline-secondary bg-transparent border-0 text-hover-primary ae-objectpicker-search" type="button" @click="localOpenPicker('ParentId')">
											<i class="fa-solid fa-search"></i>
										</button>
										<button class="btn btn-outline-secondary bg-transparent border-0 text-hover-danger ae-objectpicker-clear" type="button">
											<i class="fa-solid fa-times"></i>
										</button>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
			<div class="row">
				<div class="col-12">
					<button class="btn btn-sm btn-secondary w-100 py-2" @click="cancel" data-ae-key="ok">
						<i class="fa-solid fa-cancel pe-1"></i>
						<span>{{shared.translate("Cancel")}}</span>
					</button>
				</div>
				<div class="col-36">
					<button class="btn btn-sm btn-primary w-100 py-2" @click="ok" data-ae-key="ok">
						<i class="fa-solid fa-check pe-1"></i>
						<span>{{shared.translate("Ok")}}</span>
					</button>
				</div>
			</div>
		</div>
	</div>
</template>
<script>
let _this = { cid: "", c: null, inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "", regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_BaseInfo";
_this.submitMethod = "UpdateByKey";
_this.createComponent = ""; 
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.Common_BaseInfo.ReadByKey","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadByKey","Params":[{"Name":"Id","Value":""}]}}};




_this.pickerRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}});

_this.pickerHumanIds.push({Id:'ParentId_HumanIds',Items:["Title","ShortName"]});

export default {
	methods: {
		localSelectFiles(relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType) {
			crudSelectFiles(_this, relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType);
		},
		localAddRelation(relName) { crudAddRelation(_this, relName); },
		localRemoveRelation(relName, ind) { crudRemoveRelation(_this, relName, ind); },
		localOpenPicker(colName) { crudOpenPicker(_this, _this.c.row, colName); },
		localCrudUpdateRelation(compPath, modalSize, recordKey,ind,fkColumn,relName) { crudUpdateRelation(_this, compPath, modalSize, recordKey,ind,fkColumn,relName); },
		localCrudBaseInfo() { crudLoadBaseInfo(_this); },
		localLoadMasterRecord() { 
			if (_this.c.inputs.okAction !== 'return') 
				crudLoadMasterRecord(_this);
			else 
				_this.c.row=_this.c.inputs.row;
		},
		ok() {
			if (!_this.regulator.isValid()) return;
			if (_this.c.inputs.okAction === 'return') {
				if (_this.inputs.callback) _this.inputs.callback(_this.row);
				_this.c.close();
			} else {
				crudSaveRecord(_this, function () {
					if (_this.inputs.callback) _this.inputs.callback();
					_this.c.close();
				});
			}
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
	mounted() { initVueComponent(_this); _this.c.localCrudBaseInfo(); _this.c.localLoadMasterRecord() },
	props: { cid: String }
}


</script>