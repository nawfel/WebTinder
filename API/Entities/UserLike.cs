namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }

        public AppUser TargetUSer { get; set; }
        public int TargetUserId { get; set; }
    }
}
