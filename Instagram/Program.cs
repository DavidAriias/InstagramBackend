using Instagram.App.Auth;
using Instagram.App.UseCases.MediaCases.CaptionCase;
using Instagram.App.UseCases.MediaCases.CommentCase;
using Instagram.App.UseCases.MediaCases.LikeCase;
using Instagram.App.UseCases.MediaCases.LocationCase;
using Instagram.App.UseCases.MediaCases.MentionCase;
using Instagram.App.UseCases.MediaCases.PostCase;
using Instagram.App.UseCases.MediaCases.ReelCase;
using Instagram.App.UseCases.MediaCases.ReplyCase;
using Instagram.App.UseCases.MediaCases.StoryCase;
using Instagram.App.UseCases.MediaCases.TagsCase;
using Instagram.App.UseCases.MusicCase;
using Instagram.App.UseCases.NotificationCase;
using Instagram.App.UseCases.RecommendationSystemCase;
using Instagram.App.UseCases.SearchCase;
using Instagram.App.UseCases.SMSCase;
using Instagram.App.UseCases.UserCase.Chat;
using Instagram.App.UseCases.UserCase.EditProfile;
using Instagram.App.UseCases.UserCase.Followed;
using Instagram.App.UseCases.UserCase.Followers;
using Instagram.App.UseCases.UserCase.GetProfile;
using Instagram.App.UseCases.UserCase.SignIn;
using Instagram.App.UseCases.UserCase.SignUp;
using Instagram.App.UseCases.UserCase.SuggestFollowers;
using Instagram.config.constants;
using Instagram.Domain.Repositories.Interfaces.Auth;
using Instagram.Domain.Repositories.Interfaces.Blob;
using Instagram.Domain.Repositories.Interfaces.Cache;
using Instagram.Domain.Repositories.Interfaces.Document.Post;
using Instagram.Domain.Repositories.Interfaces.Document.Reel;
using Instagram.Domain.Repositories.Interfaces.Document.Story;
using Instagram.Domain.Repositories.Interfaces.Graph.Post;
using Instagram.Domain.Repositories.Interfaces.Graph.Reel;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Domain.Repositories.Interfaces.Music;
using Instagram.Domain.Repositories.Interfaces.Notifications;
using Instagram.Domain.Repositories.Interfaces.SMS;
using Instagram.Domain.Repositories.Interfaces.SQL.Notifications;
using Instagram.Domain.Repositories.Interfaces.SQL.Search;
using Instagram.Domain.Repositories.Interfaces.SQL.Token;
using Instagram.Domain.Repositories.Interfaces.SQL.User;
using Instagram.Infraestructure.Data.Repositories.Cache;
using Instagram.Infraestructure.Data.Repositories.Document;
using Instagram.Infraestructure.Data.Repositories.Graph;
using Instagram.Infraestructure.Data.Repositories.Mongo;
using Instagram.Infraestructure.Data.Repositories.SQL;
using Instagram.Infraestructure.Persistence.Context;
using Instagram.Infraestructure.Services.Cloud;
using Instagram.Infraestructure.Services.Identity;
using Instagram.Infraestructure.Services.Music;
using Instagram.Infraestructure.Services.PushNotifications;
using Instagram.Infraestructure.Services.Scheduling;
using Instagram.Infraestructure.Services.SMS;
using Instagram.Presentation.GraphQL.Mutations;
using Instagram.Presentation.GraphQL.Queries;
using Instagram.Presentation.GraphQL.Suscriptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

//Configurando las clases para las bases de datos
builder.Services.Configure<RedisConfig>(builder.Configuration.GetSection("RedisSettings"));
builder.Services.Configure<Neo4jConfig>(builder.Configuration.GetSection("Neo4jSettings"));
builder.Services.Configure<MongoConfig>(builder.Configuration.GetSection("MongoSettings"));

//Configurando conexiones a db
builder.Services
    .AddNpgsql<InstagramContext>(builder.Configuration.GetConnectionString("PostgreSettings"));
