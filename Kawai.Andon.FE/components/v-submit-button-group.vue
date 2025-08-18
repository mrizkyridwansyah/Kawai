<template>
  <div>
    <div class="">
      <button class="btn btn-primary btn-sm" @click="submit" :disabled="disabled">Simpan</button>
      <button v-if="this.btnBack" class="btn btn-danger btn-sm ms-2" @click="$router.push(btnBack)" :disabled="disabled">Batal</button>
      <button v-else class="btn btn-danger btn-sm ms-2" @click="$router.back" :disabled="disabled">Batal</button>
    </div>
  </div>
</template>

<script>
export default {
  props: ['submit', 'disabled', 'btnBack']
}
</script>