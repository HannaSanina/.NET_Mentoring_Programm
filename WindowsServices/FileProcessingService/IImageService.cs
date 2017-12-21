using System.Collections.Generic;
using System.Threading;

namespace FileProcessingService
{
    public interface IImageService
    {
        void ProcessDirectory(IEnumerable<string> filesArray);
        bool CheckBarcode(string fileName);
        void Start(CancellationToken ct, CancellationTokenSource cts);
        void Stop(CancellationTokenSource cts);
    }
}