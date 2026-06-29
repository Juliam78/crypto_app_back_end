namespace CryptoAppBackEnd.Domains.Shared
{
    public static class Helpers
    {
        public static void ValidateFields(params (string Name, object Value)[] fields)
        {
            foreach (var (name, value) in fields)
            {
                switch (value)
                {
                    case null:
                        throw new ArgumentNullException(name);

                    case string str when string.IsNullOrWhiteSpace(str):
                        throw new ArgumentException($"El campo '{name}' no puede estar vacío.", name);

                    case decimal dec when dec < 0:
                        throw new ArgumentException($"El campo '{name}' no puede ser negativo.", name);

                    case char c when char.IsWhiteSpace(c):
                        throw new ArgumentException($"El campo '{name}' no puede ser un espacio en blanco.", name);

                    case int i when i < 0:
                        throw new ArgumentException($"El campo '{name}' no puede ser negativo.", name);
                }
            }
        }
    }
}
