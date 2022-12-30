namespace Twitter_new;

public record Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int UserId { get; set; }
}
