using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace XI.Portal.Library.Auth
{
    public class ApplicationRoleManager : RoleManager<IdentityRole>, IDisposable
    {
        public ApplicationRoleManager(RoleStore<IdentityRole> store) : base(store) { }
    }
}
