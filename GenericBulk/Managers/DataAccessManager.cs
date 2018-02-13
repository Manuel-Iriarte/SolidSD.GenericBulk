using GenericBulk.Contratos;
using GenericBulk.Core.Helpers;
using GenericBulk.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace GenericBulk.Managers
{
    public class DataAccessManager<T> where T : IEntity
    {
        private string _conexion;
        private IEnumerable<T> _lista;

        public DataAccessManager(string conexion, IEnumerable<T> lista)
        {
            _lista = lista;
            _conexion = conexion;
        }

        public List<ResultadoCarga> BulkDataProcess()
        {
            var dataAccessEngine = new DataAccessEngine<T>(_conexion);
            var comparadorManager = new ComparadorManager<T>();
            IEnumerable<T> listaDatosOriginales;
            var resultadoComparacion = new ResultadoComparacion<T>();
            var resultadoCarga = new List<ResultadoCarga>();

            // Traer Datos
            listaDatosOriginales = dataAccessEngine.GetEntidades();

            // Comparar
            resultadoComparacion = comparadorManager.Comparar(listaDatosOriginales, _lista);

            // Insertar
            if (resultadoComparacion.ListaInsertar.Any())
                resultadoCarga.AddRange(BulkInsertData(resultadoComparacion.ListaInsertar).ListaDeObjetos);

            // Update
            if (resultadoComparacion.ListaActualizar.Any())
                resultadoCarga.AddRange(BulkUpdateData(resultadoComparacion.ListaActualizar).ListaDeObjetos);

            // Update
            if (resultadoComparacion.ListaBorrar.Any())
                resultadoCarga.AddRange(BulkDeleteData(resultadoComparacion.ListaBorrar).ListaDeObjetos);

            return resultadoCarga;
        }

        public Resultado<ResultadoCarga> BulkUpdateData(IEnumerable<T> entidades)
        {
            var dataAccess = new DataAccessEngine<T>(_conexion);

            var resultado = dataAccess.UpdateEntidades(entidades);

            return resultado;
        }

        public Resultado<ResultadoCarga> BulkInsertData(IEnumerable<T> entidades)
        {
            var dataAccess = new DataAccessEngine<T>(_conexion);

            var resultado = dataAccess.InsertEntidades(entidades);

            return resultado;
        }

        public Resultado<ResultadoCarga> BulkDeleteData(IEnumerable<T> entidades)
        {
            var dataAccess = new DataAccessEngine<T>(_conexion);

            var resultado = dataAccess.DeleteEntidades(entidades);

            return resultado;
        }
    }
}
