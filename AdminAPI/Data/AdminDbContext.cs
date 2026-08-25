using AdminAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Data;

public class AdminDbContext(DbContextOptions<AdminDbContext> options) : DbContext(options)
{
    public DbSet<AboutAssociation> AboutAssociations => Set<AboutAssociation>();

    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<TeacherMapLocation> TeacherMapLocations => Set<TeacherMapLocation>();
    public DbSet<RegisterForm> RegisterForms => Set<RegisterForm>();
    public DbSet<MrkzStudent> MrkzStudents => Set<MrkzStudent>();
    public DbSet<ParentFollowup> ParentFollowups => Set<ParentFollowup>();
    public DbSet<ParentPanelLog> ParentPanelLogs => Set<ParentPanelLog>();
    public DbSet<TipGuidance> TipGuidances => Set<TipGuidance>();
    public DbSet<ContactInfo> ContactInfos => Set<ContactInfo>();
    public DbSet<HeroSlide> HeroSlides => Set<HeroSlide>();
    public DbSet<NewsItem> NewsItems => Set<NewsItem>();
    public DbSet<Mosque> Mosques => Set<Mosque>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    public DbSet<FilesManager> FilesManagers => Set<FilesManager>();
    public DbSet<Expensive> Expensives => Set<Expensive>();
    public DbSet<QuranCircle> QuranCircles => Set<QuranCircle>();
    public DbSet<CircleDay> CircleDays => Set<CircleDay>();
    public DbSet<CircleAttendance> CircleAttendances => Set<CircleAttendance>();
    public DbSet<StudentMemorizingCard> StudentMemorizingCards => Set<StudentMemorizingCard>();
    public DbSet<StudentTest> StudentTests => Set<StudentTest>();
    public DbSet<TestHead> TestHeads => Set<TestHead>();
    public DbSet<PlanLevel> PlanLevels => Set<PlanLevel>();
    public DbSet<ReadyPlan> ReadyPlans => Set<ReadyPlan>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<MasgedSetting> MasgedSettings => Set<MasgedSetting>();
    public DbSet<IntegrationSetting> IntegrationSettings => Set<IntegrationSetting>();
    public DbSet<WhatsappTempTable> WhatsappTempTables => Set<WhatsappTempTable>();
    public DbSet<WhatsappPreConfiguredMessage> WhatsappPreConfiguredMessages => Set<WhatsappPreConfiguredMessage>();
    public DbSet<DynamicForm> DynamicForms => Set<DynamicForm>();
    public DbSet<TeachersAdminNote> TeachersAdminNotes => Set<TeachersAdminNote>();
    public DbSet<TeacherSalary> TeacherSalaries => Set<TeacherSalary>();
    public DbSet<TeacherAttendance> TeacherAttendances => Set<TeacherAttendance>();
    public DbSet<WomanActivity> WomanActivities => Set<WomanActivity>();
    public DbSet<StudentPlan> StudentPlans => Set<StudentPlan>();
    public DbSet<StudentPlanMemorizing> StudentPlanMemorizings => Set<StudentPlanMemorizing>();
    public DbSet<StudentPlanRevise> StudentPlanRevises => Set<StudentPlanRevise>();
    public DbSet<StudentPlanItemLog> StudentPlanItemLogs => Set<StudentPlanItemLog>();
    public DbSet<QuranSurah> QuranSurahs => Set<QuranSurah>();
    public DbSet<QuranAyah> QuranAyahs => Set<QuranAyah>();
    public DbSet<HolyQuran> HolyQurans => Set<HolyQuran>();
    public DbSet<AnnouncementContact> AnnouncementContacts => Set<AnnouncementContact>();
    public DbSet<AnnouncementMessage> AnnouncementMessages => Set<AnnouncementMessage>();
    public DbSet<ParentDeviceToken> ParentDeviceTokens => Set<ParentDeviceToken>();
    public DbSet<TeacherDeviceToken> TeacherDeviceTokens => Set<TeacherDeviceToken>();
    public DbSet<PushDeliveryLog> PushDeliveryLogs => Set<PushDeliveryLog>();
    public DbSet<CircleVisitRating> CircleVisitRatings => Set<CircleVisitRating>();
    public DbSet<CircleVisitRatingItem> CircleVisitRatingItems => Set<CircleVisitRatingItem>();
    public DbSet<EventPage> EventPages => Set<EventPage>();
    public DbSet<EventPageTrack> EventPageTracks => Set<EventPageTrack>();
    public DbSet<EventPageFormField> EventPageFormFields => Set<EventPageFormField>();
    public DbSet<EventPageResponse> EventPageResponses => Set<EventPageResponse>();
    public DbSet<EventPageResponseValue> EventPageResponseValues => Set<EventPageResponseValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AboutAssociation>(entity =>
        {
            entity.ToTable("AboutAssociation");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.MapsUrl).HasMaxLength(1000);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.ToTable("Activity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IconClass).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teacher");
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.Mobile).HasMaxLength(500);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<TeacherMapLocation>(entity =>
        {
            entity.ToTable("TeacherMapLocation");
            entity.Property(e => e.MapURL)
                .HasColumnName("MapURL")
                .HasMaxLength(2000);
            entity.Property(e => e.Latitude).HasMaxLength(50);
            entity.Property(e => e.Longitude).HasMaxLength(50);
            entity.HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_TeacherMapLocation_Teacher");
        });

