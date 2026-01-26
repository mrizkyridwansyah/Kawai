<template>
  <v-frame title="Classification Master" icon="database">
    <template #frame-content>
      <div class="page-wrapper">

        <!-- TAB HEADER -->
        <div class="tab-wrapper">
          <button class="nav-btn" @click="scrollLeft">◀</button>

          <div class="tabs" ref="tabs">
            <button
              v-for="item in tabs"
              :key="item.tab"
              :class="['tab-btn', { active: activeTab === item.tab }]"
              @click="setActive(item.tab)"
            >
              {{ item.title }}
            </button>
          </div>

          <button class="nav-btn" @click="scrollRight">▶</button>
        </div>

        <!-- TAB CONTENT -->
        <div class="tab-content">
          <h4>{{ activeTitle }} Cls</h4>
 
          <div v-if="ds.isLoading">Loading detail...</div>
<v-button-add :add="add" cClass="mb-1" />
  <div class="table-wrapper">
  <table class="table-grid">
    <thead>
      <tr>
         <th class="text-center">Action</th>
        <th>Code</th>
        <th>Description</th>
      
      </tr>
    </thead>

  <tbody v-if="detailItems && detailItems.length">
  <tr v-for="row in detailItems" :key="row.Code">
      <td class="text-center">
                  <font-awesome-icon
                    class="mr-2 text-success"
                    icon="pencil"
                     @click="edit(row)"
                  />
                  <font-awesome-icon
                    class="ml-2 text-danger"
                    icon="trash"
                     @click="remove(row)"
                  />
                </td>
        <td>{{ row.Code }}</td>
        <td>{{ row.Description }}</td>
     
      </tr>
    </tbody>
    <tbody v-else>
  <tr class="no-data-row">
    <td colspan="3" class="no-data">
      No results found
    </td>
  </tr>
</tbody>
  </table>
</div>

        </div>
      </div>
    </template>
  </v-frame>
  <v-modal
    ref="modalCls"
    id="modal-form-cls"
    :title="titlemodal"
    size="md"
    @hidden="
      () => {
        this.$refs.formCls && this.$refs.formCls.resetForm();
        modalMode = '';
      }
    "
  >
    <modal-form-cls
      ref="formCls"
      :id="idSelected"
      :mode="modalMode"
      @submitted="close"
    />
  </v-modal>
</template>

