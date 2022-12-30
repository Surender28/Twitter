using Twitter_new.Repositories;
using Microsoft.AspNetCore.Mvc;
using Twitter_new.DTOs;
using Microsoft.AspNetCore.Authorization;
using Twitter_new.Utilities;
using System.Security.Claims;
using Twitter_new.Models;

namespace Twitter_new.Controllers;

[ApiController]
[Authorize]
[Route("api/comment")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentRepository _comment;

    public CommentController(ILogger<CommentController> logger,
    ICommentRepository comment)
    {
        _logger = logger;
        _comment = comment;
    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }

    [HttpPost("post_id")]
    public async Task<ActionResult<Comment>> CreatePost([FromQuery] int post_id, [FromBody] CommentCreateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);
        int PostId = post_id;

        var toCreateItem = new Comment
        {
            CommentText = Data.CommentText.Trim(),
            PostId = PostId,
            UserId = userId,
        };

        var createdItem = await _comment.Create(toCreateItem);

        return StatusCode(201, createdItem);
    }


    [HttpDelete("{comment_id}")]
    public async Task<ActionResult> DeleteComment([FromRoute] int comment_id)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _comment.GetById(comment_id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(403, "You cannot delete other's Post");

        await _comment.Delete(comment_id);

        return NoContent();
    }

    [HttpGet("post_id")]
    public async Task<ActionResult<List<Comment>>> GetAllComments([FromQuery] int post_id)
    {
        var allComments = await _comment.GetAllComments(post_id);
        return Ok(allComments);
    }
}