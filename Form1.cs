using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Andmebass_TARpv23
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\opilane\Source\Repos\Andmebass_TARpv23\Andmebaas1.mdf;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter adapter;
        OpenFileDialog open;
        SaveFileDialog save;
        Form popupForm;
        DataTable laotable;
        string extension;
        private byte[] imageData;
        public Form1()
        {
            InitializeComponent();
            NaitaAndmed();
            NaitaLaod();
        }

        private void NaitaLaod()
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("SELECT Id, LaoNimetus FROM Ladu", conn);
                adapter = new SqlDataAdapter(cmd);
                laotable = new DataTable();
                adapter.Fill(laotable);

                // Привязываем данные к Ladu_cb
                Ladu_cb.DataSource = laotable;
                Ladu_cb.DisplayMember = "LaoNimetus";  // Название склада
                Ladu_cb.ValueMember = "Id";           // Идентификатор склада
                Ladu_cb.SelectedIndex = -1;           // Сбрасываем выбор
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga laod andmete laadimisel: {ex.Message}", "Viga");
            }
            finally
            {
                conn.Close();
            }
        }

        public void NaitaAndmed()
        {
            conn.Open();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Toode", conn);
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }

        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            if (Nimetus_txt.Text.Trim() != string.Empty && Kogus_txt.Text.Trim() != string.Empty && Hind_txt.Text.Trim() != string.Empty)
            {
                if (Ladu_cb.SelectedValue == null)
                {
                    MessageBox.Show("Valige ladu enne andmete lisamist.", "Viga");
                    return;
                }

                try
                {
                    conn.Open();

                    cmd = new SqlCommand("Insert into Toode(Nimetus, Kogus, Hind, Pilt, LaoID) Values (@toode,@kogus,@hind,@pilt, @laoid)", conn);
                    cmd.Parameters.AddWithValue("@toode", Nimetus_txt.Text);
                    cmd.Parameters.AddWithValue("@kogus", Kogus_txt.Text);
                    cmd.Parameters.AddWithValue("@hind", Hind_txt.Text);
                    cmd.Parameters.AddWithValue("@pilt", Nimetus_txt.Text + extension);
                    cmd.Parameters.AddWithValue("@laoid", Ladu_cb.SelectedValue); // Используем SelectedValue

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    NaitaAndmed();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Andmebaasiga viga: {ex.Message}", "Viga");
                }
            }
            else
            {
                MessageBox.Show("Sisesta andmeid");
            }
        }


        private void Kustuta_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, что есть выбранная строка
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    ID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                    string fileName = dataGridView1.SelectedRows[0].Cells["Pilt"].Value.ToString();

                    if (ID != 0)
                    {
                        conn.Open();
                        cmd = new SqlCommand("DELETE FROM Toode WHERE Id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", ID);
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        // Удаляем файл
                        Kustuta_fail(fileName);

                        Emaldamine();
                        NaitaAndmed();

                        MessageBox.Show("Tood on kustutanud", "Kustutamine");
                    }
                }
                else
                {
                    MessageBox.Show("Valige toode kustutamiseks.", "Viga");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kustutamiseks viga: {ex.Message}", "Viga");
            }
        }

        private void Kustuta_fail(string fileName)
        {
            try
            {
                // Полный путь к файлу
                string filePath = Path.Combine(Path.GetFullPath(@"..\..\Pildid"), fileName);

                // Проверяем, существует ли файл
                if (File.Exists(filePath))
                {
                    // Освобождаем картинку в PictureBox
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = null;

                    // Удаляем файл
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kustutamiseks viga: {ex.Message}", "Viga");
            }
        }


        private void Uuenda_btn_Click(object sender, EventArgs e)
        {
            if (Nimetus_txt.Text.Trim() != string.Empty && Kogus_txt.Text.Trim() != string.Empty && Hind_txt.Text.Trim() != string.Empty)
            {
                if (Ladu_cb.SelectedValue == null)
                {
                    MessageBox.Show("Valige ladu enne andmete uuendamist.", "Viga");
                    return;
                }

                try
                {
                    conn.Open();
                    cmd = new SqlCommand("Update Toode SET Nimetus=@toode, Kogus=@kogus, Hind=@hind, Pilt=@pilt, LaoID=@laoid WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.Parameters.AddWithValue("@toode", Nimetus_txt.Text);
                    cmd.Parameters.AddWithValue("@kogus", Kogus_txt.Text);
                    cmd.Parameters.AddWithValue("@hind", Hind_txt.Text);
                    cmd.Parameters.AddWithValue("@pilt", Nimetus_txt.Text + extension);
                    cmd.Parameters.AddWithValue("@laoid", Ladu_cb.SelectedValue);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    NaitaAndmed();
                    Emaldamine();
                    MessageBox.Show("Andmed edukalt uuendatud", "Uuendamine");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Andmebaasiga viga: {ex.Message}", "Viga");
                }
            }
            else
            {
                MessageBox.Show("Sisesta andmeid");
            }
        }



        private void Emaldamine()
        {
            Nimetus_txt.Text = "";
            Kogus_txt.Text = "";
            Hind_txt.Text = "";
            pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Pildid"), "pilt.jpg"));
            Ladu_cb.SelectedIndex = -1; // Сбрасываем выбор склада
        }
        int ID = 0;
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ID = (int)dataGridView1.Rows[e.RowIndex].Cells["Id"].Value; 
            Nimetus_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Nimetus"].Value.ToString();
            Kogus_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Kogus"].Value.ToString();
            Hind_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Hind"].Value.ToString();
            try
            {
                pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Pildid"),
                    dataGridView1.Rows[e.RowIndex].Cells["Pilt"].Value.ToString()));
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception)
            {
                pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Pildid"), "pilt.jpg"));
            }
        }

        private void Pildi_otsing_btn_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog();
            open.InitialDirectory = @"C:\Users\opilane\Pictures\";
            open.Multiselect = false;
            open.Filter = "Image Files(*.jpeg;*.png;*.bmp;*.jpg)|*.jpeg;*.png;*.bmp;*.jpg";

            if (open.ShowDialog() == DialogResult.OK)
            {
                // Получаем расширение файла
                extension = Path.GetExtension(open.FileName);

                // Сохраняем изображение в папке "Pildid"
                string targetPath = Path.Combine(Path.GetFullPath(@"..\..\Pildid"), Nimetus_txt.Text + extension);
                File.Copy(open.FileName, targetPath, true); // Перезапись разрешена

                // Отображаем выбранное изображение в PictureBox
                pictureBox1.Image = Image.FromFile(targetPath);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                // Сохраняем путь для последующего добавления в базу данных
                Hind_txt.Tag = Nimetus_txt.Text + extension; // Временно храним путь в "Tag"
            }
            else
            {
                MessageBox.Show("Kustutamine.", "Kustutamine");
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 4)
            {
                imageData = dataGridView1.Rows[e.RowIndex].Cells["FusPilt"].Value as byte[];
                if (imageData != null)
                {
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        Image image = Image.FromStream(ms);
                        LoopIt(image, e.RowIndex);
                    }
                }
            }
        }
        private void LoopIt(Image image, int r)
        {
            popupForm = new Form();
            popupForm.FormBorderStyle = FormBorderStyle.None;
            popupForm.StartPosition = FormStartPosition.Manual;
            popupForm.Size = image.Size;


            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = image;
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            popupForm.Controls.Add(pictureBox);

            Rectangle cellRectangle = dataGridView1.GetCellDisplayRectangle(4, r, true);
            Point popupLocation = dataGridView1.PointToScreen(cellRectangle.Location);

            popupForm.Location = new Point(popupLocation.X + cellRectangle.Width, popupLocation.Y);

            popupForm.Show();
        }
            
        private void dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (popupForm != null && !popupForm.IsDisposed)
            {
                popupForm.Close();
            }
        }
    }
} 