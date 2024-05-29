namespace Tracker.Filesys.Bencode;

public class BufferTooSmallException : Exception
{
    public BufferTooSmallException()
        : base("Buffer not long enough.")
    {
    }
}

public class UnexpectEndException : Exception
{
    public UnexpectEndException()
        : base("Unexpect Data End.")
    {
    }
}