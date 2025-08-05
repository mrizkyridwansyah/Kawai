<template>
  <div class="d-flex">
    <div>
      <img 
        :src="$http.url(`/employee/photo?id=${data.EmployeeImageId}`)" 
        class="rounded" style="width: 80px;height: 80px;border: solid 1px #fff;box-shadow: rgba(0, 0, 0, 0.2) 0px 0px 2px 0px;" 
      />
    </div>
    <div class="ps-4">
      <div>
        <span style="font-size: 1.1rem;font-weight: 500;">{{ data.EmployeeName }}</span>
      </div>
      <div>
        <small>{{ data.EmployeeFormattedId }}</small>
      </div>
      <div>
        <small>{{ data.EmployeeSubOrganizationName }} - {{ data.EmployeeOrganizationName }}</small>
      </div>
      <div>
        <span>{{ data.WorkLocationName }}</span>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ['data']
}
</script>