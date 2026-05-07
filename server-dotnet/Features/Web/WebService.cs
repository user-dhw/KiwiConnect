using System.Text;
using Dapper;
using InfoExchange.Api.Infrastructure.Data;

namespace InfoExchange.Api.Features.Web;

public sealed class WebService
{
  private readonly IDbConnectionFactory _connectionFactory;

  public WebService(IDbConnectionFactory connectionFactory)
  {
    _connectionFactory = connectionFactory;
  }

  public async Task<ApiResponse> WebGetWebHelpListAsync(PageFilterRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;

    var where = new StringBuilder(" where help.ispublic=1");
    var countWhere = new StringBuilder(" where ispublic=1");
    var p = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(request.Lable))
    {
      where.Append(" and help.help_lable=@Lable");
      countWhere.Append(" and help_lable=@Lable");
      p.Add("Lable", request.Lable);
    }

    if (!string.IsNullOrWhiteSpace(request.Tag))
    {
      where.Append(" and help.help_tag like @Tag");
      countWhere.Append(" and help_tag like @Tag");
      p.Add("Tag", $"%{request.Tag}%");
    }

    var countSql = $"select count(*) from help{countWhere}";
    var listSql = $@"select help.help_id,help.help_title,help.createtime,help.help_read_num,user.nickname
                         from help,user
                         {where} and help.user_id=user.user_id
                         order by help.help_read_num desc
                         limit @PageSize offset @Offset";

    p.Add("PageSize", pageSize);
    p.Add("Offset", offset);

    var count = await conn.ExecuteScalarAsync<int>(countSql, p);
    var list = await conn.QueryAsync(listSql, p);

