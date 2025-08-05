<template>
  <div class="search-menu-wrapper" @click.outside="hideSuggestions">
    <input
      type="text"
      class="form-control"
      :placeholder="placeholder"
      v-model="searchTerm"
      @input="handleInput"
      @focus="showSuggestions = true"
      @blur="hideSuggestions"
    />
    <ul v-if="showSuggestions && filteredOptions.length" class="suggestions">
      <li
        v-for="(option, index) in filteredOptions"
        :key="index"
        @mousedown.prevent="selectOption(option)"
      >
        {{ option.MenuDescription }}
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: "SearchMenu",
  props: {
    placeholder: {
      type: String,
      default: "Search Menu...",
    },
  },
  emits: ["select"],
  data: () => ({
    searchTerm: "",
    showSuggestions: false,
    options: [],
  }),
  computed: {
    ds: function () {
      return useMenu();
    },
    filteredOptions: function () {
      const term = this.searchTerm.toLowerCase();
      return this.options.filter((opt) =>
        opt.MenuDescription.toLowerCase().includes(term)
      );
    },
  },
  //   watch: {
  //     searchTerm: function (val) {
  //       if (!val) {
  //         this.showSuggestions = false;
  //       }
  //     },
  //   },
  mounted: function () {
    this.ds.privileges().then((dt) => (this.options = dt.Data));
  },
  methods: {
    handleInput: function () {
      this.showSuggestions = true;
    },
    selectOption: function (option) {
      this.searchTerm = "";
      this.showSuggestions = false;
      this.$emit("select", option);
      if (option.MenuName) {
        this.$router.push("/app" + option.MenuName);
      }
    },
    hideSuggestions: function () {
      //   setTimeout(() => {
      this.showSuggestions = false;
      //   }, 300);
    },
  },
  directives: {
    outside: {
      mounted: function (el, binding) {
        el.clickOutsideEvent = function (event) {
          if (!(el === event.target || el.contains(event.target))) {
            binding.value(event);
          }
        };
        document.addEventListener("click", el.clickOutsideEvent);
      },
      unmounted: function (el) {
        document.removeEventListener("click", el.clickOutsideEvent);
      },
    },
  },
};
</script>

<style scoped>
.search-menu-wrapper {
  position: relative;
  max-width: 300px;
  width: 100%;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}

input.form-control {
  padding: 0.6em 1em;
  width: 100%;
  border: 1px solid #ccc;
  border-radius: 8px;
  transition: border-color 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

input.form-control:focus {
  outline: none;
  border-color: #4d90fe;
  box-shadow: 0 0 0 2px rgba(77, 144, 254, 0.2);
}

.suggestions {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 1px solid #ccc;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  list-style: none;
  margin: 0.5em 0 0;
  padding: 0.3em 0;
  max-height: 250px;
  overflow-y: auto;
}

.suggestions li {
  padding: 0.7em 1em;
  cursor: pointer;
  transition: background-color 0.2s ease;
  font-size: 14px;
}

.suggestions li:hover {
  background-color: #f0f4ff;
  font-weight: 500;
}
</style>
