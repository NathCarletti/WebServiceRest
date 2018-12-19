using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projeto.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }

        public int OrderId { get; set; }

        public string Email { get; set; }

        public DateTime DataPedido { get; set; }

        public DateTime DataEntrega { get; set; }

        //Novo, fechado, cancelado, entregue
        public string Status { get; set; }

        public decimal PrecoTotal { get; set; }

        public decimal PesoTotal { get; set; }

        public decimal PrecoFrete { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}