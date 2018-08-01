using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace IdServer.DB
{
    public class RentdClientStore : IClientStore
    {
        private readonly IdContext _context;
        private readonly AppSettings _appSettings;

        public RentdClientStore(IOptions<AppSettings> appSettings, IdContext context)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _context.Users.Where(u => u.Id.Equals(clientId));

            if (clientId.Equals("rentd_web"))
            {
                return Task.FromResult(ReturnRentdWebClient());
            }

            if (client == null)
            {
                throw new Exception($"client with id {clientId} was not found");
            }

            //3rd party clients
            //application clients
            throw new NotImplementedException();
        }

        public Client ReturnRentdWebClient()
        {
            var client = new Client()
            {
                Enabled = true,
                ClientId = "rentd_web",
                ClientName = "Rentd Web",
                ClientUri = _appSettings.BaseUrls.Web,
                RequireConsent = false,
                AllowAccessTokensViaBrowser = true,
                AllowedGrantTypes = GrantTypes.Hybrid,
                AccessTokenLifetime = 3600,
                ClientSecrets = new List<Secret>()
                {
                    new Secret("IRTIROHEGPRIRITPWTI4T45HT4UGMFMNFDKJRJGSDRJKBGSRJGRFMVFMB".Sha256())
                },
                RedirectUris = new List<string>
                {
                    $"{_appSettings.BaseUrls.Web}signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    $"{_appSettings.BaseUrls.Web}"
                },
                AllowedScopes = new List<string>
                {
                    StandardScopes.OpenId,
                    StandardScopes.OfflineAccess,
                    StandardScopes.Profile,
                    "RENTDAPI1" //our web app can access these routes exclusively with this scope
                    // we can segrigate our endpoint to specfic people/api
                }
            };

            return client;
        }
    }
}
