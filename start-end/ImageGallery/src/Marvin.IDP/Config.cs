using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marvin.IDP
{
    public static class Config
    {
        // test users
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "Frank",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Frank"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "Main Road 1"),
                        new Claim("role", "FreeUser")
                    }
                },
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Claire",
                    Password = "password",

                    Claims = new List<Claim> //Information about the user
                    {
                        new Claim("given_name", "Claire"),
                        new Claim("family_name", "Soni"),
                        new Claim("address", "Big street 2"),
                        new Claim("role", "PayingUser")
                    }
                },
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ffe5h",
                    Username = "Vatan",
                    Password = "password",

                    Claims = new List<Claim> //Information about the user
                    {
                        new Claim("given_name", "Vatan"),
                        new Claim("family_name", "VatanSoni"),
                        new Claim("address", "agar malwa"),
                        new Claim("role", "PayingUser")
                    }
                }
            };
        }

        // identity-related resources (scopes)
        //The full list what we are supporting. #goto a
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(), //SubjectId
                new IdentityResources.Profile(), // Profile related scopes like given_name & family_name
                new IdentityResources.Address(),
                new IdentityResource("roles", "Your role(s)", new List<string>(){ "role"})
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientName = "Image Gallery",
                    ClientId = "imagegalleryclient",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44330/signin-oidc" //Client url + signin-oidc
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44330/signout-callback-oidc"
                    },
                    AllowedScopes = //#a allowed scope to be requested by this client.
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }
             };

        }
    }
}
