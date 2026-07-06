using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Application.Queries
{
    public class GetUserProfileQuery
    {
        public Guid UserId { get; set; }

        public GetUserProfileQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
