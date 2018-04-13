using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationBase
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthenticationCore(options => options.AddScheme<MyAuthenticationHandler>("myScheme", "demo scheme"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 登录
            app.Map("/login", builder => builder.Use(next =>
            {
                return async (context) =>
                {
                    var claimIdentity = new ClaimsIdentity("MyAuthentication");
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Name, "jim"));
                    await context.SignInAsync("myScheme", new ClaimsPrincipal(claimIdentity));
                    await context.Response.WriteAsync("jim login");
                };
            }));

            // 退出
            app.Map("/logout", builder => builder.Use(next =>
            {
                return async (context) =>
                {
                    await context.SignOutAsync("myScheme");
                    await context.Response.WriteAsync("logout");
                };
            }));

            // 认证
            app.Use(next =>
            {
                return async (context) =>
                {
                    var result = await context.AuthenticateAsync("myScheme");
                    if (result?.Principal != null)
                        context.User = result.Principal;

                    await next(context);
                };
            });

            // 授权
            app.Use(async (context, next) =>
            {
                var user = context.User;

                if (user?.Identity?.IsAuthenticated ?? false)
                {
                    if (user.Identity.Name != "jim")
                        await context.ForbidAsync("myScheme");
                    else
                        await next();
                }
                else
                {
                    await context.ChallengeAsync("myScheme");
                }
            });

            // 访问受保护资源
            app.Map("/resource", builder => builder.Run(async (context) => await context.Response.WriteAsync("Hello, ASP.NET Core!")));
        }
    }
}
