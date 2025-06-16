import {
    defineAsyncComponent,
    type AsyncComponentLoader,
    type Component,
  } from "vue";
  
  
  /**
   * * 异步加载组件,默认延迟80毫秒后渲染
   * @param loader
   * @returns
   */
  export const loadAsyncComponent = (
    loader: AsyncComponentLoader<any>,
    loadingComponent?: Component
  ): any =>
    defineAsyncComponent({
      loader,
      loadingComponent,
      delay: 80,
    });
  