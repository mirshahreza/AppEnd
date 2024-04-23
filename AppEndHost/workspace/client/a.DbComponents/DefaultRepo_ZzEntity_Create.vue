<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-body bg-dark-subtle bg-opacity-75 scrollable">
			<div class="row">
				<div class="card rounded-1 border-light mb-1">
					<div class="card-body">
						<div class="row">
							<div class="col-48" v-if="inputs.fkColumn!=='MyTestField'">
								<label class="fs-d9 text-muted ms-2" for="input_MyTestField">{{shared.translate('MyTestField')}}</label>
								<select class="form-select form-select-sm" v-model="row.MyTestField" data-ae-validation-required="false">
									<option value="">-</option>
									<option v-for="i in shared.getResponseObjectById(initialResponses,'MyTestField_Lookup')" :value="i['Id']">{{i.Title}} {{i.ShortName}}</option>
								</select>
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='Note'">
								<label class="fs-d9 text-muted ms-2" for="input_Note">{{shared.translate('Note')}} [{{shared.getEditorName('{    "mode": "ace/mode/csharp"}')}}]</label>
								<div class="border border-2 rounded-2 data-ae-validation ">
									<div class="code-editor-container" data-ae-widget="editorBox" data-ae-widget-options="{    &quot;mode&quot;: &quot;ace/mode/csharp&quot;}" id="ace_Note" style="height:150px;"></div>
									<input type="hidden" v-model="row.Note" data-ae-validation-required="false" data-ae-validation-rule="">
								</div>
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='ColNum'">
								<label class="fs-d9 text-muted ms-2" for="input_ColNum">{{shared.translate('ColNum')}}</label>
								<div class="input-group input-group-sm border-0">
									<span class="input-group-text" style="min-width:100px;">{{row.ColNum}}</span>
									<input type="range" class="form-control form-control-sm" min="880" max="10000" id="input_ColNum" v-model="row.ColNum" data-ae-validation-required="true" data-ae-validation-rule=":=i(880,10000)">
								</div>
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='FldD'">
								<label class="fs-d9 text-muted ms-2" for="input_FldD">{{shared.translate('FldD')}}</label>
								<div class="input-group input-group-sm">
									<button class="btn btn-sm btn-outline-secondary" id="dp_FldD" data-ae-widget="dtPicker" data-ae-widget-options="{&quot;targetTextSelector&quot;:&quot;#dpText_FldD&quot;,&quot;targetDateSelector&quot;:&quot;#dpDate_FldD&quot;,&quot;dateFormat&quot;:&quot;yyyy-MM-dd&quot;,&quot;textFormat&quot;:&quot;yyyy-MM-dd&quot;}">
										<i class="fa-solid fa-fw fa-calendar"></i>
									</button>
									<input class="form-control form-control-sm text-center" style="direction:ltr" id="dpText_FldD" disabled="">
									<input class="form-control form-control-sm" id="dpDate_FldD" type="hidden" v-model="row.FldD">
								</div>
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='FldDT'">
								<label class="fs-d9 text-muted ms-2" for="input_FldDT">{{shared.translate('FldDT')}}</label>
								<div class="input-group input-group-sm">
									<button class="btn btn-sm btn-outline-secondary" id="dp_FldDT" data-ae-widget="dtPicker" data-ae-widget-options="{&quot;targetTextSelector&quot;:&quot;#dpText_FldDT&quot;,&quot;targetDateSelector&quot;:&quot;#dpDate_FldDT&quot;,&quot;enableTimePicker&quot;:true,&quot;dateFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;,&quot;textFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;}">
										<i class="fa-solid fa-fw fa-calendar"></i>
									</button>
									<input class="form-control form-control-sm text-center" style="direction:ltr" id="dpText_FldDT" disabled="">
									<input class="form-control form-control-sm" id="dpDate_FldDT" type="hidden" v-model="row.FldDT">
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
_this.objectName = "ZzEntity";
_this.submitMethod = "Create";

_this.row = {"MyTestField":"","Note":null,"ColNum":null,"FldD":null,"FldDT":null};





_this.initialRequests.push({"Id":"MyTestField_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","Where":{"CompareClauses":[{"Name":"ParentId","Value":10000,"ClauseOperator":"Equal"}]},"OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});
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