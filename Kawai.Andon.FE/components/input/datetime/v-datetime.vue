<template>
  <div class="vdatetime" style="max-width: 80%;">
    <slot name="before"></slot>
    <input :class="inputClass" :style="inputStyle" :id="inputId" type="text" :value="inputValue" v-bind="$attrs" v-maska
      :data-maska="mask" @blur="manualInput" />
    <input v-if="hiddenName" type="hidden" :name="hiddenName" :value="modelValue" @input="setValue">
    <slot name="after"></slot>
    <transition-group name="vdatetime-fade" tag="div">
      <div key="overlay" v-if="isOpen && !hideBackdrop" class="vdatetime-overlay" @click.self="clickOutside"></div>
      <datetime-popup key="popup" v-if="isOpen" :type="type" :datetime="popupDate" :phrases="phrases"
        :use12-hour="use12Hour" :hour-step="hourStep" :minute-step="minuteStep" :min-datetime="popupMinDatetime"
        :max-datetime="popupMaxDatetime" @confirm="confirm" @cancel="cancel" :auto="auto" :week-start="weekStart"
        :flow="flow" :title="title">
        <template slot="button-cancel__internal" slot-scope="scope">
          <slot name="button-cancel" v-bind:step="scope.step"> {{ phrases.cancel }}</slot>
        </template>
        <template slot="button-confirm__internal" slot-scope="scope">
          <slot name="button-confirm" v-bind:step="scope.step"> {{ phrases.ok }}</slot>
        </template>
      </datetime-popup>
    </transition-group>
  </div>
</template>

<script>
import { vMaska } from 'maska';
import { DateTime } from 'luxon'
import DatetimePopup from './datetime-popup'
import { datetimeFromISO, datetimeXXX, startOfDay, weekStart } from './util'

