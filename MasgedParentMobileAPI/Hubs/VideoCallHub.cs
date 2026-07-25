using System.Security.Claims;

using MasgedParentMobileAPI.Services;

using MasgedTeacherMobileAPI.Data;

using MasgedTeacherMobileAPI.Enums;

using MasgedTeacherMobileAPI.Helpers;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.SignalR;

using Microsoft.EntityFrameworkCore;



namespace MasgedParentMobileAPI.Hubs;



/// <summary>Teacher ↔ parent realtime signaling during Agora calls (mic permission, raise hand, end call).</summary>

[Authorize(AuthenticationSchemes = "Bearer,TeacherJwt")]

public sealed class VideoCallHub(AppDbContext db, AgoraTokenService agoraTokens, IVideoCallTerminationService termination) : Hub

{

    private static string GroupName(int meetingId) => VideoCallHubGroups.Meeting(meetingId);



    public async Task JoinCall(int meetingId)

    {

        var meeting = await db.MeetingsInfo.AsNoTracking()

            .FirstOrDefaultAsync(m => m.Id == meetingId, Context.ConnectionAborted);



        if (meeting is null)

            throw new HubException("المكالمة غير موجودة.");



        if (meeting.Status == MeetingStatus.Ended)

            throw new HubException("انتهت هذه المكالمة.");



        int? parentStudentId = null;



        if (IsTeacher(Context.User))

        {

            var teacherId = ResolveTeacherId(Context.User);

            if (teacherId is null || teacherId != meeting.TeacherId)

                throw new HubException("غير مصرح بالانضمام لهذه المكالمة كمعلم.");

        }

        else

        {

            var fp = Context.User!.FindFirstValue("fatherPhone");

            if (string.IsNullOrEmpty(fp))

                throw new HubException("تعذر التحقق من ولي الأمر.");



            parentStudentId = await ResolveParentStudentIdForMeetingAsync(

                Services.PhoneNormalizer.ToCanonical(fp),

                meeting,

                Context.ConnectionAborted);

        }



        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(meetingId));



        var state = VideoCallMeetingRuntimeState.GetOrCreate(meetingId);

        if (parentStudentId is > 0)

        {

            var mic = state.MicAllowed.GetValueOrDefault(parentStudentId.Value, false);
            var cam = state.CameraAllowed.GetValueOrDefault(parentStudentId.Value, true);
            await Clients.Caller.SendAsync(
                "CallStateSynced",
                mic,
                cam,
                cancellationToken: Context.ConnectionAborted);

        }

        else if (IsTeacher(Context.User))

