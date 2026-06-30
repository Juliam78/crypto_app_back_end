namespace CryptoAppBackEnd.Infraestructure.Persistence.Models
{
    /// <summary>
    /// Almacenamiento del avatar de una persona como binario (<c>bytea</c>) en una tabla separada
    /// (<c>avatars</c>) para no cargar los bytes en cada consulta de personas. La clave es
    /// <c>person_id</c> (1:1 desnormalizado, sin FK al estilo del resto del esquema).
    /// </summary>
    public class AvatarDbModel
    {
        public int person_id { get; set; }
        public byte[] data { get; set; } = Array.Empty<byte>();
        public string content_type { get; set; } = "image/png";
        public DateTime updated_at { get; set; } = DateTime.UtcNow;
    }
}
