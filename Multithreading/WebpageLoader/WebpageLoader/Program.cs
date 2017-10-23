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
    public const int MAX_QUEUE_LENGTH = 100;
    public const int MAX_CONCURRENT_TASK = 5;

    static void Main()
    {
        BlockingCollection<string> pagesQueue = new BlockingCollection<string>(MAX_QUEUE_LENGTH);

        Console.WriteLine("Enter key word");
        string keyword = Console.ReadLine();
        CancellationTokenSource cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        Console.WriteLine("Enter url or print 'exit' to finish");
        string entredString = Console.ReadLine();

        for (int i = 0; i < MAX_CONCURRENT_TASK; i++)
        {
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    StartSearch(pagesQueue, keyword, cancellationToken);
                }
                Console.WriteLine("Search stopped");

            }, cancellationToken);
        }

        while (entredString != "exit")
        {
            bool isValidLink = Uri.IsWellFormedUriString(entredString, UriKind.Absolute);

            if (isValidLink && pagesQueue.All(item => item != entredString))
            {
                pagesQueue.Add(entredString, cancellationToken);
            }
            Console.WriteLine("Enter url or print 'exit' to finish");
            entredString = Console.ReadLine();
        }
        cts.Cancel();

        Console.ReadLine();
    }

    static async void StartSearch(BlockingCollection<string> pagesQueue, string keyword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string currentUrl = pagesQueue.Take(); ;
        string result = null;

        try
        {
            result = await DownloadWebPage(currentUrl, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                if (result != null)
                {
                    AddInnerLinks(result, pagesQueue);
                    CustomConsole.PrintSearchResults(currentUrl, FindKeyword(result, keyword), cancellationToken);
                }
                else
                {
                    CustomConsole.WriteLine($"Page loading failed: {currentUrl}", cancellationToken);
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine("cancel print");
        }
        catch (Exception ex)
        {
            CustomConsole.WriteLine($"{ex.Message}", cancellationToken);
        }

    }

    static async Task<string> DownloadWebPage(string url, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string result = null;
        try
        {
            result = await new WebClient().DownloadStringTaskAsync(url);
        }
        catch (OperationCanceledException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            string exeption = ex.InnerException?.Message ?? ex.Message;
            CustomConsole.WriteLine($"an error occured: {exeption}", cancellationToken);
        }
        return result;
    }

    //TODO: add smart html parsing without markup tags
    static bool FindKeyword(string data, string keyword)
    {
        return data.Contains(keyword);
    }

    static void AddInnerLinks(string html, BlockingCollection<string> queue)
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
                    queue.Add(url);
                };
            }
        }
    }

    public class CustomConsole
    {
        static ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        static ConcurrentQueue<ResultItem> _reportQueue = new ConcurrentQueue<ResultItem>();
        private const int MAX_BUFFER_LENGTH = 20;
        private const int MAX_SEARCH_LENGTH = 20;
        public static void WriteLine(string output, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logQueue.Enqueue(output);
                if (_logQueue.Count > MAX_BUFFER_LENGTH)
                {
                    Print(_logQueue.ToList());
                    _logQueue = new ConcurrentQueue<string>();
                }
            }
        }

        public static void PrintSearchResults(string output, bool isFound, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _reportQueue.Enqueue(new ResultItem(output, isFound));
                if (_reportQueue.Count > MAX_SEARCH_LENGTH)
                {
                    List<string> trueArray = _reportQueue.Where(item => item.IsKeywordFound).Select(item => item.Message).ToList();
                    List<string> falseArray = _reportQueue.Where(item => !item.IsKeywordFound).Select(item => item.Message).ToList();

                    Console.WriteLine("Word found in pages:");
                    Print(trueArray);
                    Console.WriteLine("Word not found in pages:");
                    Print(falseArray);

                    _reportQueue = new ConcurrentQueue<ResultItem>();
                }
            }

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
