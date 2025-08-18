<template>
  <div>
    <div class="d-flex mt-3">
      <div class="flex-fill">
        <button>Sort</button>
        <button class="ms-1">Filter</button>
        <select name="" id="" class="ms-1" style="height: 23.2px; margin-top: 1px;">
          <option value="">Search by:</option>
        </select>
        <input type="text" class="ms-1" placeholder="Search..">
      </div>
      <div>
        <button>Create</button>
        <button class="ms-1">Import</button>
        <button class="ms-1">Export</button>
      </div>
    </div>
    <table class="x-table mt-3 w-100">
      <thead>
        <tr>
          <th scope="col">Kode</th>
          <th scope="col">Deskripsi</th>
          <th scope="col">&nbsp;</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>404</td>
          <td>Setya NOT FOUND Toh</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>2045</td>
          <td>Indonesia Cemas</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>2013</td>
          <td>Terakhir jadi fans MU</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>82</td>
          <td>Munchen - Barca | MU - Arsenal</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>71</td>
          <td>Judi Bola</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>Kas Bank</td>
          <td>BCARP-5251</td>
          <td>Agustinus</td>
          <td>Bank Central Asia</td>
          <td>Thamrin</td>
          <td>PT Tokyu Land Indonesia</td>
          <td>IDR</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>Kas Bank</td>
          <td>BCARP_1999</td>
          <td>Agus Santoso</td>
          <td>BCA</td>
          <td>Wahid Hasyim</td>
          <td>KSO Sakura Regency 3</td>
          <td>IDR</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>Kas Bank</td>
          <td>BII-1105</td>
          <td>Agus Santoso</td>
          <td>BII 0011271105</td>
          <td>&nbsp;</td>
          <td>&nbsp;</td>
          <td>IDR</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>Kas Bank</td>
          <td>BIITD</td>
          <td>Roby</td>
          <td>BII TD</td>
          <td>&nbsp;</td>
          <td>&nbsp;</td>
          <td>IDR</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
        <tr>
          <td>Kas Bank</td>
          <td>BNI-194</td>
          <td>Agus Santoso</td>
          <td>BNI (Rp)</td>
          <td>Ratu Plaza</td>
          <td>&nbsp;</td>
          <td>IDR</td>
          <td>
            <v-button-dropdown 
              :list="[
                { href: 'javascript:void(0);', icon: 'edit', label: 'Edit' },
                { separator: true },
                { href: 'javascript:void(0);', icon: 'check-circle', label: 'Activate' },
                { href: 'javascript:void(0);', icon: 'slash', label: 'Deactivate' },
              ]"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <v-table-pagination 
      :table="{Items:[], Page: 1, Length: 10, Total: 100, Filtered: 70}"
      :page-change="() => {}"
      :length-change="() => {}"
    />
  </div>
</template>