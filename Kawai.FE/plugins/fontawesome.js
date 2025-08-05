// plugins/fontawesome.ts
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

// ⬇️ Import seluruh solid icon pack
import { fas } from '@fortawesome/free-solid-svg-icons'

library.add(fas) // ⬅️ Tambahkan seluruh icon solid ke library

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.component('font-awesome-icon', FontAwesomeIcon)
})