builder.Services.AddSingleton<RedisContext>();
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<Neo4jContext>();

//Inyectando casos de uso
builder.Services.AddScoped<IMusicCase,MusicCase>();
builder.Services.AddScoped<IFollowedCase,FollowedCase>();
builder.Services.AddScoped<IFollowerCase,FollowerCase>();
builder.Services.AddScoped<ISignInCase,SignInCase>();
builder.Services.AddScoped<ISignUpCase,SignUpCase>();
builder.Services.AddScoped<IEditProfileCase,EditProfileCase>();
builder.Services.AddScoped<ISmsCase,SmsCase>();
builder.Services.AddScoped<IGetProfileCase, GetProfileCase>();
builder.Services.AddScoped<IChatCase,ChatCase>();
builder.Services.AddScoped<ISuggestFollowersCase,SuggestFollowersCase>();
builder.Services.AddScoped<IPostCase,PostCase>();
builder.Services.AddScoped<ITagsCase,TagsCase>();
builder.Services.AddScoped<ICaptionCase,CaptionCase>();
builder.Services.AddScoped<ILocationCase,LocationCase>();
builder.Services.AddScoped<IReplyCase,ReplyCase>();
builder.Services.AddScoped<ICommentCase,CommentCase>();
builder.Services.AddScoped<ILikeCase,LikeCase>();
builder.Services.AddScoped<IReelCase,ReelCase>();
builder.Services.AddScoped<IMentionCase, MentionCase>();
builder.Services.AddScoped<IStoryCase, StoryCase>();
builder.Services.AddScoped<ISeachCase, SearchCase>();
builder.Services.AddScoped<IRecommendationSystemCase, RecommendationSystemCase>();
builder.Services.AddScoped<INotificationCase, NotificationCase>();

//Inyectando los servicios
builder.Services.AddScoped<IJwtService,JwtService>();
builder.Services.AddScoped<ISmsService,SmsService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IPushNotificationsService,PushNotificationService>();
builder.Services.AddScoped<IImageBlobService,ImageBlobService>();
builder.Services.AddScoped<IVideoBlobService,VideoBlobService>();
builder.Services.AddScoped<IQuartzJobScheduler,QuartzJobScheduler>();

//Inyectando los repositorios
builder.Services.AddScoped<IUserNeo4jRepository,UserNeo4jRepository>();
builder.Services.AddScoped<ISpotifyApiRepository,SpotifyApiRepository>();
builder.Services.AddScoped<IRedisRepository,RedisRepository>();
builder.Services.AddScoped<IUserSQLDbRepository,UserSQLDbRepository>();
builder.Services.AddScoped<IPostMongoDbRepository,PostMongoDbRepository>();
builder.Services.AddScoped<IPostNeo4jRepository,PostNeo4jRepository>();
builder.Services.AddScoped<IReelNeo4jRepository,ReelNeo4jRepository>();
builder.Services.AddScoped<IReelMongoDbRepository,ReelMongoDbRepository>();
builder.Services.AddScoped<ITokenSQLDbRepository,TokenSQLDbRepository>();
builder.Services.AddScoped<IStoryMongoDbRepository, StoryMongoDbRepository>();
builder.Services.AddScoped<ISearchSQLDbRepository,SearchSQLDbRepository>();
builder.Services.AddScoped<INotificationsSQLDbRepository, NotificationsSQLDbRepository>();

//Inyectando logger
builder.Services.AddLogging();

//Configurando JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true, // Verificar si el token ha caducado
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration["Jwt:SecretKey"])), // Clave secreta
            ClockSkew = TimeSpan.Zero // Sin margen de tiempo adicional

        };
    });

//Configurando GraphQL
builder.Services
    .AddGraphQLServer()
    .AddInMemorySubscriptions()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddType<UploadType>()
    .AddMutationConventions()
    .AddAuthorization();
  
var app = builder.Build();

app.UseWebSockets();
app.UseAuthentication();

app.MapGraphQL();
app.Run();
