using System;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Data.Deploy
{
    internal class DefaultDataSeed
    {
        public void SeedData(PortalContext context)
        {
            foreach (var role in Enum.GetValues(typeof(XtremeIdiotsGroups)))
            {
                var roleString = ((XtremeIdiotsGroups) role).ToString();
                if (context.Roles.SingleOrDefault(r => r.Name == roleString) == null)
                    context.Roles.Add(new IdentityRole(roleString));
            }

            context.SaveChanges();
        }
    }
}