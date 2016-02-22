using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features
{
    public static class Utility
    {
        private static string _newqueue;
        //added on 19-1-2016
        public static string NewQueue
        {
            get
            {
                return _newqueue;
            }
            set
            {
                _newqueue = value;
            }
        }
        //ends here 
        //added on 21-1-2016
        private static string _qId;
        private static string _qmsg;
        private static string _qadded;
        private static string _qexpry;
        public static string QID
        {
            get
            {
                return _qId;
            }
            set
            {
                 _qId = value;
            }
        }
        public static string Qmsg
        {
            get
            {
                return _qmsg;
            }
            set
            {
                _qmsg = value;
            }
        }
        public static string QAdded
        {
            get
            {
                return _qadded;
            }
            set
            {
                _qadded = value;
            }
        }
        public static string QExpry
        {
            get
            {
                return _qexpry;
            }
            set
            {
                _qexpry = value;
            }
        }
        //ends here 
        public static bool IsNetConnected()
        {
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

                System.Net.NetworkInformation.PingReply pingStatus =
                    ping.Send(IPAddress.Parse("208.69.34.231"), 1000);

                if (pingStatus.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception  e)
            {
                return false;
            }
          
           
            //string host = "http://www.sysfore.com/";
            //bool result = false;
            //Ping p = new Ping();
            //try
            //{
            //    PingReply reply = p.Send(host, 3000);
            //    if (reply.Status == IPStatus.Success)
            //        return true;
            //}
            //catch { }
            //return result;
        }

    }
}
