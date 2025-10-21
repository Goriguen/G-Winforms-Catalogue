using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using negocio;
using dominio;
using helper;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Net.Mail;

namespace modelo
{
    public partial class frmArticulo : Form
    {
        private List<Articulo> listaArticulos;
        private bool bloquearEventoSeleccion = false;
        public frmArticulo()
        {
            InitializeComponent();
            this.Text = "Menú Principal";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
            cbxCampo.Items.Clear();
            cbxCampo.Items.Add("");
            cbxCampo.Items.Add("Código de Articulo");
            cbxCampo.Items.Add("Nombre");
            cbxCampo.Items.Add("Descripción");
            cbxCampo.Items.Add("Precio");
            cbxCampo.Items.Add("Marca");
            cbxCampo.Items.Add("Categoria");
            cbxCampo.SelectedIndex = 0;

            cbxCriterio.Items.Clear();
            cbxCriterio.Items.Add(""); // vacío también
            cbxCriterio.SelectedIndex = 0;
            cbxCriterio.Enabled = false;

        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            //evalúa si la condición es verdadera (si hubo artículo modificado/agregado, para actualizar la grilla con esa selección
            if (bloquearEventoSeleccion) 
                return;

            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                help_negocio.cargarImagen(seleccionado.urlImagen, pbxArticulo);
            }

            actualizarTextoBotonBaja();
        }

