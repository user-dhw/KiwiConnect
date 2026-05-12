using System.Security.Claims;
using InfoExchange.Api.Infrastructure.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoExchange.Api.Features.WebAdmin;

[ApiController]
public sealed class WebAdminController : ControllerBase
{
  private readonly WebAdminService _service;

  public WebAdminController(WebAdminService service)
  {
    _service = service;
  }

  [HttpPost("/webadmin/registered")]
  [AllowAnonymous]
  public async Task<IActionResult> Registered([FromForm] RegisterRequest request)
  {
    var result = await _service.RegisterAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/login")]
  [AllowAnonymous]
  [Consumes("application/json")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    return await LoginInternal(request);
  }

  [HttpPost("/webadmin/login")]
  [AllowAnonymous]
  [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
  public async Task<IActionResult> LoginForm([FromForm] LoginRequest request)
  {
    return await LoginInternal(request);
  }

  private async Task<IActionResult> LoginInternal(LoginRequest request)
  {
    var result = await _service.LoginAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getuser")]
  [Authorize]
  public async Task<IActionResult> GetUser()
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    if (string.IsNullOrWhiteSpace(uid))
    {
      return this.FromApiResponse(ApiResponse.Unauthorized());
    }

    var result = await _service.GetUserAsync(uid);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/updatauser")]
  [Authorize]
  public async Task<IActionResult> UpdataUser([FromForm] UpdateUserRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.UpdateUserAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getusernumbering")]
  [Authorize]
  public async Task<IActionResult> GetUserNumbering()
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.GetUserNumberingAsync(uid);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/createhelp")]
  [Authorize]
  public async Task<IActionResult> CreateHelp([FromForm] CreateHelpRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.CreateHelpAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getwebhelplist")]
  [Authorize]
  public async Task<IActionResult> GetWebHelpList([FromForm] PagingRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.GetWebHelpListAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/gethelpdetails")]
  [Authorize]
  public async Task<IActionResult> GetHelpDetails([FromForm] IdRequest request)
  {
    var result = await _service.GetHelpDetailsAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/updateehelp")]
  [Authorize]
  public async Task<IActionResult> UpdateHelp([FromForm] UpdateHelpRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.UpdateHelpAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/deletehelp")]
  [Authorize]
  public async Task<IActionResult> DeleteHelp([FromForm] HelpIdRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.DeleteHelpAsync(uid, request.Help_Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/createactivity")]
  [Authorize]
  public async Task<IActionResult> CreateActivity([FromForm] CreateActivityRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.CreateActivityAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getwebactivitylist")]
  [Authorize]
  public async Task<IActionResult> GetWebActivityList([FromForm] PagingRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.GetWebActivityListAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getactivitydetails")]
  [Authorize]
  public async Task<IActionResult> GetActivityDetails([FromForm] IdRequest request)
  {
    var result = await _service.GetActivityDetailsAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/updateactivity")]
  [Authorize]
  public async Task<IActionResult> UpdateActivity([FromForm] UpdateActivityRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.UpdateActivityAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/deleteactivity")]
  [Authorize]
  public async Task<IActionResult> DeleteActivity([FromForm] ActivityIdRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.DeleteActivityAsync(uid, request.Activity_Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/createoldstuff")]
  [Authorize]
  public async Task<IActionResult> CreateOldstuff([FromForm] CreateOldstuffRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.CreateOldstuffAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getweboldstufflist")]
  [Authorize]
  public async Task<IActionResult> GetWebOldstuffList([FromForm] PagingRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.GetWebOldstuffListAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getoldstuffdetails")]
  [Authorize]
  public async Task<IActionResult> GetOldstuffDetails([FromForm] IdRequest request)
  {
    var result = await _service.GetOldstuffDetailsAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/updateoldstuff")]
  [Authorize]
  public async Task<IActionResult> UpdateOldstuff([FromForm] UpdateOldstuffRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.UpdateOldstuffAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/deleteoldstuff")]
  [Authorize]
  public async Task<IActionResult> DeleteOldstuff([FromForm] OldstuffIdRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.DeleteOldstuffAsync(uid, request.Oldstuff_Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/createarticle")]
  [Authorize]
  public async Task<IActionResult> CreateArticle([FromForm] CreateArticleRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.CreateArticleAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/articlelist")]
  [Authorize]
  public async Task<IActionResult> ArticleList([FromForm] PagingRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.ArticleListAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getarticledetails")]
  [Authorize]
  public async Task<IActionResult> GetArticleDetails([FromForm] IdRequest request)
  {
    var result = await _service.GetArticleDetailsAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/updatearticle")]
  [Authorize]
  public async Task<IActionResult> UpdateArticle([FromForm] UpdateArticleRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.UpdateArticleAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/deletearticle")]
  [Authorize]
  public async Task<IActionResult> DeleteArticle([FromForm] ArticleIdRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.DeleteArticleAsync(uid, request.Article_Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/deletejoin")]
  [Authorize]
  public async Task<IActionResult> DeleteJoin([FromForm] JoinIdRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.DeleteJoinAsync(uid, request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/joinslist")]
  [Authorize]
  public async Task<IActionResult> JoinsList([FromForm] JoinsListRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.JoinsListAsync(uid, request.Type);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/getwebjoinslist")]
  [Authorize]
  public async Task<IActionResult> GetWebJoinsList([FromForm] GetWebJoinsListRequest request)
  {
    var result = await _service.GetWebJoinsListAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/setannouncement")]
  [Authorize]
  public async Task<IActionResult> SetAnnouncement([FromForm] SetAnnouncementRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.SetAnnouncementAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/createfankui")]
  [Authorize]
  public async Task<IActionResult> CreateFankui([FromForm] CreateFankuiRequest request)
  {
    var result = await _service.CreateFankuiAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/createjubao")]
  [AllowAnonymous]
  public async Task<IActionResult> CreateJubao([FromForm] CreateJubaoRequest request)
  {
    var result = await _service.CreateJubaoAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/createshensu")]
  [AllowAnonymous]
  public async Task<IActionResult> CreateShensu([FromForm] CreateShensuRequest request)
  {
    var result = await _service.CreateShensuAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/webadmin/jubaocontent")]
  [AllowAnonymous]
  public async Task<IActionResult> JubaoContent([FromForm] IdRequest request)
  {
    var result = await _service.JubaoContentAsync(request.Id);
    return this.FromApiResponse(result);
  }
}
