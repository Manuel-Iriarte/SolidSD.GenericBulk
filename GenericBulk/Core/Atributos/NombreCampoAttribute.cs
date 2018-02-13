using System;

namespace GenericBulk.Core.Atributos
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class NombreCampoAttribute : Attribute
    {
        public NombreCampoAttribute(string nombre)
        {
            _nombre = nombre;
        }

        public string _nombre { get; }
    }
}