        modelBuilder.Entity<RegisterForm>(entity =>
        {
            entity.ToTable("RegisterForm");
            entity.Property(e => e.StudentName).HasMaxLength(300);
            entity.Property(e => e.FullName).HasMaxLength(300);
            entity.Property(e => e.FatherName).HasMaxLength(300);
            entity.Property(e => e.FatherPhone).HasMaxLength(50);
            entity.Property(e => e.FatherPhone2).HasMaxLength(50);
            entity.Property(e => e.StudentPhone).HasMaxLength(50);
            entity.Property(e => e.StudentGender).HasMaxLength(50);
            entity.Property(e => e.LearnCertificate).HasMaxLength(500);
            entity.Property(e => e.ThePassword).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.HasOne(d => d.QuranCircle)
                .WithMany()
                .HasForeignKey(d => d.QuranCircleId)
                .HasConstraintName("FK_RegisterForm_QuranCircle");
            entity.HasOne(d => d.WomanActivity)
                .WithMany()
                .HasForeignKey(d => d.WomanActivityType)
                .HasConstraintName("FK_RegisterForm_WomanActivity");
            entity.HasOne(d => d.PlanLevel)
                .WithMany(p => p.RegisterForms)
                .HasForeignKey(d => d.PlanLevelId)
                .HasConstraintName("FK_RegisterForm_PlanLevel");
        });

        modelBuilder.Entity<MrkzStudent>(entity =>
        {
            entity.ToTable("MrkzStudents");
            entity.Property(e => e.StudentName).HasMaxLength(300);
            entity.Property(e => e.FullName).HasMaxLength(500);
            entity.Property(e => e.FatherName).HasMaxLength(300);
            entity.Property(e => e.FatherPhone).HasMaxLength(50);
            entity.Property(e => e.FatherPhone2).HasMaxLength(50);
            entity.Property(e => e.StudentPhone).HasMaxLength(50);
            entity.Property(e => e.StudentGender).HasMaxLength(5);
            entity.Property(e => e.LearnCertificate).HasMaxLength(500);
            entity.Property(e => e.ThePassword).HasMaxLength(250);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.HasOne(d => d.QuranCircle)
                .WithMany()
                .HasForeignKey(d => d.QuranCircleId)
                .HasConstraintName("FK_MrkzStudents_QuranCircle");
            entity.HasOne(d => d.WomanActivity)
                .WithMany()
                .HasForeignKey(d => d.WomanActivityType)
                .HasConstraintName("FK_MrkzStudents_WomanActivity");
            entity.HasOne(d => d.PlanLevel)
                .WithMany()
                .HasForeignKey(d => d.PlanLevelId)
                .HasConstraintName("FK_MrkzStudents_PlanLevel");
        });

