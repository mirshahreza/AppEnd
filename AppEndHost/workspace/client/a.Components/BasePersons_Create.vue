<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0" v-if="ismodal!=='true'">
			<div class="hstack gap-1">
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
					<div class="col-48">
						<div class="row">
							<div class="col"></div>
							<div class="col-12">
								<table class="w-100">
									<tbody>
										<tr>
											<td></td>
											<td style="width:100px;" class="py-3">
												<div style="height:100px;width:100px;">
													<div data-ae-widget="aeFileField" data-ae-widget-options="{&quot;accept&quot;:&quot;image/x-png,image/gif,image/jpeg&quot;,&quot;resize&quot;:true,&quot;resizeMaxWidth&quot;:950,&quot;resizeMaxHeight&quot;:950,&quot;maxSize&quot;:800000}" class="ae-file-field w-100 h-100 border border-2 rounded-circle pointer data-ae-validation ">
														<input type="hidden" class="FileBody" v-model="row['Picture_FileBody']" data-ae-validation-required="false">
														<input type="hidden" class="FileName" v-model="row['Picture_FileName']">
														<input type="hidden" class="FileSize" v-model="row['Picture_FileSize']">
														<input type="hidden" class="FileMime" v-model="row['Picture_FileMime']">
													</div>
												</div>
											</td>
											<td></td>
										</tr>
									</tbody>
								</table>
							</div>
							<div class="col"></div>
						</div>
					</div>
				</div>
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='FirstName'">
							<label class="fs-d8 text-muted ms-2" for="input_FirstName">{{shared.translate('FirstName')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_FirstName" v-model="row.FirstName" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,64)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='LastName'">
							<label class="fs-d8 text-muted ms-2" for="input_LastName">{{shared.translate('LastName')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_LastName" v-model="row.LastName" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,64)">
						</div>
					</div>
				</div>
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='UserId'">
							<label class="fs-d8 text-muted ms-2" for="input_UserId">{{shared.translate('UserId')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_UserId" v-model="row.UserId" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,2147483647)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='GenderId'">
							<label class="fs-d8 text-muted ms-2" for="input_GenderId">{{shared.translate('GenderId')}}</label>
							<select class="form-select form-select-sm" v-model="row.GenderId" data-ae-validation-required="true">
								<option v-for="i in shared.enum(10000)" :value="i['Id']">{{i.Title}}</option>
							</select>
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='NationalCode'">
							<label class="fs-d8 text-muted ms-2" for="input_NationalCode">{{shared.translate('NationalCode')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_NationalCode" v-model="row.NationalCode" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,16)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='BirthYear'">
							<label class="fs-d8 text-muted ms-2" for="input_BirthYear">{{shared.translate('BirthYear')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_BirthYear" v-model="row.BirthYear" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,2147483647)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='BirthMonth'">
							<label class="fs-d8 text-muted ms-2" for="input_BirthMonth">{{shared.translate('BirthMonth')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_BirthMonth" v-model="row.BirthMonth" data-ae-validation-required="false" data-ae-validation-rule="">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='BirthDay'">
							<label class="fs-d8 text-muted ms-2" for="input_BirthDay">{{shared.translate('BirthDay')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_BirthDay" v-model="row.BirthDay" data-ae-validation-required="false" data-ae-validation-rule="">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='Mobile'">
							<label class="fs-d8 text-muted ms-2" for="input_Mobile">{{shared.translate('Mobile')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Mobile" v-model="row.Mobile" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,14)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='EntityTypeId'">
							<label class="fs-d8 text-muted ms-2" for="input_EntityTypeId">{{shared.translate('EntityTypeId')}}</label>
							<select class="form-select form-select-sm" v-model="row.EntityTypeId" data-ae-validation-required="false">
								<option value="">-</option>
								<option v-for="i in shared.enum(10010)" :value="i['Id']">{{i.Title}}</option>
							</select>
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
_this.objectName = "BasePersons";
_this.submitMethod = "Create";

_this.row = {"UserId":null,"GenderId":"","NationalCode":null,"FirstName":null,"LastName":null,"BirthYear":null,"BirthMonth":null,"BirthDay":null,"Mobile":null,"Picture_FileBody":null,"Picture_FileName":null,"Picture_FileSize":null,"Picture_FileMime":null,"EntityTypeId":""};






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
	mounted() { initVueComponent(_this); _this.c.loadBaseInfo(); _this.c.componentFinalization(); },
	props: { cid: String, ismodal: String }
}

</script>