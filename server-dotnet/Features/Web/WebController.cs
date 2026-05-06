using System.Security.Claims;
using InfoExchange.Api.Infrastructure.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoExchange.Api.Features.Web;

[ApiController]
public sealed class WebController : ControllerBase
{
  private readonly WebService _service;

  public WebController(WebService service)
  {
    _service = service;
  }

  [HttpPost("/web/webgetwebhelplist")]
  [AllowAnonymous]
  public async Task<IActionResult> WebGetWebHelpList([FromForm] PageFilterRequest request)
  {
    var result = await _service.WebGetWebHelpListAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/gethelpcontent")]
  [AllowAnonymous]
  public async Task<IActionResult> GetHelpContent([FromForm] IdRequest request)
  {
    var result = await _service.GetHelpContentAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/setcomment")]
  [Authorize]
  public async Task<IActionResult> SetComment([FromForm] SetCommentRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var nickname = User.FindFirstValue("nickname") ?? string.Empty;
    var result = await _service.SetCommentAsync(uid, nickname, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/getcomment")]
  [AllowAnonymous]
  public async Task<IActionResult> GetComment([FromForm] ContentIdRequest request)
  {
    var result = await _service.GetCommentAsync(request.Content_Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/setreply")]
  [Authorize]
  public async Task<IActionResult> SetReply([FromForm] SetReplyRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var nickname = User.FindFirstValue("nickname") ?? string.Empty;
    var result = await _service.SetReplyAsync(uid, nickname, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/getreply")]
  [AllowAnonymous]
  public async Task<IActionResult> GetReply([FromForm] CommentIdRequest request)
  {
    var result = await _service.GetReplyAsync(request.Comment_Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/webgetwebactivitylist")]
  [AllowAnonymous]
  public async Task<IActionResult> WebGetWebActivityList([FromForm] PageFilterRequest request)
  {
    var result = await _service.WebGetWebActivityListAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/webgetweboldstufflist")]
  [AllowAnonymous]
  public async Task<IActionResult> WebGetWebOldStuffList([FromForm] PageFilterRequest request)
  {
    var result = await _service.WebGetWebOldStuffListAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/getarticlelist")]
  [AllowAnonymous]
  public async Task<IActionResult> GetArticleList([FromForm] PageFilterRequest request)
  {
    var result = await _service.GetArticleListAsync(request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/getarticlecontent")]
  [AllowAnonymous]
  public async Task<IActionResult> GetArticleContent([FromForm] IdRequest request)
  {
    var result = await _service.GetArticleContentAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/getoldstuffcontent")]
  [AllowAnonymous]
  public async Task<IActionResult> GetOldStuffContent([FromForm] IdRequest request)
  {
    var result = await _service.GetOldStuffContentAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/getactivitycontent")]
  [AllowAnonymous]
  public async Task<IActionResult> GetActivityContent([FromForm] IdRequest request)
  {
    var result = await _service.GetActivityContentAsync(request.Id);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/setjoin")]
  [Authorize]
  public async Task<IActionResult> SetJoin([FromForm] SetJoinRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.SetJoinAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/search")]
  [AllowAnonymous]
  public async Task<IActionResult> Search([FromForm] SearchRequest request)
  {
    var result = await _service.SearchAsync(request.Search);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/getnotice")]
  [Authorize]
  public async Task<IActionResult> GetNotice([FromForm] NoticeRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.GetNoticeAsync(uid, request.Num);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/changenotice")]
  [Authorize]
  public async Task<IActionResult> ChangeNotice([FromForm] ChangeNoticeRequest request)
  {
    var uid = User.FindFirstValue("uid") ?? string.Empty;
    var result = await _service.ChangeNoticeAsync(uid, request);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/getlablelist")]
  [AllowAnonymous]
  public async Task<IActionResult> GetLableList([FromForm] LableNameRequest request)
  {
    var result = await _service.GetLableListAsync(request.Lable_Name);
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/carousellist")]
  [AllowAnonymous]
  public async Task<IActionResult> CarouselList()
  {
    var result = await _service.CarouselListAsync();
    return this.FromApiResponse(result);
  }

  [HttpPost("/web/announcementlist")]
  [AllowAnonymous]
  public async Task<IActionResult> AnnouncementList([FromForm] ContentIdRequest request)
  {
    var result = await _service.AnnouncementListAsync(request.Content_Id);
    return this.FromApiResponse(result);
  }
}