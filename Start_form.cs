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
    class Start_form: System.Windows.Forms.Form
    {
        Button Filmid_btn, Piletid_btn, Admin_btn;

        public Start_form()
        {
            this.Text = "Teretulemast kinno!";
            this.Size = new Size(420, 330);
            this.BackgroundImage = Image.FromFile(@"..\..\Pildid\Fon.jpg");
            
            Filmid_btn = new Button
            {
                Text="Kava",
                Location=new Point(50,25),
                Size=new Size(100,50)
            };
            this.Controls.Add(Filmid_btn);
            Filmid_btn.Click += Filmid_btn_Click;
            Piletid_btn = new Button
            {
                Text = "Osta pilet",
                Location = new Point(50, 125),
                Size = new Size(100, 50)
            };
            this.Controls.Add(Piletid_btn);
            Piletid_btn.Click += Piletid_btn_Click;
            Admin_btn = new Button
            {
                Text = "Admin",
                Location = new Point(50, 225),
                Size = new Size(100, 50)
            };
            this.Controls.Add(Admin_btn);
            Admin_btn.Click += Admin_btn_Click;

        }

        private void Admin_btn_Click(object sender, EventArgs e)
        {
            Admin_Form admin_form = new Admin_Form();
            admin_form.Show();
        }

        string filminimetus;
        private void Piletid_btn_Click(object sender, EventArgs e)
        {
            MyForm saal = new MyForm(5, 5, Filmid[index]);
            saal.Show();
        }

        static string conn_KinoDB = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\marina.oleinik\source\repos\MinuVorm\AppData\Kino_DB.mdf;Integrated Security=True";
        SqlConnection connect_to_DB = new SqlConnection(conn_KinoDB);
        SqlCommand command;
        SqlDataAdapter adapter;
        DataGridView dataGridView;
        DataTable tabel;
        string[] Filmid;
        int Id;
        int index;
        PictureBox kava;
        List<PictureBox> kavaBoxList;
       
        private void Filmid_btn_Click(object sender, EventArgs e)
        {
            connect_to_DB.Open();
            tabel = new DataTable();
            dataGridView = new DataGridView();
            adapter = new SqlDataAdapter("SELECT * FROM [dbo].[Filmid]", connect_to_DB);
            adapter.Fill(tabel);
            dataGridView.DataSource = tabel;

            Filmid = new string[tabel.Rows.Count];
            var index = 0;
            foreach (DataRow row in tabel.Rows)
            {
                var film = row["Poster"];
                Filmid[index++] = $"{film}";
            }
            connect_to_DB.Close();
            
            kavaBoxList = new List<PictureBox>();
            foreach (var f in Filmid)
            {
                PictureBox pic = new PictureBox
                {
                    Image = Image.FromFile(@"..\..\Posterid\" + f)
                };
                kavaBoxList.Add(pic);
                
            };
            index = 0;
            kava = new PictureBox
            {
                Image = kavaBoxList[index].Image,
                Location = new System.Drawing.Point(200, 25),
                Size=new Size(150,260)

            };
            this.Controls.Add(kava);
            kava.MouseDown += Kava_MouseDown;
            MessageBox.Show("Vajuta hiire paremat või vasakut nuppu","Paremale või vasakule liikumiseks");
        }

       

        private void Kava_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    if (index >= Filmid.Count()-1)
                    {
                        index = Filmid.Count()-1;
                    }
                    else
                    {index++;}
                    break;
                case MouseButtons.Left:
                    if (index <= 0)
                    {
                        index = 0;
                    }
                    else
                    {index--;}
                    break;
            }
            kava.Image = kavaBoxList[index].Image;
            //filminimetus = Film(kavaBoxList[index].ToString());
        }
 
        private string Film(string filminimetus)
        {

            return filminimetus;
        }
     
       
    }
}
