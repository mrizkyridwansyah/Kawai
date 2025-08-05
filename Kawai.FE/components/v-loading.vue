<template>
  <div class="loading-1-container">
    <div class="loading-1">
      <div class="loading-1-line"></div>
      <div class="loading-1-subline inc"></div>
      <div class="loading-1-subline dec"></div>
    </div>
  </div>
</template>


<style lang="scss">
.loading-1-container {
  position: relative;
}

.loading-1{
  position:absolute;
  width:100%;
  height:5px;
  overflow-x: hidden;
}

.loading-1-line{
  position:absolute;
  opacity: 0.4;
  background:#4a8df8;
  width:150%;
  height:5px;
}

.loading-1-subline{
  position:absolute;
  background:#4a8df8;
  height:5px; 
}
.inc{
  animation: increase 2s infinite;
}
.dec{
  animation: decrease 2s 0.5s infinite;
}

@keyframes increase {
 from { left: -5%; width: 5%; }
 to { left: 130%; width: 100%;}
}
@keyframes decrease {
 from { left: -80%; width: 80%; }
 to { left: 110%; width: 10%;}
}
</style>