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
            var hero = HeroesStore.Heroes.FirstOrDefault(h => h.Id == id);
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

        [HttpPost("serialize")]
        public ActionResult Serialize([FromBody] Hero hero) 
        {
            var options = new JsonSerializerOptions 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            string serialized = JsonSerializer.Serialize(hero, options);
            Hero deserializedHero = JsonSerializer.Deserialize<Hero>(serialized, options);

            return Ok(new 
            {
                serialized,
                deserializedHero,
                internalNotesAfterDeserialize = deserializedHero?.InternalNotes ?? "null - none поле было проигнорировано"
            });
        }

        [HttpPost("create")]
        public ActionResult CreateHero()
        {
            var hero = new Hero 
            {
                Id = 99,
                Name = "Тестовый герой",
                RealName = "Студент",
                Universe = Universe.Marvel,
                PowerLevel = 50,
                Powers = new List<string> { "программирование", "дебаггинг" },
                Weapon = new Weapon { Name = "Клавиатура", IsRanged = false },
                InternalNotes = "Это поле не попадет в JSON"
            };

            var options = new JsonSerializerOptions 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            string serialized = JsonSerializer.Serialize(hero, options);
            var deserialized = JsonSerializer.Deserialize<Hero>(serialized, options);

            return Ok(new 
            {
                serializedJson = serialized,
                deserializedObject = deserialized,
                internalNotesAfterDeserialize = deserialized?.InternalNotes ?? "null - none поле было проигнорировано"
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
