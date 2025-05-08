
namespace Vemo.Gestion.Recursos.Data.Models
{
    public class Roles
    {
        public string Text { get; set; }
    }


    public class RolesAcceso
    {
        public const string Admin = "Admin";
        public const string Usuario = "Usuario";


        public List<Roles> GetRoles()
        {
            var tipo = typeof(RolesAcceso);
            var propiedades = tipo.GetFields();


            var roles = new List<Roles>();


            for (int i = 0; i < propiedades.Length; i++)
            {
                roles.Add(new Roles()
                {
                    Text = propiedades[i].GetValue(null).ToString()
                });
            }


            return roles;

        }
    }
}