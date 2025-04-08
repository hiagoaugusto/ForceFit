namespace ForceFit.Models
{
    public class Aluno
    {
        public int AlunoID { get; set; }
        public string NomeAluno { get; set; }
        public DateTime Data_Nascimento { get; set; }
        public string Instagram { get; set; }
        public string Telefone { get; set; }
        public int? PersonalID { get; set; }
        public Personal? Personal { get; set; }
        public string Observacoes { get; set; }
        public ICollection<Treino> Treinos { get; set; }
        public string? IdentityUserId {  get; set; }



    }
}
