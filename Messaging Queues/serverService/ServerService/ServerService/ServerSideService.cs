using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApplication1;
using CsvHelper;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using ZXing;

namespace ServerSideService
{
    public class ServerSideService
    {
        private const string InfoQueueName = "ServiceStatus";
        private const string SettingsTopicName = "SettingsTopic2";
        private readonly string _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
        private readonly string _statusFilePath;
        private readonly FileSystemWatcher _watcher;
        private CancellationToken _cancellationToken;
        private Result _separatingBarcode;

        public ServerSideService(string outDir)
        {
            var namespaceManager = NamespaceManager.Create();
            if (!namespaceManager.QueueExists(InfoQueueName))
            {
                namespaceManager.CreateQueue(InfoQueueName);
            }
            if (!namespaceManager.TopicExists(SettingsTopicName))
            {
                namespaceManager.CreateTopic(SettingsTopicName);
            }

            _statusFilePath = Path.Combine(outDir, "res.csv");
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            if (!File.Exists(_statusFilePath))
            {
                File.Create(_statusFilePath);
            }

            _watcher = new FileSystemWatcher(_settingsPath);
            _watcher.Changed += CheckSeparatingBarcode;
            LoadSeparatingBarcode();
        }

        private void SaveServiceStatus()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    QueueClient client = QueueClient.Create(InfoQueueName, ReceiveMode.ReceiveAndDelete);

                    var message = client.Receive();
                    if (message != null)
                    {
                        string data = message.GetBody<string>();
                        using (FileStream fs = new FileStream(_statusFilePath, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                sw.WriteLine(data);
                                sw.Close();
                            }
                            fs.Close();
                        }

                        client.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void UpdateSettings()
        {
            var client = TopicClient.Create(SettingsTopicName);
            SettingsMessage message = new SettingsMessage()
            {
                Format = _separatingBarcode.BarcodeFormat,
                RawBytes = _separatingBarcode.RawBytes,
                Text = _separatingBarcode.Text,
                ResultPoints = _separatingBarcode.ResultPoints
            };
            client.Send(new BrokeredMessage(message));
        }

        private void LoadSeparatingBarcode()
        {
            BarcodeReader reader = new BarcodeReader() { AutoRotate = true };
            using (Bitmap image = (Bitmap)Image.FromFile(Path.Combine(_settingsPath, "separatingBarcode.png")))
            {
                _separatingBarcode = reader.Decode(image);

            }
            UpdateSettings();
        }

        private void CheckSeparatingBarcode(object obj, FileSystemEventArgs e)
        {
            BarcodeReader reader = new BarcodeReader() { AutoRotate = true };
            using (Bitmap image = (Bitmap)Image.FromFile(e.FullPath))
            {
                var newBarcode = reader.Decode(image);
                if (newBarcode != null && _separatingBarcode.ToString() != newBarcode.ToString())
                {
                    _separatingBarcode = newBarcode;
                    UpdateSettings();
                }
            }
        }

        public void Start(CancellationToken ct)
        {
            _cancellationToken = ct;
            _watcher.EnableRaisingEvents = true;
            Task.Run(() =>
            {
                SaveServiceStatus();
            }, _cancellationToken);
        }

        public void Stop(CancellationTokenSource cts)
        {
            _watcher.EnableRaisingEvents = false;
            cts.Cancel();
        }
    }
}
