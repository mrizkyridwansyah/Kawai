<template>
    <div>
      <!-- <h1>Index</h1> -->
    </div>
  </template>
  
  <script>
  export default {
    mounted: function() {
      location.href = '/app'
    }
  }
  </script>
  
  <style lang="scss">
  body,
  input,
  textarea,
  select {
    font-family: Arial;
    font-size: 9pt;
    color: #222;
  }
  ul, ol
  {
    list-style: none;
    padding: 0;
    margin: 0;
  }
  ul.float li {
    float: left;
    padding-right: 15px;
  }
  a {
    text-decoration: none;
  }
  .x-table {
    border-collapse: collapse;
    thead {
      tr {
        th {
          font-size: 8pt;
          font-weight: bold;
          border-bottom: solid 1px #999 !important;
          padding: 2px 10px 2px 5px;
          vertical-align: bottom;
        }
      }
    }
    tbody {
      tr {
        td {
          font-size: 9pt;
          border-bottom: dotted 1px #DCDCDC;
          padding: 2px 10px 2px 5px !important;
          color: #444;
          .small
          {
              font-size: 8pt;
          }
          .link {
            color: #666;
            border-right: solid 1px #999;
            padding-right: 5px;
          }
        }
      }
    }
  }
  </style>