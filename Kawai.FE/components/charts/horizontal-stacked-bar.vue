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
  name: "HorizontalStackedBarChart",
  props: {
    title: String,
    labels: {
      type: Array,
      required: true,
    },
    datasets: {
      type: Array,
      required: true,
    },
    options: {
      type: Object,
      default: () => ({}),
    },
  },
  mounted() {
    Chart.register(...registerables);
    const ctx = this.$refs.canvas.getContext("2d");

    const defaultOptions = {
      indexAxis: "y",
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: !!this.title,
          text: this.title || "Horizontal Stacked Bar Chart",
          // font: { size: 18 },
        },
        legend: {
          position: "top",
          labels: {
            font: { size: 12 },
          },
        },
        tooltip: {
          mode: "index",
          intersect: false,
        },
      },
      scales: {
        x: {
          stacked: true,
          beginAtZero: true,
          title: {
            display: true,
            text: "Value",
          },
        },
        y: {
          stacked: true,
          title: {
            display: true,
            text: "Categories",
          },
        },
      },
    };

    const mergedOptions = {
      ...defaultOptions,
      ...this.options,
      plugins: {
        ...defaultOptions.plugins,
        ...(this.options.plugins || {}),
        title: {
          ...defaultOptions.plugins.title,
          ...(this.options.plugins?.title || {}),
        },
        legend: {
          ...defaultOptions.plugins.legend,
          ...(this.options.plugins?.legend || {}),
        },
      },
      scales: {
        ...defaultOptions.scales,
        ...(this.options.scales || {}),
        x: {
          ...defaultOptions.scales.x,
          ...(this.options.scales?.x || {}),
        },
        y: {
          ...defaultOptions.scales.y,
          ...(this.options.scales?.y || {}),
        },
      },
    };

    this.chart = new Chart(ctx, {
      type: "bar",
      data: {
        labels: this.labels,
        datasets: this.datasets,
      },
      options: mergedOptions,
    });
  },
  beforeUnmount() {
    if (this.chart) this.chart.destroy();
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
