namespace StableAPI.Models.Dto
{
    public class StockEntryDto
    {
        public int StockItemId { get; set; }
        public int StableId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}