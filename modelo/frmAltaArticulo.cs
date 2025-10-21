using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using helper;
using negocio;
using System.Configuration; //a partir de acá se puede utilizar configuration manager y todas sus propiedades

namespace modelo
{
    public partial class frmAltaArticulo : Form
    {
        public int? IdArticuloSeleccionado { get; private set; } //esta propiedad se usa para identificar que articulo del DGV se elegió, y poder tomarlo como referencia en el DGV principal y permanezca seleccionado
        private Articulo articulo = null;
        private OpenFileDialog archivo = null;
        private string nombreImagenDestino = null;
        public frmAltaArticulo()
        {
            InitializeComponent();
        }
        public frmAltaArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificación de Articulo";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

            if (!validarCampos())
                return;

            //dato: shift + ALT + LMB: para seleccionar varias lineas tipo "insert" para modificar su texto
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (articulo == null)
                    articulo = new Articulo();

                articulo.codArticulo = txtCodArt.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.urlImagen = txtUrlImagen.Text;
                articulo.Marca = (Marca)cboMarca.SelectedItem;
                articulo.Categoria = (Categoria)cboCategoria.SelectedItem;
                articulo.Precio = decimal.Parse(txtPrecio.Text);

                // Validamos si el usuario editó manualmente la URL
                // Si el usuario editó manualmente la URL,el archivo debe anularse
                if (archivo != null && txtUrlImagen.Text.ToUpper().Contains("HTTP"))
                    archivo = null;

                if (archivo != null)
                {
                    articulo.urlImagen = help_negocio.guardarImagenLocal(archivo.FileName);
                }
                else
                {
                    articulo.urlImagen = txtUrlImagen.Text;
                }

                if (articulo.Id != 0)
                {
                    negocio.modificar(articulo);
                    MessageBox.Show("Modificado exitosamente.");
                }
                else
                {
                    negocio.agregar(articulo);
                    MessageBox.Show("Agregado exitosamente.");
                }

                //Esto se usa para que el DGV pueda tomar como referencia el articulo seleccionado en el frmPrincipal, para saber cual se elegió
                this.IdArticuloSeleccionado = articulo.Id;
                this.DialogResult = DialogResult.OK;


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }


