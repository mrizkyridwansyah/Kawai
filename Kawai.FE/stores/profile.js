var app = useNuxtApp();

export const useProfile = defineStore('Profile', {
    state: () => ({
        isLoading: false,
        isLoaded: false,
        data: {},
    }),
    actions: {
        load: function () {
            this.isLoading = true;
            this.isLoaded = false;
            return new Promise((resolve, reject) => {
                app.$http.get(`/user/user-info`)
                    .then(({ data }) => {
                        this.data = data.Data;
                        this.isLoaded = true;
                        resolve(data);
                    })
                    .catch((err) => reject(err.response?.data))
                    .finally(_ => this.isLoading = false);
            })
        },
        allowed: function (featureId) {
            return true;
            if (this.data.Privileges.length == 0) return false;
            return this.data.Privileges?.some(p => p.FeatureId == featureId);
        },
        groupAllowed: function (group) {
            return true;
            if (JSON.stringify(this.data?.Privileges) == "[]") return false;
            return this.data?.Privileges?.some(p => p.FeatureGroup == group);
        },
        allowedOr: function (featureIds = []) {
            return true;
            if (JSON.stringify(this.data?.Privileges) == "[]") return false;
            return this.data.Privileges?.some(p => featureIds.some(c => c == p.FeatureId));
        },
        allowedAnd: function (featureIds = []) {
            return true;
            if (JSON.stringify(this.data?.Privileges) == "[]") return false;
            return (
                this.data?.Privileges?.filter(p =>
                    featureIds.some(c => c == p.FeatureId)
                ).length == featureIds.length
            );
        },
    },
    persist: true
});

if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(useProfile, import.meta.hot));
}