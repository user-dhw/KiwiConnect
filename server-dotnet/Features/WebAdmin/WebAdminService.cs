using Dapper;
using InfoExchange.Api.Infrastructure.Data;
using InfoExchange.Api.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace InfoExchange.Api.Features.WebAdmin;

public sealed class WebAdminService
{
  private readonly IDbConnectionFactory _connectionFactory;
  private readonly IMd5Hasher _md5Hasher;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly IConfiguration _configuration;

  public WebAdminService(
      IDbConnectionFactory connectionFactory,
      IMd5Hasher md5Hasher,
      IJwtTokenService jwtTokenService,
      IConfiguration configuration)
  {
    _connectionFactory = connectionFactory;
    _md5Hasher = md5Hasher;
    _jwtTokenService = jwtTokenService;
    _configuration = configuration;
  }

  public async Task<ApiResponse> RegisterAsync(RegisterRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var existing = await conn.QueryFirstOrDefaultAsync<UserEntity>(
        "select * from user where username = @Username",
        new { request.Username });

    if (existing is not null)
    {
      return ApiResponse.Error("ERROR_PARAMS_EXIST", "Username already exists");
    }

    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;
    var salt = _configuration["Security:PasswordSalt"] ?? "adadadadad";

    var sql = @"insert into user
                    (user_id, username, password, user_createtime, nickname, avatar, realstate, user_state)
                    values
                    (@UserId, @Username, @Password, @CreateTime, @Nickname, @Avatar, @RealState, @UserState)";

    await conn.ExecuteAsync(sql, new
    {
      UserId = Guid.NewGuid().ToString(),
      request.Username,
      Password = _md5Hasher.Hash($"{request.Password}{salt}"),
      CreateTime = now,
      Nickname = "This user has not set a nickname yet",
      Avatar = "/uplodes/avatar.jpg",
      RealState = 1,
      UserState = 2
    });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> LoginAsync(LoginRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var salt = _configuration["Security:PasswordSalt"] ?? "adadadadad";
    var hashedPassword = _md5Hasher.Hash($"{request.Password}{salt}");

    var user = await conn.QueryFirstOrDefaultAsync<UserEntity>(
        "select * from user where username=@Username and password=@Password",
        new { request.Username, Password = hashedPassword });

    if (user is null)
    {
      return ApiResponse.Error("INVALID_CREDENTIALS", "Invalid username or password", code: 401);
    }

    if (request.Type == string.Empty && user.Activationdate.HasValue && user.Activationdate.Value > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
    {
      return ApiResponse.Error(
        "ACCOUNT_SUSPENDED",
        "Your account is currently suspended due to violations",
        data: user.Activationdate.Value);
    }

    var token = _jwtTokenService.CreateUserToken(user.User_Id, user.Nickname);

    return ApiResponse.Success(new
    {
      shensu = new
      {
        activationdate = user.Activationdate,
        jubao_id = user.Jubao_Id,
        username = user.Username
      },
      token,
      userinfo = new
      {
        uid = user.Id,
        nickname = user.Nickname,
        avatar = user.Avatar,
        realstate = user.Realstate
      }
    });
  }

  public async Task<ApiResponse> GetUserAsync(string uid)
  {
    using var conn = _connectionFactory.CreateConnection();
    var user = await conn.QueryFirstOrDefaultAsync<UserEntity>(
        "select * from user where user_id=@Uid",
        new { Uid = uid });

    if (user is null)
    {
      return ApiResponse.NotFound("User not found");
    }

    return ApiResponse.Success(user);
  }

  public async Task<ApiResponse> GetUserNumberingAsync(string uid)
  {
    using var conn = _connectionFactory.CreateConnection();

    var help = await conn.ExecuteScalarAsync<int>("select count(*) from help where user_id=@Uid", new { Uid = uid });
    var article = await conn.ExecuteScalarAsync<int>("select count(*) from article where user_id=@Uid", new { Uid = uid });
    var activity = await conn.ExecuteScalarAsync<int>("select count(*) from activity where user_id=@Uid", new { Uid = uid });
    var oldstuff = await conn.ExecuteScalarAsync<int>("select count(*) from oldstuff where user_id=@Uid", new { Uid = uid });

    return ApiResponse.Success(new { article, help, activity, oldstuff });
  }

  public async Task<ApiResponse> CreateHelpAsync(string uid, CreateHelpRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await conn.ExecuteAsync(
      @"insert into help
        (help_id, user_id, help_title, help_lable, help_tag, help_content, createtime, updatetime,
         help_favour_num, help_read_num, help_state, help_istop, ispublic)
        values
        (@HelpId, @UserId, @HelpTitle, @HelpLable, @HelpTag, @HelpContent, @CreateTime, @UpdateTime,
         0, 0, 0, 0, 0)",
      new
      {
        HelpId = Guid.NewGuid().ToString(),
        UserId = uid,
        HelpTitle = request.Help_Title,
        HelpLable = request.Help_Lable,
        HelpTag = request.Help_Tag,
        HelpContent = request.Help_Content,
        CreateTime = now,
        UpdateTime = now
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> GetWebHelpListAsync(string uid, PagingRequest request)
  {
    return await GetOwnPagedListAsync(uid, request, "help");
  }

  public async Task<ApiResponse> GetHelpDetailsAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    var item = await conn.QueryFirstOrDefaultAsync("select * from help where help_id=@Id", new { Id = id });
    return ApiResponse.FromNullable(item);
  }

  public async Task<ApiResponse> UpdateHelpAsync(string uid, UpdateHelpRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync(
      "update help set help_title=@HelpTitle, help_lable=@HelpLable, help_content=@HelpContent where user_id=@Uid and help_id=@Id",
      new
      {
        HelpTitle = request.Help_Title,
        HelpLable = request.Help_Lable,
        HelpContent = request.Help_Content,
        Uid = uid,
        request.Id
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> DeleteHelpAsync(string uid, string helpId)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("delete from help where help_id=@HelpId and user_id=@Uid", new { HelpId = helpId, Uid = uid });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> CreateActivityAsync(string uid, CreateActivityRequest request)
  {
    try
    {
      using var conn = _connectionFactory.CreateConnection();

      // Validate required fields
      if (string.IsNullOrWhiteSpace(request.Activity_Title))
      {
        return ApiResponse.Error("MISSING_TITLE", "Activity title cannot be empty");
      }

      if (string.IsNullOrWhiteSpace(request.Activity_Type))
      {
        return ApiResponse.Error("MISSING_TYPE", "Activity type cannot be empty");
      }

      if (string.IsNullOrWhiteSpace(request.Activity_Lable))
      {
        return ApiResponse.Error("MISSING_LABLE", "Activity category cannot be empty");
      }

      if (string.IsNullOrWhiteSpace(request.Activity_Locale))
      {
        return ApiResponse.Error("MISSING_LOCALE", "Location cannot be empty");
      }

      if (string.IsNullOrWhiteSpace(request.Activity_Statetime) || !long.TryParse(request.Activity_Statetime, out var startTime))
      {
        return ApiResponse.Error("INVALID_STARTTIME", "Invalid start time format");
      }

      if (string.IsNullOrWhiteSpace(request.Activity_Endtime) || !long.TryParse(request.Activity_Endtime, out var endTime))
      {
        return ApiResponse.Error("INVALID_ENDTIME", "Invalid end time format");
      }

      if (startTime >= endTime)
      {
        return ApiResponse.Error("INVALID_TIME_RANGE", "Start time must be before end time");
      }

      var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

      var rowsAffected = await conn.ExecuteAsync(
        @"insert into activity
          (activity_id, user_id, activity_title, activity_lable, activity_type, activity_content, activity_locale,
           activity_impose, createtime, activity_num, activity_statetime, activity_endtime, updatetime,
           activity_favour_num, activity_read_num, activity_state, activity_istop, ispublic)
          values
          (@ActivityId, @UserId, @ActivityTitle, @ActivityLable, @ActivityType, @ActivityContent, @ActivityLocale,
           @ActivityImpose, @CreateTime, @ActivityNum, @ActivityStatetime, @ActivityEndtime, @UpdateTime,
           0, 0, 0, 0, 0)",
        new
        {
          ActivityId = Guid.NewGuid().ToString(),
          UserId = uid,
          ActivityTitle = request.Activity_Title,
          ActivityLable = request.Activity_Lable,
          ActivityType = request.Activity_Type,
          ActivityContent = request.Activity_Content,
          ActivityLocale = request.Activity_Locale,
          ActivityImpose = request.Activity_Impose,
          ActivityNum = request.Activity_Num,
          ActivityStatetime = request.Activity_Statetime,
          ActivityEndtime = request.Activity_Endtime,
          CreateTime = now,
          UpdateTime = now
        });

      if (rowsAffected == 0)
      {
        return ApiResponse.Error("INSERT_FAILED", "Failed to insert activity record");
      }

      return ApiResponse.Success();
    }
    catch (Exception ex)
    {
      return ApiResponse.Error("CREATE_ACTIVITY_FAILED", $"Failed to create activity: {ex.Message}");
    }
  }

  public async Task<ApiResponse> GetWebActivityListAsync(string uid, PagingRequest request)
  {
    return await GetOwnPagedListAsync(uid, request, "activity");
  }

  public async Task<ApiResponse> GetActivityDetailsAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    var item = await conn.QueryFirstOrDefaultAsync("select * from activity where activity_id=@Id", new { Id = id });
    return ApiResponse.FromNullable(item);
  }

  public async Task<ApiResponse> UpdateActivityAsync(string uid, UpdateActivityRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var updateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await conn.ExecuteAsync(
      @"update activity
        set activity_title=@ActivityTitle, activity_lable=@ActivityLable, activity_content=@ActivityContent,
            activity_locale=@ActivityLocale, activity_type=@ActivityType, activity_impose=@ActivityImpose,
            activity_num=@ActivityNum, activity_statetime=@ActivityStatetime, activity_endtime=@ActivityEndtime,
            updatetime=@UpdateTime
        where user_id=@Uid and activity_id=@Id",
      new
      {
        ActivityTitle = request.Activity_Title,
        ActivityLable = request.Activity_Lable,
        ActivityContent = request.Activity_Content,
        ActivityLocale = request.Activity_Locale,
        ActivityType = request.Activity_Type,
        ActivityImpose = request.Activity_Impose,
        ActivityNum = request.Activity_Num,
        ActivityStatetime = request.Activity_Statetime,
        ActivityEndtime = request.Activity_Endtime,
        UpdateTime = updateTime,
        Uid = uid,
        request.Id
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> DeleteActivityAsync(string uid, string activityId)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("delete from activity where activity_id=@ActivityId and user_id=@Uid", new { ActivityId = activityId, Uid = uid });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> CreateOldstuffAsync(string uid, CreateOldstuffRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await conn.ExecuteAsync(
      @"insert into oldstuff
        (oldstuff_id, user_id, oldstuff_img, oldstuff_name, oldstuff_lable, oldstuff_price, oldstuff_content,
         createtime, updatetime, oldstuff_favour_num, oldstuff_read_num, oldstuff_state, oldstuff_istop, ispublic)
        values
        (@OldstuffId, @UserId, @OldstuffImg, @OldstuffName, @OldstuffLable, @OldstuffPrice, @OldstuffContent,
         @CreateTime, @UpdateTime, 0, 0, 0, 0, 0)",
      new
      {
        OldstuffId = Guid.NewGuid().ToString(),
        UserId = uid,
        OldstuffImg = request.Oldstuff_Img,
        OldstuffName = request.Oldstuff_Name,
        OldstuffLable = request.Oldstuff_Lable,
        OldstuffPrice = request.Oldstuff_Price,
        OldstuffContent = request.Oldstuff_Content,
        CreateTime = now,
        UpdateTime = now
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> GetWebOldstuffListAsync(string uid, PagingRequest request)
  {
    return await GetOwnPagedListAsync(uid, request, "oldstuff");
  }

  public async Task<ApiResponse> GetOldstuffDetailsAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    var item = await conn.QueryFirstOrDefaultAsync("select * from oldstuff where oldstuff_id=@Id", new { Id = id });
    return ApiResponse.FromNullable(item);
  }

  public async Task<ApiResponse> UpdateOldstuffAsync(string uid, UpdateOldstuffRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync(
      @"update oldstuff
        set oldstuff_img=@OldstuffImg, oldstuff_name=@OldstuffName, oldstuff_price=@OldstuffPrice,
            oldstuff_content=@OldstuffContent, oldstuff_lable=@OldstuffLable
        where user_id=@Uid and oldstuff_id=@OldstuffId",
      new
      {
        OldstuffImg = request.Oldstuff_Img,
        OldstuffName = request.Oldstuff_Name,
        OldstuffPrice = request.Oldstuff_Price,
        OldstuffContent = request.Oldstuff_Content,
        OldstuffLable = request.Oldstuff_Lable,
        Uid = uid,
        request.Oldstuff_Id
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> DeleteOldstuffAsync(string uid, string oldstuffId)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("delete from oldstuff where oldstuff_id=@OldstuffId and user_id=@Uid", new { OldstuffId = oldstuffId, Uid = uid });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> CreateArticleAsync(string uid, CreateArticleRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await conn.ExecuteAsync(
      @"insert into article
        (article_id, user_id, article_title, article_introduction, article_lable, article_content,
         article_createtime, article_updatetime, article_favour_num, article_read_num, article_state, article_istop, ispublic)
        values
        (@ArticleId, @UserId, @ArticleTitle, @ArticleIntroduction, @ArticleLable, @ArticleContent,
         @CreateTime, @UpdateTime, 0, 0, 0, 0, 0)",
      new
      {
        ArticleId = Guid.NewGuid().ToString(),
        UserId = uid,
        ArticleTitle = request.Article_Title,
        ArticleIntroduction = request.Article_Introduction,
        ArticleLable = request.Article_Lable,
        ArticleContent = request.Article_Content,
        CreateTime = now,
        UpdateTime = now
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> ArticleListAsync(string uid, PagingRequest request)
  {
    return await GetOwnPagedListAsync(uid, request, "article");
  }

  public async Task<ApiResponse> GetArticleDetailsAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    var item = await conn.QueryFirstOrDefaultAsync("select * from article where article_id=@Id", new { Id = id });
    return ApiResponse.FromNullable(item);
  }

  public async Task<ApiResponse> UpdateArticleAsync(string uid, UpdateArticleRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync(
      @"update article
        set article_title=@ArticleTitle, article_lable=@ArticleLable, article_content=@ArticleContent,
            article_introduction=@ArticleIntroduction
        where user_id=@Uid and article_id=@ArticleId",
      new
      {
        ArticleTitle = request.Article_Title,
        ArticleLable = request.Article_Lable,
        ArticleContent = request.Article_Content,
        ArticleIntroduction = request.Article_Introduction,
        Uid = uid,
        request.Article_Id
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> DeleteArticleAsync(string uid, string articleId)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync(
      "delete from article where article_id=@ArticleId and user_id=@Uid",
      new { ArticleId = articleId, Uid = uid });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> DeleteJoinAsync(string uid, string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("delete from joins where join_id=@Id and user_id=@Uid", new { Id = id, Uid = uid });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> UpdateUserAsync(string uid, UpdateUserRequest request)
  {
    try
    {
      using var conn = _connectionFactory.CreateConnection();

      if (request.M == "student")
      {
        await conn.ExecuteAsync(
          "update user set realname=@Realname, studentid=@Studentid, studentcard=@Studentcard, realstate=2 where user_id=@Uid",
          new { request.Realname, request.Studentid, request.Studentcard, Uid = uid });
        return ApiResponse.Success();
      }

      if (request.M == "user")
      {
        var affectedRows = await conn.ExecuteAsync(
          "update user set avatar=@Avatar, nickname=@Nickname, synopsis=@Synopsis, mail=@Mail, phone=@Phone where user_id=@Uid",
          new { request.Avatar, request.Nickname, request.Synopsis, request.Mail, request.Phone, Uid = uid });

        if (affectedRows == 1)
        {
          var user = await conn.QueryFirstOrDefaultAsync<UserEntity>("select * from user where user_id=@Uid", new { Uid = uid });
          return ApiResponse.Success(new
          {
            userinfo = new
            {
              uid = user?.Id,
              nickname = user?.Nickname,
              avatar = user?.Avatar,
              realstate = user?.Realstate
            }
          });
        }

        return ApiResponse.NotFound("User not found");
      }

      // If M doesn't match any expected value, return detailed error
      return ApiResponse.Error("INVALID_M_FIELD", $"Invalid m parameter: '{request.M}'. Expected 'student' or 'user'.");
    }
    catch (Exception ex)
    {
      return ApiResponse.Error("UPDATE_FAILED", $"Failed to update user information: {ex.Message}");
    }
  }

  public async Task<ApiResponse> JoinsListAsync(string uid, string type)
  {
    using var conn = _connectionFactory.CreateConnection();
    string sql;

    if (type == "oldstuffcontent")
    {
      sql = "select * from joins,oldstuff where oldstuff.oldstuff_id=joins.content_id and joins.user_id=@Uid";
    }
    else if (type == "activitycontent")
    {
      sql = "select * from joins,activity where activity.activity_id=joins.content_id and joins.user_id=@Uid";
    }
    else
    {
      return ApiResponse.NotFound("No matching records were found");
    }

    var list = await conn.QueryAsync(sql, new { Uid = uid });
    return ApiResponse.Success(list);
  }

  public async Task<ApiResponse> GetWebJoinsListAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    var list = await conn.QueryAsync(
      "select * from joins,user where user.user_id=joins.user_id and joins.content_id=@Id",
      new { Id = id });

    return list.Any() ? ApiResponse.Success(list) : ApiResponse.NotFound("No matching records were found");
  }

  public async Task<ApiResponse> CreateFankuiAsync(CreateFankuiRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await conn.ExecuteAsync(
      @"insert into fankui (fankui_id, fankui_content, fankui_user, fankui_createtime, fankui_state)
        values (@FankuiId, @FankuiContent, @FankuiUser, @CreateTime, 0)",
      new
      {
        FankuiId = Guid.NewGuid().ToString(),
        FankuiContent = request.Fankui_Content,
        FankuiUser = request.Fankui_User,
        CreateTime = now
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> CreateJubaoAsync(CreateJubaoRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await conn.ExecuteAsync(
      @"insert into jubao (jubao_id, jubao_content, jubao_user, jubao_img, jubao_url, jubao_createtime, jubao_state)
        values (@JubaoId, @JubaoContent, @JubaoUser, @JubaoImg, @JubaoUrl, @CreateTime, 0)",
      new
      {
        JubaoId = Guid.NewGuid().ToString(),
        JubaoContent = request.Jubao_Content,
        JubaoUser = request.Jubao_User,
        JubaoImg = request.Jubao_Img,
        JubaoUrl = request.Jubao_Url,
        CreateTime = now
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> CreateShensuAsync(CreateShensuRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await conn.ExecuteAsync(
      @"insert into shensu (shensu_id, shensu_content, shensu_user, shensu_jubao_id, shensu_createtime, shensu_state)
        values (@ShensuId, @ShensuContent, @ShensuUser, @ShensuJubaoId, @CreateTime, 0)",
      new
      {
        ShensuId = Guid.NewGuid().ToString(),
        ShensuContent = request.Shensu_Content,
        ShensuUser = request.Shensu_User,
        ShensuJubaoId = request.Shensu_Jubao_Id,
        CreateTime = now
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> JubaoContentAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    var item = await conn.QueryFirstOrDefaultAsync("select * from jubao where jubao_id=@Id", new { Id = id });
    return ApiResponse.FromNullable(item);
  }

  private async Task<ApiResponse> GetOwnPagedListAsync(string uid, PagingRequest request, string tableName)
  {
    using var conn = _connectionFactory.CreateConnection();
    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;

    var count = await conn.ExecuteScalarAsync<int>($"select count(*) from {tableName} where user_id=@Uid", new { Uid = uid });
    var list = await conn.QueryAsync($"select * from {tableName} where user_id=@Uid limit @PageSize offset @Offset", new
    {
      Uid = uid,
      PageSize = pageSize,
      Offset = offset
    });

    return new ApiResponse
    {
      State = new ApiState { Type = "SUCCESS", Msg = "Operation successful" },
      Data = list,
      Count = count
    };
  }
}
