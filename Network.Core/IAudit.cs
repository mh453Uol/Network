using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Core
{
    public interface IAudit
    {
        public ApplicationUser CreatedBy { get; set; }
        public Guid CreatedById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public Guid UpdatedById { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public bool IsDeleted{ get; set; }
    }
}
