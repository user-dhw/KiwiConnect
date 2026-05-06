using System.ComponentModel.DataAnnotations;

namespace InfoExchange.Api.Features.Admin;

public sealed class AdminLoginRequest
{
  [Required]
  public string Username { get; set; } = string.Empty;
  [Required]
  public string Password { get; set; } = string.Empty;
}

public sealed class AdminEntity
{
  public string User_Id { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string Nickname { get; set; } = "Administrator";
  public string Isyh { get; set; } = "0";
  public string Isgl { get; set; } = "0";
  public string Issh { get; set; } = "0";
  public string Isfk { get; set; } = "0";
  public string User_State { get; set; } = "1";
}

public sealed class AdminContentExamineRequest
{
  public string Type { get; set; } = string.Empty;
  public string User { get; set; } = string.Empty;
  public string Admin { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public string Search { get; set; } = string.Empty;
  public int Page { get; set; } = 1;
  public int Pagesize { get; set; } = 10;
}

public sealed class ChangeContentStateRequest
{
  public string Type { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public string Id { get; set; } = string.Empty;
}

public sealed class CommentIdRequest
{
  public string Comment_Id { get; set; } = string.Empty;
}

public sealed class UserListRequest
{
  public string User { get; set; } = string.Empty;
  public string Realstate { get; set; } = string.Empty;
  public int Page { get; set; } = 1;
  public int Pagesize { get; set; } = 10;
}

public sealed class AdminListRequest
{
  public string User { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public int Page { get; set; } = 1;
  public int Pagesize { get; set; } = 10;
}

public sealed class ChangeUserStateRequest
{
  public string Type { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public string User_Id { get; set; } = string.Empty;
}

public sealed class AdminRegisterRequest
{
  [Required]
  public string Username { get; set; } = string.Empty;
  [Required]
  public string Password { get; set; } = string.Empty;
}

public sealed class ChangePasswordRequest
{
  public string Type { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;
  public string Newpassword { get; set; } = string.Empty;
}

public sealed class ChangeAdminStateRequest
{
  public string Isfk { get; set; } = string.Empty;
  public string Isyh { get; set; } = string.Empty;
  public string Isgl { get; set; } = string.Empty;
  public string Issh { get; set; } = string.Empty;
  public string User_State { get; set; } = string.Empty;
  public string User_Id { get; set; } = string.Empty;
}

public sealed class ChangeUserUserStateRequest
{
  public string User_State { get; set; } = string.Empty;
  public string User_Id { get; set; } = string.Empty;
}

public sealed class DeleteUserRequest
{
  public string Usertype { get; set; } = string.Empty;
  public string User_Id { get; set; } = string.Empty;
}

public sealed class AdminDeleteRequest
{
  public string Type { get; set; } = string.Empty;
  public string Id { get; set; } = string.Empty;
}

public sealed class LableListRequest
{
  public string Lable_Name { get; set; } = string.Empty;
}

public sealed class ChangeLableRequest
{
  public string Lable { get; set; } = string.Empty;
  public string Lable_Id { get; set; } = string.Empty;
}

public sealed class ChangeCarouselRequest
{
  public string Carousel_Id { get; set; } = string.Empty;
  public string Carousel_Img { get; set; } = string.Empty;
  public string Carousel_Title { get; set; } = string.Empty;
}

public sealed class DeleteCarouseRequest
{
  public string Carousel_Id { get; set; } = string.Empty;
}

public sealed class KefuListRequest
{
  public string Kefu_Type { get; set; } = string.Empty;
  public string Id { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public int Page { get; set; } = 1;
  public int Pagesize { get; set; } = 10;
}

public sealed class ChangeKefuStateRequest
{
  public string Type { get; set; } = string.Empty;
  public string Kefu_State { get; set; } = string.Empty;
  public string Kefu_Id { get; set; } = string.Empty;
}

public sealed class ChangeResultRequest
{
  public string Jubao_Id { get; set; } = string.Empty;
  public string Result { get; set; } = string.Empty;
}

public sealed class ChangeActivationDateRequest
{
  public int Time { get; set; }
  public string Jubao_Id { get; set; } = string.Empty;
  public string Userid { get; set; } = string.Empty;
}

public sealed class DeleteKefuRequest
{
  public string Type { get; set; } = string.Empty;
  public string Id { get; set; } = string.Empty;
}

public sealed class ChangeAdminUserRequest
{
  public string User_Id { get; set; } = string.Empty;
  public string Nickname { get; set; } = string.Empty;
  public string Newpassword { get; set; } = string.Empty;
}

public sealed class SetAnnouncementRequest
{
  public string Announcement_Name { get; set; } = string.Empty;
  public string Announcement_Content { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public string Content_Id { get; set; } = string.Empty;
  public string Contentname { get; set; } = string.Empty;
}

public sealed class AnnouncementListRequest
{
  public string Content_Id { get; set; } = string.Empty;
}

public sealed class AdminCommentListRequest
{
  public string Admin { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public int Page { get; set; } = 1;
  public int Pagesize { get; set; } = 10;
}
