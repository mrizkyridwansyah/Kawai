<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <h4 class="panel-title">{{ titlePanel || "Apex Chart Bar" }}</h4>
    </div>
    <div class="panel-body">
      <div class="chart-container">
        <apexchart
          type="bar"
          :options="defaultOptions"
          :series="series"
          height="350"
        />
      </div>
    </div>
  </div>
</template>

<script>
export default {
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
    horizontal: {
      type: Boolean,
      default: false,
    },
    colors: {
      type: Array,
      default: () => ["#008FFB", "#00E396", "#FEB019", "#FF4560"],
    },
    enableDatalabel: {
      type: Boolean,
      default: true,
    },
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
    defaultOptions: {},
  }),
  mounted: function () {
    this.defaultOptions = {
      chart: {
        id: "basic-bar",
        toolbar: {
          show: true,
        },
      },
      plotOptions: {
        bar: {
          //   borderRadius: 5,
          horizontal: this.horizontal || false,
          columnWidth: "50%", // Atur lebar batang
          endingShape: "rounded",
        },
      },
      dataLabels: {
        enabled: this.enableDatalabel,
        formatter: this.funcDataLabelFormatter,
        style: {
          fontSize: "12px",
          colors: ["#fff"],
        },
      },
      stroke: {
        show: true,
        width: 2,
        colors: ["transparent"],
      },
      xaxis: {
        categories: this.labels,
        labels: {
          formatter: this.funcAxisFormatter,
          style: {
            fontSize: "13px",
          },
        },
      },
      yaxis: {
        // title: {
        //   text: this.series[0].name,
        // },
        labels: {
          formatter: this.funcAxisFormatter,
        },
      },
      fill: {
        opacity: 1,
        colors: this.colorBar,
      },
      tooltip: {
        y: {
          formatter: this.funcFormatter,
        },
      },
      title: {
        text: this.titleChart || "Apex Chart Bar",
        align: "center",
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
.chart-wrapper {
  max-width: 100%;
  margin: auto;
}
</style>
