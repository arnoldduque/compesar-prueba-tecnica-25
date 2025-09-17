using Microsoft.EntityFrameworkCore;
using bibliaAPI.Model.Output;

public class BibleContext : DbContext
{
    public BibleContext(DbContextOptions<BibleContext> options) : base(options) { }
    public DbSet<Consulta> Consultas { get; set; }
}
