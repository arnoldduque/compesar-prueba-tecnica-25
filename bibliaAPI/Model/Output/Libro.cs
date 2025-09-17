using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace bibliaAPI.Model.Output
{
    public class Libro
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }        
        public int? Chapter { get; set; }
        public List<Verso>? Verses { get; set; }        

        [ForeignKey("ConsultaId")]
        [JsonIgnore]
        public int ConsultaId { get; set; }
    }
}