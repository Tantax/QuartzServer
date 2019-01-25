using log4net;
using log4net.Config;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));

            UseDBConfig();

        }

        /// <summary>
        /// 第一种方法：使用配置文件
        /// </summary>
        public static void UseConfigFile()
        {

            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();
                x.SetDescription("QuartzDemo服务描述");
                x.SetDisplayName("QuartzDemo服务显示名称");
                x.SetServiceName("QuartzDemo服务名称");

                x.Service(factory =>
                {
                    QuartzServer server = new QuartzServer();
                    server.Initialize().GetAwaiter().GetResult();
                    return server;
                });

            });
        }

        /// <summary>
        /// 第二种方法：使用RAMJobStore
        /// </summary>
        public static async void UseRAMJobStore()
        {
            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            // get a scheduler
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            // define the job and tie it to our SampleJob class
            IJobDetail job = JobBuilder.Create<SampleJob>()
                .WithIdentity("myJob", "group1")
                .UsingJobData("jobSays", "HelloWorld")
                .Build();

            // Trigger the job to run now, and then every 4 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                //.StartNow()
                //.WithSimpleSchedule(x => x
                //    .WithIntervalInSeconds(4)
                //    .RepeatForever())
                .WithCronSchedule("0/10 * * * * ?")
                .Build();

            await sched.ScheduleJob(job, trigger);

            Console.ReadKey();

        }

        /// <summary>
        /// 第三种方法：使用数据库
        /// </summary>
        public static async void UseDBConfig()
        {
            
            try
            {
                //NameValueCollection props = new NameValueCollection
                //{
                //    ["quartz.scheduler.instanceName"] = "TestScheduler",
                //    ["quartz.scheduler.instanceId"] = "instance_one",
                //    ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                //    ["quartz.threadPool.threadCount"] = "5",
                //    ["quartz.jobStore.misfireThreshold"] = "60000",
                //    ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                //    ["quartz.jobStore.useProperties"] = "false",
                //    ["quartz.jobStore.dataSource"] = "default",
                //    ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                //    ["quartz.jobStore.clustered"] = "true",
                //    ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz",
                //    ["quartz.dataSource.default.connectionString"] = "Database=quartznet;Server=192.168.0.40;PORT=3306;uid=root;pwd=zztx123!@#",
                //    ["quartz.dataSource.default.provider"] = "MySql",
                //    ["quartz.serializer.type"] = "json"
                //};
                //NameValueCollection props = new NameValueCollection
                //{
                //    { "quartz.serializer.type", "json" }
                //};

                StdSchedulerFactory factory = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = await factory.GetScheduler();

                await sched.Start();

                // define the job and tie it to our SampleJob class
                IJobDetail job = JobBuilder.Create<SampleJob>()
                    .WithIdentity("myJob", "group1")
                    .UsingJobData("jobSays", "HelloWorld")
                    .Build();

                // Trigger the job to run now, and then every 4 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("myTrigger", "group1")
                    //.StartNow()
                    //.WithSimpleSchedule(x => x
                    //    .WithIntervalInSeconds(4)
                    //    .RepeatForever())
                    .WithCronSchedule("0/10 * * * * ?")
                    .Build();

                

                await sched.ScheduleJob(job, trigger);

            }
            catch (JobExecutionException ex)
            {
                await Console.Error.WriteLineAsync("出错了，错误信息：" + ex.ToString());
            }
            Console.ReadKey();

        }
    }
}
