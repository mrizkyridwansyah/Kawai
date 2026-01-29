<template>
  <div class="modal fade zoom" :id="id" data-bs-backdrop="static" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Confirmation</h5>
          <button type="button" class="btn-close" 
            data-bs-dismiss="modal" 
            aria-label="Close"
            :data-bs-target="`#${id}`"
            :disabled="isLoading"
          >

          </button>
        </div>
        <div class="modal-body">
          <span v-html="message || 'Are you sure want to continue?'"></span>
          <div class="d-flex justify-content-end mt-4">
            <button class="btn btn-light btn-sm rounded-pill"
              @click="onNo"
              :disabled="isLoading"
            >
              <span class="mx-3">Cancel</span>
            </button>
            <button class="btn btn-primary btn-sm rounded-pill ms-3 btn-confirm"
              @click="confirm"
              :disabled="isLoading"
            >
              <div class="spinner-border spinner-border-sm text-light" role="status"
                v-if="isLoading"
              >
                <span class="visually-hidden">Loading...</span>
              </div>
              <span class="mx-3">Yes</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>  
</template>

<script>
import bootstrap from 'bootstrap/dist/js/bootstrap.bundle'
export default {
  props: [],
  data: () => ({
    id: 'v_modal-alert-confirm',
    isLoading: false,
    message: null,
  }),
  methods: {
    submit: function(e) {
      e.preventDefault();
    },
    setId: function(id) {
      this.id = id;
    },
    hide: function() {
      var el = document.getElementById(this.id);
      var modal = bootstrap.Modal.getInstance(el);
      modal.hide();
    },
    confirm: function() {
      this.isLoading = true;
      this.onYes()?.then(_ => {
        this.isLoading = false;
        this.hide();
      })
      .catch(_ => this.isLoading = false);
    },
  }
}
</script>

<style lang="scss">
.btn-confirm {
  animation: pulse-button-confirm 2s infinite;
}
@-webkit-keyframes pulse-button-confirm {
  0% {
    color: #fff;
    -webkit-box-shadow: 0 0 0 0 rgba(73, 144, 226, 0.8);
    box-shadow: 0 0 0 0 rgba(73, 144, 226, 0.8);
  }

  70% {
    -webkit-box-shadow: 0 0 0 8px rgba(73, 144, 226, 0);
    box-shadow: 0 0 0 8px rgba(73, 144, 226, 0);
  }

  100% {
    color: #fff;
    -webkit-box-shadow: 0 0 0 0 rgba(73, 144, 226, 0);
    box-shadow: 0 0 0 0 rgba(73, 144, 226, 0);
  }

}

@keyframes pulse-button-confirm {
  0% {
    color: #fff;
    -webkit-box-shadow: 0 0 0 0 rgba(73, 144, 226, 0.8);
    box-shadow: 0 0 0 0 rgba(73, 144, 226, 0.8);
  }

  70% {
    -webkit-box-shadow: 0 0 0 5px rgba(73, 144, 226, 0);
    box-shadow: 0 0 0 5px rgba(73, 144, 226, 0);
  }

  100% {
    color: #fff;
    -webkit-box-shadow: 0 0 0 0 rgba(73, 144, 226, 0);
    box-shadow: 0 0 0 0 rgba(73, 144, 226, 0);
  }
}
</style>