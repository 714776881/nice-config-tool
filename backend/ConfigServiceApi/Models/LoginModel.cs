using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServiceApi.Models
{

    public class LoginForm
    {
        public string? loginToken { get; set; }
    }

    public class LoginVO
    {
        public string? token { get; set; }
        public string? userId { get; set; }
        public string? userName { get; set; }
        public string? userCode { get; set; }
        public string? userActor { get; set; }
        public string? userRole { get; set; }
        public string? regionId { get; set; }
        public string? hospitalId { get; set; }
        public string? departmentId { get; set; }
        public string? password { get; set; }
    }

    public class TUserEntity
    {
        // 主键
        public string? UserId { get; set; }

        // 用户编号
        public string? UserCode { get; set; }

        // 快输输入码
        public string? SRM { get; set; }

        // 用户名
        public string? UserName { get; set; }

        // 英文名
        public string? UserNameEng { get; set; }

        // 部门ID
        public string? DepartmentId { get; set; }

        // 角色
        public string? UserActor { get; set; }

        // 权限
        public string? UserRole { get; set; }

        // 用户密码
        public string? UserPassword { get; set; }

        // 开始日期
        public string? StartDt { get; set; }

        // 结束日期
        public string? EndDt { get; set; }

        // 性别
        public string? Sex { get; set; }

        // 出生日期
        public string? Birthday { get; set; }

        // 技术职位
        public string? TechnicalPost { get; set; }

        // 职责
        public string? Duty { get; set; }

        // 入室时间
        public string? EnterDate { get; set; }

        // 离室时间
        public string? LeaveDate { get; set; }

        // 国籍
        public string? Country { get; set; }

        // 籍贯
        public string? Nationality { get; set; }

        // 身份证
        public string? IdCard { get; set; }

        // 电话
        public string? TelephoneNo { get; set; }

        // 地址
        public string? Address { get; set; }

        // Email
        public string? Email { get; set; }

        // 状态
        public string? Status { get; set; }

        // 备注
        public string? UserComment { get; set; }

        // 删除标志
        public string? Deleted { get; set; }

        // pacsid
        public string? PacsId { get; set; }

        // pacs用户密码
        public string? PacsPwd { get; set; }

        // 排序字段
        public string? Sequence { get; set; }

        // modality
        public string? Modality { get; set; }

        // 照片
        public byte[]? PersonalPic { get; set; }

        // 签名
        public byte[]? SignaturePic { get; set; }

        // 是否锁住
        public string? Locked { get; set; }

        // 是否用户外部认证
        public string? UseExAuth { get; set; }

        // 外部认证ID
        public string? ExternalId { get; set; }

        // 上次登录日期
        public string? LastLoginDt { get; set; }

        // 单点登录的用户编码
        public string? SingleUserCode { get; set; }

        // 加密的用户密码
        public string? EncryptedUserPwd { get; set; }

        // 活动IP地址
        public string? ActiveIpAddress { get; set; }

        // 心跳时间
        public string? Heartbeat { get; set; }

        // 上次密码修改时间
        public string? ModifyPwdDt { get; set; }

        // 用户可见的受控检查条件，json字符串
        public string? VisibleOrder { get; set; }

        // 瞩目视频会议系统的用户编码
        public string? ZhumuCode { get; set; }

        // 用户登录账户（WebRIS用户账户，全局唯一）
        public string? LoginUserCode { get; set; }

        // 用户锁定时间
        public string? LockedDt { get; set; }

    }
}
