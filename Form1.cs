using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PruebagRID
{
    public partial class Form1 : Form
    {
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        public Form1()
        {
            InitializeComponent();
        }

        public void copiar_portapapeles(DataGridView datagrid)
        {
            DataObject objeto_datos = datagrid.GetClipboardContent();
            Clipboard.SetDataObject(objeto_datos);
        }
        public void pegar_portapapeles(DataGridView datagrid)
        {
            try
            {
                string texto_copiado = Clipboard.GetText();
                string[] lineas = texto_copiado.Split('\n');
                int error = 0;
                int fila = datagrid.CurrentCell.RowIndex;
                int columna = datagrid.CurrentCell.ColumnIndex;
                DataGridViewCell objeto_celda;

                foreach (string linea in lineas)
                {
                    if (fila < datagrid.RowCount && linea.Length > 0)
                    {
                        string[] celdas = linea.Split('\t');

                        for (int indice = 0; indice < celdas.GetLength(0); ++indice)
                        {
                            if (columna + indice < datagrid.ColumnCount)
                            {
                                objeto_celda = datagrid[columna + indice, fila];

                                //Mientras celda sea Diferente de ReadOnly
                                if (!objeto_celda.ReadOnly)
                                {
                                    if (objeto_celda.Value.ToString() != celdas[indice])
                                    {
                                        objeto_celda.Value = Convert.ChangeType(celdas[indice], objeto_celda.ValueType);
                                        //A continuación la linea añadida para eliminar los '\r'. De paso, y por si acaso en algún contexto ocurre, tambien los: '\t' y '\n'
                                        objeto_celda.Value = objeto_celda.Value.ToString().Trim(new Char[] { '\t', '\n', '\r' });
                                        // Fin linea añadida
                                    }
                                }
                                else
                                {
                                    // solo intercepta un error si los datos que está pegando es en una celda de solo lectura.
                                    error++;
                                }
                            }
                            else
                            { break; }
                        }
                        fila++;
                    }
                    else
                    { break; }

                    if (error > 0)
                        MessageBox.Show(string.Format("{0}  La celda no puede ser actualizada, debido a que la configuración de la columna es de solo lectura.", error),
                                                  "ADVERTENCIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (FormatException fexcepcion)
            {
                MessageBox.Show("Los datos que pegó están en el formato incorrecto para la celda." + "\n\nDETALLES: \n\n" + fexcepcion.Message,
                                "ADVERTENCIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }





        private void Agregar_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("Col_Num", "Num");
            dataGridView1.Columns.Add("Col_Nombre", "Nombre");
            dataGridView1.Columns["Col_Nombre"].Width = 150;
            dataGridView1.Columns.Add("Col_Fecha", "Fecha");
            dataGridView1.Columns.Add("Col_Turno", "Turno");
            dataGridView1.Columns.Add("Col_Valor", "Valor");
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void datagrid_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // Si el mouse se mueve fuera del rectángulo, comience a arrastrar.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Continúe arrastrando y soltando, pasando el elemento de la lista.                   
                    DragDropEffects dropEffect = dataGridView1.DoDragDrop(
                    dataGridView1.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }

        private void datagrid_MouseDown(object sender, MouseEventArgs e)
        {
            // Obtenga el índice del elemento que se encuentra debajo del mouse.
            rowIndexFromMouseDown = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)),
                                    dragSize);
            }
            else
                dragBoxFromMouseDown = Rectangle.Empty;
            // Reset the rectangle if the mouse is not over an item in the ListBox.
        }

        private void datagrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void datagrid_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dataGridView1.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop =
                dataGridView1.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                DataGridViewRow rowToMove = e.Data.GetData(
                    typeof(DataGridViewRow)) as DataGridViewRow;
                dataGridView1.Rows.RemoveAt(rowIndexFromMouseDown);
                dataGridView1.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);

            }
        }

        private void rojoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Obtener la numero de fila seleccionada por el usuario (Codigo Encabezado)
            if(dataGridView1.CurrentRow.Index != null)
            {
                int NumFila = -1;
                NumFila = dataGridView1.CurrentRow.Index;

                if (NumFila >= 0) {
                    dataGridView1.Rows[NumFila].DefaultCellStyle.BackColor = Color.FromArgb(255, 213, 79);
                }
            }
        }

        private void amarilloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Obtener la numero de fila seleccionada por el usuario (Codigo Encabezado)
            if (dataGridView1.CurrentRow.Index != null)
            {
                int NumFila = -1;
                NumFila = dataGridView1.CurrentRow.Index;

                if (NumFila >= 0)
                {
                    dataGridView1.Rows[NumFila].DefaultCellStyle.BackColor = Color.FromArgb(23, 89, 123);
                }
            }
        }

        private void azulToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Obtener la numero de fila seleccionada por el usuario (Codigo Encabezado)
            if (dataGridView1.CurrentRow.Index != null)
            {
                int NumFila = -1;
                NumFila = dataGridView1.CurrentRow.Index;

                if (NumFila >= 0)
                {
                    dataGridView1.Rows[NumFila].DefaultCellStyle.BackColor = Color.FromArgb(34, 100, 155);
                }
            }
        }

        private void ningunoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Obtener la numero de fila seleccionada por el usuario (Codigo Encabezado)
            if (dataGridView1.CurrentRow.Index != null)
            {
                int NumFila = -1;
                NumFila = dataGridView1.CurrentRow.Index;

                if (NumFila >= 0)
                {
                    dataGridView1.Rows[NumFila].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.AllowDrop = true;

            if (dataGridView1.Rows.Count > 0) {
                
                for (int i = 0; i < dataGridView1.Rows.Count; i++) {
                    if (dataGridView1.Rows[i].DefaultCellStyle.BackColor.ToArgb() != 0 )
                        dataGridView1.Rows.RemoveAt(i);
                    
                    //MessageBox.Show(dataGridView1.Rows[i].DefaultCellStyle.BackColor.ToArgb().ToString());   
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            dataGridView1.AllowDrop = false;

            if (dataGridView1.Rows.Count > 0)
            {

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells["Col_Num"].Value.ToString() == "")
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(23, 89, 123);

                }
            }
        }
    }
}
