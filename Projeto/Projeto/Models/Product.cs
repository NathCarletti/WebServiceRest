using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projeto.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O	campo	nome	é	obrigatório")]
        public string nome { get; set; }
        public string descricao { get; set; }
        public string cor { get; set; }
        public string peso { get; set; }
        public string altura { get; set; }
        public string largura { get; set; }
        public string comprimento { get; set; }
        public string diametro { get; set; }
        public string imgURL { get; set; }
        [Required(ErrorMessage = "O	campo	código	é	obrigatório")]
        public string codigo { get; set; }
        [Required(ErrorMessage = "O	campo	modelo	é	obrigatório")]
        public string modelo { get; set; }
        public decimal preco { get; set; }
    }
}