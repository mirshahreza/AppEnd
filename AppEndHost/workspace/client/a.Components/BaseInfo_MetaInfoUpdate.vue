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
					<div class="row">
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_Note">{{shared.translate('Note')}}</label>
							<div class="border border-2 rounded-2 data-ae-validation ">
								<textarea type="text" v-model="row.Note" data-ae-widget="trumbowyg" data-ae-widget-options="{    &quot;svgPath&quot;: &quot;/a..lib/Trumbowyg/ui/icons.svg&quot;}" style="display:none" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,256)" id="input_Note"></textarea>
							</div>
						</div>
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_Metadata">{{shared.translate('Metadata')}} [{{shared.getEditorName('{    "mode": "ace/mode/json"}')}}]</label>
							<div class="border border-2 rounded-2 data-ae-validation ">
								<div class="code-editor-container" data-ae-widget="editorBox" data-ae-widget-options="{    &quot;mode&quot;: &quot;ace/mode/json&quot;}" id="ace_Metadata" style="height:150px;"></div>
								<input type="hidden" v-model="row.Metadata" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,4000)">
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="card-footer p-0 bg-secondary-subtle bg-gradient border-0 rounded-0" v-if="ismodal==='true'">
			<button class="btn btn-link text-decoration-none bg-hover-light w-100 py-3 rounded-0" @click="ok" data-ae-key="ok">
				<i class="fa-solid fa-save pe-1"></i>
				<span>{{shared.translate("Save")}}</span>
			</button>
		</div>
	</div>
</template>
<script>
let _this = { cid: "", ismodal:"", c: null, templateType:"UpdateByKey", inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "", regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "BaseInfo";
_this.submitMethod = "MetaInfoUpdate";
_this.createComponent = ""; 
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.BaseInfo.ReadByKey","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.BaseInfo.ReadByKey","Params":[{"Name":"Id","Value":""}]}}};

_this.pickerRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});

_this.pickerHumanIds.push({Id:'ParentId_HumanIds',Items:["Title"]});

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
	mounted() { initVueComponent(_this); _this.c.loadMasterRecord(); _this.c.componentFinalization(); },
	props: { cid: String, ismodal: String }
}


</script>