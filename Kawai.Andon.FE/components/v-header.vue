<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-light fixed-top">
    <div class="container-fluid container">
      <a class="navbar-brand" href="/">Meteora</a>
      <span class="badge text-bg-warning"
        v-if="ws.current.IsTrial"
      >
        Trial
      </span>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent"
        aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse">
        <div class="me-auto"></div>
        <form class="d-flex">
          <ul class="navbar-nav me-auto mb-2 mb-lg-0">
            <li class="nav-item me-3">
              <a href="#" class="nav-link">
                {{ws.current.Name}}
              </a>
            </li>
            <!-- <li class="nav-item me-3">
              <a href="#" class="nav-link">
                <v-icon name="bell" 
                  :width="17"
                  weight="3"
                  style="color: #555"
                />
              </a>
            </li> -->
            <header-menu-notification class=" me-3" />
            <header-menu-user-info />
          </ul>
        </form>
      </div>
    </div>
  </nav>
</template>

<script setup>

var ws = useWorkspace();
var route = useRoute();
onMounted(() => {
  // ws.load().then(v => {
  //   if(!ws.inWorkspace && v.Data.length > 0) {
  //     location.href = `/app/${v.Data[0].Id}`
  //     ws.setCurrent(v.Data[0])
  //   }
  //   else {

  //   }
  // });
})
</script>

<style lang="scss">

.menu-item {
  padding: 0px;
  color: rgb(24, 28, 50);
  font-family: Poppins, Helvetica, 'sans-serif';
  font-size: 13.975px;
  letter-spacing: normal;
}

.menu-link {
  color: rgb(94, 98, 120);
  transition: none 0s ease 0s;
  cursor: pointer;
  display: flex;
  align-items: center;
  flex: 0 0 100%;
  padding: 0.65rem 1rem;
  border-radius: 4px;
  border-left: 3px solid transparent;
  outline: none !important;
  text-decoration: none !important;;
  font-weight: 500;
}
.menu-link.active {
  color: rgb(94, 98, 120);
  transition: color 0.2s ease 0s, background-color 0.2s ease 0s;
  cursor: pointer;
  display: flex;
  align-items: center;
  flex: 0 0 100%;
  padding: 0.65rem 1rem;
  border-radius: 4px;
  background-color: rgba(245, 248, 250, 0.8);
  border-left: 3px solid rgb(0, 158, 247);
  outline: none !important;

  .menu-title {
    display: flex; align-items: center; flex-grow: 1; color: rgb(63, 66, 84); font-weight: 600;
  }
}
.menu-content {
  padding: 0.65rem 1rem; margin-left: 3px;
}
.menu-section {
  --bs-text-opacity: 1;
  font-size: 0.95rem !important;
  font-weight: 600 !important;
  color: rgb(161, 165, 183) !important;
}
.menu-title {
  display: flex;
  align-items: center;
  flex-grow: 1;
  color: rgb(63, 66, 84);
}
.page-item.active .page-link {
  z-index: 1!important;
}
</style>