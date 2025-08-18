<template>
  <div v-if="noGroup !== undefined">
    <div class="spin-button">
      <div class="input-group">
        <button class="btn btn-light" type="button" @click="decrease" :disabled="disabledMin || disabled">
          <v-icon name="minus" 
            :width="17"
            weight="3"
            style="color: #555"
          />
        </button>
        <input type="text" class="form-control text-center" 
          v-model="tempValue"
          :disabled="disabledInput || disabled"
        >
        <button class="btn btn-light" type="button" @click="increase" :disabled="disabledMax || disabled">
          <v-icon name="plus" 
            :width="17"
            weight="3"
            style="color: #555"
          />
        </button>
      </div>
    </div>
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
    <label class="d-block"> 
      <div v-if="checkbox !== undefined">
        <div class="form-check">
          <input class="form-check-input" type="checkbox" @input="onCheck" :checked="checked">
          <label class="form-check-label">
            {{(label)}}
          </label>
        </div>
      </div>
      <div v-else>{{(label)}}</div>
    </label>
    <div>
      <div class="spin-button">
        <div class="input-group">
          <button class="btn btn-light" type="button" @click="decrease" :disabled="disabledMin || disabled">
            <v-icon name="minus" 
              :width="17"
              weight="3"
              style="color: #555"
            />
          </button>
          <input type="text" class="form-control text-center" 
            v-model="tempValue"
            :disabled="disabledInput || disabled"
          >
          <button class="btn btn-light" type="button" @click="increase" :disabled="disabledMax || disabled">
            <v-icon name="plus" 
              :width="17"
              weight="3"
              style="color: #555"
            />
          </button>
        </div>
      </div>
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
</template>

<script>
export default {
  // key: uniqueId(),
  model: {
    prop: 'modelValue',
    event: 'update',
  },
  emits: ['update:modelValue', 'update:checkbox'],
  props: [
    'modelValue', 'label', 'col', 'description'
    , 'placeholder', 'onSelect', 'errors', 'value'
    , 'multiline', 'rows', 'no-group', 'disabled'
    , 'min', 'max'
    , 'checkbox'
  ],
  data: () => ({
    tempValue: null,
    key: uniqueId(),
    checked: false,
  }),
  computed: {
    disabledInput: function() {
      if(this.checkbox !== undefined)
        return !this.checkbox;
      
      return false;
    },
    disabledMin: function() {
      if(this.checkbox !== undefined)
        return !this.checkbox;

      return this.min >= this.tempValue;
    },
    disabledMax: function() {
      if(this.checkbox !== undefined)
        return !this.checkbox;

      return this.max <= this.tempValue;
    },
  },
  watch: {
    modelValue: function(after, before) {
      this.tempValue = after;
    },
    checkbox: function(after) {
      console.log('checkbox',after)
      this.checked = after;
    }
  },
  mounted: function() {
    if(this.modelValue === 0 )
      this.tempValue = 0;
    else
      this.tempValue = structuredClone(this.modelValue)

    if(this.checkbox === true) 
      this.checked = true;
  },
  methods: {
    change: function(e) {
      this.$emit("update:modelValue", e.target.value);
    },
    onCheck: function(e) {
      this.tempValue = 0;
      this.$emit("update:modelValue", 0);
      this.$emit("update:checkbox", e.target.checked);
    },
    increase: function() {
      if(isNaN(this.tempValue))
        this.tempValue = this.min-1 ?? 0;

      if(!isNaN(this.max))
        if(this.max <= this.tempValue)
          return;

      this.tempValue++;
      this.$emit("update:modelValue", this.tempValue);
    },
    decrease: function() {
      if(isNaN(this.tempValue))
        this.tempValue = this.min-1 ?? 0;

      if(!isNaN(this.min))
        if(this.min >= this.tempValue)
          return;

      this.tempValue--;
      this.$emit("update:modelValue", this.tempValue);
    },
  },
}
</script>

<style lang="scss">
.spin-button {
  .btn {
    border: solid 1px rgba($color: #000000, $alpha: .1);
  }
}
</style>