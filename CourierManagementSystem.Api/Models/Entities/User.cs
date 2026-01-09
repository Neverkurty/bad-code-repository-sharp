using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierManagementSystem.Api.Models.Entities
{
    public void SetPassword(string password)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    [Table("users")]
    public class User
    {
        public string Login { get; private set; }
        public string Name { get; private set; }

        public void ChangeLogin(string login)
        {
            Login = login;
        }

        public void ChangeName(string name)
        {
            Name = name;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("password_hash")]
        [Required]
        public string PasswordHash { get; set; } = null!;

        [Column("role")]
        [Required]
        public UserRole Role { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum UserRole
    {
        admin,
        manager,
        courier
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }

    public bool CanChangeLogin(string newLogin)
    {
        return !string.IsNullOrWhiteSpace(newLogin) && newLogin != Login;
    }


}
