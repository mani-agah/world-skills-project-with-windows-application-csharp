using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace worldSkills
{
    public partial class UserPanel : Form
    {
        public UserTbl usert;
        WorldSkillsDbEntities context = new WorldSkillsDbEntities();

        public UserPanel(UserTbl user)
        {
            InitializeComponent();
            usert = user; ;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += timer1_Tick;
        }
        private void UserPanel_Load(object sender, EventArgs e)
        {
            LoginLogTbl loginlog = new LoginLogTbl();
            loginlog.LoginLogTime = DateTime.Now;
            loginlog.LoginLogUser = usert.UserId;
            context.LoginLogTbls.Add(loginlog);
            context.SaveChanges();
            var getcom = (from a in context.UserTbls
                          join b in context.CompetitionTbls on a.UserCompetition equals b.CompetitionId
                          where a.UserId == usert.UserId
                          select b).FirstOrDefault();
            timer.Start();
            label13.Text = getcom.CompetitionName;
            var getCompetition = (from competition in context.CompetitionTbls
                                  join time in context.CompetitionTimeTbls on competition.CompetitionId equals time.CompetitionId
                                  join survay in context.SurvayTimeTbls on competition.CompetitionId equals survay.SurvayCom
                                  where competition.CompetitionId == usert.UserCompetition
                                  select new
                                  {
                                      competition.CompetitionId,
                                      time.CompetitionTimeId,
                                      competition.CompetitionName,
                                      time.CompetitionStartDateTime,
                                      survay.SurvayTimeDate
                                  }).ToList();

            foreach (var item in getCompetition)
            {
                if (item.SurvayTimeDate.Subtract(DateTime.Now).TotalSeconds > 0)
                {

                    dataGridView1.DataSource = getCompetition.ToList();
                    startTime = item.SurvayTimeDate;
                    endTime = startTime.AddMinutes(1);
                    timer.Start();
                }
            }
            label1.Text = "لطفا نظرسنجی مورد نظر خود را انتخاب کنید و در آن شرکت کنید";
            var checkNational = (from a in context.SurvayTbls
                                 where a.SurvayUserNationalCode == usert.UserNationalCode
                                 select a).FirstOrDefault();
            try
            {
                txtName.Text = usert.UserName;
                txtLastName.Text = usert.UserLastName;
                txtNationalCode.Text = usert.UserNationalCode;
                dateTimePicker1.Value = usert.UserBirhtday;
                dateTimePicker1.Enabled = false;
                pictureBox1.Load($"C:\\Users\\mani\\Desktop\\c\\worldSkills\\worldSkills\\avatar\\{usert.UserPhoto}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (checkNational == null)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = true;
                groupBox4.Enabled = true;
                groupBox5.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = false;
                groupBox4.Enabled = false;
                groupBox5.Enabled = false;
            }
            /*namayeshe sabtenam konandegan */
            try
            {
                var getUser = from a in context.UserTbls
                              join b in context.States on a.UserStateId equals b.Id
                              join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                              select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                dataGridView2.DataSource = getUser.ToList();
                comboStateWatch.DataSource = context.States.ToList();
                comboStateWatch.DisplayMember = "Name";
                comboStateWatch.ValueMember = "id";
                comboCompetition.DataSource = context.CompetitionTbls.ToList();
                comboCompetition.DisplayMember = "CompetitionName";
                comboCompetition.ValueMember = "CompetitionId";
                comboStateWatch.SelectedIndex = -1;
                comboCompetition.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            /*********************question bank*********************/
            try
            {
                comboLevel.DataSource = context.LevelTbls.ToList();
                comboHardship.DataSource = context.HardshipTbls.ToList();
                comboReshte.DataSource = context.CompetitionTbls.ToList();
                comboTime.DataSource = context.FileTimeTbls.ToList();
                /*display member and value member*/
                comboLevel.DisplayMember = "LevelLabel";
                comboLevel.ValueMember = "LevelId";
                comboHardship.DisplayMember = "HardshipLabel";
                comboHardship.ValueMember = "HardshipId";
                comboReshte.DisplayMember = "CompetitionName";
                comboReshte.ValueMember = "CompetitionId";
                comboTime.ValueMember = "timeId";
                comboTime.DisplayMember = "timeLabel";
            }
            catch
            {

            }
            /*set data*/
            comboReshte.SelectedValue = 2042;
            comboLevel.SelectedValue = 5;
            comboHardship.SelectedValue = 6;
            comboTime.SelectedValue = 23;
            dataGridView3.DataSource = context.QuestionFileTbls.Where(x => x.QuestionFIleId != 3).ToList();

        }
        private DateTime startTime;
        private DateTime endTime;
        private Timer timer;
        private void UserPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogoutLogTbl logoutlog = new LogoutLogTbl();
            logoutlog.LogoutLogTime = DateTime.Now;
            logoutlog.LogoutLogUser = usert.UserId;
            context.LogoutLogTbls.Add(logoutlog);
            context.SaveChanges();
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan remainingTime = endTime - DateTime.Now;
            label2.Text = remainingTime.ToString();
            if (remainingTime <= TimeSpan.Zero)
            {
                timer.Stop();
                var getTheMore = context.SurvayCountTbls.OrderBy(x => x.CompetitionTimeId).Take(1).FirstOrDefault();
                var findCOm = (from a in context.SurvayCountTbls
                               join b in context.CompetitionTbls on a.CompetitionId equals b.CompetitionId
                               join c in context.CompetitionTimeTbls on a.CompetitionId equals c.CompetitionId
                               where a.SurvayCountId == getTheMore.SurvayCountId
                               select new { b.CompetitionName, c.CompetitionStartDateTime, c.CompetitionFinishDateTime }).FirstOrDefault();
                label13.Text = findCOm.CompetitionName;
                label14.Text = findCOm.CompetitionStartDateTime.ToString();
                label16.Text = findCOm.CompetitionFinishDateTime.ToString();
                var getAllCom = (from a in context.UserTbls
                                 where a.UserCompetition == usert.UserCompetition
                                 select a).ToList();
                label12.Text = $"نفر {getAllCom.Count}";
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                SHA256 sha = SHA256.Create();
                byte[] MakeHash = sha.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text));
                string pass = Convert.ToBase64String(MakeHash);
                var checkNowPassword = (from a in context.UserTbls
                                        where a.UserPassword == pass
                                        select a).FirstOrDefault();
                if (checkNowPassword != null)
                {
                    if (txtNewPassword.Text == txtAgainPassword.Text)
                    {
                        byte[] createHash = sha.ComputeHash(Encoding.UTF8.GetBytes(txtAgainPassword.Text));
                        string password = Convert.ToBase64String(createHash);
                        checkNowPassword.UserPassword = password;
                        context.UserTbls.AddOrUpdate(checkNowPassword);
                        context.SaveChanges();
                        MessageBox.Show("رمز عبور با موفقیت عوض شد");
                        txtAgainPassword.Text = null;
                        txtPassword.Text = null;
                        txtNewPassword.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("رمز های وارد شده باهم همخوانی ندارند");
                        txtAgainPassword.Text = null;
                        txtPassword.Text = null;
                        txtNewPassword.Text = null;
                    }
                }
                else
                {
                    MessageBox.Show("رمز عبور فعلی اشتباه وارد شده است");
                    txtAgainPassword.Text = null;
                    txtPassword.Text = null;
                    txtNewPassword.Text = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطایی در تعویض رمز ایجاد شد");
                txtAgainPassword.Text = null;
                txtPassword.Text = null;
                txtNewPassword.Text = null;
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            int comId = (int)dataGridView1.CurrentRow.Cells[0].Value;
            int startTimeId = (int)dataGridView1.CurrentRow.Cells[1].Value;
            SurvayTbl survay = new SurvayTbl();
            survay.SurvayUserNationalCode = usert.UserNationalCode;
            survay.SurvayCompetitionStartTime = startTimeId;
            survay.SurvayCompetition = (int)usert.UserCompetition;
            context.SurvayTbls.Add(survay);
            context.SaveChanges();
            try
            {
                SurvayCountTbl Sc = new SurvayCountTbl();
                Sc.CompetitionId = comId;
                Sc.CompetitionTimeId = startTimeId;
                context.SurvayCountTbls.Add(Sc);
                context.SaveChanges();
            }
            catch
            {
                MessageBox.Show("خطایی در اضافه کردن نظر به وجود امد");
            }
            MessageBox.Show("نظر شما با موفقیت ثبت شد");
            var checkNational = (from a in context.SurvayTbls
                                 where a.SurvayUserNationalCode == usert.UserNationalCode
                                 select a).FirstOrDefault();
            if (checkNational == null)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioButton1.Checked)
                {
                    var getUser = from a in context.UserTbls
                                  join b in context.States on a.UserStateId equals b.Id
                                  join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                  orderby a.UserCompetitionCount
                                  select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                    dataGridView2.DataSource = getUser.ToList();
                    int i = 0;
                    if (Convert.ToInt32(comboStateWatch.SelectedValue) > 0)
                    {
                        int g = Convert.ToInt32(comboStateWatch.SelectedValue);
                        if (g == 32)
                        {
                            var showUser = from a in context.UserTbls
                                           join b in context.States on a.UserStateId equals b.Id
                                           join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                           select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                            dataGridView2.DataSource = showUser.ToList();
                            i = 1;
                        }
                        else
                        {
                            var getUser2 = from a in context.UserTbls
                                           join b in context.States on a.UserStateId equals b.Id
                                           join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                           where a.UserStateId == g
                                           select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                            dataGridView2.DataSource = getUser2.ToList();
                            i = 1;
                        }
                    }
                    if (Convert.ToInt32(comboCompetition.SelectedValue) > 0)
                    {
                        if (i == 1)
                        {
                            int g = Convert.ToInt32(comboCompetition.SelectedValue);
                            int f = Convert.ToInt32(comboStateWatch.SelectedValue);
                            if (g == 32)
                            {
                                if (f == 2042)
                                {
                                    var getUser3 = from a in context.UserTbls
                                                   join b in context.States on a.UserStateId equals b.Id
                                                   join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                                   select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                    dataGridView2.DataSource = getUser3.ToList();
                                }
                                else
                                {
                                    var getUser3 = from a in context.UserTbls
                                                   join b in context.States on a.UserStateId equals b.Id
                                                   join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                                   where a.UserStateId == f
                                                   select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                    dataGridView2.DataSource = getUser3.ToList();
                                }
                            }
                            else
                            {
                                var getUser3 = from a in context.UserTbls
                                               join b in context.States on a.UserStateId equals b.Id
                                               join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                               where a.UserStateId == f &&
                                                   d.CompetitionId == g
                                               select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                dataGridView2.DataSource = getUser3.ToList();
                            }
                        }
                        else
                        {
                            int g = Convert.ToInt32(comboCompetition.SelectedValue);
                            if (g == 2042)
                            {
                                var getUser4 = from a in context.UserTbls
                                               join b in context.States on a.UserStateId equals b.Id
                                               join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                               select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                dataGridView2.DataSource = getUser4.ToList();
                            }
                            else
                            {
                                var getUser4 = from a in context.UserTbls
                                               join b in context.States on a.UserStateId equals b.Id
                                               join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                               where d.CompetitionId == g
                                               select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                dataGridView2.DataSource = getUser4.ToList();
                            }
                        }
                    }
                }
                else if (radioButton2.Checked)
                {
                    var getUser5 = from a in context.UserTbls
                                   join b in context.States on a.UserStateId equals b.Id
                                   join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                   select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                    dataGridView2.DataSource = getUser5.ToList();
                    int i = 0;
                    if (Convert.ToInt32(comboStateWatch.SelectedValue) > 0)
                    {
                        int g = Convert.ToInt32(comboStateWatch.SelectedValue);
                        if (g == 32)
                        {

                            var getUser6 = from a in context.UserTbls
                                           join b in context.States on a.UserStateId equals b.Id
                                           join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                           select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                            dataGridView2.DataSource = getUser6.ToList();
                            i = 1;
                        }
                        else
                        {
                            var getUser6 = from a in context.UserTbls
                                           join b in context.States on a.UserStateId equals b.Id
                                           join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                           where a.UserStateId == g
                                           select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                            dataGridView2.DataSource = getUser6.ToList();
                            i = 1;
                        }
                    }
                    if (Convert.ToInt32(comboCompetition.SelectedValue) > 0)
                    {
                        if (i == 1)
                        {
                            int g = Convert.ToInt32(comboCompetition.SelectedValue);
                            int f = Convert.ToInt32(comboStateWatch.SelectedValue);
                            if (g == 32)
                            {
                                if (f == 2042)
                                {
                                    var getUser3 = from a in context.UserTbls
                                                   join b in context.States on a.UserStateId equals b.Id
                                                   join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                                   select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                    dataGridView2.DataSource = getUser3.ToList();
                                }
                                else
                                {
                                    var getUser3 = from a in context.UserTbls
                                                   join b in context.States on a.UserStateId equals b.Id
                                                   join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                                   where a.UserStateId == f
                                                   select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                    dataGridView2.DataSource = getUser3.ToList();
                                }
                            }
                            else
                            {
                                var getUser3 = from a in context.UserTbls
                                               join b in context.States on a.UserStateId equals b.Id
                                               join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                               where a.UserStateId == f &&
                                                   d.CompetitionId == g
                                               select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                dataGridView2.DataSource = getUser3.ToList();
                            }
                        }
                    }
                    else
                    {
                        var getUser12 = from a in context.UserTbls
                                        join b in context.States on a.UserStateId equals b.Id
                                        join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                        select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                        dataGridView2.DataSource = getUser12.ToList();
                        int q = 0;
                        if (Convert.ToInt32(comboStateWatch.SelectedValue) > 0)
                        {
                            int g = Convert.ToInt32(comboStateWatch.SelectedValue);
                            if (g == 32)
                            {

                                var getUser6 = from a in context.UserTbls
                                               join b in context.States on a.UserStateId equals b.Id
                                               join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                               select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                dataGridView2.DataSource = getUser6.ToList();
                                q = 1;
                            }
                            else
                            {
                                var getUser6 = from a in context.UserTbls
                                               join b in context.States on a.UserStateId equals b.Id
                                               join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                               where a.UserStateId == g
                                               select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                dataGridView2.DataSource = getUser6.ToList();
                                q = 1;
                            }
                        }
                        if (Convert.ToInt32(comboCompetition.SelectedValue) > 0)
                        {
                            if (q == 1)
                            {
                                int g = Convert.ToInt32(comboCompetition.SelectedValue);
                                int f = Convert.ToInt32(comboStateWatch.SelectedValue);
                                if (g == 32)
                                {
                                    if (f == 2042)
                                    {
                                        var getUser3 = from a in context.UserTbls
                                                       join b in context.States on a.UserStateId equals b.Id
                                                       join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                                       select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                        dataGridView2.DataSource = getUser3.ToList();
                                    }
                                    else
                                    {
                                        var getUser3 = from a in context.UserTbls
                                                       join b in context.States on a.UserStateId equals b.Id
                                                       join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                                       where a.UserStateId == f
                                                       select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                        dataGridView2.DataSource = getUser3.ToList();
                                    }
                                }
                                else
                                {
                                    var getUser3 = from a in context.UserTbls
                                                   join b in context.States on a.UserStateId equals b.Id
                                                   join d in context.CompetitionTbls on a.UserCompetition equals d.CompetitionId
                                                   where a.UserStateId == f &&
                                                       d.CompetitionId == g
                                                   select new { a.UserId, a.UserName, a.UserLastName, d.CompetitionName, b.Name, a.UserCompetitionCount };
                                    dataGridView2.DataSource = getUser3.ToList();
                                }
                            }
                        }
                    }
                }
            }

            catch
            {

            }
        }

        private void comboStateWatch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public int i = 0;
        public int j = 0;
        private void button6_Click(object sender, EventArgs e)
        {
            /*all of the id*/
            int LevelId = Convert.ToInt32(comboLevel.SelectedValue);
            int HardShipId = Convert.ToInt32(comboHardship.SelectedValue);
            int CompetitionId = Convert.ToInt32(comboReshte.SelectedValue);
            int TimeId = Convert.ToInt32(comboTime.SelectedValue);
            /*filter*/
            if (LevelId == 5)
            {
                var getAllData = (from a in context.QuestionFileTbls
                                  select a).ToList();
                dataGridView3.DataSource = getAllData;
                if (HardShipId == 6)
                {
                    dataGridView3.DataSource = getAllData;
                }
                else
                {
                    var getDataWithHardShip = (from a in context.QuestionFileTbls
                                               where a.QuestionHardship == HardShipId
                                               select a).ToList();
                    dataGridView3.DataSource = getDataWithHardShip;
                    j = 1;
                }
                if (CompetitionId == 2042)
                {
                    if (j == 1)
                    {
                        dataGridView3.DataSource = getAllData.Where(x => x.QuestionHardship == HardShipId).ToList();
                    }
                    else
                    {
                        dataGridView3.DataSource = getAllData;
                    }
                }
                else
                {
                    if (j == 1)
                    {
                        var getCompetition = (from a in context.QuestionFileTbls
                                              where a.QuestionCompetitionId == CompetitionId && a.QuestionHardship == HardShipId
                                              select a).ToList();
                        dataGridView3.DataSource = getCompetition;
                    }
                    else
                    {
                        var getCompetition = (from a in context.QuestionFileTbls
                                              where a.QuestionCompetitionId == CompetitionId
                                              select a).ToList();
                        dataGridView3.DataSource = getCompetition;
                    }
                    i = 1;
                }
                if (TimeId == 23)
                {
                    if (j == 1)
                    {
                        if (i == 1)
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionId == CompetitionId && a.QuestionHardship == HardShipId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                        else
                        {
                            dataGridView3.DataSource = getAllData.Where(x => x.QuestionHardship == HardShipId).ToList();
                        }
                    }
                    else
                    {
                        if (i == 1)
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionId == CompetitionId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                        else
                        {
                            dataGridView3.DataSource = getAllData;
                        }
                    }
                }
                else
                {
                    if (j == 1)
                    {
                        if (i == 1)
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionDateTime == TimeId && a.QuestionCompetitionId == CompetitionId && a.QuestionHardship == HardShipId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                        else
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionDateTime == TimeId && a.QuestionHardship == HardShipId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                    }
                    else
                    {
                        if (i == 1)
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionDateTime == TimeId && a.QuestionCompetitionId == CompetitionId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                        else
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionDateTime == TimeId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                    }
                }
            }
            else
            {
                var getAllData = (from a in context.QuestionFileTbls
                                  where a.QuestionCompetitionLevel == LevelId
                                  select a).ToList();
                dataGridView3.DataSource = getAllData;
                if (HardShipId == 6)
                {
                    dataGridView3.DataSource = getAllData;
                }
                else
                {
                    var getDataWithHardShip = (from a in context.QuestionFileTbls
                                               where a.QuestionHardship == HardShipId && a.QuestionCompetitionLevel == LevelId
                                               select a).ToList();
                    dataGridView3.DataSource = getDataWithHardShip;
                    j = 1;
                }
                if (CompetitionId == 2042)
                {
                    if (j == 1)
                    {
                        dataGridView3.DataSource = getAllData.Where(x => x.QuestionHardship == HardShipId).ToList();
                    }
                    else
                    {
                        dataGridView3.DataSource = getAllData;
                    }
                }
                else
                {
                    if (j == 1)
                    {
                        var getCompetition = (from a in context.QuestionFileTbls
                                              where a.QuestionCompetitionId == CompetitionId && a.QuestionHardship == HardShipId
                                              && a.QuestionCompetitionLevel == LevelId
                                              select a).ToList();
                        dataGridView3.DataSource = getCompetition;
                    }
                    else
                    {
                        var getCompetition = (from a in context.QuestionFileTbls
                                              where a.QuestionCompetitionId == CompetitionId
                                              && a.QuestionCompetitionLevel == LevelId
                                              select a).ToList();
                        dataGridView3.DataSource = getCompetition;
                    }
                    i = 1;
                }
                if (TimeId == 23)
                {
                    if (j == 1)
                    {
                        if (i == 1)
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionId == CompetitionId && a.QuestionHardship == HardShipId
                                           && a.QuestionCompetitionLevel == LevelId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                        else
                        {
                            dataGridView3.DataSource = getAllData.Where(x => x.QuestionHardship == HardShipId).ToList();
                        }
                    }
                    else
                    {
                        if (i == 1)
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionId == CompetitionId
                                           && a.QuestionCompetitionLevel == LevelId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                        else
                        {
                            dataGridView3.DataSource = getAllData;
                        }
                    }
                }
                else
                {
                    if (j == 1)
                    {
                        if (i == 1)
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionDateTime == TimeId && a.QuestionCompetitionId == CompetitionId && a.QuestionHardship == HardShipId
                                           && a.QuestionCompetitionLevel == LevelId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                        else
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionDateTime == TimeId && a.QuestionHardship == HardShipId
                                            && a.QuestionCompetitionLevel == LevelId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                    }
                    else
                    {
                        if (i == 1)
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionDateTime == TimeId && a.QuestionCompetitionId == CompetitionId
                                           && a.QuestionCompetitionLevel == LevelId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                        else
                        {
                            var getTime = (from a in context.QuestionFileTbls
                                           where a.QuestionCompetitionDateTime == TimeId
                                           && a.QuestionCompetitionLevel == LevelId
                                           select a).ToList();
                            dataGridView3.DataSource = getTime;
                        }
                    }
                }
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {

            int id = (int)dataGridView3.CurrentRow.Cells[0].Value;
            var getFile = (from a in context.QuestionFileTbls
                           where a.QuestionFIleId == id
                           select a).FirstOrDefault();
            string filepath = @"C:\Users\mani\Desktop\c\worldSkills\worldSkills\bin\Debug\" + getFile.QuestionFileName;
            string path = filepath;
            if (System.IO.File.Exists(path))
            {
                try
                {
                    Process.Start("chrome.exe", path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"خطا در اجرای Chrome: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("فایل مورد نظر یافت نشد.");
            }
        }
    }
}

