<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">
                <button class="btn btn-sm btn-primary" @click="setupEditMode">EnableDesign</button>
                <div class="input-group-text">Some buttons ...</div>
                <span class="input-group-text border-0 bg-transparent fs-d4"> </span>
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 bg-transparent border-0">
                <div class="card-body p-0 bg-transparent">
                    <div class="row h-100">
                        <div class="col-38 h-100 p-1">
                            <div class="h-100 rounded rounded-2 bg-white edit-area">
                                <component-loader src="qs:edt" uid="toEdit" cid="toEdit" />
                            </div>
                        </div>
                        <div class="col-10 h-100 p-1">
                            <div class="h-100 rounded rounded-2 bg-white p-2">
                                <div class="accordion accordion-flush" id="accComponents">
                                    <div class="accordion-item">
                                        <h2 class="accordion-header">
                                            <button class="accordion-button collapsed accordion-btn" type="button" data-bs-toggle="collapse" data-bs-target="#accItemHtml" aria-expanded="false" aria-controls="accItemHtml">
                                                Html Snippets
                                            </button>
                                        </h2>
                                        <div id="accItemHtml" class="accordion-collapse collapse" data-bs-parent="#accComponents">
                                            <div class="accordion-body p-1">

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-code fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Div</div>
                                                </div>

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-minus fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Line</div>
                                                </div>

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-image fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Image</div>
                                                </div>

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-anchor fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Anchor</div>
                                                </div>

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-icons fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Icon</div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                    <div class="accordion-item">
                                        <h2 class="accordion-header">
                                            <button class="accordion-button collapsed accordion-btn" type="button" data-bs-toggle="collapse" data-bs-target="#accItemBsComponents" aria-expanded="false" aria-controls="accItemBsComponents">
                                                Bootstrap Snippets
                                            </button>
                                        </h2>
                                        <div id="accItemBsComponents" class="accordion-collapse collapse" data-bs-parent="#accComponents">
                                            <div class="accordion-body p-1">

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-rectangle-xmark fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Container</div>
                                                </div>

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-xmarks-lines fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Row</div>
                                                </div>

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-table-columns fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Column</div>
                                                </div>

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-hand-pointer fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Button</div>
                                                </div>

                                                <div class="badge badge-tool-item">
                                                    <i class="fa-solid fa-2x fa-icons fa-fw"></i>
                                                    <div class="mt-2 fs-d9">Icon</div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                    <div class="accordion-item">
                                        <h2 class="accordion-header">
                                            <button class="accordion-button collapsed accordion-btn" type="button" data-bs-toggle="collapse" data-bs-target="#accItemDbComponents" aria-expanded="false" aria-controls="accItemDbComponents">
                                                DbComponents
                                            </button>
                                        </h2>
                                        <div id="accItemDbComponents" class="accordion-collapse collapse" data-bs-parent="#accComponents">
                                            <div class="accordion-body p-1">

                                                all Db Components

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</template>

<script>
    shared.setAppTitle("Control Designer");
    shared.setAppSubTitle(getQueryString("edt"));

    let _this = { cid: "", c: null, d: {} };

    export default {
        methods: {
            setupEditMode() {
                $(document).ready(function () {
                    setTimeout(function () {
                        $("#toEdit").addClass("editmode");
                    }, 100);
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.setupEditMode(); },
        props: { cid: String }
    }
</script>

