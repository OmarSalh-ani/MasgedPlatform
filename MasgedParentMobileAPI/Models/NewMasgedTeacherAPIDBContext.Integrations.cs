using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Models;

public partial class NewMasgedTeacherAPIDBContext
{
    public virtual DbSet<IntegrationSetting> IntegrationSettings { get; set; }
}
