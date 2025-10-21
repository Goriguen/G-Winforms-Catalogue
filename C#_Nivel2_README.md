

#### **TP Final - Catalogo de Articulos - C# Nivel 2 (MaxiPrograma)**



###### Descripcion general



Esta es una aplicacion de escritorio (WinForms) desarrollada en C# con arquitectura en capas, orientada a la gestion de articulos de un catalogo general, adaptable a cualquier tipo de comercio.



-----------------------------------------------------------------------------------------------------------------------------------------------------------------------





Tecnologias utilizadas:

---

\- C# (.NET Framework)

\- WinForms

\- SQL Server

\- Arquitectura en capas: Dominio  Negocio  Presentacion  Helper

\- Uso de Configuration Manager para rutas configurables





-----------------------------------------------------------------------------------------------------------------------------------------------------------------------





Funcionalidades principales



| Funcionalidad          | Descripcion                                                                 |

|------------------------|-----------------------------------------------------------------------------|

| Listado de articulos   | Se muestran en un DataGridView con imagen incluida (URL o local).           |

| Alta de articulos      | Valida todos los campos antes de guardar. Imagen local o remota.            |

| Modificacion           | Reutiliza el mismo formulario de alta. Carga campos previamente.            |

| Eliminacion logica     | Cambia el nombre agregando \[ELIMINADO]. Puede restaurarse.                  |

| Eliminacion fisica     | Borra completamente el articulo de la base.                                 |

| Detalle del articulo   | Se accede con boton. Muestra toda la informacion completa.                  |

| Busqueda rapida        | Busca por nombre y categoria al instante.                                   |

| Filtro avanzado        | Filtra por campo, criterio y valor. Aplica tambien para precios.            |



-----------------------------------------------------------------------------------------------------------------------------------------------------------------------



Validaciones implementadas



\- Los campos codigo, nombre, descripcion, precio, marca y categoria son obligatorios.

\- El campo precio solo permite numeros mayores a cero.

\- Se muestran mensajes contextuales en los campos con errores (ej: "Campo obligatorio").

\- Filtros rapidos y avanzados no se pueden usar al mismo tiempo.

\- Si se escribe en uno, el otro se desactiva automaticamente.

\- Se usan colores (MistyRose) para marcar errores de entrada.



-----------------------------------------------------------------------------------------------------------------------------------------------------------------------



Interfaz visual



\- Articulos dados de baja logica se muestran en rojo en la grilla.

\- Los formularios tienen titulos contextuales claros.

\- No es posible maximizar las ventanas.



-----------------------------------------------------------------------------------------------------------------------------------------------------------------------



Como testear



1\. Agregar articulo: Clic en "Agregar articulo". Completa todos los campos. Podes probar tanto URL como imagen local.

2\. Modificar articulo: Selecciona un articulo y presiona "Modificar". Cambia algun campo y acepta.

3\. Eliminar logicamente: Clic en "Dar de baja". Se marca como "\[ELIMINADO]".

4\. Restaurar: Tilda el checkbox "Ver eliminados" y presiona "Restaurar".

5\. Eliminar fisicamente: Usa "Eliminar fisico" y confirma.

6\. Detalle: Clic en "Ver detalle" para ver la info completa.

7\. Filtro rapido: Escribi una palabra clave en el campo de busqueda rapida.

8\. Filtro avanzado: Selecciona un campo (ej. Precio), criterio (ej. Mayor a) y completa el valor. Luego clic en "Buscar".



-----------------------------------------------------------------------------------------------------------------------------------------------------------------------



Observaciones



\- La imagen generica se carga automaticamente si la URL es invalida o el archivo no se encuentra.

\- El sistema pregunta si se desea sobrescribir al guardar una imagen local repetida.

\- El helper centraliza funcionalidades reutilizables para mantener la limpieza del codigo.



