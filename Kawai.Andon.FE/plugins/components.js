import '@vueform/multiselect/themes/default.css'
// import 'v-calendar/dist/style.css';

import VueApexCharts from "vue3-apexcharts";
// import VCalendar from 'v-calendar';
// import feather from 'vue-icon'

// import { autoAnimatePlugin } from '@formkit/auto-animate/vue'

export default defineNuxtPlugin(nuxtApp => {
  nuxtApp.vueApp.use(VueApexCharts)
  // nuxtApp.vueApp.use(VCalendar)
  // nuxtApp.vueApp.use(autoAnimatePlugin)
  // nuxtApp.vueApp.use(feather, 'v-icon')
})