        private void cargar()
        {
            bloquearEventoSeleccion = true; //impide que se carga el 1er articulo de la grilla, para que se quede seleccionado el articulo modificado/agregado (en caso de que lo hubiese) 
            ArticuloNegocio negocio = new ArticuloNegocio();
            bool errorDGB = false;

            try
            {
                listaArticulos = negocio.despliegue();

                //se evalúa si el está tildado el checkbox, y de ser así, se muestran todos los articulos
                //que no empiecen con [Eliminado]
                if (!ckbxEliminados.Checked)
                {
                    listaArticulos = listaArticulos.FindAll(x => !x.Nombre.StartsWith("[ELIMINADO]"));
                }

                dgvArticulos.DataSource = null;
                dgvArticulos.DataSource = listaArticulos;
                dgvArticulos.DataError += dgvArticulos_DataError;

                //no cargar la imagen acá, se contradice con el selection changed para que actualice a la imagen del articulo modificado/seleccionado
                //help_negocio.cargarImagen(listaArticulos[0].urlImagen, pbxArticulo);
                ocultarColumnas();

                if (dgvArticulos.Columns["Precio"] != null)
                {
                    dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "#,##0.00"; // Alternativa a "N2"
                    dgvArticulos.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                bloquearEventoSeleccion = false; //se devuelve a su estado inicial, actualizándose al 1er artículo nuevamente, sino quedaría siempre seleccionando el articulo modificado/agregado.

                //Fuerza a que cargue la imagen al iniciar la APP, validando
                if (dgvArticulos.Rows.Count > 0)
                {
                    var primerFila = dgvArticulos.Rows[0];
                    Articulo seleccionado = (Articulo)primerFila.DataBoundItem;
                    help_negocio.cargarImagen(seleccionado.urlImagen, pbxArticulo);

                    primerFila.Selected = true; // fuerza la aparición del puntero
                    dgvArticulos.CurrentCell = primerFila.Cells.Cast<DataGridViewCell>().FirstOrDefault(c => c.Visible); // fuerza foco para el DGV
                }

                actualizarTextoBotonBaja();

            }
            catch (Exception ex)
            {
                errorDGB = true;
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                bloquearEventoSeleccion = false;

                if (errorDGB)
                {
                    //muestra la lista que se está pidiendo, pero no se muestra en el dgv
                    string datos = "";
                    foreach (var articulo in listaArticulos)
                    {
                        datos += $"Nombre: {articulo.Nombre}, Descripción: {articulo.Descripcion}, Imagen: {articulo.urlImagen}\n";
                    }
                    MessageBox.Show("Error al mostrar datos en el DataGridView.\n\n" + datos);
                }
            }
        }

        // Método que actualiza el texto del botón "Dar de baja / Restaurar" según el estado del artículo seleccionado
        // Si el artículo comienza con "[ELIMINADO]", el botón muestra "Restaurar"
        // Caso contrario, muestra "Dar de baja"
        private void actualizarTextoBotonBaja()
        {
            if (dgvArticulos.CurrentRow == null)
            {
                btnBajaRestaurar.Text = "Dar de baja";
                return;
            }

            Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

            if (seleccionado.Nombre.StartsWith("[ELIMINADO]"))
                btnBajaRestaurar.Text = "Restaurar";
            else
                btnBajaRestaurar.Text = "Dar de baja";
        }

        private void btnAgregarArticulo_Click(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            //No permite salir de la ventana hasta terminar de trabajar [no abrir más de 1 ventana]
            alta.ShowDialog();
            help_negocio.checkFrmArticulo(dgvArticulos, alta, alta.IdArticuloSeleccionado, cargar, pbxArticulo);
            //cargar();
        }

        public void ocultarColumnas()
        {
            dgvArticulos.Columns["urlImagen"].Visible = false;
            dgvArticulos.Columns["codArticulo"].Visible = false;
            dgvArticulos.Columns["Id"].Visible = false;
            //dgvArticulos.Columns["IdMarca"].Visible = false;
            //dgvArticulos.Columns["IdCategoria"].Visible = false;
        }

        //MÉTODO PARA CAPTURAR ERRORES EN EL DATAGRIDVIEW
        private void dgvArticulos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Error en la celda [{e.ColumnIndex}] - {e.Exception.Message}");
            e.ThrowException = false; // Evita que el programa se cierre por el error
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow == null)
            {
                MessageBox.Show("Por favor seleccione un artículo para modificar.");
                return;
            }
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
            //No permite salir de la ventana hasta terminar de trabajar [no abrir más de 1 ventana]
            modificar.ShowDialog();
            help_negocio.checkFrmArticulo(dgvArticulos, modificar, modificar.IdArticuloSeleccionado, cargar, pbxArticulo);
            //cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;
            try
            {
                //valida si no hay un artículo seleccionado en el DGV
                if (dgvArticulos.CurrentRow == null)
                {
                    MessageBox.Show("Debe seleccionar un artículo para eliminarlo.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                DialogResult respuesta = MessageBox.Show("¿Está seguro de querer eliminar el articulo?", "Eliminando", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if(respuesta == DialogResult.OK)
                {
                seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                negocio.eliminar(seleccionado.Id);
                cargar();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnBajaRestaurar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow == null)
            {
                MessageBox.Show("Por favor seleccioná un artículo.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            ArticuloNegocio negocio = new ArticuloNegocio();

            if (!seleccionado.Nombre.StartsWith("[ELIMINADO]"))
            {
                // Dar de baja lógica
                DialogResult confirmacion = MessageBox.Show("¿Seguro que querés dar de baja este artículo?", "Confirmar Baja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    seleccionado.Nombre = "[ELIMINADO] " + seleccionado.Nombre;
                    negocio.modificar(seleccionado);

                    cargar(); // Actualiza DGV después de modificar
                }
            }
            else
            {
                // Restaurar artículo
                DialogResult confirmacion = MessageBox.Show("¿Querés restaurar este artículo?", "Confirmar Restauración", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    seleccionado.Nombre = seleccionado.Nombre.Replace("[ELIMINADO] ", "");
                    negocio.modificar(seleccionado);

                    cargar(); // ACTUALIZA DGV después de modificar
                }
            }
        }

        private void btnFiltroBuscar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (cbxCampo.SelectedIndex <= 0 || cbxCriterio.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Por favor completá todos los campos del filtro.");
                    return;
                }



                if (dgvArticulos.Columns["Precio"] != null)
                {
                    dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "#,##0.00"; // Formato personalizado
                    dgvArticulos.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (cbxCampo.SelectedItem.ToString() == "Precio" && !decimal.TryParse(txtFiltroAvanzado.Text, out _))
                {
                    MessageBox.Show("Por favor ingresá solo números para filtrar por precio.");
                    txtFiltroAvanzado.BackColor = Color.MistyRose;
                    return;
                }
                else
                {
                    txtFiltroAvanzado.BackColor = SystemColors.Window;
                }

                string campo = cbxCampo.SelectedItem.ToString();
                string criterio = cbxCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                dgvArticulos.DataSource = negocio.filtrar(campo, criterio, filtro);
                ocultarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {

            List<Articulo> listaFiltrada;
            string filtro = txtFiltro.Text;

            // Si hay texto escrito en filtro rápido, desactiva el filtro avanzado
            if (filtro.Length >= 1)
            {
                cbxCampo.Enabled = false;
                cbxCriterio.Enabled = false;
                txtFiltroAvanzado.Enabled = false;
                btnFiltroBuscar.Enabled = false;

                listaFiltrada = listaArticulos.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Categoria.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                // Si el filtro rápido está vacío, se reactiva el filtro avanzado
                cbxCampo.Enabled = true;
                cbxCriterio.Enabled = cbxCampo.SelectedIndex > 0;
                txtFiltroAvanzado.Enabled = cbxCampo.SelectedIndex > 0;
                btnFiltroBuscar.Enabled = cbxCampo.SelectedIndex > 0;

                listaFiltrada = listaArticulos;
            }


            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listaFiltrada;
            ocultarColumnas();

            if (dgvArticulos.Columns["Precio"] != null)
            {
                dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "#,##0.00";
                dgvArticulos.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void cbxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            controlarFiltroRapido();
            cbxCriterio.Items.Clear();
            cbxCriterio.Enabled = false;
            txtFiltroAvanzado.Text = "";
            txtFiltroAvanzado.Enabled = false;
            btnFiltroBuscar.Enabled = false;

            if (cbxCampo.SelectedIndex <= 0)
            {
                if (string.IsNullOrWhiteSpace(txtFiltro.Text))
                {
                    cbxCampo.Enabled = true;
                }
                cargar();
                return;
            }

            cbxCampo.Enabled = true;
            string opcion = cbxCampo.SelectedItem.ToString();

            switch (opcion)
            {
                case "Precio":
                    cbxCriterio.Items.Add("Mayor a");
                    cbxCriterio.Items.Add("Menor a");
                    cbxCriterio.Items.Add("Igual a");
                    break;
                default:
                    cbxCriterio.Items.Add("Comienza con");
                    cbxCriterio.Items.Add("Termina con");
                    cbxCriterio.Items.Add("Contiene");
                    break;
            }

            cbxCriterio.Enabled = true;
            txtFiltroAvanzado.Enabled = true;
            btnFiltroBuscar.Enabled = true;
            txtFiltroAvanzado.Text = "";
            cbxCriterio.SelectedIndex = -1;
        }

        private void ckbxEliminados_CheckedChanged(object sender, EventArgs e)
        {
            cargar();
        }

        private void dgvArticulos_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            {
                if (dgvArticulos.Rows[e.RowIndex].DataBoundItem != null)
                {
                    Articulo articulo = (Articulo)dgvArticulos.Rows[e.RowIndex].DataBoundItem;

                    if (articulo.Nombre.StartsWith("[ELIMINADO]"))
                    {
                        dgvArticulos.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                    }
                    else
                    {
                        dgvArticulos.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                frmDetalleArticulo detalle = new frmDetalleArticulo(seleccionado);
                detalle.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor seleccioná un artículo.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void controlarFiltroRapido()
        {
            // Verifica si hay algo en el filtro avanzado
            bool filtroAvanzadoActivo = cbxCampo.SelectedIndex > 0 || cbxCriterio.SelectedIndex > 0 || !string.IsNullOrWhiteSpace(txtFiltroAvanzado.Text);

            if (filtroAvanzadoActivo)
            {
                txtFiltro.Enabled = false;
                txtFiltro.Text = "Filtro rápido desactivado";
                txtFiltro.ForeColor = Color.Gray;
                txtFiltro.BackColor = Color.LightGray;
            }
            else
            {
                txtFiltro.Enabled = true;
                txtFiltro.Text = "";
                txtFiltro.ForeColor = Color.Black;
                txtFiltro.BackColor = Color.White;
            }
        }

        private void cbxCriterio_SelectedIndexChanged(object sender, EventArgs e)
        {
            controlarFiltroRapido();
        }

        private void txtFiltroAvanzado_TextChanged(object sender, EventArgs e)
        {
            if (!txtFiltro.Enabled)
                return;

            // Si el usuario empieza a escribir, se limpian los campos del filtro avanzado
            if (!string.IsNullOrWhiteSpace(txtFiltro.Text))
            {
                cbxCampo.SelectedIndex = -1;
                cbxCriterio.SelectedIndex = -1;
                cbxCriterio.Items.Clear(); // vacía también los ítems del criterio
                txtFiltroAvanzado.Text = "";
            }

            // se aplica el filtro rápido
            List<Articulo> listaFiltrada;
            string filtro = txtFiltro.Text;

            if (filtro.Length >= 1)
            {
                listaFiltrada = listaArticulos.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Categoria.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaArticulos;
            }

            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listaFiltrada;
            ocultarColumnas();


        }
    }
}
//dato: edit programatically: permite no modificar texto en la grilla
//dato: selection mode: permite la cantidad de selección en la grilla