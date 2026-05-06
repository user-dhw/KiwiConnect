using System.ComponentModel.DataAnnotations;

namespace InfoExchange.Api.Features.WebAdmin;

public sealed class RegisterRequest
{
  [Required]
  public string Username { get; set; } = string.Empty;
  [Required]
  public string Password { get; set; } = string.Empty;
}

public sealed class LoginRequest
{
  [Required]
  public string Username { get; set; } = string.Empty;
  [Required]
  public string Password { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
}

public sealed class UserEntity
{
  public int Id { get; set; }
  public string User_Id { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string Nickname { get; set; } = string.Empty;
  public string Avatar { get; set; } = string.Empty;
  public int Realstate { get; set; }
  public long? Activationdate { get; set; }
  public string? Jubao_Id { get; set; }
  public string? Realname { get; set; } = string.Empty;
  public string? Studentid { get; set; } = string.Empty;
  public string? Studentcard { get; set; } = string.Empty;
  public string? Mail { get; set; } = string.Empty;
  public string? Phone { get; set; } = string.Empty;
  public string? Synopsis { get; set; } = string.Empty;
}

public sealed class PagingRequest
{
  public int Page { get; set; } = 1;
  public int Pagesize { get; set; } = 10;
}

public sealed class IdRequest
{
  public string Id { get; set; } = string.Empty;
}

public sealed class HelpIdRequest
{
  public string Help_Id { get; set; } = string.Empty;
}

public sealed class ActivityIdRequest
{
  public string Activity_Id { get; set; } = string.Empty;
}

public sealed class OldstuffIdRequest
{
  public string Oldstuff_Id { get; set; } = string.Empty;
}

public sealed class JoinIdRequest
{
  public string Id { get; set; } = string.Empty;
}

public sealed class CreateHelpRequest
{
  public string Help_Title { get; set; } = string.Empty;
  public string Help_Lable { get; set; } = string.Empty;
  public string Help_Tag { get; set; } = string.Empty;
  public string Help_Content { get; set; } = string.Empty;
}

public sealed class UpdateHelpRequest
{
  public string Id { get; set; } = string.Empty;
  public string Help_Title { get; set; } = string.Empty;
  public string Help_Lable { get; set; } = string.Empty;
  public string Help_Content { get; set; } = string.Empty;
}

public sealed class CreateActivityRequest
{
  public string Activity_Title { get; set; } = string.Empty;
  public string Activity_Lable { get; set; } = string.Empty;
  public string Activity_Type { get; set; } = string.Empty;
  public string Activity_Content { get; set; } = string.Empty;
  public string Activity_Locale { get; set; } = string.Empty;
  public string Activity_Impose { get; set; } = string.Empty;
  public string Activity_Num { get; set; } = string.Empty;
  public string Activity_Statetime { get; set; } = string.Empty;
  public string Activity_Endtime { get; set; } = string.Empty;
}

public sealed class UpdateActivityRequest
{
  public string Id { get; set; } = string.Empty;
  public string Activity_Title { get; set; } = string.Empty;
  public string Activity_Lable { get; set; } = string.Empty;
  public string Activity_Content { get; set; } = string.Empty;
  public string Activity_Locale { get; set; } = string.Empty;
  public string Activity_Type { get; set; } = string.Empty;
  public string Activity_Impose { get; set; } = string.Empty;
  public string Activity_Num { get; set; } = string.Empty;
  public string Activity_Statetime { get; set; } = string.Empty;
  public string Activity_Endtime { get; set; } = string.Empty;
}

public sealed class CreateOldstuffRequest
{
  public string Oldstuff_Img { get; set; } = string.Empty;
  public string Oldstuff_Name { get; set; } = string.Empty;
  public string Oldstuff_Lable { get; set; } = string.Empty;
  public string Oldstuff_Price { get; set; } = string.Empty;
  public string Oldstuff_Content { get; set; } = string.Empty;
}

public sealed class UpdateOldstuffRequest
{
  public string Oldstuff_Id { get; set; } = string.Empty;
  public string Oldstuff_Img { get; set; } = string.Empty;
  public string Oldstuff_Name { get; set; } = string.Empty;
  public string Oldstuff_Price { get; set; } = string.Empty;
  public string Oldstuff_Content { get; set; } = string.Empty;
  public string Oldstuff_Lable { get; set; } = string.Empty;
}

public sealed class CreateArticleRequest
{
  public string Article_Title { get; set; } = string.Empty;
  public string Article_Introduction { get; set; } = string.Empty;
  public string Article_Lable { get; set; } = string.Empty;
  public string Article_Content { get; set; } = string.Empty;
}

public sealed class UpdateArticleRequest
{
  public string Article_Id { get; set; } = string.Empty;
  public string Article_Title { get; set; } = string.Empty;
  public string Article_Lable { get; set; } = string.Empty;
  public string Article_Content { get; set; } = string.Empty;
  public string Article_Introduction { get; set; } = string.Empty;
}

public sealed class ArticleIdRequest
{
  public string Article_Id { get; set; } = string.Empty;
}

public sealed class UpdateUserRequest
{
  public string M { get; set; } = string.Empty;
  public string Realname { get; set; } = string.Empty;
  public string Studentid { get; set; } = string.Empty;
  public string Studentcard { get; set; } = string.Empty;
  public string Avatar { get; set; } = string.Empty;
  public string Nickname { get; set; } = string.Empty;
  public string Synopsis { get; set; } = string.Empty;
  public string Mail { get; set; } = string.Empty;
  public string Phone { get; set; } = string.Empty;
}

public sealed class JoinsListRequest
{
  public string Type { get; set; } = string.Empty;
}

public sealed class GetWebJoinsListRequest
{
  public string Id { get; set; } = string.Empty;
}

public sealed class CreateFankuiRequest
{
  public string Fankui_Content { get; set; } = string.Empty;
  public string Fankui_User { get; set; } = string.Empty;
}

public sealed class CreateJubaoRequest
{
  public string Jubao_Content { get; set; } = string.Empty;
  public string Jubao_User { get; set; } = string.Empty;
  public string Jubao_Img { get; set; } = string.Empty;
  public string Jubao_Url { get; set; } = string.Empty;
}

public sealed class CreateShensuRequest
{
  public string Shensu_Content { get; set; } = string.Empty;
  public string Shensu_User { get; set; } = string.Empty;
  public string Shensu_Jubao_Id { get; set; } = string.Empty;
}
