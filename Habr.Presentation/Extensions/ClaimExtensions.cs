﻿using System.Security.Claims;
using System.Security.Principal;
using Habr.Common.Exceptions;

namespace Habr.Presentation.Extensions
{
    static public class ClaimExtensions
    {
        static public int GetAuthorizedUserId(this IIdentity principal)
        {
            var identity = principal as ClaimsIdentity;
            var claim = identity.Claims;
            var id = claim
                .Where(x => x.Type == "jti")
                .FirstOrDefault();

            if (id is null)
            {
                throw new AuthorizationException(Common.Resources.UserExceptionMessageResource.AccessError);
            }

            return int.Parse(id.Value);
        }

        static public string GetAuthorizedUserRole(this IIdentity principal)
        {
            var identity = principal as ClaimsIdentity;
            var claim = identity.Claims;
            var id = claim
                .Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                .FirstOrDefault();

            if (id is null)
            {
                throw new AuthorizationException(Common.Resources.UserExceptionMessageResource.AccessError);
            }

            return id.Value;
        }
    }
}
