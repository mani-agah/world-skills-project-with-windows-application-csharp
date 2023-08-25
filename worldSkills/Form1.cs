using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace worldSkills
{
    public partial class Form1 : Form
    {
        WorldSkillsDbEntities context = new WorldSkillsDbEntities();
        private const int CaptchaWidth = 200;
        private const int CaptchaHeight = 60;
        private const int CaptchaLength = 6;
        private Random random = new Random();   
        private string captchaCode;

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            GenerateCaptcha();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void GenerateCaptcha()
        {
            captchaCode = GenerateRandomCode(CaptchaLength);
            DrawCaptcha(captchaCode);
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz1234567890";
            char[] code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }
            return new string(code);
        }

        private void DrawCaptcha(string code)
        {
            Bitmap bmp = new Bitmap(CaptchaWidth, CaptchaHeight);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.Clear(Color.White);
                Font font = new Font("Arial", 20, FontStyle.Bold);
                Brush brush = new SolidBrush(Color.Black);
                Point point = new Point(10, 10);
                graphics.DrawString(code, font, brush, point);
            }

            pictureBox1.Image = bmp;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SHA256 sha = SHA256.Create();
                byte[] hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text));
                var pass = Convert.ToBase64String(hashed);
                var checkUser = (from a in context.UserTbls
                                 where a.UserName == textBox1.Text && a.UserPassword == pass
                                 select a).FirstOrDefault();

                if (textBox1.Text != string.Empty && textBox2.Text != string.Empty)
                {
                    if (checkUser != null)
                    {
                        if (textBoxCaptcha.Text == captchaCode)
                        {
                            if (checkUser.UserRull == 1)
                            {
                                adminPanel admin = new adminPanel(checkUser);
                                admin.ShowDialog();
                                textBox1.Text = null;
                                textBox2.Text = null;
                                textBoxCaptcha.Text = null;
                                GenerateCaptcha();
                            }
                            else
                            {
                                if (checkUser.UserStatus == 2)
                                {
                                    MessageBox.Show("وضعیت شما در حال برسی میباشد");
                                }
                                else if (checkUser.UserStatus == 3)
                                {
                                    MessageBox.Show("درخواست شما در شده است");
                                }
                                else if (checkUser.UserStatus == 1)
                                {
                                    UserPanel user = new UserPanel(checkUser);
                                    user.ShowDialog();
                                    textBox1.Text = null;
                                    textBox2.Text = null;
                                    textBoxCaptcha.Text = null;
                                    GenerateCaptcha();
                                }
                                else
                                {
                                    MessageBox.Show("برنامه با مشکل رو به رو شد");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("کد کپچا اشتباه است. دوباره امتحان کنید.");
                            GenerateCaptcha();
                            textBoxCaptcha.Clear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("نام کاربری و یا رمز عبور اشتباه است");
                    }
                }
                else
                {
                    MessageBox.Show("لطفا فیلد هارا پر کنید");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ورود");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SignUpChamp su = new SignUpChamp();
            su.ShowDialog();
            textBox1.Text = null;
            textBox2.Text = null;
            textBoxCaptcha.Text = null;
            GenerateCaptcha();
        }
        private void time_lbl_Click(object sender, EventArgs e)
        {

        }
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            DateTime datetime = DateTime.Now;
            this.label3.Text = $"{datetime.Hour}:{datetime.Minute}:{datetime.Second}";
        }

        private void buttonRefresh_Click_1(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }
        private void button5_Click(object sender, EventArgs e)
        {
        }
        private void textBoxCaptcha_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
