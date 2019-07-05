using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using HuobiProject.Models;
using HuobiProject.Interface;
using HuobiProject.Impl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using HuobiProject.Utils;
using StackExchange.Redis;
using HuobiProject.WebSocket;

namespace HuobiProject
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
            #region 注入
            //MySql
            services.AddDbContext<HuobiContext>(option => option.UseMySQL(Configuration.GetConnectionString("HuobiDB")));

            //Redis
            services.AddSingleton<HuobiProject.Interface.IRedis, RedisImpl>(opt =>
            {
                return new RedisImpl(
                    ConfigurationOptions.Parse(Configuration.GetConnectionString("HuobiRedis")),
                    Configuration["Redis:InstanceName"]);
            });

            //实体操作
            services.AddScoped<IUser, UserImpl>();
            services.AddScoped<IResource, ResourceImpl>();
            services.AddScoped<IType, TypeImpl>();
            services.AddScoped<ILang, LangImpl>();
            services.AddScoped<IArticle, ArticleImpl>();
            services.AddScoped<IComment, CommentImpl>();
            services.AddScoped<IHelpful, HelpfulImpl>();
            services.AddScoped<ITicket, TicketImpl>();
            services.AddScoped<IAttention, AttentionImpl>();
            services.AddScoped<ICommentLike, CommentLikeImpl>();
            services.AddSingleton(typeof(Token), typeof(Token));
            services.AddSingleton(typeof(Tools), typeof(Tools));
            #endregion

            //跨域
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(Configuration["AllowedHosts"]);
                    policy.WithHeaders("*");
                });
            });

            services.AddMvc()
                    .AddJsonOptions(
                        options => options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                    );

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //添加权限验证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(k =>
                        {
                            k.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = Configuration["Jwt:Issuer"],
                                ValidateAudience = true,
                                ValidAudience = Configuration["Jwt:Issuer"],
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                            };
                        });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler(builder => builder.Run(async context => await ErrorEvent(context)));
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseMvc();

            //绑定WebSocket
            app.Map("/ws", (con) =>
            {
                con.UseWebSockets(new WebSocketOptions()
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(5),
                    ReceiveBufferSize = 2 * 1024,
                });
                con.Run(async context =>
                {
                    await SocketRouteHandler.DistributeAsync(context, typeof(HuobiWebSocket));
                });
            });
        }

        public Task ErrorEvent(HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            ResultException ex;
            if (error.GetType() == typeof(ResultException))
                ex = (ResultException)error;
            else ex = new ResultException();
            context.Response.ContentType="application/json; charset=utf-8";
            context.Response.StatusCode = ex.Code;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new Result(ex.Code, ex.Message)), Encoding.GetEncoding("UTF-8"));
        }
    }
}
