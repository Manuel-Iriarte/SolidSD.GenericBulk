using System;

namespace GenericBulk.Core.Atributos
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NombreTablaAttribute : Attribute
    {
        private string _nombre;

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }


        public NombreTablaAttribute(string nombre)
        {
            _nombre = nombre;
        }
    }
}
