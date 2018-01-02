using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InMemoryCache.Models;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace InMemoryCache.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache _cache;

        public HomeController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            //Verifica se existe ou não valor em um cache chamado "DataAtual"
            if (!_cache.TryGetValue("DataAtual", out DateTime dataAtual))
            {
                //Caso não exista, colocamos a data atual
                dataAtual = DateTime.Now;

                //Aqui configuramos as opções do cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetAbsoluteExpiration(TimeSpan.FromSeconds(5)); //Tempo de expiração do cache

                //Salvando :)
                _cache.Set("DataAtual", dataAtual, cacheEntryOptions);
            }

            ViewBag.DataAtual = dataAtual;

            return View();
        }

        public IActionResult GetOrCreateCache()
        {
            var dataAtual = _cache.GetOrCreate("DataAtual", dataCache =>
            {
                dataCache.SetSlidingExpiration(TimeSpan.FromSeconds(10));
                dataCache.SetPriority(CacheItemPriority.NeverRemove);
                return DateTime.Now;
            });

            ViewBag.DataAtual = dataAtual;

            return View();
        }

        public IActionResult RemoverCache()
        {
            _cache.Remove("DataAtual");
            return View("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
