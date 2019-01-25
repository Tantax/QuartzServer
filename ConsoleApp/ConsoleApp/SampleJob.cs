using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class SampleJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SampleJob));

        /// <summary>
        /// 是否单例执行
        /// </summary>
        public bool DisallowConcurrentExecution { get; set; }

        /// <summary>
        /// 是否存储数据更改，如把JobSays的变化后的值带入到下一次调度任务中
        /// </summary>
        public bool PersistJobDataAfterExecution { get; set; }

        /// <summary>
        /// job数据获取测试属性
        /// </summary>
        public string JobSays { get; set; }

        /// <summary>
        /// 任务执行方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                logger.Info("开始执行任务...");
                await Task.Delay(TimeSpan.FromSeconds(1));
                JobKey key = context.JobDetail.Key;//显示group1.myJob

                //JobDataMap dataMap = context.JobDetail.JobDataMap;
                //string jobSays = dataMap.GetString("jobSays");//JobSays可以直接写成属性，会自动映射

                await Console.Error.WriteLineAsync("Instance " + key + " of SampleJob says: " + JobSays);
                logger.Info("任务结束！");
            }
            catch (JobExecutionException)//官方推荐异常捕获对象
            {

                throw;
            }
        }
    }
}
