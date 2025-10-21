using helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using System.Configuration;

namespace helper
{
    public static class help_negocio
    {
        public static string cargarImagenGenerica()
        {
            return "https://cdn-icons-png.flaticon.com/512/1828/1828843.png";
        }

        //Se encarga de cargar la imagen del dgv, recibe 2 argumentos: la dirección (url o local) y el picturebox, si falla, recoge la excepción y llama a la función de cargar imagen genérica (sin imagen)
        public static void cargarImagen(string imagen, PictureBox pbx)
        {
            try
            {
                pbx.Image = null;
                pbx.Refresh();

                if (string.IsNullOrEmpty(imagen))
                {
                    pbx.Load(cargarImagenGenerica());
                    return;
                }

                if (imagen.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    pbx.Load(imagen);
                }
                else
                {
                    if (File.Exists(imagen))
                    {
                        pbx.Load(imagen);
                    }
                    else
                    {
                        pbx.Load(cargarImagenGenerica());
                    }
                }
            }
            catch (System.Net.WebException)
            {
                pbx.Load(cargarImagenGenerica());
            }
            catch (Exception)
            {
                pbx.Load(cargarImagenGenerica());
            }
        }

        // Selecciona la fila correspondiente al artículo por ID y establece el foco en una celda visible
        // para que el DataGridView refleje visualmente la selección con la flecha ►
        public static void seleccionarArtDGV(DataGridView dgv, int idArtBuscado)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                var art = row.DataBoundItem as Articulo;
                if (art != null && art.Id == idArtBuscado)
                {
                    //dgv.ClearSelection();
                    row.Selected = true;
                    dgv.CurrentCell = row.Cells.Cast<DataGridViewCell>().FirstOrDefault(c => c.Visible); // Establece el foco en la primera celda visible de la fila seleccionada para que el DGV muestre la flecha ►
                    break;
                }
            }
        }

        // Este método se ejecuta al volver del formulario de alta o modificación.
        // Si el usuario hizo clic en Aceptar, recarga la grilla y selecciona el artículo modificado o agregado.
        // Si no se puede determinar el ID del artículo, muestra un mensaje de advertencia.
        public static void checkFrmArticulo(DataGridView dgv,Form form, int? idArtSeleccionado, Action recargarGrilla, PictureBox pbx)
        {
            if (form.DialogResult == DialogResult.OK)
            {
                if (idArtSeleccionado.HasValue)
                {
                    recargarGrilla();
                    help_negocio.seleccionarArtDGV(dgv, idArtSeleccionado.Value);
                    dgv.Focus();

                    //con esta condición, forzamos a que la imagen del pbx se actualice a la seleccionada después de modificar
                    if (dgv.CurrentRow != null)
                    {
                        Articulo seleccionado = (Articulo)dgv.CurrentRow.DataBoundItem;
                        help_negocio.cargarImagen(seleccionado.urlImagen, pbx);
                    }
                }
                else
                {
                    MessageBox.Show("No se pudo determinar qué artículo fue afectado.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static string guardarImagenLocal(string rutaOrigen)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaOrigen) || !File.Exists(rutaOrigen))
                    return null;

                string rutaDestino = ConfigurationManager.AppSettings["Imagen_TPFINAL"];
                string nombreArchivo = Path.GetFileName(rutaOrigen);
                string pathCompletoDestino = Path.Combine(rutaDestino, nombreArchivo);

                if (File.Exists(pathCompletoDestino))
                {
                    DialogResult respuesta = MessageBox.Show("La imagen ya existe. ¿Deseás sobrescribirla?", "Imagen Existente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (respuesta == DialogResult.No)
                    {
                        nombreArchivo = Path.GetFileNameWithoutExtension(nombreArchivo)
                                        + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                                        + Path.GetExtension(nombreArchivo);
                        pathCompletoDestino = Path.Combine(rutaDestino, nombreArchivo);
                    }
                }

                File.Copy(rutaOrigen, pathCompletoDestino, true);

                return pathCompletoDestino;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar imagen: " + ex.Message);
                return null;
            }
        }
    }
}

//método exiliado (anterior para cargar imagen)
//private void cargarImagen(string imagen)
//{
//    try
//    {
//        pbxArticulo.Image = null;
//        pbxArticulo.Refresh();
//        Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

//        if (seleccionado.urlImagen.StartsWith("http"))
//        {
//            pbxArticulo.Load(seleccionado.urlImagen);
//        }
//        else
//        {
//            if (File.Exists(seleccionado.urlImagen))
//            {
//                pbxArticulo.Load(seleccionado.urlImagen);
//            }
//            else
//            {
//                pbxArticulo.Load(help_negocio.cargarImagenGenerica());
//            }
//        }
//    }
//    catch (System.Net.WebException)
//    {
//        pbxArticulo.Load(help_negocio.cargarImagenGenerica());

//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show(ex.ToString());
//        pbxArticulo.Load(help_negocio.cargarImagenGenerica());
//    }
//}


//    }

//

// public static void checkFrmArticulo(DataGridView dgv,Form form, int? idArtSeleccionado, Action recargarGrilla, PictureBox pbx)
//{
//    if (form.DialogResult == DialogResult.OK)
//    {
//        if (idArtSeleccionado.HasValue)
//        {
//            recargarGrilla();
//            help_negocio.seleccionarArtDGV(dgv, idArtSeleccionado.Value);

//            //con esta condición, forzamos a que la imagen del pbx se actualice a la seleccionada después de modificar
//            if (dgv.CurrentRow != null)
//            {
//                dgv.CurrentCell = dgv.CurrentRow.Cells[0];
//                dgv.Focus();
//                Articulo seleccionado = (Articulo)dgv.CurrentRow.DataBoundItem;
//                help_negocio.cargarImagen(seleccionado.urlImagen, pbx);
//            }
//        }
//        else
//        {
//            MessageBox.Show("No se pudo determinar qué artículo fue afectado.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        }
//    }
//}


//public static void cargarImagen(string imagen, PictureBox pbx)
//{
//    try
//    {
//        pbx.Image = null;
//        pbx.Refresh();

//        if (string.IsNullOrEmpty(imagen))
//        {
//            pbx.Load(help_negocio.cargarImagenGenerica());
//            return;
//        }

//        if (imagen.StartsWith("http", StringComparison.OrdinalIgnoreCase))
//        {
//            pbx.Load(imagen);
//        }
//        else
//        {
//            if (File.Exists(imagen))
//            {
//                pbx.Load(imagen);
//            }
//            else
//            {
//                pbx.Load(help_negocio.cargarImagenGenerica());
//            }
//        }
//    }
//    catch (System.Net.WebException)
//    {
//        pbx.Load(help_negocio.cargarImagenGenerica());

//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show(ex.ToString());
//        pbx.Load(help_negocio.cargarImagenGenerica());
//    }
//}