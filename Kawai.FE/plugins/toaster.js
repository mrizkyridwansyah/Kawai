import bootstrap from 'bootstrap/dist/js/bootstrap.bundle';
import { createApp } from 'vue';
import ToastComponent from '~~/components/v-toast.vue';
import ToastNotifComponent from '~~/components/v-toast-notif.vue';

const container = document.createElement('div');
container.className = 'toaster-top-right';

const subContainer = document.createElement('div');
subContainer.className = 'toast-slot p-3';

container.appendChild(subContainer);
document.body.appendChild(container);

const BToast = app => {

  function showToast(Component, props = {}) {
    const instance = createApp(Component, props).mount(document.createElement('div'));

    instance.$el.className += ' mb-3';
    instance.$el.addEventListener('hidden.bs.toast', () => {
      instance.$el.remove();
    });

    subContainer.prepend(instance.$el);

    const toast = new bootstrap.Toast(instance.$el);
    toast.show();

    setTimeout(() => instance.$el.remove(), 5000);
  }

  window.toastNotif = (notif) => showToast(ToastNotifComponent, { notif });
  window.toastInfo = (msg) => showToast(ToastComponent, { message: msg, variant: 'info' });
  window.toastSuccess = (msg) => showToast(ToastComponent, { message: msg, variant: 'success' });
  window.toastWarning = (msg) => showToast(ToastComponent, { message: msg, variant: 'warning' });
  window.toastDanger = (msg) => showToast(ToastComponent, { message: msg, variant: 'danger' });
}

export default defineNuxtPlugin(nuxtApp => {
  nuxtApp.vueApp.use(BToast);
});
