<template>
  <div class="panel panel-inverse" v-if="usePanel">
    <div class="panel-heading ui-sortable-handle">
      <h4 class="panel-title">{{ titlePanel || "Progress Bar" }}</h4>
    </div>

    <div class="panel-body">
      <div class="chart-container">
        <apexchart
          type="bar"
          :options="defaultOptions"
          :series="computedSeries"
          :height="height"
        />
      </div>
    </div>
  </div>

  <div v-else>
    <apexchart
      type="bar"
      :options="defaultOptions"
      :series="computedSeries"
      :height="height"
    />
  </div>
</template>

<script>
export default {
  props: {
    usePanel: {
      type: Boolean,
      default: true,
    },

    titlePanel: String,
    titleChart: String,

    // core value
    current: {
      type: Number,
      required: true,
    },
    max: {
      type: Number,
      default: 100,
    },

    height: {
      type: [Number, String],
      default: 60,
    },

    // styling
    color: {
      type: String,
      default: "#00E396",
    },
    remainingColor: {
      type: String,
      default: "#E0E0E0",
    },
    labelFontSize: {
      type: String,
      default: "14px",
    },
    showLabel: {
      type: Boolean,
      default: true,
    },

    funcFormatter: {
      type: Function,
      default: (val) => `${val}`,
    },
  },

  data() {
    return {
      defaultOptions: {},
    };
  },

  computed: {
    percent() {
      return Math.min((this.current / this.max) * 100, 100);
    },

    computedSeries() {
      console.log([
        {
          name: "Progress",
          data: [this.percent],
        },
        {
          name: "Remaining",
          data: [100 - this.percent],
        },
      ]);
      return [
        {
          name: "Progress",
          data: [this.percent],
        },
        {
          name: "Remaining",
          data: [100 - this.percent],
        },
      ];
    },
  },

  mounted() {
    this.defaultOptions = {
      chart: {
        id: "progress-bar",
        stacked: true,
        sparkline: {
          enabled: true,
        },
        toolbar: {
          show: false,
        },
      },

      plotOptions: {
        bar: {
          horizontal: true,
          barHeight: "100%",
          borderRadius: 6,
        },
      },

      colors: [this.color, this.remainingColor],

      dataLabels: {
        enabled: this.showLabel,
        formatter: (val, opts) => {
          const seriesName = opts.w.globals.seriesNames[opts.seriesIndex];

          if (seriesName === "Progress") return `${this.percent.toFixed(0)}%`;
          if (seriesName === "Remaining") {
            let remaining = 100 - this.percent;
            return `${remaining.toFixed(0)}%`;
          }

          return ""; // hide remaining
        },
        style: {
          fontSize: this.labelFontSize,
          colors: ["#fff", "#FF4560"],
        },
      },

      xaxis: {
        max: 100,
        labels: {
          show: false,
        },
        axisBorder: {
          show: false,
        },
        axisTicks: {
          show: false,
        },
      },

      yaxis: {
        show: false,
      },

      grid: {
        show: false,
      },

      tooltip: {
        enabled: false,
      },

      fill: {
        opacity: 1,
      },

      title: {
        text: this.titleChart || "",
        align: "center",
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
