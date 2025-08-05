<template>
  <div class="row">
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-stacked-bar
        title-chart="Chart Grouped Stacked Bar"
        :labels="[
          'Online advertising',
          'Sales Training',
          'Print advertising',
          'Catalogs',
          'Meetings',
        ]"
        :series="[
          {
            name: 'Q1 Budget',
            group: 'budget',
            data: [44000, 55000, 41000, 67000, 22000],
          },
          {
            name: 'Q1 Actual',
            group: 'actual',
            data: [48000, 50000, 40000, 65000, 25000],
          },
          {
            name: 'Q2 Budget',
            group: 'budget',
            data: [13000, 36000, 20000, 8000, 13000],
          },
          {
            name: 'Q2 Actual',
            group: 'actual',
            data: [20000, 40000, 25000, 10000, 12000],
          },
        ]"
        :color-bar="['#80c7fd', '#008FFB', '#80f1cb', '#00E396']"
        :horizontal="true"
        :func-data-label-formatter="(val) => `${this.$func.formatMoney(val)}`"
        :func-formatter="(val) => `${this.$func.formatMoney(val)}`"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-stacked-bar
        title-chart="Leadtime"
        :labels="['Standard', 'Actual']"
        :series="[
          { name: 'Assy', data: [1, 1] },
          { name: 'Aging', data: [5, 6] },
          { name: 'Cooling', data: [1, 2] },
        ]"
        :func-data-label-formatter="
          (val) => `${this.$func.formatMoney(val)} Days`
        "
        :func-formatter="(val) => `${this.$func.formatMoney(val)} Days`"
        :horizontal="true"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-stacked-bar
        title-chart="Chart Stacked Bar"
        :labels="['Q1', 'Q2', 'Q3', 'Q4']"
        :series="[
          { name: 'Marketing', data: [20, 30, 40, 30] },
          { name: 'Sales', data: [10, 20, 40, 30] },
          { name: 'Development', data: [15, 35, 25, 25] },
        ]"
        stack-type="100%"
        :func-data-label-formatter="(val) => `${Math.round(val).toFixed(0)} %`"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-line
        title-chart="Line Chart"
        :series="[
          { name: 'Sales', data: [30, 40, 35, 50, 49, 60, 70, 91] },
          { name: 'Sales', data: [35, 45, 35, 55, 49, 65, 75, 91] },
          { name: 'Sales', data: [39, 49, 39, 99, 49, 69, 79, 91] },
        ]"
        :labels="['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug']"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-bar
        title-chart="Chart Bar"
        :series="[
          {
            name: 'Sales',
            data: [30000, 40000, 35000, 50000, 49000, 60000, 70000, 91000],
          },
          {
            name: 'Sales',
            data: [30000, 40000, 35000, 50000, 49000, 60000, 70000, 91000],
          },
        ]"
        :labels="['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug']"
        :horizontal="false"
        :color-bar="['salmon', 'blue']"
        :enable-datalabel="false"
        :func-data-label-formatter="
          (val) => `Rp ${this.$func.formatMoney(val)}`
        "
        :func-formatter="(val) => `Rp ${this.$func.formatMoney(val)}`"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-pareto
        title-chart="Pareto"
        title-panel="Chart Pareto"
        :labels="['Bulan 1', 'Bulan 2', 'Bulan 3', 'Bulan 4', 'Bulan 5', 'Bulan 6', 'Bulan 7', 'Bulan 8']"
        :data="[22, 6, 5, 2, 1, 5, 7, 8]"
        :color-bar="['salmon', 'blue']"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-mixed
        title-chart="Mixed Line Column"
        :labels="['Jan', 'Feb', 'Mar', 'Apr']"
        :series="[
          { name: 'Revenue', type: 'column', data: [400, 430, 448, 470] },
          { name: 'Growth %', type: 'line', data: [10, 20, 15, 25] },
        ]"
        :color-bar="['salmon', 'blue']"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-mixed-multi-yaxis
        title-chart="Mixed Multi Y-Axis"
        :labels="[2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016]"
        :series="[
          {
            name: 'Income',
            type: 'column',
            data: [1.4, 2, 2.5, 1.5, 2.5, 2.8, 3.8, 4.6],
          },
          {
            name: 'Cashflow',
            type: 'column',
            data: [1.1, 3, 3.1, 4, 4.1, 4.9, 6.5, 8.5],
          },
          {
            name: 'Revenue',
            type: 'line',
            data: [20, 29, 37, 36, 44, 45, 50, 58],
          },
        ]"
        :y-axis="[
          {
            key: 'Income',
            value: 'Income (Rp)',
            opposite: false,
            color: '#008FFB',
          },
          {
            key: 'Cashflow',
            value: 'Cashflow (Rp)',
            opposite: true,
            color: '#00E396',
          },
          {
            key: 'Revenue',
            value: 'Revenue (Rp)',
            opposite: true,
            color: '#FEB019',
          },
        ]"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-mixed-type
        title-chart="Mixed Line & Area Chart"
        :labels="[
          '01-12-2025',
          '02-12-2025',
          '03-12-2025',
          '04-12-2025',
          '05-12-2025',
          '06-12-2025',
          '07-12-2025',
          '08-12-2025',
          '09-12-2025 ',
          '10-12-2025',
          '11-12-2025',
        ]"
        :series="[
          {
            name: 'TEAM A',
            type: 'area',
            data: [44, 55, 31, 47, 31, 43, 26, 41, 31, 47, 33],
          },
          {
            name: 'TEAM B',
            type: 'line',
            data: [55, 69, 45, 61, 43, 54, 37, 52, 44, 61, 43],
          },
        ]"
        :y-axis="[
          {
            title: {
              text: 'Series A',
            },
          },
          {
            opposite: true,
            title: {
              text: 'Series B',
            },
          },
        ]"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-mixed-type
        title-chart="Mixed Line Column Area Chart"
        :labels="[
          '01-12-2025',
          '02-12-2025',
          '03-12-2025',
          '04-12-2025',
          '05-12-2025',
          '06-12-2025',
          '07-12-2025',
          '08-12-2025',
          '09-12-2025 ',
          '10-12-2025',
          '11-12-2025',
        ]"
        :series="[
          {
            name: 'TEAM A',
            type: 'area',
            data: [44, 55, 31, 47, 31, 43, 26, 41, 31, 47, 33],
          },
          {
            name: 'TEAM B',
            type: 'line',
            data: [55, 69, 45, 61, 43, 54, 37, 52, 44, 61, 43],
          },
          {
            name: 'TEAM C',
            type: 'column',
            data: [19, 29, 39, 59, 19, 19, 49, 59, 49, 69, 43],
          },
        ]"
        :y-axis="[
          {
            labels: {
              formatter: (val) => `${val}`,
            },
          },
        ]"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-brush
        title="Brush Chart Example"
        :series="chartSeries"
        :initial-min="new Date('2022-01-01').getTime()"
        :initial-max="new Date('2022-01-30').getTime()"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-timeline
        title="Timeline Chart Example"
        :series="[
          {
            name: 'Normal', // Group Flow
            data: [
              {
                x: 'A01',
                y: [
                  new Date('2025-06-01').getTime(),
                  new Date('2025-06-02').getTime(),
                ],
                process: 'Assy',
              },
              {
                x: 'A01',
                y: [
                  new Date('2025-06-02').getTime(),
                  new Date('2025-06-04').getTime(),
                ],
                process: 'Aging',
              },
              {
                x: 'A01',
                y: [
                  new Date('2025-06-04').getTime(),
                  new Date('2025-06-05').getTime(),
                ],
                process: 'Cooling',
              },
              {
                x: 'A02',
                y: [
                  new Date('2025-06-01').getTime(),
                  new Date('2025-06-02').getTime(),
                ],
                process: 'Assy',
              },
              {
                x: 'A02',
                y: [
                  new Date('2025-06-02').getTime(),
                  new Date('2025-06-04').getTime(),
                ],
                process: 'Aging',
              },
              {
                x: 'A02',
                y: [
                  new Date('2025-06-04').getTime(),
                  new Date('2025-06-05').getTime(),
                ],
                process: 'Cooling',
              },
            ],
          },
          {
            name: 'Hold 1',
            data: [
              {
                x: 'A01',
                y: [
                  new Date('2025-06-05').getTime(),
                  new Date('2025-06-06').getTime(),
                ],
                process: 'Aging',
              },
              {
                x: 'A01',
                y: [
                  new Date('2025-06-06').getTime(),
                  new Date('2025-06-07').getTime(),
                ],
                process: 'Cooling',
              },
            ],
          },
        ]"
        :custom-tooltip="
          ({ series, seriesIndex, dataPointIndex, w }) =>
            customtooltip({ series, seriesIndex, dataPointIndex, w })
        "
        :custom-legend="{
          Assy: '#008FFB',
          Aging: '#FEB019',
          Cooling: '#00E396',
        }"
        :func-data-label-formatter="(val) => funcDataLabelFormatter(val)"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-histogram-bar
        :data="[12, 15, 17, 22, 23, 25, 28, 31, 32, 46, 45]"
      />
    </div>
    <!-- <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <apex-chart-histogram-line
        :data="[12, 15, 17, 22, 23, 25, 28, 31, 32, 46, 45]"
      />
    </div> -->
  </div>

  <div class="row">
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-mixed
        title="Mixed Chart Example"
        :labels="mixedLabels"
        :datasets="mixedDatasets"
        :options="mixedOptions"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-horizontal-bar
        title="Horizontal Bar Chart Example"
        :labels="horizontalBarLabels"
        :datasets="horizontalBarDatasets"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-stackedbar
        title="Stacked Bar Chart Example"
        :datasets="stackedDatasets"
        :labels="stackedLabels"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-bar
        title="Bar Chart Example"
        :labels="barLabels"
        :datasets="barDatasets"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-line
        title="Line Chart Example"
        :labels="lineLabels"
        :datasets="lineDatasets"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-pie
        title="Pie Chart Example"
        :labels="pieLabels"
        :datasets="pieDatasets"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-doughnut
        title="Doughnut Chart Example"
        :labels="doughnutLabels"
        :datasets="doughnutDatasets"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-polar
        title="Polar Chart Example"
        :labels="polarLabels"
        :datasets="polarDatasets"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-radar
        title="Radar Chart Example"
        :labels="radarLabels"
        :datasets="radarDatasets"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-bubble title="Bubble Chart Example" :datasets="bubbleDatasets" />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-scatter
        title="Scatter Chart Example"
        :datasets="scatterDatasets"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-horizontal-stacked-bar
        title="Sales by Region"
        :labels="['North', 'South', 'East', 'West']"
        :datasets="[
          { label: 'Q1', data: [120, 90, 70, 100], backgroundColor: '#3b82f6' },
          { label: 'Q2', data: [80, 100, 60, 90], backgroundColor: '#10b981' },
        ]"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-pareto
        title="Pareto of Defects"
        :labels="['Jan', 'Feb', 'Mar', 'Apr']"
        :values="[22, 6, 5, 2]"
        bar-color="lightblue"
        line-color="salmon"
        border-color="blue"
      />
    </div>
    <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-histogram-bar
        :labels="['A', 'B', 'C', 'D', 'E', 'F']"
        :data="[0, 12, 19, 3, 5, 1]"
        title="Histogram Bar Chart"
      />
    </div>
    <!-- <div class="col-lg-6 col-md-6 col-sm-12 col-12">
      <charts-histogram-line
        :labels="['A', 'B', 'C', 'D', 'E', 'F']"
        :data="[0,12, 19, 3, 5, 1]"
        title="Histogram Line Chart"
      />
    </div> -->
  </div>
