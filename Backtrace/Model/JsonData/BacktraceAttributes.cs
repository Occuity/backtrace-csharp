﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Runtime.CompilerServices;
using System.Linq;

[assembly: InternalsVisibleTo("Backtrace.Tests")]
namespace Backtrace.Model.JsonData
{
    //todo: Add custom converter to values
    //missing information about: context switches, idle cpu time, iowait, kernel, nice, 

    /// <summary>
    /// Class instance to get a built-in attributes from current application
    /// </summary>
    internal class BacktraceAttributes<T>
    {
        /// <summary>
        /// Get built-in attributes
        /// </summary>
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();

        /// <summary>
        /// Create instance of Backtrace Attribute
        /// </summary>
        /// <param name="report">Received report</param>
        /// <param name="scopedAttributes">Client scoped attributes</param>
        public BacktraceAttributes(BacktraceReport<T> report, Dictionary<string, T> scopedAttributes)
        {
            Attributes = BacktraceReport<T>.ConcatAttributes(report, scopedAttributes)
                .ToDictionary(n => n.Key, v => v.Value.ToString());

            //A unique identifier to a machine
            //Environment attributes override user attributes
            Attributes["guid"] = Guid.NewGuid().ToString();
            //Base name of application generating the report
            Attributes["application"] = report.CallingAssembly.GetName().Name;
            SetMachineAttributes();
            SetProcessAttributes();
            SetExceptionAttributes(report.Exception);
        }

        /// <summary>
        /// Set attributes from exception
        /// </summary>
        internal void SetExceptionAttributes(Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            Attributes["classifier"] = exception.GetType().FullName;
            Attributes["error.Message"] = exception.Message;
        }

        /// <summary>
        /// Set attributes from current process
        /// </summary>
        private void SetProcessAttributes()
        {
            var process = Process.GetCurrentProcess();

            //How long the application has been running] = in millisecounds
            Attributes["process.age"] = process.TotalProcessorTime.TotalMilliseconds.ToString();
            Attributes["gc.heap.used"] = GC.GetTotalMemory(false).ToString();

            try
            {
                Attributes["cpu.process.count"] = Process.GetProcesses().Count().ToString();

                //Resident memory usage.
                Attributes["vm.rss.size"] = process.PagedMemorySize64.ToString();

                //Peak resident memory usage.
                Attributes["vm.rss.peak"] = process.PeakPagedMemorySize64.ToString();

                //Virtual memory usage
                Attributes["vm.vma.size"] = process.VirtualMemorySize64.ToString();

                //Peak virtual memory usage
                Attributes["vm.wma.peak"] = process.PeakVirtualMemorySize64.ToString();
            }
            catch (Exception exception)
            {
                Trace.TraceWarning($"Cannot retrieve information about process memory: ${exception.Message}");
            }
        }

        /// <summary>
        /// Set attributes about current machine
        /// </summary>
        private void SetMachineAttributes()
        {
            //The processor architecture.
            Attributes["uname.machine"] = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

            //Operating system name] = such as "windows"
            Attributes["uname.sysname"] = Environment.OSVersion.Platform.ToString();

            //The version of the operating system
            Attributes["uname.version"] = Environment.OSVersion.Version.ToString();

            //The count of processors on the system
            Attributes["cpu.count"] = Environment.ProcessorCount.ToString();

            //CPU brand string or type.
            Attributes["cpu.brand"] = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");

            //Time when system was booted
            Attributes["cpu.boottime"] = Environment.TickCount.ToString();


            //Time when system was booted

            //The hostname of the crashing system.
            Attributes["hostname"] = Environment.MachineName;


        }
    }
}
