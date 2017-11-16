namespace ScanerProcessingService
{
    public enum StatusCodes
    {
        NotStarted,
        Started,
        WaitForNewFile,
        PrepareForSending,
        SendMessageToServer,
        ProcessDirectory
    }
}