<template>
    <div class="card h-100 border-0 rounded-0 bg-transparent">
        <div class="card-header bg-transparent p-0">
            <img src="assets/AppEnd-Logo-Full.png" class="w-100" />
        </div>
        <div class="card-body p-2 bg-transparent scrollable">
            <comp-loader src="/a.SharedComponents/SideMenu" uid="mySide" />
        </div>
        <div class="card-footer fs-d6 text-center border-0">
            @copywrite 2024
        </div>
    </div>
</template>


<script>
    let _this = { cid: "", c: null, inputs: {}, local: {} };
    export default {
        methods: {
        },
        data() { return _this; },
        mounted() {}
    }
</script>


