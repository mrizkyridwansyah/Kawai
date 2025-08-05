<template>
  <div class="panel panel-inverse">
    <div class="panel-heading">
      <h4 class="panel-title">{{ titlePanel || "Pareto Chart" }}</h4>
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
    data: {
      type: Array,
      required: true,
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
      default: true,
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
  data() {
    return {
      chartOptions: {},
      series: [],
    };
  },
  mounted() {
    this.setupChart();
  },
  methods: {
    setupChart() {
      const sorted = this.data
        .map((v, i) => ({ value: v, label: this.labels[i] }))
        // .sort((a, b) => b.value - a.value);

      const sortedValues = sorted.map((v) => v.value);
      const sortedLabels = sorted.map((v) => v.label);

      const total = sortedValues.reduce((a, b) => a + b, 0);
      const cumulative = sortedValues.reduce((acc, val, i) => {
        acc.push((acc[i - 1] || 0) + val);
        return acc;
      }, []);

      const percentage = cumulative.map((val) =>
        Number(((val / total) * 100).toFixed(0))
      );

      const maxValue = sortedValues.reduce((acc, idx) => acc + idx, 0);
      const barData = sorted.map((v, i) => ({
        x: i,
        y: v.value,
      }));

      const lineData = sorted.map((v, i) => ({
        x: i + 0.5,
        y: percentage[i],
      }));

      this.series = [
        {
          name: "Jumlah",
          type: "column",
          // yAxisIndex: 1,
          data: barData,
        },
        {
          name: "Kumulatif %",
          type: "line",
          data: [{ x: 0, y: 0 }, ...lineData],
        },
      ];

      this.chartOptions = {
        chart: {
          type: "line",
          stacked: false,
          toolbar: { show: true },
          zoom: {enabled: false}
        },
        title: {
          text: this.titleChart || "Pareto Chart",
          align: "left",
          style: {
            fontSize: "18px",
            fontWeight: "bold",
          },
        },
        stroke: {
          width: [0, 3],
          //   curve: "smooth",
        },
        plotOptions: {
          bar: {
            columnWidth: "120%",
            distributed: true,
            barPadding: 0,
            endingShape: "flat",
          },
        },
        colors: this.colors,
        dataLabels: {
          enabled: false,
        },
        xaxis: {
          type: "category",
          labels: {
            rotate: -45,
            style: {
              fontSize: "12px",
            },
          },
        },
        yaxis: [
          {
            title: { text: "Jumlah" },
            min: 0,
            max: maxValue, // rounded up
          },
          {
            opposite: true,
            title: { text: "Kumulatif %" },
            min: 0,
            max: 100,
          },
        ],
        grid: {
          padding: {
            // left: -100  ,
            right: 0,
            top: 20,
            bottom: 0,
          },
        },
        tooltip: {
          shared: true,
          intersect: false,
          y: {
            formatter: this.funcFormatter,
          },
        },
        legend: {
          position: "bottom",
        },
      };
    },
  },
};
</script>

<style scoped>
.chart-container {
  width: 100%;
}
</style>
