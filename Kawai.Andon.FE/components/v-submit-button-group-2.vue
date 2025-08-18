<template>
  <div class="my-5 py-5">
    <hr />
    <div class="">
      <button class="form-submit bg-success" @click="submit" :disabled="disabled">Simpan</button>
      <button class="form-submit bg-danger ms-2" @click="$router.back" :disabled="disabled">Batal</button>
    </div>
  </div>
</template>

<script>
export default {
  props: ['submit', 'disabled']
}
</script>