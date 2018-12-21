using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Projeto.Models;
using Projeto.br.com.correios.ws;
using Projeto.CRMClient;
using System.Diagnostics;

namespace Projeto.Controllers
{
    [Authorize]
    public class OrdersController : ApiController
    {
        private ProjetoContext db = new ProjetoContext();

        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            return db.Orders.Include(order => order.OrderItems).ToList();
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


        // PUT: api/Orders/close?id={orderId}
        //Pedido apenas fecha caso frete já tenha sido calculado - ADMIN ou Dono do pedido
        [ResponseType(typeof(Order))]
        [HttpPut]
        [Route("api/Orders/closed")]
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
        // PUT: api/Orders/frete?id={orderId}
        // Calcula o frete de um pedido através de seu id
        [ResponseType(typeof(Order))]
        [HttpPut]
        [Route("api/Orders/frete")]
        public IHttpActionResult CalculaFreteData(int id)
        {
            string frete;
            Order order = db.Orders.Find(id);
            Customer customer;

            if (order == null) return BadRequest("O pedido não existe.");

            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            if (IsAuthorized(order.Email))
            {

               CRMRestClient client = new CRMRestClient();
               customer = client.GetCustomerByEmail(User.Identity.Name);

                if (customer == null) return BadRequest("Falha ao consultar CEP: usuário não existe.");
                else
                {
                    //.count = .length
                    if (order.OrderItems.Count <= 0) return BadRequest("O pedido não contêm itens.");

                    decimal pesoTotal = 0;
                    decimal larguraTotal = 0;
                    decimal comprimentoTotal = 0;
                    decimal alturaTotal = 0;
                    decimal diametroTotal = 0;

                    foreach (OrderItem orderItem in order.OrderItems)
                    {
                        if(Convert.ToInt32(orderItem.Product.peso) > 0) pesoTotal = pesoTotal + (Convert.ToInt32(orderItem.Product.peso) * orderItem.Quantity);
                        if (Convert.ToInt32(orderItem.Product.largura) > 0) larguraTotal += (Convert.ToInt32(orderItem.Product.largura) * orderItem.Quantity);
                        if (Convert.ToInt32(orderItem.Product.comprimento) > 0)
                        {
                            if (Convert.ToInt32(orderItem.Product.comprimento) > comprimentoTotal)
                                comprimentoTotal = Convert.ToInt32(orderItem.Product.comprimento);
                        }
                        if (Convert.ToInt32(orderItem.Product.altura) > 0)
                        {
                            if (Convert.ToInt32(orderItem.Product.altura) > alturaTotal)
                                alturaTotal = Convert.ToInt32(orderItem.Product.altura);
                        }
                        if (Convert.ToInt32(orderItem.Product.diametro) > 0)
                        {
                            if (Convert.ToInt32(orderItem.Product.diametro) > diametroTotal)
                                diametroTotal = Convert.ToInt32(orderItem.Product.diametro);
                        }
                    }
                    

                    string nCdServico = "40010";
                    string sCdCepOrigem = "69096010";
                    string sCdCepDestino = customer.zip.Trim().Replace("-", "");
                    string nVIPeso = pesoTotal.ToString();
                    int nCdFormato = 1;
                    decimal nVIComprimento = comprimentoTotal;
                    decimal nVIAltura = alturaTotal;
                    decimal nVILargura = larguraTotal;
                    decimal nVIDiametro = diametroTotal;
                    string sCdMaoPropria = "N";
                    decimal nVIValorDeclarado = order.PrecoTotal;
                    string sCdAvisoRecebimento = "S";

                    cResultado resultado;

                    resultado = correios.CalcPrecoPrazo("", "", nCdServico, sCdCepOrigem, sCdCepDestino, nVIPeso, nCdFormato,
                            nVIComprimento, nVIAltura, nVILargura, nVIDiametro, sCdMaoPropria, nVIValorDeclarado, sCdAvisoRecebimento);
                    
                    if(resultado == null) return BadRequest("Falha ao calcular o frete e prazo de entrega.");
                    if (!resultado.Servicos[0].MsgErro.Equals("")) return BadRequest("Falha ao calcular o frete e prazo de entrega: " + resultado.Servicos[0].MsgErro);
                    
                    frete = "Valor	do	frete:	" + resultado.Servicos[0].Valor + "	-	Prazo	de	entrega:" + 
                            resultado.Servicos[0].PrazoEntrega + "	dia(s)";
                   
                    
                    order.PrecoFrete = decimal.Parse(resultado.Servicos[0].Valor);
                    order.DataEntrega = DateTime.Now.AddDays(int.Parse(resultado.Servicos[0].PrazoEntrega));
                    order.PesoTotal = pesoTotal;
                    order.PrecoTotal = order.PrecoTotal + decimal.Parse(resultado.Servicos[0].Valor);

                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();

                    return Ok(db.Orders.Find(id));
                }
            }
            else
            {
                return BadRequest("Usuário não autorizado.");
            }
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