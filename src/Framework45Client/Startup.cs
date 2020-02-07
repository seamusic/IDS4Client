using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using IdentityModel.Client;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace Framework45Client
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                //CookieHttpOnly = true
            });

            var secret = "secret";
            secret = GetSHA256HashFromString(secret);

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "http://localhost:5000",
                ClientId = "framework45client",
                ClientSecret = secret,
                ResponseType = "id_token code",
                RedirectUri = "http://localhost:3047",
                PostLogoutRedirectUri = "http://localhost:3047/Home/OidcSignOut",
                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                UseTokenLifetime = false,
                RequireHttpsMetadata = false,
                Scope = "openid email",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    //AuthorizationCodeReceived = async n =>
                    //{
                    //    // use the code to get the access and refresh token
                    //    var tokenClient = new TokenClient(
                    //        "http://localhost:5000",
                    //        "framework45client",
                    //        "secret");

                    //    var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(
                    //        n.Code, n.RedirectUri);

                    //    if (tokenResponse.IsError)
                    //    {
                    //        throw new Exception(tokenResponse.Error);
                    //    }

                    //    // use the access token to retrieve claims from userinfo
                    //    var userInfoClient = new UserInfoClient("http://localhost:5000/connect/userinfo");

                    //    var userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);

                    //    // create new identity
                    //    var id = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                    //    id.AddClaims(userInfoResponse.Claims);

                    //    id.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                    //    id.AddClaim(new Claim("expires_at", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToLocalTime().ToString()));
                    //    id.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
                    //    id.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                    //    id.AddClaim(new Claim("sid", n.AuthenticationTicket.Identity.FindFirst("sid").Value));

                    //    n.AuthenticationTicket = new AuthenticationTicket(
                    //        new ClaimsIdentity(id.Claims, n.AuthenticationTicket.Identity.AuthenticationType, "name", "role"),
                    //        n.AuthenticationTicket.Properties);
                    //},
                    //SecurityTokenValidated = async n =>
                    //{
                    //    var nid = new ClaimsIdentity(
                    //        n.AuthenticationTicket.Identity.AuthenticationType,
                    //        "given_name",
                    //        "role");

                    //    // get userinfo data
                    //    //var userInfoClient = new UserInfoClient(n.Options.Authority + "/connect/userinfo");

                    //    //var userInfo = await userInfoClient.GetAsync(n.ProtocolMessage.AccessToken);
                    //    //userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui., ui.Item2)));

                    //    // keep the id_token for logout
                    //    nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                    //    // add access token for sample API
                    //    nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                    //    // keep track of access token expiration
                    //    nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                    //    // add some other app specific claim
                    //    nid.AddClaim(new Claim("app_specific", "some data"));

                    //    n.AuthenticationTicket = new AuthenticationTicket(
                    //        nid,
                    //        n.AuthenticationTicket.Properties);
                    //},

                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectRequestType.Logout)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            });

            //app.UseAuthentication();
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string GetSHA256HashFromString(string strData)
        {
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(strData);
            SHA256 sha256 = new SHA256CryptoServiceProvider();

            byte[] retVal = sha256.ComputeHash(bytValue);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}