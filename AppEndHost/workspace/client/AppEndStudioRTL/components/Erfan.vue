<template>
    <div class="btn btn-primary pointer" @click="test">This is Erfan Component :)</div>
</template>

<script>
    shared.setAppTitle(`$auto$`);
    let _this = { cid: "", c: null };

    export default {
        methods: {
            test() {

                showConfirm({
                    title: "Test", message1: "By this action AppEnd will rebuild a new assembly from c# codes.", message2: "It is not danger, be relax and do it",
                    callback: function () {

                        alert("We r doing that function.")


                    }
                });

            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String }
    }
</script>