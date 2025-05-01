namespace E_comerce.DTO
{
    public class BillboardDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string? Label { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
