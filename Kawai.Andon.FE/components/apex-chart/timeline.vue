<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <h4 class="panel-title">{{ titlePanel || "Apex Chart Timeline" }}</h4>
    </div>
    <div class="panel-body">
      <div class="chart-container">
        <apexchart
          ref="chart"
          type="rangeBar"
          :options="defaultOptions"
          :series="filteredSeries"
          height="350"
        />
      </div>

      <!-- Custom Legend -->
      <div class="custom-legend">
        <div
          v-for="(color, process) in processColorMap"
          :key="process"
          class="legend-item"
          @click="toggleProcess(process)"
        >
          <span
            class="legend-color"
            :style="{
              backgroundColor: visibleProcesses.includes(process)
                ? color
                : '#ccc',
            }"
          ></span>
          <span
            :style="{
              textDecoration: visibleProcesses.includes(process)
                ? 'none'
                : 'line-through',
              opacity: visibleProcesses.includes(process) ? 1 : 0.4,
            }"
          >
            {{ process }}
          </span>
        </div>
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
    titlePanel: String,
    titleChart: String,
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
    customTooltip: Function,
  },
  data: () => ({
    defaultOptions: {},
    processColorMap: {}, // process -> color
    visibleProcesses: [], // auto dari props
  }),
  computed: {
    filteredSeries: function () {
      return this.series.map((s) => ({
        ...s,
        data: s.data
          .filter((d) => this.visibleProcesses.includes(d.process))
          .map((d) => ({
            ...d,
            fillColor: this.processColorMap[d.process] || "#999",
          })),
      }));
    },
  },
  mounted: function () {
    this.setupVisibleProcesses();
    this.setupChart();
  },
  methods: {
    setupVisibleProcesses: function () {
      // Ambil semua unique process
      const processes = new Set();
      this.series.forEach((s) => {
        s.data.forEach((d) => {
          if (d.process) processes.add(d.process);
        });
      });

      this.visibleProcesses = Array.from(processes);

      // Assign warna otomatis
      const defaultColors = [
        "#008FFB",
        "#FEB019",
        "#00E396",
        "#FF4560",
        "#775DD0",
        "#3F51B5",
        "#546E7A",
        "#D4526E",
        "#8D5B4C",
        "#F86624",
      ];
      let colorMap = {};
      this.visibleProcesses.forEach((proc, i) => {
        colorMap[proc] = defaultColors[i % defaultColors.length];
      });
      this.processColorMap = colorMap;
    },

    setupChart: function () {
      this.defaultOptions = {
        chart: {
          type: "rangeBar",
          toolbar: { show: true },
          zoom: { enabled: false },
        },
        plotOptions: {
          bar: {
            horizontal: true,
            barHeight: "50%",
          },
        },
        dataLabels: {
          enabled: this.enableDatalabel,
          formatter: this.funcDataLabelFormatter,
        },
        xaxis: {
          type: "datetime",
        },
        fill: {
          type: "solid",
        },
        legend: {
          show: false, // custom legend
        },
        tooltip: {
          custom: this.customTooltip,
        },
        title: {
          text: this.titleChart || "Apex Timeline Chart",
          align: "center",
          style: {
            fontSize: "18px",
            fontWeight: "bold",
          },
        },
      };
    },

    toggleProcess: function (process) {
      if (this.visibleProcesses.includes(process)) {
        this.visibleProcesses = this.visibleProcesses.filter(
          (p) => p !== process
        );
      } else {
        this.visibleProcesses.push(process);
      }
    },
  },
};
</script>

<style scoped>
.chart-container {
  max-width: 100%;
}
.custom-legend {
  display: flex;
  gap: 12px;
  margin-top: 10px;
  flex-wrap: wrap;
  cursor: pointer;
  justify-content: center;
}
.legend-item {
  display: flex;
  align-items: center;
  font-size: 12px;
  cursor: pointer;
}
.legend-color {
  width: 14px;
  height: 14px;
  margin-right: 6px;
  border-radius: 2px;
}
</style>
