<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <!--<div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">

            <div class="input-group input-group-sm border-0 bg-transparent">

                <div>aaaaaaa</div>


                <span class="input-group-text border-0 bg-transparent fs-d4"> </span>

                

            </div>


        </div>-->
        <div class="card-body p-2">
            <div class="card h-100 border-0">
                <div class="card-header p-0 px-2">
                    <ul class="nav nav-underline fs-d8 my-0">
                        <li class="nav-item">
                            <!--<a class="nav-link py-1 active" aria-current="page" href="#">Design</a>-->
                            <button class="nav-link py-1 active" style="min-width:100px;" id="designView" data-bs-toggle="tab" data-bs-target="#tpDesignView" role="tab" aria-controls="tpDesignView" aria-selected="true">Design</button>

                        </li>
                        <li class="nav-item">
                            <button class="nav-link py-1" style="min-width:100px;" id="codeView" data-bs-toggle="tab" data-bs-target="#tpCodeView" role="tab" aria-controls="tpCodeView" aria-selected="true">Code</button>
                            <!--<a class="nav-link py-1" href="#">Code</a>-->
                        </li>
                    </ul>
                </div>
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent">
                    <div class="tab-content h-100" id="myTabContent">
                        <div class="tab-pane h-100 scrollable fade show active" id="tpDesignView" role="tabpanel" aria-labelledby="designView" tabindex="0">

                            design view<br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />
                            design view<br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />
                            design view<br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />
                            design view<br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />
                            design view<br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />
                            design view<br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />
                            design view<br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />

                        </div>
                        <div class="tab-pane h-100 scrollable fade" id="tpCodeView" role="tabpanel" aria-labelledby="codeView" tabindex="0">

                            code view

                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

</template>

<script>
    shared.setAppTitle("Control Designer");

    let _this = { cid: "", c: null, d: {} };

    export default {
        methods: {
            loadControl() {
                
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.loadControl(); },
        props: { cid: String }
    }
</script>

