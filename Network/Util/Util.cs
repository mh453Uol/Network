using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Network.Util
{
    public static class Util
    {
        public static Nullable<Guid> GetUserId(this ClaimsPrincipal claims)
        {
            if (claims.Identity.IsAuthenticated)
            {
                return Guid.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            else
            {
                return null;
            }
        }
    }
}
