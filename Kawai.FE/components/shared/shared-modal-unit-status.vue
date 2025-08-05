<template>
    <div class="row">
      <table class="ml-3" style="width: 50%;">
        <tr>
          <td>Status</td>
          <td>:</td>
          <td><input-radio :options="[
          { text: 'Available', value: 'AVAILABLE' },
          { text: 'Hold', value: 'HOLD' }
        ]" v-model="newStatus" text-field="text" value-field="value" /></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <button class="btn btn-sm btn-primary" @click="() => actions(this.newStatus)">Save</button>
            </td>
        </tr>
      </table>
    </div>
</template>
<script>
export default {
    props: ['status', 'actions'],
    data: () => ({
        newStatus: ""
    }),
    mounted: function () {
        this.newStatus = this.status
    },}
</script>