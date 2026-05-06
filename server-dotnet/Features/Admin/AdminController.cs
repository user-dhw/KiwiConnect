using System.Security.Claims;
using InfoExchange.Api.Infrastructure.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoExchange.Api.Features.Admin;

[ApiController]
public sealed class AdminController : ControllerBase
{
  private readonly AdminService _service;

  public AdminController(AdminService service)
  {
    _service = service;
  }

  private bool HasPermission(string claimName)
  {
    var username = User.FindFirstValue("username") ?? string.Empty;
    if (username == "admin")
    {
      return true;
    }

    return (User.FindFirstValue(claimName) ?? string.Empty) == "1";
  }

  private IActionResult NoAuthority()
  {
    return this.FromApiResponse(ApiResponse.Forbidden());
  }

  [HttpPost("/admin/login")]
  [AllowAnonymous]
  [Consumes("application/json")]
  public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
  {
    return await LoginInternal(request);
  }

  [HttpPost("/admin/login")]
  [AllowAnonymous]
  [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
  public async Task<IActionResult> LoginForm([FromForm] AdminLoginRequest request)
  {
    return await LoginInternal(request);
  }

  private async Task<IActionResult> LoginInternal(AdminLoginRequest request)
  {
    var result = await _service.LoginAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/registered")]
  [Authorize]
  public async Task<IActionResult> Registered([FromForm] AdminRegisterRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.RegisterAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/getadmin")]
  [Authorize]
  public async Task<IActionResult> GetAdmin()
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    if (string.IsNullOrWhiteSpace(uid))
    {
      return this.FromApiResponse(ApiResponse.Unauthorized());
    }

    var result = await _service.GetAdminAsync(uid);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/numbering")]
  [Authorize]
  public async Task<IActionResult> Numbering()
  {
    var result = await _service.NumberingAsync();
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/contentexamine")]
  [Authorize]
  [Consumes("application/json")]
  public async Task<IActionResult> ContentExamine([FromBody] AdminContentExamineRequest request)
  {
    return await ContentExamineInternal(request);
  }

  [HttpPost("/admin/contentexamine")]
  [Authorize]
  [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
  public async Task<IActionResult> ContentExamineForm([FromForm] AdminContentExamineRequest request)
  {
    return await ContentExamineInternal(request);
  }

  private async Task<IActionResult> ContentExamineInternal(AdminContentExamineRequest request)
  {
    if (!HasPermission("issh")) return NoAuthority();
    var result = await _service.ContentExamineAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changecontentstate")]
  [Authorize]
  public async Task<IActionResult> ChangeContentState([FromForm] ChangeContentStateRequest request)
  {
    if (!HasPermission("issh")) return NoAuthority();
    var username = User.FindFirstValue("username") ?? string.Empty;
    var result = await _service.ChangeContentStateAsync(username, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/getcomment")]
  [Authorize]
  public async Task<IActionResult> GetComment([FromForm] AdminCommentListRequest request)
  {
    if (!HasPermission("issh")) return NoAuthority();
    var result = await _service.GetCommentAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/getreply")]
  [Authorize]
  public async Task<IActionResult> GetReply([FromForm] CommentIdRequest request)
  {
    if (!HasPermission("issh")) return NoAuthority();
    var result = await _service.GetReplyAsync(request.Comment_Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/getuserlist")]
  [Authorize]
  public async Task<IActionResult> GetUserList([FromForm] UserListRequest request)
  {
    if (!HasPermission("isyh")) return NoAuthority();
    var result = await _service.GetUserListAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/getadminlist")]
  [Authorize]
  public async Task<IActionResult> GetAdminList([FromForm] AdminListRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.GetAdminListAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changeuserstate")]
  [Authorize]
  public async Task<IActionResult> ChangeUserState([FromForm] ChangeUserStateRequest request)
  {
    if (!HasPermission("isyh")) return NoAuthority();
    var result = await _service.ChangeUserStateAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/deleteuser")]
  [Authorize]
  public async Task<IActionResult> DeleteUser([FromForm] DeleteUserRequest request)
  {
    if (!HasPermission("isyh")) return NoAuthority();
    var result = await _service.DeleteUserAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changepassword")]
  [Authorize]
  public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.ChangePasswordAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changeadminstate")]
  [Authorize]
  public async Task<IActionResult> ChangeAdminState([FromForm] ChangeAdminStateRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var username = User.FindFirstValue("username") ?? string.Empty;
    var result = await _service.ChangeAdminStateAsync(username, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changeuseruserstate")]
  [Authorize]
  public async Task<IActionResult> ChangeUserUserState([FromForm] ChangeUserUserStateRequest request)
  {
    if (!HasPermission("isyh")) return NoAuthority();
    var username = User.FindFirstValue("username") ?? string.Empty;
    var isyh = User.FindFirstValue("isyh") ?? string.Empty;
    var result = await _service.ChangeUserUserStateAsync(username, isyh, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/admindelete")]
  [Authorize]
  public async Task<IActionResult> AdminDelete([FromForm] AdminDeleteRequest request)
  {
    if (!HasPermission("issh")) return NoAuthority();
    var result = await _service.AdminDeleteAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/lablelist")]
  [Authorize]
  public async Task<IActionResult> LableList([FromForm] LableListRequest request)
  {
    var result = await _service.LableListAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changelable")]
  [Authorize]
  public async Task<IActionResult> ChangeLable([FromForm] ChangeLableRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.ChangeLableAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changecarousel")]
  [Authorize]
  public async Task<IActionResult> ChangeCarousel([FromForm] ChangeCarouselRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.ChangeCarouselAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/carousellist")]
  [Authorize]
  public async Task<IActionResult> CarouselList()
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.CarouselListAsync();
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/deletecarouse")]
  [Authorize]
  public async Task<IActionResult> DeleteCarouse([FromForm] DeleteCarouseRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.DeleteCarouseAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/kefullist")]
  [Authorize]
  public async Task<IActionResult> KefuList([FromForm] KefuListRequest request)
  {
    if (!HasPermission("isfk")) return NoAuthority();
    var result = await _service.KefuListAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changkefustate")]
  [Authorize]
  public async Task<IActionResult> ChangeKefuState([FromForm] ChangeKefuStateRequest request)
  {
    if (!HasPermission("isfk")) return NoAuthority();
    var username = User.FindFirstValue("username") ?? string.Empty;
    var result = await _service.ChangeKefuStateAsync(username, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changresult")]
  [Authorize]
  public async Task<IActionResult> ChangeResult([FromForm] ChangeResultRequest request)
  {
    if (!HasPermission("isfk")) return NoAuthority();
    var username = User.FindFirstValue("username") ?? string.Empty;
    var result = await _service.ChangeResultAsync(username, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changeactivationdate")]
  [Authorize]
  public async Task<IActionResult> ChangeActivationDate([FromForm] ChangeActivationDateRequest request)
  {
    if (!HasPermission("isfk")) return NoAuthority();
    var result = await _service.ChangeActivationDateAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/deletekefu")]
  [Authorize]
  public async Task<IActionResult> DeleteKefu([FromForm] DeleteKefuRequest request)
  {
    if (!HasPermission("isfk")) return NoAuthority();
    var result = await _service.DeleteKefuAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/changeadminuser")]
  [Authorize]
  public async Task<IActionResult> ChangeAdminUser([FromForm] ChangeAdminUserRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.ChangeAdminUserAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/setannouncement")]
  [Authorize]
  public async Task<IActionResult> SetAnnouncement([FromForm] SetAnnouncementRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var username = User.FindFirstValue("username") ?? string.Empty;
    var result = await _service.SetAnnouncementAsync(uid, username, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/admin/announcementlist")]
  [Authorize]
  public async Task<IActionResult> AnnouncementList([FromForm] AnnouncementListRequest request)
  {
    if (!HasPermission("isgl")) return NoAuthority();
    var result = await _service.AnnouncementListAsync(request.Content_Id);
    return this.FromApiResponse(result);
  }
}
