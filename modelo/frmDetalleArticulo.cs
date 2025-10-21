using dominio;
using helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace modelo
{
    public partial class frmDetalleArticulo : Form
    {
        private Articulo articulo;
        public frmDetalleArticulo(Articulo seleccionado)
        {
            InitializeComponent();
            this.articulo = seleccionado;
            this.Text = "Detalle de Artículo";
        }

        private void frmDetalleArticulo_Load(object sender, EventArgs e)
        {
            if (articulo != null)
            {
                lblCodigo.Text = "Código: " + articulo.codArticulo;
                lblNombre.Text = "Nombre: " + articulo.Nombre;
                lblDescripcion.Text = "Descripción: " + articulo.Descripcion;
                lblMarca.Text = "Marca: " + articulo.Marca.Descripcion;
                lblCategoria.Text = "Categoría: " + articulo.Categoria.Descripcion;
                lblPrecio.Text = "Precio: $" + articulo.Precio.ToString("#,0.00");

                try
                {
                    if (!string.IsNullOrEmpty(articulo.urlImagen))
                        pbxArticulo.Load(articulo.urlImagen);
                }
                catch
                {
                    pbxArticulo.Load(help_negocio.cargarImagenGenerica());
                }
            }
        }
    }
}
