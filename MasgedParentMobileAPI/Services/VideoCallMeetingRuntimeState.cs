using System.Collections.Concurrent;



namespace MasgedParentMobileAPI.Services;



/// <summary>In-memory per-meeting state for active video calls (hub signaling).</summary>

public static class VideoCallMeetingRuntimeState

{

    private static readonly ConcurrentDictionary<int, MeetingState> States = new();



    public sealed class MeetingState

    {

        public ConcurrentDictionary<int, bool> MicAllowed { get; } = new();

        public ConcurrentDictionary<int, bool> CameraAllowed { get; } = new();

    }



    public static MeetingState GetOrCreate(int meetingId) =>

        States.GetOrAdd(meetingId, _ => new MeetingState());



    public static void Remove(int meetingId) => States.TryRemove(meetingId, out _);



    public static void SetMic(int meetingId, int studentId, bool allowed) =>

        GetOrCreate(meetingId).MicAllowed[studentId] = allowed;



    public static void SetCamera(int meetingId, int studentId, bool allowed) =>

        GetOrCreate(meetingId).CameraAllowed[studentId] = allowed;

}

