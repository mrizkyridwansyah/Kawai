<template>
    <div class="mb-3">
        <label class="form-label">Unit Classification Code</label>
        <input-text placeholder="Unit Classification Code" v-model="model.UnitClassificationCode" :disabled="mode === 'edit'"
            :errors="errors?.UnitClassificationCode" />
    </div>
    <div class="mb-3">
        <label class="form-label">Unit Classification Name</label>
        <input-text placeholder="Unit Classification Name" v-model="model.UnitClassificationName" :errors="errors?.UnitClassificationName" />
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
            UnitClassificationCode: "",
            UnitClassificationName: "",
        },
        errorResponse: {},
        errors: {},
    }),
    computed: {
        ds: function () {
            return useUnitClassification();
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
                UnitClassificationCode: "",
                UnitClassificationName: "",
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
                    toastSuccess('Anying........ nge-sep!');
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
                    toastSuccess('Anying........ nge-sep!');
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