using System.Data;

namespace ForceFit.Models
{
    public class Treino
    {
        public int TreinoID { get; set; }
        public int PersonalID { get; set; }
        public int AlunoID { get; set; }
        public string NomeTreino { get; set; }
        public Personal Personal { get; set; }

        public Aluno? Aluno { get; set; }
        public DateTime Data {  get; set; }
        public DateTime Hora { get; set; }
        public ICollection<Exercicio> Exercicios { get; set;}

    }
}
