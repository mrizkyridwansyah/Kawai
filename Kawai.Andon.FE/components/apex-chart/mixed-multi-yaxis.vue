<template>
  <div class="panel panel-inverse">
    <div class="panel-heading">
      <h4 class="panel-title">{{ titlePanel || "Multi Y-Axis Chart" }}</h4>
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
  name: "MultiYAxisChart",
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
    colors: {
      type: Array,
      default: () => ["#008FFB", "#00E396", "#FEB019"],
    },
    yAxis: {
      type: Array,
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
    let yAxisOpt = this.yAxis.map((p) => {
      return p.opposite
        ? {
            opposite: p.opposite,
            seriesName: p.key,
            axisTicks: {
              show: true,
            },
            axisBorder: {
              show: true,
              color: p.color,
            },
            labels: {
              style: {
                colors: p.color,
              },
            },
            title: {
              text: p.value,
              style: {
                color: p.color,
              },
            },
            tooltip: {
              enabled: true,
            },
          }
        : {
            seriesName: p.key,
            axisTicks: {
              show: true,
            },
            axisBorder: {
              show: true,
              color: p.color,
            },
            labels: {
              style: {
                colors: p.color,
              },
            },
            title: {
              text: p.value,
              style: {
                color: p.color,
              },
            },
            tooltip: {
              enabled: true,
            },
          };
    });

    this.chartOptions = {
      chart: {
        height: 350,
        type: "line",
        stacked: false,
        toolbar: {
          show: true,
        },
      },
      colors: this.colors,
      dataLabels: {
        enabled: this.enableDatalabel,
        formatter: this.funcDataLabelFormatter
      },
      stroke: {
        width: [3, 0, 3],
        curve: "smooth",
      },
      title: {
        text: this.titleChart || "Multi Y-Axis Chart",
        align: "left",
        style: {
          fontSize: "18px",
          fontWeight: "bold",
        },
      },
      xaxis: {
        categories: this.labels,
      },
      yaxis: yAxisOpt,
      tooltip: {
        shared: true,
        intersect: false,
        y: {
            formatter: this.funcFormatter
        }
      },
      legend: {
        position: "bottom",
        horizontalAlign: "center",
        offsetX: 40,
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
