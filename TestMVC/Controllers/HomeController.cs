using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using TestMVC.Models;

namespace TestMVC.Controllers
{
    public class HomeController : Controller
    {
        private ClientsContext _db;
        public HomeController(ClientsContext clientsContext)
        {
            _db = clientsContext;
        }
        public IActionResult Index()
        {
            var clients = _db.Clients.Include(c => c.Founders).ToList();
            return View(clients);
        }

        [HttpPost]
        public IActionResult AddClient(Client client)
        {
            if (Validate(client))
            {
                _db.Clients.Add(client);
                _db.SaveChanges();
				return new JsonResult(new { insertedId = client.ClientId });
			}

            return PartialView(client);
        }

        [HttpPost]
        public IActionResult AddFounder(Founder founder)
        {
            var client = _db.Clients.Where(c => c.ClientId == founder.ClientId).SingleOrDefault();
            founder.Client = client;

            if (Validate(founder))
            {                
                client?.Founders.Add(founder);
                _db.SaveChanges();
                return new JsonResult(new { insertedId = founder.FounderId });
            }
            return PartialView(founder);
		}

        [HttpGet]
        public PartialViewResult GetClients()
        {
            var clients = _db.Clients.Include(c => c.Founders).AsNoTracking().ToList();
            return PartialView(clients);
        }

        [HttpGet]
        public PartialViewResult GetClient(int id)
        {
            var client = _db.Clients.Where(c => c.ClientId == id).Include(c => c.Founders).AsNoTracking().SingleOrDefault();
            return PartialView(client);
        }

        [HttpGet]
        public PartialViewResult GetFounders(int clientId)
        {
            var client = _db.Founders.Where(f => f.ClientId == clientId).Include(f => f.Client).AsNoTracking();
            return PartialView(client);
        }

        [HttpGet]
        public PartialViewResult GetFounder(int id)
        {
            var client = _db.Founders.Where(f => f.FounderId == id).AsNoTracking().SingleOrDefault();
            return PartialView(client);
        }

        [HttpPost]
        public void DeleteClient(int id)
        {
            var client = _db.Clients.Where(c => c.ClientId == id).SingleOrDefault();
            if (client != null)                                                             
                _db.Clients.Remove(client);
            _db.SaveChanges();
        }
        [HttpPost]
        public void DeleteFounder(int id)
        {
            var founder = _db.Founders.Where(f => f.FounderId == id).SingleOrDefault();
            if (founder != null)
                _db.Founders.Remove(founder);
            _db.SaveChanges();
        }

        [HttpPost]
        public IActionResult EditClient(Client client)
        {
            var editedClient = _db.Clients.Where(c => c.ClientId == client.ClientId).Include(c => c.Founders).Single();
            editedClient.Inn = client.Inn;
            editedClient.Name = client.Name;
            editedClient.Type = client.Type;

            if (Validate(editedClient))
            {
				_db.SaveChanges();
				return StatusCode(200); 
			}
            return PartialView(client);
        }

        [HttpPost]
        public IActionResult EditFounder(Founder founder)
        {
            var editedFounder = _db.Founders.Where(f => f.FounderId == founder.FounderId).Single();
            editedFounder.Inn = founder.Inn;
            editedFounder.FullName = founder.FullName;

            if (Validate(editedFounder))
            {
				_db.SaveChanges();
				return StatusCode(200);
			}
            return PartialView(editedFounder);
        }

        private bool Validate<TModel>(TModel model)
        {
            if (model is IValidatableObject validatableModel)
            {
                var validationResults = validatableModel.Validate(new ValidationContext(model));
                foreach (var error in validationResults)
                    ModelState.AddModelError(error.MemberNames.FirstOrDefault()!, error.ErrorMessage!);
            }

            return ModelState.IsValid;
        }
    }
}
