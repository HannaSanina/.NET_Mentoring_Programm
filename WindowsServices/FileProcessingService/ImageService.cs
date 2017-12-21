using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileProcessingService;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PostSharp.Patterns.Diagnostics;
using ZXing;

namespace ScanerProcessingService
{
    class ImageService : IImageService
    {
        private readonly FileSystemWatcher watcher;
        private string inDir;
        private string outDir;
        private ConcurrentQueue<string> filesList;
        private CancellationToken _cancellationToken;
        static readonly Logger logger = Logger.GetLogger();

        public ImageService(string inDir, string outDir)
        {
            this.inDir = inDir;
            this.outDir = outDir;
            logger.Write(LogLevel.Info, "{Time} ImageService was created with {Input}, {Output}", DateTime.Now, this.inDir, this.outDir);

            if (!Directory.Exists(inDir))
                Directory.CreateDirectory(inDir);

            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            filesList = new ConcurrentQueue<string>();
            watcher = new FileSystemWatcher(inDir);
            watcher.Created += NewFileCreated;
        }

        public void ProcessDirectory(IEnumerable<string> filesArray)
        {
            var activity = logger.OpenActivity("{Time} Processing directory ", DateTime.Now);
            _cancellationToken.ThrowIfCancellationRequested();
            var document = new Document();
            var section = document.AddSection();

            activity.Write(LogLevel.Info, "{Time} Working with a {Count} files.", DateTime.Now, filesArray.Count());
            foreach (var file in filesArray)
            {
                var img = section.AddImage(file);
                img.Height = document.DefaultPageSetup.PageHeight;
                img.Width = document.DefaultPageSetup.PageWidth;

                section.AddPageBreak();
            }

            var render = new PdfDocumentRenderer { Document = document };

            render.RenderDocument();
            activity.SetSuccess("{Time} Success. Rendered PDF doc.", DateTime.Now);
            _cancellationToken.ThrowIfCancellationRequested();
            var path = Path.Combine(outDir, $"result{Guid.NewGuid()}.pdf");
            render.Save(path);
        }

        private void NewFileCreated(object sender, FileSystemEventArgs e)
        {
            logger.Write(LogLevel.Info, "{Time} NewFileCreated was created", DateTime.Now);
            _cancellationToken.ThrowIfCancellationRequested();
            bool isBarcode = CheckBarcode(e.Name);

            if (isBarcode)
            {
                var tempList = new List<string>(filesList);
                ProcessDirectory(tempList);
                filesList = new ConcurrentQueue<string>();
            }
            else
            {
                filesList.Enqueue(e.Name);
            }
        }

        public bool CheckBarcode(string fileName)
        {
            logger.Write(LogLevel.Info, "{Time} CheckBarcode was invoked", DateTime.Now);
            var reader = new BarcodeReader() { AutoRotate = true };
            var bmp = (Bitmap)Image.FromFile(fileName);
            var result = reader.Decode(bmp);
            Console.WriteLine(fileName + " " + ((result == null) ? "No barcode" : result.Text));

            return result != null;
        }

        public void Start(CancellationToken ct, CancellationTokenSource cts)
        {
            Task.Run(() =>
                {
                    watcher.EnableRaisingEvents = true;
                    _cancellationToken = ct;
                    ProcessDirectory(Directory.EnumerateFiles(inDir));
                }, ct);
        }

        public void Stop(CancellationTokenSource cts)
        {
            watcher.EnableRaisingEvents = false;
            cts.Cancel();
        }
    }
}
