// Copy into your app. Map to table whatsapp_temp_table (same schema as AdminAPI).
namespace YourApp.WhatsApp;

public class WhatsappTempTableEntity
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public string? Image { get; set; }
    public string? Mobile { get; set; }
    public int? IsGirl { get; set; }
}

// DbContext snippet:
//
// public DbSet<WhatsappTempTableEntity> WhatsappTempTables => Set<WhatsappTempTableEntity>();
//
// modelBuilder.Entity<WhatsappTempTableEntity>(entity =>
// {
//     entity.ToTable("whatsapp_temp_table");
//     entity.Property(e => e.Id).HasColumnName("id");
//     entity.Property(e => e.Image).HasColumnName("image");
//     entity.Property(e => e.Message).HasColumnName("message");
//     entity.Property(e => e.Mobile).HasMaxLength(50).HasColumnName("mobile");
//     entity.Property(e => e.IsGirl).HasColumnName("IsGirl");
// });
