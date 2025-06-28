<template>
  <div class="page-401">
    <div class="content">
      <img src="@/assets/401.svg" alt="401 Error" />
      <p>抱歉！当前因权限问题，您无法查看此页面！</p>
    </div>
  </div>
</template>

<style scoped>
.page-401 {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f2f2f2;
  color: #333;
}

.content {
  text-align: center;
}

.content h1 {
  font-size: 10rem;
  margin: 0;
  color: #ff4d4f;
}

.content p {
  font-size: 1.5rem;
  margin: 1rem 0;
}

button {
  padding: 0.8rem 1.5rem;
  font-size: 1rem;
  color: white;
  background-color: #1890ff;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #40a9ff;
}
</style>
