<template>
  <div>
    <div id="site-plan-container" style="background: #fff;">
      <button @click="zoomIn">Zoom In [+]</button>
      <button @click="zoomOut">Zoom Out [-]</button>
      <button @click="toggleFullScreen">
        <span v-if="isFullScreen">Exit Full Screen</span>
        <span v-else>Full Screen</span>
      </button>
      <button @click="$router.back">
        <span>Back</span>
      </button>
      <button @click="() => mode = 'MAP'" class="ms-2" :disabled="mode == 'MAP'">Map</button>
      <button @click="() => mode = 'LIST'" :disabled="mode == 'LIST'">List</button>
      <div class="text-center">
        <h3>{{ data.Name }}</h3>
        <!-- <h5>{{ data.ProjectName }}</h5> -->
      </div>
      <div class="m-3 p-2"
        style="border: solid 1px rgba(0, 0, 0, 0.1);background: rgba(0, 0, 0, .05); border-radius: 5px; ">
        <table class="w-100">
          <tbody>
            <tr>
              <template v-for="(item, k) in legends">
                <td></td>
                <td style="width: 25px;">
                  <div :style="`background: ${item.Color};`" style="width: 25px; height: 25px;border-radius: 5px;">
                  </div>
                </td>
                <td style="width: 105px;">
                  <span class="mx-3">{{ item.Label }}</span>
                </td>
                <td style="width: 55px;">
                  <span style="font-size: 18px;font-weight: 600;">{{ item.Count }}</span>
                </td>
                <td></td>
              </template>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="mx-3 text-center" ref="site-plan-canvas-container" v-if="mode == 'MAP'">
        <canvas id="site-plan-canvas" />
      </div>
      <div v-if="mode == 'LIST'" class="px-5 pt-5">
        <div class="row">
          <div class="col-sm-2 px-5" v-for="ix in 6">
            <table class="table x-table">
              <thead>
                <th>No. Unit</th>
                <th>Status</th>
              </thead>
              <tbody>
                <tr
                  v-for="(item, i) in Math.floor((data.Coordinates || []).length / 6) == 0 ? 1 : Math.floor((data.Coordinates || []).length / 6)">
                  <td v-if="ix == (i + 1)">
                    {{ data.Coordinates[(Math.floor((data.Coordinates || []).length / 6) * (ix - 1)) + i].UnitName }}
                  </td>
                  <td v-if="ix == (i + 1)">
                    <strong :style="`color: ${getColor(ix, i)};`">
                      {{ $func.unitStatus(data.Coordinates[(Math.floor((data.Coordinates || []).length / 6) * (ix - 1))
                        +
                        i].UnitStatus) }}
                    </strong>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <div id="popup"></div>
    </div>
  </div>
</template>

