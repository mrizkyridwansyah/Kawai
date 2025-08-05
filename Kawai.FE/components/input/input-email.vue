<template>
  <div :class="'form-group'">
    <label class="d-block">{{ label }}</label>
    <div class="email-input-container"
      :class="errors ? 'is-invalid' : null"
    >
      <span v-for="(item, index) in emails" :key="index"
        class="email-chip"
        :class="isValidEmail(item) ? 'email-chip-valid' : 'email-chip-invalid'"
      >
        {{ item }} 
        <span class="email-remove" @click="() => removeEmail(index)">x</span>
      </span>
      <input class="email-input form-control" type="email"
        @keydown="keyDown"
        @blur="blur"
        @paste="onPaste"
        :placeholder="placeholder"
        v-model="email"
      />
    </div>
    <div class="invalid-feedback d-block" v-if="errors">
      {{ errors[0] }}
    </div>
    <small class="form-text text-muted" v-if="description">{{description}}</small>
  </div>
</template>

<script>
export default {
  props: ['modelValue', 'label', 'errors', 'description', 'placeholder'],
  emits: ['update:modelValue', 'input'],
  data: () => ({
    email: null,
    emails: [],
  }),
  mounted: function() {
    
  },
  methods: {
    keyDown: function(e) {
      if(e.keyCode !== 13 && e.keyCode !== 32)
        return;

      if(!this.isValidEmail(this.email))
        return;

      this.emails.push(e.target.value);
      this.email = null;
      this.$emit('update:modelValue', this.emails);

      if(this['onInput']) 
        this['onInput'](e.target.value);
    },
    onPaste: function(e) {
      var texts = e.clipboardData.getData('text');

      // new line
      var a = texts.split('\n');
      if(a.length > 0) {
        for(var i=0; i < a.length; i++) {
          var email = a[i].replace('\n','').replace(' ','').replace('\n', '').replace('\r', '').replace(',', '').replace('|', '');
          if(this.isValidEmail(email)) {
            this.emails.push(email)
          }
        }
        setTimeout(() => this.email = null, 100)
      }

      // semi-colon
      a = texts.split(';');
      if(a.length > 0) {
        for(var i=0; i < a.length; i++) {
          var email = a[i].replace(';','').replace('\n', '').replace('\r', '').replace(',', '').replace('|', '');
          if(this.isValidEmail(email)) {
            this.emails.push(email)
          }
        }
        setTimeout(() => this.email = null, 100)
      }

      // space
      a = texts.split(' ');
      if(a.length > 0) {
        for(var i=0; i < a.length; i++) {
          var email = a[i].replace(' ','').replace('\n', '').replace('\r', '').replace(',', '').replace('|', '');
          if(this.isValidEmail(email)) {
            this.emails.push(email)
          }
        }
        setTimeout(() => this.email = null, 100)
      }

      // comma
      a = texts.split(',');
      if(a.length > 0) {
        for(var i=0; i < a.length; i++) {
          var email = a[i].replace(' ','').replace('\n', '').replace('\r', '').replace(',', '').replace('|', '');
          if(this.isValidEmail(email)) {
            this.emails.push(email)
          }
        }
        setTimeout(() => this.email = null, 100)
      }

      // comma
      a = texts.split('|');
      if(a.length > 0) {
        for(var i=0; i < a.length; i++) {
          var email = a[i].replace(' ','').replace('\n', '').replace('\r', '').replace(',', '').replace('|', '');
          if(this.isValidEmail(email)) {
            this.emails.push(email)
          }
        }
        setTimeout(() => this.email = null, 100)
      }
    },
    blur: function(e) {
      if(!this.isValidEmail(this.email))
        return;

      this.emails.push(e.target.value);
      this.email = null;
      this.$emit('update:modelValue', this.emails);

      if(this['onInput']) 
        this['onInput'](e.target.value);
    },
    removeEmail: function(i) {
      this.emails.splice(i, 1)
    },
    isValidEmail: function(email) {
      return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
    }
  }
}
</script>

<style lang="scss">
.email-chip {
  display: inline-block;
  margin: 2px;
  /* margin-bottom: 10px; */
  padding: 2px;
  padding-left:  15px;
  padding-right: 10px;
  border-radius: 15px;
  border: solid 1px #000;
  font-size: 14px;
}

.email-chip-valid {
  border: solid 1px #009ef7 !important;
  background: rgb(0, 158, 247, .1);
  // background: #009ef7;
}

.email-chip-invalid {
  border: solid 1px #ff4560 !important;
  background: rgb(255, 69, 96, .1);
  // background: #ff4560;
}

.email-remove {
  margin-left: 5px;
  cursor: pointer;
}
.email-input{
  margin: 2px;
  border: none;
  outline: none;
  width: 100%;
  &:focus, &:active {
    border: none;
    outline: none;
  } 
}
</style>