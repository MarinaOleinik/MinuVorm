using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MinuVorm
{
    public partial class MyForm: Form
    {       
        Label message = new Label();
        Button[] btn = new Button[4];
        string[] texts = new string[4];
        TableLayoutPanel tlp = new TableLayoutPanel();
        Button btn_tabel;
        static List<Pilet> piletid;
        int k, r;
        string film;
        static string[] read_kohad;
        public MyForm()
        {}
        public MyForm(string title,string body,string button1,string button2,string button3,string button4)
        {
            texts[0] = button1;
            texts[1] = button2;
            texts[2] = button3;
            texts[3] = button4;
            this.ClientSize = new System.Drawing.Size(400, 100);
            this.Text = title;
            int x = 10;
            for (int i = 0; i < 4; i++)
            {
                btn[i] = new Button
                {
                    Location = new System.Drawing.Point(x, 50),
                    Size=new System.Drawing.Size(80,25),
                    Text=texts[i]
                };
                btn[i].Click += MyForm_Click;
                x += 100;
                this.Controls.Add(btn[i]);
            }
            message.Location = new System.Drawing.Point(10, 10);
            message.Text = body;
            this.Controls.Add(message);
        }
        public MyForm(string title, string body, string button1, string button2)
        {
            texts[0] = button1;
            texts[1] = button2;

            this.ClientSize = new System.Drawing.Size(400, 100);
            this.Text = title;
            int x = 10;
            for (int i = 0; i < 3; i++)
            {
                btn[i] = new Button
                {
                    Location = new System.Drawing.Point(x, 50),
                    Size = new System.Drawing.Size(80, 25),
                    Text = texts[i]
                };
                btn[i].Click += MyForm_Click;
                x += 100;
                this.Controls.Add(btn[i]);
            }
            message.Location = new System.Drawing.Point(10, 10);
            message.Text = "Kas tahad saada e-mailile?";
            this.Controls.Add(message);
        }
        public string[] Ostetud_piletid()
        {
            try
            {
                StreamReader f = new StreamReader(@"..\..\Piletid\piletid.txt");
                read_kohad = f.ReadToEnd().Split(';');
                f.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return read_kohad;
        }
        public Button Uusnupp(Action<object, EventArgs> click)
        {
            btn_tabel = new Button
            {
                Text = string.Format("rida {0},koht {1}", r + 1, k + 1),
                Name = string.Format("{1}{0}", k+1, r+1),
                Dock = DockStyle.Fill,
                BackColor = Color.Green
            };           
            btn_tabel.Click += new EventHandler(Pileti_valik);
            return btn_tabel;
        }
        public MyForm(int read, int kohad,string film)
        {
            this.tlp.ColumnCount = kohad;
            this.tlp.RowCount = read;
            this.tlp.ColumnStyles.Clear();
            this.tlp.RowStyles.Clear();
            int i, j;
            read_kohad = Ostetud_piletid();
            piletid = new List<Pilet> { };
            this.Text = film;
            for (i = 0; i < read; i++)
            {
                this.tlp.RowStyles.Add(new RowStyle(SizeType.Percent));
                this.tlp.RowStyles[i].Height = 100 / read;
            }            
            for (j = 0; j < kohad; j++)
            {
                this.tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
                this.tlp.ColumnStyles[j].Width = 100 / kohad;
            }            
            this.Size = new System.Drawing.Size(read*100, kohad*100);
            for (r = 0; r < read; r++)
            {
                for (k = 0; k < kohad; k++)
                {
                    Button btn_tabel = Uusnupp((sender,e)=> Pileti_valik(sender,e));
                    foreach (var item in read_kohad)
                    {
                        if (item.ToString() == btn_tabel.Name)
                        {
                            btn_tabel.BackColor = Color.Red;
                            btn_tabel.Enabled = false;
                        }
                    }
                    this.tlp.Controls.Add(btn_tabel, k, r);
                }
            }
            this.tlp.Dock = DockStyle.Fill;
            this.Controls.Add(tlp);
            
        }
        
        public void Saada_piletid(List<Pilet> piletid)
        {
            
            string text="Sinu ost on \n";
            foreach (var item in piletid)
            {
                text += "Pilet:\n" + "Rida: "+item.Rida+"Koht: "+item.Koht+"\n";  
            }
            
            //message.Attachments.Add(new Attachment("file.pdf"));
            string email = "programmeeriminetthk@gmail.com";
            string password = "2.kuursus";
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.Credentials = new NetworkCredential(email, password);
            client.EnableSsl = true;            
            //client.UseDefaultCredentials = true;

            try
            {

                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress("programmeeriminetthk@gmail.com"));//kellele saada vaja küsida
                message.From = new MailAddress("programmeeriminetthk@gmail.com");
                message.Subject = "Ostetud piletid";
                message.Body = text;
                message.IsBodyHtml = true;
                client.Send(message);
                piletid.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Pileti_valik(object sender, EventArgs e)
        {
            Button btn_click = (Button)sender;
            btn_click.BackColor = Color.Yellow;
            var rida = int.Parse(btn_click.Name[0].ToString());
            var koht = int.Parse(btn_click.Name[1].ToString());
            var vas = MessageBox.Show("Sinu pilet on: Rida: " + rida + " Koht: " +koht, "Kas ostad?", MessageBoxButtons.YesNo);
            if (vas == DialogResult.Yes)
            {
                btn_click.BackColor = Color.Red;
                btn_click.Enabled = false;
                try
                {
                    Pilet pilet = new Pilet(rida, koht);
                    piletid.Add(pilet);
                    StreamWriter ost = new StreamWriter(@"..\..\Piletid\piletid.txt", true);
                    ost.Write(btn_click.Name.ToString() + ';');
                    ost.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (vas == DialogResult.No)
            {
                btn_click.BackColor = Color.Green;
            };

            if (MessageBox.Show("Sul on ostetud: " + piletid.Count() + "piletid", "Kas tahad saada neid e-mailile?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //SendMail("Text");
                Saada_piletid(piletid);
            }

        }
        private void MyForm_Click(object sender, EventArgs e)
            {
                Button btn_click = (Button)sender;
                MessageBox.Show("Oli valitud " + btn_click.Text + " nupp");
            }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Kino";
            this.ResumeLayout(false);
        } 
    }
}