export default {
  directives: { maska: vMaska },
  components: {
    DatetimePopup
  },

  inheritAttrs: false,

  props: {
    modelValue: {
      type: String
    },
    valueZone: {
      type: String,
      default: 'UTC'
    },
    inputId: {
      type: String,
      default: null
    },
    inputClass: {
      type: [Object, Array, String],
      default: ''
    },
    inputStyle: {
      type: [Object, Array, String],
      default: ''
    },
    hiddenName: {
      type: String
    },
    zone: {
      type: String,
      default: 'local'
    },
    mask: {
      type: String,
      default: '##/##/####'
    },
    format: {
      type: [Object, String],
      default: null
    },
    type: {
      type: String,
      default: 'date'
    },
    phrases: {
      type: Object,
      default() {
        return {
          cancel: 'Cancel',
          ok: 'Ok'
        }
      }
    },
    use12Hour: {
      type: Boolean,
      default: false
    },
    hourStep: {
      type: Number,
      default: 1
    },
    minuteStep: {
      type: Number,
      default: 1
    },
    minDatetime: {
      type: String,
      default: null
    },
    maxDatetime: {
      type: String,
      default: null
    },
    auto: {
      type: Boolean,
      default: false
    },
    weekStart: {
      type: Number,
      default() {
        return weekStart()
      }
    },
    flow: {
      type: Array
    },
    title: {
      type: String
    },
    hideBackdrop: {
      type: Boolean,
      default: false
    },
    backdropClick: {
      type: Boolean,
      default: true
    },
    openPopUp: {
      type: Boolean,
      default: true
    }
  },

  emits: ['update:modelValue', 'update:openPopUp'],

  data() {
    return {
      isOpen: false,
      datetime: datetimeFromISO(this.modelValue)
    }
  },

  watch: {
    modelValue(newValue) {
      if (newValue == null) {
        this.datetime = null
        return;
      }

      if (this.datetime != null) {
        var a = this.datetime.c;
        var b = datetimeFromISO(newValue).c;
        if (a.year == b.year && a.month == b.month && a.day == b.day && this.type == 'date')
          return;
      }

      this.datetime = datetimeFromISO(newValue)
    },
    openPopUp: function (after) {
      this.isOpen = after;
    }
  },

  created() {
    this.emitInput()
  },

  computed: {
    inputValue() {
      let format = this.format

      if (!format) {
        switch (this.type) {
          case 'date':
            format = DateTime.DATE_MED
            break
          case 'time':
            format = DateTime.TIME_24_SIMPLE
            break
          case 'datetime':
          case 'default':
            format = DateTime.DATETIME_MED
            break
        }
      }

      if (this.type == 'year') {
        if (typeof (format) == 'string') {
          var c = DateTime.fromISO(this.datetime).setZone(this.zone);
          return this.datetime ? `${c.year}` : ''
        } else {
          var c = this.datetime.setZone(this.zone);
          return this.datetime ? `${c.year}` : ''
        }
      }

      if (this.type == 'month') {
        if (typeof (format) == 'string') {
          var c = DateTime.fromISO(this.datetime).setZone(this.zone);
          return this.datetime ? `${c.month.toString().padStart(2, '0')}/${c.year}` : ''
        } else {
          var c = this.datetime.setZone(this.zone);
          return this.datetime ? `${c.month.toString().padStart(2, '0')}/${c.year}` : ''
        }
      }

      if (typeof format === 'string') {
        return this.datetime ? DateTime.fromISO(this.datetime).toFormat(format) : ''
      } else {
        return this.datetime ? this.datetime.toLocaleString(format) : ''
      }
    },
    popupDate() {
      return this.datetime ? this.datetime.setZone(this.zone) : this.newPopupDatetime()
    },
    popupMinDatetime() {
      return this.minDatetime ? DateTime.fromISO(this.minDatetime).setZone(this.zone) : null
    },
    popupMaxDatetime() {
      return this.maxDatetime ? DateTime.fromISO(this.maxDatetime).setZone(this.zone) : null
    }
  },

  methods: {
    emitInput() {
      let datetime = this.datetime ? this.datetime.setZone(this.valueZone) : null

      if (datetime && this.type === 'date') {
        datetime = startOfDay(datetime)
      }

      this.$emit('update:modelValue', datetime ? datetime.toISO() : null)
    },
    open(event) {
      event.target.blur()

      this.isOpen = true
    },
    close() {
      this.isOpen = false
      this.$emit('update:openPopUp', false)
    },
    confirm(datetime) {
      // var asd = datetime.toUTC();
      var asd = datetime;
      var aa = {
        year: asd.c.year,
        month: asd.c.month,
        day: asd.c.day,
        hour: asd.c.hour,
        minute: asd.c.minute,
        second: asd.c.second,
        millisecond: asd.c.millisecond,
      }
      if (this.type == 'month') {
        aa.day = 1;
        aa.hour = 0;
        aa.minute = 0;
        aa.second = 0;
        aa.millisecond = 0;
      }
      this.datetime = DateTime.utc().setZone(this.zone).set(aa)
      this.emitInput()
      this.close()
    },
    cancel() {
      this.close()
    },
    clickOutside() {
      if (this.backdropClick === true) { this.cancel() }
    },
    newPopupDatetime() {
      let datetime = DateTime.utc().setZone(this.zone).set({ seconds: 0, milliseconds: 0 })

      if (this.popupMinDatetime && datetime < this.popupMinDatetime) {
        datetime = this.popupMinDatetime.set({ seconds: 0, milliseconds: 0 })
      }

      if (this.popupMaxDatetime && datetime > this.popupMaxDatetime) {
        datetime = this.popupMaxDatetime.set({ seconds: 0, milliseconds: 0 })
      }

      if (this.minuteStep === 1) {
        return datetime
      }

      const roundedMinute = Math.round(datetime.minute / this.minuteStep) * this.minuteStep

      if (roundedMinute === 60) {
        return datetime.plus({ hours: 1 }).set({ minute: 0 })
      }

      return datetime.set({ minute: roundedMinute })
    },
    setValue(event) {
      this.datetime = datetimeFromISO(event.target.value)
      this.emitInput()
    },
    manualInput: function (e) {
      if (this.type == 'month') {
        if (!e.target.value) {
          this.$emit('update:modelValue', null)
          return;
        }
        var c = DateTime.fromFormat(e.target.value, "MM/yyyy");
        if (c.invalid == null) {
          this.datetime = datetimeFromISO(c)
          this.emitInput()
        }
        else {
          this.$emit('update:modelValue', null)
        }
        return;
      }
      if (this.type == 'year') {
        if (!e.target.value) {
          this.$emit('update:modelValue', null)
          return;
        }
        var c = DateTime.fromFormat(e.target.value, "yyyy");
        if (c.invalid == null) {
          this.datetime = datetimeFromISO(c)
          this.emitInput()
        }
        else {
          this.$emit('update:modelValue', null)
        }
        return;
      }
      if (e.target.value?.length == 10) {
        var c = DateTime.fromFormat(e.target.value, "dd/MM/yyyy");
        if (c.invalid == null) {          
          this.datetime = datetimeXXX(c, this.zone)
          this.emitInput()
        }
        else {
          this.$emit('update:modelValue', null)
        }
      }
      else if (e.target.value?.length == 9) {
        var c = DateTime.fromFormat(e.target.value, "dd/MM/yyy");
        if (c.invalid == null) {
          this.datetime = datetimeFromISO(c)
          this.emitInput()
        }
        else {
          this.$emit('update:modelValue', null)
        }
      }
      else if (e.target.value?.length == 8) {
        var c = DateTime.fromFormat(e.target.value, "dd/MM/yy");
        if (c.invalid == null) {
          this.datetime = datetimeFromISO(c)
          this.emitInput()
        }
        else {
          this.$emit('update:modelValue', null)
        }
      }
      else if (e.target.value?.length == 7) {
        var c = DateTime.fromFormat(e.target.value, "dd/MM/y");
        if (c.invalid == null) {
          this.datetime = datetimeFromISO(c)
          this.emitInput()
        }
        else {
          this.$emit('update:modelValue', null)
        }
      }
      else {
        this.$emit('update:modelValue', null)
        this.datetime = null;
      }
    },
  }
}
</script>

<style>
.vdatetime-fade-enter-active,
.vdatetime-fade-leave-active {
  transition: opacity .4s;
}

.vdatetime-fade-enter,
.vdatetime-fade-leave-to {
  opacity: 0;
}

.vdatetime-overlay {
  z-index: 999;
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background: rgba(0, 0, 0, 0.5);
  transition: opacity .5s;
}
</style>