using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExpressionDemo
{
    public class Users : BaseUser
    {
        /// <summary>
        /// 客户类型
        /// </summary>
        public UserType userType { set; get; }

        /// <summary>
        /// 角色id
        /// </summary>
        public int roleId { get; set; }

        /// <summary>
        /// 本次登录时间
        /// </summary>
        public DateTime? loginTime { get; set; }
    }

    public enum UserType
    {
        内部管理 = 1,

        客户账号 = 9,

        [Description("普通管理员")]
        普通管理员 = 10,
    }
    public class BaseUser
    {
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [StringLength(50)]
        [Required]
        public string AccountNumber { set; get; }

        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(100)]
        [Required]
        public string Password { set; get; }

        /// <summary>
        /// 用户状态
        /// </summary>
        [Required]
        public UserStatus userStatus { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreateDateTime { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string Remark { set; get; }

    }

    public enum UserStatus
    {
        none = 0,
        启用 = 1,
        停用 = -1
    }
}
