<template>
    <div class="mb-3">
        <label class="form-label">Supplier Code</label>
        <input-trade v-model="model.SupplierCode" disabled="true" />
    </div>
    <div class="mb-3">
        <label class="form-label">Item Code</label>
        <input-item v-model="model.ItemCode" :errors="errors?.ItemCode" />
    </div>
    <div class="mb-3">
        <label class="form-label">Qty Packing</label>
        <input-money placeholder="Qty Packing" v-model="model.QtyPacking" :errors="errors?.QtyPacking" />
    </div>
    <div class="mt-4 mb-3">
        <v-button-submit-modal :submit="submit" :disabled="(btnDisabled !== undefined && btnDisabled !== false)"
            :is-loading="isLoading" />
    </div>
</template>
<script>
export default {
    props: ['id', 'supplier', 'btnDisabled', "mode"],
    data: () => ({
        isLoading: false,
        model: {
            SupplierCode: "",
            ItemCode: "",
            QtyPacking: 0
        },
        errorResponse: {},
        errors: {},
    }),
    computed: {
        ds: function () {
            return useItemPackingSupplier();
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
        supplier: function (val) {
            this.model.SupplierCode = val;
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
            this.ds.loadDetail(this.supplier, this.id).then(dt => this.model = dt.Data);
        },
        resetForm: function () {
            if (this.mode === "edit") {
                this.model.ItemCode = this.model.ItemCode; // Jangan reset ItemCode di Edit
            } else {
                // Kosongkan form untuk mode Add
                this.model = {
                    SupplierCode: this.supplier || "",
                    ItemCode: "",
                    QtyPacking: 0
                };
            }
            this.errors = {}; // Reset errors
        },
        submit: function () {
            this.model.SupplierCode = this.supplier;
            // this.model.ItemCode = this.mode === "edit" ? this.id : "";

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