<template>
    <div>
        <table class="x-table mt-3 w-100" v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError">
            <thead>
                <tr>
                    <th style="width: 50px;"></th>
                    <th>No. Stock</th>
                    <th>No. Unit</th>
                    <th>Unit Type</th>
                    <th>Luas Tanah</th>
                    <th>Luas Bangunan</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, i) in ds.data.Items" :key="i">
                    <td>
                        <div class="form-check" v-if="(item.NUPSelected < 3 || selected.some(s => s.Id === item.Id))">
                            <input class="form-check-input" type="checkbox" @input="e => check(e, item)"
                                :checked="selected.filter(p => p.Id == item.Id).length > 0">
                        </div>
                    </td>
                    <td>{{ item.FormattedId }}</td>
                    <td>{{ item.Name }}</td>
                    <td>{{ item.TypeName }}</td>
                    <td class="text-right">{{ item.LandArea }}</td>
                    <td class="text-right">{{ item.BuildingArea }}</td>
                </tr>
            </tbody>
            <tfoot>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td><button class="btn btn-sm btn-primary" @click="() => actions(this.selected)">Select</button>
                    </td>
                </tr>
            </tfoot>
        </table>

        <v-loading-2 class="m-5 p-5" v-if="ds.isLoading" />
        <div>
            <v-table-pagination
                v-if="!ds.isLoading && ds.data.Items.length > 0 && !ds.isNetworkError && !ds.isServerError" class="mt-3"
                :table="ds.data" :page-change="ds.setPage" :length-change="ds.setLength" />
            <v-data-empty class="mt-3"
                v-if="!ds.isLoading && ds.data.Items.length == 0 && !ds.isNetworkError && !ds.isServerError" />
            <v-error-server class="mt-3" v-if="!ds.isLoading && ds.isServerError" :refresh="ds.load" />
            <v-error-network class="mt-3" v-if="!ds.isLoading && ds.isNetworkError" :refresh="ds.load" />
        </div>
    </div>
</template>

<script>
export default {
    props: ['data', 'actions', 'list', "selected", "companyId", "projectId", "filters"],
    data: () => ({
        model: [],
        filter: {
            sorts: {
                NUPSelected: "asc",
                FormattedId: "asc",
            },
        },
        errorResponse: {},
        errors: {},
        filteredItems: []
    }),
    computed: {
        ds: function () {
            return this.list;
        }
    },
    mounted: function () {
        this.ds.setSort(this.filter.sorts);
        let filters = [
            ["CompanyId", "=", this.companyId], 
            ["ProjectId", "=", this.projectId], 
            ["Status", '=', `AVAILABLE`]
        ];
        
        // if (this.mode !== "edit") {
        //     filters.push(["NUPSelected", "<", 3]);
        // }
        
        this.ds.setFilter(filters);
        this.ds.load().then(dt => {
            this.filteredItems = dt.Data.Items;
        });
    },
    methods: {
        check: function (e, v) {
            let selectedItems = [...this.selected];
            if (!e.target.checked)
                selectedItems = selectedItems.filter(p => p.Id != v.Id);
            else {
                if (selectedItems.length >= 3) {
                    console.log(selectedItems, selectedItems.length);
                    e.target.checked = false;
                    return;
                };

                selectedItems.push({ Id: v.Id, FormattedId: v.FormattedId, Name: v.Name });
            }
            this.$emit('update:selected', selectedItems);
        },
    }
}
</script>