using ForceFit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class SeedData
{
    public static void semearDados(IApplicationBuilder app)
    {
        Context context = app.ApplicationServices.GetRequiredService<Context>();
        context.Database.Migrate();

        if (context.Alunos.Any() || context.Personais.Any() || context.Treinos.Any() || context.Exercicios.Any())
        {
            return;   // dados já foram semeados
        }

        // Insere dados de teste para aluno
        var alunos = new List<Aluno>
            {
                new Aluno
                {
                    NomeAluno="João Pedro",
                    Data_Nascimento = new DateTime(1995, 05, 10), // Informação 1 para Aluno
                    Instagram = "@joaopedro_fit",
                    PersonalID = context.Personais.FirstOrDefault(p => p.NomePersonal == "João Silva")?.PersonalID,
                    Observacoes = "a",
                    Telefone="35 999771580",
                    IdentityUserId = "teste-id-joao"
                },
            };
        context.Alunos.AddRange(alunos);

        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Erro ao salvar os dados: " + ex.InnerException?.Message);
            throw; // ou trate conforme preferir
        }
    }

    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
    {
        var defaultUser = await userManager.FindByEmailAsync("admin@admin.com");

        if (defaultUser == null)
        {
            var user = new IdentityUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = false

            };

            var result = await userManager.CreateAsync(user, "Admin@123");


        }
    }
}