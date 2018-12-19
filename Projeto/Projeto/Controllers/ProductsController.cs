using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Projeto.Models;

namespace Projeto.Controllers
{
    public class ProductsController : ApiController
    {
        private ProjetoContext db = new ProjetoContext();

        // GET: api/Products
        [Authorize(Roles = "ADMIN,USER")]
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: api/Products/5
        [Authorize(Roles = "ADMIN,USER")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);

            if (product == null) return NotFound();

            bool prodSameCode = db.Products.Any(prod => prod.codigo == product.codigo);
            bool prodSameModel = db.Products.Any(prod => prod.modelo == product.modelo);

            if (prodSameCode && prodSameModel) return BadRequest("Código e modelo já existente.");
            else if (prodSameCode) return BadRequest("Código já existente.");
            else if (prodSameModel) return BadRequest("Modelo já existente.");

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // POST: api/Products
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // PUT: api/Products/5
        //Não altera modelo e código para valores existentes
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != product.Id) return BadRequest();

            bool prodSameCode = db.Products.Where(prod => prod.codigo != product.codigo)
                .Any(prod => prod.codigo == product.codigo);
            bool prodSameModel = db.Products.Where(prod => prod.modelo != product.modelo)
                .Any(prod => prod.modelo == product.modelo);

            if (prodSameCode && prodSameModel) return BadRequest("Código e modelo já existente.");
            
            else if (prodSameCode) return BadRequest("Código já existente.");
            
            else if (prodSameModel) return BadRequest("Modelo já existente.");

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id)) return NotFound();
                else throw;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }


        // DELETE: api/Products/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return BadRequest("Produto não encontrado!");
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}