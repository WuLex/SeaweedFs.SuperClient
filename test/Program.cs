using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using smartbox.SeaweedFs.Client;
using smartbox.SeaweedFs.Client.Core.File;
using FileHost.Data;
using FileHost.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new ZX().Op();
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }

    public class ZX
    {
        private readonly SeaweedFsService service = TestService.Provider.GetService<SeaweedFsService>();

        public async Task Op()
        {
            using (var m = new MemoryStream())
            {
                await m.WriteAsync(Encoding.UTF8.GetBytes("images"));
                await m.FlushAsync();
                m.Seek(0, SeekOrigin.Begin);
                await service.UploadAsync("{datadir:2}/test.jpg", m);

                var r = await service.DownloadAsync("{datadir:2}/test.jpg");
                //Assert.IsNotNull(r);

                var buffer = new byte[(int) r.Length];
                await r.ReadAsync(buffer, 0, (int) r.Length);
                r.Dispose();
                //Assert.AreEqual("test", Encoding.UTF8.GetString(buffer));
            }
        }

        //public async Task FileTest()
        //{
        //    await using (var m = new MemoryStream())
        //    {
        //        await m.WriteAsync(Encoding.UTF8.GetBytes("test"));
        //        await m.FlushAsync();
        //        m.Seek(0, SeekOrigin.Begin);
        //        await service.UploadAsync("{datadir:2}/test.jpg", m);

        //        var r = await service.DownloadAsync("{datadir:2}/test.jpg");
        //        //Assert.IsNotNull(r);

        //        var buffer = new byte[(int)r!.Length];
        //        await r.ReadAsync(buffer, 0, (int)r.Length);
        //        await r.DisposeAsync();
        //        //Assert.AreEqual("test", Encoding.UTF8.GetString(buffer));
        //    }

        //    await using (var m = new MemoryStream())
        //    {
        //        await m.WriteAsync(Encoding.UTF8.GetBytes("test2"));
        //        await m.FlushAsync();
        //        m.Seek(0, SeekOrigin.Begin);
        //        await service.UploadAsync("{datadir:2}/test.jpg", m);

        //        var r = await service.DownloadAsync("{datadir:2}/test.jpg");
        //        //Assert.IsNotNull(r);

        //        var buffer = new byte[(int)r!.Length];
        //        await r.ReadAsync(buffer, 0, (int)r.Length);
        //        await r.DisposeAsync();
        //        //Assert.AreEqual("test2", Encoding.UTF8.GetString(buffer));
        //    }

        //    var d = await service.DeleteAsync("{datadir:2}/test.jpg");
        //    //Assert.IsTrue(d);

        //    //Assert.IsNull(await service.DownloadAsync("{datadir:2}/test.jpg"));
        //}
    }

    class TestService
    {
        public static IServiceProvider Provider = InitService();

        private static IServiceProvider InitService()
        {
            var services = new ServiceCollection();

            services.AddDbContext<FileHostDbContext>(options =>
            {
                options.UseInMemoryDatabase("test");
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.EnableServiceProviderCaching();
            });

            services.AddEntityFrameworkInMemoryDatabase();

            services.AddScoped<SeaweedFsService>()
                .Configure<SeaweedFsOptions>(options =>
                {
                    options.MasterHostName = "localhost";
                    options.Port = 9333;
                });

            return services.BuildServiceProvider();
        }
    }
}