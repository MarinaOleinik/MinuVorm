using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinuVorm
{
    class Admin_Form: System.Windows.Forms.Form
    {
        static string conn_KinoDB = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\marina.oleinik\source\repos\MinuVorm\AppData\Kino_DB.mdf;Integrated Security=True";
        SqlConnection connect_to_DB = new SqlConnection(conn_KinoDB);
        SqlCommand command;
        SqlDataAdapter adapter;
        Button film_uuenda, film_kustuta, film_naita, film_lisa,fail_lisa;
        public Admin_Form()
        {

            this.Size = new System.Drawing.Size(800, 450);
            
            film_naita = new Button
            {
                Location = new System.Drawing.Point(50, 25),
                Size = new System.Drawing.Size(80, 25),
                Text = "Näita filmid"
            };
            this.Controls.Add(film_naita);
            film_naita.Click += Film_naita_Click;
            film_uuenda = new Button
            {
                Location = new System.Drawing.Point(650, 75),
                Size = new System.Drawing.Size(80, 25),
                Text = "Uuendamine",
            };
            this.Controls.Add(film_uuenda);
            film_uuenda.Click += Film_uuenda_Click;
            film_kustuta = new Button
            {
                Location = new System.Drawing.Point(650, 100),
                Size = new System.Drawing.Size(80, 25),
                Text = "Kustutamine",
            };
            this.Controls.Add(film_kustuta);
            
            fail_lisa = new Button
            {
                Location = new System.Drawing.Point(650, 125),
                Size = new System.Drawing.Size(80, 25),
                Text = "Faili valimine",
            };
            this.Controls.Add(fail_lisa);
            fail_lisa.Click += Fail_lisa_Click;
            film_lisa = new Button
            {
                Location = new System.Drawing.Point(650, 150),
                Size = new System.Drawing.Size(80, 25),
                Text = "Lisamine",
            };
            this.Controls.Add(film_lisa);
            film_lisa.Click += Film_lisa_Click;
        }
        SaveFileDialog save;
        private void Fail_lisa_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";
            open.InitialDirectory = Path.GetFullPath(@"C:\Users\marina.oleinik\Pictures\Filmid");//kust
            if (open.ShowDialog() == DialogResult.OK)
            {
                save = new SaveFileDialog();
                //save.FileName = poster_txt.Text;
                save.Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";
                save.InitialDirectory = Path.GetFullPath(@"..\..\Posterid");//kuhu
                
                if (save.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(open.FileName, save.FileName);
                    save.RestoreDirectory = true;
                    poster.Image = Image.FromFile(save.FileName);
                    film_txt.Text= Path.GetFileNameWithoutExtension(save.FileName);
                    poster_txt.Text = Path.GetFileNameWithoutExtension(save.FileName)+".JPG";
                }

            }
        }

        

        
        
        private void Film_lisa_Click(object sender, EventArgs e)
        {
            if (film_txt.Text != "" && aasta_txt.Text != "" && poster_txt.Text != "")
            {
                try
                {
                    connect_to_DB.Open();
                    command = new SqlCommand("INSERT INTO Filmid(Filmi_nimetus,Aasta,Poster) VALUES(@film,@aasta,@poster)", connect_to_DB);
                    
                    command.Parameters.AddWithValue("@film", film_txt.Text);
                    command.Parameters.AddWithValue("@aasta", aasta_txt.Text);
                    command.Parameters.AddWithValue("@poster", poster_txt.Text);

                    //string file_pilt = poster_txt.Text + ".jpg";
                    //command.Parameters.AddWithValue("@pilt", file_pilt);
                   
                    command.ExecuteNonQuery();
                    connect_to_DB.Close();
                    Data();
                    ClearData();
                    MessageBox.Show("Andmed on lisatud");
                }
                catch (Exception)
                {

                    MessageBox.Show("Viga lisamisega");
                }
            }
            else
            {
                MessageBox.Show("Viga else");
            }
        }

        int Id;
        
        private void Film_uuenda_Click(object sender, EventArgs e)
        {
            
            if (film_txt.Text != "" && aasta_txt.Text != "" && poster_txt.Text != "" && poster.Image != null)
            {       
                connect_to_DB.Open();
                command = new SqlCommand("UPDATE Filmid  SET Filmi_nimetus=@film,Aasta=@aasta,Poster=@poster WHERE Id=@id", connect_to_DB);
                
                command.Parameters.AddWithValue("@id", Id);
                command.Parameters.AddWithValue("@film", film_txt.Text);
                command.Parameters.AddWithValue("@aasta", aasta_txt.Text);
                command.Parameters.AddWithValue("@poster", poster_txt.Text);
                //string file_pilt = poster_txt.Text + ".jpg";
                //command.Parameters.AddWithValue("@poster", file_pilt);
                command.ExecuteNonQuery();
                dataGridView.Refresh();
                connect_to_DB.Close();
                ClearData();
                Data();
                MessageBox.Show("Andmed uuendatud");
                
            }
            else
            {
                MessageBox.Show("Viga");
            }
            
        }

        private void Pilet_naita_Click(object sender, EventArgs e)
        {
            connect_to_DB.Open();
            DataTable tabel_p = new DataTable();
            DataGridView dataGridView_p = new DataGridView();
            DataSet dataset_p = new DataSet();
            SqlDataAdapter adapter_p = new SqlDataAdapter("SELECT Rida,Koht,Film_Id FROM [dbo].[Piletid]; SELECT Filmi_nimetus FROM [dbo].[Filmid]", connect_to_DB);
            
            //adapter_p.TableMappings.Add("Piletid", "Rida");
            //adapter_p.TableMappings.Add("Filmid", "Filmi_nimetus");
            //adapter_p.Fill(dataset_p);
            adapter_p.Fill(tabel_p);
            dataGridView_p.DataSource = tabel_p;
            dataGridView_p.Location = new System.Drawing.Point(10, 75);
            dataGridView_p.Size = new System.Drawing.Size(400, 200);
            

            SqlDataAdapter adapter_f = new SqlDataAdapter("SELECT Filmi_nimetus FROM [dbo].[Filmid]", connect_to_DB);
            DataTable tabel_f = new DataTable();
            DataSet dataset_f = new DataSet();
            adapter_f.Fill(tabel_f);
            /*fkc = new ForeignKeyConstraint(tabel_f.Columns["Id"], tabel_p.Columns["Film_Id"]);
            tabel_p.Constraints.Add(fkc);*/
            poster.Image = Image.FromFile("../../Posterid/Start.jpg");

            DataGridViewComboBoxCell cbc = new DataGridViewComboBoxCell();
            ComboBox com_f = new ComboBox();
            foreach (DataRow row in tabel_f.Rows)
            {
                com_f.Items.Add(row["Filmi_nimetus"]);
                cbc.Items.Add(row["Filmi_nimetus"]);
            }
            cbc.Value = com_f;
            connect_to_DB.Close();
            this.Controls.Add(dataGridView_p);
            this.Controls.Add(com_f);
        }


        TextBox film_txt, aasta_txt, poster_txt;
        PictureBox poster;
        DataGridView dataGridView;
        DataTable tabel;
        private void Film_naita_Click(object sender, EventArgs e)
        {
            film_naita.Text = "Peida filmid";
            film_uuenda.Visible = true;
            film_kustuta.Visible = true;

            film_txt = new TextBox
            {Location = new System.Drawing.Point(450, 75)};
            aasta_txt = new TextBox
            {Location = new System.Drawing.Point(450, 100)};
            poster_txt = new TextBox
            {Location = new System.Drawing.Point(450, 125)};
            poster = new PictureBox
            {
                Size = new System.Drawing.Size(180, 250),
                Location=new System.Drawing.Point(450,150),
                Image = Image.FromFile("../../Posterid/Start.jpg")
        };

            Data();
        }
        public void Data()
        {
            connect_to_DB.Open();
            tabel = new DataTable();
            dataGridView = new DataGridView();
            dataGridView.RowHeaderMouseClick += DataGridView_RowHeaderMouseClick;
            adapter = new SqlDataAdapter("SELECT * FROM [dbo].[Filmid]", connect_to_DB);
            adapter.Fill(tabel);
            dataGridView.DataSource = tabel;
            dataGridView.Location = new System.Drawing.Point(10, 75);
            dataGridView.Size = new System.Drawing.Size(400, 200);
            connect_to_DB.Close();
            
            this.Controls.Add(dataGridView);
            this.Controls.Add(film_txt);
            this.Controls.Add(aasta_txt);
            this.Controls.Add(poster_txt);
            this.Controls.Add(poster);
        }
        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            Id = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString());
            film_txt.Text = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            aasta_txt.Text = dataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            poster_txt.Text = dataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();
            poster.Image = Image.FromFile(@"..\..\Posterid\" + dataGridView.Rows[e.RowIndex].Cells[3].Value.ToString());
            this.Text = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            

        }
        private void ClearData()
        {
            //Id = 0;
            film_txt.Text = "";
            aasta_txt.Text = "";
            poster_txt.Text = "";
            //save.FileName = "";
            poster.Image = Image.FromFile("../../Posterid/Start.jpg");

        }
    }
}
