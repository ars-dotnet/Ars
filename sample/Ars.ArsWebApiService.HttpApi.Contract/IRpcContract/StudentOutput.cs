using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.ArsWebApiService.HttpApi.Contract.IRpcContract
{
    public class StudentOutput
    {
        public Guid Id { get; set; }

        public string LastName { get; set; }

        public string FirstMidName { get; set; }

        public DateTime EnrollmentDate { get; set; }

        /// <summary>
        /// 并发标记
        /// </summary>
        public DateTime TimeStamp { get; set; }

        public int? TenantId { get; set; }

        public int? CreationUserId { get; set; }

        public DateTime? CreationTime { get; set; }

        public int? UpdateUserId { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool IsDeleted { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? DeleteTime { get; set; }
    }
}
