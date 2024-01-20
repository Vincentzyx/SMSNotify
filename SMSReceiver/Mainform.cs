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



        private async void Mainform_Load(object sender, EventArgs e)
        {
            this.Hide();
            LoadSettings();
            MqttClient = InitMqttClient();
            await Connect();
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
                if (!MqttClient.IsConnected)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }

        private async Task HandleMqttDisconnect(MqttClientDisconnectedEventArgs arg)
        {
            Log("MQTT客户端已断开连接。正在尝试重连...");
            SyncStatus();
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
            if (!Settings.ShowAllSms && !sms.Contains("验证码") && !sms.ToLower().Contains("code"))
            {
                Log("非验证码消息，不提示");
                return;
            }
            Log("验证码消息，发出提示");
            string[] lines = sms.Split(new string[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            string fromNumber = lines[0];
            string content = string.Join("\n", lines.Skip(1));
            string code = ExtractVerifyCode(content);
            notifyIcon.ShowBalloonTip(5000, "验证码 " + code + " 来自 " + fromNumber, content, ToolTipIcon.Info);
            if (code != "" && Settings.EnableAutoCopy)
            {
                SetClipboardText(code);
                Log("检测到验证码 " + code + "，复制到剪切板");
            }
            else
            {
                Log("未启用自动复制");
            }
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
