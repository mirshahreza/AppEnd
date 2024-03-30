<template>
<div class="card h-100 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
		<div class="card-body p-0">
			<div class="card h-100 border-light bg-primary-subtle bg-opacity-75 border-0 pt-2 border-0 rounded-0 scrollable">
				<div class="card-body fs-d8 pt-0 ps-3 pe-3 bg-transparent">
					<div class="row">
						<div class="card rounded-1 border-light mb-1">
							<div class="card-body">
								<div class="row">
									<div class="col-48" v-if="inputs.fkColumn!=='UserId'">
										<label class="fs-d9 text-muted ms-2" for="input_UserId">{{shared.translate('UserId')}}</label>
										<select class="form-select form-select-sm" v-model="row.UserId" data-ae-validation-required="true">
											<option v-for="i in shared.getResponseObjectById(initialResponses,'UserId_Lookup')" :value="i['Id']">{{i.UserName}}</option>
										</select>
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='RoleId'">
										<label class="fs-d9 text-muted ms-2" for="input_RoleId">{{shared.translate('RoleId')}}</label>
										<select class="form-select form-select-sm" v-model="row.RoleId" data-ae-validation-required="true">
											<option v-for="i in shared.getResponseObjectById(initialResponses,'RoleId_Lookup')" :value="i['Id']">{{i.RoleName}}</option>
										</select>
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
let _this = { cid: "", c: null, inputs: {}, dbConfName: "", objectName: "", submitMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], row: {}, Relations: {}, RelationsMetaData: {}, regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "AAA_Users_R_Roles";
_this.submitMethod = "Create";

_this.row = {"UserId":"","RoleId":""};





_this.initialRequests.push({"Id":"UserId_Lookup","Method":"DefaultRepo.AAA_Users.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Users.ReadList","OrderClauses":[{"Name":"UserName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}});_this.initialRequests.push({"Id":"RoleId_Lookup","Method":"DefaultRepo.AAA_Roles.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Roles.ReadList","OrderClauses":[{"Name":"RoleName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}});
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