using System.Text;
using AdminAPI.Configuration;
using AdminAPI.Data;
using AdminAPI.Mapping;
using AdminAPI.Middleware;
using AdminAPI.Repositories;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using AdminAPI.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Masged.WhatsApp.Extensions;
using Masged.WhatsApp.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.Configure<DeploymentOptions>(
    builder.Configuration.GetSection(DeploymentOptions.SectionName));
builder.Services.AddHostedService<DatabaseBootstrapHostedService>();
builder.Services.AddSingleton<IntegrationSecretsCache>();
builder.Services.AddSingleton<Masged.WhatsApp.Options.IWasenderRuntimeOverride>(sp =>
    sp.GetRequiredService<IntegrationSecretsCache>());
builder.Services.AddHostedService<IntegrationSecretsBootstrapHostedService>();
builder.Services.AddScoped<IIntegrationSettingsService, IntegrationSettingsService>();

builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateAboutRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<WhatsappValidatorsAnchor>();

builder.Services.AddScoped<IAboutRepository, AboutRepository>();
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IMasgedSettingsRepository, MasgedSettingsRepository>();
builder.Services.AddScoped<IMasgedSettingsService, MasgedSettingsService>();
builder.Services.AddScoped<IWorkDayService, WorkDayService>();
builder.Services.AddScoped<ITipGuidanceRepository, TipGuidanceRepository>();
builder.Services.AddScoped<ITipGuidanceService, TipGuidanceService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAgeUpdateService, AgeUpdateService>();
builder.Services.Configure<AgeUpdateOptions>(
    builder.Configuration.GetSection(AgeUpdateOptions.SectionName));
builder.Services.AddSingleton<AgeUpdateLastRunState>();
builder.Services.AddHostedService<AgeUpdateBackgroundService>();
builder.Services.AddSingleton<JwtTokenFactory>();
var activityUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Uploads",
    "Activities");
Directory.CreateDirectory(activityUploadDirectory);
builder.Services.Configure<ActivityUploadOptions>(options =>
{
    options.Directory = activityUploadDirectory;
});
var mosqueUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Uploads",
    "Mosques");
Directory.CreateDirectory(mosqueUploadDirectory);
builder.Services.Configure<MosqueUploadOptions>(options =>
{
    options.Directory = mosqueUploadDirectory;
});
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IActivityService, ActivityService>();
var eventPageUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Uploads",
    "EventPages");
