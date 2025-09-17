namespace bibliaAPI.Model.Input
{
    public class Verse
    {
        public string? book_id { get; set; }
        public string? book { get; set; }
        public int? chapter { get; set; }
        public int? verse { get; set; }
        public string? text { get; set; }
    }
}