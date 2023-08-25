using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace worldSkills
{
    public partial class SignUpChamp : Form
    {
        WorldSkillsDbEntities context = new WorldSkillsDbEntities();
        public SignUpChamp()
        {
            InitializeComponent();
        }
        static bool IsNationalCodeValid(string input)
        {
            string sanitizedInput = Regex.Replace(input, @"\s", "");
            string nationalCodePattern = @"^\d{11}$";
            return Regex.IsMatch(sanitizedInput, nationalCodePattern);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            bool checkBool = IsNationalCodeValid(txtNationalCode.Text);
            try
            { 
                if (checkBool)
                {
                    if (DateTime.Now.Subtract(dateTimePicker1.Value).TotalDays > 7665 || DateTime.Now.Subtract(dateTimePicker1.Value).TotalDays < 4745)
                    {
                        MessageBox.Show("سن شما مناسب این مسابقات نیست");
                    }
                    else
                    {
                        if (txtLastName != null && txtName.Text != null && txtNationalCode.Text != null && txtPassword.Text != null && dateTimePicker1.Value != DateTime.Now && comboBox1.SelectedValue != null && comboState.SelectedValue != null && comboBox3.SelectedValue != null)
                        {
                            SHA256 sha = SHA256.Create();
                            byte[] hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text));
                            var pass = Convert.ToBase64String(hashed);
                            UserTbl user = new UserTbl();
                            user.UserName = txtName.Text;
                            user.UserLastName = txtLastName.Text;
                            user.UserPassword = pass;
                            user.UserNationalCode = txtNationalCode.Text;
                            user.UserCompetition = (int)comboBox1.SelectedValue;
                            user.UserStatus = 2;
                            user.UserBirhtday = dateTimePicker1.Value;
                            string[] photoName = { "beautiful-latin-woman-avatar-character-icon-vector-33983177.jpg", "download.jpg", "images.jpg",
                "woman-female-avatar-character-vector-11708688.jpg", "young-woman-avatar-character-vector-14212379.jpg",
                "young-woman-avatar-character-vector-14213306.jpg", "young-woman-avatar-character-vector-25604264.jpg" };
                            int i = listView1.SelectedItems[0].ImageIndex;
                            string findPhoto = photoName[i];
                            user.UserPhoto = findPhoto;
                            user.UserStateId = Convert.ToInt32(comboState.SelectedValue);
                            user.UserCityId = Convert.ToInt32(comboBox3.SelectedValue);
                            user.UserRull = 2;
                            user.UserCompetitionCount = (int)numericUpDown1.Value;
                            context.UserTbls.Add(user);
                            context.SaveChanges();
                            MessageBox.Show("شما با موفقیت ثبت نام شدید" +
                                "لطفا منتظر برسی شدن باشید ");
                            txtName.Text = null;
                            txtLastName.Text = null;
                            txtNationalCode.Text = null;
                            txtPassword.Text = null;
                            dateTimePicker1.Value = DateTime.Now;
                            comboBox1.SelectedIndex = 0;
                        }
                        else
                        {
                            MessageBox.Show("لطفا تمام فیلد هارا پر کنید");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("کد ملی اشتباه است");
                }
            }
            catch
            {
                MessageBox.Show("خطایی در ثبت نام به وجود آمده");
            }
        }
        private void SignUpChamp_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = context.CompetitionTbls.Where(x=>x.CompetitionId != 2042).ToList();
            comboBox1.ValueMember = "CompetitionId";
            comboBox1.DisplayMember = "CompetitionName";
            comboState.DataSource = context.States.ToList();
            comboState.ValueMember = "Id";
            comboState.DisplayMember = "Name";
            progressBar1.Maximum = 7;
            if (comboState.SelectedIndex == 0)
            {
                comboState.SelectedItem = null;
            }
        }
        public int count = 0;
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtName.Text != string.Empty)
            {
                progressBar1.Value = 2;
                label8.Text = "29%";
            }
        }

        private void txtNationalCode_TextChanged(object sender, EventArgs e)
        {

            if (txtNationalCode.Text != string.Empty)
            {
                progressBar1.Value = 4;
                label8.Text = "58%";
            }
        }



        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtPassword.Text != string.Empty)
            {
                progressBar1.Value = 5;
                label8.Text = "72.5%";
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value != DateTime.Now)
            {
                progressBar1.Value = 6;
                label8.Text = "87%";
            }
        }

        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            if (txtLastName.Text != string.Empty)
            {
                progressBar1.Value = 3;
                label8.Text = "43.5%";
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                progressBar1.Value = 1;
                label8.Text = "14.5%";
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                progressBar1.Value = 7;
                label8.Text = "completed";
            }
        }

        private void comboState_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(comboState.SelectedValue) > 0)
                {
                    int id = (int)comboState.SelectedValue;
                    var getState = (from b in context.States
                                    where b.Id == id
                                    select b).FirstOrDefault();
                    if (getState.Id != 1)
                    {
                        comboBox3.DataSource = context.Cities.Where(x => x.StateId == getState.Id).ToList();
                        comboBox3.DisplayMember = "Name";
                        comboBox3.ValueMember = "Id";
                    }
                }
                else
                {

                }
            }
            catch
            {

            }
        }
    }
}
