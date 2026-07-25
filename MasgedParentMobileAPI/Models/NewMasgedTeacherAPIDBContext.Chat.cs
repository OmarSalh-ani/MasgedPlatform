#nullable disable
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Models;

public partial class NewMasgedTeacherAPIDBContext
{
    public virtual DbSet<ParentTeacherChatMessage> ParentTeacherChatMessages { get; set; }

    public virtual DbSet<ParentRegistrationOtp> ParentRegistrationOtps { get; set; }

    public virtual DbSet<ParentDeviceToken> ParentDeviceTokens { get; set; }

    public virtual DbSet<TeacherDeviceToken> TeacherDeviceTokens { get; set; }

    public virtual DbSet<PushDeliveryLog> PushDeliveryLogs { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParentTeacherChatMessage>(entity =>
        {
            entity.ToTable("ParentTeacherChatMessages");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.ParentPhone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.MessageText).HasMaxLength(2000).IsRequired();
            entity.Property(e => e.SentAt).HasColumnType("datetime2");

            entity.HasOne<Teacher>()
                .WithMany()
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<RegisterForm>()
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ParentRegistrationOtp>(entity =>
        {
            entity.ToTable("ParentRegistrationOtp");
            entity.HasKey(e => e.CanonicalPhone);
            entity.Property(e => e.CanonicalPhone).HasMaxLength(32);
            entity.Property(e => e.FatherName).HasMaxLength(200);
            entity.Property(e => e.PasswordPlain).HasMaxLength(200);
            entity.Property(e => e.OtpCode).HasMaxLength(6);
        });

        modelBuilder.Entity<ParentDeviceToken>(entity =>
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

        modelBuilder.Entity<TeacherDeviceToken>(entity =>
        {
            entity.ToTable("TeacherDeviceTokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FcmToken).HasMaxLength(512).IsRequired();
            entity.Property(e => e.Platform).HasMaxLength(20).IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");
            entity.HasIndex(e => e.FcmToken).IsUnique();
            entity.HasIndex(e => e.TeacherId);
        });

        modelBuilder.Entity<PushDeliveryLog>(entity =>
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