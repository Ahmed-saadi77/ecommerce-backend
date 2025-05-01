namespace E_comerce.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; } = Guid.NewGuid(); // Unique identifier for the user
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp when the user is created
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Timestamp when the user is last updated
    }
}