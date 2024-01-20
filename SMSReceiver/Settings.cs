using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSReceiver
{
    public class Settings
    {

        public string MqttAddress = "";
        public short MqttPort = 8333;
        public string MqttUsername = "";
        public string MqttPassword = "";
        public string MqttTopic = "";
        public bool EnableAutoCopy = true;
        public bool ShowAllSms = true;
    }
}
