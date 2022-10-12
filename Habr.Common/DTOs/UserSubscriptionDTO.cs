namespace Habr.Common.DTOs
{
    public class UserSubscriptionDTO
    {
        public int UserId { get; set; }
        public string UserMail { get; set; }
        public string UserName { get; set; }
        public DateTime DataSubcribe { get; set; }
        public string AvatarPath { get; set; }
    }
}
