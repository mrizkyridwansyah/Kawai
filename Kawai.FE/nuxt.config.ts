export default defineNuxtConfig({
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5056',
      cookieName: process.env.NUXT_PUBLIC_COOKIE_NAME || '__SIDX'
    }
  },
  build: {
    transpile: ["pdfjs-dist", "vuetify"]
   
  },

  vite: {
    optimizeDeps: {
      include: ["pdfjs-dist/legacy/build/pdf"]
    }
  },

  devtools: { enabled: true },
  ssr: false,

  experimental: {
    payloadExtraction: false
  },

  app: {
    head: {
      title: 'TOS App',
    },
  },

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
});