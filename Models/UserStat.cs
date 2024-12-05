namespace Spotify_Project.Models
{
    public class UserStat
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int CorrectGuesses { get; set; }
        public int WrongGuesses { get; set; }
    }
}
