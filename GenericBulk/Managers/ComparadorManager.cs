using GenericBulk.Contratos;
using GenericBulk.Core.Atributos;
using GenericBulk.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenericBulk.Managers
{
    public class ComparadorManager<T> where T : IEntity
    {
        public ResultadoComparacion<T> Comparar(IEnumerable<T> listaOriginal, IEnumerable<T> lista)
        {
            ResultadoComparacion<T> resultadoComparacion = new ResultadoComparacion<T>();
            List<T> listaInserts = new List<T>();
            List<T> listaUpdates = new List<T>();
            List<T> listaDeletes = new List<T>();
            List<T> listaDescartados = new List<T>();

            Type type = typeof(T);
            PropertyInfo[] propiedades = typeof(T).GetProperties();
            FieldInfo[] fields = type.GetFields();
            PropertyInfo campoComparable = propiedades
                                            .FirstOrDefault(p => Attribute.IsDefined(p, typeof(CampoComparableAttribute)));


            // Limpio Lista donde el campo de comparacion es nulo o vacio
            lista = lista.Where(l =>
            {
                bool retorno = true;
                var valorComparable = l
                                        .GetType()
                                        .GetProperties()
                                        .Where(propiedad => propiedad.Name == campoComparable.Name)
                                        .Select(propiedad => propiedad.GetValue(l, null) as string)
                                        .FirstOrDefault();

                if (String.IsNullOrEmpty(valorComparable))
                {
                    listaDescartados.Add(l);
                    retorno = false;
                }

                return retorno;
            });

            // Deletes
            listaOriginal.ToList().ForEach(lo =>
            {
                string valorl;
                var valor = lo.GetType().GetProperties()
                                .Where(propiedad => propiedad.Name == campoComparable.Name)
                                .Select(propiedad => propiedad.GetValue(lo, null) as string)
                                .FirstOrDefault();

                var objeto = lista.ToList().Where(l =>
                {
                    var resultado = false;
                    valorl = l.GetType().GetProperties()
                                 .Where(propiedad => propiedad.Name == campoComparable.Name)
                                 .Select(propiedad => propiedad.GetValue(l, null) as string)
                                 .FirstOrDefault();

                    if (valor == valorl)
                        resultado = true;

                    return resultado;
                }).FirstOrDefault();

                if (objeto == null)
                    listaDeletes.Add(lo);
            });

            // Inserts y Updates
            lista.ToList().ForEach(objeto =>
            {
                bool esInsert = true;

                var valor = objeto
                               .GetType()
                               .GetProperties()
                               .Where(propiedad => propiedad.Name == campoComparable.Name)
                               .Select(propiedad => propiedad.GetValue(objeto, null) as string)
                               .FirstOrDefault();

                listaOriginal.ToList().ForEach(objetoOriginal =>
                {
                    var valorOriginal = objetoOriginal
                                        .GetType()
                                        .GetProperties()
                                        .Where(propiedad => propiedad.Name == campoComparable.Name)
                                        .Select(propiedad => propiedad.GetValue(objetoOriginal, null) as string)
                                        .FirstOrDefault();

                    if (valorOriginal == valor)
                    {
                        esInsert = false;

                        if (ObjetoHelper<T>.Compara(objetoOriginal, objeto) != 0)
                        {
                            var nombreId = objetoOriginal
                                                .GetType().GetProperties()
                                                .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute)) == true)
                                                .Select(propiedad => propiedad.Name).FirstOrDefault();

                            var id = objetoOriginal
                                        .GetType().GetProperty(nombreId)
                                        .GetValue(objetoOriginal, null);

                            objeto.GetType().GetProperty(nombreId).SetValue(objeto, (int)id);

                            listaUpdates.Add(objeto);
                        }
                    }
                });

                if (esInsert)
                    listaInserts.Add(objeto);
            });

            resultadoComparacion.ListaActualizar = listaUpdates;
            resultadoComparacion.ListaInsertar = listaInserts;
            resultadoComparacion.ListaBorrar = listaDeletes;

            return resultadoComparacion;
        }
    }
}
