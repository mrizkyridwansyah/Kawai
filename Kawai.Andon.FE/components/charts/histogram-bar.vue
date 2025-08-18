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
import annotationPlugin from "chartjs-plugin-annotation";

Chart.register(...registerables, annotationPlugin);

export default {
  name: "HistogramChart",
  props: {
    title: String,
    labels: Array,
    data: Array,
    options: {
      type: Object,
      default: () => ({}),
    },
  },
  mounted() {
    const ctx = this.$refs.canvas.getContext("2d");

    const medianIndex = Math.floor(this.labels.length / 2);

    const mergedOptions = {
      indexAxis: "y",
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: true,
          position: "top",
        },
        title: {
          display: !!this.title,
          text: this.title || "",
        },
        annotation: {
          annotations: {
            batasAtas: {
              type: "line",
              yMin: 0,
              yMax: 0,
              borderColor: "red",
              borderWidth: 2,
              label: {
                content: "Batas Atas",
                enabled: true,
                position: "start",
              },
            },
            batasBawah: {
              type: "line",
              yMin: this.labels.length - 1,
              yMax: this.labels.length - 1,
              borderColor: "green",
              borderWidth: 2,
              label: {
                content: "Batas Bawah",
                enabled: true,
                position: "start",
              },
            },
            median: {
              type: "line",
              yMin: medianIndex,
              yMax: medianIndex,
              borderColor: "orange",
              borderWidth: 2,
              label: {
                content: "Median",
                enabled: true,
                position: "start",
              },
            },
          },
        },
      },
      scales: {
        x: {
          beginAtZero: true,
        },
        y: {
          beginAtZero: true,
          ticks: {
            autoSkip: false,
          },
        },
      },
      ...this.options,
    };

    this.chart = new Chart(ctx, {
      type: "bar",
      data: {
        labels: this.labels,
        datasets: [
          {
            label: this.title,
            data: this.data,
            backgroundColor: "rgba(54, 162, 235, 0.7)",
            borderColor: "rgba(54, 162, 235, 1)",
            borderWidth: 1,
            categoryPercentage: 1.0,
            barPercentage: 1.0,
            // barThickness: 20,
          },
        ],
      },
      options: mergedOptions,
    });
  },
  beforeUnmount() {
    if (this.chart) {
      this.chart.destroy();
    }
  },
};
</script>

<style scoped>
.chart-container {
  width: 100%;
  height: 400px;
  /* Tinggi dikontrol dari script (inline style canvas) */
}
canvas {
  width: 100% !important;
  height: 100% !important;
}
</style>
