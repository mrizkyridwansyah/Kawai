<template>
  <div class="panel panel-inverse">
    <div class="panel-heading">
      <h4 class="panel-title">{{ titlePanel || "Line & Column Chart" }}</h4>
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
  name: "LineColumnMixedChart",
  props: {
    series: {
      type: Array,
      required: true,
      // Contoh format:
      // [
      //   { name: "Revenue", type: "column", data: [400, 430, 448, 470] },
      //   { name: "Growth", type: "line", data: [10, 20, 15, 25] }
      // ]
    },
    labels: {
      type: Array,
      required: true,
    },
    titlePanel: String,
    titleChart: String,
    colors: {
      type: Array,
      default: () => ["#008FFB", "#FEB019"],
    },
    enableDatalabel: {
        type: Boolean,
        default: true
    },
    funcDataLabelFormatter: {
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
        height: 350,
        type: "line",
        stacked: false,
        toolbar: {
          show: true,
        },
      },
      title: {
        text: this.titleChart || "Mixed Line & Column Chart",
        align: "left",
        style: {
          fontSize: "18px",
          fontWeight: "bold",
        },
      },
      stroke: {
        width: [0, 4],
      },
      plotOptions: {
        bar: {
          columnWidth: "50%",
        },
      },
      colors: this.colors,
      dataLabels: {
        enabled: this.enableDatalabel,
        formatter: this.funcDataLabelFormatter,
        enabledOnSeries: [1],
      },
      xaxis: {
        categories: this.labels,
      },
      yaxis: [
        {
          title: {
            text: this.series[0]?.name || "Column Series",
          },
        },
        {
          opposite: true,
          title: {
            text: this.series[1]?.name || "Line Series",
          },
        },
      ],
      tooltip: {
        shared: true,
        intersect: false,
        y: {
            formatter: this.funcFormatter
        }
      },
      legend: {
        position: "bottom",
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
