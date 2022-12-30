using Twitter_new.DTOs;

namespace Twitter_new.Models;

public record Comment

{
    public int Id { get; set; }
    public string CommentText { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
}