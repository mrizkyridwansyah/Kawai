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

const defaultOptions = {
  indexAxis: "y",
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: "top",
    },
    title: {
      display: false,
      text: "",
    },
  },
  scales: {
    x: {
      beginAtZero: true,
    },
    y: {
      beginAtZero: true,
    },
  },
};

export default {
  name: "HorizontalBarChart",
  props: {
    title: String,
    labels: Array,
    datasets: Array,
    options: {
      type: Object,
      default: () => ({}),
    },
  },
  mounted: function () {
    Chart.register(...registerables);
    const ctx = this.$refs.canvas.getContext("2d");

    // Merge default options dan options dari props, dengan prioritas props.options
    const mergedOptions = {
      ...defaultOptions,
      ...this.options,
      plugins: {
        ...defaultOptions.plugins,
        ...this.options.plugins,
        title: {
          display:
            !!this.title ||
            (this.options.plugins &&
              this.options.plugins.title &&
              this.options.plugins.title.display),
          text:
            this.title ||
            (this.options.plugins &&
              this.options.plugins.title &&
              this.options.plugins.title.text) ||
            "",
        },
      },
      scales: {
        ...defaultOptions.scales,
        ...this.options.scales,
      },
    };

    new Chart(ctx, {
      type: "bar",
      data: {
        labels: this.labels,
        datasets: this.datasets,
      },
      options: mergedOptions,
    });
  },
  beforeUnmount: function () {
    if (this.Chart) this.chart.destroy();
  },
};
</script>

<style scoped>
.chart-container {
  width: 100%;
  height: 400px;
}
canvas {
  width: 100% !important;
  height: 100% !important;
}
</style>
