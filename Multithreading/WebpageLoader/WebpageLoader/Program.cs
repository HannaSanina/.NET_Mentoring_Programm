using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

class Program
{
    public const int MAX_QUEUE_LENGTH = 300;
    public const int MAX_CONCURRENT_TASK = 7;

    static void Main()
    {
        ConcurrentQueue<string> pagesQueue = new ConcurrentQueue<string>();

        Console.WriteLine("Enter key word");
        string keyword = Console.ReadLine();
        CancellationTokenSource cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(MAX_CONCURRENT_TASK))
        {
            Console.WriteLine("Enter url or print 'exit' to finish");

            string entredString = Console.ReadLine();

            while (entredString != "exit")
            {
                bool isValidLink = Uri.IsWellFormedUriString(entredString, UriKind.Absolute);
                if (isValidLink)
                {
                    pagesQueue.Enqueue(entredString);
                    Task.Factory.StartNew(() =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        StartSearch(pagesQueue, keyword, concurrencySemaphore, cancellationToken);
                    }, cancellationToken);
                }
                else
                {
                    Console.WriteLine("Wrong url. Try again");
                }
                Console.WriteLine("Enter url or print 'exit' to finish");
                entredString = Console.ReadLine();
            }
            cts.Cancel();
            Console.WriteLine("Search stopped");
            Console.ReadLine();
        }
    }

    static void StartSearch(ConcurrentQueue<string> pagesQueue, string keyword, SemaphoreSlim concurrencySemaphore, CancellationToken cancellationToken)
    {
        concurrencySemaphore.Wait();
        string currentUrl = null;

        if (pagesQueue.Count > 0)
        {
            Task.Factory.StartNew(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                string result = null;

                if (pagesQueue.TryPeek(out currentUrl) && currentUrl != null)
                {
                    pagesQueue.TryDequeue(out currentUrl);
                    result = DownloadWebPage(currentUrl).Result;
                }
                return result;
            }, cancellationToken).ContinueWith(task =>
            {
                AddInnerLinks(task.Result, pagesQueue);
                CompletedDownloadPage(task, currentUrl, keyword);

                concurrencySemaphore.Release();
            }, TaskContinuationOptions.NotOnCanceled)
            .ContinueWith(task =>
            {
                Console.WriteLine("Request was canceled");
            }, TaskContinuationOptions.OnlyOnCanceled);
        }
        else
        {
            CustomConsole.WriteLine("Page queue is empty.");
        }

        StartSearch(pagesQueue, keyword, concurrencySemaphore, cancellationToken);
    }

    static async Task<string> DownloadWebPage(string url)
    {
        string result = null;
        try
        {
            result = await new WebClient().DownloadStringTaskAsync(url);
        }
        catch (Exception ex)
        {
            string exeption = ex.InnerException?.Message ?? ex.Message;
            CustomConsole.WriteLine($"an error occured: {exeption}");
            throw ex;
        }
        return result;
    }

    //TODO: add smart html parsing without markup tags
    static bool FindKeyword(string data, string keyword)
    {
        return data.Contains(keyword);
    }

    static void CompletedDownloadPage(Task<string> task, string url, string keyword)
    {
        switch (task.Status)
        {
            case TaskStatus.RanToCompletion:
                if (task.Result != null)
                {
                    CustomConsole.PrintSearchResults(url, FindKeyword(task.Result, keyword));
                }
                break;
            case TaskStatus.Faulted:
                CustomConsole.WriteLine($"Page loading failed: wrong url {url}");
                break;
        }
    }

    static void AddInnerLinks(string html, ConcurrentQueue<string> queue)
    {
        if (queue.Count < MAX_QUEUE_LENGTH && html != null)
        {
            var document = new HtmlParser().Parse(html);
            var innerURLList = document.QuerySelectorAll("a").Select(element => element.GetAttribute("href")).ToList();

            foreach (string url in innerURLList)
            {
                bool isValidLink = Uri.IsWellFormedUriString(url, UriKind.Absolute);
                if (isValidLink && queue.All(item => item != url))
                {
                    queue.Enqueue(url);
                };
            }
        }
    }

    public class CustomConsole
    {
        static ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        static ConcurrentQueue<ResultItem> _reportQueue = new ConcurrentQueue<ResultItem>();
        const int MAX_BUFFER_LENGTH = 10;
        public static void WriteLine(string output)
        {
            _logQueue.Enqueue(output);
            if (_logQueue.Count > MAX_BUFFER_LENGTH)
            {
                Print(_logQueue.ToList());
                _logQueue = new ConcurrentQueue<string>();
            }
        }

        public static void PrintSearchResults(string output, bool isFound)
        {
            _reportQueue.Enqueue(new ResultItem(output, isFound));
            if (_reportQueue.Count > MAX_BUFFER_LENGTH)
            {
                List<string> trueArray = (from item in _reportQueue
                                          where item.IsKeywordFound
                                          select item.Message).ToList();
                List<string> falseArray = (from item in _reportQueue
                                           where !item.IsKeywordFound
                                           select item.Message).ToList();
                Console.WriteLine("Word found in pages:");
                Print(trueArray);
                Console.WriteLine("Word not found in pages:");
                Print(falseArray);

                _reportQueue = new ConcurrentQueue<ResultItem>();
            }
        }

        private static void Print(IEnumerable<string> resultArray)
        {
            if (!resultArray.Any())
            {
                Console.WriteLine("no pages");
            }
            else
            {
                foreach (string message in resultArray)
                {
                    Console.WriteLine(message);
                }
            }

        }
    }

    public class ResultItem
    {
        public ResultItem(string message, bool isFound)
        {
            Message = message;
            IsKeywordFound = isFound;
        }
        public string Message { get; set; }
        public bool IsKeywordFound { get; set; }
    }
}