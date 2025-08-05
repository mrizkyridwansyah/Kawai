<template>
  <div v-if="noGroup !== undefined">
    <div class="input-group" :class="errors ? 'is-invalid' : null">
      <date-picker
        class="form-control"
        :class="errors ? 'is-invalid' : null"
        v-model="tempValue"
        value-zone="local"
        zone="local"
        type="year"
        format="yyyy"
        :input-id="key"
        v-model:open-pop-up="isOpen"
        :flow="['year']"
        mask="####"
      />
      <span class="input-group-text btn btn-primary" @click="open">
        <v-icon name="calendar" width="16" />
      </span>
    </div>
    <!-- <v-date-picker v-model="tempValue" mode="date" /> -->
    <!-- <b-popover 
      v-if="errors" 
      :target="`input-text-${key}`" 
      triggers="hover" 
      placement="top"
      variant="danger"
    >
      <template #title><span class="text-danger">Invalid</span></template>
      {{ errors[0] }}
    </b-popover> -->
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
  <div v-else :class="col ? ('form-group col-' + col) : 'form-group'">
    <label class="d-block">{{(label)}}</label>
    <div class="input-group" :class="errors ? 'is-invalid' : null">
      <date-picker 
        class="form-control"
        :class="errors ? 'is-invalid' : null"
        v-model="tempValue"
        value-zone="local"
        type="year"
        zone="local"
        format="yyyy"
        :input-id="key"
        v-model:open-pop-up="isOpen"
        placeholder="yyyy"
        :flow="['year']"
        mask="####"
      />
      <span class="input-group-text btn btn-primary" @click="open">
        <v-icon name="calendar" width="16" />
      </span>
    </div>
    <!-- {{ tempValue }} {{ new Date(tempValue).getTime() }} {{ new Date(tempValue) }} -->
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
</template>

<script>
import moment from 'moment';
import DatePicker from './datetime/v-datetime.vue'
export default {
  // key: uniqueId(),
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue'],
  props: ['modelValue', 'label', 'col', 'description', 'placeholder', 'onSelect', 'errors', 'value', 'multiline', 'rows', 'no-group', 'disabled'],
  components: { DatePicker, },
  data: () => ({
    tempValue: null,
    key: uniqueId(),
    isOpen: false,
  }),
  watch: {
    modelValue: function(after, before) {
      if(after === null) {
        this.tempValue = null;
        return;
      }
      var d = new Date(after);
      this.tempValue = after == null ? null : moment(d).toISOString();
    },
    tempValue: function(after, before) {
      if(JSON.stringify(after) == JSON.stringify(before)) 
        return;
      
      if(after === null) {
        this.$emit("update:modelValue", null);
        return;
      }
        
      var d = new Date(after).getTime();
      this.$emit("update:modelValue", d);
    }
  },
  mounted: function() {
    if(this.modelValue) {
      var d = new Date(this.modelValue);
      this.tempValue = moment(d).toISOString();
    }
  },
  methods: {
    change: function(e) {
      this.$emit("update:modelValue", e.target.value);
    },
    open: function() {
      this.isOpen = true;
      // var c = document.getElementById(this.key)
      // c.blur();
    },
  },
}
</script>