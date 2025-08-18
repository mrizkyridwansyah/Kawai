
export default defineNuxtRouteMiddleware((to, from) => {
  
  if (!getCookie("__SIDX")) {
    location.href = '/auth/sign-in';
  }
  var user = useAuth();
  var router = useRouter();
  var profile = useProfile();
  if((JSON.stringify(user.data) == "{}" || user.data == null) && !user.isLoading) {
    user.load().then(() => {
      // console.log(user.data)
      // location.href = '/app'
      // router.push('/app')
    });
  }
  if(!profile.isLoaded && !profile.isLoading) {
    profile.load().then(() => {
    });
  }
  var featureId = to.matched[to.matched.length-1].components?.default?.featureId;
  if(!profile.allowed(featureId)) {
    // location.href = '/404'
  }
    
})