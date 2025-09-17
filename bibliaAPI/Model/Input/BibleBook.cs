namespace bibliaAPI.Model.Input
{
    public class BibleBook
    {
        public Translation? translation { get; set; }
        public List<Verse>? verses { get; set; }
    }
}