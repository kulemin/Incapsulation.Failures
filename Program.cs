using System.Collections.Generic;
using System.Linq;
using NUnitLite;

class Program
{
	static void Main(string[] args)
	{
		new AutoRun().Execute(args);
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Failures
{
    public enum FailureType
    {
        UnexpectedShutdown,
        ShortNonResponding,
        HardwareFailures,
        ConnectionProblems
    }

    public class Common
    {
        public static int WasEarlier(DateTime date1, DateTime date2)
        {
            if (date1.Year < date2.Year) return 1;
            if (date1.Year > date2.Year) return 0;
            if (date1.Month < date2.Month) return 1;
            if (date1.Month > date2.Month) return 0;
            if (date1.Day < date2.Day) return 1;
            return 0;
        }
    }

    public class ReportMaker
    {
        /// <summary>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="failureTypes">
        /// 0 for unexpected shutdown, 
        /// 1 for short non-responding, 
        /// 2 for hardware failures, 
        /// 3 for connection problems
        /// </param>
        /// <param name="deviceId"></param>
        /// <param name="times"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes,
            int[] deviceId,
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            FailureType failureType;
            DateTime firstTime = new DateTime(year, month, day);
            var failures = new Failure[failureTypes.Length];
            for (int i = 0; i < failures.Length; i++)
            {
                if (failureTypes[i] == 0) failureType = FailureType.UnexpectedShutdown;
                else if (failureTypes[i] == 1) failureType = FailureType.ShortNonResponding;
                else if (failureTypes[i] == 2) failureType = FailureType.HardwareFailures;
                else failureType = FailureType.ConnectionProblems;
                failures[i] = new Failure(failureType,
                                            deviceId[i],
                                            new DateTime((int)times[i][2],
                                            (int)times[i][1],
                                            (int)times[i][0]));
            }
            var newDevices = new List<Device>();
            foreach (var device in devices)
                newDevices.Add(new Device(device));
            return FindDevicesFailedBeforeDate(firstTime, failures, newDevices);
        }

        public static List<string> FindDevicesFailedBeforeDate(DateTime time, Failure[] failures, List<Device> devices)
        {
            var problematicDevices = new HashSet<int>();
            for (int i = 0; i < failures.Length; i++)
                if ((failures[i].FailureType == FailureType.UnexpectedShutdown
                    || failures[i].FailureType == FailureType.HardwareFailures)
                    && Common.WasEarlier(failures[i].Time, time) == 1)
                    problematicDevices.Add(failures[i].DeviceId);
            var result = new List<string>();
            foreach (var device in devices)
                if (problematicDevices.Contains((int)device.Devices["DeviceId"]))
                    result.Add(device.Devices["Name"] as string);
            return result;
        }
    }

    public class Failure
    {
        public FailureType FailureType;
        public int DeviceId;
        public DateTime Time;
        public Failure(FailureType failureType, int deviceId, DateTime time)
        {
            FailureType = failureType;
            DeviceId = deviceId;
            Time = time;
        }
    }

    public class Device
    {
        public Dictionary<string, object> Devices;
        public Device(Dictionary<string, object> devices)
        {
            Devices = devices;
        }
    }
}