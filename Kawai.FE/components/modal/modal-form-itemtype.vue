<template>
    <div class="mb-3">
        <label class="form-label">Item Type Code</label>
        <input-text placeholder="Item Type Code" v-model="model.ItemTypeCode" :disabled="mode === 'edit'"
            :errors="errors?.ItemTypeCode" />
    </div>
    <div class="mb-3">
        <label class="form-label">Item Type Name</label>
        <input-text placeholder="Item Type Name" v-model="model.ItemTypeName" :errors="errors?.ItemTypeName" />
    </div>
    <div class="mt-4 mb-3">
        <v-button-submit :submit="submit" :disabled="(btnDisabled !== undefined && btnDisabled !== false)"
            :is-loading="isLoading" />
    </div>
</template>
<script>
export default {
    props: ['id', 'btnDisabled', "mode"],
    data: () => ({
        isLoading: false,
        model: {
            ItemTypeCode: "",
            ItemTypeName: "",
        },
        errorResponse: {},
        errors: {},
    }),
    computed: {
        ds: function () {
            return useItemType();
        }
    },
    mounted: function () {
        if (this.mode === "edit" && this.id) {
            this.loadDetail(this.id);
        } else if (this.mode === "add") {
            this.resetForm();
        }
    },
    watch: {
        mode: function (val) {
            if (val === "edit") {
                this.loadDetail(this.id);
            } else {
                this.resetForm();
            }
        },
    },
    methods: {
        loadDetail: function () {
            this.ds.loadDetail(this.id).then(dt => this.model = dt.Data);
        },
        resetForm: function () {
            // Kosongkan form untuk mode Add
            this.model = {
                ItemTypeCode: "",
                ItemTypeName: "",
            };
            this.errors = {}; // Reset errors
        },
        submit: function () {
            if (this.mode === "add") this.create();
            else this.update();
        },
        create: function () {
            this.ds.create(this.model)
                .then(datas => {
                    toastSuccess('Data saved successfully!');
                    this.$emit('submitted');
                })
                .catch(err => {
                    this.errors = err?.Errors;
                    toastDanger(err?.Message)
                });
        },
        update: function () {
            this.ds.update(this.model)
                .then(datas => {
                    toastSuccess('Data saved successfully!');
                    this.$emit('submitted');
                })
                .catch(err => {
                    this.errors = err?.Errors;
                    toastDanger(err?.Message)
                });
        }
    }
}
</script>