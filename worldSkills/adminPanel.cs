using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace worldSkills
{
    public partial class adminPanel : Form
    {
        public int count = 1;
        WorldSkillsDbEntities context = new WorldSkillsDbEntities();
        public int comId;
        public UserTbl users;
        public adminPanel(UserTbl user)
        {
            InitializeComponent();

            users = user;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        private void getCom()
        {
            var getCom = (from a in context.CompetitionTbls
                          select a).ToList();
            foreach (var item in getCom)
            {
                var CountSurvay = (from a in context.SurvayTbls
                                   where a.SurvayCompetition == item.CompetitionId
                                   select a).ToList();

            }
        }
        private void adminPanel_Load(object sender, EventArgs e)
        {
            LoginLogTbl log = new LoginLogTbl();
            log.LoginLogTime = DateTime.Now;
            log.LoginLogUser = users.UserId;
            context.LoginLogTbls.Add(log);
            context.SaveChanges();
            dataGridView4.DataSource = context.UserTbls.Where(x => x.UserRull == 2).ToList();
            button2.Text = $"ثبت زمان {count}";
            dataGridView1.DataSource = context.CompetitionTbls.ToList();
            var getUser = (from a in context.UserTbls
                           where a.UserRull == 2 && a.UserStatus == 2
                           select new { a.UserId, a.UserName, a.UserLastName, a.UserNationalCode, a.UserStatus }).ToList();
            dataGridView2.DataSource = getUser;
            var getAdmin = (from a in context.UserTbls
                            join b in context.RullTbls on a.UserRull equals b.RullId
                            where a.UserRull == 1
                            select new { a.UserId, a.UserName, a.UserLastName, a.UserNationalCode, b.RullName, a.UserBirhtday }).ToList();
            dataGridView3.DataSource = getAdmin;
            dataGridView1.DataSource = context.CompetitionTbls.Where(x => x.CompetitionId != 2042).ToList();
            /********************file upload*********************/
            var getQuestion = (from a in context.QuestionFileTbls
                               join b in context.LevelTbls on a.QuestionCompetitionLevel equals b.LevelId
                               join c in context.HardshipTbls on a.QuestionHardship equals c.HardshipId
                               join d in context.CompetitionTbls on a.QuestionCompetitionId equals d.CompetitionId
                               join f in context.FileTimeTbls on a.QuestionCompetitionDateTime equals f.timeId
                               select new { a.QuestionFIleId, a.QuestionFileName, b.LevelLabel, c.Hardshiplabel, d.CompetitionName, f.timeLabel }).ToList();
            dgvFIle.DataSource = getQuestion.Where(x => x.QuestionFIleId != 3).ToList();
            comboCompetitionFile.DataSource = context.CompetitionTbls.Where(x => x.CompetitionId != 2042).ToList();
            comboFileTime.DataSource = context.FileTimeTbls.Where(x => x.timeId != 23).ToList();
            comboLevel.DataSource = context.LevelTbls.Where(x => x.LevelId != 5).ToList();
            comboHardest.DataSource = context.HardshipTbls.Where(x => x.HardshipId != 6).ToList();
            comboCompetitionFile.ValueMember = "CompetitionId";
            comboCompetitionFile.DisplayMember = "CompetitionName";
            comboLevel.ValueMember = "LevelId";
            comboLevel.DisplayMember = "LevelLabel";
            comboHardest.ValueMember = "HardshipId";
            comboHardest.DisplayMember = "HardshipLabel";
            comboFileTime.DisplayMember = "TimeLabel";
            comboFileTime.ValueMember = "TimeId";
            comboState.DataSource = context.States.Where(x => x.Id != 32).ToList();
            comboState.ValueMember = "Id";
            comboState.DisplayMember = "Name";
            if (comboState.SelectedIndex == 0)
            {
                comboState.SelectedItem = null;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            LogoutLogTbl log = new LogoutLogTbl();
            log.LogoutLogTime = DateTime.Now;
            log.LogoutLogUser = users.UserId;
            context.LogoutLogTbls.Add(log);
            context.SaveChanges();
            this.Close();
        }
        public CompetitionTbl comID;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CompetitionTbl com = new CompetitionTbl();
                com.CompetitionName = txtMos.Text;
                context.CompetitionTbls.Add(com);
                context.SaveChanges();
                dataGridView1.DataSource = context.CompetitionTbls.ToList();
                var getCOm = (from a in context.CompetitionTbls
                              where a.CompetitionId == com.CompetitionId
                              select a).FirstOrDefault();
                MessageBox.Show("مسابقه با موفقیت اضافه شد");
                dataGridView1.DataSource = context.CompetitionTbls.Where(x => x.CompetitionId != 2042).ToList();
                txtMos.Text = null;
                comID = getCOm;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطایی در اضافه کردن مسابقه پیش امده است");
            }
        }
        public CompetitionTimeTbl comtime;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                CompetitionTimeTbl com = new CompetitionTimeTbl();
                com.CompetitionStartDateTime = dateTimePicker1.Value;
                com.CompetitionId = comID.CompetitionId;
                com.CompetitionFinishDateTime = dateTimePicker2.Value;
                context.CompetitionTimeTbls.Add(com);
                context.SaveChanges();
                MessageBox.Show($"زمان {count} ثبت شد");
                count++;
                button2.Text = $"ثبت زمان {count}";
                var getCom = (from a in context.CompetitionTimeTbls
                              where a.CompetitionTimeId == com.CompetitionTimeId
                              select a).FirstOrDefault();
                comtime = getCom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ثبت زمان");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridView2.CurrentRow.Cells[0].Value;
                var getUser = (from a in context.UserTbls
                               where a.UserId == id
                               select a).FirstOrDefault();
                getUser.UserStatus = 1;
                context.UserTbls.AddOrUpdate(getUser);
                context.SaveChanges();
                MessageBox.Show("درخواست کاربر با موفقیت قبول شد");
                var showUser = (from a in context.UserTbls
                                where a.UserRull == 2 && a.UserStatus == 2
                                select new
                                {
                                    a.UserId,
                                    a.UserName,
                                    a.UserLastName,
                                    a.UserNationalCode,
                                    a.UserStatus
                                }).ToList();
                dataGridView2.DataSource = showUser;
            }
            catch
            {
                MessageBox.Show("خطایی در قبول کردن درخواست شرکت کننده پیش امده");
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridView2.CurrentRow.Cells[0].Value;
                var getUser = (from a in context.UserTbls
                               where a.UserId == id
                               select a).FirstOrDefault();
                getUser.UserStatus = 3;
                context.UserTbls.AddOrUpdate(getUser);
                context.SaveChanges();
                MessageBox.Show("درخواست کاربر با موفقیت رد شد");
                var showUser = (from a in context.UserTbls
                                where a.UserRull == 2 && a.UserStatus == 2
                                select new { a.UserId, a.UserName, a.UserLastName, a.UserNationalCode, a.UserStatus }).ToList();
                dataGridView2.DataSource = showUser;
            }
            catch
            {
                MessageBox.Show("خطایی در قبول کردن درخواست شرکت کننده پیش امده");
            }
        }
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
        private void adminPanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            LogoutLogTbl log = new LogoutLogTbl();
            log.LogoutLogTime = DateTime.Now;
            log.LogoutLogUser = users.UserId;
            context.LogoutLogTbls.Add(log);
            context.SaveChanges();
            this.Close();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = (int)dataGridView3.CurrentRow.Cells[0].Value;
            var getPhoto = (from a in context.UserTbls
                            where a.UserId == id
                            select a).FirstOrDefault();
            pictureBox1.Image = new Bitmap($"~/avatar/{getPhoto.UserPhoto}");
            
            
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int id = (int)dataGridView3.CurrentRow.Cells[0].Value;
                var getPhoto = (from a in context.UserTbls
                                where a.UserId == id
                                select a).FirstOrDefault();
                pictureBox1.Load($"C:\\Users\\mani\\Desktop\\c\\worldSkills\\worldSkills\\avatar\\{getPhoto.UserPhoto}");
                txtName.Text = getPhoto.UserName;
                txtLastName.Text = getPhoto.UserLastName;
                txtNationalCode.Text = getPhoto.UserNationalCode;
                dateBirthday.Value = getPhoto.UserBirhtday;
                comboState.SelectedValue = getPhoto.UserStateId;
                comboCity.SelectedValue = getPhoto.UserCityId;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtLastName != null && txtName.Text != null && txtNationalCode.Text != null && txtPassword.Text != null && dateBirthday.Value != DateTime.Now)
                {
                    SHA256 sha = SHA256.Create();
                    byte[] hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text));
                    var pass = Convert.ToBase64String(hashed);
                    UserTbl user = new UserTbl();
                    user.UserStateId = Convert.ToInt32(comboState.SelectedValue);
                    user.UserCityId = Convert.ToInt32(comboCity.SelectedValue);
                    user.UserName = txtName.Text;
                    user.UserCompetitionCount = 0;
                    user.UserLastName = txtLastName.Text;
                    user.UserPassword = pass;
                    user.UserNationalCode = txtNationalCode.Text;
                    user.UserCompetition = null;
                    user.UserStatus = null;
                    user.UserBirhtday = dateBirthday.Value;
                    string[] photoName = { "beautiful-latin-woman-avatar-character-icon-vector-33983177.jpg", "download.jpg", "images.jpg",
                "woman-female-avatar-character-vector-11708688.jpg", "young-woman-avatar-character-vector-14212379.jpg",
                "young-woman-avatar-character-vector-14213306.jpg", "young-woman-avatar-character-vector-25604264.jpg" };
                    int i = listView1.SelectedItems[0].ImageIndex;
                    string findPhoto = photoName[i];
                    user.UserPhoto = findPhoto;
                    user.UserRull = 1;
                    context.UserTbls.Add(user);
                    context.SaveChanges();
                    MessageBox.Show("کاربر با موفقیت ثبت نام شدید");
                    txtName.Text = null;
                    comboState.SelectedIndex = -1;
                    comboCity.SelectedIndex = -1;

                    txtLastName.Text = null;
                    txtNationalCode.Text = null;
                    txtPassword.Text = null;
                    dateBirthday.Value = DateTime.Now;
                    var getAdmin = (from a in context.UserTbls
                                    join b in context.RullTbls on a.UserRull equals b.RullId
                                    where a.UserRull == 1
                                    select new { a.UserId, a.UserName, a.UserLastName, a.UserNationalCode, b.RullName, a.UserBirhtday }).ToList();
                    dataGridView3.DataSource = getAdmin;
                }
                else
                {
                    MessageBox.Show("لطفا تمام فیلد هارا پر کنید");
                }

            }
            catch
            {
                MessageBox.Show("خطایی در ثبت نام به وجود آمده");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridView1.CurrentRow.Cells[0].Value;
                var getChamp = (from a in context.CompetitionTbls
                                where a.CompetitionId == id
                                select a).FirstOrDefault();
                context.CompetitionTbls.Remove(getChamp);
                context.SaveChanges();
                MessageBox.Show("حذف مسابقه با موفقیت انجام شد");
                dataGridView1.DataSource = context.CompetitionTbls.Where(x => x.CompetitionId != 2042).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف رشته");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                string c = $"{txtYear.Text}/{txtMonth.Text}/{txtDay.Text} {txtHour.Text}:{txtMin.Text}";
                DateTime g = Convert.ToDateTime(c);
                SurvayTimeTbl survay = new SurvayTimeTbl();
                survay.SurvayTimeDate = g;
                survay.SurvayStartComDate = comtime.CompetitionTimeId;
                survay.SurvayCom = comID.CompetitionId;
                context.SurvayTimeTbls.Add(survay);
                context.SaveChanges();
                MessageBox.Show("زمان نظر سنجی با موفقیت ثبت شد");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ثبت نظرسنجی با مشکل رو به رو شد");
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridView4.CurrentRow.Cells[0].Value;
                var getUser = (from a in context.UserTbls
                               where a.UserId == id
                               select a).FirstOrDefault();
                context.UserTbls.Remove(getUser);
                context.SaveChanges();
                MessageBox.Show("کاربر با موفقیت حذف شد");
                dataGridView4.DataSource = context.UserTbls.Where(x => x.UserRull == 2).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حذف کاربر با مشکل رو به رو شد");
            }
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }
        public string fileName;
        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog sd = new OpenFileDialog();
                sd.Filter = "*.pdf | *.pdf";
                sd.ShowDialog();
                File.Copy(sd.FileName, Path.Combine(@"C:\Users\mani\Desktop\c\worldSkills\worldSkills\bin\Debug", Path.GetFileName(sd.FileName)), true);
                fileName = sd.SafeFileName;
            }
            catch
            {

            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                HardshipTbl hard = new HardshipTbl();
                LevelTbl level = new LevelTbl();
                QuestionFileTbl question = new QuestionFileTbl();
                CompetitionTbl competition = new CompetitionTbl();
                FileTimeTbl fileTime = new FileTimeTbl();
                int levelId = Convert.ToInt32(comboLevel.SelectedValue);
                int hardshipId = Convert.ToInt32(comboHardest.SelectedValue);
                int competitionId = Convert.ToInt32(comboCompetitionFile.SelectedValue);
                int timeId = Convert.ToInt32(comboFileTime.SelectedValue);
                var getHardShip = (from a in context.HardshipTbls
                                   where a.HardshipId == hardshipId
                                   select a).FirstOrDefault();
                var getLevel = (from a in context.LevelTbls
                                where a.LevelId == levelId
                                select a).FirstOrDefault();
                var getCompetition = (from a in context.CompetitionTbls
                                      where a.CompetitionId == competitionId
                                      select a).FirstOrDefault();
                var getTimeId = (from a in context.FileTimeTbls
                                 where a.timeId == timeId
                                 select a).FirstOrDefault();
                question.QuestionCompetitionLevel = getLevel.LevelId;
                question.QuestionCompetitionDateTime = getTimeId.timeId;
                question.QuestionHardship = getHardShip.HardshipId;
                question.QuestionFileName = fileName;
                question.QuestionCompetitionId = getCompetition.CompetitionId;
                question.QuestionUploadedUser = users.UserId;
                context.QuestionFileTbls.Add(question);
                context.SaveChanges();
                var getQuestion = (from a in context.QuestionFileTbls
                                   join b in context.LevelTbls on a.QuestionCompetitionLevel equals b.LevelId
                                   join c in context.HardshipTbls on a.QuestionHardship equals c.HardshipId
                                   join d in context.CompetitionTbls on a.QuestionCompetitionId equals d.CompetitionId
                                   join f in context.FileTimeTbls on a.QuestionCompetitionDateTime equals f.timeId
                                   select new { a.QuestionFIleId, a.QuestionFileName, b.LevelLabel, c.Hardshiplabel, d.CompetitionName, f.timeLabel }).ToList();
                dgvFIle.DataSource = getQuestion.Where(x => x.QuestionFIleId != 3).ToList();
                MessageBox.Show("نمونه سوال با موفقیت ثبت شد");
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطایی در ثبت نمونه سوال به وجود امده است");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dgvFIle.CurrentRow.Cells[0].Value;
                var getFIle = (from a in context.QuestionFileTbls
                               where a.QuestionFIleId == id
                               select a).FirstOrDefault();
                context.QuestionFileTbls.Remove(getFIle);
                context.SaveChanges();
                var getQuestion = (from a in context.QuestionFileTbls
                                   join b in context.LevelTbls on a.QuestionCompetitionLevel equals b.LevelId
                                   join c in context.HardshipTbls on a.QuestionHardship equals c.HardshipId
                                   join d in context.CompetitionTbls on a.QuestionCompetitionId equals d.CompetitionId
                                   join f in context.FileTimeTbls on a.QuestionCompetitionDateTime equals f.timeId
                                   select new { a.QuestionFIleId, a.QuestionFileName, b.LevelLabel, c.Hardshiplabel, d.CompetitionName, f.timeLabel }).ToList();
                dgvFIle.DataSource = getQuestion.Where(x => x.QuestionFIleId != 3).ToList();
                MessageBox.Show("حذف سوال با موفقیت انجام شد");

            }
            catch
            {
                MessageBox.Show("خطایی در حذف سوال به وجود امده است");
            }

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboState_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboState_SelectedIndexChanged_1(object sender, EventArgs e)
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
                        comboCity.DataSource = context.Cities.Where(x => x.StateId == getState.Id).ToList();
                        comboCity.DisplayMember = "Name";
                        comboCity.ValueMember = "Id";
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