</template>

<script>
import moment from "moment";

function generateData(startDate, count, min, max) {
  const series = [];
  let baseTime = new Date(startDate).getTime();

  for (let i = 0; i < count; i++) {
    const y = Math.floor(Math.random() * (max - min + 1)) + min;
    series.push([baseTime, y]);
    baseTime += 86400000; // add 1 day
  }

  return series;
}

export default {
  data: () => ({
    chartSeries: [
      {
        name: "Trouble",
        data: generateData("2022-01-01", 100, 10, 90),
      },
      {
        name: "Downtime",
        data: generateData("2022-01-01", 100, 20, 100),
      },
    ],
    // Bar Chart
    barLabels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"],
    barDatasets: [
      {
        label: "Sales",
        backgroundColor: "rgba(75, 192, 192, 0.6)",
        data: [12, 19, 3, 5, 2, 3],
      },
    ],

    // Line Chart
    lineLabels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"],
    lineDatasets: [
      {
        label: "Visitors",
        borderColor: "rgba(54, 162, 235, 1)",
        backgroundColor: "rgba(54, 162, 235, 0.2)",
        fill: true,
        data: [30, 45, 28, 80, 99, 43],
      },
    ],

    // Pie Chart
    pieLabels: ["Red", "Blue", "Yellow"],
    pieDatasets: [
      {
        label: "Votes",
        backgroundColor: ["#FF6384", "#36A2EB", "#FFCE56"],
        data: [12, 19, 7],
      },
    ],

    // Doughnut Chart
    doughnutLabels: ["Chrome", "Firefox", "Edge"],
    doughnutDatasets: [
      {
        label: "Usage",
        backgroundColor: ["#FF6384", "#36A2EB", "#FFCE56"],
        data: [55, 25, 20],
      },
    ],

    // Polar Area Chart
    polarLabels: ["Red", "Green", "Yellow", "Grey", "Blue"],
    polarDatasets: [
      {
        label: "Popularity",
        backgroundColor: [
          "rgba(255, 99, 132, 0.5)",
          "rgba(75, 192, 192, 0.5)",
          "rgba(255, 205, 86, 0.5)",
          "rgba(201, 203, 207, 0.5)",
          "rgba(54, 162, 235, 0.5)",
        ],
        data: [11, 16, 7, 3, 14],
      },
    ],

    // Radar Chart
    radarLabels: [
      "Eating",
      "Drinking",
      "Sleeping",
      "Designing",
      "Coding",
      "Cycling",
      "Running",
    ],
    radarDatasets: [
      {
        label: "Person A",
        backgroundColor: "rgba(255, 99, 132, 0.2)",
        borderColor: "rgba(255, 99, 132, 1)",
        pointBackgroundColor: "rgba(255, 99, 132, 1)",
        data: [65, 59, 90, 81, 56, 55, 40],
      },
      {
        label: "Person B",
        backgroundColor: "rgba(54, 162, 235, 0.2)",
        borderColor: "rgba(54, 162, 235, 1)",
        pointBackgroundColor: "rgba(54, 162, 235, 1)",
        data: [28, 48, 40, 19, 96, 27, 100],
      },
    ],

    // Bubble Chart
    bubbleDatasets: [
      {
        label: "Bubble Dataset",
        backgroundColor: "rgba(255, 99, 132, 0.5)",
        data: [
          { x: 20, y: 30, r: 15 },
          { x: 40, y: 10, r: 10 },
          { x: 25, y: 40, r: 12 },
        ],
      },
    ],

    // Scatter Chart
    scatterDatasets: [
      {
        label: "Scatter Dataset",
        backgroundColor: "rgba(75, 192, 192, 0.5)",
        data: [
          { x: -10, y: 0 },
          { x: 0, y: 10 },
          { x: 10, y: 5 },
          { x: 0.5, y: 5.5 },
        ],
      },
    ],

    // Stacked Bar Chart
    stackedLabels: ["Q1", "Q2", "Q3", "Q4"],
    stackedDatasets: [
      {
        label: "Product A",
        backgroundColor: "rgba(255, 99, 132, 0.6)",
        data: [12, 19, 3, 5],
      },
      {
        label: "Product B",
        backgroundColor: "rgba(54, 162, 235, 0.6)",
        data: [2, 3, 20, 3],
      },
    ],

    // Horizontal Bar Chart (reuse barLabels for simplicity)
    horizontalBarLabels: ["January", "February", "March", "April"],
    horizontalBarDatasets: [
      {
        label: "Revenue",
        backgroundColor: "rgba(153, 102, 255, 0.6)",
        data: [10, 15, 12, 8],
      },
    ],

    // Mixed Chart
    mixedLabels: ["January", "February", "March", "April", "May"],
    mixedDatasets: [
      {
        type: "bar",
        label: "Bar Dataset",
        backgroundColor: "rgba(54, 162, 235, 0.5)",
        data: [10, 20, 30, 40, 50],
      },
      {
        type: "line",
        label: "Line Dataset",
        borderColor: "rgba(255, 99, 132, 1)",
        borderWidth: 2,
        fill: false,
        data: [50, 40, 30, 20, 10],
      },
    ],
    mixedOptions: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: "top" },
        title: { display: true, text: "Dynamic Mixed Chart" },
      },
      scales: {
        y: { beginAtZero: true },
      },
    },
    funcDataLabelFormatter: function (val) {
      let a = moment(val[0]);
      let b = moment(val[1]);
      let diff = b.diff(a, "days");
      return diff + (diff > 1 ? " days" : " day");
    },
    customtooltip: function ({ series, seriesIndex, dataPointIndex, w }) {
      const d = w.globals.initialSeries[seriesIndex].data[dataPointIndex];
      const group = w.globals.initialSeries[seriesIndex].name;
      const start = this.$func.formatDate(d.y[0]);
      const end = this.$func.formatDate(d.y[1]);
      return `
      <div style="
        padding: 10px;
        background: #fff;
        border: 1px solid #ddd;
        border-radius: 8px;
        box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        min-width: 200px;
      ">
        <div style="font-weight: bold; font-size: 14px; margin-bottom: 5px; color: #333;">
          Lot No: ${d.x}
        </div>
        <div style="margin-bottom: 4px;">
          <span style="font-weight: 500; color: #888;">Group:</span>
          <span style="color: #000;">${group}</span>
        </div>
        <div style="margin-bottom: 4px;">
          <span style="font-weight: 500; color: #888;">Process:</span>
          <span style="color: #000;">${d.process}</span>
        </div>
        <div style="margin-bottom: 4px;">
          <span style="font-weight: 500; color: #888;">Start:</span>
          <span style="color: #000;">${start}</span>
        </div>
        <div>
          <span style="font-weight: 500; color: #888;">End:</span>
          <span style="color: #000;">${end}</span>
        </div>
      </div>
    `;
    },
  }),
};
</script>
