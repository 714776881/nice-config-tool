declare namespace Api {
  namespace Auth {
    interface LoginToken {
      token: string
      refreshToken: string
    }

    interface UserInfo {
      userId: string
      userCode: string
      userName: string
      userActor: string
      userRole: string
      departmentId: string
      hospitalId: string
      regionId: string
    }
  }

  namespace Crud {
    interface Data {
      data: any[]
    }
    interface EexResult {
      success: boolean
    }
    interface File {
      fileName: String
      fileContent: any
    }
  }

  namespace ReptCatelog {
    interface ReptLibStateForm {
      UserId: string
      UserCode: string
      RegionId: string
      HospitalId: string
      DepartmentId: string
      CategoryType: App.Common.CategoryType
    }

    interface ReptCatelogVO {
      modality: string
      nodes: Node[]
    }

    interface CatelogNodeForm {
      Node: Node
    }

    interface Node {
      key: string
      title: string
      type: App.Common.ReptNodeTypeEnum
      isPublic: boolean
      children: Node[]
    }

    interface FlatNode {
      key: string
      title: string
      type: App.Common.ReptNodeTypeEnum
      isPublic: boolean
      parentKey?: string // 可选的父节点 key
    }

    interface ReptTempVO {
      reptTempId: string
      reptTemp: string
      studyResult: string
      diagResult: string
      isPublic: boolean
      modifyDT: string
    }

    interface ReptTempForm {
      // 模板ID
      ReptTempId?: string
      // 模板名称
      ReptTemp?: string
      // 检查所见
      StudyResult?: string
      CStudyResult?: string
      // 检查诊断
      DiagResult?: string
      CDiagResult?: string
      // 是否公开
      IsPublic: boolean
      // 区域ID
      RegionId?: string
      // 院区ID
      HospitalId?: string
      // 部门ID
      DepartmentId?: string
    }
  }
}
