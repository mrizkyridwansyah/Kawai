// plugins/router-fix.client.ts
export default defineNuxtPlugin(() => {
  const router = useRouter()
  const route = useRoute()

  const originalPush = router.push.bind(router)

  router.push = (to) => {
    if (typeof to === 'string' && !to.startsWith('/')) {
      const currentSegments = route.path.split('/').filter(Boolean)
      const toSegments = to.split('/').filter(Boolean)

      // kalau segment pertama sama dengan last segment current path
      if (
        currentSegments.length &&
        toSegments.length &&
        currentSegments[currentSegments.length - 1] === toSegments[0]
      ) {
        // buang duplikat
        toSegments.shift()
        to = route.path.replace(/\/$/, '') + '/' + toSegments.join('/')
      }
    }

    return originalPush(to)
  }
})
