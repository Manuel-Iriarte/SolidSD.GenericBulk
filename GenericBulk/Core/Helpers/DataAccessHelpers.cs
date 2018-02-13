using GenericBulk.Contratos;

namespace GenericBulk.Core.Helpers
{
    public static class DataAccessHelpers
    {
        public static string GetNombreTabla<T>(T objeto) where T : IEntity
        {
            return string.Empty;
        }
    }
}
