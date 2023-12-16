namespace API.Helpers
{
    public class LikesParams : UserParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}
