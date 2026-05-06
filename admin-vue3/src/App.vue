<template>
  <div id="app">
    <router-view />
  </div>
</template>

<style>
html,
body {
	padding: 0;
	margin: 0;
	background-color: #f7f7f7;
	font-family: 'Segoe UI', 'PingFang SC', 'Microsoft YaHei', sans-serif;
}

#app {
	font-family: inherit;
}

li {
	list-style: none;
}

.el-popper,
.el-select__popper {
	z-index: 3000 !important;
}
</style>
