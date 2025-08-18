<template>
  <div class="panel panel-inverse">
    <div class="panel-heading">
      <h4 class="panel-title">{{ title }}</h4>
    </div>
    <div class="panel-body">
      <div id="wrapper">
        <!-- Main Chart -->
        <div id="chart-line2">
          <apexchart
            type="line"
            height="230"
            :options="mainOptions"
            :series="mainSeries"
          />
        </div>

        <!-- Brush Chart -->
        <div id="chart-line">
          <apexchart
            type="area"
            height="130"
            :options="brushOptions"
            :series="series"
            @selection="onBrushSelection"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "BrushChart",
  props: {
    title: { type: String, default: "Brush Chart" },
    series: { type: Array, required: true },
    initialMin: { type: Number, required: true },
    initialMax: { type: Number, required: true },
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
    fullSeries: [],
    mainSeries: [],
    colors: ["#008FFB", "#FF4560"],
  }),
  computed: {
    mainOptions: function () {
      const axis1 = this.mainSeries?.[0]?.name || "Axis 1";
      const axis2 = this.mainSeries?.[1]?.name || "Axis 2";

      return {
        chart: {
          id: "mainChart",
          type: "line",
          height: 230,
          toolbar: { autoSelected: "pan", show: true },
          zoom: { enabled: false },
        },
        colors: this.colors,
        dataLabels: {
          enabled: this.enableDatalabel,
          formatter: this.funcDataLabelFormatter,
        },
        stroke: {
          width: [2, 3],
          curve: ["straight", "smooth"],
        },
        fill: { opacity: [1, 0.6] },
        markers: { size: 0 },
        xaxis: { type: "datetime" },
        yaxis: this.series.map((s, i) => ({
          title: {
            text: s.name,
            style: { color: this.colors[i] },
          },
          labels: { style: { colors: this.colors[i] } },
          opposite: i % 2 !== 0,
          axisTicks: { show: true },
          axisBorder: { show: true, color: this.colors[i] },
        })),
      };
    },
    brushOptions: function () {
      return {
        chart: {
          id: "brushChart",
          type: "area",
          height: 130,
          brush: {
            target: "mainChart",
            enabled: true,
          },
          selection: {
            enabled: true,
            xaxis: {
              min: this.initialMin,
              max: this.initialMax,
            },
          },
        },
        colors: this.colors,
        stroke: {
          width: [1, 2],
          curve: ["straight", "smooth"],
        },
        fill: {
          type: "gradient",
          gradient: {
            opacityFrom: 0.91,
            opacityTo: 0.1,
          },
        },
        xaxis: {
          type: "datetime",
          tooltip: { enabled: false },
        },
        yaxis: {
          tickAmount: 2,
        },
      };
    },
  },
  mounted: function () {
    this.fullSeries = this.series;
    this.updateMainChartData(this.initialMin, this.initialMax);
  },
  methods: {
    filterByRange: function (data, min, max) {
      return data.filter(([ts]) => ts >= min && ts <= max);
    },
    updateMainChartData: function (min, max) {
      this.mainSeries = this.fullSeries.map((serie) => ({
        name: serie.name,
        data: this.filterByRange(serie.data, min, max),
      }));
    },
    onBrushSelection: function (event, config) {
      const { xaxis } = config || {};
      if (xaxis?.min != null && xaxis?.max != null) {
        this.updateMainChartData(xaxis.min, xaxis.max);
      }
    },
  },
};
</script>

<style scoped>
.panel-body {
  padding: 1rem;
}
</style>
