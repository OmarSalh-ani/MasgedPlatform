using MasgedTeacherMobileAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<QuranCircle> QuranCircles => Set<QuranCircle>();
    public DbSet<RegisterForm> RegisterForms => Set<RegisterForm>();
    public DbSet<CircleAttendance> CircleAttendances => Set<CircleAttendance>();
    public DbSet<ParentFollowup> ParentFollowups => Set<ParentFollowup>();
    public DbSet<TeacherNote> TeacherNotes => Set<TeacherNote>();
    public DbSet<TeachersAdminNote> TeachersAdminNotes => Set<TeachersAdminNote>();
    public DbSet<ParentNote> ParentNotes => Set<ParentNote>();
    public DbSet<PlanLevel> PlanLevels => Set<PlanLevel>();
    public DbSet<TeacherAttendance> TeacherAttendances => Set<TeacherAttendance>();
    public DbSet<TeacherMapLocation> TeacherMapLocations => Set<TeacherMapLocation>();
    public DbSet<WhatsappPreConfiguredMessage> WhatsappPreConfiguredMessages => Set<WhatsappPreConfiguredMessage>();
    public DbSet<WhatsappTempTable> WhatsappTempTables => Set<WhatsappTempTable>();
    public DbSet<MeetingInfo> MeetingsInfo => Set<MeetingInfo>();
    public DbSet<QuranSurah> QuranSurahs => Set<QuranSurah>();
    public DbSet<StudentPlanMemorizing> StudentPlanMemorizings => Set<StudentPlanMemorizing>();
    public DbSet<StudentPlanRevise> StudentPlanRevises => Set<StudentPlanRevise>();
    public DbSet<StudentPlanItemLog> StudentPlanItemLogs => Set<StudentPlanItemLog>();
    public DbSet<HolyQuran> HolyQurans => Set<HolyQuran>();
    public DbSet<StudentPlan> StudentPlans => Set<StudentPlan>();
    public DbSet<CircleDay> CircleDays => Set<CircleDay>();
    public DbSet<QuranAyah> QuranAyahs => Set<QuranAyah>();
    public DbSet<ReadyPlan> ReadyPlans => Set<ReadyPlan>();
    public DbSet<StudentMemorizingCard> StudentMemorizingCards => Set<StudentMemorizingCard>();
    public DbSet<TestHead> TestHeads => Set<TestHead>();
    public DbSet<TestBody> TestBodies => Set<TestBody>();
    public DbSet<StudentCircleEnrollment> StudentCircleEnrollments => Set<StudentCircleEnrollment>();
    public DbSet<HeroSlide> HeroSlides => Set<HeroSlide>();
    public DbSet<NewsItem> NewsItems => Set<NewsItem>();
    public DbSet<ParentTeacherChatMessage> ParentTeacherChatMessages => Set<ParentTeacherChatMessage>();
    public DbSet<MasgedParentMobileAPI.Models.ApiRequestLog> ApiRequestLogs => Set<MasgedParentMobileAPI.Models.ApiRequestLog>();
    public DbSet<MasgedParentMobileAPI.Models.ParentDeviceToken> ParentDeviceTokens => Set<MasgedParentMobileAPI.Models.ParentDeviceToken>();
    public DbSet<MasgedParentMobileAPI.Models.TeacherDeviceToken> TeacherDeviceTokens => Set<MasgedParentMobileAPI.Models.TeacherDeviceToken>();
    public DbSet<MasgedParentMobileAPI.Models.PushDeliveryLog> PushDeliveryLogs => Set<MasgedParentMobileAPI.Models.PushDeliveryLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>()
            .HasMany(t => t.QuranCircles)
            .WithOne(c => c.Teacher)
            .HasForeignKey(c => c.TeacherId)
            .IsRequired(false);

        modelBuilder.Entity<RegisterForm>()
            .HasOne(r => r.QuranCircle)
            .WithMany()
            .HasForeignKey(r => r.QuranCircleId)
            .IsRequired(false);

        modelBuilder.Entity<RegisterForm>()
            .HasOne(r => r.ParentFollowup)
            .WithOne(p => p.RegisterForm)
            .HasForeignKey<ParentFollowup>(p => p.StudentId);

        modelBuilder.Entity<ParentNote>()
            .HasOne(n => n.RegisterForm)
            .WithMany()
            .HasForeignKey(n => n.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RegisterForm>()
            .HasOne(r => r.PlanLevel)
            .WithMany()
            .HasForeignKey(r => r.PlanLevelId)
            .IsRequired(false);

        modelBuilder.Entity<RegisterForm>()
            .HasMany(r => r.CircleAttendances)
            .WithOne(a => a.RegisterForm)
            .HasForeignKey(a => a.StudentId);

        modelBuilder.Entity<StudentPlanMemorizing>()
            .HasOne(m => m.RegisterForm)
            .WithMany()
            .HasForeignKey(m => m.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentPlanMemorizing>()
            .HasOne(m => m.QuranSurah)
            .WithMany()
            .HasForeignKey(m => m.SurahId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentPlanRevise>()
            .HasOne(r => r.RegisterForm)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentPlanRevise>()
            .HasOne(r => r.QuranSurah)
            .WithMany()
            .HasForeignKey(r => r.SurahId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentMemorizingCard>()
            .HasOne(c => c.RegisterForm)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentPlanItemLog>()
            .HasOne(l => l.Teacher)
            .WithMany()
            .HasForeignKey(l => l.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestHead>()
            .HasOne(t => t.RegisterForm)
            .WithMany()
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestHead>()
            .HasOne(t => t.QuranCircle)
            .WithMany()
            .HasForeignKey(t => t.CircleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestHead>()
            .HasOne(t => t.Teacher)
            .WithMany()
            .HasForeignKey(t => t.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestBody>()
            .HasOne(b => b.TestHead)
            .WithMany()
            .HasForeignKey(b => b.TestHeadId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentCircleEnrollment>(entity =>
        {
            entity.ToTable("StudentCircleEnrollment");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartDate).HasColumnType("datetime2");
            entity.Property(e => e.EndDate).HasColumnType("datetime2");
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.CircleId);
            entity.HasIndex(e => new { e.StudentId, e.EndDate });

            entity.HasOne(e => e.RegisterForm)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.QuranCircle)
                .WithMany()
                .HasForeignKey(e => e.CircleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedByTeacher)
                .WithMany()
                .HasForeignKey(e => e.AssignedByTeacherId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        });

        modelBuilder.Entity<ParentTeacherChatMessage>()
            .HasOne(m => m.Teacher)
            .WithMany()
            .HasForeignKey(m => m.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ParentTeacherChatMessage>()
            .HasOne(m => m.RegisterForm)
            .WithMany()
            .HasForeignKey(m => m.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MasgedParentMobileAPI.Models.ApiRequestLog>(e =>
        {
            e.ToTable("ApiRequestLogs");
            e.HasKey(x => x.Id);
            e.Property(x => x.RequestedAt).HasColumnType("datetime2");

            e.Property(x => x.Method).HasMaxLength(10).IsRequired();
            e.Property(x => x.Path).HasMaxLength(500).IsRequired();
            e.Property(x => x.QueryString).HasMaxLength(2000);
            e.Property(x => x.RequestHeaders).HasColumnType("nvarchar(max)");
            e.Property(x => x.RequestBody).HasColumnType("nvarchar(max)");
            e.Property(x => x.ResponseBody).HasColumnType("nvarchar(max)");
            e.Property(x => x.ClientIp).HasMaxLength(64);
            e.Property(x => x.UserId).HasMaxLength(100);
            e.Property(x => x.UserName).HasMaxLength(200);
        });

        modelBuilder.Entity<MasgedParentMobileAPI.Models.ParentDeviceToken>(entity =>
        {
            entity.ToTable("ParentDeviceTokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ParentPhone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FcmToken).HasMaxLength(512).IsRequired();
            entity.Property(e => e.Platform).HasMaxLength(20).IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            entity.HasIndex(e => e.FcmToken).IsUnique();
            entity.HasIndex(e => e.ParentPhone);
        });

        modelBuilder.Entity<MasgedParentMobileAPI.Models.TeacherDeviceToken>(entity =>
        {
            entity.ToTable("TeacherDeviceTokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FcmToken).HasMaxLength(512).IsRequired();
            entity.Property(e => e.Platform).HasMaxLength(20).IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            entity.HasIndex(e => e.FcmToken).IsUnique();
            entity.HasIndex(e => e.TeacherId);
        });

        modelBuilder.Entity<MasgedParentMobileAPI.Models.PushDeliveryLog>(entity =>
        {
            entity.ToTable("PushDeliveryLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2");
            entity.Property(e => e.Source).HasMaxLength(40).IsRequired();
            entity.Property(e => e.Context).HasMaxLength(200).IsRequired();
            entity.Property(e => e.AudienceKind).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Platform).HasMaxLength(20).IsRequired();
            entity.Property(e => e.OwnerKey).HasMaxLength(64);
            entity.Property(e => e.FcmToken).HasMaxLength(512);
            entity.Property(e => e.ErrorCode).HasMaxLength(100);
            entity.Property(e => e.ErrorDetail).HasMaxLength(2000);
            entity.Property(e => e.MessageId).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.BodyPreview).HasMaxLength(300);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.Success, e.CreatedAt });
            entity.HasIndex(e => new { e.Platform, e.CreatedAt });
        });
    }
}
