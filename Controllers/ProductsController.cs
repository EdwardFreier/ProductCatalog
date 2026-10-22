using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Models;
using System.Text.Json;

namespace ProductCatalog.Controllers
{
    public class ProductsController : Controller
    {
        //class variables
        private readonly string _dataDir;
        private readonly string _dataFile;
        private readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        //constructor
        public ProductsController(IWebHostEnvironment env)
        {
            _dataDir = Path.Combine(env.ContentRootPath, "data");
            _dataFile = Path.Combine(_dataDir, "products.json");
            Directory.CreateDirectory(_dataDir);
        }

        //list of products
        private List<Product> Load()
        {
            if (!System.IO.File.Exists(_dataFile)) return new();
            var json = System.IO.File.ReadAllText(_dataFile);
            return JsonSerializer.Deserialize<List<Product>>(json) ?? new();
        }

        //save method
        private void Save(List<Product> products)
        {
            var json = JsonSerializer.Serialize(products, _json);
            System.IO.File.WriteAllText(_dataFile, json);
        }

        //index method
        public IActionResult Index()
        {
            var products = Load();
            return View(products);
        }

        //details method
        public IActionResult Details(int id)
        {
            var product = Load().FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }


        //create method
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Product product)
        {
            //validate model
            if (!ModelState.IsValid) return View(product);


            //add to the list and json
            var products = Load();
            product.Id = products.Count == 0 ? 1 : products.Max(p => p.Id) + 1;
            products.Add(product);
            Save(products);

            //return to the index
            return RedirectToAction(nameof(Index));
        }
    }
}
