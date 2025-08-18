export default defineNuxtPlugin(nuxtApp => {
  nuxtApp.provide('export', {
    excel: (function () {
      var uri = 'data:application/vnd.ms-excel;base64,'
        , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--><meta http-equiv="content-type" content="text/plain; charset=UTF-8"/></head><body><table>{table}</table></body></html>'
        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
      return function (table, name) {
        if (!table.nodeType) table = document.getElementById(table)
        var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
        var link = document.createElement('a');
        link.href = uri + base64(format(template, ctx));
        link.download = (name || 'Worksheet') + '.xls';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      }
    })(),
    csv: (function () {
      return function (tableId, filename) {
        var table = document.getElementById(tableId);
        var csv = [];
        var rows = table.querySelectorAll('tr');

        for (var i = 0; i < rows.length; i++) {
          var row = rows[i];
          var cols = row.querySelectorAll('td, th');
          var csvRow = [];

          for (var j = 0; j < cols.length; j++) {
            var cell = cols[j];
            var text = cell.textContent || cell.innerText;

            // Escape double quotes
            text = text.replace(/"/g, '""');

            // Add to CSV row
            csvRow.push('"' + text + '"');
          }

          // Join row and add to CSV array
          csv.push(csvRow.join(','));
        }

        // Join all rows into CSV content
        var csvContent = 'data:text/csv;charset=utf-8,' + csv.join('\n');

        // Encode URI for download
        var encodedUri = encodeURI(csvContent);
        var link = document.createElement('a');
        link.setAttribute('href', encodedUri);
        link.setAttribute('download', (filename || 'data') + '.csv');
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      }
    })()
  })
})