        //Dato en el front (propiedades): dropdownlist te permite que el usuario no ingrese valores al campo de texto, solo elija los predefinidos
        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {
            MarcaNegocio marcaNegocio = new MarcaNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
            try
            {
                cboMarca.DataSource = marcaNegocio.despliegue();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";
                cboCategoria.DataSource = categoriaNegocio.despliegue();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";

                if (articulo != null)
                {
                    txtCodArt.Text = articulo.codArticulo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtUrlImagen.Text = articulo.urlImagen;
                    help_negocio.cargarImagen(txtUrlImagen.Text, this.pbxArticulo);
                    cboMarca.SelectedValue = articulo.Marca.Id;
                    cboCategoria.SelectedValue = articulo.Categoria.Id;
                    txtPrecio.Text = articulo.Precio.ToString();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            try
            {
                helper.help_negocio.cargarImagen(txtUrlImagen.Text, pbxArticulo);
            }
            catch (Exception)
            {
                helper.help_negocio.cargarImagenGenerica();
            }
        }

        //método para poder levantar una imagen desde la PC
        private void btnAggImagen_Click(object sender, EventArgs e)
        {
            //genera una ventana de diálogo
            archivo = new OpenFileDialog();
            archivo.Filter = "Imágenes (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlImagen.Text = archivo.FileName; //guarda ruta temporal
                help_negocio.cargarImagen(archivo.FileName, pbxArticulo);
            }
        }

        private bool validarCampos()
        {
            bool esValido = true;

            // COD ARTICULO
            if (string.IsNullOrWhiteSpace(txtCodArt.Text) || txtCodArt.Text == "Campo obligatorio")
            {
                txtCodArt.Text = "Campo obligatorio";
                txtCodArt.ForeColor = Color.Gray;
                txtCodArt.BackColor = Color.MistyRose;
                esValido = false;
            }

            // NOMBRE
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || txtNombre.Text == "Campo obligatorio")
            {
                txtNombre.Text = "Campo obligatorio";
                txtNombre.ForeColor = Color.Gray;
                txtNombre.BackColor = Color.MistyRose;
                esValido = false;
            }

            // DESCRIPCION
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text) || txtDescripcion.Text == "Campo obligatorio")
            {
                txtDescripcion.Text = "Campo obligatorio";
                txtDescripcion.ForeColor = Color.Gray;
                txtDescripcion.BackColor = Color.MistyRose;
                esValido = false;
            }

            // PRECIO
            if (txtPrecio.Text == "Solo números")
            {
                // Ya se validó y falló: no es necesario pisar el texto
                txtPrecio.BackColor = Color.MistyRose;
                esValido = false;
            }
            else if (string.IsNullOrWhiteSpace(txtPrecio.Text) || txtPrecio.Text == "Campo obligatorio")
            {
                txtPrecio.Text = "Campo obligatorio";
                txtPrecio.ForeColor = Color.Gray;
                txtPrecio.BackColor = Color.MistyRose;
                esValido = false;
            }
            else if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio < 1)
            {
                txtPrecio.Text = "Solo números mayores a cero.";
                txtPrecio.ForeColor = Color.Gray;
                txtPrecio.BackColor = Color.MistyRose;
                esValido = false;
            }
            else
            {
                txtPrecio.ForeColor = Color.Black;
                txtPrecio.BackColor = SystemColors.Window;
            }

            // MARCA
            if (cboMarca.SelectedItem == null)
            {
                cboMarca.BackColor = Color.MistyRose;
                esValido = false;
            }
            else
            {
                cboMarca.BackColor = SystemColors.Window;
            }

            // CATEGORIA
            if (cboCategoria.SelectedItem == null)
            {
                cboCategoria.BackColor = Color.MistyRose;
                esValido = false;
            }
            else
            {
                cboCategoria.BackColor = SystemColors.Window;
            }

            return esValido;
        }

        private void txtCodArt_Enter(object sender, EventArgs e)
        {
            if (txtCodArt.Text == "Campo obligatorio")
            {
                txtCodArt.Text = "";
                txtCodArt.ForeColor = Color.Black;
            }
        }

        private void txtCodArt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCodArt.Text) && txtCodArt.Text != "Campo obligatorio")
                txtCodArt.BackColor = SystemColors.Window;
        }

        private void txtNombre_Enter(object sender, EventArgs e)
        {
            if (txtNombre.Text == "Campo obligatorio")
            {
                txtNombre.Text = "";
                txtNombre.ForeColor = Color.Black;
            }
        }

        private void txtNombre_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCodArt.Text) && txtNombre.Text != "Campo obligatorio")
                txtNombre.BackColor = SystemColors.Window;
        }

        private void txtDescripcion_Enter(object sender, EventArgs e)
        {
            if (txtDescripcion.Text == "Campo obligatorio")
            {
                txtDescripcion.Text = "";
                txtDescripcion.ForeColor = Color.Black;
            }
        }

        private void txtDescripcion_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCodArt.Text) && txtDescripcion.Text != "Campo obligatorio")
                txtDescripcion.BackColor = SystemColors.Window;
        }

        private void txtPrecio_Enter(object sender, EventArgs e)
        {
            if (txtPrecio.Text == "Campo obligatorio" || txtPrecio.Text == "Solo números")
            {
                txtPrecio.Text = "";
                txtPrecio.ForeColor = Color.Black;
            }
        }

        private void txtPrecio_Leave(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtPrecio.Text, out decimal _))
            {
                txtPrecio.Text = "Solo números";
                txtPrecio.ForeColor = Color.Gray;
                txtPrecio.BackColor = Color.MistyRose;
            }
            else
            {
                txtPrecio.ForeColor = Color.Black;
                txtPrecio.BackColor = SystemColors.Window;
            }
        }

        private void cboMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMarca.SelectedIndex > 0)
                cboMarca.BackColor = SystemColors.Window;
        }

        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCategoria.SelectedIndex > 0)
                cboCategoria.BackColor = SystemColors.Window;
        }
    }
}
