namespace Habr.DataAccess.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageEncodedBase64 { get; set; }
        public DateTime LoadDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
