namespace E_commerce.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Label { get; set; }
        public int StoreId { get; set; }
        public int BillboardId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
