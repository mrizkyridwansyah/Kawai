<template>
    <div class="mb-3">
        <label class="form-label">Warehouse Code</label>
        <input-warehouse class="form-control" v-model="model.WarehouseCode" disabled="true" />
    </div>
    <div class="mb-3">
        <label class="form-label">Location Code</label>
        <input-text placeholder="#AUTO" disabled="true" v-model="model.LocationCode" :errors="errors.LocationCode" />
    </div>
    <div class="mb-3">
        <label class="form-label">Location Name</label>
        <input-text placeholder="Location Name" v-model="model.LocationName" :errors="errors.LocationName" />
    </div>
    <div class="mt-4 mb-3">
        <v-button-submit :submit="submit" :disabled="(btnDisabled !== undefined && btnDisabled !== false)"
            :is-loading="isLoading" />
    </div>
</template>
<script>
export default {
    props: ['id', 'warehouse', 'btnDisabled', "mode"],
    data: () => ({
        isLoading: false,
        model: {
            WarehouseCode: "",
            LocationCode: "",
            LocationName: ""
        },
        errorResponse: {},
        errors: {},
    }),
    computed: {
        ds: function () {
            return useLocation();
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
        warehouse: function (val) {
            this.model.WarehouseCode = val;
        },
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
            if (this.mode === "edit") {
                this.model.LocationCode = this.model.LocationCode; // Jangan reset LocationCode di Edit
            } else {
                // Kosongkan form untuk mode Add
                this.model = {
                    WarehouseCode: this.warehouse || "",
                    LocationCode: "", // Kosongkan LocationCode
                    LocationName: "",
                };
            }
            this.errors = {}; // Reset errors
        },
        submit: function () {
            this.model.WarehouseCode = this.warehouse;
            this.model.LocationCode = this.mode === "edit" ? this.id : "#AUTO";

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