import bootstrap from 'bootstrap/dist/js/bootstrap.bundle'
import { createApp } from 'vue'

import AlertComponent from '~~/components/v-modal-alert-remove.vue';
import ConfirmComponent from '~~/components/v-modal-alert-confirm.vue';

const BAlert = (app) => {
  const alertInstance = createApp(AlertComponent).mount(document.createElement('div'));
  
  window.confirmRemove = (onYes, onNo, name) => {
    var modal =  new bootstrap.Modal(alertInstance.$el, { backdrop: true });
    alertInstance.onHide = modal.hide;
    alertInstance.onYes = onYes;
    alertInstance.name = name;
    alertInstance.onNo = function() {
      if(onNo !== undefined && onNo !== null)
        onNo();
      modal.hide();
    };

    modal.show();
  }

  const confirmInstance = createApp(ConfirmComponent).mount(document.createElement('div'));
  
  window.confirmSubmit = (onYes, onNo, msg) => {
    var modal =  new bootstrap.Modal(confirmInstance.$el, { backdrop: true });
    confirmInstance.onYes = onYes;
    confirmInstance.message = msg;
    confirmInstance.onNo = function() {
      if(onNo !== undefined && onNo !== null) {
        onNo();
      }
      modal.hide();
    };

    modal.show();
  }
}

export default defineNuxtPlugin(nuxtApp => {
  nuxtApp.vueApp.use(BAlert)
})