using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using HeroesApi.Models;
using HeroesApi.Data;

namespace HeroesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeroesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Hero>> GetAll()
        {
            return Ok(HeroesStore.Heroes);
        }

        [HttpGet("{id}")]
        public ActionResult<Hero> GetById(int id)
        {
            var hero = HeroesStore.Heroes.FirstOrDefault(hero => hero.Id == id);
            if (hero is null)
            {
                return NotFound(new { message = $"Герой с id = {id} не найден" });
            }
            return Ok(hero);
        }

        [HttpGet("demo")]
        public ActionResult GetDemo()
        {
            var hero = HeroesStore.Heroes.First();
            var defaultOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var ourOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            return Ok(new
            {
                withDefaultSettings = JsonSerializer.Deserialize<object>(
                    JsonSerializer.Serialize(hero, defaultOptions),
                    defaultOptions
                ),
                withOurSettings = JsonSerializer.Deserialize<object>(
                    JsonSerializer.Serialize(hero, ourOptions),
                    ourOptions
                ),
                note = "Сравните имена полей и значение universe в двух вариантах"
            });
        }

        private static readonly Hero exampleHero = new Hero
        {
            Id = 7,
            Name = "Дэдпул",
            RealName = "Уэйд Уилсон",
            Universe = Universe.Marvel,
            PowerLevel = 80,
            Powers = new List<string> { "регенерация", "владение оружием", "болтовня" },
            Weapon = new Weapon { Name = "Катангы и пистолеты", IsRanged = true },
            InternalNotes = "Разрушает четвертую стену"
        };
    }
}
