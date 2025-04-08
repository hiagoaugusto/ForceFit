using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace ForceFit.Models
{
    public class Context : IdentityDbContext//Identity dbcontext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }
        //criar tabelas
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Personal> Personais { get; set; }
        public DbSet<Exercicio> Exercicios { get; set; }
        public DbSet<Treino> Treinos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Personal>()
            .HasKey(p => p.PersonalID);
            //configuraçãoda entidade treino e exercicio
            modelBuilder.Entity<Treino>()
                //Especifica que um treino tem muitos exercicios
                .HasMany(e => e.Exercicios)
                //Especifica que exercicio tem muitos treinos
                .WithMany(t => t.Treinos)
                //Especifica a tabela de junção
                .UsingEntity(j => j.ToTable("TreinoExercios"));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

    }
}
