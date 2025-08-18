<template>
  <div class="panel panel-inverse">
    <div class="panel-heading ui-sortable-handle">
      <h4 class="panel-title">{{ titlePanel || "Apex Chart Bar" }}</h4>
    </div>
    <div class="panel-body">
      <div class="chart-container">
        <apexchart
          type="bar"
          height="350"
          :options="defaultOptions"
          :series="series"
        />
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    data: {
      type: Array,
      required: true,
    },
    labels: {
      type: Array,
      required: true,
    },
    rangeSize: {
      type: Number,
      default: 10,
    },
    titlePanel: String,
    titleChart: String,
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
    batasAtas: 0,
    batasBawah: 0,
    median: 0,
    rawData: [],
    series: [],
  }),
  mounted: function () {
    this.getQuartiles();
    this.groupingRawData();
    this.setupChart();
  },
  methods: {
    getMedian: function (data) {
      const sorted = [...data].sort((a, b) => a - b);
      const mid = Math.floor(sorted.length / 2);
      return sorted.length % 2 === 0
        ? (sorted[mid - 1] + sorted[mid]) / 2
        : sorted[mid];
    },
    getQuartiles: function () {
      const sorted = [...this.data].sort((a, b) => a - b);
      const mid = Math.floor(sorted.length / 2);
      const lowerHalf = sorted.slice(0, mid);
      const upperHalf =
        sorted.length % 2 === 0 ? sorted.slice(mid) : sorted.slice(mid + 1);

      this.batasBawah = this.getMedian(lowerHalf);
      this.batasAtas = this.getMedian(upperHalf);
      this.median = (this.batasAtas + this.batasBawah) / 2
    },
    groupingRawData: function () {
      const minVal = Math.min(...this.data);
      const maxVal = Math.max(...this.data);

      const minBin = Math.floor(minVal / this.rangeSize) * this.rangeSize + 1;
      const maxBin = Math.ceil(maxVal / this.rangeSize) * this.rangeSize;

      const bins = [];

      for (let i = minBin; i <= maxBin; i += this.rangeSize) {
        bins.push({
          range: `${i}-${i + this.rangeSize - 1}`,
          min: i,
          max: i + this.rangeSize - 1,
          value: 0,
        });
      }

      for (const num of this.data) {
        const bin = bins.find((b) => num >= b.min && num <= b.max);
        if (bin) bin.value++;
      }

      this.rawData = bins;
    },
    setupChart: function () {
      const sortedData = this.rawData.sort((a, b) => b.min - a.min);
      const categoryPositions = sortedData.map((d, i) => d.min);

      let dtBatasBawah =
        sortedData.find(
          (x) => x.min <= this.batasBawah && x.max >= this.batasBawah
        )?.min || "";
      let dtBatasAtas =
        sortedData.find(
          (x) => x.min <= this.batasAtas && x.max >= this.batasAtas
        )?.min || "";
      let dtMedian =
        sortedData.find((x) => x.min <= this.median && x.max >= this.median)
          ?.min || "";

      this.series = [
        {
          name: "Jumlah",
          data: sortedData.map((d) => d.value),
        },
      ];

      this.defaultOptions = {
        chart: {
          type: "bar",
        },
        plotOptions: {
          bar: {
            horizontal: true,
            distributed: true,
          },
        },
        dataLabels: {
          enabled: true,
          style: {
            fontSize: "12px",
            colors: ["#fff"],
          },
        },
        legend: {
          show: false,
          labels: {
            formatter: (val) => `${val}-${val + 10}`,
          },
        },
        xaxis: {
          categories: categoryPositions, // pakai array posisi numerik
          title: {
            text: "Jumlah",
          },
        },
        yaxis: {
          min: 0,
          max: Math.max(...this.rawData.map((d) => d.value)) + 10,
          labels: {
            formatter: function (val) {
              return (
                sortedData.find((x) => x.min <= val && x.max >= val)?.range ||
                ""
              ); // tampilkan label sesuai posisi
            },
          },
          title: {
            text: "Frekuensi",
          },
        },
        annotations: {
          yaxis: [
            {
              y: dtBatasBawah,
              borderColor: "#00E396",
              strokeDashArray: 4,
              label: {
                borderColor: "#00E396",
                style: {
                  color: "#fff",
                  background: "#00E396",
                },
                text: `Batas Bawah: ${this.batasBawah}`,
                position: "right",
              },
              // Tambahkan garis horizontal
              xAxisIndex: 0,
            },
            {
              y: dtBatasAtas,
              borderColor: "#FF4560",
              strokeDashArray: 4,
              label: {
                borderColor: "#FF4560",
                style: {
                  color: "#fff",
                  background: "#FF4560",
                },
                text: `Batas Atas: ${this.batasAtas}`,
                position: "right",
              },
            },
            {
              y: dtMedian,
              borderColor: "#775DD0",
              strokeDashArray: 4,
              label: {
                borderColor: "#775DD0",
                style: {
                  color: "#fff",
                  background: "#775DD0",
                },
                text: `Median: ${this.median}`,
                position: "right",
              },
            },
          ],
        },
        tooltip: {
          y: {
            formatter: (val) => `${val}`,
          },
        },
      };
    },
  },
};
</script>
