using System.Text;

using MasgedParentMobileAPI.Configuration;

using MasgedParentMobileAPI.Hubs;

using MasgedParentMobileAPI.Middleware;

using MasgedParentMobileAPI.Models;

using MasgedParentMobileAPI.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.HttpOverrides;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection.Extensions;

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



builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.Configure<TeacherJwtSettings>(builder.Configuration.GetSection("TeacherJwt"));

builder.Services.Configure<ChatInternalSettings>(builder.Configuration.GetSection("Chat"));

builder.Services.Configure<AgoraOptions>(builder.Configuration.GetSection(AgoraOptions.SectionName));
builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection(FirebaseSettings.SectionName));

builder.Services.AddSingleton<AgoraTokenService>();
builder.Services.AddSingleton<AgoraSecretsCache>();
builder.Services.AddHostedService<AgoraSecretsRefreshHostedService>();
builder.Services.AddScoped<PushNotificationService>();

builder.Services.Configure<RequestLoggingSettings>(builder.Configuration.GetSection("RequestLogging"));
builder.Services.Configure<StudentQrOptions>(
    builder.Configuration.GetSection(StudentQrOptions.SectionName));
builder.Services.AddSingleton<StudentQrTokenService>();

var studentPhotoUploadDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Uploads");
Directory.CreateDirectory(studentPhotoUploadDirectory);
builder.Services.Configure<StudentPhotoUploadOptions>(options =>
{
    options.Directory = studentPhotoUploadDirectory;
});



builder.Services.AddMemoryCache();

builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddScoped<StudentService>(sp =>

{

    var db = sp.GetRequiredService<NewMasgedTeacherAPIDBContext>();

    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

    var workDayService = sp.GetRequiredService<IWorkDayService>();

    return new StudentService(db, settings.MediaBaseUrl, workDayService);

});

builder.Services.AddScoped<MemorizingArchiveService>();
builder.Services.AddScoped<IWorkDayService, WorkDayService>();
builder.Services.AddScoped<StudentRegistrationService>();
builder.Services.AddScoped<AccountDeletionService>();

builder.Services.AddScoped<ChatService>();

builder.Services.AddSingleton<IChatRealtimePublisher, ChatRealtimePublisher>();
builder.Services.AddScoped<IVideoCallTerminationService, VideoCallTerminationService>();



builder.Services.AddDbContext<NewMasgedTeacherAPIDBContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



var jwtSettings = builder.Configuration.GetSection("ApiSettings:Jwt").Get<JwtSettings>()

    ?? throw new InvalidOperationException("Jwt settings are not configured.");

var teacherJwtSettings = builder.Configuration.GetSection("TeacherJwt").Get<TeacherJwtSettings>()

    ?? throw new InvalidOperationException("TeacherJwt settings are not configured.");



static void JwtMessageReceivedForHub(JwtBearerOptions options)

{

    options.Events = new JwtBearerEvents

    {

        OnMessageReceived = context =>

        {

            var path = context.Request.Path;

            if (path.StartsWithSegments("/hubs/chat")
                || path.StartsWithSegments("/hubs/video-call"))

            {

                var token = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(token))

                    context.Token = token;

            }



            return Task.CompletedTask;

        },

    };

}



const string SmartAuthScheme = "SmartAuth";

builder.Services

    .AddAuthentication(options =>

    {

        options.DefaultAuthenticateScheme = SmartAuthScheme;

        options.DefaultChallengeScheme = SmartAuthScheme;

    })

    .AddPolicyScheme(SmartAuthScheme, "Parent or Teacher JWT", options =>

    {

        options.ForwardDefaultSelector = context =>

            context.Request.Path.StartsWithSegments("/api/teacher")

                ? "TeacherJwt"

                : JwtBearerDefaults.AuthenticationScheme;

    })

    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>

    {

        options.TokenValidationParameters = new TokenValidationParameters

        {

            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,

            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

        };

        JwtMessageReceivedForHub(options);

    })

    .AddJwtBearer("TeacherJwt", options =>

    {

        options.TokenValidationParameters = new TokenValidationParameters

        {

            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ClockSkew = TimeSpan.Zero,

            ValidIssuer = teacherJwtSettings.Issuer,

            ValidAudience = teacherJwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(teacherJwtSettings.Key)),

        };

        JwtMessageReceivedForHub(options);

    });



builder.Services.AddAuthorization();

builder.Services.AddSignalR();

builder.Services.AddUnifiedTeacherMobileApi(builder.Configuration);

builder.Services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IApplicationModelProvider, TeacherApiApplicationModelProvider>());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>

{

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Masged Unified Mobile API (Parent + Teacher)",
        Version = "v1",
        Description = "Parent routes: /api/* — Teacher routes: /api/teacher/*",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme

    {

        Description = "JWT Authorization header using the Bearer scheme.",

        Name = "Authorization",

        In = ParameterLocation.Header,

        Type = SecuritySchemeType.ApiKey,

        Scheme = "Bearer",

    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement

    {

        {

            new OpenApiSecurityScheme

            {

                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },

            },

            Array.Empty<string>()

        },

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

if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}



app.UseCors("AllowAll");

app.UseWebSockets();

app.UseAuthentication();

app.UseAuthorization();

app.UseRequestResponseLogging();

app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

app.MapHub<VideoCallHub>("/hubs/video-call");



app.Run();


