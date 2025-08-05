<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <h4 class="panel-title">{{ title }}</h4>
    </div>
    <div class="panel-body">
      <div class="chart-container">
        <canvas ref="canvas"></canvas>
      </div>
    </div>
  </div>
</template>

<script>
import { Chart, registerables } from "chart.js";

export default {
  name: "ScatterChart",
  props: ["title", "datasets", "options"],
  mounted: function () {
    Chart.register(...registerables);
    const ctx = this.$refs.canvas.getContext("2d");

    new Chart(ctx, {
      type: "scatter",
      data: {
        datasets: this.datasets,
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { title: { display: !!this.title, text: this.title } },
        ...this.options
      },
    });
  },
  beforeUnmount: function () {
    if (this.Chart) this.chart.destroy();
  },
};
</script>

<style scoped>
.chart-container {
  position: relative;
  width: 100%;
  height: 400px;
}
canvas {
  width: 100% !important;
  height: 100% !important;
}
</style>
