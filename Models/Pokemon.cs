using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pokedex_Api.Models
{
    public class Pokemon
    {
        [BsonId]
        public string Numero { get; set; }
        [BsonElement("url")]
        public string Url { get; set; }
        [BsonElement("nombre")]
        public string Nombre { get; set; }
        [BsonElement("imagen")]
        public string Imagen { get; set; }
        [BsonElement("descripcionversionx")]
        public string DescripcionVersionX { get; set; }
        [BsonElement("descripcionversiony")]
        public string DescripcionVersionY { get; set; }
        [BsonElement("altura")]
        public string Altura { get; set; }
        [BsonElement("categoria")]
        public string Categoria { get; set; }
        [BsonElement("peso")]
        public string Peso { get; set; }
        [BsonElement("habilidad")]
        public string Habilidad { get; set; }
        [BsonElement("sexo")]
        public string Sexo { get; set; }
        [BsonElement("tipo")]
        public string Tipo { get; set; }
        [BsonElement("debilidad")]
        public string Debilidad { get; set; }
        [BsonElement("puntosbase")]
        public PuntosBase PuntosBase { get; set; }
        [BsonElement("evoluciones")]
        public string Evoluciones {get; set;}
    }
    public class PuntosBase
    {
        [BsonElement("ps")]
        public int PS { get; set; }
        [BsonElement("ataque")]
        public int Ataque { get; set; }
        [BsonElement("defensa")]
        public int Defensa { get; set; }
        [BsonElement("ataqueespecial")]
        public int AtaqueEspecial { get; set; }
        [BsonElement("defensaespecial")]
        public int DefensaEspecial { get; set; }
        [BsonElement("velocidad")]
        public int Velociad { get; set; }
    }
}
