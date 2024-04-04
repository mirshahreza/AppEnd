<template>
    <div class="row">
        <div class="col-6">
            <div class="card rounded-0 border-0 bg-transparent">

                <div class="card-header border-0 rounded-0 text-center bg-transparent d-none d-md-block d-lg-block">
                    <img src="/..lib/images/AppEnd-Logo-Full.png" @click="shared.openComponentByEl($event);"
                         data-ae-src="/.PublicComponents/baseAbout.vue" data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"draggable":false,"closeByOverlay":true}'
                         class="animate__animated animate__slideInDown shadow shadow-sm border border-0 rounded rounded-2 pointer mt-3" style="width:75%;" />
                </div>

                <div class="card-header border-0 rounded-0 text-center bg-transparent p-2 d-sm-block d-md-none d-lg-none">
                    <img src="/..lib/images/AppEnd-Logo-Only.png" @click="shared.openComponentByEl($event);"
                         data-ae-src="/.PublicComponents/baseAbout.vue" data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"draggable":false,"closeByOverlay":true}'
                         class="animate__animated animate__slideInDown border border-o rounded rounded-1 pointer w-100" />
                </div>

                <div class="card-body p-0 bg-transparent">
                    <component-loader src="/.SharedComponents/SideMenu" uid="sideMenu" />
                </div>

            </div>
        </div>
        <div class="col-42">
            <div class="card rounded-0 border-0">
                <div class="card-header border-0">
                    <div class="input-group input-group-sm border-0">
                        <div style="padding-top:3px;" class="d-sm-block d-md-none d-lg-none">
                            <span class="fw-bold text-secondary px-2 shadow5 fs-d8 app-title"></span>
                            <span class="fw-bolder text-success px-2 shadow5 fs-d8 app-subtitle"></span>
                            <span class="px-1 fs-d7" id="appMessage"></span>
                        </div>
                        <div style="padding-top:2px;" class="d-none d-md-block d-lg-block">
                            <span class="fw-bold text-secondary px-2 shadow5 fs-1d1 app-title"></span>
                            <span class="fw-bolder text-success px-2 shadow5 fs-1d1 app-subtitle"></span>
                            <span class="px-1 fs-d9" id="appMessage"></span>
                        </div>
                        <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                        
                    </div>
                </div>
                <div class="card-body p-0">
                    <component-loader src="qs:c" uid="dynamicContent" cid="dynamicContent" />
                </div>
            </div>
        </div>
    </div>
</template>
