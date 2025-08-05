<template>
        <div>
            <table class="x-table mt-3 w-100" v-if="!ds.isLoading && !ds.isNetworkError && !ds.isServerError">
                <thead>
                    <tr>
                        <th style="width: 50px;">#</th>
                        <th>NUP</th>
                        <th>Nama</th>
                        <th>Alamat</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, i) in ds.data.Items" :key="i">
                        <td>{{ i + 1 }}.</td>
                        <td>
                            <span v-for="(x, i) in (actions || [])">
                                <span v-if="x.separator === true"> | </span>
                                <nuxt-link @click="() => x?.event(item)" v-else-if="x.to">
                                    {{ item.FormattedId }}
                                </nuxt-link>
                                <a :href="item.href || 'javascript:void(0)'" @click="() => x?.event(item)" v-else>
                                    {{ item.FormattedId }}
                                </a>
                            </span>
                        </td>
                        <td>{{ item.CustomerName }} ({{item.CustomerFormattedId}})</td>
                        <td>
                            {{ item.AddressIdentity }}
                            <br>
                            {{ item.AddressRTRWIdentity }}
                            <br>
                            {{ item.AddressDistrictIdentity }}
                            <br>
                            {{ item.AddressTownIdentity }}
                        </td>
                    </tr>
                </tbody>
            </table>

            <v-loading-2 class="m-5 p-5" v-if="ds.isLoading" />
            <div>
                <v-table-pagination
                    v-if="!ds.isLoading && ds.data.Items.length > 0 && !ds.isNetworkError && !ds.isServerError"
                    class="mt-3" :table="ds.data" :page-change="ds.setPage" :length-change="ds.setLength" />
                <v-data-empty class="mt-3"
                    v-if="!ds.isLoading && ds.data.Items.length == 0 && !ds.isNetworkError && !ds.isServerError" />
                <v-error-server class="mt-3" v-if="!ds.isLoading && ds.isServerError" :refresh="ds.load" />
                <v-error-network class="mt-3" v-if="!ds.isLoading && ds.isNetworkError" :refresh="ds.load" />
            </div>
        </div>
</template>

<script>
export default {
    props: ['data', 'actions', 'list', 'company', 'project', 'isActive'],
    data: () => ({
        model: {
            Code: null,
            Name: null,
            Description: null,
        },
        filter: {
            sorts: {
                FormattedId: "asc",
            },
        },
        errorResponse: {},
        errors: {},
    }),
    computed: {
        ds: function () {
            return this.list;
        }
    },
    mounted: function () {
        var filter = [];
        filter.push(["CompanyId", '=', this.company]);
        filter.push(["ProjectId", '=', this.project]);

        if(this.isActive)
            filter.push(["StatusNUP", '=', 'ACTIVE']);

        this.ds.setSort(this.filter.sorts);
        this.ds.setFilter(filter);
        this.ds.load();
    },
    methods: {
        submit: function (e) {

        },
        shown: function () {
            // this.errors = this.errorResponse = {};
        },
        hidden: function () {
            // this.errors = this.errorResponse = {};
        },
    }
}
</script>