using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Twitter_new.DTOs;



public record UserDTO
{
    [Required]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [Required]
    [JsonPropertyName("full_name")]
    public string FullName { get; set; }
    [Required]
    [JsonPropertyName("email")]
    public string Email { get; set; }


}


public record UserCreateDTO
{
    [JsonPropertyName("full_name")]
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string FullName { get; set; }

    [JsonPropertyName("email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [JsonPropertyName("password")]
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Password { get; set; }


}

public record UserLoginDTO
{
    [JsonPropertyName("email")]
    [EmailAddress]
    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [JsonPropertyName("password")]
    [MaxLength(255)]
    public string Password { get; set; }
}

public record UserLoginResDTO
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("fullname")]
    public string FullName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }


    [JsonPropertyName("id")]
    public int Id { get; set; }
}

public record UserUpdateDTO
{

    [JsonPropertyName("full_name")]
    [Required]
    public string FullName { get; set; }


}