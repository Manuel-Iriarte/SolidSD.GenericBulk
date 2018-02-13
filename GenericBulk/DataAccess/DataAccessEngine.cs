using Dapper;
using GenericBulk.Contratos;
using GenericBulk.Core.Atributos;
using GenericBulk.Core.Enums;
using GenericBulk.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GenericBulk.DataAccess
{
    public class DataAccessEngine<T> where T : IEntity
    {
        private readonly string _conexion;

        public DataAccessEngine(string conexion)
        {
            _conexion = conexion;
        }

        public IEnumerable<T> GetEntidades()
        {
            IEnumerable<T> resultado = null;
            var tabla = typeof(T).Name;
            var campos = string.Empty;

            typeof(T)
                .GetProperties()
                .Select(p => p.Name)
                .ToList()
                .ForEach(p => campos += $"{ p },");

            campos = campos.Substring(0, campos.Length - 1);

            using (IDbConnection db = new SqlConnection(_conexion))
            {
                resultado = db.Query<T>($"SELECT { campos } FROM { tabla }").ToList();
            }

            return resultado;
        }

        public Resultado<ResultadoCarga> InsertEntidades(IEnumerable<T> entidades)
        {
            var resultado = new Resultado<ResultadoCarga>();
            resultado.ListaDeObjetos = new List<ResultadoCarga>();


            foreach (var entidad in entidades)
            {
                resultado.ListaDeObjetos.Add(InsertEntidad(entidad));
            }

            return resultado;
        }

        public ResultadoCarga InsertEntidad(T entidad)
        {
            var resultado = new ResultadoCarga { TipoTarea = TipoTarea.Crear };
            var nombreTabla = typeof(T).Name;
            var propiedades = typeof(T)
                                .GetProperties()
                                .Where(p => Attribute.IsDefined(p, typeof(IgnorarAlInsertarAttribute)) == false)
                                .ToArray();
            var valores = string.Empty;
            var campos = string.Empty;

            try
            {
                foreach (var prop in propiedades)
                {
                    if (!Attribute.IsDefined(prop, typeof(IgnorarAlInsertarAttribute)))
                    {
                        if (!Attribute.IsDefined(prop, typeof(KeyAttribute)))
                        {
                            var comillas = string.Empty;
                            if (prop.PropertyType.Name == "String" || prop.PropertyType.Name == "DateTime") comillas = "'";

                            campos += prop.Name + ",";
                            valores += comillas + prop.GetValue(entidad) + comillas + ",";
                        }
                    }
                }

                campos = campos.Substring(0, campos.Length - 1);
                valores = valores.Substring(0, valores.Length - 1);

                using (IDbConnection db = new SqlConnection(_conexion))
                {
                    db.Query<T>($"INSERT INTO { nombreTabla }({campos})VALUES({valores}) ");
                }

                resultado.Mensaje = "Creado";
            }
            catch (Exception ex)
            {
                resultado.Mensaje = "Error al crear: " + ex.Message;
            }

            return resultado;
        }

        public Resultado<ResultadoCarga> UpdateEntidades(IEnumerable<T> entidades)
        {
            var resultado = new Resultado<ResultadoCarga>();
            resultado.ListaDeObjetos = new List<ResultadoCarga>();


            foreach (var entidad in entidades)
            {
                resultado.ListaDeObjetos.Add(UpdateEntidad(entidad));
            }

            return resultado;
        }

        public ResultadoCarga UpdateEntidad(T entidad)
        {
            var resultado = new ResultadoCarga { TipoTarea = TipoTarea.Actualizar };

            var nombreTabla = typeof(T).Name;
            var propiedades = typeof(T).GetProperties()
                                            .Where(p => Attribute.IsDefined(p, typeof(IgnorarAlActualizarAttribute)) == false)
                                            .ToArray();
            var id = string.Empty;
            var idNombreCampo = string.Empty;
            var sets = string.Empty;


            try
            {
                foreach (var prop in propiedades)
                {
                    if (!Attribute.IsDefined(prop, typeof(IgnorarAlActualizarAttribute)))
                    {
                        if (Attribute.IsDefined(prop, typeof(KeyAttribute)))
                        {
                            idNombreCampo = prop.Name;
                            id += prop.GetValue(entidad);
                        }
                        else
                        {
                            var comillas = string.Empty;
                            if (prop.PropertyType.Name == "String" || prop.PropertyType.Name == "DateTime") comillas = "'";

                            sets += $" {prop.Name} = {comillas}{prop.GetValue(entidad)}{comillas},";
                        }
                    }
                }

                sets = sets.Substring(0, sets.Length - 1);

                using (IDbConnection db = new SqlConnection(_conexion))
                {
                    string query = string.Empty;

                    query += $"UPDATE { nombreTabla } SET {sets} WHERE {idNombreCampo} = {id}";

                    db.Query<T>(query);
                }

                resultado.Mensaje = "Actualizado";
            }
            catch (Exception ex)
            {
                resultado.Mensaje = "Error al actualizar: " + ex.Message;
            }

            return resultado;
        }

        public Resultado<ResultadoCarga> DeleteEntidades(IEnumerable<T> entidades)
        {
            var resultado = new Resultado<ResultadoCarga>();
            resultado.ListaDeObjetos = new List<ResultadoCarga>();


            foreach (var entidad in entidades)
            {
                resultado.ListaDeObjetos.Add(DeleteEntidad(entidad));
            }

            return resultado;
        }

        public ResultadoCarga DeleteEntidad(T entidad)
        {
            var resultado = new ResultadoCarga { TipoTarea = TipoTarea.Eliminar };

            var nombreTabla = typeof(T).Name;
            var propiedades = typeof(T).GetProperties()
                                            .Where(p => Attribute.IsDefined(p, typeof(IgnorarAlBorrarAttribute)) == false)
                                            .ToArray();
            var id = string.Empty;
            var idNombreCampo = string.Empty;
            var sets = string.Empty;


            try
            {
                foreach (var prop in propiedades)
                {
                    if (!Attribute.IsDefined(prop, typeof(IgnorarAlBorrarAttribute)))
                    {
                        if (Attribute.IsDefined(prop, typeof(KeyAttribute)))
                        {
                            idNombreCampo = prop.Name;
                            id += prop.GetValue(entidad);
                        }
                        else
                        {
                            var comillas = string.Empty;
                            if (prop.PropertyType.Name == "String" || prop.PropertyType.Name == "DateTime") comillas = "'";

                            sets += $" {prop.Name} = {comillas}{prop.GetValue(entidad)}{comillas},";
                        }
                    }
                }

                sets = sets.Substring(0, sets.Length - 1);

                using (IDbConnection db = new SqlConnection(_conexion))
                {
                    string query = string.Empty;

                    query += $"UPDATE { nombreTabla } SET {sets} WHERE {idNombreCampo} = {id}";

                    db.Query<T>(query);
                }

                resultado.Mensaje = "Actualizado";
            }
            catch (Exception ex)
            {
                resultado.Mensaje = "Error al actualizar: " + ex.Message;
            }

            return resultado;
        }
    }
}
