using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Projeto.Models;

namespace Projeto.Controllers
{
    [Authorize]
    public class OrdersController : ApiController
    {
        private ProjetoContext db = new ProjetoContext();

        // GET: api/Orders
        //Listar todos apenas ADMIN
        [Authorize(Roles = "ADMIN")]
        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }

        // GET: api/Orders/5
        //Lista pedido pelo Id - ADMIM ou dono do pedido
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrderById(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return BadRequest("O pedido não existe!");
            }
            if (IsAuthorized(order.Email)) return Ok(order); else return BadRequest("Usuário não autorizado!");
        }

        // GET: api/Orders/byEmail?email={email}
        // Lista pedidos do usuário pelo email - ADMIN ou dono        
        [ResponseType(typeof(Order))]
        [HttpGet]
        [Route("api/Orders/byEmail")]
        public IHttpActionResult GetOrdersByEmail(string email)
        {
            if (IsAuthorized(email))
            {
                List<Order> orders = db.Orders.Where(order => order.Email.ToLower().Trim().Equals(email.ToLower().Trim())).ToList();

                if (orders == null) return BadRequest("Usuário não possui pedidos.");

                return Ok(orders);
            }
            else return BadRequest("Usuário não autorizado.");
        }

        // POST: api/Orders
        //Apenas usuários autenticados criam pedidos
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            //Valores pré-definidos
            order.Status = "novo";
            order.PesoTotal = 0;
            order.PrecoFrete = 0;
            order.PrecoTotal = 0;
            order.DataEntrega = DateTime.Now;

            //Retorna o email do usuário autenticado
            order.Email = User.Identity.Name;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
        }


        // PUT: api/Orders/close/5
        //Pedido apenas fecha caso frete já tenha sido calculado - ADMIN ou Dono do pedido
        [ResponseType(typeof(void))]
        [Route("api/Orders/close")]
        public IHttpActionResult PutOrder(int id)
        {
            Order order = db.Orders.Find(id);

            if (order == null) return BadRequest("O pedido não existe!");
            
            if (IsAuthorized(order.Email))
            {
                if (order.PrecoFrete != 0)
                {
                    order.Status = "fechado";
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();

                    return Ok(order);
                }
                else return BadRequest("Frete não calculado!");
            }
            else return BadRequest("Usuário não autorizado!");
        }

     
        // DELETE: api/Orders/5
        //Deleta pedido pelo Id apenas pelo ADM e Dono do pedido
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return BadRequest("O pedido não existe!");
            }
            if (IsAuthorized(order.Email))
            {
                db.Orders.Remove(order);
                db.SaveChanges();
                return Ok(order);
            }
            else return BadRequest("Usuário não autorizado!");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
        //Verifica se email é de usuário logado
        private bool IsAuthorized(string email)
        {
            bool userOwnOrder = User.Identity.Name.Equals(email);

            Trace.TraceInformation(User.Identity.Name);

            return (User.IsInRole("ADMIN") || userOwnOrder);
        }
    }
}