using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MQTTnet.Channel;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using MQTTnet.Server;
using System.Timers;

namespace SMSReceiver
{
    public partial class Mainform : Form
    {
        public Mainform()
        {
            InitializeComponent();
        }

        public static string ConfigPath = "./config.json";
        public IMqttClient MqttClient;
        public Settings Settings;
        public DateTime LastConnectTime = DateTime.Now;
        public System.Timers.Timer ReconnectTimer;



        private async void Mainform_Load(object sender, EventArgs e)
        {
            this.Hide();
            LoadSettings();
            MqttClient = InitMqttClient();
            await Connect();
            ReconnectTimer = new System.Timers.Timer(1000);
            ReconnectTimer.Elapsed += ReconnectTimerElapsed;
            ReconnectTimer.Start();
        }

        private async void ReconnectTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!MqttClient.IsConnected && (DateTime.Now - LastConnectTime).TotalSeconds > 60)
            {
                LastConnectTime = DateTime.Now;
                Log("连接守护：检测到连接断开且60秒内没有尝试重连");
                await ReconnectMqttClient();
            }
        }

        public void LoadSettings()
        {
            if (File.Exists(ConfigPath))
            {
                string text = File.ReadAllText(ConfigPath);
                if (!string.IsNullOrEmpty(text))
                {
                    Settings = JsonConvert.DeserializeObject<Settings>(text);
                    Log($"读取配置文件\nMqttAddress: {Settings.MqttAddress}\nMqttTopic: {Settings.MqttTopic}");
                    return;
                }
            }
            Settings = new Settings();
            SaveSettings();
            MessageBox.Show("请先配置配置文件");
            Application.Exit();
        }

        public void SaveSettings()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }

        void Log(string msg)
        {
            this.Invoke(new Action(() =>
            {
                richTextBox_log.AppendText("[" + DateTime.Now.ToString() + "]\n" + msg + "\n\n");
                richTextBox_log.ScrollToCaret();
            }));
        }

        void SyncStatus()
        {
            this.Invoke(new Action(() =>
            {
                if (MqttClient.IsConnected)
                {
                    toolStripStatusLabel_status.Text = "已连接";
                }
                else
                {
                    toolStripStatusLabel_status.Text = "未连接";
                }
            }));
        }

        void SetClipboardText(string text)
        {
            this.Invoke(new Action(() =>
            {
                Clipboard.SetText(text);
            }));
        }

        async void Paste()
        {
            await Task.Run(() =>
            {
                SendKeys.SendWait("^v");
                if (Settings.EnterAfterPaste)
                {
                    Thread.Sleep(300);
                    SendKeys.SendWait("{enter}");
                }
            });
        }


        public IMqttClient InitMqttClient()
        {
            try
            {
                var mqttFactory = new MqttFactory();
                IMqttClient mqttClient = mqttFactory.CreateMqttClient();
                mqttClient.ApplicationMessageReceivedAsync += HandleMessage;
                mqttClient.DisconnectedAsync += HandleMqttDisconnect;
                return mqttClient;
            } 
            catch (Exception ex)
            {
                Log("初始化Mqtt客户端时出错: " + ex.Message);
                return null;
            }
        }

        public async Task Connect()
        {
            try
            {
                Log("尝试连接到Mqtt服务器");
                LastConnectTime = DateTime.Now;
                var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(Settings.MqttAddress, Settings.MqttPort)
                                   .WithCredentials(Settings.MqttUsername, Settings.MqttPassword)
                                   .WithTlsOptions(o => o.WithCertificateValidationHandler(_ => true))
                                   .Build();

                await MqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                                .WithTopic(Settings.MqttTopic)
                                .Build());
                if (MqttClient.IsConnected)
                {
                    Log("MQTT客户端已连接");
                }
                SyncStatus();
            }
            catch (Exception ex)
            {
                Log("连接Mqtt时出错: " + ex.Message);
            }
        }

        private async Task ReconnectMqttClient()
        {
            if (!MqttClient.IsConnected)
            {
                await Connect();
            }
        }

        private async Task HandleMqttDisconnect(MqttClientDisconnectedEventArgs arg)
        {
            Thread.Sleep(100);
            Log("MQTT客户端已断开连接，5秒后进行重新连接");
            SyncStatus();
            Thread.Sleep(5000);
            await ReconnectMqttClient();
        }

        public Task HandleMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            var payload = e.ApplicationMessage.PayloadSegment.Array ?? Array.Empty<byte>();
            int offset = e.ApplicationMessage.PayloadSegment.Offset;
            int count = e.ApplicationMessage.PayloadSegment.Count;
            byte[] payloadBytes = new byte[count];
            Array.Copy(payload, offset, payloadBytes, 0, count);
            var payloadString = Encoding.UTF8.GetString(payloadBytes);
            Log("接收到MQTT消息:\n" + payloadString);
            HandleSMS(payloadString);
            return Task.CompletedTask;
        }

        public string ExtractVerifyCode(string content)
        {
            MatchCollection matches = Regex.Matches(content, @"\d{6}");
            if (matches.Count == 0)
            {
                matches = Regex.Matches(content, @"\d{4}");
            }
            if (matches.Count == 0)
            {
                return "";
            }

            return matches[0].Value;
        }

        public void HandleSMS(string sms)
        {
            bool isVerifyCode = sms.Contains("验证码") || sms.ToLower().Contains("code");
            if (isVerifyCode && !Settings.ShowAllSms)
            {
                Log("非验证码消息，不提示");
                return;
            }
            Log("验证码消息，发出提示");
            string[] lines = sms.Split(new string[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            string fromNumber = null, content = null;
            if (lines.Length > 1 && long.TryParse(lines[0], out long result)) 
            {
                fromNumber = lines[0];
                content = string.Join("\n", lines.Skip(1));
            }
            else
            {
                content = sms;
            }
            string notifyTitle = "";
            if (isVerifyCode)
            {
                string code = ExtractVerifyCode(content);
                notifyTitle = string.IsNullOrEmpty(code) ? "短信验证码" : "短信验证码 " + code;
                if (!string.IsNullOrEmpty(code) && Settings.EnableAutoCopy)
                {
                    SetClipboardText(code);
                    Log("检测到验证码 " + code + "，复制到剪切板");

                    if (Settings.EnableAutoPaste)
                    {
                        Paste();
                        Log("自动粘贴");
                    }
                }
                else
                {
                    Log("未启用自动复制");
                }
            }
            else
            {
                notifyTitle = "短信消息";

            }

            if (!string.IsNullOrEmpty(fromNumber))
            {
                notifyTitle += " 来自 " + fromNumber;
            }
            notifyIcon.ShowBalloonTip(5000, notifyTitle, content, ToolTipIcon.Info);
        }

        private void Mainform_Shown(object sender, EventArgs e)
        {
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void ToolStripMenuItem_exit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
