namespace GFT_API_DB.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public bool IsMorning { get; set; }
        public string OrderContents { get; set; }
    }
}
