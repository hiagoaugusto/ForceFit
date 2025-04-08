using System.ComponentModel.DataAnnotations;

namespace ForceFit.Models
{
    public class Personal
    {

        [Key]
        public int PersonalID { get; set; }
        public string NomePersonal { get; set; }

        public string Especialidade { get; set; }
        public ICollection<Aluno>? Alunos { get; set; }

        public ICollection<Treino>? Treinos { get; set; }
        public string? IdentityUserId { get; set; }

    }
}
