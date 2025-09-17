using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace bibliaAPI.Model.Output
{
    public class Verso
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public int? Verse { get; set; }
        public string? Text { get; set; }

        [ForeignKey("LibroId")]
        [JsonIgnore]
        public int LibroId { get; set; }
    }
}