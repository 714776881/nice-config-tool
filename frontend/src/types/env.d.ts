declare namespace Env {
  interface ImportMeta extends ImportMetaEnv {
    /** The base url of the application */
    /** 基础地址 */
    readonly VITE_BASE_URL: string
    /** The title of the application */
    /** 应用的标题 */
    readonly VITE_APP_TITLE: string
    /** The description of the application */
    /** 应用的描述 */
    readonly VITE_APP_DESC: string
    /** backend service base url */
    /** 后端服务基础地址 */
    readonly VITE_SERVICE_BASE_URL: string
    /** 配置模式 */
    readonly VITE_CONFIG_MODE: 'true' | 'false'
  }
}
