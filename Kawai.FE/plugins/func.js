import moment from "moment";
export default defineNuxtPlugin(nuxtApp => {
  nuxtApp.provide('func', {
    formatDate: function (date, format = "DD MMM YYYY") {
      if (date == null) return null;
      return moment(date).format(format);
    },
    formatDateMonth: function (date, format = "DD MMM") {
      if (date == null) return null;
      return moment(date).format(format);
    },
    formatDateTime: function (date, format = "DD MMM YYYY HH:mm") {
      if (date == null) return null;
      return moment(date).format(format);
    },
    formatTime: function (date, format = "HH:mm") {
      if (date == null) return null;
      return moment(date).format(format);
    },
    formatDateFromNow: function (date) {
      if (date == null) return null;
      return moment(date).fromNow();
    },
    formatNumber: function (num) {
      if (!!!num) return 0;

      let result = this.formatIfExponential(num);
      var parts = result.toString().split(".");
      parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
      return parts.join(".");
    },
    formatIfExponential: function (val) {
      const str = String(val);

      // Cek apakah bentuk eksponensial (ada huruf 'e' atau 'E')
      if (str.includes("e") || str.includes("E")) {
        console.log(str);
        return Number(val).toFixed(9); // konversi ke desimal string
      }

      return val;
    },
    formatMoney: function (num) {
      if (!!!num) return 0;

      let result = this.formatIfExponential(num);
      var parts = result.toString().split(".");
      parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
      return parts.join(".");
    },
    formatMoneyK: function (num) {
      if (!!!num) return 0;

      let result = this.formatIfExponential(num);
      var parts = result.toString().split(".");
      parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
      parts[0] = parts[0].split(',').slice(0, -1).join(',');
      return parts.join(".");
    },
    formatDateDay: function (date) {
      if (date == null) return null;
      return moment(date).format("dddd, D MMMM YYYY");
    },
    dateInRange: function (date, before, after) {
      return (
        moment(date).isBefore(new Date(before), "day") || moment(date).isAfter(new Date(after), "day")
      );
    },
    dateOutOfRange: function (date, before, after) {
      return moment(date).isAfter(new Date(after)) || moment(date).isBefore(before);
    },
    getInitialName: function (text = "") {
      return text
        .match(/(^\S\S?|\b\S)?/g)
        .join("")
        .match(/(^\S|\S$)?/g)
        .join("")
        .toUpperCase();
    },
    dateDiffInDays: function(d1, d2) {
      d1 = moment(d1, 'YYYY-MM-DD');
      d2 = moment(d2, 'YYYY-MM-DD');
      if(!d1.isValid())
        return null;
      
      if(!d2.isValid())
        return null;

      var t2 = d2.toDate().getTime();
      var t1 = d1.toDate().getTime();

      return Math.floor((t2-t1)/(24*3600*1000));
    },
    dateDiffInHourMinutes: function(d1, d2) {
      d1 = moment(d1);
      d2 = moment(d2);
      if(!d1.isValid())
        return null;
      
      if(!d2.isValid())
        return null;

      var durationH = moment.duration(d2.diff(d1)).asHours();
      var durationM = moment.duration(d2.diff(d1)).asMinutes();

      return `${durationH >= 1 ? Math.floor(durationH) : 0}h ${(durationM >= 60 ? (durationM - (Math.floor(durationH) * 60)) : durationM)}m`;
    },
    dateDiffInHours: function(d1, d2) {
      d1 = moment(d1);
      d2 = moment(d2);
      if(!d1.isValid())
        return null;
      
      if(!d2.isValid())
        return null;

      var duration = moment.duration(d2.diff(d1));

      return duration.asHours();
    },
    dateDiffInWeeks: function(d1, d2) {
      if(!(d1 instanceof Date))
        return null;
      
      if(!(d2 instanceof Date))
        return null;

      var t2 = d2.getTime();
      var t1 = d1.getTime();

      return parseInt((t2-t1)/(24*3600*1000*7));
    },
    dateDiffInMonths: function(d1, d2) {
      console.log(d1, d2)
      if(!(d1 instanceof Date))
        return null;
      
      if(!(d2 instanceof Date))
        return null;

      var d1Y = d1.getFullYear();
      var d2Y = d2.getFullYear();
      var d1M = d1.getMonth();
      var d2M = d2.getMonth();

      return ((d2M+12*d2Y)-(d1M+12*d1Y))+1;
    },
    dateDiffInYears: function(d1, d2) {
      if(!(d1 instanceof Date))
        return null;
      
      if(!(d2 instanceof Date))
        return null;

      return d2.getFullYear() - d1.getFullYear();
    },
    fromNow: function (date, nowDate = Date.now(), rft = new Intl.RelativeTimeFormat(undefined, { numeric: "auto" })) {
      const SECOND = 1000;
      const MINUTE = 60 * SECOND;
      const HOUR = 60 * MINUTE;
      const DAY = 24 * HOUR;
      const WEEK = 7 * DAY;
      const MONTH = 30 * DAY;
      const YEAR = 365 * DAY;
      const intervals = [
          { ge: YEAR, divisor: YEAR, unit: 'year' },
          { ge: MONTH, divisor: MONTH, unit: 'month' },
          { ge: WEEK, divisor: WEEK, unit: 'week' },
          { ge: DAY, divisor: DAY, unit: 'day' },
          { ge: HOUR, divisor: HOUR, unit: 'hour' },
          { ge: MINUTE, divisor: MINUTE, unit: 'minute' },
          { ge: 30 * SECOND, divisor: SECOND, unit: 'seconds' },
          { ge: 0, divisor: 1, text: 'just now' },
      ];
      const now = typeof nowDate === 'object' ? nowDate.getTime() : new Date(nowDate).getTime();
      const diff = now - (typeof date === 'object' ? date : new Date(date)).getTime();
      const diffAbs = Math.abs(diff);
      for (const interval of intervals) {
          if (diffAbs >= interval.ge) {
              const x = Math.round(Math.abs(diff) / interval.divisor);
              const isFuture = diff < 0;
              return interval.unit ? rft.format(isFuture ? x : -x, interval.unit) : interval.text;
          }
      }
    },
    bytesToSize: function(bytes) {
      var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
      if (bytes == 0) return '0 Byte';
      var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
      return Math.round(bytes / Math.pow(1024, i), 2) + ' ' + sizes[i];
    },
    randomImgUrl: function() {
      return `https://randomuser.me/api/portraits/men/${Math.floor(Math.random() * 100)}.jpg`;
    },
    isValidEmail: function(email) {
      return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
    },
    unitStatus: function(status) {
      if(status == 'AVAILABLE') return 'AVAILABLE';
      if(status == 'ON_PROCESS') return 'ON PROCESS';

      return status;
    },
    asUtcString: function(date) {
      // Get the local components
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');  // Months are zero-based
      const day = String(date.getDate()).padStart(2, '0');
      const hours = String(date.getHours()).padStart(2, '0');
      const minutes = String(date.getMinutes()).padStart(2, '0');
      const seconds = String(date.getSeconds()).padStart(2, '0');

      // Format it in ISO-like string but without timezone conversion
      return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
    },
    asUtcStringDateOnly: function(date) {
      // Get the local components
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');  // Months are zero-based
      const day = String(date.getDate()).padStart(2, '0');

      // Format it in ISO-like string but without timezone conversion
      return `${year}-${month}-${day}`;
    },
    printDocumentTypeName: function(key) {
      var d = [
        { value: 'RESERVATION', text: 'Sales - Reservasi' },
        { value: 'RESERVATION_WAITING_LIST', text: 'Sales - Kartu Waiting List' },
        { value: 'MKT_ORDER', text: 'Sales - Surat Pesanan' },
        { value: 'MKT_CANCELLATION_FORM', text: 'Sales - Form Pembatalan' },
        { value: 'MKT_PAYMENT_SCHEDULE', text: 'Sales - Jadwal Pembayaran' },
        { value: 'MKT_AGREEMENT', text: 'Sales - PPJB' },
        { value: 'MKT_HANDOVER', text: 'Sales - BAST' },
        { value: 'MKT_TAX_ADJUSTMENT', text: 'Sales - Adjustment PPN' },
        { value: 'MKT_CASH_BACK_INCENTIVE', text: 'Sales - Cash Back Incentive' },
        { value: 'MKT_PAYMENT_CONFIRMATION_RECEIPT', text: 'Finance & AR - Tanda Terima Sementara' },
        { value: 'MKT_RECEIPT', text: 'Finance & AR - Bukti Kas Masuk' },
        { value: 'MKT_MEMO', text: 'Finance & AR - Memo Pelunasan' },
        { value: 'MKT_INVOICE', text: 'Collection - Invoice' },
        { value: 'MKT_WARNING_LETTER', text: 'Collection - Surat Peringatan' },
        { value: 'MKT_WARNING_LETTER_LAST', text: 'Collection - Surat Peringatan (Somasi)' },
        { value: 'NUP_STATEMENT_INTEREST', text: 'NUP - Pernyataan Minat' },
        { value: 'NUP_REGULER', text: 'NUP - Reguler' },
        { value: 'NUP_EKSKLUSIF', text: 'NUP - Eksklusif' },
        ]
      var templ = d.find(p => p.value == key);
      if(templ)
        return templ.text;

      return key;
    }
  })
})