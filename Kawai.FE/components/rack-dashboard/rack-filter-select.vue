<template>
  <div class="d-flex align-items-center gap-2">
    <div :style="styleCode" class="filter-multiselect-container">
      <Multiselect
        v-model="internalValue"
        :options="options"
        track-by="Code"
        value-prop="Code"
        label="DDLDescription"
        :searchable="false"
        :can-clear="false"
        :can-deselect="false"
        :close-on-select="true"
        open-direction="bottom"
        :placeholder="placeholder || ' '"
        :disabled="disabled"
        @select="onSelect"
        class="custom-rack-multiselect"
      >
        <template #singlelabel="{ value }">
          <div class="multiselect-single-label">
            <span class="multiselect-single-label-text">{{ selectedCodeText }}</span>
          </div>
        </template>
        <template #option="{ option }">
          <span class="multiselect-option-text">{{ option.DDLDescription }}</span>
        </template>
      </Multiselect>
    </div>
    <input
      type="text"
      disabled
      class="form-control"
      :style="styleDesc"
      :value="computedDescription"
      :placeholder="descPlaceholder || ''"
    />
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import Multiselect from '@vueform/multiselect';

const props = defineProps({
  modelValue: {
    type: [String, Number],
    default: '',
  },
  options: {
    type: Array,
    default: () => [],
  },
  selectedDesc: {
    type: String,
    default: '',
  },
  placeholder: {
    type: String,
    default: ' ',
  },
  descPlaceholder: {
    type: String,
    default: '',
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  styleCode: {
    type: String,
    default: 'width: 140px;',
  },
  styleDesc: {
    type: String,
    default: 'width: 250px;',
  },
});

const emit = defineEmits(['update:modelValue', 'change']);

const internalValue = ref('ALL');

watch(
  () => props.modelValue,
  (val) => {
    internalValue.value = (!val || val === 'ALL') ? 'ALL' : String(val);
  },
  { immediate: true }
);

const selectedItem = computed(() => {
  const current = internalValue.value;
  if (!current || current === 'ALL') {
    return props.options.find((o) => o.Code === 'ALL') || null;
  }
  return props.options.find((o) => String(o.Code) === String(current)) || null;
});

const selectedCodeText = computed(() => {
  return internalValue.value || 'ALL';
});

const computedDescription = computed(() => {
  if (props.selectedDesc) return props.selectedDesc;
  if (selectedItem.value) return selectedItem.value.Name || selectedItem.value.Code;
  return '';
});

const onSelect = (val) => {
  const code = (typeof val === 'object' && val !== null) ? val.Code : val;
  const outVal = (!code || code === 'ALL') ? '' : String(code);
  internalValue.value = (!code || code === 'ALL') ? 'ALL' : String(code);
  emit('update:modelValue', outVal);
  emit('change', outVal);
};
</script>

<style scoped>
.filter-multiselect-container {
  min-width: 0;
}

.custom-rack-multiselect {
  width: 100%;
  height: 30px !important;
  min-height: 30px !important;
  cursor: pointer;
  --ms-ring-color: rgba(13, 110, 253, 0.2);
  --ms-border-color-active: #86b7fe;
  --ms-caret-color: #64748b;
  --ms-option-bg-selected: #e2e8f0;
  --ms-option-color-selected: #1e293b;
  --ms-option-bg-selected-pointed: #cbd5e1;
  --ms-option-color-selected-pointed: #0f172a;
  --ms-option-bg-pointed: #f1f5f9;
  --ms-option-color-pointed: #1e293b;
}

/* Remove green highlight and clear icon */
:deep(.multiselect-clear) {
  display: none !important;
}

:deep(.multiselect-search) {
  display: none !important;
}

:deep(.multiselect.is-active) {
  border-color: #86b7fe !important;
  box-shadow: 0 0 0 2px rgba(13, 110, 253, 0.2) !important;
}

:deep(.multiselect-option.is-selected) {
  background-color: #e2e8f0 !important;
  color: #1e293b !important;
  font-weight: 600 !important;
}

:deep(.multiselect-option.is-selected.is-pointed) {
  background-color: #cbd5e1 !important;
  color: #0f172a !important;
}

:deep(.multiselect-option.is-pointed) {
  background-color: #f1f5f9 !important;
  color: #1e293b !important;
}

:deep(.multiselect-dropdown) {
  width: max-content !important;
  min-width: 100% !important;
  z-index: 1050 !important;
}

.multiselect-option-text {
  font-size: 9pt;
  white-space: nowrap;
}
</style>