    return ApiResponse.Success(list, count);
  }

  public async Task<ApiResponse> GetHelpContentAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    var item = await conn.QueryFirstOrDefaultAsync(
        "select * from help,user where help.user_id=user.user_id and help_id=@Id",
        new { Id = id });

    await conn.ExecuteAsync("update help set help_read_num=help_read_num+1 where help_id=@Id", new { Id = id });

    return ApiResponse.FromNullable(item);
  }

  public async Task<ApiResponse> GetArticleContentAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("update article set article_read_num=article_read_num+1 where article_id=@Id", new { Id = id });
    var item = await conn.QueryFirstOrDefaultAsync(
        "select * from article,user where article.user_id=user.user_id and article_id=@Id",
        new { Id = id });

    return ApiResponse.FromNullable(item);
  }



  public async Task<ApiResponse> WebGetWebActivityListAsync(PageFilterRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;

    var where = new StringBuilder(" where activity.ispublic=1");
    var countWhere = new StringBuilder(" where ispublic=1");
    var p = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(request.Lable))
    {
      where.Append(" and activity.activity_lable=@Lable");
      countWhere.Append(" and activity_lable=@Lable");
      p.Add("Lable", request.Lable);
    }

    var countSql = $"select count(*) from activity{countWhere}";
    var listSql = $@"select * from activity,user
                         {where} and activity.user_id=user.user_id
                         order by activity.activity_read_num desc
                         limit @PageSize offset @Offset";

    p.Add("PageSize", pageSize);
    p.Add("Offset", offset);

    var count = await conn.ExecuteScalarAsync<int>(countSql, p);
    var list = await conn.QueryAsync(listSql, p);

    return ApiResponse.Success(list, count);
  }

  public async Task<ApiResponse> WebGetWebOldStuffListAsync(PageFilterRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;

    var where = new StringBuilder(" where oldstuff.ispublic=1");
    var countWhere = new StringBuilder(" where ispublic=1");
    var p = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(request.Lable))
    {
      where.Append(" and oldstuff.oldstuff_lable=@Lable");
      countWhere.Append(" and oldstuff_lable=@Lable");
      p.Add("Lable", request.Lable);
    }

    var countSql = $"select count(*) from oldstuff{countWhere}";
    var listSql = $@"select * from oldstuff,user
                         {where} and oldstuff.user_id=user.user_id
                         order by oldstuff.oldstuff_read_num desc
                         limit @PageSize offset @Offset";

    p.Add("PageSize", pageSize);
    p.Add("Offset", offset);

    var count = await conn.ExecuteScalarAsync<int>(countSql, p);
    var list = await conn.QueryAsync(listSql, p);

    return ApiResponse.Success(list, count);
  }

  public async Task<ApiResponse> GetArticleListAsync(PageFilterRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();
    var pageSize = request.Pagesize <= 0 ? 10 : request.Pagesize;
    var offset = ((request.Page <= 0 ? 1 : request.Page) - 1) * pageSize;

    var where = new StringBuilder(" where article.ispublic=1");
    var countWhere = new StringBuilder(" where ispublic=1");
    var p = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(request.Lable))
    {
      where.Append(" and article.article_lable=@Lable");
      countWhere.Append(" and article_lable=@Lable");
      p.Add("Lable", request.Lable);
    }

    var countSql = $"select count(*) from article{countWhere}";
    var listSql = $@"select article.article_read_num,article.article_id,article.article_title,article.article_introduction,
                                article.article_createtime,user.nickname
                         from article,user
                         {where} and article.user_id=user.user_id
                         order by article.article_read_num desc
                         limit @PageSize offset @Offset";

    p.Add("PageSize", pageSize);
    p.Add("Offset", offset);

    var count = await conn.ExecuteScalarAsync<int>(countSql, p);
    var list = await conn.QueryAsync(listSql, p);

    return ApiResponse.Success(list, count);
  }

  public async Task<ApiResponse> GetOldStuffContentAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("update oldstuff set oldstuff_read_num=oldstuff_read_num+1 where oldstuff_id=@Id", new { Id = id });
    var item = await conn.QueryFirstOrDefaultAsync(
        "select * from oldstuff,user where user.user_id=oldstuff.user_id and oldstuff.oldstuff_id=@Id",
        new { Id = id });

    return ApiResponse.FromNullable(item);
  }

  public async Task<ApiResponse> GetActivityContentAsync(string id)
  {
    using var conn = _connectionFactory.CreateConnection();
    await conn.ExecuteAsync("update activity set activity_read_num=activity_read_num+1 where activity_id=@Id", new { Id = id });
    var item = await conn.QueryFirstOrDefaultAsync(
        "select * from activity,user where user.user_id=activity.user_id and activity.activity_id=@Id",
        new { Id = id });

    return ApiResponse.FromNullable(item);
  }



  public async Task<ApiResponse> GetCommentAsync(string contentId)
  {
    using var conn = _connectionFactory.CreateConnection();
    var count = await conn.ExecuteScalarAsync<int>("select count(*) from comment where content_id=@ContentId", new { ContentId = contentId });
    var list = await conn.QueryAsync(
        "select * from user,comment where comment.user_id=user.user_id and content_id=@ContentId order by comment.comment_createtime asc",
        new { ContentId = contentId });

    return ApiResponse.Success(list, count);
  }

  public async Task<ApiResponse> GetReplyAsync(string commentId)
  {
    using var conn = _connectionFactory.CreateConnection();
    var list = await conn.QueryAsync(
        "select * from user,reply where reply.user_id=user.user_id and comment_id=@CommentId order by reply.createtime asc",
        new { CommentId = commentId });

    return ApiResponse.Success(list);
  }

  public async Task<ApiResponse> SetCommentAsync(string uid, string nickname, SetCommentRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();

    // Check if user is verified (realstate must be 3)
    var user = await conn.QueryFirstOrDefaultAsync<dynamic>(
        "select realstate from user where user_id=@Uid",
        new { Uid = uid });

    if (user == null || user.realstate != 3)
    {
      return ApiResponse.Error(msg: "Only verified users can comment on posts");
    }

    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await SetNoticeAsync(conn, uid, request.To_Userid, nickname, request.Content_Id, request.Contentname, "commented on your post", request.Router);
    await conn.ExecuteAsync(
        @"insert into comment
              (comment_id, user_id, content_id, comment_content, comment_createtime, comment_state, comment_istop, ispublic)
              values
              (@CommentId, @UserId, @ContentId, @CommentContent, @CreateTime, 0, 0, 0)",
        new
        {
          CommentId = Guid.NewGuid().ToString(),
          UserId = uid,
          ContentId = request.Content_Id,
          CommentContent = request.Comment_Content,
          CreateTime = now
        });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> SetReplyAsync(string uid, string nickname, SetReplyRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();

    // Check if user is verified (realstate must be 3)
    var user = await conn.QueryFirstOrDefaultAsync<dynamic>(
        "select realstate from user where user_id=@Uid",
        new { Uid = uid });

    if (user == null || user.realstate != 3)
    {
      return ApiResponse.Error(msg: "Only verified users can reply to comments");
    }

    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await SetNoticeAsync(conn, uid, request.To_Userid, nickname, request.Content_Id, request.Contentname, "replied to you", request.Router);
    await conn.ExecuteAsync(
        @"insert into reply
              (reply_id, user_id, comment_id, reply_content, tousernickname, touserid, createtime, reply_state, reply_istop, ispublic)
              values
              (@ReplyId, @UserId, @CommentId, @ReplyContent, @ToUserNickname, @ToUserId, @CreateTime, 0, 0, 0)",
        new
        {
          ReplyId = Guid.NewGuid().ToString(),
          UserId = uid,
          CommentId = request.Comment_Id,
          ReplyContent = request.Comment_Content,
          ToUserNickname = request.Tousernickname,
          ToUserId = request.Touserid,
          CreateTime = now
        });

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> GetNoticeAsync(string uid, string num)
  {
    using var conn = _connectionFactory.CreateConnection();
    var unreadCount = await conn.ExecuteScalarAsync<int>("select count(*) from notice where user_to=@Uid and state=0", new { Uid = uid });
    var totalCount = await conn.ExecuteScalarAsync<int>("select count(*) from notice where user_to=@Uid", new { Uid = uid });

    IEnumerable<dynamic>? list = null;
    if (string.IsNullOrWhiteSpace(num))
    {
      list = await conn.QueryAsync("select * from notice where user_to=@Uid order by createtime desc", new { Uid = uid });
    }

    return ApiResponse.Success(new { list, count = unreadCount, num = totalCount });
  }

  public async Task<ApiResponse> ChangeNoticeAsync(string uid, ChangeNoticeRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();

    switch (request.Change)
    {
      case "change":
        await conn.ExecuteAsync("update notice set state=1 where user_to=@Uid and notice_id=@NoticeId", new { Uid = uid, NoticeId = request.Notice_Id });
        break;
      case "delete":
        await conn.ExecuteAsync("delete from notice where user_to=@Uid and notice_id=@NoticeId", new { Uid = uid, NoticeId = request.Notice_Id });
        break;
      case "changeall":
        await conn.ExecuteAsync("update notice set state=1 where user_to=@Uid", new { Uid = uid });
        break;
      case "deleteall":
        await conn.ExecuteAsync("delete from notice where user_to=@Uid", new { Uid = uid });
        break;
    }

    return ApiResponse.Success();
  }

  public async Task<ApiResponse> SetJoinAsync(string uid, SetJoinRequest request)
  {
    using var conn = _connectionFactory.CreateConnection();

    // Check if user is verified (realstate must be 3)
    var user = await conn.QueryFirstOrDefaultAsync<dynamic>(
        "select realstate from user where user_id=@Uid",
        new { Uid = uid });

    if (user == null || user.realstate != 3)
    {
      return ApiResponse.Error(msg: "Only verified users can join activities");
    }

    var exists = await conn.QueryFirstOrDefaultAsync(
        "select * from joins where content_id=@ContentId and user_id=@Uid",
        new { ContentId = request.Content_Id, Uid = uid });

    if (exists is not null)
    {
      return ApiResponse.Error();
    }

    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 8 * 60 * 60;

    await SetNoticeAsync(conn, string.Empty, uid, "System notification", request.Content_Id, request.Contentname, "You have successfully joined", request.Type);
    await SetNoticeAsync(conn, string.Empty, request.To_Userid, "System notification", request.Content_Id, request.Contentname, "A user has joined your published content", request.Type);

    await conn.ExecuteAsync(
      @"insert into joins (join_id, user_id, type, jions_createtime, name, `describe`, content_id)
              values (@JoinId, @UserId, @Type, @CreateTime, @Name, @Describe, @ContentId)",
        new
        {
          JoinId = Guid.NewGuid().ToString(),
          UserId = uid,
          request.Type,
          CreateTime = now,
          request.Name,
          Describe = request.Describe,
          ContentId = request.Content_Id
        });

    return ApiResponse.Success();
  }
  // The search endpoint currently performs simple fuzzy matching
  public async Task<ApiResponse> SearchAsync(string search)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(search))
      {
        // Return empty data structure even if empty, not an error
        return ApiResponse.Success(new
        {
          help = new object[] { },
          activity = new object[] { },
          oldstuff = new object[] { },
          article = new object[] { }
        });
      }

      using var conn = _connectionFactory.CreateConnection();
      var key = $"%{search}%";

      var help = new object[] { };
      var activity = new object[] { };
      var oldstuff = new object[] { };
      var article = new object[] { };

      // 对每个表分别进行错误处理
      try
      {
        help = (await conn.QueryAsync(
          @"select * from help,user
            where help.user_id=user.user_id
              and (
                help.help_title like @Key
                or help.help_content like @Key
                or help.help_tag like @Key
              )",
          new { Key = key })).ToArray();
      }
      catch (Exception)
      {
      }

      try
      {
        activity = (await conn.QueryAsync(
          @"select * from activity
            where activity.activity_title like @Key
               or activity.activity_content like @Key
               or activity.activity_locale like @Key",
          new { Key = key })).ToArray();
      }
      catch (Exception)
      {
      }



      try
      {
        oldstuff = (await conn.QueryAsync(
          @"select * from oldstuff
            where oldstuff_name like @Key
               or oldstuff_content like @Key
               or oldstuff_lable like @Key",
          new { Key = key })).ToArray();
      }
      catch (Exception)
      {
      }

      try
      {
        article = (await conn.QueryAsync(
          @"select * from article,user
            where article.user_id=user.user_id
              and (
                article.article_title like @Key
                or article.article_introduction like @Key
                or article.article_content like @Key
              )",
          new { Key = key })).ToArray();
      }
      catch (Exception)
      {
      }

      return ApiResponse.Success(new { help, activity, oldstuff, article });
    }
    catch (Exception)
    {
      // 返回空数据结构而不是错误
      return ApiResponse.Success(new
      {
        help = new object[] { },
        activity = new object[] { },
        oldstuff = new object[] { },
        article = new object[] { }
      });
    }
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

  public async Task<ApiResponse> CarouselListAsync()
  {
    using var conn = _connectionFactory.CreateConnection();
    var data = await conn.QueryAsync("select * from carousel order by carousel_createtime desc");
    return ApiResponse.Success(data);
  }

  public async Task<ApiResponse> AnnouncementListAsync(string contentId)
  {
    using var conn = _connectionFactory.CreateConnection();
    var data = await conn.QueryAsync(
        "select * from announcement where content_id=@ContentId order by announcement_createtime desc",
        new { ContentId = contentId });
    return ApiResponse.Success(data);
  }

  public async Task<ApiResponse> GetLableListAsync(string lableName)
  {
    try
    {
      using var conn = _connectionFactory.CreateConnection();

      if (string.IsNullOrWhiteSpace(lableName))
      {
        var allData = await conn.QueryAsync("select * from lable");
        return ApiResponse.Success(allData);
      }

      var filtered = await conn.QueryAsync("select * from lable where lable_name=@LableName", new { LableName = lableName });
      return ApiResponse.Success(filtered);
    }
    catch (Exception ex)
    {
      return ApiResponse.Error("GET_LABLE_FAILED", $"获取标签列表失败: {ex.Message}");
    }
  }

}