<script>
export default {
  props: ['sitePlanId', 'onItemClick'],
  data: () => ({
    canvas: null,
    image: new Image(),
    context: null,
    data: {},
    list: [],
    legends: [],
    zoomLvl: 1,
    isFullScreen: false,
    mode: 'MAP',
  }),
  computed: {
    ds: function () {
      return useMktSitePlan();
    },
  },
  watch: {
    sitePlanId: function (after) {
      console.log(after)
    },
    mode: function (after) {
      if (after != 'MAP') {
        this.loadDetail();
        return;
      };

      setTimeout(() => {
        this.init()
        this.loadDetail();
      }, 500)
    }
  },
  mounted: function () {
    // var _this = this
    // this.canvas = document.getElementById('site-plan-canvas');
    // this.context = this.canvas.getContext('2d')
    // this.image.onload = function() {
    //   _this.canvas.setAttribute('width', this.width)
    //   _this.canvas.setAttribute('height', this.height)
    //   _this.context.drawImage(_this.image,0,0); 
    // }; 
    // this.image.src = this.$http.url(`/marketing/site-plan/image?id=${this.sitePlanId}`); 
    // this.canvas.width = this.canvas.width;
    // this.context.drawImage(this.image,0,0); 
    this.init()
    this.loadDetail();
    setInterval(() => {
      this.loadDetail();
    }, 5000);
  },
  methods: {
    init: function () {
      var _this = this
      this.image = new Image();
      this.canvas = document.getElementById('site-plan-canvas');
      this.context = this.canvas.getContext('2d')
      this.canvas.width = this.canvas.width; // Clears the canvas 
      this.image.onload = function () {
        this.width = this.width * _this.zoomLvl;
        this.height = this.height * _this.zoomLvl;
        _this.canvas.setAttribute('width', this.width)
        _this.canvas.setAttribute('height', this.height)
        _this.context.drawImage(_this.image, 0, 0, this.width, this.height);
        _this.drawList(_this.list)
        // setTimeout(() => _this.drawList(_this.list), 200)
      };
      this.image.src = this.$http.url(`/marketing/site-plan/image?id=${this.sitePlanId}`);
    },
    loadDetail: function () {
      this.ds.loadDetail(this.sitePlanId)
        .then((data) => {
          data.Data.Coordinates.forEach(r => {
            this.list.push(r)
          })
          this.data = data.Data;
          this.legends = data.Data.Legends;
          setTimeout(() => this.drawList(this.list), 200)
        })
    },
    drawList: function (list) {
      if (!Array.isArray(list)) return;

      this.canvas.width = this.image.width; // Clears the canvas 
      // this.context.drawImage(this.image,0,0); 
      this.context.drawImage(this.image, 0, 0, this.image.width, this.image.height);

      list.forEach((e, i) => this.drawPoly(e));
      var _this = this;
      const popup = document.getElementById('popup');
      this.canvas.addEventListener('mousemove', function (event) {
        var doc = document.documentElement;
        var top = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);
        var left = (window.pageXOffset || doc.scrollLeft) - (doc.clientLeft || 0);
        const mouseX = event.clientX - _this.canvas.getBoundingClientRect().left;
        const mouseY = event.clientY - _this.canvas.getBoundingClientRect().top;

        for (const x of _this.list) {
          var poly = JSON.parse(x.Coordinates)
          if (_this.isInsidePolygon(poly, mouseX, mouseY)) {
            // console.log(poly, 'inside')
            // Show the popup and position it near the mouse
            popup.style.display = 'block';
            popup.style.zIndex = '999999';
            popup.style.left = `${event.clientX + 10 + left}px`;
            popup.style.top = `${event.clientY + 10 + top}px`;
            // You can add content to the popup here
            popup.innerHTML = `${x.UnitName} \n ${x.UnitStatus}`;
            break;
          }
          else {
            popup.style.display = 'none';
          }
        }
      });
      this.canvas.addEventListener('click', function (event) {
        var doc = document.documentElement;
        var top = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);
        var left = (window.pageXOffset || doc.scrollLeft) - (doc.clientLeft || 0);
        const mouseX = event.clientX - _this.canvas.getBoundingClientRect().left;
        const mouseY = event.clientY - _this.canvas.getBoundingClientRect().top;

        for (const x of _this.list) {
          var poly = JSON.parse(x.Coordinates)
          if (_this.isInsidePolygon(poly, mouseX, mouseY)) {
            if (_this.onItemClick)
              _this.onItemClick(x)
            break;
          }
        }
      });
    },
    redraw: function () {
      this.drawPoly(this.list);
    },
    drawPoly: function (item) {
      var coords = JSON.parse(item.Coordinates)
      if (coords.length == 0) return;

      if (item.IsSelecting) {
        this.context.fillStyle = this.legends.find(p => p.Label == 'SELECTING')?.Color || 'rgba(0, 0, 0, .5)';
      }
      else
        this.context.fillStyle = item.Color;

      this.context.globalAlpha = .7;
      // this.context.strokeStyle = item.Color;
      this.context.strokeStyle = "#2d6f30";
      // if(item.UnitStatus == 'AVAILABLE') {
      //   this.context.fillStyle = 'rgba(67, 160, 71,0.7)';
      //   this.context.strokeStyle = "#2d6f30";
      // }
      this.context.lineWidth = 1;

      this.context.beginPath();
      this.context.moveTo(coords[0].x * this.zoomLvl, coords[0].y * this.zoomLvl);
      for (var i = 1; i < coords.length; i++) {
        this.context.lineTo(coords[i].x * this.zoomLvl, coords[i].y * this.zoomLvl);
      }
      if (coords[0].x == coords[coords.length - 1].x && coords[0].y == coords[coords.length - 1].y) {
        this.context.closePath();
        this.context.fill();
      }
      this.context.stroke();
    },
    isInsidePolygon: function (poly, mouseX, mouseY) {
      let inside = false;
      for (let i = 0, j = poly.length - 1; i < poly.length; j = i++) {
        const xi = poly[i].x * this.zoomLvl, yi = poly[i].y * this.zoomLvl;
        const xj = poly[j].x * this.zoomLvl, yj = poly[j].y * this.zoomLvl;

        const intersect = ((yi > mouseY) !== (yj > mouseY)) &&
          (mouseX < (xj - xi) * (mouseY - yi) / (yj - yi) + xi);
        if (intersect) inside = !inside;
      }
      return inside;
    },
    zoomIn: function () {
      if (this.zoomLvl.toFixed(1) == 3) return;
      this.zoomLvl += .2;
      this.init();
    },
    zoomOut: function () {
      if (this.zoomLvl.toFixed(1) == .4) return;
      this.zoomLvl -= .2;
      this.init();
    },
    toggleFullScreen: function () {
      var elem = document.getElementById("site-plan-container");
      if (!document.fullscreenElement &&    // alternative standard method
        !document.mozFullScreenElement &&
        !document.webkitFullscreenElement &&
        !document.msFullscreenElement) {  // current working methods
        if (elem.requestFullscreen) {
          this.isFullScreen = true;
          elem.requestFullscreen();
        } else if (elem.msRequestFullscreen) {
          this.isFullScreen = true;
          elem.msRequestFullscreen();
        } else if (elem.mozRequestFullScreen) {
          this.isFullScreen = true;
          elem.mozRequestFullScreen();
        } else if (elem.webkitRequestFullscreen) {
          this.isFullScreen = true;
          elem.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
        }
      } else {
        if (document.exitFullscreen) {
          document.exitFullscreen();
          this.isFullScreen = false;
        } else if (document.msExitFullscreen) {
          document.msExitFullscreen();
          this.isFullScreen = false;
        } else if (document.mozCancelFullScreen) {
          document.mozCancelFullScreen();
          this.isFullScreen = false;
        } else if (document.webkitExitFullscreen) {
          document.webkitExitFullscreen();
          this.isFullScreen = false;
        }
      }
    },
    getColor: function (c, r) {
      var cc = this.data.Coordinates[(Math.floor((this.data.Coordinates || []).length / 6) * (c - 1)) + r];
      if (cc.UnitStatus != 'ON_PROCESS')
        return cc.Color;

      var cx = this.legends.find(p => p.Label == 'SELECTING');
      return cx?.Color || '#000000';
    }
  }
}
</script>

<style>
#popup {
  display: none;
  position: absolute;
  background-color: #f9f9f9;
  border: 1px solid #ccc;
  padding: 10px;
}
</style>
