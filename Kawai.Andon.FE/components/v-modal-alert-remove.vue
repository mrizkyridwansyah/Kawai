<template>
  <div class="modal fade zoom" :id="id" data-bs-backdrop="static" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Confirmation</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" :data-bs-target="`#${id}`"
            :disabled="isLoading">

          </button>
        </div>
        <div class="modal-body">
          <span>If you delete the <strong>"{{ name || 'data' }}"</strong>, you <strong>CAN'T recover it</strong>.
            Are you sure to delete the data? </span>
          <!-- <div class="mt-5">
            <label>Reason</label>
            <input-text 
              label="Reason"
              multiline
              v-model="reasonText"
            />
          </div> -->
          <div class="d-flex justify-content-end mt-4">
            <button class="btn btn-white btn-sm rounded-pill " @click="onNo" :disabled="isLoading">
              <span class="mx-3">Cancel</span>
            </button>
            <button class="btn btn-danger btn-sm rounded-pill ms-3 btn-cancel" @click="confirm" :disabled="isLoading">
              <div class="spinner-border spinner-border-sm text-light" role="status" v-if="isLoading">
                <span class="visually-hidden">Loading...</span>
              </div>
              <span class="mx-3">Delete</span>
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
    id: 'v_modal-alert-remove',
    isLoading: false,
    reasonText: null,
    name: null,
  }),
  methods: {
    submit: function (e) {
      e.preventDefault();
    },
    setId: function (id) {
      this.id = id;
    },
    hide: function () {
      var el = document.getElementById(this.id);
      var modal = bootstrap.Modal.getInstance(el);
      modal.hide();
    },
    // cancel: function() {
    //   if(this.onNo) {
    //     this.onNo();
    //   }
    // },
    confirm: function () {
      this.isLoading = true;
      this.onYes(this.reasonText)?.then(_ => {
        this.isLoading = false;
        this.hide();
      })
        .catch(_ => this.isLoading = false);
    },
  }
}
</script>

<style lang="scss">
.btn-cancel {
  animation: pulse-button-danger 2s infinite;
}

@-webkit-keyframes pulse-button-danger {
  0% {
    color: #fff;
    background: #dc3545;
    -webkit-box-shadow: 0 0 0 0 rgba(73, 144, 226, 0.8);
    box-shadow: 0 0 0 0 rgba(73, 144, 226, 0.8);
  }

  70% {
    -webkit-box-shadow: 0 0 0 5px rgba(73, 144, 226, 0);
    box-shadow: 0 0 0 5px rgba(73, 144, 226, 0);
  }

  100% {
    color: #fff;
    background: #dc3545;
    -webkit-box-shadow: 0 0 0 0 rgba(73, 144, 226, 0);
    box-shadow: 0 0 0 0 rgba(73, 144, 226, 0);
  }

}

@keyframes pulse-button-danger {
  0% {
    color: #fff;
    background: #dc3545;
    -webkit-box-shadow: 0 0 0 0 rgba(220, 53, 69, 0.8);
    box-shadow: 0 0 0 0 rgba(220, 53, 69, 0.8);
  }

  70% {
    -webkit-box-shadow: 0 0 0 8px rgba(220, 53, 69, 0);
    box-shadow: 0 0 0 8px rgba(220, 53, 69, 0);
  }

  100% {
    color: #fff;
    background: #dc3545;
    -webkit-box-shadow: 0 0 0 0 rgba(220, 53, 69, 0);
    box-shadow: 0 0 0 0 rgba(220, 53, 69, 0);
  }
}
</style>