        modelBuilder.Entity<ParentFollowup>(entity =>
        {
            entity.ToTable("ParentFollowup");
            entity.HasKey(e => e.StudentId);
            entity.Property(e => e.StudentId).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.MaritalStatus)
                .HasMaxLength(500)
                .HasColumnName("maritalStatus");
            entity.Property(e => e.HealthCondition)
                .HasMaxLength(500)
                .HasColumnName("healthCondition");
            entity.Property(e => e.HealthDetails)
                .HasMaxLength(500)
                .HasColumnName("healthDetails");
            entity.Property(e => e.LearningDifficulties)
                .HasMaxLength(500)
                .HasColumnName("learningDifficulties");
            entity.Property(e => e.LearningDifficultiesNotes)
                .HasMaxLength(500)
                .HasColumnName("learningDifficultiesNotes");
            entity.Property(e => e.PhotoPath)
                .HasMaxLength(500)
                .HasColumnName("photoPath");
            entity.HasOne(d => d.Student)
                .WithOne(p => p.ParentFollowup)
                .HasForeignKey<ParentFollowup>(d => d.StudentId)
                .HasConstraintName("FK_ParentFollowup_RegisterForm");
        });

        modelBuilder.Entity<ParentPanelLog>(entity =>
        {
            entity.ToTable("ParentPanelLog");
            entity.HasIndex(e => e.AccessDateTime, "IX_ParentPanelLog_AccessDateTime");
            entity.HasIndex(e => e.ParentMobile, "IX_ParentPanelLog_ParentMobile");
            entity.HasIndex(e => e.StudentId, "IX_ParentPanelLog_StudentId");
            entity.Property(e => e.AccessDateTime).HasColumnType("datetime");
            entity.Property(e => e.ParentMobile)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasOne(d => d.RegisterForm)
                .WithMany(p => p.ParentPanelLogs)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_ParentPanelLog_RegisterForm");
        });

        modelBuilder.Entity<TipGuidance>(entity =>
        {
            entity.ToTable("Competition");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.LinkUrl).HasMaxLength(500);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<ContactInfo>(entity =>
        {
            entity.ToTable("ContactInfo");
            entity.Property(e => e.ContactType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Label).HasMaxLength(100);
            entity.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(500);
        });

        modelBuilder.Entity<HeroSlide>(entity =>
        {
            entity.ToTable("HeroSlide");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<NewsItem>(entity =>
        {
            entity.ToTable("NewsItem");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.LinkUrl).HasMaxLength(500);
            entity.Property(e => e.NewsDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(300);
        });

        modelBuilder.Entity<Mosque>(entity =>
        {
            entity.ToTable("Mosque");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GoogleMapsUrl).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<SocialLink>(entity =>
        {
            entity.ToTable("SocialLink");
            entity.Property(e => e.PlatformName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.IconClass).HasMaxLength(100);
        });

        modelBuilder.Entity<FilesManager>(entity =>
        {
            entity.ToTable("FilesManagers");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(500);
        });

        modelBuilder.Entity<Expensive>(entity =>
        {
            entity.ToTable("Expensives");
            entity.Property(e => e.AttachmentsFolder).HasMaxLength(250);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ForGirls)
                .HasDefaultValue(false);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Reason)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Supplier)
                .IsRequired()
                .HasMaxLength(250);

            entity.HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Expensives_Teacher");
        });

        modelBuilder.Entity<QuranCircle>(entity =>
        {
            entity.ToTable("QuranCircle");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).IsRequired();
            entity.HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_QuranCircle_Teacher");
        });

        modelBuilder.Entity<CircleDay>(entity =>
        {
            entity.ToTable("CircleDay");
            entity.HasOne(d => d.Circle)
                .WithMany(p => p.CircleDays)
                .HasForeignKey(d => d.CircleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CircleAttendance>(entity =>
        {
            entity.ToTable("CircleAttendance");
            entity.HasOne(d => d.RegisterForm)
                .WithMany(p => p.CircleAttendances)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<StudentMemorizingCard>(entity =>
        {
            entity.ToTable("StudentMemorizingCard");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.HasOne(d => d.Student)
                .WithMany(p => p.StudentMemorizingCards)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_StudentMemorizingCard_RegisterForm");
        });

        modelBuilder.Entity<StudentTest>(entity =>
        {
            entity.ToTable("StudentTest");
            entity.HasOne(d => d.Student)
                .WithMany(p => p.StudentTests)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_StudentTest_RegisterForm");
        });

        modelBuilder.Entity<TestHead>(entity =>
        {
            entity.ToTable("TestHead");
            entity.Property(e => e.TestDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FinalResult).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MemorizationScore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TajweedScore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RevisionScore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalScore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TestFrom).HasMaxLength(500);
            entity.Property(e => e.TestTo).HasMaxLength(500);
            entity.Property(e => e.SurahName).HasMaxLength(500);
            entity.Property(e => e.HezbNumber).HasMaxLength(100);
            entity.Property(e => e.Grade).HasMaxLength(100);
            entity.Property(e => e.TestName).HasMaxLength(200);
            entity.Property(e => e.TestType).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasOne(d => d.Student)
                .WithMany(p => p.TestHeads)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_TestHead_RegisterForm");
            entity.HasOne(d => d.QuranCircle)
                .WithMany()
                .HasForeignKey(d => d.CircleId)
                .HasConstraintName("FK_TestHead_QuranCircle");
            entity.HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_TestHead_Teacher");
        });

        modelBuilder.Entity<PlanLevel>(entity =>
        {
            entity.ToTable("PlanLevel");
            entity.Property(e => e.LevelName).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<ReadyPlan>(entity =>
        {
            entity.ToTable("ReadyPlan");
            entity.HasOne(d => d.PlanLevel)
                .WithMany(p => p.ReadyPlans)
                .HasForeignKey(d => d.PlanLevelId)
                .HasConstraintName("FK_ReadyPlan_PlanLevel");
        });

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.ToTable("AppSetting");
            entity.Property(e => e.Key).HasMaxLength(200);
            entity.Property(e => e.Value).HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<MasgedSetting>(entity =>
        {
            entity.ToTable("MasgedSettings");
            entity.Property(e => e.MasgedName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.LogoFileName).HasMaxLength(500);
            entity.Property(e => e.ParentAppStoreUrl).HasMaxLength(500);
            entity.Property(e => e.ParentGooglePlayUrl).HasMaxLength(500);
            entity.Property(e => e.TeacherAppStoreUrl).HasMaxLength(500);
            entity.Property(e => e.TeacherGooglePlayUrl).HasMaxLength(500);
            entity.Property(e => e.PrimaryColor).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<IntegrationSetting>(entity =>
        {
            entity.ToTable("IntegrationSettings");
            entity.Property(e => e.WasenderApiToken).HasMaxLength(500);
            entity.Property(e => e.WasenderSessionApiKey).HasMaxLength(500);
            entity.Property(e => e.AgoraAppId).HasMaxLength(200);
            entity.Property(e => e.AgoraAppCertificate).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<TeacherSalary>(entity =>
        {
            entity.ToTable("TeacherSalary");
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CalculatedSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalHours).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_TeacherSalary_Teacher");
        });

        modelBuilder.Entity<TeacherAttendance>(entity =>
        {
            entity.ToTable("TeacherAttendance");
            entity.HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_TeacherAttendance_Teacher");
        });

        modelBuilder.Entity<TeachersAdminNote>(entity =>
        {
            entity.ToTable("TeachersAdminNotes");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).IsRequired();
            entity.Property(e => e.ReadTime).HasColumnType("datetime");
            entity.HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_TeachersAdminNotes_Teacher");
        });

        modelBuilder.Entity<WomanActivity>(entity =>
        {
            entity.ToTable("WomanActivity");
            entity.Property(e => e.IsVisible).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(500);
        });

        modelBuilder.Entity<StudentPlan>(entity =>
        {
            entity.ToTable("StudentPlan");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.HasOne(d => d.RegisterForm)
                .WithMany(p => p.StudentPlans)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_StudentPlan_RegisterForm");
        });

        modelBuilder.Entity<StudentPlanMemorizing>(entity =>
        {
            entity.ToTable("StudentPlanMemorizing");
            entity.Property(e => e.MemorizationLevel).HasMaxLength(50);
            entity.HasOne(d => d.RegisterForm)
                .WithMany()
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_StudentPlanMemorizing_RegisterForm");
            entity.HasOne(d => d.QuranSurah)
                .WithMany(p => p.StudentPlanMemorizings)
                .HasForeignKey(d => d.SurahId)
                .HasConstraintName("FK_StudentPlanMemorizing_QuranSurah");
            entity.HasOne<StudentPlan>()
                .WithMany(p => p.StudentPlanMemorizings)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_StudentPlanMemorizing_StudentPlan");
        });

        modelBuilder.Entity<StudentPlanRevise>(entity =>
        {
            entity.ToTable("StudentPlanRevise");
            entity.Property(e => e.MemorizationLevel).HasMaxLength(50);
            entity.HasOne(d => d.RegisterForm)
                .WithMany()
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_StudentPlanRevise_RegisterForm");
            entity.HasOne(d => d.QuranSurah)
                .WithMany(p => p.StudentPlanRevises)
                .HasForeignKey(d => d.SurahId)
                .HasConstraintName("FK_StudentPlanRevise_QuranSurah");
            entity.HasOne<StudentPlan>()
                .WithMany(p => p.StudentPlanRevises)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_StudentPlanRevise_StudentPlan");
        });

        modelBuilder.Entity<StudentPlanItemLog>(entity =>
        {
            entity.ToTable("StudentPlanItemLog");
            entity.HasOne<StudentPlan>()
                .WithMany(p => p.StudentPlanItemLogs)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_StudentPlanItemLog_StudentPlan");
        });

        modelBuilder.Entity<QuranSurah>(entity =>
        {
            entity.ToTable("QuranSurah");
            entity.Property(e => e.NameAr).HasMaxLength(200);
        });

        modelBuilder.Entity<QuranAyah>(entity =>
        {
            entity.ToTable("QuranAyah");
            entity.HasOne(d => d.Surah)
                .WithMany(p => p.QuranAyahs)
                .HasForeignKey(d => d.SurahId)
                .HasConstraintName("FK_QuranAyah_QuranSurah");
        });

        modelBuilder.Entity<HolyQuran>(entity =>
        {
            entity.ToTable("HolyQuran");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Jozz).HasColumnName("jozz");
            entity.Property(e => e.SuraNo).HasColumnName("sura_no");
            entity.Property(e => e.SuraNameAr).HasMaxLength(100).HasColumnName("sura_name_ar");
            entity.Property(e => e.Page).HasColumnName("page");
            entity.Property(e => e.LineStart).HasColumnName("line_start");
            entity.Property(e => e.LineEnd).HasColumnName("line_end");
            entity.Property(e => e.AyaNo).HasColumnName("aya_no");
            entity.Property(e => e.AyaTextEmlaey).HasColumnName("aya_text_emlaey");
            entity.Property(e => e.HezbNo).HasColumnName("hezb_no");
            entity.Property(e => e.HezbQuarter).HasColumnName("hezb_quarter");
        });

        modelBuilder.Entity<WhatsappPreConfiguredMessage>(entity =>
        {
            entity.ToTable("WhatsappPreConfiguredMessages");
            entity.Property(e => e.WhatsappMessage).IsRequired();
            entity.Property(e => e.Event).IsRequired();
        });

        modelBuilder.Entity<DynamicForm>(entity =>
        {
            entity.ToTable("DynamicForms");
            entity.Property(e => e.Title).HasMaxLength(500);
        });

        modelBuilder.Entity<WhatsappTempTable>(entity =>
        {
            entity.ToTable("whatsapp_temp_table");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Mobile)
                .HasMaxLength(50)
                .HasColumnName("mobile");
            entity.Property(e => e.IsGirl).HasColumnName("IsGirl");
        });

        modelBuilder.Entity<AnnouncementContact>(entity =>
        {
            entity.ToTable("AnnouncementContacts");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<AnnouncementMessage>(entity =>
        {
            entity.ToTable("AnnouncementMessages");
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SentAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<ParentDeviceToken>(entity =>
        {
            entity.ToTable("ParentDeviceTokens");
            entity.Property(e => e.ParentPhone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FcmToken).HasMaxLength(512).IsRequired();
            entity.Property(e => e.Platform).HasMaxLength(20).IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            entity.HasIndex(e => e.FcmToken).IsUnique();
            entity.HasIndex(e => e.ParentPhone);
        });

        modelBuilder.Entity<TeacherDeviceToken>(entity =>
        {
            entity.ToTable("TeacherDeviceTokens");
            entity.Property(e => e.FcmToken).HasMaxLength(512).IsRequired();
            entity.Property(e => e.Platform).HasMaxLength(20).IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            entity.HasIndex(e => e.FcmToken).IsUnique();
            entity.HasIndex(e => e.TeacherId);
        });

        modelBuilder.Entity<PushDeliveryLog>(entity =>
        {
            entity.ToTable("PushDeliveryLogs");
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

        modelBuilder.Entity<CircleVisitRating>(entity =>
        {
            entity.ToTable("CircleVisitRatings");
            entity.Property(e => e.VisitDate).HasColumnType("date");
            entity.Property(e => e.VisitTime).HasColumnType("time");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CircleVisitRatings_Teacher");
            entity.HasOne(d => d.QuranCircle)
                .WithMany()
                .HasForeignKey(d => d.QuranCircleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CircleVisitRatings_QuranCircle");
            entity.HasIndex(e => new { e.TeacherId, e.VisitDate });
            entity.HasIndex(e => e.CreatedBy);
        });

        modelBuilder.Entity<CircleVisitRatingItem>(entity =>
        {
            entity.ToTable("CircleVisitRatingItems");
            entity.Property(e => e.Criterion).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Rating).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasOne(d => d.CircleVisitRating)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.CircleVisitRatingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CircleVisitRatingItems_CircleVisitRatings");
        });

        modelBuilder.Entity<EventPage>(entity =>
        {
            entity.ToTable("EventPages");
            entity.Property(e => e.ActivityName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(120);
            entity.Property(e => e.CourseTitle).IsRequired().HasMaxLength(300);
            entity.Property(e => e.InvitationText).HasMaxLength(500);
            entity.Property(e => e.MosqueName).HasMaxLength(300);
            entity.Property(e => e.SubjectText).HasMaxLength(1000);
            entity.Property(e => e.DateText).HasMaxLength(300);
            entity.Property(e => e.TimeText).HasMaxLength(300);
            entity.Property(e => e.SupervisorsText).HasMaxLength(1000);
            entity.Property(e => e.ContactPhone).HasMaxLength(50);
            entity.Property(e => e.SocialAccounts).HasMaxLength(200);
            entity.Property(e => e.LocationNote).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(e => e.ActivityName).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        modelBuilder.Entity<EventPageTrack>(entity =>
        {
            entity.ToTable("EventPageTracks");
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.HasOne(d => d.EventPage)
                .WithMany(p => p.Tracks)
                .HasForeignKey(d => d.EventPageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventPageTracks_EventPages");
        });

        modelBuilder.Entity<EventPageFormField>(entity =>
        {
            entity.ToTable("EventPageFormFields");
            entity.Property(e => e.Label).IsRequired().HasMaxLength(300);
            entity.Property(e => e.FieldType).IsRequired().HasMaxLength(30);
            entity.HasOne(d => d.EventPage)
                .WithMany(p => p.FormFields)
                .HasForeignKey(d => d.EventPageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventPageFormFields_EventPages");
        });

        modelBuilder.Entity<EventPageResponse>(entity =>
        {
            entity.ToTable("EventPageResponses");
            entity.Property(e => e.ActivityName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SubmittedAt).HasColumnType("datetime");
            entity.HasOne(d => d.EventPage)
                .WithMany(p => p.Responses)
                .HasForeignKey(d => d.EventPageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventPageResponses_EventPages");
            entity.HasIndex(e => e.ActivityName);
            entity.HasIndex(e => e.SubmittedAt);
        });

        modelBuilder.Entity<EventPageResponseValue>(entity =>
        {
            entity.ToTable("EventPageResponseValues");
            entity.Property(e => e.FieldLabel).IsRequired().HasMaxLength(300);
            entity.HasOne(d => d.Response)
                .WithMany(p => p.Values)
                .HasForeignKey(d => d.ResponseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventPageResponseValues_Responses");
        });
    }
}
