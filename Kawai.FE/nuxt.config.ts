// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  ssr: false,
  experimental: {
    payloadExtraction: false
  },
  app: {
    // pageTransition: { name: 'page', mode: 'out-in' },
    head: {
      title: 'TOS App',
    },
  },
  transpile: ["vuetify"],
  modules: [
    '@bootstrap-vue-next/nuxt',
    [
      '@pinia/nuxt',
      {
        autoImports: ['defineStore', 'acceptHMRUpdate'],
      },
    ],
  ],
  css: [
    'bootstrap/dist/css/bootstrap.min.css',
    '@/assets/app.scss',
    '@/assets/default/app.min.css',
    '@/assets/css/vendor.min.css'
  ],
  imports: {
    dirs: ['stores', 'stores/data-sources/**', 'stores/forms'],
  },
})
