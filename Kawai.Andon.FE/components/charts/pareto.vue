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
  name: "ParetoChart",
  props: {
    title: {
      type: String,
      default: "Pareto Chart",
    },
    labels: {
      type: Array,
      required: true,
    },
    values: {
      type: Array,
      required: true,
    },
    barColor: {
      type: String,
      default: "#3b82f6",
    },
    lineColor: {
      type: String,
      default: "#f97316",
    },
    borderColor: {
      type: String,
      default: "#f97316",
    },
    options: {
      type: Object,
      default: () => ({}),
    },
  },
  mounted() {
    Chart.register(...registerables);
    const ctx = this.$refs.canvas.getContext("2d");

    // Sort data
    const sorted = this.values.map((v, i) => ({
      value: v,
      label: this.labels[i],
    }));
    // .sort((a, b) => b.value - a.value);

    const sortedValues = sorted.map((v) => v.value);
    const sortedLabels = sorted.map((v, i) => v.label);

    // Cumulative %
    const total = sortedValues.reduce((a, b) => a + b, 0);
    const cumulative = sortedValues.reduce((acc, val, i) => {
      const sum = (acc[i - 1] || 0) + val;
      acc.push(sum);
      return acc;
    }, []);

    const percentage = cumulative.reduce((acc, val, i) => {
      acc.push({
        x: i + 0.5,
        y: Number(((val / total) * 100).toFixed(0)),
      });
      return acc;
    }, []);

    const maxValue = sortedValues.reduce((acc, idx) => acc + idx, 0);
    const yMax = Math.ceil(maxValue * 1.1);

    // Data
    const data = {
      labels: sortedLabels,
      datasets: [
        {
          type: "line",
          label: "Cumulative %",
          data: [{ x: -0.5, y: 0 }, ...percentage],
          parsing: false,
          borderColor: this.lineColor,
          backgroundColor: this.lineColor,
          fill: false,
          yAxisID: "y1",
          tension: 0.1,
          pointRadius: 3,
        },
        {
          type: "bar",
          label: "Frequency",
          data: sortedValues,
          backgroundColor: this.barColor,
          borderColor: this.borderColor,
          borderWidth: 1,
          yAxisID: "y",
        },
      ],
    };

    const defaultOptions = {
      responsive: true,
      maintainAspectRatio: false,
      layout: {
        padding: { left: 0, right: 0 },
      },
      interaction: {
        mode: "nearest", // tooltip hanya muncul saat mendekati elemen yang aktif
        intersect: true, // harus berada langsung di atas elemen
      },
      plugins: {
        title: {
          display: true,
          text: this.title,
        },
        legend: {
          position: "top",
        },
        tooltip: {
          enabled: true,
          callbacks: {
            title: (tooltipItems) => {
              const item = tooltipItems[0];
              const labelIndex =
                item.dataset.type === "line"
                  ? Math.floor(item.raw?.x ?? 0)
                  : item.dataIndex;
              return sortedLabels[labelIndex] || "";
            },
            label: (tooltipItem) => {
              const dataset = tooltipItem.dataset;
              const index = tooltipItem.dataIndex;

              if (dataset.type === "bar") {
                return `Frequency: ${dataset.data[index]}`;
              }

              if (dataset.type === "line") {
                const value = tooltipItem.raw?.y ?? tooltipItem.raw;
                return `Cumulative %: ${value}%`;
              }

              return "";
            },
          },
        },
      },
      scales: {
        x: {
          beginAtZero: true,

          offset: true,
          grid: {
            offset: false,
          },
          ticks: {
            align: "center",
          },
        },
        y: {
          beginAtZero: true,
          max: maxValue,
          ticks: {
            stepSize: maxValue / 5,
          },
          title: {
            display: true,
            text: "Frequency",
          },
        },
        y1: {
          beginAtZero: true,
          max: 100,
          position: "right",
          grid: {
            drawOnChartArea: false,
          },
          ticks: {
            stepSize: 20,
            callback: (value) => `${value}%`,
          },
          title: {
            display: true,
            text: "Cumulative %",
          },
        },
      },
      datasets: {
        bar: {
          barPercentage: 1,
          categoryPercentage: 1,
          order: 1,
        },
        line: {
          order: 2,
        },
      },
    };

    this.chart = new Chart(ctx, {
      data,
      options: {
        ...defaultOptions,
        ...this.options,
      },
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
