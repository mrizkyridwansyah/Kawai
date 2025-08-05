export const useSidebar = defineStore('sidebar', {
    state: () => ({
        activeGroups: new Set(),
        activeMenus: new Set(),
        activeSubMenus: new Set()
    }),
    actions: {
        toggleGroup(group) {
            this.activeGroups.clear();
            this.activeGroups.add(group)
        },
        isGroupActive(group) {
            return this.activeGroups.has(group)
        },
        toggleMenu(menu) {
            this.activeMenus.clear();
            this.activeMenus.add(menu)
        },
        isMenuActive(menu) {
            return this.activeMenus.has(menu)
        },
        toggleSubMenu(subMenu) {
            this.activeSubMenus.clear();
            this.activeSubMenus.add(subMenu)
        },
        isSubMenuActive(subMenu) {
            return this.activeSubMenus.has(subMenu)
        },
    },
    persist: true // ⬅️ Optional: tambahkan agar tetap tersimpan di localStorage
})


if (import.meta.hot) {
    import.meta.hot.accept(acceptHMRUpdate(useSidebar, import.meta.hot));
}