        {

            await Clients.Caller.SendAsync(

                "CallStateSynced",

                false,

                true,

                cancellationToken: Context.ConnectionAborted);

        }

    }



    /// <summary>Teacher ends the call for everyone and persists meeting lifecycle.</summary>

    public async Task EndCall(int meetingId, string? teacherNotes)

    {

        var teacherId = ResolveTeacherId(Context.User);

        if (teacherId is null || !IsTeacher(Context.User))

            throw new HubException("فقط المعلم يمكنه إنهاء المكالمة.");



        var terminated = await termination.TerminateMeetingAsync(

            meetingId,

            teacherId.Value,

            teacherNotes,

            Context.ConnectionAborted);



        if (!terminated)

            throw new HubException("المكالمة غير موجودة.");

    }



    /// <summary>Teacher saves draft notes during an active call.</summary>

    public async Task SaveMeetingNotes(int meetingId, string? notes)

    {

        if (!IsTeacher(Context.User))

            throw new HubException("فقط المعلم يمكنه حفظ الملاحظات.");



        var teacherId = ResolveTeacherId(Context.User);

        if (teacherId is null)

            throw new HubException("غير مصرح.");



        var meeting = await db.MeetingsInfo

            .FirstOrDefaultAsync(m => m.Id == meetingId, Context.ConnectionAborted);



        if (meeting is null)

            throw new HubException("المكالمة غير موجودة.");



        if (meeting.TeacherId != teacherId)

            throw new HubException("غير مصرح.");



        if (meeting.Status == MeetingStatus.Ended)

            throw new HubException("لا يمكن تعديل ملاحظات مكالمة منتهية.");



        meeting.TeacherNotes = notes?.Trim();

        await db.SaveChangesAsync(Context.ConnectionAborted);

    }



    /// <summary>Parent raises hand to ask a question.</summary>

    public async Task RaiseHand(int meetingId, int studentId)

    {

        await SetHandRaisedAsync(meetingId, studentId, raised: true);

    }



    /// <summary>Parent lowers hand.</summary>

    public async Task LowerHand(int meetingId, int studentId)

    {

        await SetHandRaisedAsync(meetingId, studentId, raised: false);

    }



    /// <summary>Teacher allows or denies parent audio for a student in this meeting.</summary>

    public async Task SetMicAllowed(int meetingId, int studentId, bool allowed)

    {

        var meeting = await db.MeetingsInfo.AsNoTracking()

            .FirstOrDefaultAsync(m => m.Id == meetingId, Context.ConnectionAborted);



        if (meeting is null)

            throw new HubException("المكالمة غير موجودة.");



        if (meeting.Status == MeetingStatus.Ended)

            throw new HubException("انتهت هذه المكالمة.");



        if (!IsTeacher(Context.User))

            throw new HubException("فقط المعلم يمكنه التحكم بسماعة ولي الأمر.");



        var teacherId = ResolveTeacherId(Context.User);

        if (teacherId is null || teacherId != meeting.TeacherId)

            throw new HubException("غير مصرح.");



        VideoCallMeetingRuntimeState.SetMic(meetingId, studentId, allowed);



        await Clients.Group(GroupName(meetingId))

            .SendAsync("MicPermissionChanged", studentId, allowed, cancellationToken: Context.ConnectionAborted);

    }



    /// <summary>Teacher allows or denies parent camera for a student in this meeting.</summary>

    public async Task SetCameraAllowed(int meetingId, int studentId, bool allowed)

    {

        var meeting = await db.MeetingsInfo.AsNoTracking()

            .FirstOrDefaultAsync(m => m.Id == meetingId, Context.ConnectionAborted);



        if (meeting is null)

            throw new HubException("المكالمة غير موجودة.");



        if (meeting.Status == MeetingStatus.Ended)

            throw new HubException("انتهت هذه المكالمة.");



        if (!IsTeacher(Context.User))

            throw new HubException("فقط المعلم يمكنه التحكم بكاميرا ولي الأمر.");



        var teacherId = ResolveTeacherId(Context.User);

        if (teacherId is null || teacherId != meeting.TeacherId)

            throw new HubException("غير مصرح.");



        VideoCallMeetingRuntimeState.SetCamera(meetingId, studentId, allowed);



        await Clients.Group(GroupName(meetingId))

            .SendAsync("CameraPermissionChanged", studentId, allowed, cancellationToken: Context.ConnectionAborted);

    }



    private async Task SetHandRaisedAsync(int meetingId, int studentId, bool raised)

    {

        if (IsTeacher(Context.User))

            throw new HubException("رفع اليد متاح لولي الأمر فقط.");



        var meeting = await db.MeetingsInfo.AsNoTracking()

            .FirstOrDefaultAsync(m => m.Id == meetingId, Context.ConnectionAborted);



        if (meeting is null)

            throw new HubException("المكالمة غير موجودة.");



        if (meeting.Status == MeetingStatus.Ended)

            throw new HubException("انتهت هذه المكالمة.");



        var fp = Context.User!.FindFirstValue("fatherPhone");

        if (string.IsNullOrEmpty(fp))

            throw new HubException("تعذر التحقق من ولي الأمر.");



        await EnsureParentAuthorizedForMeetingAsync(

            Services.PhoneNormalizer.ToCanonical(fp),

            meeting,

            Context.ConnectionAborted,

            studentId);



        await Clients.Group(GroupName(meetingId))

            .SendAsync("HandRaised", studentId, raised, cancellationToken: Context.ConnectionAborted);

    }



    private static bool IsTeacher(ClaimsPrincipal? user) =>

        user?.FindFirstValue("fatherPhone") is null or "" &&

        user?.FindFirstValue("circleId") is { Length: > 0 };



    private static int? ResolveTeacherId(ClaimsPrincipal? user)

    {

        var idClaim = user?.FindFirstValue("id") ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(idClaim, out var id) ? id : null;

    }



    private async Task<int> ResolveParentStudentIdForMeetingAsync(

        string canonicalFatherPhone,

        MasgedTeacherMobileAPI.Entities.MeetingInfo meeting,

        CancellationToken cancellationToken)

    {

        if (string.IsNullOrWhiteSpace(meeting.StudentIds))

            throw new HubException("لا يوجد طلاب مدعوون لهذه المكالمة.");



        var invitedIds = meeting.StudentIds

            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)

            .Select(s => int.TryParse(s, out var sid) ? sid : (int?)null)

            .Where(s => s.HasValue)

            .Select(s => s!.Value)

            .ToHashSet();



        var variants = Services.PhoneNormalizer.GetVariants(canonicalFatherPhone).ToList();



        var parentStudentIds = await db.RegisterForms.AsNoTracking()

            .Where(r => invitedIds.Contains(r.Id) &&

                        ((r.FatherPhone != null && variants.Contains(r.FatherPhone))

                         || (r.FatherPhone2 != null && variants.Contains(r.FatherPhone2))))

            .Select(r => r.Id)

            .ToListAsync(cancellationToken);



        if (parentStudentIds.Count == 0)

            throw new HubException("غير مصرح بالانضمام لهذه المكالمة.");



        return parentStudentIds[0];

    }



    private async Task EnsureParentAuthorizedForMeetingAsync(

        string canonicalFatherPhone,

        MasgedTeacherMobileAPI.Entities.MeetingInfo meeting,

        CancellationToken cancellationToken,

        int? requiredStudentId = null)

    {

        if (string.IsNullOrWhiteSpace(meeting.StudentIds))

            throw new HubException("لا يوجد طلاب مدعوون لهذه المكالمة.");



        var invitedIds = meeting.StudentIds

            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)

            .Select(s => int.TryParse(s, out var sid) ? sid : (int?)null)

            .Where(s => s.HasValue)

            .Select(s => s!.Value)

            .ToHashSet();



        if (requiredStudentId is > 0 && !invitedIds.Contains(requiredStudentId.Value))

            throw new HubException("الطالب غير مدعو لهذه المكالمة.");



        var variants = Services.PhoneNormalizer.GetVariants(canonicalFatherPhone).ToList();



        var parentStudentIds = await db.RegisterForms.AsNoTracking()

            .Where(r => invitedIds.Contains(r.Id) &&

                        ((r.FatherPhone != null && variants.Contains(r.FatherPhone))

                         || (r.FatherPhone2 != null && variants.Contains(r.FatherPhone2))))

            .Select(r => r.Id)

            .ToListAsync(cancellationToken);



        if (parentStudentIds.Count == 0)

            throw new HubException("غير مصرح بالانضمام لهذه المكالمة.");



        if (requiredStudentId is > 0 && !parentStudentIds.Contains(requiredStudentId.Value))

            throw new HubException("غير مصرح لهذا الطالب.");

    }

}


