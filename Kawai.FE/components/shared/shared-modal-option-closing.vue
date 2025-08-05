<template>
    <div class="row">
        <div class="col-sm-12" style="display: flex; justify-content: center;">
            <button v-if="profile.allowed(`marketing-order-create`)" class="btn btn-sm btn-primary ms-4 mr-4"
                style="width: 30em" @click="() => actions('1')">Closing
                Langsung</button>
            <button v-if="profile.allowed(`marketing-reservation-create`)" class="btn btn-sm btn-primary mr-4"
                style="width: 30em" @click="() => actions('2')">SKP</button>
            <button v-if="profile.allowed(`marketing-unit-booking-create`)" class="btn btn-sm btn-primary"
                style="width: 30em" @click="() => actions('3')">Unit Booking</button>
        </div>
    </div>
</template>
<script>
export default {
    props: ['data', 'actions'],
    computed: {
        profile: function () {
            return useProfile()
        },
    },
}
</script>