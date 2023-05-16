using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Pokedex_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Pokedex_Api.Scraper
{
    public static class Pokedex_Api
    {

        public static List<string> urlPokemon; // Lista de URLs de los Pokémon.
        public static List<Pokemon> pokemons; // Lista de objetos Pokémon.

        public static async Task<List<Pokemon>> DescargaPokedexAsync()
        {
            urlPokemon = new List<string>(); // Inicializar la lista de URLs.
            pokemons = new List<Pokemon>(); // Inicializar la lista de Pokémon.

            await getUrls(); // Obtener las URLs de los Pokémon.

            // Obtener la información de cada pokemon
            foreach (var url in urlPokemon) // Iterar sobre las URLs
            {
                await getInfo(url); // Obtener la información del Pokémon.
                Thread.Sleep(300); // Esperar 300 milisegundos antes de continuar.
            }

            return pokemons; // Devolver la lista de Pokémon descargados.
        }

        // Obtener las URLs de los Pokémon.
        public static async Task<bool> getUrls()
        {
            var client = new HttpClient(); // Crear una instancia de HttpClient para hacer la solicitud HTTP.
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.pokemon.com/el/api/pokedex/"); // Crear una solicitud HTTP GET a la URL especificada.
            var response = await client.SendAsync(request); // Enviar la solicitud HTTP y esperar la respuesta.

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                JArray Pokedex = JArray.Parse(await response.Content.ReadAsStringAsync()); // Parsear el contenido de la respuesta a un JArray.
                foreach (var Pokemon in Pokedex)
                {
                    urlPokemon.Add($"https://www.pokemon.com/el/pokedex/{Pokemon["slug"]}"); // Agregar la URL del Pokémon a la lista de URLs.
                }
                urlPokemon = urlPokemon.Distinct().ToList(); // Eliminar duplicados de la lista de URLs.
                return true; // Devolver true para indicar que se obtuvieron las URLs correctamente.
            }
            else
            {
                return false; // Devolver false si la solicitud no fue exitosa.
            }
        }

        // Obtener la información de cada pokemon
        public static async Task<bool> getInfo(string url) {
            Pokemon pokemon = new Pokemon(); // Crear una nueva instancia de la clase Pokemon.

            var client = new HttpClient(); // Crear una instancia de HttpClient para hacer la solicitud HTTP.
            var request = new HttpRequestMessage(HttpMethod.Get, url); // Crear una solicitud HTTP GET a la URL especificada.
            var response = await client.SendAsync(request); // Enviar la solicitud HTTP y esperar la respuesta.

            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode) // Si la respuesta es correcta
            {
                string html = await response.Content.ReadAsStringAsync(); // Lee el contenido de la respuesta como una cadena de texto.

                HtmlDocument document = new HtmlDocument(); // Crea una nueva instancia de la clase HtmlDocument.
                document.LoadHtml(html); // Carga el contenido HTML en el objeto HtmlDocument.

                // Url
                pokemon.Url = url; // Asigna la URL al objeto Pokemon.
                // Nombre
                //Busca todos los elementos div que tienen la clase pokedex-pokemon-pagination-title dentro del documento HTML y los guarda en la variable nombre
                var nombre = document.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("pokedex-pokemon-pagination-title"));
                // Verifica si la variable nombre no es nula y contiene al menos un elemento.
                if (nombre != null && nombre.Any()) {
                    //  Verifica si la variable nombre no es nula y contiene al menos un elemento.
                    pokemon.Numero = nombre.First().Descendants("span").Where(span => span.GetClasses().Contains("pokemon-number")).First().InnerText;
                    // Accede al elemento de nombre, busca los elementos span que tienen la clase pokemon-number dentro de ese elemento y
                    // obtiene el texto interno utilizando la propiedad InnerText. El número del Pokémon se asigna a la propiedad Numero del objeto Pokemon.
                    pokemon.Nombre = nombre.First().InnerText.Replace(pokemon.Numero, "").Replace("\n","").Trim();
                    pokemon.Numero = pokemon.Numero.Replace("&nbsp;", "");
                }
                // Imagen
                // Busca todos los elementos div que tienen la clase profile-images dentro del documento HTML y los guarda en la variable imagen.
                var imagen = document.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("profile-images"));
                // Verifica si la variable imagen no es nula y contiene al menos un elemento.
                if (imagen != null && imagen.Any()) {
                    // Accede al elemento de imagen, busca los elementos img que tienen la clase active dentro de ese elemento
                    // y obtiene el valor del atributo "src" utilizando la propiedad attributes
                    pokemon.Imagen = imagen.First().Descendants("img").Where(img => img.GetClasses().Contains("active")).First().Attributes["src"].Value;
                }
                // Descripcion X
                //  Busca todos los elementos p que tienen la clase version-x dentro del documento HTML y los guarda en la variable descX.
                var descX = document.DocumentNode.Descendants("p").Where(p => p.GetClasses().Contains("version-x"));
                // Verifica si la variable descX no es nula y contiene al menos un elemento.
                if (descX != null && descX.Any())
                {
                    //  Accede al elemento de descX y obtiene el texto
                    pokemon.DescripcionVersionX = descX.First().InnerText.Trim();
                }
                // Descripcion Y
                // Busca todos los elementos p que tienen la clase CSS "version-y" dentro del documento HTML y los guarda en la variable descY.
                var descY = document.DocumentNode.Descendants("p").Where(p => p.GetClasses().Contains("version-y"));
                // Verifica si la variable descY no es nula y contiene al menos un elemento.
                if (descY != null && descY.Any())
                {
                    // Accede al elemento de descY y obtiene el texto
                    pokemon.DescripcionVersionY = descY.First().InnerText.Trim();
                }
                // Tabla con Descripcion
                // Busca todos los elementos div que tienen la clase column-7 dentro del documento HTML y los guarda en la variable tabla como un array.
                var tabla = document.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("column-7")).ToArray();
                // Verifica si la variable tabla no es nula y contiene al menos un elemento.
                if (tabla != null && tabla.Any()) 
                {
                    foreach (var t in tabla) // itera sobre cada elemento div de tabla.
                    {
                        foreach (var li in t.Descendants("li")) //  itera sobre cada elemento li dentro de tabla
                        {
                            // Busca todos los elementos span que tienen la clase attribute-title dentro de li y los guarda en la variable attTitle
                            var attTitle = li.Descendants("span").Where(span => span.GetClasses().Contains("attribute-title"));
                            // Busca todos los elementos span que tienen la clase attribute-value dentro de li y los guarda en la variable attValue
                            var attValue = li.Descendants("span").Where(span => span.GetClasses().Contains("attribute-value"));
                            //  Verifica si tanto attTitle como attValue contienen al menos un elemento.
                            if (attTitle.Any() && attValue.Any())
                            {
                                // Obtiene el texto interno del primer elemento de attTitle y lo guarda en la variable title.
                                var title = attTitle.First().InnerText;
                                // Asigna el primer elemento de attValue a la variable value.
                                var value = attValue.First();
                                // se realizan comprobaciones adicionales basadas en el contenido de title para determinar qué propiedad del objeto Pokemon se debe actualizar:
                                // si contiene la palabra "altura" en cualquier forma (mayúsculas o minúsculas), se asigna el texto interno de value a la propiedad Altura del objeto Pokemon.
                                if (title.ToLower().Contains("altura"))
                                {
                                    pokemon.Altura = value.InnerText;
                                }
                                // si contiene la palabra "peso" en cualquier forma, se asigna el texto interno de value a la propiedad Peso del objeto Pokemon
                                else if (title.ToLower().Contains("peso"))
                                {
                                    pokemon.Peso = value.InnerText;
                                }
                                // si contiene la palabra "categoría" en cualquier forma, se asigna el texto interno de value a la propiedad Categoria del objeto Pokemon.
                                else if (title.ToLower().Contains("categoría"))
                                {
                                    pokemon.Categoria = value.InnerText;
                                }
                                // si contiene la palabra "habilidad" en cualquier forma, se asigna el texto interno de value a la propiedad Habilidad del objeto Pokemon.
                                else if (title.ToLower().Contains("habilidad"))
                                {
                                    pokemon.Habilidad = value.InnerText;
                                }
                                // Si contiene la palabra "sexo" en cualquier forma, 
                                else if (title.ToLower().Contains("sexo"))
                                {
                                 /*
                                 * Se realiza un bucle a través de los elementos i dentro de value y se verifica si contienen la clase "female" o "male". 
                                 * En función de esto, se actualiza la propiedad Sexo del objeto Pokemon agregando la cadena "Female" o "Male" respectivamente. 
                                 * Se realiza un procesamiento adicional para eliminar comas redundantes y espacios en blanco.
                                 */
                                    foreach (var sexo in value.Descendants("i"))
                                    {
                                        var valueSexo = sexo.Attributes["class"].Value;
                                        if (valueSexo.ToLower().Contains("female"))
                                        {
                                            pokemon.Sexo += ", Female,";
                                        }
                                        else if (valueSexo.ToLower().Contains("male"))
                                        {
                                            pokemon.Sexo += ", Male,";
                                        }
                                    }
                                    if(pokemon.Sexo != null && pokemon.Sexo.Any())
                                        pokemon.Sexo = pokemon.Sexo.Trim(',').Replace(",,",",").Trim();
                                }
                            }
                            // Verifica si hay elementos en attTitle
                            else if (attTitle.Any())
                            {
                                // Comprueba si el texto interno del primer elemento de attTitle contiene la palabra "habilidad" en cualquier forma.
                                if (attTitle.First().InnerText.ToLower().Contains("habilidad"))
                                {
                                    // Asigna a attValue los elementos ul que contienen la clase CSS "attribute-list" y están dentro de li.
                                    attValue = li.Descendants("ul").Where(ul => ul.GetClasses().Contains("attribute-list"));
                                    // Recorre los elementos span que contienen la clase CSS "attribute-value" y están dentro del primer elemento de attValue.
                                    foreach (var value in attValue.First().Descendants("span").Where(span => span.GetClasses().Contains("attribute-value")))
                                    {
                                        pokemon.Habilidad += $"- {value.InnerText} -";
                                    }
                                }
                            }
                        }
                    }
                }
                // Tipo 
                //  Se busca el elemento div que tiene la clase CSS "dtm-type" y se asigna a la variable tipo.
                var tipo = document.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("dtm-type"));
                // Se verifica si tipo no es nulo y si contiene elementos.
                if (tipo != null && tipo.Any()) 
                {
                    // Se recorren los elementos li que están dentro del primer elemento de tipo.
                    foreach (var li in tipo.First().Descendants("li")) 
                    {
                        // Se agrega el tipo obtenido al objeto Pokemon, concatenándolo a la propiedad Tipo. Se utiliza InnerText para
                        // obtener el texto interno del elemento li y se utiliza Trim() para eliminar espacios en blanco adicionales.
                        pokemon.Tipo += $" {li.InnerText.Trim()},";
                    }
                    //  Se eliminan las comas redundantes y se eliminan espacios en blanco adicionales al inicio y al final de la cadena de tipos.
                    pokemon.Tipo = pokemon.Tipo.Trim(',').Trim();
                }
                // Debilidad 
                // Se busca el elemento div que tiene la clase CSS "dtm-weaknesses" y se asigna a la variable debilidad.
                var debilidad = document.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("dtm-weaknesses"));
                //  Se verifica si debilidad no es nulo y si contiene elementos.
                if (debilidad != null && debilidad.Any())
                {
                    // Se recorren los elementos span que están dentro del primer elemento de debilidad.
                    foreach (var span in debilidad.First().Descendants("span"))
                    {
                        // Se agrega la debilidad obtenida al objeto Pokemon, concatenándola a la propiedad Debilidad.
                        // Se utiliza InnerText para obtener el texto interno del elemento span
                        // y se utiliza Trim() para eliminar espacios en blanco adicionales.
                        pokemon.Debilidad += $" {span.InnerText.Trim()},";
                    }
                    // Se eliminan las comas redundantes y se eliminan espacios en blanco adicionales al inicio y al final de la cadena de debilidades.
                    pokemon.Debilidad = pokemon.Debilidad.Trim(',').Trim();
                }
                // Puntos Base
                // Se busca el primer elemento div con la clase CSS "pokemon-stats-info" y se selecciona el primer elemento ul dentro de él. El resultado se asigna a la variable puntosBase.
                var puntosBase = document.DocumentNode.Descendants("div").Where(div => div.GetClasses().Contains("pokemon-stats-info")).First().Descendants("ul").First();
                //  Se verifica si puntosBase no es nulo.
                if (puntosBase != null) {
                    // Se inicializan las variables para almacenar los valores de los puntos base
                    int ps = 0, ataque = 0, defensa = 0, ataqueEspecial = 0, defensaEspecial = 0, velociad = 0;
                    // Se recorren los elementos li dentro de puntosBase.
                    foreach (var li in puntosBase.Descendants("li")) 
                    {
                        // Se verifica si el elemento li tiene texto interno.
                        if (li.InnerText.Length > 0) 
                        {
                            //  Se obtiene el texto interno del primer elemento span dentro de li y se asigna a la variable titulo.
                            var titulo = li.Descendants("span").First().InnerText;
                            // Se obtiene el valor del atributo "data-value" del primer elemento li dentro de li que tiene la clase CSS "meter" y se asigna a la variable value
                            var value = li.Descendants("li").Where(li => li.GetClasses().Contains("meter")).First().Attributes["data-value"].Value;
                            // Se comparan las cadenas titulo en minúsculas con los diferentes títulos relacionados con los puntos base y se asignan los valores correspondientes
                            if (titulo.ToLower().Contains("ps"))
                            {
                                ps = int.Parse(value);
                            }
                            else if (titulo.ToLower().Contains("ataque especial"))
                            {
                                ataqueEspecial = int.Parse(value);
                            }
                            else if (titulo.ToLower().Contains("defensa especial"))
                            {
                                defensaEspecial = int.Parse(value);
                            }
                            else if (titulo.ToLower().Contains("ataque"))
                            {
                                ataque = int.Parse(value);
                            }
                            else if (titulo.ToLower().Contains("defensa"))
                            {
                                defensa = int.Parse(value);
                            }
                            else if (titulo.ToLower().Contains("velocidad"))
                            {
                                velociad = int.Parse(value);
                            }
                        }
                    }
                    // Finalmente, se asignan los valores de los puntos base al objeto PuntosBase y se asigna este objeto a la propiedad PuntosBase del objeto Pokemon.
                    pokemon.PuntosBase = (new PuntosBase { 
                        PS = ps,
                        Ataque = ataque,
                        Defensa = defensa,
                        AtaqueEspecial = ataqueEspecial,
                        DefensaEspecial = defensaEspecial,
                        Velociad = velociad
                    });
                }
                // Evolucion 
                // Se buscan todos los elementos h3 que contengan la clase CSS "match" y se almacenan en la lista evoluciones.
                var evoluciones = document.DocumentNode.Descendants("h3").Where(div => div.GetClasses().Contains("match")).ToList();
                //  Se verifica si la lista de evoluciones no es nula y contiene elementos.
                if (evoluciones != null && evoluciones.Any()) {
                    // Se recorre cada elemento evolucion en la lista evoluciones.
                    foreach (var evolucion in evoluciones) 
                    {
                        // Se concatena el texto interno del elemento evolucion al atributo Evoluciones del objeto Pokemon.
                        // Se eliminan los caracteres especiales " " y las nuevas líneas, se reemplazan múltiples espacios
                        // en blanco por uno solo, y se realiza un recorte de espacios en blanco al inicio y al final.
                        pokemon.Evoluciones += " "+Regex.Replace(evolucion.InnerText.Replace("&nbsp;", "").Replace("\n", "") + " > ", @"\s{2,}", " ").Trim();
                    }
                    // Se eliminan los símbolos ">" al inicio y al final de la cadena de evoluciones.
                    pokemon.Evoluciones = pokemon.Evoluciones.Trim('>').Trim();
                }
                // MongoDB
                //  Se construye la cadena de conexión a la base de datos MongoDB utilizando las variables 
                var connectionString = $"mongodb+srv://{Database.Username}:{Database.Password}@{Database.UrlDB}/?retryWrites=true&w=majority";
                //  Se crea una instancia del cliente de MongoDB pasando la cadena de conexión como argumento.
                var clientMongo = new MongoClient(connectionString);
                // Se obtiene una referencia a la base de datos "Pokedex-Api" en MongoDB.
                var database = clientMongo.GetDatabase("Pokedex-Api");
                // Se obtiene una referencia a la colección "Pokemones" en la base de datos.
                var collection = database.GetCollection<Pokemon>("Pokemones");
                // Se inserta el objeto pokemon en la colección de MongoDB.
                collection.InsertOne(pokemon);
                //  Se agrega el objeto pokemon a la lista pokemons.
                pokemons.Add(pokemon);
                // Se retorna el valor booleano true para indicar que la operación fue exitosa.
                return true; 
            }
            else 
            { // Si no
                // Se retorna el valor booleano false para indicar que la operación no fue exitosa.
                return false;
            }
        }
    }
}
