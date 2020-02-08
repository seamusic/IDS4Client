using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NetcoreClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services
                .AddAuthentication(options =>
                {
                    /*
                     要想使用认证系统，必要先注册Scheme
                     而每一个Scheme必须指定一个Handler
                     AuthenticationHandler 负责对用户凭证的验证
                     这里指定的默认认证是cookie认证
                     Scheme可以翻译为方案，即默认的认证方案

                    因为这里用到了多个中间件，（AddAuthentication，AddCookie，AddOpenIdConnect）
                    OpenIdConnectDefaults.DisplayName 的默认值是oidc
                    指定AddOpenIdConnect是默认中间件，在AddOpenIdConnect配置了很多选项

                    如果只用了一个中间件，则可以不写，是否还记得cookie认证
                    //     services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    //     .AddCookie(option =>
                    //     {
                    //         ///Account/Login?ReturnUrl=%2Fadmin
                    //         option.LoginPath = "/login/index";
                    //         //option.ReturnUrlParameter = "params"; //指定参数名称
                    //         //option.Cookie.Domain
                    //         option.AccessDeniedPath = "/login/noAccess";
                    //         option.Cookie.Expiration = TimeSpan.FromSeconds(4);
                    //         option.Events = new CookieAuthenticationEvents
                    //         {
                    //             OnValidatePrincipal = LastChangedValidator.ValidateAsync
                    //         };
                    //     });

                     */
                    //默认的认证方案：cookie认证，信息是保存在cookie中的
                    options.DefaultScheme = "Cookies";
                    //名字随便取，只要AddOpenIdConnect中的的oidc名字一样即可，
                    //这样才能找到
                    options.DefaultChallengeScheme = "oidc";
                    //默认使用oidc中间件
                    //options.DefaultChallengeScheme = OpenIdConnectDefaults.DisplayName;
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "netcoremvcclient";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    //options.Scope.Add("offline_access");
                    options.Scope.Add("email");
                    options.Scope.Add("api");
                    /*
                     默认值是:id_token
                     */
                    //options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    //options.Events = new OpenIdConnectEvents
                    //{
                    //    /*
                    //     远程异常触发
                    //     在授权服务器取消登陆或者取消授权      
                    //     */
                    //    OnRemoteFailure = OAuthFailureHandler =>
                    //    {
                    //        //跳转首页
                    //        OAuthFailureHandler.Response.Redirect("/");
                    //        OAuthFailureHandler.HandleResponse();
                    //        return Task.FromResult(0);
                    //    }
                    //};
                    /***********************************相关事件***********************************/
                    // 未授权时，重定向到OIDC服务器时触发
                    //o.Events.OnRedirectToIdentityProvider = context => Task.CompletedTask;

                    // 获取到授权码时触发
                    //o.Events.OnAuthorizationCodeReceived = context => Task.CompletedTask;
                    // 接收到OIDC服务器返回的认证信息（包含Code, ID Token等）时触发
                    //o.Events.OnMessageReceived = context => Task.CompletedTask;
                    // 接收到TokenEndpoint返回的信息时触发
                    //o.Events.OnTokenResponseReceived = context => Task.CompletedTask;
                    // 验证Token时触发
                    //o.Events.OnTokenValidated = context => Task.CompletedTask;
                    // 接收到UserInfoEndpoint返回的信息时触发
                    //o.Events.OnUserInformationReceived = context => Task.CompletedTask;
                    // 出现异常时触发
                    //o.Events.OnAuthenticationFailed = context => Task.CompletedTask;

                    // 退出时，重定向到OIDC服务器时触发
                    //o.Events.OnRedirectToIdentityProviderForSignOut = context => Task.CompletedTask;
                    // OIDC服务器退出后，服务端回调时触发
                    //o.Events.OnRemoteSignOut = context => Task.CompletedTask;
                    // OIDC服务器退出后，客户端重定向时触发
                    //o.Events.OnSignedOutCallbackRedirect = context => Task.CompletedTask;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
