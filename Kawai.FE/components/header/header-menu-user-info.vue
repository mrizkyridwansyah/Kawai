<template>
    <div class="navbar-item navbar-user dropdown">
        <a v-if="image" href="#" class="navbar-link dropdown-toggle d-flex align-items-center" data-bs-toggle="dropdown">
            <img :src="'data:image/png;base64,' + image" alt="" class="user-avatar" />
            <span>
                <span class="d-none d-md-inline fw-bold">{{ this.userData?.FullName }}</span>
            </span>
        </a>
        <a v-else href="#" class="navbar-link dropdown-toggle d-flex align-items-center" data-bs-toggle="dropdown">
            <img src="../../assets/img/user/user-12.jpg" alt="" />
            <span>
                <span class="d-none d-md-inline fw-bold">{{ this.userData?.FullName }}</span>
            </span>
        </a>
        <div class="dropdown-menu dropdown-menu-end me-1">
            <a class="dropdown-item" href="javascript:void(0)" @click="$bvModal.show('change-password')">Change
                Password</a>
            <div class="dropdown-divider"></div>
            <li><a class="dropdown-item" href="javascript:void(0)" @click="signOut">Sign Out</a></li>
        </div>
    </div>

</template>

<script>
export default {
    data: () => ({
        showBackdrop: false,
        inSetupWizard: true,
        workspaceList: [],
        userData: {},
        image: "",
    }),
    computed: {
        auth: function () {
            return useAuth();
        },
    },
    mounted: function () {
        this.auth.load().then(dt => this.userData = dt.data.Data);
        this.image = localStorage.getItem('UserPhoto');
    },
    methods: {
        onOver: function () {
            this.$refs["header-menu-user-info"].visible = this.showBackdrop = true;
        },
        onLeave: function () {
            this.$refs["header-menu-user-info"].visible = this.showBackdrop = false;
        },
        signOut: function () {
            clearCookies();
            localStorage.clear();
            location.href = '/auth/sign-in'
        },
    },
};
</script>

<style lang="scss">
.dropdown-user-profile {
    .dropdown-item {
        font-size: 13px;
        font-weight: 500;
        // padding: 0.35rem .55rem;
        border-radius: 5px;
    }

    a {
        text-decoration: none !important;
    }
}

.user-info-name {
    font-size: 15px;
    font-weight: 500;
}

.user-avatar {
  border-radius: 50%;      /* bulat */
  object-fit: cover;       /* ini yang penting biar nggak stretch */
  object-position: center; /* posisi tengah */
}
</style>
