using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace bibliaAPI.Model.Output
{
    public class Consulta
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }        
        public List<Libro>? Genesis { get; set; }
    }
}
