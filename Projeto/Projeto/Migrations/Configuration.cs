namespace Projeto.Migrations
{
    using Projeto.CRMClient;
    using Projeto.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Projeto.Models.ProjetoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Projeto.Models.ProjetoContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
          

            context.Products.AddOrUpdate(
                p => p.Id,
                new Product
                {
                    Id = 1, nome = "produto	1",
                    codigo = "COD1",
                    descricao	=	"descrição  produto 1",
                    preco	=	10,
                    modelo = "carro1",
                    altura = "10",
                    comprimento = "10",
                    diametro = "10",
                    imgURL = "carro.jpeg",
                    cor = "verde",
                    peso = "10"
                },

                new Product
                {
                    Id = 2, nome = "produto	2",
                    codigo = "COD2",
                    descricao	=	"descrição  produto 2",
                    preco	=	20,
                    modelo = "carro2",
                    altura = "20",
                    comprimento = "20",
                    diametro = "20",
                    imgURL = "carro2.jpeg",
                    cor = "amarelo",
                    peso = "20"
                },

                new Product
                {
                    Id = 3, nome = "produto	3",
                    codigo = "COD3",
                    descricao	=	"descrição  produto 3",
                    preco	=	30,
                    modelo = "carro3",
                    altura = "30",
                    comprimento = "30",
                    diametro = "30",
                    imgURL = "carro3.jpeg",
                    cor = "vermelho",
                    peso = "30"
                }
            );

           /* context.Customer.AddOrUpdate(
                    c => c.Id, new Customer
                    { Id = 1, cpf = "12345678901",
                name = "CRM	Web	API",
                address = "Rua	1,	100",
                city = "São	Paulo",
                state = "São	Paulo",
                country = "Brasil",
                zip = "12345000",
                email = "matilde@siecolasystems.com",
                mobile = "+551112345678", });
                */
        }
    }
}
