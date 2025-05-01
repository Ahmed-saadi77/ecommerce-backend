namespace E_comrece.DTO
{
    public class CreateStoreDto
    {
        public int Id { get; set; }  // Public ID
        public DateTime CreatedAt { get; set; }  // Public CreatedAt
        public DateTime UpdatedAt { get; set; }  // Public UpdatedAt

        public string Name { get; set; } = null!;  // Name property
        public string? UserId { get; set; }  // Nullable UserId for authentication
    }
}
