using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class Usuario
    {
        public static ML.Result GetPassword(string email)
        {
            ML.Result result = new ML.Result();
            try
            {
                using(DL.LsantosMovieContext context = new DL.LsantosMovieContext())
                {
                    var objusuario = context.Usuarios.FromSqlRaw($"UsuarioGetPasswordByEmail '{email}'").AsEnumerable().FirstOrDefault();

                    if(objusuario != null)
                    {
                        ML.Usuario usuario = new ML.Usuario();

                        usuario.IdUsuario = objusuario.IdUsuario;
                        usuario.Nombre = objusuario.Nombre;
                        usuario.ApellioPaterno = objusuario.ApellidoPaterno;
                        usuario.ApellidoMaterno = objusuario.ApellidoMaterno;
                        usuario.Email = objusuario.Email;
                        usuario.Password = objusuario.Password;

                        result.Object = usuario;
                        result.Correct = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.EX = ex;
                result.Message = "Error: "+result.EX;
            }

            return result;
        }
    }
}
