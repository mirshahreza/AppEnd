<template>
    <div class="card bg-transparent rounded-0 border-0">
        <div class="card-body p-1">
            <div class="card border-light bg-transparent bg-opacity-75 border-0">
                <div class="card-body rounded rounded-1 border border-3 border-light fs-d9 p-2 bg-transparent">
                    <div class="container-fluid">
                        <div class="row fs-1d5">
                            <div class="col-48 text-center fs-1d5 font-monospace">
                                <span class="fw-bold text-primary timeH">00</span>:<span class="fw-bold text-success timeM">00</span>:<span class="text-secondary timeS">00</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

</template>

<script>
    let _this = { cid: "", c: null, alloweds: [] };

    export default {
        methods: {
            start() {
                $(document).ready(function () {
                    let timeH = $(".timeH");
                    let timeM = $(".timeM");
                    let timeS = $(".timeS");
                    setInterval(setTime, 1000);
                    setTime();
                    function setTime() {
                        const d = new Date();
                        timeH.text(fix2Char(d.getHours()));
                        timeM.text(fix2Char(d.getMinutes()));
                        timeS.text(fix2Char(d.getSeconds()));
                    }
                });
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.start(); },
        props: { cid: String }
    }

</script>

