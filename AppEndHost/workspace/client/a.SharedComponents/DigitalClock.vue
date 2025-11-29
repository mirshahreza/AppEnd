<template>
    <span class="fw-bold text-primary timeH">00</span>:<span class="fw-bold text-success timeM">00</span>:<span class="text-secondary timeS">00</span>
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

