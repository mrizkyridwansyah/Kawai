import bootstrap from 'bootstrap/dist/js/bootstrap.bundle'
import 'bootstrap-icons/font/bootstrap-icons.css'
export default defineNuxtPlugin(nuxtApp => {
  
  nuxtApp.provide('bootstrap', bootstrap)
  nuxtApp.provide('bvModal', {
    show: function(id) {
      var el = document.getElementById(id);
      var modal = new bootstrap.Modal(el, { backdrop: true });
      modal.show();
    },
    hide: function(id) {
      var el = document.getElementById(id);
      var modal = bootstrap.Modal.getInstance(el);
      modal.hide();
      document.querySelector('.modal-backdrop').remove();
    }
  })
})