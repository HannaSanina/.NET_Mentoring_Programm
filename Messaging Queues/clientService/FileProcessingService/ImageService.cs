using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using ZXing;

namespace ScanerProcessingService
{
    public class ImageService
    {
        private readonly string _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings//separatingBarcode.png");
        private readonly FileSystemWatcher _watcher;
        private readonly Timer _timer;
        private readonly string _inDir;
        private readonly string _outDir;

        private StatusCodes workStatus = StatusCodes.NotStarted;
        private ConcurrentQueue<string> _filesList;
        private CancellationToken _cancellationToken;
        private readonly MessagingClient _messagingClient;
        private Result _separatingBarcode;

        public ImageService(string inDir, string outDir)
        {
            _inDir = inDir;
            _outDir = outDir;
            workStatus = StatusCodes.Started;
            _messagingClient = new MessagingClient();

            if (!Directory.Exists(inDir))
                Directory.CreateDirectory(inDir);

            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            _filesList = new ConcurrentQueue<string>();
            _watcher = new FileSystemWatcher(inDir);
            _watcher.Created += NewFileCreated;

            LoadSeparatingBarcode();
            _timer = new Timer(SendServiceInfo);
        }

        public void ProcessDirectory(IEnumerable<string> files)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            var document = new Document();
            var section = document.AddSection();
            workStatus = StatusCodes.ProcessDirectory;
            var array = files.ToArray();
            
            for (var i = 0; i < array.Count(); i++)
            {
                if (CheckBarcode(array[i]))
                {
                    SaveDocument(document);
                    workStatus = StatusCodes.PrepareForSending;
                    document = new Document();
                    section = document.AddSection();
                }
                else
                {
                    var img = section.AddImage(array[i]);
                    img.Height = document.DefaultPageSetup.PageHeight;
                    img.Width = document.DefaultPageSetup.PageWidth;
                }
            }
            SaveDocument(document);
            workStatus = StatusCodes.PrepareForSending;
        }

        private void NewFileCreated(object sender, FileSystemEventArgs e)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            bool isBarcode = CheckBarcode(e.FullPath);
            if (isBarcode || _filesList.Count > 5)
            {
                var tempList = new List<string>(_filesList);
                ProcessDirectory(tempList);
                _filesList = new ConcurrentQueue<string>();
            }
            else
            {
                _filesList.Enqueue(e.Name);
            }
        }

        private void SaveDocument(Document document)
        {
            byte[] message;
            var render = new PdfDocumentRenderer { Document = document };
            var path = Path.Combine(_outDir, $"result{Guid.NewGuid()}.pdf");
            render.RenderDocument();
            if (render.PageCount > 0)
            {
                render.Save(path);

                using (MemoryStream stream = new MemoryStream())
                {
                    render.Save(stream, false);
                    message = stream.ToArray();
                }

                _messagingClient.SendMessage(message);
                workStatus = StatusCodes.SendMessageToServer;
            }
        }

        private void SendServiceInfo(object obj)
        {
            _messagingClient.SendStatus(workStatus.ToString());
        }

        private bool CheckBarcode(string fileName)
        {
            bool isSeparatingBarcode = false;
            Result result;
            BarcodeReader reader = new BarcodeReader() { AutoRotate = true };
            using (Bitmap image = (Bitmap)Image.FromFile(fileName))
            {
                result = reader.Decode(image);
                image.Dispose();
            }
            if (result != null)
            {
                isSeparatingBarcode = result.ToString() == _separatingBarcode.ToString();
            }

            return isSeparatingBarcode;
        }

        private void LoadSeparatingBarcode()
        {
            BarcodeReader reader = new BarcodeReader() { AutoRotate = true };
            using (Bitmap image = (Bitmap)Image.FromFile(_settingsPath))
            {
                _separatingBarcode = reader.Decode(image);
            }
        }

        public void ReadScannerSettings()
        {
            _cancellationToken.ThrowIfCancellationRequested();
            while (!_cancellationToken.IsCancellationRequested)
            {
                Result setting = _messagingClient.ReadSettings();
                if (setting != null)
                {
                    _separatingBarcode = setting;

                    using (FileStream fs = new FileStream(_settingsPath, FileMode.CreateNew, FileAccess.Write))
                    {
                        fs.Write(setting.RawBytes, 0, setting.RawBytes.Length);
                        fs.Close();
                    }
                }
            }
        }

        public void Start(CancellationToken ct, CancellationTokenSource cts)
        {
            _timer.Change(0, 5 * 1000);
            Task.Run(() =>
                {
                    _watcher.EnableRaisingEvents = true;
                    _cancellationToken = ct;
                    ProcessDirectory(Directory.EnumerateFiles(_inDir));
                }, ct).ContinueWith(task =>
                {
                    Console.WriteLine("Unhandled Exception");
                    ProcessDirectory(_filesList);
                }, TaskContinuationOptions.OnlyOnFaulted)
                .ContinueWith(task =>
                {
                    ProcessDirectory(_filesList);
                }, TaskContinuationOptions.OnlyOnCanceled);

            Task.Run(() =>
                {
                    ReadScannerSettings();
                }, ct);
        }

        public void Stop(CancellationTokenSource cts)
        {
            _timer.Change(Timeout.Infinite, 0);
            _watcher.EnableRaisingEvents = false;
            cts.Cancel();
        }
    }
}
