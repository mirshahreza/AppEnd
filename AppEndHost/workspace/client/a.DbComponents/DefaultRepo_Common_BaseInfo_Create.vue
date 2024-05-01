<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-body bg-primary-subtle-light scrollable">
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
					<div class="card-body">
						<div class="row">
							<div class="col-48" v-if="inputs.fkColumn!=='ParentId'">
								<label class="fs-d9 text-muted ms-2" for="input_ParentId">{{shared.translate('ParentId')}}</label>
								<select class="form-select form-select-sm" v-model="row.ParentId" data-ae-validation-required="false">
									<option value="">-</option>
									<option v-for="i in shared.getResponseObjectById(initialResponses,'ParentId_Lookup')" :value="i['Id']">{{i.Title}} {{i.ShortName}}</option>
								</select>
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='ViewOrder'">
								<label class="fs-d9 text-muted ms-2" for="input_ViewOrder">{{shared.translate('ViewOrder')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_ViewOrder" v-model="row.ViewOrder" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,10000)">
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='Note'">
								<label class="fs-d9 text-muted ms-2" for="input_Note">{{shared.translate('Note')}}</label>
								<textarea type="text" class="form-control form-control-sm " id="input_Note" v-model="row.Note" data-ae-validation-required="false" data-ae-validation-rule=""></textarea>
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='Metadata'">
								<label class="fs-d9 text-muted ms-2" for="input_Metadata">{{shared.translate('Metadata')}}</label>
								<textarea type="text" class="form-control form-control-sm " id="input_Metadata" v-model="row.Metadata" data-ae-validation-required="false" data-ae-validation-rule=""></textarea>
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='MetaInfoUpdatedBy'">
								<label class="fs-d9 text-muted ms-2" for="input_MetaInfoUpdatedBy">{{shared.translate('MetaInfoUpdatedBy')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_MetaInfoUpdatedBy" v-model="row.MetaInfoUpdatedBy" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,10000)">
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='MetaInfoUpdatedOn'">
								<label class="fs-d9 text-muted ms-2" for="input_MetaInfoUpdatedOn">{{shared.translate('MetaInfoUpdatedOn')}}</label>
								<div class="input-group input-group-sm">
									<button class="btn btn-sm btn-outline-secondary" id="dp_MetaInfoUpdatedOn" data-ae-widget="dtPicker" data-ae-widget-options="{&quot;targetTextSelector&quot;:&quot;#dpText_MetaInfoUpdatedOn&quot;,&quot;targetDateSelector&quot;:&quot;#dpDate_MetaInfoUpdatedOn&quot;,&quot;enableTimePicker&quot;:true,&quot;dateFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;,&quot;textFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;}">
										<i class="fa-solid fa-fw fa-calendar"></i>
									</button>
									<input class="form-control form-control-sm text-center" style="direction:ltr" id="dpText_MetaInfoUpdatedOn" disabled="">
									<input class="form-control form-control-sm" id="dpDate_MetaInfoUpdatedOn" type="hidden" v-model="row.MetaInfoUpdatedOn">
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
let _this = { cid: "", c: null, inputs: {}, dbConfName: "", objectName: "", submitMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], row: {}, Relations: {}, RelationsMetaData: {}, regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_BaseInfo";
_this.submitMethod = "Create";

_this.row = {"ParentId":"","Title":null,"ShortName":null,"ViewOrder":null,"Note":null,"IsActive":null,"UiColor":null,"UiIcon":null,"Metadata":null,"MetaInfoUpdatedBy":null,"MetaInfoUpdatedOn":null,"IsActiveUpdatedBy":null,"IsActiveUpdatedOn":null,"UiInfoUpdatedBy":null,"UiInfoUpdatedOn":null};





_this.initialRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});
export default {
	methods: {
		localSelectFiles(relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType) {
			crudSelectFiles(_this, relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType);
		},
		localAddRelation(relName) { crudAddRelation(_this, relName); },
		localRemoveRelation(relName, ind) { crudRemoveRelation(_this, relName, ind); },
		localOpenPicker(colName) { crudOpenPicker(_this, _this.c.row, colName); },
		localCrudUpdateRelation(compPath, modalSize, recordKey, ind,fkColumn,relName) { crudUpdateRelation(_this, compPath, modalSize, recordKey, ind,fkColumn,relName); },
		localCrudBaseInfo() { crudLoadBaseInfo(_this); },
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
	mounted() { initVueComponent(_this); _this.c.localCrudBaseInfo(); },
	props: { cid: String }
}

</script>