Directory.CreateDirectory(eventPageUploadDirectory);
builder.Services.Configure<EventPageUploadOptions>(options =>
{
    options.Directory = eventPageUploadDirectory;
});
builder.Services.AddScoped<IEventPageRepository, EventPageRepository>();
builder.Services.AddScoped<IEventPageService, EventPageService>();
builder.Services.AddScoped<IEventPageResponseRepository, EventPageResponseRepository>();
builder.Services.AddScoped<IEventPageResponseService, EventPageResponseService>();
builder.Services.AddScoped<IPublicEventPageService, PublicEventPageService>();
builder.Services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
builder.Services.AddScoped<IContactInfoService, ContactInfoService>();
builder.Services.AddScoped<IHeroSlideRepository, HeroSlideRepository>();
builder.Services.AddScoped<IHeroSlideService, HeroSlideService>();
builder.Services.AddScoped<IMosqueRepository, MosqueRepository>();
builder.Services.AddScoped<IMosqueService, MosqueService>();
builder.Services.AddScoped<IPlanLevelRepository, PlanLevelRepository>();
builder.Services.AddScoped<IPlanLevelService, PlanLevelService>();
builder.Services.AddScoped<ISocialLinkRepository, SocialLinkRepository>();
builder.Services.AddScoped<ISocialLinkService, SocialLinkService>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IQuranCircleRepository, QuranCircleRepository>();
builder.Services.AddScoped<IQuranCircleService, QuranCircleService>();
builder.Services.AddScoped<IQuranCirclePlansClearService, QuranCirclePlansClearService>();
builder.Services.AddScoped<IFilesManagerRepository, FilesManagerRepository>();
builder.Services.AddScoped<IFilesManagerService, FilesManagerService>();
builder.Services.AddScoped<IExpensiveRepository, ExpensiveRepository>();
builder.Services.AddScoped<IExpensiveService, ExpensiveService>();
builder.Services.AddScoped<ITeacherSendNoteRepository, TeacherSendNoteRepository>();
builder.Services.AddScoped<ITeacherSendNoteService, TeacherSendNoteService>();
builder.Services.AddScoped<ITeacherSalaryRepository, TeacherSalaryRepository>();
builder.Services.AddScoped<ITeacherSalaryService, TeacherSalaryService>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<TeacherLocationService>();
builder.Services.AddScoped<ITeacherFormService, TeacherFormService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentCardPrintService, StudentCardPrintService>();
builder.Services.AddScoped<IWomansActivityRepository, WomansActivityRepository>();
builder.Services.AddScoped<IWomansActivityService, WomansActivityService>();
builder.Services.AddScoped<ICurrentStudentPlanRepository, CurrentStudentPlanRepository>();
builder.Services.AddScoped<ICurrentStudentPlanService, CurrentStudentPlanService>();
builder.Services.AddScoped<IStudentPlanRepository, StudentPlanRepository>();
builder.Services.AddScoped<IStudentPlanService, StudentPlanService>();
builder.Services.AddScoped<IAttendanceReportRepository, AttendanceReportRepository>();
builder.Services.AddScoped<IAttendanceReportService, AttendanceReportService>();
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IOthaiminCenterRepository, OthaiminCenterRepository>();
builder.Services.AddScoped<IOthaiminCenterService, OthaiminCenterService>();
builder.Services.AddScoped<IMemorizationRevisionReportRepository, MemorizationRevisionReportRepository>();
builder.Services.AddScoped<IMemorizationRevisionReportService, MemorizationRevisionReportService>();
builder.Services.AddScoped<ICircleMemorizationRevisionReportRepository, CircleMemorizationRevisionReportRepository>();
builder.Services.AddScoped<ICircleMemorizationRevisionReportService, CircleMemorizationRevisionReportService>();
builder.Services.AddScoped<ICircleVisitRatingRepository, CircleVisitRatingRepository>();
builder.Services.AddScoped<ICircleVisitRatingService, CircleVisitRatingService>();
builder.Services.AddScoped<ISpecialStudentsReportRepository, SpecialStudentsReportRepository>();
builder.Services.AddScoped<ISpecialStudentsReportService, SpecialStudentsReportService>();
builder.Services.AddScoped<IStudents2Repository, Students2Repository>();
builder.Services.AddScoped<IStudents2Service, Students2Service>();
builder.Services.AddScoped<IParentPanelLogStatisticsRepository, ParentPanelLogStatisticsRepository>();
builder.Services.AddScoped<IParentPanelLogStatisticsService, ParentPanelLogStatisticsService>();
builder.Services.AddScoped<ITeachersAttendanceRepository, TeachersAttendanceRepository>();
builder.Services.AddScoped<ITeachersAttendanceService, TeachersAttendanceService>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IParentsFollowupRepository, ParentsFollowupRepository>();
builder.Services.AddScoped<IParentsFollowupService, ParentsFollowupService>();
builder.Services.AddScoped<ISubscribeRepository, SubscribeRepository>();
builder.Services.AddScoped<ISubscribeService, SubscribeService>();
builder.Services.AddScoped<ITestCertificateRepository, TestCertificateRepository>();
builder.Services.AddScoped<ITestCertificateService, TestCertificateService>();
builder.Services.AddScoped<ITestsReportRepository, TestsReportRepository>();
builder.Services.AddScoped<ITestsReportService, TestsReportService>();
builder.Services.Configure<PublicSiteOptions>(
    builder.Configuration.GetSection(PublicSiteOptions.SectionName));
builder.Services.Configure<StudentQrOptions>(
    builder.Configuration.GetSection(StudentQrOptions.SectionName));
builder.Services.AddSingleton<StudentQrTokenService>();
builder.Services.Configure<PublicRegistrationOptions>(
    builder.Configuration.GetSection(PublicRegistrationOptions.SectionName));
