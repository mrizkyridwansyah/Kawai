<template>
  <div>
    <div class="pl1">
      <div class="pl1__a"></div>
      <div class="pl1__b"></div>
      <div class="pl1__c"></div>
      <div class="pl1__d"></div>
    </div>
  </div>
</template>
<style lang="scss">
.pl1 {
	justify-content: space-around;
  margin: 0 auto;
	.pl1__a, .pl1__b, .pl1__c, .pl1__d {
		border-radius: 50%;
		width: 1em;
		height: 1em;
		transform-origin: 50% 100%;
  }
}
.pl1 {
	.pl1__a, .pl1__b, .pl1__c, .pl1__d {
		animation: bounce1 1s linear infinite;
  }
}
.pl1 {
	display: flex;
	margin: 1.5em;
	width: 6em;
	height: 6em; 
  margin: 0 auto;
  .pl1__a {
		background: #ff4560;
  }
  .pl1__b {
		background: #feb019;
		animation-delay: 0.1s;
  }
  .pl1__c {
		background: #00e396;
		animation-delay: 0.2s;
  }
  .pl1__d {
		background: #008ffb;
		animation-delay: 0.3s;
  }
}

/* Animations */
@keyframes bounce1 {
  0% {
		transform: translateY(0) scale(1,1);
		animation-timing-function: ease-in;
  }
  45% {
		transform: translateY(1.2em) scale(1,1);
		animation-timing-function: linear;
  }
  50% {
		transform: translateY(1.2em) scale(1.5,0.5);
		animation-timing-function: linear;
  }
  55% {
		transform: translateY(1.2em) scale(1,1);
		animation-timing-function: ease-out;
  }
}
</style>

<!-- <template>
  <div>
    <span class="loader"></span>
  </div>
</template>

<style>
.loader {
  width: 15px;
  height: 15px;
  border-radius: 50%;
  display: block;
  margin:25px auto;
  position: relative;
  color: #596296;
  left: -100px;
  box-sizing: border-box;
  animation: shadowRolling 2s linear infinite;
}

@keyframes shadowRolling {
  0% {
    box-shadow: 0px 0 rgba(255, 255, 255, 0), 0px 0 rgba(255, 255, 255, 0), 0px 0 rgba(255, 255, 255, 0), 0px 0 rgba(255, 255, 255, 0);
  }
  12% {
    box-shadow: 100px 0 #596296, 0px 0 rgba(255, 255, 255, 0), 0px 0 rgba(255, 255, 255, 0), 0px 0 rgba(255, 255, 255, 0);
  }
  25% {
    box-shadow: 110px 0 #596296, 
                100px 0 #596296, 
                0px 0 rgba(255, 255, 255, 0), 
                0px 0 rgba(255, 255, 255, 0);
  }
  36% {
    box-shadow: 120px 0 #596296, 
                110px 0 #596296, 
                100px 0 #596296, 
                0px 0 rgba(255, 255, 255, 0);
  }
  50% {
    box-shadow: 130px 0 #596296, 
                120px 0 #596296, 
                110px 0 #596296, 
                100px 0 #596296;
  }
  62% {
    box-shadow: 200px 0 rgba(255, 255, 255, 0), 
                130px 0 #596296, 
                120px 0 #596296, 
                110px 0 #596296;
  }
  75% {
    box-shadow: 200px 0 rgba(255, 255, 255, 0), 
                200px 0 rgba(255, 255, 255, 0), 
                130px 0 #596296, 
                120px 0 #596296;
  }
  87% {
    box-shadow: 200px 0 rgba(255, 255, 255, 0), 
                200px 0 rgba(255, 255, 255, 0), 
                200px 0 rgba(255, 255, 255, 0), 
                130px 0 #596296;
  }
  100% {
    box-shadow: 200px 0 rgba(255, 255, 255, 0), 
                200px 0 rgba(255, 255, 255, 0), 
                200px 0 rgba(255, 255, 255, 0), 
                200px 0 rgba(255, 255, 255, 0);
  }
}
</style> -->