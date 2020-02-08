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
                     Ҫ��ʹ����֤ϵͳ����Ҫ��ע��Scheme
                     ��ÿһ��Scheme����ָ��һ��Handler
                     AuthenticationHandler ������û�ƾ֤����֤
                     ����ָ����Ĭ����֤��cookie��֤
                     Scheme���Է���Ϊ��������Ĭ�ϵ���֤����

                    ��Ϊ�����õ��˶���м������AddAuthentication��AddCookie��AddOpenIdConnect��
                    OpenIdConnectDefaults.DisplayName ��Ĭ��ֵ��oidc
                    ָ��AddOpenIdConnect��Ĭ���м������AddOpenIdConnect�����˺ܶ�ѡ��

                    ���ֻ����һ���м��������Բ�д���Ƿ񻹼ǵ�cookie��֤
                    //     services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    //     .AddCookie(option =>
                    //     {
                    //         ///Account/Login?ReturnUrl=%2Fadmin
                    //         option.LoginPath = "/login/index";
                    //         //option.ReturnUrlParameter = "params"; //ָ����������
                    //         //option.Cookie.Domain
                    //         option.AccessDeniedPath = "/login/noAccess";
                    //         option.Cookie.Expiration = TimeSpan.FromSeconds(4);
                    //         option.Events = new CookieAuthenticationEvents
                    //         {
                    //             OnValidatePrincipal = LastChangedValidator.ValidateAsync
                    //         };
                    //     });

                     */
                    //Ĭ�ϵ���֤������cookie��֤����Ϣ�Ǳ�����cookie�е�
                    options.DefaultScheme = "Cookies";
                    //�������ȡ��ֻҪAddOpenIdConnect�еĵ�oidc����һ�����ɣ�
                    //���������ҵ�
                    options.DefaultChallengeScheme = "oidc";
                    //Ĭ��ʹ��oidc�м��
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
                     Ĭ��ֵ��:id_token
                     */
                    //options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    //options.Events = new OpenIdConnectEvents
                    //{
                    //    /*
                    //     Զ���쳣����
                    //     ����Ȩ������ȡ����½����ȡ����Ȩ      
                    //     */
                    //    OnRemoteFailure = OAuthFailureHandler =>
                    //    {
                    //        //��ת��ҳ
                    //        OAuthFailureHandler.Response.Redirect("/");
                    //        OAuthFailureHandler.HandleResponse();
                    //        return Task.FromResult(0);
                    //    }
                    //};
                    /***********************************����¼�***********************************/
                    // δ��Ȩʱ���ض���OIDC������ʱ����
                    //o.Events.OnRedirectToIdentityProvider = context => Task.CompletedTask;

                    // ��ȡ����Ȩ��ʱ����
                    //o.Events.OnAuthorizationCodeReceived = context => Task.CompletedTask;
                    // ���յ�OIDC���������ص���֤��Ϣ������Code, ID Token�ȣ�ʱ����
                    //o.Events.OnMessageReceived = context => Task.CompletedTask;
                    // ���յ�TokenEndpoint���ص���Ϣʱ����
                    //o.Events.OnTokenResponseReceived = context => Task.CompletedTask;
                    // ��֤Tokenʱ����
                    //o.Events.OnTokenValidated = context => Task.CompletedTask;
                    // ���յ�UserInfoEndpoint���ص���Ϣʱ����
                    //o.Events.OnUserInformationReceived = context => Task.CompletedTask;
                    // �����쳣ʱ����
                    //o.Events.OnAuthenticationFailed = context => Task.CompletedTask;

                    // �˳�ʱ���ض���OIDC������ʱ����
                    //o.Events.OnRedirectToIdentityProviderForSignOut = context => Task.CompletedTask;
                    // OIDC�������˳��󣬷���˻ص�ʱ����
                    //o.Events.OnRemoteSignOut = context => Task.CompletedTask;
                    // OIDC�������˳��󣬿ͻ����ض���ʱ����
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
