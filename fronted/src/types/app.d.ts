declare namespace App {
  /** Service namespace */
  namespace Service {
    /** The backend service response data */
    /** 后端服务响应数据 */
    type Response<T = unknown> = {
      /** 响应码：200-成功，其他-失败 */
      code: string
      /** 响应信息 */
      message: string
      /** 响应数据 */
      data: T
    }
  }

  namespace Common {
    interface KV<T = any> {
      [k: string | number]: T
    }

    type Dictionary = DicElement[]

    type DicElement = {
      label: string
      value: string
      category: string
    }

    enum CategoryType {
      ReptTemp = 1002,
      SpecDes = 1003,
      GMMacr = 1004,
      ExamItem = 1010
    }

    enum ReptNodeTypeEnum {
      BodyPart = 0,
      ExamItem = 1,
      Category = 2,
      ReportTemplate = 3
    }
  }

  namespace Crud {
    type Config = {
      columnConfig: ColumnConfig[]
      sqlTemplates: SqlTemplates
      buttonsShow: ButtonsShow
      addedButtons: AddedButton[]
    }
    /**
     * @typedef Config
     * @description 表单配置类型，用于描述每个表单字段的配置属性。
     */
    type ColumnConfig = {
      /**
       * @property {string} title - 表单项的标题，显示在界面上的名称。
       */
      title: string

      /**
       * @property {string} key - 表单项的键名，用于表单数据的唯一标识。
       */
      key: string

      /**
       * @property {boolean} isShow - 表单项是否在界面上显示。
       */
      isShow: boolean

      /**
       * @property {boolean} [isQuery] - 表单项是否在查询条件中使用（可选）。
       */
      isQuery?: boolean

      /**
       * @property {boolean} isEdit - 表单项是否允许编辑。
       */
      isEdit: boolean

      /**
       * @property {number|string} [width] - 表单项的宽度，可以是数字或字符串（可选）。
       */
      width?: number | string

      /**
       * @property {Object} component - 表单项的组件配置，描述使用的 UI 组件。
       * @property {string} component.name - 组件的名称，例如 "a-input"。
       * @property {Object} component.props - 组件的属性配置。
       * @property {string} component.props.placeholder - 输入框的占位符提示文字。
       * @property {Array<{label: string, value: string | number}>} [component.props.options] - 组件的选项（如 select 或 radio），可选字段。 group, 分组
       * @property {string} [component.props.disabled] - 是否禁用输入框，值为 "true" 表示禁用（可选）。
       */
      component: {
        name: string
        props: {
          placeholder: string
          options?: {
            label: string
            value: string | number
            group: string
          }[]
          disabled?: string
        }
      }

      /**
       * @property {Object} formItem - 表单项的验证和默认值配置。
       * @property {Array<{required: boolean, message: string}>} formItem.rules - 验证规则，描述表单项是否必填及对应的提示信息。
       * @property {string|number} [formItem.defaultValue] - 表单项的默认值（可选）。
       */
      formItem: {
        rules?: {
          required?: boolean // 是否必填。
          message?: string // 验证失败后的提示信息。
          validator?: stirng | ((rule: any, value: any, callback: any) => void) // 验证函数
        }[]
        defaultValue?: string | number
        defaultValueBySql?: string
        addShow?: boolean
        groupBy?: string
        allOptions?: {
          label: string
          value: string | number
          group: string
        }[] // 存储所有选项，在需要根据分组过滤选项时使用。
      }

      /**
       * @property {string} [dicSql] - 表单项的数据字典，表示查询数据的 SQL 语句或其他数据来源（可选）。
       */
      dicSql?: string
    }

    type FormState = {
      /**
       * @property {string} ID - 表单数据的主键字段，用于标识记录的唯一性。
       */
      ID?: string
      /**
       * @property {string} DELETED - 表单数据的删除状态字段，用于标识记录是否已删除。
       */
      DELETED?: string
    }

    // Sql 模板
    type SqlTemplates = {
      /**
       * @property {string} select - SQL 查询语句模板，用于查询数据。
       */
      select: string

      /**
       * @property {string} insert - SQL 插入语句模板，用于插入新数据。
       */
      insert: string

      /**
       * @property {string} update - SQL 更新语句模板，用于更新现有数据。
       */
      update: string
      /**
       * @property {string} updateState - SQL 更新状态语句模板，用于更新状态。
       */
      setState: string

      /**
       * @property {string} setSequence - SQL 更新排序语句模板，用于更新排序。
       */
      setSequence: string
      /**
       * @property {string} delete - SQL 删除语句模板，逻辑删除某条记录。
       */
      delete: string

      /**
       * @property {string} exportData - SQL 导出数据语句模板，用于导出数据。
       */
      exportData: string
    }

    type ButtonsShow = {
      add: boolean
      edit: boolean
      delete: boolean
      setState: boolean
      setStateToDelete: boolean
      setSequence: boolean
      exportData: boolean
      importData: boolean
    }
    // 附加操作按钮
    export type AddedButton = {
      /**
       * @property {string} name - 按钮显示名称
       */
      name: string
      /**
       * @property {string} width - 弹窗宽度
       */
      width: string
      /**
       * @property {string} exeSql - 执行SQL
       */
      exeSql: string
      /**
       * @property {string} remark - 在上层未输入插槽的时候，使用的默认传参
       */
      defaultShowText: string
      /**
       * @property {string} isResetFetchData - 点击后是否立即刷新页面，并重新获取数据
       */
      isResetFetchData: boolean
      /**
       * @property {string} isBetch - 是否批量执行
       */
      isBetch: boolean
    }
  }

  namespace Dict {
    /**
     * @typedef {Object} Node
     * @description 节点类型，用于描述字典树节点的数据结构。
     * @property {Array<Node>} item - 子节点数组。
     * @property {Object} [:@] - 节点的属性对象，包含 name、value 和 label。
     * @property {Object} [_] - 节点的注释信息。
     */
    type Node = {
      item: Node[]
      [':@']: {
        name: string
        value: string
        label: string
      }
      ['_']: {
        key: string
      }
    }

    type Config = {
      keyValueConfig: KeyValueConfig[]
      inputConfig: InputConfig[]
      keyLabelMap: keyLabel[]
    }

    type keyLabel = {
      key: string
      label: string
    }

    type KeyValueConfig = {
      name: string
      filePath: string
      nodePath: String
      nodeContent: string
    }

    type InputConfig = {
      isShow: string // 是否显示
      key: string
      component: {
        name: string
        props: {
          placeholder: string
          options?: {
            label: string
            value: string | number
          }[]
          disabled?: string
        }
      }
      formItem: {
        rules: {
          required: boolean
          message: string
        }[]
        defaultValue?: string | number
      }
      dicSql?: string
    }
  }

  namespace ReptCatelog {
    type Config = {
      isLoadExamItem: boolean // 是否加载检查项目
      catelogMaxLayerNumber: number // 分组节点的最大层数
      adminRoles: string[] // 管理员权限控制
    }
  }
}
