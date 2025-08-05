<template>
  <div class="panel panel-inverse">
    <div class="panel-heading">
      <h4 class="panel-title">{{ titlePanel || "Line Area Chart" }}</h4>
    </div>
    <div class="panel-body">
      <div class="chart-container">
        <apexchart
          type="area"
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
  name: "LineAreaChart",
  props: {
    series: {
      type: Array,
      required: true,
      // example: [{ name: 'Sales', data: [30, 40, 35, 50] }]
    },
    labels: {
      type: Array,
      required: true,
    },
    yAxis: {
      type: Array,
      required: true,
    },
    titlePanel: String,
    titleChart: String,
    fillOption: {
      type: String,
      default: "solid",
    },
    colors: {
      type: Array,
      default: () => ["#008FFB", "#00E396", "#FEB019"],
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
    fillSolidOption: {
      type: "solid",
      opacity: [0.35, 1],
    },
    fillGradientOption: {
      type: "gradient",
      gradient: {
        shadeIntensity: 1,
        opacityFrom: 0.5,
        opacityTo: 1,
        stops: [0, 90, 100],
      },
    },
  }),
  mounted: function () {
    this.chartOptions = {
      chart: {
        type: "area",
        toolbar: {
          show: true,
        },
        zoom: {
          enabled: true,
        },
      },
      //   colors: this.colors,
      dataLabels: {
        enabled: this.enableDatalabel,
        formatted: this.funcDataLabelFormatter
      },
      stroke: {
        curve: "smooth",
        width: 2,
      },
      fill:
        this.fillOption == "solid"
          ? this.fillSolidOption
          : this.fillGradientOption,
      xaxis: {
        categories: this.labels,
        labels: {
          style: {
            fontSize: "13px",
          },
        },
      },
      markers: {
        size: 0,
      },
      yaxis: this.yAxis,
      title: {
        text: this.titleChart || "Line Area Chart",
        align: "left",
        style: {
          fontSize: "18px",
          fontWeight: "bold",
        },
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
