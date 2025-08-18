<template>
  <div :class="col ? ('form-group col-' + col) : 'form-group'">
    <label class="d-block">{{(label )}}</label>
    <div class="ps-1 d-flex justify-content-between"
      style="border-bottom: solid 1px rgba(0, 0, 0, .1);"
    >
      <div>
        <b-form-checkbox 
          v-model="checkedAll"
          @input="checkAll"
        >
          <small v-if="checkedAll" class="text-primary">Uncheck All</small>
          <small v-else="checkedAll" class="text-primary">Check All</small>
        </b-form-checkbox>
      </div>
      <div>
        <input type="text" placeholder="Search for..." 
          style="width: 160px;border: none;"
          v-model="keyword"
          @input="search"
        />
      </div>
    </div>
    <v-loading class="mb-2" v-if="isLoading" />
    <div class="cbl-container ps-1">
      <div class="alert alert-light text-center mt-2" v-if="filteredItems.length == 0">
        No Items
      </div>
      <b-form-checkbox
        :value="true"
        :unchecked-value="false"
        :checked="isChecked(item.Id)"
        @input="v => change(v, item, i)"
        size="md"
        v-for="(item, i) in filteredItems"
        :key="i"
      >{{item.Name}}</b-form-checkbox>
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{description}}</small>
    <small class="form-text text-muted"
      v-if="selectedValue.length > 0"
    >
      Selected Items: {{ selectedValue.length }}
    </small>
  </div>
</template>

<script>
export default {
  model: {
    prop: 'modelValue',
    event: 'update'
  },
  emits: ['update:modelValue'],
  props: [
    'modelValue', 'label', 'level',  'col', 
    'description', 'placeholder', 'onSelect', 
    'errors', 'value', 'multiline', 'rows',
    'levelParentIds', 'levelParentIdsRequired',
  ],
  data: () => ({
    tempValue: [],
    items: [],
    selectedValue: [],
    isLoading: false,
    checkedAll: false,
    keyword: null,
  }),
  computed: {
    filteredItems: function() {
      return this.items.filter(p => !this.keyword ? true : p.Name.toLowerCase()?.includes(this.keyword?.toLowerCase()));
    },
    checkedAllFiltered: function() {
      var d = this.filteredItems.length == this.filteredItems.filter(p => this.selectedValue.some(c => c == p.Id)).length;
      this.checkedAll = d;
      return d;
    }
  },
  watch: {
    modelValue: {
      deep: true,
      handler: function(after) {
        if(Array.isArray(after)) {
          this.selectedValue = objCopy(after);
        }
      }
    },
    selectedValue: function(after) {
      var d = this.filteredItems.length == this.filteredItems.filter(p => this.selectedValue.some(c => c == p.Id)).length;
      this.checkedAll = d && this.filteredItems.length > 0;
    },
    levelParentIds: function(after) {
      if(after?.length == 0) {
        this.selectedValue = [];
        this.$emit('update:modelValue', this.selectedValue)
      }
      if(this.levelParentIdsRequired === true && after?.length == 0) {
        this.items = [];
        return;
      }

      this.load();
    },
  },
  created: function() {
    if(Array.isArray(this.modelValue)) {
      // this.selectedValue = objCopy(this.modelValue);
    }
  },
  mounted: function() {
    if(Array.isArray(this.modelValue)) {
      // this.selectedValue = objCopy(this.modelValue);
    }

    if(this.levelParentIdsRequired === true && this.levelParentIds?.length == 0) {
      return;
    }
    this.load();
  },
  methods: {
    load: function() {
      console.log(this.levelParentIdsRequired, this.levelParentIds?.length)
      if(this.levelParentIdsRequired === true && (!this.levelParentIds || this.levelParentIds?.length == 0)) {
        this.items = [];
        return;
      }

      this.isLoading = true;
      var payload = {
        Length: 9999,
        Filters: [],
        Sorts: {
          Name: 'asc'
        },
      }

      payload.Filters.push(["LevelId", this.level.Id])

      if(this.levelParentIds?.length > 0)
        payload.Filters.push(["LevelParentOrganizationId", 'in', this.levelParentIds])

      this.$http.post(`/organization/list`, payload)
        .then(({data}) => {
          this.items = data.Data.Items;
          this.selectedValue = data.Data.Items.filter(p => this.selectedValue.some(c => c == p.Id)).map(p => p.Id)
          this.$emit('update:modelValue', this.selectedValue)
        })
        .finally(() => this.isLoading = false)
    },
    change: function(v, item, i) {
      var da = this.selectedValue.filter(p => p == item.Id);
      if(da.length == 0) {
        if(!v) return;
        this.selectedValue.push(item.Id)
      }
      else {
        if(v) return;
        
        var idx = this.selectedValue.indexOf(da[0]);
        this.selectedValue.splice(idx, 1);
      }

      if(this['onSelect']) {
        this['onSelect'](item);
      }
      this.$emit('update:modelValue', this.selectedValue)
    },
    isChecked: function(r) {
      return this.selectedValue?.filter(p => p == r).length > 0;
    },
    checkAll: function(v) {
      if(v) {
        var d = this.filteredItems.filter(p => !this.selectedValue.some(c => c == p.Id)).map(p => p.Id);
        d.forEach(p => this.selectedValue.push(p))
      }
      else {
        this.selectedValue = this.selectedValue.filter(p => !this.filteredItems.some(c => c.Id == p));
      }
      this.$emit('update:modelValue', this.selectedValue)
    },
    search: function(e) {
      if(!this.checkedAllFiltered) {
        this.checkedAll = false;
      }
    }
  }
}
</script>


<style lang="scss">


.cbl-container {
  max-height: 200px;
  overflow: auto;

  &::-webkit-scrollbar {
    width: 7px;
    height: 7px;
  }
  
  /* Track */
  &::-webkit-scrollbar-track {
    background: #f1f1f1;
  }
  
  /* Handle */
  &::-webkit-scrollbar-thumb {
    background: rgba(0, 0, 0, 0.2);
    border-radius: 5px;
  }
  
  /* Handle on hover */
  &::-webkit-scrollbar-thumb:hover {
    background: rgba(0, 0, 0, 0.3);
  }
}

</style>