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
		<div class="card-body bg-primary-subtle-light scrollable" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">
			<div class="card rounded-1 border-light mb-1">
				<div class="card-header text-bg-light p-1">
					{{shared.translate('ContentTags')}}
				</div>
				<div class="card-body">
					<div class="row">
						<div class="col-48">
							<div class="form-control form-control-sm pb-0 data-ae-validation" data-ae-validation-required="false" data-ae-validation-rule=":=n(0)">
								<div class="form-check form-check-inline" v-for="i in shared.enum(152)">
									<input class="form-check-input" type="checkbox" v-model="Relations.CmsContentsTags" :value="i.Id" :id="i.Id+'TagId_Lookup'">
									<label class="form-check-label" :for="i.Id+'TagId_Lookup'">
										{{i.Title}}
									</label>
								</div>
							</div>
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
let _this = { cid: "", ismodal:"", c: null, templateType:"UpdateByKey", inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "" };
_this.dbConfName = "DefaultRepo";
_this.objectName = "CmsContents";
_this.submitMethod = "TagsUpdateByKey";
_this.createComponent = "";
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.CmsContents.ReadTags","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.CmsContents.ReadTags","Params":[{"Name":"Id","Value":""}]}}};



_this.RelationsMetaData['ContentTags']={"RelationName":"ContentTags","RelationTable":"CmsContentsTags","RelationPkColumn":"Id","RelationFkColumn":"ContentId","RelationType":"ManyToMany","LinkingTargetTable":"BaseInfo","LinkingColumnInManyToMany":"TagId","CreateQuery":"Create","ReadListQuery":"ReadList","UpdateByKeyQuery":"UpdateByKey","DeleteByKeyQuery":"DeleteByKey","DeleteQuery":"Delete","IsFileCentric":false,"RelationUiWidget":"CheckboxList"};




export default {
	methods: {
	},
	setup(props) {
		_this.cid = props['cid'];
		_this.ismodal = props['ismodal'];
		_this.inputs = shared["params_" + _this.cid];
		if(fixNull(getQueryString("key"),'') !== '') {
			_this.inputs = {};
			_this.inputs["key"] = getQueryString("key");
		}
	},
	data() { return _this; },
	created() { _this.c = this; assignDefaultMethods(_this); },
	mounted() { _this.c.loadBaseInfo(); _this.c.loadMasterRecord(function () { initVueComponent(_this); }); _this.c.componentFinalization(); },
	props: { cid: String, ismodal: String }
}


</script>