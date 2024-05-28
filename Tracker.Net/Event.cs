namespace Tracker.Data;

public enum Event
{
    None = 0,
    Completed = 1,
    Started = 2,
    Stopped = 3,
    Unknown = 4 //Sent by UTorrent when the full file exists for some reason. Out of spec.
}