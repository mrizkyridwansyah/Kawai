

window.setCookie = function (cname, cvalue, date) {
  var expires = 'expires=' + new Date(date);
  document.cookie = cname + '=' + cvalue + ';' + expires + ';path=/';
};

window.getCookie = function (cname) {
  var name = cname + "=";
  var decodedCookie = decodeURIComponent(document.cookie);
  var ca = decodedCookie.split(';');
  for(var i = 0; i <ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0) == ' ') {
      c = c.substring(1);
    }
    if (c.indexOf(name) == 0) {
      return c.substring(name.length, c.length);
    }
  }
  return null;
}

window.clearCookies = function() {
  document.cookie.split(";").forEach(function(c) { 
    if(c.replace(/^ +/, "").split("=")[0] == "__SIDX") {
      document.cookie = c.replace(/^ +/, "").replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/"); 
    }
  });
}

window.uniqueId = function(){
  return Date.now().toString(36) + Math.random().toString(36);
}

window.objCopy = function(data) {
  return JSON.parse(JSON.stringify(data));
}

export default defineNuxtPlugin(nuxtApp => {
  // Doing something with nuxtApp
})

