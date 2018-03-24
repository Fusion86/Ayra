namespace Ayra.Core.Structs
{
    public enum DownloadContentProgressStatus
    {
        Downloading,
        AlreadyDownloaded,
        Error,
    }

    public struct DownloadContentProgress
    {
        public int ContentIndex;
        public long BytesReceived;
        public long TotalBytesToReceive;
        public DownloadContentProgressStatus Status;
    }
}