builder.Services.AddScoped<RegistrationSettingsService>();
builder.Services.AddScoped<IPublicIndexService, PublicIndexService>();
builder.Services.Configure<CountryDialCodeOptions>(
    builder.Configuration.GetSection(CountryDialCodeOptions.SectionName));
builder.Services.AddSingleton<ICountryDialCodeService, CountryDialCodeService>();

builder.Services.AddMasgedWhatsApp(builder.Configuration);
builder.Services.AddScoped<IWhatsappSessionStore, WhatsappQrSessionStore>();
builder.Services.AddScoped<IWhatsappQueueRepository, WhatsappQueueRepository>();
builder.Services.AddScoped<IWhatsappPendingRepository, WhatsappPendingRepository>();
builder.Services.AddScoped<IWhatsappPendingService, WhatsappPendingService>();
builder.Services.AddScoped<IWhatsappPreConfiguredService, WhatsappPreConfiguredService>();
builder.Services.AddScoped<IWhatsappQrService, WhatsappQrService>();
builder.Services.AddScoped<IWhatsappSenderService, WhatsappSenderService>();
builder.Services.Configure<FirebaseSettings>(
    builder.Configuration.GetSection(FirebaseSettings.SectionName));
builder.Services.AddScoped<IAdminPushNotificationService, AdminPushNotificationService>();

var tipGuidanceUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Uploads",
    "Competitions");
Directory.CreateDirectory(tipGuidanceUploadDirectory);
builder.Services.Configure<TipGuidanceUploadOptions>(options =>
{
    options.Directory = tipGuidanceUploadDirectory;
});

var heroUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Uploads",
    "hero");
Directory.CreateDirectory(heroUploadDirectory);
builder.Services.Configure<HeroUploadOptions>(options =>
{
    options.Directory = heroUploadDirectory;
});

var newsUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Uploads",
    "news");
Directory.CreateDirectory(newsUploadDirectory);
builder.Services.Configure<NewsUploadOptions>(options =>
{
    options.Directory = newsUploadDirectory;
});

var filesManagerUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "FilesManager");
Directory.CreateDirectory(filesManagerUploadDirectory);
builder.Services.Configure<FilesManagerUploadOptions>(options =>
{
    options.Directory = filesManagerUploadDirectory;
    options.PublicBaseUrl = builder.Configuration["FilesManager:PublicBaseUrl"] ?? "https://mosque-mbark-j.com";
});

var expensiveUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Uploads");
Directory.CreateDirectory(expensiveUploadDirectory);
builder.Services.Configure<ExpensiveUploadOptions>(options =>
{
    options.Directory = expensiveUploadDirectory;
});
builder.Services.Configure<TeacherUploadOptions>(options =>
{
    options.Directory = expensiveUploadDirectory;
});
builder.Services.Configure<ParentsFollowupUploadOptions>(options =>
{
    options.Directory = expensiveUploadDirectory;
});

var qcfFontDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    QcfFontStorage.DirectoryName);
Directory.CreateDirectory(qcfFontDirectory);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AdminAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "AdminPanelUI";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(activityUploadDirectory),
    RequestPath = ActivityImageStorage.RequestPath
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(eventPageUploadDirectory),
    RequestPath = EventPageImageStorage.RequestPath
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(mosqueUploadDirectory),
    RequestPath = MosqueImageStorage.RequestPath
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(tipGuidanceUploadDirectory),
    RequestPath = TipGuidanceImageStorage.RequestPath
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(heroUploadDirectory),
    RequestPath = HeroImageStorage.RequestPath
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(newsUploadDirectory),
    RequestPath = NewsImageStorage.RequestPath
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(filesManagerUploadDirectory),
    RequestPath = FilesManagerStorage.RequestPath
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(expensiveUploadDirectory),
    RequestPath = TeacherImageStorage.RequestPath
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(qcfFontDirectory),
    RequestPath = QcfFontStorage.RequestPath,
    ServeUnknownFileTypes = true,
    DefaultContentType = "font/woff",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = "public,max-age=31536000,immutable";
    }
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
