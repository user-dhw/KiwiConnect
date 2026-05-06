using Dapper;
using InfoExchange.Api.Infrastructure.Data;
using InfoExchange.Api.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace InfoExchange.Api.Features.Admin;

public sealed class AdminService
{
  private readonly IDbConnectionFactory _connectionFactory;
  private readonly IMd5Hasher _md5Hasher;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly IConfiguration _configuration;

  public AdminService(
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

  public async Task<ApiResponse> LoginAsync(AdminLoginRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var salt = _configuration["Security:PasswordSalt"] ?? "adadadadad";
    var hashedPassword = _md5Hasher.Hash($"{request.Password}{salt}");

    var admin = await conn.QueryFirstOrDefaultAsync<AdminEntity>(
        "select * from admin where username=@Username and password=@Password",
        new { request.Username, Password = hashedPassword });

    if (admin is null)
    {
      return ApiResponse.Error("INVALID_CREDENTIALS", "Invalid username or password", code: 401);
    }

    var jurisdiction = new
    {
      isyh = admin.Isyh,
      isgl = admin.Isgl,
      issh = admin.Issh,
      isfk = admin.Isfk,
      user_state = admin.User_State
    };

    var token = _jwtTokenService.CreateAdminToken(admin.User_Id, admin.Username, jurisdiction);

    return ApiResponse.Success(new
    {
      token,
      userinfo = new
      {
        uid = admin.User_Id,
        nickname = admin.Nickname,
        username = admin.Username,
        jurisdiction
      }
    });
  }

  public async Task<ApiResponse> RegisterAsync(AdminRegisterRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var existing = await conn.QueryFirstOrDefaultAsync("select * from admin where username=@Username", new { request.Username });
    if (existing is not null)
    {
      return ApiResponse.Error("ERROR_PARAMS_EXIST", "Username already exists");
    }

    var salt = _configuration["Security:PasswordSalt"] ?? "adadadadad";
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;
    await conn.ExecuteAsync(
      @"insert into admin (user_id, username, password, nickname, user_createtime, isfk, isgl, issh, isyh, user_state)
        values (@UserId, @Username, @Password, @Nickname, @CreateTime, '0', '0', '0', '0', '1')",
      new
      {
        UserId = Guid.NewGuid().ToString(),
        request.Username,
        Password = _md5Hasher.Hash($"{request.Password}{salt}"),
        Nickname = "Administrator",
        CreateTime = now
      });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> GetAdminAsync(string uid)
  {
    using var conn = _connectionFactory.CreateConnection();
    var admin = await conn.QueryFirstOrDefaultAsync<AdminEntity>(
        "select * from admin where user_id=@Uid",
        new { Uid = uid });

    if (admin is null)
    {
      return ApiResponse.Error();
    }

    return ApiResponse.Success(admin);
  }

  public async Task<ApiResponse> NumberingAsync()
  {
    using var conn = _connectionFactory.CreateConnection();

    var user = await conn.QueryAsync(
      "select from_unixtime(user_createtime/1000, '%Y-%m') as time, count(user_id) as num from user group by time order by time");

    // Use COUNT(*) to get accurate row counts for content and comment tables
    var helpCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM help");
    var activityCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM activity");
    var oldstuffCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM oldstuff");
    var articleCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM article");
    var commentCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM comment");
    var replyCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM reply");

    // Build response data - includes 4 content tables + 2 comment tables
    var count = new[]
    {
      new { TABLE_NAME = "help", TABLE_ROWS = helpCount },
      new { TABLE_NAME = "activity", TABLE_ROWS = activityCount },
      new { TABLE_NAME = "oldstuff", TABLE_ROWS = oldstuffCount },
      new { TABLE_NAME = "article", TABLE_ROWS = articleCount },
      new { TABLE_NAME = "comment", TABLE_ROWS = commentCount },
      new { TABLE_NAME = "reply", TABLE_ROWS = replyCount }
    };

    return ApiResponse.Success(new { user, count });
  }

  public async Task<ApiResponse> ContentExamineAsync(AdminContentExamineRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();

    if (string.IsNullOrWhiteSpace(request.Type))
    {
      return ApiResponse.Error("MISSING_TYPE", "Type parameter missing");
    }

    if (!IsAllowedContentTable(request.Type))
    {
      return ApiResponse.Error("INVALID_TYPE", "Unsupported content type");
    }

    try
    {
      var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
      var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;
      var title = request.Type is "oldstuff" ? "name" : "title";

      var where = " where " + request.Type + ".user_id=user.user_id";
      var p = new DynamicParameters();

      if (!string.IsNullOrWhiteSpace(request.User))
      {
        where += " and user.nickname like @User";
        p.Add("User", $"%{request.User}%");
      }

      if (!string.IsNullOrWhiteSpace(request.Admin))
      {
        where += " and " + request.Type + ".admin like @Admin";
        p.Add("Admin", $"%{request.Admin}%");
      }

      if (!string.IsNullOrWhiteSpace(request.State))
      {
        where += " and " + request.Type + ".ispublic=@State";
        p.Add("State", request.State);
      }

      if (!string.IsNullOrWhiteSpace(request.Search))
      {
        where += " and " + request.Type + "." + request.Type + "_" + title + " like @Search";
        p.Add("Search", $"%{request.Search}%");
      }

      var countSql = "select count(*) from " + request.Type + ",user" + where;
      var listSql = "select * from " + request.Type + ",user" + where + " limit @PageSize offset @Offset";

      p.Add("PageSize", pageSize);
      p.Add("Offset", offset);

      var count = await conn.ExecuteScalarAsync<int>(countSql, p);
      var data = await conn.QueryAsync(listSql, p);

      return ApiResponse.Success(data, count);
    }
    catch (Exception ex)
    {
      return ApiResponse.Error("CONTENT_EXAMINE_FAILED", $"Query failed: {ex.Message}");
    }
  }

  public async Task<ApiResponse> ChangeContentStateAsync(string username, ChangeContentStateRequest request)
  {
    try
    {
      using var conn = _connectionFactory.CreateConnection();

      if (string.IsNullOrWhiteSpace(request.Type))
      {
        return ApiResponse.Error("MISSING_TYPE", "Content type parameter is missing or empty");
      }

      // Allow both content types and comment/reply types
      var isValidType = IsAllowedContentTable(request.Type) || request.Type is "comment" or "reply";

      if (!isValidType)
      {
        return ApiResponse.Error("INVALID_TYPE", $"Invalid type: '{request.Type}'. Expected one of: help, activity, oldstuff, article, comment, reply");
      }

      if (string.IsNullOrWhiteSpace(request.Id))
      {
        return ApiResponse.Error("MISSING_ID", "ID is missing or empty");
      }

      if (string.IsNullOrWhiteSpace(request.State))
      {
        return ApiResponse.Error("MISSING_STATE", "State parameter is missing or empty");
      }

      var sql = "update " + request.Type + " set ispublic=@State, admin=@Admin where " + request.Type + "_id=@Id";
      var rowsAffected = await conn.ExecuteAsync(sql, new { State = request.State, Admin = username, request.Id });

      if (rowsAffected == 0)
      {
        return ApiResponse.Error("NO_RECORDS_UPDATED", $"No records found or updated for {request.Type} with id {request.Id}");
      }

      return ApiResponse.Success();
    }
    catch (Exception ex)
    {
      return ApiResponse.Error("UPDATE_FAILED", $"Failed to update state: {ex.Message}");
    }
  }

  public async Task<ApiResponse> GetCommentAsync(AdminCommentListRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;
    var where = " where comment.user_id=user.user_id";
    var p = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(request.Admin))
    {
      where += " and comment.admin like @Admin";
      p.Add("Admin", $"%{request.Admin}%");
    }

    if (!string.IsNullOrWhiteSpace(request.State))
    {
      where += " and comment.ispublic=@State";
      p.Add("State", request.State);
    }

    var count = await conn.ExecuteScalarAsync<int>("select count(*) from user,comment" + where, p);
    p.Add("PageSize", pageSize);
    p.Add("Offset", offset);
    var data = await conn.QueryAsync("select * from user,comment" + where + " order by comment.comment_createtime desc limit @PageSize offset @Offset", p);

    return ApiResponse.Success(data, count);
  }

  public async Task<ApiResponse> GetReplyAsync(string commentId)
  {
    using var conn = _connectionFactory.CreateConnection();
    var data = await conn.QueryAsync(
      "select reply.reply_content, reply.createtime, reply.touserid, reply.tousernickname, user.user_id, user.avatar, user.nickname from user,reply where reply.user_id=user.user_id and comment_id=@CommentId order by reply.createtime asc",
      new { CommentId = commentId });
    return ApiResponse.Success(data);
  }

  public async Task<ApiResponse> DeleteUserAsync(DeleteUserRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    if (!IsAllowedDeleteUserTable(request.Usertype))
    {
      return ApiResponse.Error();
    }

    await conn.ExecuteAsync("delete from " + request.Usertype + " where user_id=@UserId", new { UserId = request.User_Id });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var salt = _configuration["Security:PasswordSalt"] ?? "adadadadad";
    var hashed = _md5Hasher.Hash($"{request.Newpassword}{salt}");

    if (request.Type == "adminadmin")
    {
      await conn.ExecuteAsync("update admin set password=@Password where username=@Username", new { Password = hashed, request.Username });
      return ApiResponse.Success();
    }
    if (request.Type == "adminuser")
    {
      await conn.ExecuteAsync("update user set password=@Password where username=@Username", new { Password = hashed, request.Username });
      return ApiResponse.Success();
    }

    return ApiResponse.Error();
  }

  public async Task<ApiResponse> ChangeAdminStateAsync(string requestUserName, ChangeAdminStateRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    if (requestUserName != "admin")
    {
      return ApiResponse.Forbidden();
    }

    await conn.ExecuteAsync(
      "update admin set isfk=@Isfk, isyh=@Isyh, isgl=@Isgl, issh=@Issh, user_state=@UserState where user_id=@UserId",
      new
      {
        request.Isfk,
        request.Isyh,
        request.Isgl,
        request.Issh,
        UserState = request.User_State,
        UserId = request.User_Id
      });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> ChangeUserUserStateAsync(string requestUserName, string requestIsyh, ChangeUserUserStateRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    if (!(requestIsyh == "1" || requestUserName == "admin"))
    {
      return ApiResponse.Forbidden();
    }

    await conn.ExecuteAsync("update user set user_state=@UserState where user_id=@UserId", new
    {
      UserState = request.User_State,
      UserId = request.User_Id
    });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> AdminDeleteAsync(AdminDeleteRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    if (!IsAllowedContentTable(request.Type) && request.Type != "comment" && request.Type != "reply")
    {
      return ApiResponse.Error();
    }

    await conn.ExecuteAsync("delete from " + request.Type + " where " + request.Type + "_id=@Id", new { request.Id });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> LableListAsync(LableListRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    if (string.IsNullOrWhiteSpace(request.Lable_Name))
    {
      var data = await conn.QueryAsync("select * from lable");
      return ApiResponse.Success(data);
    }
    var filtered = await conn.QueryAsync("select * from lable where lable_name=@LableName", new { LableName = request.Lable_Name });
    return ApiResponse.Success(filtered);
  }

  public async Task<ApiResponse> ChangeLableAsync(ChangeLableRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    // Explicitly map parameter names to avoid mismatch between anonymous object properties and SQL placeholders
    await conn.ExecuteAsync(
      "update lable set lable=@Lable where lable_id=@LableId",
      new { Lable = request.Lable, LableId = request.Lable_Id }
    );
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> ChangeCarouselAsync(ChangeCarouselRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;
    if (string.IsNullOrWhiteSpace(request.Carousel_Id))
    {
      await conn.ExecuteAsync(
        "insert into carousel (carousel_img, carousel_url, carousel_title, carousel_createtime, carousel_id) values (@Img,@Url,@Title,@CreateTime,@Id)",
        new
        {
          Img = request.Carousel_Img,
          Url = request.Carousel_Url,
          Title = request.Carousel_Title,
          CreateTime = now,
          Id = Guid.NewGuid().ToString()
        });
    }
    else
    {
      await conn.ExecuteAsync(
        "update carousel set carousel_img=@Img, carousel_url=@Url, carousel_title=@Title where carousel_id=@Id",
        new { Img = request.Carousel_Img, Url = request.Carousel_Url, Title = request.Carousel_Title, Id = request.Carousel_Id });
    }
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> CarouselListAsync()
  {
    using var conn = _connectionFactory.CreateConnection();
    var data = await conn.QueryAsync("select * from carousel order by carousel_createtime desc");
    return ApiResponse.Success(data);
  }

  public async Task<ApiResponse> DeleteCarouseAsync(DeleteCarouseRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("delete from carousel where carousel_id=@Id", new { Id = request.Carousel_Id });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> KefuListAsync(KefuListRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    if (!IsAllowedKefuTable(request.Kefu_Type))
    {
      return ApiResponse.Error();
    }

    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;
    var where = " where 1=1";
    var p = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(request.Id))
    {
      where += " and " + request.Kefu_Type + "_id=@Id";
      p.Add("Id", request.Id);
    }
    if (!string.IsNullOrWhiteSpace(request.State))
    {
      where += " and " + request.Kefu_Type + "_state=@State";
      p.Add("State", request.State);
    }

    var count = await conn.ExecuteScalarAsync<int>("select count(*) from " + request.Kefu_Type + where, p);
    p.Add("PageSize", pageSize);
    p.Add("Offset", offset);
    var data = await conn.QueryAsync("select * from " + request.Kefu_Type + where + " order by " + request.Kefu_Type + "_createtime desc limit @PageSize offset @Offset", p);

    return ApiResponse.Success(data, count);
  }

  public async Task<ApiResponse> ChangeKefuStateAsync(string username, ChangeKefuStateRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    if (!IsAllowedKefuTable(request.Type))
    {
      return ApiResponse.Error();
    }

    await conn.ExecuteAsync(
      "update " + request.Type + " set " + request.Type + "_state=@State, admin=@Admin where " + request.Type + "_id=@Id",
      new { State = request.Kefu_State, Admin = username, Id = request.Kefu_Id });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> ChangeResultAsync(string username, ChangeResultRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("update jubao set jubao_state=1, admin=@Admin, result=@Result where jubao_id=@JubaoId", new
    {
      Admin = username,
      request.Result,
      JubaoId = request.Jubao_Id
    });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> ChangeActivationDateAsync(ChangeActivationDateRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (long)request.Time * 24 * 60 * 60 * 1000;
    await conn.ExecuteAsync("update user set activationdate=@ActivationDate, jubao_id=@JubaoId where user_id=@UserId", new
    {
      ActivationDate = time,
      JubaoId = request.Jubao_Id,
      UserId = request.Userid
    });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> DeleteKefuAsync(DeleteKefuRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    if (!IsAllowedKefuTable(request.Type))
    {
      return ApiResponse.Error();
    }
    await conn.ExecuteAsync("delete from " + request.Type + " where " + request.Type + "_id=@Id", new { request.Id });
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> ChangeAdminUserAsync(ChangeAdminUserRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var salt = _configuration["Security:PasswordSalt"] ?? "adadadadad";

    if (string.IsNullOrWhiteSpace(request.Newpassword))
    {
      await conn.ExecuteAsync("update admin set nickname=@Nickname where user_id=@UserId", new { request.Nickname, UserId = request.User_Id });
    }
    else
    {
      var hashed = _md5Hasher.Hash($"{request.Newpassword}{salt}");
      await conn.ExecuteAsync("update admin set nickname=@Nickname, password=@Password where user_id=@UserId", new
      {
        request.Nickname,
        Password = hashed,
        UserId = request.User_Id
      });
    }
    return ApiResponse.Success();
  }

  public async Task<ApiResponse> SetAnnouncementAsync(string uid, string username, SetAnnouncementRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;
    await conn.ExecuteAsync(
      @"insert into announcement (announcement_id, announcement_name, announcement_content, announcement_type, content_id, announcement_createtime)
        values (@AnnouncementId, @AnnouncementName, @AnnouncementContent, @AnnouncementType, @ContentId, @CreateTime)",
      new
      {
        AnnouncementId = Guid.NewGuid().ToString(),
        AnnouncementName = request.Announcement_Name,
        AnnouncementContent = request.Announcement_Content,
        AnnouncementType = request.Type,
        ContentId = request.Content_Id,
        CreateTime = now
      });

    if (request.Type == "activity")
    {
      var userList = await conn.QueryAsync("select user_id from joins where content_id=@ContentId", new { ContentId = request.Content_Id });
      foreach (var u in userList)
      {
        var userTo = (string)u.user_id;
        await SetNoticeAsync(conn, uid, userTo, username, request.Content_Id, request.Contentname, "Published a new activity notification", "activitycontent");
      }
    }

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> AnnouncementListAsync(string contentId)
  {
    using var conn = _connectionFactory.CreateConnection();
    var data = await conn.QueryAsync("select * from announcement where content_id=@ContentId order by announcement_createtime desc", new { ContentId = contentId });
    return ApiResponse.Success(data);
  }

  public async Task<ApiResponse> GetUserListAsync(UserListRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;
    var where = " where 1=1";
    var p = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(request.User))
    {
      where += " and username like @User";
      p.Add("User", $"%{request.User}%");
    }
    if (!string.IsNullOrWhiteSpace(request.Realstate))
    {
      where += " and realstate=@Realstate";
      p.Add("Realstate", int.Parse(request.Realstate));
    }

    var countSql = "select count(*) from user" + where;
    var listSql = "select * from user" + where + " limit @PageSize offset @Offset";
    p.Add("PageSize", pageSize);
    p.Add("Offset", offset);

    var count = await conn.ExecuteScalarAsync<int>(countSql, p);
    var data = await conn.QueryAsync(listSql, p);

    return ApiResponse.Success(data, count);
  }

  public async Task<ApiResponse> GetAdminListAsync(AdminListRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;
    var where = " where 1=1";
    var p = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(request.User))
    {
      where += " and username like @User";
      p.Add("User", $"%{request.User}%");
    }
    if (!string.IsNullOrWhiteSpace(request.State))
    {
      where += " and user_state=@State";
      p.Add("State", request.State);
    }

    var countSql = "select count(*) from admin" + where;
    var listSql = "select * from admin" + where + " limit @PageSize offset @Offset";
    p.Add("PageSize", pageSize);
    p.Add("Offset", offset);

    var count = await conn.ExecuteScalarAsync<int>(countSql, p);
    var data = await conn.QueryAsync(listSql, p);

    return new ApiResponse
    {
      State = new ApiState { Type = "SUCCESS", Msg = "Operation successful" },
      Data = data,
      Count = count
    };
  }

  public async Task<ApiResponse> ChangeUserStateAsync(ChangeUserStateRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    var column = request.Type is "realstate" or "user_state" ? request.Type : string.Empty;
    if (string.IsNullOrWhiteSpace(column))
    {
      return ApiResponse.Error();
    }

    await conn.ExecuteAsync("update user set " + column + "=@State where user_id=@UserId", new
    {
      request.State,
      UserId = request.User_Id
    });

    return ApiResponse.Success();
  }

  private static bool IsAllowedContentTable(string type)
  {
    return type is "help" or "activity" or "oldstuff" or "article";
  }

  private static bool IsAllowedKefuTable(string type)
  {
    return type is "fankui" or "jubao" or "shensu";
  }

  private static bool IsAllowedDeleteUserTable(string type)
  {
    return type is "user" or "admin";
  }

  private static async Task SetNoticeAsync(
    System.Data.IDbConnection conn,
    string userFrom,
    string userTo,
    string nickname,
    string contentId,
    string contentName,
    string action,
    string router)
  {
    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;
    await conn.ExecuteAsync(
      @"insert into notice (notice_id, user_from, nickname, user_to, content_name, action, content_id, router, createtime, state)
        values (@NoticeId, @UserFrom, @Nickname, @UserTo, @ContentName, @Action, @ContentId, @Router, @CreateTime, 0)",
      new
      {
        NoticeId = Guid.NewGuid().ToString(),
        UserFrom = userFrom,
        Nickname = nickname,
        UserTo = userTo,
        ContentName = contentName,
        Action = action,
        ContentId = contentId,
        Router = router,
        CreateTime = now
      });
  }
}
