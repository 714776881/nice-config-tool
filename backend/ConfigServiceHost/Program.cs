using ConfigService.Business;
using ConfigService.Request;
using ConfigServiceHost.Business;
using ConfigServiceHost.Filter;
using log4net.Appender;
using log4net.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Model.Config;
using Model.Resource;
using ServiceManager.Tool;
using System.Net;
using System.ServiceModel;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Tool;
using XService;
using static System.Net.Mime.MediaTypeNames;

namespace ConfigServiceHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogAdapter.LogInfo("ConfigServiceHost...");

            ConfigManagerServiceAgent Agent = new ConfigManagerServiceAgent();
            if (Agent.Initialize() == false)
            {
                LogAdapter.LogError("配置管理服务初始化失败.");
                System.Environment.Exit(0);
            }

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
                options.Filters.Add(typeof(GlobalExceptionsFilter));
            }).AddJsonOptions(options =>
            {
                //格式化日期时间格式
                //options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
                //数据格式首字母小写
                //options.JsonSerializerOptions.PropertyNamingPolicy =JsonNamingPolicy.CamelCase;
                //数据格式原样输出
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //取消Unicode编码
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //忽略空值
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                //允许额外符号
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                //反序列化过程中属性名称是否使用不区分大小写的比较
                //options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;（********）
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

            //注册appsettings读取类
            builder.Services.AddSingleton(new ConfigHelper(builder.Configuration));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Cors", p =>
                {
                    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.KeyLengthLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue;
                options.Limits.MaxRequestBufferSize = int.MaxValue;
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60); // 会话超时
                options.Cookie.HttpOnly = true; // 防止客户端访问会话
            });

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = "";
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
            });

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            // 启用会话中间件
            app.UseSession();

            app.MapControllers();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "LoginManagerService/{controller}/{action}/{id}");
            //});


            //使用跨域策略
            app.UseCors("Cors");
            app.Run();
            LogAdapter.LogInfo("配置管理服务成功启动！");
        }

        private static void OnShutdown()
        {
            // 在这里执行应用程序关闭时的清理工作
            // 例如关闭数据库连接、释放资源等
            Environment.Exit(0);
        }
    }
}