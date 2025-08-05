import bootstrap from 'bootstrap/dist/js/bootstrap.bundle';
import { createApp } from 'vue'
import ToastComponent from '~~/components/v-toast.vue';

var container = document.createElement('div');
container.className = 'toaster-top-right';

var subContainer = document.createElement('div');
subContainer.className = 'toast-slot p-3';

container.appendChild(subContainer);
document.body.appendChild(container)

const BToast = app => {
  window.toastSuccess = (msg) => {

    const instance = createApp(ToastComponent).mount(document.createElement('div'));

    instance.message = msg;
    instance.variant = 'success';

    instance.$el.className += ' mb-3';
    instance.$el.addEventListener('hidden.bs.toast', () => {
      instance.$el.remove();
    })

    subContainer.prepend(instance.$el);

    var toast =  new bootstrap.Toast(instance.$el, {});
    toast.show();
    
    setTimeout(instance.$el.remove, 5000)
  }
  window.toastWarning = (msg) => {

    const instance = createApp(ToastComponent).mount(document.createElement('div'));

    instance.message = msg;
    instance.variant = 'warning';

    instance.$el.className += ' mb-3';
    instance.$el.addEventListener('hidden.bs.toast', () => {
      instance.$el.remove();
    })

    subContainer.prepend(instance.$el);

    var toast =  new bootstrap.Toast(instance.$el, {});
    toast.show();
    
    setTimeout(instance.$el.remove, 5000)
  }
  window.toastDanger = (msg) => {

    const instance = createApp(ToastComponent).mount(document.createElement('div'));
    instance.message = msg;
    instance.variant = 'danger';

    instance.$el.className += ' mb-3';
    instance.$el.addEventListener('hidden.bs.toast', () => {
      instance.$el.remove();
    })

    subContainer.prepend(instance.$el);

    var toast =  new bootstrap.Toast(instance.$el, {});
    toast.show();
    
    setTimeout(instance.$el.remove, 5000)
  }
}

export default defineNuxtPlugin(nuxtApp => {
  nuxtApp.vueApp.use(BToast)
})