using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Core
{
    public class UserRole: IdentityRole<Guid>
    {
        public UserRole(): base()
        {

        }
    }
}
