namespace InfoExchange.Api.Features.Web;

public sealed class PageFilterRequest
{
  public int Page { get; set; } = 1;
  public int Pagesize { get; set; } = 10;
  public string Lable { get; set; } = string.Empty;
  public string Tag { get; set; } = string.Empty;
}

public sealed class IdRequest
{
  public string Id { get; set; } = string.Empty;
}

public sealed class ContentIdRequest
{
  public string Content_Id { get; set; } = string.Empty;
}

public sealed class CommentIdRequest
{
  public string Comment_Id { get; set; } = string.Empty;
}

public sealed class SetCommentRequest
{
  public string To_Userid { get; set; } = string.Empty;
  public string Content_Id { get; set; } = string.Empty;
  public string Contentname { get; set; } = string.Empty;
  public string Comment_Content { get; set; } = string.Empty;
  public string Router { get; set; } = string.Empty;
}

public sealed class SetReplyRequest
{
  public string To_Userid { get; set; } = string.Empty;
  public string Content_Id { get; set; } = string.Empty;
  public string Contentname { get; set; } = string.Empty;
  public string Comment_Id { get; set; } = string.Empty;
  public string Comment_Content { get; set; } = string.Empty;
  public string Tousernickname { get; set; } = string.Empty;
  public string Touserid { get; set; } = string.Empty;
  public string Router { get; set; } = string.Empty;
}

public sealed class SetJoinRequest
{
  public string Content_Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Describe { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public string To_Userid { get; set; } = string.Empty;
  public string Contentname { get; set; } = string.Empty;
}

public sealed class SearchRequest
{
  public string Search { get; set; } = string.Empty;
}

public sealed class NoticeRequest
{
  public string Num { get; set; } = string.Empty;
}

public sealed class ChangeNoticeRequest
{
  public string Change { get; set; } = string.Empty;
  public string Notice_Id { get; set; } = string.Empty;
}
public sealed class LableNameRequest
{
  public string Lable_Name { get; set; } = string.Empty;
}