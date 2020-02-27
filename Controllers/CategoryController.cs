using System.Collections.Generic;
using ProductCatalog.Data;
using ProductCatalog.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Controllers
{

    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        // Chamando o Context para se trabalhar com o banco de dados
        // Não se deve instanciar um novo Context, pois toda vez se criara uma nova conexão, deve-se usar injeção de dependência para só haver uma conexão aberta.
        private readonly StoreDataContext _context;
        
        // Injeção de dependência
        public CategoryController(StoreDataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public IEnumerable<Category> Get() 
        {
            // Retorna uma lista com todas as Categorias
            // EF Core traz junto com os dados um proxy com informações a mais como se ele foi atualizado, removido, incluso
            // AsNoTracking - Remove essas informações a mais e ajuda na performance
            return _context.Categories.AsNoTracking().ToList();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id) 
        {
            return Ok(_context.Categories.AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefault());

            // ou

            // Porém não funciona com o AsNoTracking()
            //  return _context.Categories.Find(id);
        }

        [HttpGet("{id}/products")]
        public IEnumerable<Product> GetProducts(int id) 
        {
            // Lista os Produtos de uma determinada Categoria

            return _context.Products.AsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToList();
        }

        [HttpPost]
        public IActionResult Add(Category category)
        {
            _context.Categories.Add(category); // Fica na memória
            _context.SaveChanges(); // Aplica no banco

            return Ok(category);
        }

        [HttpPut]
        public IActionResult Update(Category category)
        {
            // EF faz as alterações e muda o estado para modified
            _context.Entry<Category>(category).State = EntityState.Modified; 
            _context.SaveChanges();

            return Ok(category);
        }

        [HttpDelete]
        public IActionResult Delete(Category category)
        {
            // EF faz as alterações e muda o estado para modified
            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(category);
        }

    }
}