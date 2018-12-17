namespace Projeto.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false),
                        descricao = c.String(),
                        cor = c.String(),
                        peso = c.String(),
                        altura = c.String(),
                        largura = c.String(),
                        comprimento = c.String(),
                        diametro = c.String(),
                        imgURL = c.String(),
                        codigo = c.String(nullable: false),
                        modelo = c.String(nullable: false),
                        preco = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Products");
        }
    }
}
