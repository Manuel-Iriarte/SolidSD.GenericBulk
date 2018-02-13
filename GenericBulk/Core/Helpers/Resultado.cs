using GenericBulk.Core.Enums;
using System.Collections.Generic;

namespace GenericBulk.Core.Helpers
{
    public class Resultado<T> : Resultado where T : class
    {
        public T Objeto { get; set; }
        public List<T> ListaDeObjetos { get; set; }
    }

    public class Resultado
    {
        public bool Ok { get; set; } = false;
    }

    public class ResultadoCarga
    {
        public string Mensaje { get; set; }
        public TipoTarea TipoTarea { get; set; }
    }

    public class ResultadoComparacion<T>
    {
        public List<T> ListaInsertar { get; set; }
        public List<T> ListaActualizar { get; set; }
        public List<T> ListaBorrar { get; set; }
    }
}
