﻿using Quartz;
using Quartz.Impl;
using Slf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartzmin.TopshelfHost
{
    public class QuartzSchedulerService
    {
        private readonly IScheduler scheduler;

        public QuartzSchedulerService()
        {
            var conStr = ConfigurationManager.ConnectionStrings["QUARTZNETJOBDB"]?.ConnectionString;
            try
            {
                NameValueCollection configuration = new NameValueCollection
                {
                     { "quartz.scheduler.instanceName", "LocalServer" },
                     { "quartz.scheduler.instanceId", "LocalServer" },
                     { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                     //{ "quartz.jobStore.useProperties", "true" },
                     { "quartz.jobStore.dataSource", "default" },
                     { "quartz.jobStore.tablePrefix", "QRTZ_" },
                     { "quartz.dataSource.default.connectionString",conStr },
                     { "quartz.dataSource.default.provider", "SqlServer" },
                     //{ "quartz.threadPool.threadCount", "1" },
                     { "quartz.serializer.type", "binary" }
                };

                configuration["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                configuration["quartz.threadPool.threadCount"] = "5";
                configuration["quartz.threadPool.threadPriority"] = "Normal";
                configuration["quartz.plugin.recentHistory.type"] = "Quartz.Plugins.RecentHistory.ExecutionHistoryPlugin, Quartz.Plugins.RecentHistory";

                configuration["quartz.plugin.recentHistory.storeType"] = "Quartz.Plugins.RecentHistory.Impl.InProcExecutionHistoryStore, Quartz.Plugins.RecentHistory";

                StdSchedulerFactory factory = new StdSchedulerFactory(configuration);
                scheduler = factory.GetScheduler().GetAwaiter().GetResult();
            }
            catch (System.Exception ex)
            {
                LoggerService.GetLogger("LOGIJMS").Log(new LogItem()
                {
                    LoggerName = "LOGIJMS",
                    Title = "Scheduler Service Create",
                    Message = "Scheduler Service Create",
                    LogItemProperties = new List<LogItemProperty>() {
                                new LogItemProperty("ServiceName", "JOB") ,
                                new LogItemProperty("AppName", "LogiJMS.TopshelfHost") ,
                                new LogItemProperty("ActionName", "QuartzSchedulerService")
                            },
                    LogLevel = LogLevel.Error,
                    Exception = ex
                });
            }
        }

        public void Start()
        {
            scheduler.Start().ConfigureAwait(false).GetAwaiter().GetResult();

            if (scheduler.IsStarted)
            {
                LoggerService.GetLogger("LOGIJMS").Log(new LogItem()
                {
                    LoggerName = "LOGIJMS",
                    Title = "Scheduler Started",
                    Message = "Scheduler Started",
                    LogItemProperties = new List<LogItemProperty>() {
                                new LogItemProperty("ServiceName", "JOB") ,
                                new LogItemProperty("AppName", "LogiJMS.TopshelfHost") ,
                                new LogItemProperty("ActionName", "Start")
                            },
                    LogLevel = LogLevel.Info
                });
            }
            else
            {
                LoggerService.GetLogger("LOGIJMS").Log(new LogItem()
                {
                    LoggerName = "LOGIJMS",
                    Title = "Scheduler Not Started",
                    Message = "Scheduler Not  Started",
                    LogItemProperties = new List<LogItemProperty>() {
                                new LogItemProperty("ServiceName", "JOB") ,
                                new LogItemProperty("AppName", "LogiJMS.TopshelfHost") ,
                                new LogItemProperty("ActionName", "Start")
                            },
                    LogLevel = LogLevel.Info
                });
            }
        }

        public void Stop()
        {
            scheduler.Shutdown().ConfigureAwait(false).GetAwaiter().GetResult();
            LoggerService.GetLogger("LOGIJMS").Log(new LogItem()
            {
                LoggerName = "LOGIJMS",
                Title = "Scheduler Stopped!",
                Message = "Scheduler Stopped!",
                LogItemProperties = new List<LogItemProperty>() {
                                new LogItemProperty("ServiceName", "JOB") ,
                                new LogItemProperty("AppName", "LogiJMS.TopshelfHost") ,
                                new LogItemProperty("ActionName", "Stop")
                            },
                LogLevel = LogLevel.Info
            });
        }
    }
}