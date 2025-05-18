using EM.Modules.Feature.Outflow.Models;
using Microsoft.AspNetCore.Mvc;


namespace EM.Modules.Feature.Outflow.Controllers
{

    [Route("users")]
    public class OutflowController : Controller
    {
        private IList<OutflowModel> _db;

        public OutflowController()
        {
            _db = new List<OutflowModel> {
                new OutflowModel{ Id = 1, Purpose = "Purchasing cells", Amount = 150}
            };

        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var users = _db.ToList();
            return View("Modules/Views/Outflow/Index.cshtml", users);
        }

        [HttpGet("create")]
        public IActionResult Create() => View();

        [HttpPost("create")]
        public async Task<IActionResult> Create(OutflowModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Add(model);

            return RedirectToAction("Index");
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = _db.First(x => x.Id == id);
            return View(user);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(OutflowModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _db.FirstOrDefault(x => x.Id == model.Id);

            if (user != null)
            {
                user.Amount = model.Amount;
                user.Purpose = model.Purpose;
            }
            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = _db.FirstOrDefault(x => x.Id == id);

            if (user != null)
            {
                _db.Remove(user);
            }
            return RedirectToAction("Index");
        }
    }
}
