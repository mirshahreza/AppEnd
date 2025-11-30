<template>
  <div :style="{ 'margin-left': (depth * 12) + 'px' }" class="property-grid-item">
    <div v-if="isObject(value)" class="pg-object">
      <div class="d-flex align-items-center mb-2 pg-header">
        <button class="btn btn-sm btn-link p-0 me-2" @click="collapsed = !collapsed"><i :class="collapsed ? 'fa-solid fa-caret-right' : 'fa-solid fa-caret-down'"></i></button>
        <div class="fw-bold text-secondary">Object</div>
      </div>
      <div v-show="!collapsed" class="pg-children">
        <div v-for="(v,k) in value" :key="k" class="mb-2">
          <div class="fw-medium text-secondary mb-1">{{k}}</div>
          <property-grid :value="value[k]" :depth="depth+1" @update:value="onChildUpdate(k,$event)" />
        </div>
      </div>
    </div>

    <div v-else-if="isArray(value)" class="pg-array">
      <div class="d-flex align-items-center mb-2 pg-header">
        <button class="btn btn-sm btn-link p-0 me-2" @click="collapsed = !collapsed"><i :class="collapsed ? 'fa-solid fa-caret-right' : 'fa-solid fa-caret-down'"></i></button>
        <div class="fw-bold text-secondary">Array ({{ value.length }})</div>
      </div>
      <div v-show="!collapsed" class="pg-children">
        <div v-for="(it,idx) in value" :key="idx" class="d-flex gap-2 align-items-start mb-1">
          <div style="flex:1">
            <property-grid :value="it" :depth="depth+1" @update:value="onArrayItemUpdate(idx,$event)" />
          </div>
          <div>
            <button class="btn btn-sm btn-danger" @click="removeArrayItem(idx)"><i class="fa-solid fa-trash"></i></button>
          </div>
        </div>
        <div class="input-group input-group-sm mt-1">
          <input v-model="newArrayValue" placeholder="new item (json or primitive)" class="form-control form-control-sm" />
          <button class="btn btn-sm btn-primary" @click="addArrayItem">Add</button>
        </div>
      </div>
    </div>

    <div v-else class="pg-primitive">
      <!-- primitive -->
      <div v-if="type==='string'">
        <input class="form-control form-control-sm" v-model="localValue" @change="emitChange" />
      </div>
      <div v-else-if="type==='number'">
        <input type="number" class="form-control form-control-sm" v-model.number="localValue" @change="emitChange" />
      </div>
      <div v-else-if="type==='boolean'">
        <select class="form-select form-select-sm" v-model="localValue" @change="emitChange">
          <option :value="true">true</option>
          <option :value="false">false</option>
        </select>
      </div>
      <div v-else>
        <input class="form-control form-control-sm" v-model="localValue" @change="emitChange" />
      </div>
    </div>
  </div>
</template>

<script>
import { ref, watch } from 'vue';

export default {
  name: 'PropertyGrid',
  props: { value: { required: true }, depth: { type: Number, default: 0 } },
  components: {},
  setup(props, ctx) {
    const localValue = ref(props.value);
    const newArrayValue = ref('');
    const collapsed = ref(false);

    function isObject(v) { return v && typeof v === 'object' && !Array.isArray(v); }
    function isArray(v) { return Array.isArray(v); }
    function getType(v) {
      if (v === null) return 'null';
      return Array.isArray(v) ? 'array' : typeof v;
    }

    watch(() => props.value, (n) => { localValue.value = n; });

    function emitChange() { ctx.emit('update:value', localValue.value); }

    function onChildUpdate(key, val) {
      if (isObject(localValue.value)) {
        // ensure reactive update
        localValue.value = { ...localValue.value, [key]: val };
        emitChange();
      }
    }
    function onArrayItemUpdate(idx, val) {
      if (isArray(localValue.value)) {
        const arr = [...localValue.value];
        arr.splice(idx, 1, val);
        localValue.value = arr;
        emitChange();
      }
    }
    function addArrayItem() {
      let txt = newArrayValue.value.trim();
      if (txt === '') return;
      let parsed = txt;
      try { parsed = JSON.parse(txt); } catch { /* keep as string */ }
      if (!isArray(localValue.value)) localValue.value = [];
      localValue.value = [...localValue.value, parsed];
      newArrayValue.value = '';
      emitChange();
    }
    function removeArrayItem(idx) {
      if (isArray(localValue.value)) {
        const arr = [...localValue.value];
        arr.splice(idx,1);
        localValue.value = arr;
        emitChange();
      }
    }

    return { localValue, newArrayValue, isObject, isArray, getType, emitChange, onChildUpdate, onArrayItemUpdate, addArrayItem, removeArrayItem, type: getType(props.value), collapsed };
  },
  created() {
    // recursive registration
    this.$options.components.PropertyGrid = this.$options;
  }
}
</script>

<style scoped>
.property-grid-item { }
.pg-object, .pg-array { border-left: 3px solid rgba(0,0,0,0.03); padding-left:10px; }
.pg-header { font-size: 0.95rem; }
.pg-children { margin-top:4px; }
.pg-primitive { margin-bottom:6px; }
</style>