<script>
export default {
  data: () => ({
   activeTab: null,
  tabs: [],
titlemodal: '',  
  modalMode: '',
  idSelected: null,
  activeTableName: ''   // 🔥 SIMPAN TABLENAME AKTIF
  }),

  computed: {
    ds() {
      return useClassification();
    },

    activeTitle() {
      const f = this.tabs.find(x => x.tab === this.activeTab);
      return f ? f.title : "";
    },

    activeTabConfig() {
      return this.tabs.find(x => x.tab === this.activeTab) || {};
    },

    // 🔥 DETAIL DARI loadtable
    detailItems() {
      return this.ds.data?.TableData || [];
    }
  },

  mounted() {
    // LOAD TAB HEADER SEKALI
    this.ds.load().then(() => {
      this.tabs = (this.ds.data?.Items || []).map(x => ({
        tab: Number(x.SeqNo),
        title: x.TabHeader,
        table: x.TableName
      }));

      if (this.tabs.length) {
        this.setActive(this.tabs[0].tab);
      }
    });
  },

  methods: {
     add: function () {
      this.titlemodal =   this.activeTableName;
      this.modalMode = "add";
      this.idSelected = null; // Reset ID for Add mode
     
     this.$bvModal.show("modal-form-cls");

  // 🔥 kirim ke modal form
  this.$nextTick(() => {
    if (this.$refs.formCls) {
      this.$refs.formCls.setTableName(this.activeTableName);
    }
  });
      },
      edit: function (dt) {
      this.titlemodal = this.activeTableName;
      this.modalMode = "edit";
      this.idSelected = dt.Code;
      this.$bvModal.show("modal-form-cls");
       // 🔥 kirim ke modal form
 this.$nextTick(() => {
    if (this.$refs.formCls) {
      this.$refs.formCls.setTableName(this.activeTableName);
    }
  });
      },
      remove: function (item) {
  confirmRemove(
    () =>
      new Promise((resolve) => {
        this.ds
          .remove({
            code: item.Code,                 // ✅ code yg benar
            tableName: this.activeTableName  // ✅ tab aktif
          })
          .then(() => {
            // 🔥 reload detail table tab aktif
            this.ds.loadtable(this.activeTableName);

            toastSuccess("Data deleted successfully");
            resolve();
          })
          .catch((err) => {
            toastDanger(err?.Message);
            resolve();
          });
      }),
    null,
    item.Description
  );
},

      close() {
    this.$bvModal.hide("modal-form-cls");

    // 🔥 reload table aktif
    if (this.activeTableName) {
      this.ds.loadtable(this.activeTableName);
    }
  },
   setActive(tab) {
this.activeTab = tab;

  const cfg = this.activeTabConfig;
  if (!cfg.table) return;

  // 🔥 SIMPAN TABLENAME
  this.activeTableName = cfg.table;

  this.ds.loadtable(cfg.table);

  this.$nextTick(() => this.scrollToActive());
},

    scrollLeft() {
      this.$refs.tabs.scrollLeft -= 200;
    },

    scrollRight() {
      this.$refs.tabs.scrollLeft += 200;
    },

    scrollToActive() {
  if (!this.$refs.tabs) return;

  const el = this.$refs.tabs.querySelector('.tab-btn.active');
  if (el && el.scrollIntoView) {
    el.scrollIntoView({ behavior: 'smooth', inline: 'center' });
  }
}

    
  }
};
</script>
<style scoped>
.page-wrapper {
  padding: 16px;
}

.tab-wrapper {
  display: flex;
  align-items: center;
  background: #1f262d;
  padding: 6px;
  border-radius: 4px;
}

.tabs {
  display: flex;
  flex: 1;
  overflow-x: auto;
  scrollbar-width: none;
}

.tabs::-webkit-scrollbar {
  display: none;
}

.tab-btn {
  white-space: nowrap;
  padding: 8px 14px;
  margin-right: 4px;
  background: #2c343c;
  color: #fff;
  border: none;
  cursor: pointer;
}

.tab-btn.active {
  background: #ffffff;
  color: #000;
  font-weight: bold;
}

.nav-btn {
  background: #111;
  color: #4da3ff;
  border: none;
  cursor: pointer;
  padding: 6px 10px;
  font-size: 16px;
}

.tab-content {
  margin-top: 16px;
  padding: 16px;
  border: 1px solid #ddd;
  border-radius: 4px;
}
.table-wrapper {
  max-height: 250px;
  overflow-y: auto;
  border: 1px solid #cfd8e3;
}

/* table base */
.table-grid {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
 
}

/* header */
.table-grid thead th {
  position: sticky;
  top: 0;
  z-index: 10;

  background: #8ec5ff;
  color: #000;
  text-align: center;
  padding: 8px;
  border-right: 1px solid #bcdcff;
  border-bottom: 1px solid #bcdcff;
  font-weight: 600;
}

.table-grid thead th:last-child {
  border-right: none;
}

/* body */
.table-grid td {
  padding: 8px;
  border-bottom: 1px solid #d0d7de;
  border-right: 1px solid #bcdcff;
}

/* zebra row */
.table-grid tbody tr:nth-child(odd) {
  background: #f0f1f3;
}

.table-grid tbody tr:nth-child(even) {
  background: #ffffff;
}

.table-grid tbody tr:hover {
  background: #e6f2ff;
}

/* action column */
.col-action {
  width: 80px;
  text-align: center;
}

.icon-edit {
  color: #00c08b;
  cursor: pointer;
  margin-right: 8px;
}

.icon-delete {
  color: #ff4d4f;
  cursor: pointer;
}
</style>