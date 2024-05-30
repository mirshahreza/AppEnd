<template>
    <component :is="comp" :cid="cid" :id="uid" :ismodal="ismodal" />
</template>

<script>
    let _this = { cid: "", uid: "", ismodal: "", c: null };
    let ismodal = "false";
    import { shallowRef } from "vue";
    export default {
        setup(props) {
            const comp = shallowRef();
            let src = props['src'];
            _this.uid = fixNull(props['uid'], genUN('c_'));
            _this.ismodal = props['ismodal'];
            ismodal = props['ismodal'];
            if (src.startsWith('qs:')) src = getQueryString(src.replace('qs:', ''));
            src = fixEndBy(fixNull(src, shared.getAppConfig().defaultComponent), ".vue");
            comp.value = loadVM(src);
            return { comp };
        },
        created() { _this.c = this; _this.cid = _this.uid; },
        mounted() {
            if (fixNull(_this.uid, '') !== '') {
                $(document).ready(function () {
                    let testDone = function () {
                        if (ismodal === "true") {
                            if ($("#" + _this.uid).attr("ae-data-ready") === "true") {
                                clearInterval(testInterval);
                                initVueComponent(_this);
                            }
                        } else {
                            clearInterval(testInterval);
                            initVueComponent(_this);
                        }
                    };
                    let testInterval = setInterval(testDone, 50);
                });
            }
        },
        props: { src: String, uid: String, ismodal: String, cid: String }
    }
</script>