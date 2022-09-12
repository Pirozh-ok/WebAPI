namespace Habr.DataAccess.Entities
{
    public class PostImage : Image
    {
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
