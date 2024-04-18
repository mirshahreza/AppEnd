<template>
    <div class="col-48 p-2">

        <div class="px-2 fw-bold fs-1d1">MyShortcuts</div>
        <div class="bg-primary-subtle fs-d5 p-1"> </div>
        <div v-on:drop="onDrop" v-on:dragover="allowDrop" class="p-2">
            <div v-if="shortcuts.length===0">
                Drag menu Items here here
            </div>
            <div v-if="shortcuts.length>0">
                <a v-for="mi in shortcuts" :href="'/'+themeName+'/?'+mi.href" class="badge text-center text-decoration-none p-3 border border-1 mx-1 text-secondary text-hover-primary position-relative">
                    <i :class="mi.icon+' fa-4x'"></i>
                    <div class="mt-2">
                        {{mi.title}}
                    </div>
                    <i class="fa-solid fa-times position-absolute text-secondary text-hover-danger" style="top:5px;right:5px;" @click="removeItem"></i>
                </a>
            </div>
        </div>

    </div>
</template>


<script>
    let _this = { cid: "", c: null, shortcuts: [], themeName: "" };
    _this.themeName = getThemeName();
    export default {
        methods: {
            removeItem(event) {
                let ind = $(event.target).parent().index();
                _this.c.shortcuts.splice(ind, 1);
                setUserShortcuts(_this.c.shortcuts);
                event.preventDefault(); 
            },
            onDrop(event) {
                var mi = event.dataTransfer.getData("menu-item");
                _this.c.shortcuts.push(JSON.parse(mi));
                setUserShortcuts(_this.c.shortcuts);
            },
            allowDrop(event) {
                event.preventDefault();
            },
            loadUserMenu() {
                _this.c.shortcuts = getUserShortcuts();
            }
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.loadUserMenu(); },
        props: { cid: String }
    }
</script>


