<template>
  <div class="panel panel-inverse" v-if="this.usePanel">
    <div class="panel-heading ui-sortable-handle">
      <h4 class="panel-title">{{ titlePanel || "Apex Chart Bar" }}</h4>
    </div>
    <div class="panel-body">
      <div class="chart-container">
        <apexchart
          type="radialBar"
          :options="defaultOptions"
          :series="[value]"
          height="350"
        />
      </div>
    </div>
  </div>
  <div v-else>
    <apexchart
      type="radialBar"
      :options="defaultOptions"
      :series="[value]"
      height="350"
    />
  </div>
</template>

<script>
export default {
  props: {
    usePanel: {
      type: Boolean,
      required: true,
    },
    value: {
      type: Number,
      required: true,
    },
    label: {
      type: String,
      required: true,
    },
    titlePanel: String,
    titleChart: String,
    labelColor: {
      type: String,
      default: "#fff",//() => ["#008FFB", "#00E396", "#FEB019", "#FF4560"],
    },
    chartColor: {
      type: String,
      default: "#008FFB",//() => ["#008FFB", "#00E396", "#FEB019", "#FF4560"],
    },
  },
  data: () => ({
    defaultOptions: {},
  }),
  mounted: function () {
    this.defaultOptions = {
      chart: {
        id: "radialBar",
        toolbar: {
          show: true,
        },
      },
      plotOptions: {
        radialBar: {
          hollow: {
            size: "70%",
          },
          dataLabels: {
            name: {
              show: true,
              color: this.labelColor,
              offsetY: 40,
              fontSize: "14px",
            },
            value: {
              show: true,
              color: this.labelColor,
              offsetY: 0,
              fontSize: "70px",
            },
          },
        },
      },
      labels: [this.label],
      colors: [this.chartColor],
      fill: {
        opacity: 1,
        colors: this.colorBar,
      },
      // theme: {
      //   mode: "dark",
      // },
    };
  },
};
</script>

<style scoped>
.chart-wrapper {
  max-width: 100%;
  margin: auto;
}
</style>
