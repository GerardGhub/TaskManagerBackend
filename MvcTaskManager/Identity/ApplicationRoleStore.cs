using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MvcTaskManager.Identity
{
    public class ApplicationRoleStore : RoleStore<ApplicationRole, ApplicationDbContext>
    {
        public ApplicationRoleStore(ApplicationDbContext applicationDbContext, IdentityErrorDescriber identityErrorDescriber) : base(applicationDbContext, identityErrorDescriber)
        {

        }
    }
}


