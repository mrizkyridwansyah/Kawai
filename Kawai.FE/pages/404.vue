<template>
  <div class="not-found">
    <img src="/404.png" alt="404 Illustration" class="not-found-image" />
    <h1 class="title">Oops! Halaman tidak ditemukan</h1>
    <p class="subtitle">Kayaknya kamu tersesat di alam semesta lain 🛸</p>
    <nuxt-link to="/app" class="home-button">🏠 Back to Home</nuxt-link>
  </div>
</template>

<script>
export default {
  layout: 'default', // pakai layout biasa
  head() {
    return {
      bodyAttrs: {
        class: 'body-404'
      }
    }
  },
}
</script>

<style scoped>
.not-found {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  min-height: 100vh;
  padding: 40px 20px;
  background-color: #f9f9f9;
  font-family: "Segoe UI", sans-serif;
}

.not-found-image {
  max-width: 500px;
  width: 90%;
  margin-bottom: 30px;
}

.title {
  font-size: 28px;
  font-weight: bold;
  color: #333;
}

.subtitle {
  font-size: 16px;
  color: #666;
  margin: 15px 0 30px;
}

.home-button {
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  text-decoration: none;
  border-radius: 6px;
  font-weight: bold;
  transition: background-color 0.3s ease;
}

.home-button:hover {
  background-color: #0056b3;
}
</style>

<!-- Global style khusus untuk 404 -->
<style>
body.body-404 {
  background-color: white !important;
}
</style>
