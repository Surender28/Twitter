using System.ComponentModel.DataAnnotations;

namespace Twitter_new.DTOs;

public record PostCreateDTO
{
  [Required]
  [MinLength(3)]
  [MaxLength(90)]
  public string Title { get; set; }


}

public record PostUpdateDTO
{
  [MinLength(3)]
  [MaxLength(255)]
  public string Title { get; set; } = null;

}