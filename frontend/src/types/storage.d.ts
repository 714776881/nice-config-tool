/** The storage namespace */
declare namespace StorageType {
  interface Session {
    /** The theme color */
    themeColor: string
    // /**
    //  * the theme settings
    //  */
    // themeSettings: App.Theme.ThemeSetting;
  }

  interface Local {
    /** The token */
    loginToken: Api.Auth.LoginToken
    /** 用户信息 */
    userInfo: Api.Auth.UserInfo
    /** The current menu */
    currentMenu: App.Menu.Menu
    /** 是否完成登录认证 */
    isAuthenticated
  }
}
