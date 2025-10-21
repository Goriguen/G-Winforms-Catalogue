using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;
using negocio;

namespace negocio
{
    public class ArticuloNegocio
    {
        private AccesoDatos datos;

        public ArticuloNegocio()
        {
            datos = new AccesoDatos();
        }

        public List<Articulo> despliegue()
        {
            //Se genera una lista para acceder a ella posteriormente
            List<Articulo> lista = new List<Articulo>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "SELECT a.Id, a.Codigo, a.Nombre, a.Descripcion, m.Id AS IdMarca, m.Descripcion AS Marca, c.Id AS IdCategoria, c.Descripcion AS Categoria, a.ImagenUrl, a.Precio FROM ARTICULOS a INNER JOIN MARCAS m ON a.IdMarca = m.Id INNER JOIN CATEGORIAS c ON a.IdCategoria = c.Id ;";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = Convert.ToInt32(lector["Id"]);
                    aux.codArticulo = Convert.ToString(lector["Codigo"]);
                    aux.Nombre = Convert.ToString(lector["Nombre"]);
                    aux.Descripcion = Convert.ToString(lector["Descripcion"]);

                    aux.Marca = new Marca();
                    if(!(lector["IdMarca"] is DBNull))
                        aux.Marca.Id = Convert.ToInt32(lector["IdMarca"]);
                    aux.Marca.Descripcion = Convert.ToString(lector["Marca"]);

                    aux.Categoria = new Categoria();
                    if (!(lector["IdCategoria"] is DBNull))
                        aux.Categoria.Id = Convert.ToInt32(lector["IdCategoria"]);
                    aux.Categoria.Descripcion = Convert.ToString(lector["Categoria"]);

                    aux.urlImagen = lector["ImagenUrl"] is DBNull ? "" : Convert.ToString(lector["ImagenUrl"]);

                    aux.Precio = lector["Precio"] is DBNull ? 0m : Convert.ToDecimal(lector["Precio"]);

                    //Articulo aux = new Articulo();
                    //aux.Id = (int)lector["Id"];
                    //aux.codArticulo = (string)lector["Codigo"];
                    //aux.Nombre = (string)lector["Nombre"];
                    //aux.Descripcion = (string)lector["Descripcion"];
                    //aux.Marca = new Marca();
                    //aux.Marca.Id = (int)lector["IdMarca"];
                    //aux.Categoria = new Categoria();
                    //aux.Categoria.Id = (int)lector["IdCategoria"];
                    //if (!(lector["ImagenUrl"] is DBNull))
                    //    aux.urlImagen = (string)lector["ImagenUrl"];
                    //aux.Precio = Convert.ToDouble(lector["Precio"]);
                    //aux.Marca.Descripcion = (string)lector["Marca"];
                    //aux.Categoria.Descripcion = (string)lector["Categoria"];

                    lista.Add(aux);
                }

                lector.Close();
                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        public void agregar(Articulo nuevo)
        {
            //AccesoDatos datos = new AccesoDatos();

            try
            {
                //datos.setearConsulta("insert into ARTICULOS (Codigo, Nombre, Descripcion, Precio)values(" + nuevo.codArticulo + " , '" + nuevo.Nombre + "', '" + nuevo.Descripcion + "' , " + nuevo.Precio + " )");
                datos.setearConsulta("INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, ImagenUrl, IdMarca, IdCategoria, Precio) VALUES (@Codigo, @Nombre, @Descripcion, @urlImagen, @IdMarca, @IdCategoria, @Precio)");


                datos.setearParametro("@Codigo", nuevo.codArticulo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@urlImagen", nuevo.urlImagen);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@Precio", nuevo.Precio);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar (Articulo modificar)
        {
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @codArt, Nombre = @nombre, Descripcion = @descripcion, IdMarca = @idMarca, IdCategoria = @idCategoria, ImagenUrl = @img, Precio = @precio where Id = @id");

                datos.setearParametro("@codArt", modificar.codArticulo);
                datos.setearParametro("@nombre", modificar.Nombre);
                datos.setearParametro("@descripcion", modificar.Descripcion);
                datos.setearParametro("@idMarca", modificar.Marca.Id);
                datos.setearParametro("@idCategoria", modificar.Categoria.Id);
                datos.setearParametro("@img", modificar.urlImagen);
                datos.setearParametro("@precio", modificar.Precio);
                datos.setearParametro("@id", modificar.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion(); 
            }
        }

        public void eliminar(int id)
        {
            try
            {
                datos.setearConsulta("delete from ARTICULOS where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> lista = new List<Articulo>();

            try
            {
                if (string.IsNullOrEmpty(filtro))
                    throw new Exception("El filtro no puede estar vacío.");

                string consulta = "SELECT a.Id, a.Codigo, a.Nombre, a.Descripcion, " +
                                   "m.Id AS IdMarca, m.Descripcion AS Marca, " +
                                   "c.Id AS IdCategoria, c.Descripcion AS Categoria, " +
                                   "a.ImagenUrl, a.Precio " +
                                   "FROM ARTICULOS a " +
                                   "INNER JOIN MARCAS m ON a.IdMarca = m.Id " +
                                   "INNER JOIN CATEGORIAS c ON a.IdCategoria = c.Id WHERE ";

                string campoBD = "";

                switch (campo)
                {
                    case "Código de Articulo":
                        campoBD = "a.Codigo";
                        break;
                    case "Nombre":
                        campoBD = "a.Nombre";
                        break;
                    case "Descripción":
                        campoBD = "a.Descripcion";
                        break;
                    case "Precio":
                        campoBD = "a.Precio";
                        break;
                    case "Marca":
                        campoBD = "m.Descripcion";
                        break;
                    case "Categoria":
                        campoBD = "c.Descripcion";
                        break;
                    default:
                        throw new Exception("Campo no válido.");
                }

                // Armo el criterio
                if (campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += campoBD + " > " + filtro;
                            break;
                        case "Menor a":
                            consulta += campoBD + " < " + filtro;
                            break;
                        case "Igual a":
                            consulta += campoBD + " = " + filtro;
                            break;
                        default:
                            throw new Exception("Criterio no válido para Precio.");
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += campoBD + " LIKE '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += campoBD + " LIKE '%" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += campoBD + " LIKE '%" + filtro + "%'";
                            break;
                        default:
                            throw new Exception("Criterio no válido para texto.");
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.codArticulo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Marca = new Marca
                    {
                        Id = (int)datos.Lector["IdMarca"],
                        Descripcion = (string)datos.Lector["Marca"]
                    };
                    aux.Categoria = new Categoria
                    {
                        Id = (int)datos.Lector["IdCategoria"],
                        Descripcion = (string)datos.Lector["Categoria"]
                    };
                    aux.urlImagen = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = (decimal)datos.Lector["Precio"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}