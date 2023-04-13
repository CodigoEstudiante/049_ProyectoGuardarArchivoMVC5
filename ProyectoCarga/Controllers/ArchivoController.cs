using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;

using ProyectoCarga.Models;


namespace ProyectoCarga.Controllers
{
    public class ArchivoController : Controller
    {
        static string cadena = "Data Source=(local);Initial Catalog=PRUEBAS;Integrated security=true";
        //NO CREAR ESTO HASTA LA VISTA DE INDEX
        //using ProyectoCarga.Models;
        static List<Archivos> olista = new List<Archivos>();


        //1.-PRIMERO CREAR LA VISTA DE SUBIR
        public ActionResult Subir()
        {
            return View();
        }


        //2.- CREAR EL MODELO - ARCHIVO
        //3.- CREAR PRIMERO LA LOGICA Y POR ULTIMO LA VISTA
        public ActionResult Index()
        {

            olista = new List<Archivos>();

            using (SqlConnection oconexion = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("select * from ARCHIVOS", oconexion);
                cmd.CommandType = CommandType.Text;
                oconexion.Open();

                using (SqlDataReader dr = cmd.ExecuteReader()) {

                    while (dr.Read()) {

                        Archivos archivo_encontrado = new Archivos();
                        archivo_encontrado.IdArchivo = Convert.ToInt32(dr["IdArchivo"]);
                        archivo_encontrado.Nombre = dr["Nombre"].ToString();
                        archivo_encontrado.Archivo = dr["Archivo"] as byte[];
                        archivo_encontrado.Extension = dr["Extension"].ToString();


                        olista.Add(archivo_encontrado);
                    }
                }

            }
            return View(olista);
        }





        [HttpPost]
        public ActionResult SubirArchivo(string Nombre, HttpPostedFileBase Archivo) {

            //using System.IO;
            //using System.Data.SqlClient;
            //using System.Data;

            string Extension = Path.GetExtension(Archivo.FileName);

            MemoryStream ms = new MemoryStream();
            Archivo.InputStream.CopyTo(ms);
            byte[] data = ms.ToArray();
            
            using (SqlConnection oconexion = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("insert into ARCHIVOS(Nombre,Archivo,Extension) values(@nombre,@archivo,@extension)", oconexion);
                cmd.Parameters.AddWithValue("@nombre", Nombre);
                cmd.Parameters.AddWithValue("@archivo", data);
                cmd.Parameters.AddWithValue("@extension", Extension);
                cmd.CommandType = CommandType.Text;
                oconexion.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", "Archivo");
        }

        [HttpPost]
        public FileResult DescargarArchivo(int IdArchivo) {

            Archivos oArchivo = olista.Where(a => a.IdArchivo == IdArchivo).FirstOrDefault();
            string NombreCompleto = oArchivo.Nombre + oArchivo.Extension;

            return File(oArchivo.Archivo, "application/" + oArchivo.Extension.Replace(".","") , NombreCompleto);

        }


    }
}