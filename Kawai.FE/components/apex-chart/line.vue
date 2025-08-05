<template>
  <div class="panel panel-inverse">
    <div class="panel-heading">
      <h4 class="panel-title">{{ titlePanel || "Apex Line Chart" }}</h4>
    </div>
    <div class="panel-body">
      <div class="chart-container">
        <apexchart
          type="line"
          :options="chartOptions"
          :series="series"
          height="350"
        />
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "LineChart",
  props: {
    series: {
      type: Array,
      required: true,
    },
    labels: {
      type: Array,
      required: true,
    },
    titlePanel: String,
    titleChart: String,
    colorLine: {
      type: Array,
      default: () => ["#008FFB", "#00E396", "#FEB019"], // Default warna line
    },
    curve: {
      type: String,
      default: "smooth", // bisa juga: "straight" | "stepline"
    },
    showMarkers: {
      type: Boolean,
      default: true,
    },
    enableDatalabel: { type: Boolean, default: true },
    funcDataLabelFormatter: {
      type: Function,
      default: (val) => `${val}`,
    },
    funcAxisFormatter: {
      type: Function,
      default: (val) => `${val}`,
    },
    funcFormatter: {
      type: Function,
      default: (val) => `${val}`,
    },
  },
  data: () => ({
    chartOptions: {},
  }),
  mounted: function () {
    this.chartOptions = {
      chart: {
        id: "line-chart",
        toolbar: {
          show: true,
        },
        zoom: {
          enabled: true,
        },
      },
      title: {
        text: this.titleChart || "Apex Line Chart",
        align: "center",
        style: {
          fontSize: "18px",
          fontWeight: "bold",
        },
      },
      xaxis: {
        categories: this.labels,
        labels: {
          formatter: this.funcAxisFormatter,
          style: {
            fontSize: "12px",
          },
        },
      },
      stroke: {
        curve: this.curve,
        width: 2,
      },
      markers: {
        size: this.showMarkers ? 4 : 0,
        strokeWidth: 0,
      },
      colors: this.colorLine,
      dataLabels: {
        enabled: this.enableDatalabel,
        formatter: this.funcDataLabelFormatter,
      },
      tooltip: {
        enabled: true,
        shared: true,
        intersect: false,
        y: {
          formatter: this.funcFormatter,
        },
      },
      grid: {
        borderColor: "#e0e0e0",
      },
      legend: {
        position: "bottom",
        horizontalAlign: "center",
      },
    };
  },
};
</script>

<style scoped>
.chart-container {
  width: 100%;
}
</style>
