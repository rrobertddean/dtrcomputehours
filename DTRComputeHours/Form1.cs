using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DTRComputeHours
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnSetTime_Click(object sender, EventArgs e)
        {
            var timein = txtTimeIn.Text;
            var timeout = txtTimeOut.Text;

            lblTimeIn.Text = DateTime.Parse(timein).ToString(@"hh\:mm tt");
            lblTimeOut.Text = DateTime.Parse(timeout).ToString(@"hh\:mm tt");

            
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            if (lblTimeIn.Text == "0" || lblTimeOut.Text == "0")
            {
                MessageBox.Show("Please Set Time In Time Out");
                return;
            }

            int late = 0;
            int undertime = 0;

            //Time in Time Out Company
            var companytimein = lblTimeIn.Text;
            var companytimeout = lblTimeOut.Text;


            #region Day 1 Computation

            //Day 1 Computation
            try
            {

                int totalminuteday1 = 0;
                int totalotminuteday1 = 0;
                int deducttimeoverbreak = 0;

                var morninginday1 = DateTime.Parse(txtMorningInDay1.Text);
                var morningoutday1 = DateTime.Parse(txtMorningOutDay1.Text);

                var afternooninday1 = DateTime.Parse(txtAftInDay1.Text);
                var afternoonoutday1 = DateTime.Parse(txtAftOutDay1.Text);

                var otinday1 = DateTime.Parse(txtOTInDay1.Text);
                var otoutday1 = DateTime.Parse(txtOTOutDay1.Text);

                var ambreaktime = Convert.ToInt32(cmbBreakAMDay1.Text);
                var pmbreaktime = Convert.ToInt32(cmbBreakPMDay1.Text);

               
                //check if emplyoee is earlier if earlier set time in to company time in
                if (morninginday1 <= DateTime.Parse(companytimein))
                {
                    morninginday1 = DateTime.Parse(lblTimeIn.Text);
                }

                //afternoon out time
                if (afternoonoutday1 >= DateTime.Parse(companytimeout)) 
                {
                    afternoonoutday1 = DateTime.Parse(lblTimeOut.Text);
                }


                if (cmbBreakAMDay1.Text != "0")
                {
                    //morning break time
                    var ambreaktimets = afternooninday1.Subtract(morningoutday1);
                    var ambreaktimeint = Convert.ToInt32(ambreaktimets.TotalMinutes);

                    if (ambreaktime < ambreaktimeint)
                    {
                        int breaktime = ambreaktimeint - ambreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }
                

                if (cmbBreakPMDay1.Text != "0") 
                {
                    //afternoon break time
                    var pmbreaktimets = otinday1.Subtract(afternoonoutday1);
                    var pmbreaktimeint = Convert.ToInt32(pmbreaktimets.TotalMinutes);

                    if (pmbreaktime < pmbreaktimeint)
                    {
                        int breaktime = pmbreaktimeint - pmbreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }

                //add late if breaktime is over 
                //late = late + deducttime;
               
                //get total minute of day
                if (chkDay1CoffeeBreak.Checked == true && chkOTDay1.Checked == true)
                {

                    if (otoutday1 > DateTime.Parse(companytimeout))
                    {
                        otoutday1 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday1 = getTotalMinuteCoffeeBreakOT(morninginday1, morningoutday1,
                    afternooninday1, afternoonoutday1, otinday1, otoutday1,
                    deducttimeoverbreak, txtMorningInDay1.Text, txtMorningOutDay1.Text,
                    txtAftInDay1.Text, txtAftOutDay1.Text, txtOTInDay1.Text, txtOTOutDay1.Text,
                    companytimeout);

                    TimeSpan tstotalotminuteday1;
                    tstotalotminuteday1 = DateTime.Parse(txtOTOutDay1.Text).Subtract(DateTime.Parse(companytimeout));

                    totalotminuteday1 = Convert.ToInt32(tstotalotminuteday1.TotalMinutes);
                    
                }
                else if (chkDay1CoffeeBreak.Checked == true && chkOTDay1.Checked == false)
                {
                 
                    if (otoutday1 > DateTime.Parse(companytimeout)) 
                    {
                        otoutday1 = DateTime.Parse(companytimeout);   
                    }
   
                    totalminuteday1 = getTotalMinuteCoffeeBreakOT(morninginday1, morningoutday1,
                    afternooninday1, afternoonoutday1, otinday1, otoutday1,
                    deducttimeoverbreak, txtMorningInDay1.Text, txtMorningOutDay1.Text,
                    txtAftInDay1.Text, txtAftOutDay1.Text, txtOTInDay1.Text, txtOTOutDay1.Text,
                    companytimeout);

                    totalotminuteday1 = 0;
                }
                else if (chkDay1CoffeeBreak.Checked == false && chkOTDay1.Checked == true)
                {
                    totalminuteday1 = getTotalMinutes(morninginday1, morningoutday1,
                       afternooninday1, afternoonoutday1, deducttimeoverbreak,
                       txtMorningInDay1.Text, txtMorningOutDay1.Text,
                       txtAftInDay1.Text, txtAftOutDay1.Text);

                    totalotminuteday1 = getOTMinutes(otoutday1, otinday1,afternoonoutday1,
                        chkDay1CoffeeBreak.Checked, chkOTDay1.Checked);
                }
                else if (chkDay1CoffeeBreak.Checked == false && chkOTDay1.Checked == false)
                {
                    totalminuteday1 = getTotalMinutes(morninginday1, morningoutday1,
                       afternooninday1, afternoonoutday1, deducttimeoverbreak,
                       txtMorningInDay1.Text, txtMorningOutDay1.Text,
                       txtAftInDay1.Text, txtAftOutDay1.Text);

                    totalotminuteday1 = 0;
                }

                //get late
                late = getLate(txtMorningInDay1.Text, txtMorningOutDay1.Text,
                        companytimein, morninginday1) + deducttimeoverbreak;
                

                //get undertime
                undertime = getUnderTime(txtMorningInDay1.Text, txtMorningOutDay1.Text,
                txtAftInDay1.Text, txtAftOutDay1.Text, companytimein, companytimeout,
                morninginday1, morningoutday1, afternooninday1, afternoonoutday1,
                chkDay1CoffeeBreak.Checked, chkOTDay1.Checked,
                txtOTInDay1.Text, txtOTOutDay1.Text);



                lvDraft.Items.Clear();
                ListViewItem lvDraftData = new ListViewItem();
                lvDraftData.Text = "0";
                lvDraftData.SubItems.Add("1");
                lvDraftData.SubItems.Add(totalminuteday1.ToString());
                lvDraftData.SubItems.Add(totalotminuteday1.ToString());
                lvDraftData.SubItems.Add(late.ToString());
                lvDraftData.SubItems.Add(undertime.ToString());
                lvDraft.Items.Add(lvDraftData);

                //MessageBox.Show("Undertime: " + undertime);s
                //MessageBox.Show("Late Time: " + late);
                //MessageBox.Show("Days Duty " + daysduty);
                //MessageBox.Show("Total Minutes Day 1: " + totalminuteday1);
                //MessageBox.Show("Total OT Minutes Day 1: " + totalotminuteday1);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Input Day 1");
                //throw;
            }

            #endregion

            #region Day 2 Computation

            //Day 2 Computation
            try
            {

                int totalminuteday2 = 0;
                int totalotminuteday2 = 0;
                int deducttimeoverbreak = 0;

                var morninginday2 = DateTime.Parse(txtMorningInDay2.Text);
                var morningoutday2 = DateTime.Parse(txtMorningOutDay2.Text);

                var afternooninday2 = DateTime.Parse(txtAftInDay2.Text);
                var afternoonoutday2 = DateTime.Parse(txtAftOutDay2.Text);

                var otinday2 = DateTime.Parse(txtOTInDay2.Text);
                var otoutday2 = DateTime.Parse(txtOTOutDay2.Text);

                var ambreaktime = Convert.ToInt32(cmbBreakAMDay2.Text);
                var pmbreaktime = Convert.ToInt32(cmbBreakPMDay2.Text);


                //check if emplyoee is earlier if earlier set time in to company time in
                if (morninginday2 <= DateTime.Parse(companytimein))
                {
                    morninginday2 = DateTime.Parse(lblTimeIn.Text);
                }

                //afternoon out time
                if (afternoonoutday2 >= DateTime.Parse(companytimeout))
                {
                    afternoonoutday2 = DateTime.Parse(lblTimeOut.Text);
                }


                if (cmbBreakAMDay2.Text != "0")
                {
                    //morning break time
                    var ambreaktimets = afternooninday2.Subtract(morningoutday2);
                    var ambreaktimeint = Convert.ToInt32(ambreaktimets.TotalMinutes);

                    if (ambreaktime < ambreaktimeint)
                    {
                        int breaktime = ambreaktimeint - ambreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }


                if (cmbBreakPMDay2.Text != "0")
                {
                    //afternoon break time
                    var pmbreaktimets = otinday2.Subtract(afternoonoutday2);
                    var pmbreaktimeint = Convert.ToInt32(pmbreaktimets.TotalMinutes);

                    if (pmbreaktime < pmbreaktimeint)
                    {
                        int breaktime = pmbreaktimeint - pmbreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }

                //add late if breaktime is over 
                //late = late + deducttime;

                //get total minute of day
                if (chkDay2CoffeeBreak.Checked == true && chkOTDay2.Checked == true)
                {

                    if (otoutday2 > DateTime.Parse(companytimeout))
                    {
                        otoutday2 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday2 = getTotalMinuteCoffeeBreakOT(morninginday2, morningoutday2,
                    afternooninday2, afternoonoutday2, otinday2, otoutday2,
                    deducttimeoverbreak, txtMorningInDay2.Text, txtMorningOutDay2.Text,
                    txtAftInDay2.Text, txtAftOutDay2.Text, txtOTInDay2.Text, txtOTOutDay2.Text,
                    companytimeout);

                    TimeSpan tstotalotminuteday2;
                    tstotalotminuteday2 = DateTime.Parse(txtOTOutDay2.Text).Subtract(DateTime.Parse(companytimeout));

                    totalotminuteday2 = Convert.ToInt32(tstotalotminuteday2.TotalMinutes);

                }
                else if (chkDay2CoffeeBreak.Checked == true && chkOTDay2.Checked == false)
                {

                    if (otoutday2 > DateTime.Parse(companytimeout))
                    {
                        otoutday2 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday2 = getTotalMinuteCoffeeBreakOT(morninginday2, morningoutday2,
                    afternooninday2, afternoonoutday2, otinday2, otoutday2,
                    deducttimeoverbreak, txtMorningInDay2.Text, txtMorningOutDay2.Text,
                    txtAftInDay2.Text, txtAftOutDay2.Text, txtOTInDay2.Text, txtOTOutDay2.Text,
                    companytimeout);

                    totalotminuteday2 = 0;
                }
                else if (chkDay2CoffeeBreak.Checked == false && chkOTDay2.Checked == true)
                {
                    totalminuteday2 = getTotalMinutes(morninginday2, morningoutday2,
                       afternooninday2, afternoonoutday2, deducttimeoverbreak,
                       txtMorningInDay2.Text, txtMorningOutDay2.Text,
                       txtAftInDay2.Text, txtAftOutDay2.Text);

                    totalotminuteday2 = getOTMinutes(otoutday2, otinday2, afternoonoutday2,
                        chkDay2CoffeeBreak.Checked, chkOTDay2.Checked);
                }
                else if (chkDay2CoffeeBreak.Checked == false && chkOTDay2.Checked == false)
                {
                    totalminuteday2 = getTotalMinutes(morninginday2, morningoutday2,
                       afternooninday2, afternoonoutday2, deducttimeoverbreak,
                       txtMorningInDay2.Text, txtMorningOutDay2.Text,
                       txtAftInDay2.Text, txtAftOutDay2.Text);

                    totalotminuteday2 = 0;
                }

                //get late
                late = getLate(txtMorningInDay2.Text, txtMorningOutDay2.Text,
                        companytimein, morninginday2) + deducttimeoverbreak;


                //get undertime
                undertime = getUnderTime(txtMorningInDay2.Text, txtMorningOutDay2.Text,
                txtAftInDay2.Text, txtAftOutDay2.Text, companytimein, companytimeout,
                morninginday2, morningoutday2, afternooninday2, afternoonoutday2,
                chkDay2CoffeeBreak.Checked, chkOTDay2.Checked,
                txtOTInDay2.Text, txtOTOutDay2.Text);



                ListViewItem lvDraftData = new ListViewItem();
                lvDraftData.Text = "1";
                lvDraftData.SubItems.Add("2");
                lvDraftData.SubItems.Add(totalminuteday2.ToString());
                lvDraftData.SubItems.Add(totalotminuteday2.ToString());
                lvDraftData.SubItems.Add(late.ToString());
                lvDraftData.SubItems.Add(undertime.ToString());
                lvDraft.Items.Add(lvDraftData);

                //MessageBox.Show("Undertime: " + undertime);s
                //MessageBox.Show("Late Time: " + late);
                //MessageBox.Show("Days Duty " + daysduty);
                //MessageBox.Show("Total Minutes Day 1: " + totalminuteday1);
                //MessageBox.Show("Total OT Minutes Day 1: " + totalotminuteday1);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Input Day 2");
                //throw;
            }

            #endregion

            #region Day 3 Computation

            //Day 3 Computation
            try
            {

                int totalminuteday3 = 0;
                int totalotminuteday3 = 0;
                int deducttimeoverbreak = 0;

                var morninginday3 = DateTime.Parse(txtMorningInDay3.Text);
                var morningoutday3 = DateTime.Parse(txtMorningOutDay3.Text);

                var afternooninday3 = DateTime.Parse(txtAftInDay3.Text);
                var afternoonoutday3 = DateTime.Parse(txtAftOutDay3.Text);

                var otinday3 = DateTime.Parse(txtOTInDay3.Text);
                var otoutday3 = DateTime.Parse(txtOTOutDay3.Text);

                var ambreaktime = Convert.ToInt32(cmbBreakAMDay3.Text);
                var pmbreaktime = Convert.ToInt32(cmbBreakPMDay3.Text);


                //check if emplyoee is earlier if earlier set time in to company time in
                if (morninginday3 <= DateTime.Parse(companytimein))
                {
                    morninginday3 = DateTime.Parse(lblTimeIn.Text);
                }

                //afternoon out time
                if (afternoonoutday3 >= DateTime.Parse(companytimeout))
                {
                    afternoonoutday3 = DateTime.Parse(lblTimeOut.Text);
                }


                if (cmbBreakAMDay3.Text != "0")
                {
                    //morning break time
                    var ambreaktimets = afternooninday3.Subtract(morningoutday3);
                    var ambreaktimeint = Convert.ToInt32(ambreaktimets.TotalMinutes);

                    if (ambreaktime < ambreaktimeint)
                    {
                        int breaktime = ambreaktimeint - ambreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }


                if (cmbBreakPMDay3.Text != "0")
                {
                    //afternoon break time
                    var pmbreaktimets = otinday3.Subtract(afternoonoutday3);
                    var pmbreaktimeint = Convert.ToInt32(pmbreaktimets.TotalMinutes);

                    if (pmbreaktime < pmbreaktimeint)
                    {
                        int breaktime = pmbreaktimeint - pmbreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }

                //add late if breaktime is over 
                //late = late + deducttime;

                //get total minute of day
                if (chkDay3CoffeeBreak.Checked == true && chkOTDay3.Checked == true)
                {

                    if (otoutday3 > DateTime.Parse(companytimeout))
                    {
                        otoutday3 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday3 = getTotalMinuteCoffeeBreakOT(morninginday3, morningoutday3,
                    afternooninday3, afternoonoutday3, otinday3, otoutday3,
                    deducttimeoverbreak, txtMorningInDay3.Text, txtMorningOutDay3.Text,
                    txtAftInDay3.Text, txtAftOutDay3.Text, txtOTInDay3.Text, txtOTOutDay3.Text,
                    companytimeout);

                    TimeSpan tstotalotminuteday3;
                    tstotalotminuteday3 = DateTime.Parse(txtOTOutDay3.Text).Subtract(DateTime.Parse(companytimeout));

                    totalotminuteday3 = Convert.ToInt32(tstotalotminuteday3.TotalMinutes);

                }
                else if (chkDay3CoffeeBreak.Checked == true && chkOTDay3.Checked == false)
                {

                    if (otoutday3 > DateTime.Parse(companytimeout))
                    {
                        otoutday3 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday3 = getTotalMinuteCoffeeBreakOT(morninginday3, morningoutday3,
                    afternooninday3, afternoonoutday3, otinday3, otoutday3,
                    deducttimeoverbreak, txtMorningInDay3.Text, txtMorningOutDay3.Text,
                    txtAftInDay3.Text, txtAftOutDay3.Text, txtOTInDay3.Text, txtOTOutDay3.Text,
                    companytimeout);

                    totalotminuteday3 = 0;
                }
                else if (chkDay3CoffeeBreak.Checked == false && chkOTDay3.Checked == true)
                {
                    totalminuteday3 = getTotalMinutes(morninginday3, morningoutday3,
                       afternooninday3, afternoonoutday3, deducttimeoverbreak,
                       txtMorningInDay3.Text, txtMorningOutDay3.Text,
                       txtAftInDay3.Text, txtAftOutDay3.Text);

                    totalotminuteday3 = getOTMinutes(otoutday3, otinday3, afternoonoutday3,
                        chkDay3CoffeeBreak.Checked, chkOTDay2.Checked);
                }
                else if (chkDay3CoffeeBreak.Checked == false && chkOTDay3.Checked == false)
                {
                    totalminuteday3 = getTotalMinutes(morninginday3, morningoutday3,
                       afternooninday3, afternoonoutday3, deducttimeoverbreak,
                       txtMorningInDay3.Text, txtMorningOutDay3.Text,
                       txtAftInDay3.Text, txtAftOutDay3.Text);

                    totalotminuteday3 = 0;
                }

                //get late
                late = getLate(txtMorningInDay3.Text, txtMorningOutDay3.Text,
                        companytimein, morninginday3) + deducttimeoverbreak;


                //get undertime
                undertime = getUnderTime(txtMorningInDay3.Text, txtMorningOutDay3.Text,
                txtAftInDay3.Text, txtAftOutDay3.Text, companytimein, companytimeout,
                morninginday3, morningoutday3, afternooninday3, afternoonoutday3,
                chkDay3CoffeeBreak.Checked, chkOTDay3.Checked,
                txtOTInDay3.Text, txtOTOutDay3.Text);


                ListViewItem lvDraftData = new ListViewItem();
                lvDraftData.Text = "2";
                lvDraftData.SubItems.Add("3");
                lvDraftData.SubItems.Add(totalminuteday3.ToString());
                lvDraftData.SubItems.Add(totalotminuteday3.ToString());
                lvDraftData.SubItems.Add(late.ToString());
                lvDraftData.SubItems.Add(undertime.ToString());
                lvDraft.Items.Add(lvDraftData);

                //MessageBox.Show("Undertime: " + undertime);s
                //MessageBox.Show("Late Time: " + late);
                //MessageBox.Show("Days Duty " + daysduty);
                //MessageBox.Show("Total Minutes Day 1: " + totalminuteday1);
                //MessageBox.Show("Total OT Minutes Day 1: " + totalotminuteday1);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Input Day 3");
                //throw;
            }

            #endregion

            #region Day 4 Computation

            //Day 4 Computation
            try
            {

                int totalminuteday4 = 0;
                int totalotminuteday4 = 0;
                int deducttimeoverbreak = 0;

                var morninginday4 = DateTime.Parse(txtMorningInDay4.Text);
                var morningoutday4 = DateTime.Parse(txtMorningOutDay4.Text);

                var afternooninday4 = DateTime.Parse(txtAftInDay4.Text);
                var afternoonoutday4 = DateTime.Parse(txtAftOutDay4.Text);

                var otinday4 = DateTime.Parse(txtOTInDay4.Text);
                var otoutday4 = DateTime.Parse(txtOTOutDay4.Text);

                var ambreaktime = Convert.ToInt32(cmbBreakAMDay4.Text);
                var pmbreaktime = Convert.ToInt32(cmbBreakPMDay4.Text);


                //check if emplyoee is earlier if earlier set time in to company time in
                if (morninginday4 <= DateTime.Parse(companytimein))
                {
                    morninginday4 = DateTime.Parse(lblTimeIn.Text);
                }

                //afternoon out time
                if (afternoonoutday4 >= DateTime.Parse(companytimeout))
                {
                    afternoonoutday4 = DateTime.Parse(lblTimeOut.Text);
                }


                if (cmbBreakAMDay4.Text != "0")
                {
                    //morning break time
                    var ambreaktimets = afternooninday4.Subtract(morningoutday4);
                    var ambreaktimeint = Convert.ToInt32(ambreaktimets.TotalMinutes);

                    if (ambreaktime < ambreaktimeint)
                    {
                        int breaktime = ambreaktimeint - ambreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }


                if (cmbBreakPMDay4.Text != "0")
                {
                    //afternoon break time
                    var pmbreaktimets = otinday4.Subtract(afternoonoutday4);
                    var pmbreaktimeint = Convert.ToInt32(pmbreaktimets.TotalMinutes);

                    if (pmbreaktime < pmbreaktimeint)
                    {
                        int breaktime = pmbreaktimeint - pmbreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }

                //add late if breaktime is over 
                //late = late + deducttime;

                //get total minute of day
                if (chkDay4CoffeeBreak.Checked == true && chkOTDay4.Checked == true)
                {

                    if (otoutday4 > DateTime.Parse(companytimeout))
                    {
                        otoutday4 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday4 = getTotalMinuteCoffeeBreakOT(morninginday4, morningoutday4,
                    afternooninday4, afternoonoutday4, otinday4, otoutday4,
                    deducttimeoverbreak, txtMorningInDay4.Text, txtMorningOutDay4.Text,
                    txtAftInDay4.Text, txtAftOutDay4.Text, txtOTInDay4.Text, txtOTOutDay4.Text,
                    companytimeout);

                    TimeSpan tstotalotminuteday4;
                    tstotalotminuteday4 = DateTime.Parse(txtOTOutDay4.Text).Subtract(DateTime.Parse(companytimeout));

                    totalotminuteday4 = Convert.ToInt32(tstotalotminuteday4.TotalMinutes);

                }
                else if (chkDay4CoffeeBreak.Checked == true && chkOTDay4.Checked == false)
                {

                    if (otoutday4 > DateTime.Parse(companytimeout))
                    {
                        otoutday4 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday4 = getTotalMinuteCoffeeBreakOT(morninginday4, morningoutday4,
                    afternooninday4, afternoonoutday4, otinday4, otoutday4,
                    deducttimeoverbreak, txtMorningInDay4.Text, txtMorningOutDay4.Text,
                    txtAftInDay4.Text, txtAftOutDay4.Text, txtOTInDay4.Text, txtOTOutDay4.Text,
                    companytimeout);

                    totalotminuteday4 = 0;
                }
                else if (chkDay4CoffeeBreak.Checked == false && chkOTDay4.Checked == true)
                {
                    totalminuteday4 = getTotalMinutes(morninginday4, morningoutday4,
                       afternooninday4, afternoonoutday4, deducttimeoverbreak,
                       txtMorningInDay4.Text, txtMorningOutDay4.Text,
                       txtAftInDay4.Text, txtAftOutDay4.Text);

                    totalotminuteday4 = getOTMinutes(otoutday4, otinday4, afternoonoutday4,
                        chkDay4CoffeeBreak.Checked, chkOTDay2.Checked);
                }
                else if (chkDay4CoffeeBreak.Checked == false && chkOTDay4.Checked == false)
                {
                    totalminuteday4 = getTotalMinutes(morninginday4, morningoutday4,
                       afternooninday4, afternoonoutday4, deducttimeoverbreak,
                       txtMorningInDay4.Text, txtMorningOutDay4.Text,
                       txtAftInDay4.Text, txtAftOutDay4.Text);

                    totalotminuteday4 = 0;
                }

                //get late
                late = getLate(txtMorningInDay4.Text, txtMorningOutDay4.Text,
                        companytimein, morninginday4) + deducttimeoverbreak;


                //get undertime
                undertime = getUnderTime(txtMorningInDay4.Text, txtMorningOutDay4.Text,
                txtAftInDay4.Text, txtAftOutDay4.Text, companytimein, companytimeout,
                morninginday4, morningoutday4, afternooninday4, afternoonoutday4,
                chkDay4CoffeeBreak.Checked, chkOTDay4.Checked,
                txtOTInDay4.Text, txtOTOutDay4.Text);


                ListViewItem lvDraftData = new ListViewItem();
                lvDraftData.Text = "3";
                lvDraftData.SubItems.Add("4");
                lvDraftData.SubItems.Add(totalminuteday4.ToString());
                lvDraftData.SubItems.Add(totalotminuteday4.ToString());
                lvDraftData.SubItems.Add(late.ToString());
                lvDraftData.SubItems.Add(undertime.ToString());
                lvDraft.Items.Add(lvDraftData);

                //MessageBox.Show("Undertime: " + undertime);s
                //MessageBox.Show("Late Time: " + late);
                //MessageBox.Show("Days Duty " + daysduty);
                //MessageBox.Show("Total Minutes Day 1: " + totalminuteday1);
                //MessageBox.Show("Total OT Minutes Day 1: " + totalotminuteday1);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Input Day 4");
                //throw;
            }

            #endregion

            #region Day 5 Computation

            //Day 5 Computation
            try
            {

                int totalminuteday5 = 0;
                int totalotminuteday5 = 0;
                int deducttimeoverbreak = 0;

                var morninginday5 = DateTime.Parse(txtMorningInDay5.Text);
                var morningoutday5 = DateTime.Parse(txtMorningOutDay5.Text);

                var afternooninday5 = DateTime.Parse(txtAftInDay5.Text);
                var afternoonoutday5 = DateTime.Parse(txtAftOutDay5.Text);

                var otinday5 = DateTime.Parse(txtOTInDay5.Text);
                var otoutday5 = DateTime.Parse(txtOTOutDay5.Text);

                var ambreaktime = Convert.ToInt32(cmbBreakAMDay5.Text);
                var pmbreaktime = Convert.ToInt32(cmbBreakPMDay5.Text);


                //check if emplyoee is earlier if earlier set time in to company time in
                if (morninginday5 <= DateTime.Parse(companytimein))
                {
                    morninginday5 = DateTime.Parse(lblTimeIn.Text);
                }

                //afternoon out time
                if (afternoonoutday5 >= DateTime.Parse(companytimeout))
                {
                    afternoonoutday5 = DateTime.Parse(lblTimeOut.Text);
                }


                if (cmbBreakAMDay5.Text != "0")
                {
                    //morning break time
                    var ambreaktimets = afternooninday5.Subtract(morningoutday5);
                    var ambreaktimeint = Convert.ToInt32(ambreaktimets.TotalMinutes);

                    if (ambreaktime < ambreaktimeint)
                    {
                        int breaktime = ambreaktimeint - ambreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }


                if (cmbBreakPMDay5.Text != "0")
                {
                    //afternoon break time
                    var pmbreaktimets = otinday5.Subtract(afternoonoutday5);
                    var pmbreaktimeint = Convert.ToInt32(pmbreaktimets.TotalMinutes);

                    if (pmbreaktime < pmbreaktimeint)
                    {
                        int breaktime = pmbreaktimeint - pmbreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }

                //add late if breaktime is over 
                //late = late + deducttime;

                //get total minute of day
                if (chkDay5CoffeeBreak.Checked == true && chkOTDay5.Checked == true)
                {

                    if (otoutday5 > DateTime.Parse(companytimeout))
                    {
                        otoutday5 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday5 = getTotalMinuteCoffeeBreakOT(morninginday5, morningoutday5,
                    afternooninday5, afternoonoutday5, otinday5, otoutday5,
                    deducttimeoverbreak, txtMorningInDay5.Text, txtMorningOutDay5.Text,
                    txtAftInDay5.Text, txtAftOutDay5.Text, txtOTInDay5.Text, txtOTOutDay5.Text,
                    companytimeout);

                    TimeSpan tstotalotminuteday5;
                    tstotalotminuteday5 = DateTime.Parse(txtOTOutDay5.Text).Subtract(DateTime.Parse(companytimeout));

                    totalotminuteday5 = Convert.ToInt32(tstotalotminuteday5.TotalMinutes);

                }
                else if (chkDay5CoffeeBreak.Checked == true && chkOTDay5.Checked == false)
                {

                    if (otoutday5 > DateTime.Parse(companytimeout))
                    {
                        otoutday5 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday5 = getTotalMinuteCoffeeBreakOT(morninginday5, morningoutday5,
                    afternooninday5, afternoonoutday5, otinday5, otoutday5,
                    deducttimeoverbreak, txtMorningInDay5.Text, txtMorningOutDay5.Text,
                    txtAftInDay5.Text, txtAftOutDay5.Text, txtOTInDay5.Text, txtOTOutDay5.Text,
                    companytimeout);

                    totalotminuteday5 = 0;
                }
                else if (chkDay5CoffeeBreak.Checked == false && chkOTDay5.Checked == true)
                {
                    totalminuteday5 = getTotalMinutes(morninginday5, morningoutday5,
                       afternooninday5, afternoonoutday5, deducttimeoverbreak,
                       txtMorningInDay5.Text, txtMorningOutDay5.Text,
                       txtAftInDay5.Text, txtAftOutDay5.Text);

                    totalotminuteday5 = getOTMinutes(otoutday5, otinday5, afternoonoutday5,
                        chkDay5CoffeeBreak.Checked, chkOTDay5.Checked);
                }
                else if (chkDay5CoffeeBreak.Checked == false && chkOTDay5.Checked == false)
                {
                    totalminuteday5 = getTotalMinutes(morninginday5, morningoutday5,
                       afternooninday5, afternoonoutday5, deducttimeoverbreak,
                       txtMorningInDay5.Text, txtMorningOutDay5.Text,
                       txtAftInDay5.Text, txtAftOutDay5.Text);

                    totalotminuteday5 = 0;
                }

                //get late
                late = getLate(txtMorningInDay5.Text, txtMorningOutDay5.Text,
                        companytimein, morninginday5) + deducttimeoverbreak;


                //get undertime
                undertime = getUnderTime(txtMorningInDay5.Text, txtMorningOutDay5.Text,
                txtAftInDay5.Text, txtAftOutDay5.Text, companytimein, companytimeout,
                morninginday5, morningoutday5, afternooninday5, afternoonoutday5,
                chkDay5CoffeeBreak.Checked, chkOTDay5.Checked,
                txtOTInDay5.Text, txtOTOutDay5.Text);


                ListViewItem lvDraftData = new ListViewItem();
                lvDraftData.Text = "4";
                lvDraftData.SubItems.Add("5");
                lvDraftData.SubItems.Add(totalminuteday5.ToString());
                lvDraftData.SubItems.Add(totalotminuteday5.ToString());
                lvDraftData.SubItems.Add(late.ToString());
                lvDraftData.SubItems.Add(undertime.ToString());
                lvDraft.Items.Add(lvDraftData);

                //MessageBox.Show("Undertime: " + undertime);s
                //MessageBox.Show("Late Time: " + late);
                //MessageBox.Show("Days Duty " + daysduty);
                //MessageBox.Show("Total Minutes Day 1: " + totalminuteday1);
                //MessageBox.Show("Total OT Minutes Day 1: " + totalotminuteday1);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Input Day 5");
                //throw;
            }

            #endregion

            #region Day 6 Computation

            //Day 6 Computation
            try
            {

                int totalminuteday6 = 0;
                int totalotminuteday6 = 0;
                int deducttimeoverbreak = 0;

                var morninginday6 = DateTime.Parse(txtMorningInDay6.Text);
                var morningoutday6 = DateTime.Parse(txtMorningOutDay6.Text);

                var afternooninday6 = DateTime.Parse(txtAftInDay6.Text);
                var afternoonoutday6 = DateTime.Parse(txtAftOutDay6.Text);

                var otinday6 = DateTime.Parse(txtOTInDay6.Text);
                var otoutday6 = DateTime.Parse(txtOTOutDay6.Text);

                var ambreaktime = Convert.ToInt32(cmbBreakAMDay6.Text);
                var pmbreaktime = Convert.ToInt32(cmbBreakPMDay6.Text);


                //check if emplyoee is earlier if earlier set time in to company time in
                if (morninginday6 <= DateTime.Parse(companytimein))
                {
                    morninginday6 = DateTime.Parse(lblTimeIn.Text);
                }

                //afternoon out time
                if (afternoonoutday6 >= DateTime.Parse(companytimeout))
                {
                    afternoonoutday6 = DateTime.Parse(lblTimeOut.Text);
                }


                if (cmbBreakAMDay6.Text != "0")
                {
                    //morning break time
                    var ambreaktimets = afternooninday6.Subtract(morningoutday6);
                    var ambreaktimeint = Convert.ToInt32(ambreaktimets.TotalMinutes);

                    if (ambreaktime < ambreaktimeint)
                    {
                        int breaktime = ambreaktimeint - ambreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }


                if (cmbBreakPMDay6.Text != "0")
                {
                    //afternoon break time
                    var pmbreaktimets = otinday6.Subtract(afternoonoutday6);
                    var pmbreaktimeint = Convert.ToInt32(pmbreaktimets.TotalMinutes);

                    if (pmbreaktime < pmbreaktimeint)
                    {
                        int breaktime = pmbreaktimeint - pmbreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }

                //add late if breaktime is over 
                //late = late + deducttime;

                //get total minute of day
                if (chkDay6CoffeeBreak.Checked == true && chkOTDay6.Checked == true)
                {

                    if (otoutday6 > DateTime.Parse(companytimeout))
                    {
                        otoutday6 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday6 = getTotalMinuteCoffeeBreakOT(morninginday6, morningoutday6,
                    afternooninday6, afternoonoutday6, otinday6, otoutday6,
                    deducttimeoverbreak, txtMorningInDay6.Text, txtMorningOutDay6.Text,
                    txtAftInDay6.Text, txtAftOutDay6.Text, txtOTInDay6.Text, txtOTOutDay6.Text,
                    companytimeout);

                    TimeSpan tstotalotminuteday6;
                    tstotalotminuteday6 = DateTime.Parse(txtOTOutDay6.Text).Subtract(DateTime.Parse(companytimeout));

                    totalotminuteday6 = Convert.ToInt32(tstotalotminuteday6.TotalMinutes);

                }
                else if (chkDay6CoffeeBreak.Checked == true && chkOTDay6.Checked == false)
                {

                    if (otoutday6 > DateTime.Parse(companytimeout))
                    {
                        otoutday6 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday6 = getTotalMinuteCoffeeBreakOT(morninginday6, morningoutday6,
                    afternooninday6, afternoonoutday6, otinday6, otoutday6,
                    deducttimeoverbreak, txtMorningInDay6.Text, txtMorningOutDay6.Text,
                    txtAftInDay6.Text, txtAftOutDay6.Text, txtOTInDay6.Text, txtOTOutDay6.Text,
                    companytimeout);

                    totalotminuteday6 = 0;
                }
                else if (chkDay6CoffeeBreak.Checked == false && chkOTDay6.Checked == true)
                {
                    totalminuteday6 = getTotalMinutes(morninginday6, morningoutday6,
                       afternooninday6, afternoonoutday6, deducttimeoverbreak,
                       txtMorningInDay6.Text, txtMorningOutDay6.Text,
                       txtAftInDay6.Text, txtAftOutDay6.Text);

                    totalotminuteday6 = getOTMinutes(otoutday6, otinday6, afternoonoutday6,
                        chkDay6CoffeeBreak.Checked, chkOTDay6.Checked);
                }
                else if (chkDay6CoffeeBreak.Checked == false && chkOTDay6.Checked == false)
                {
                    totalminuteday6 = getTotalMinutes(morninginday6, morningoutday6,
                       afternooninday6, afternoonoutday6, deducttimeoverbreak,
                       txtMorningInDay6.Text, txtMorningOutDay6.Text,
                       txtAftInDay6.Text, txtAftOutDay6.Text);

                    totalotminuteday6 = 0;
                }

                //get late
                late = getLate(txtMorningInDay6.Text, txtMorningOutDay6.Text,
                        companytimein, morninginday6) + deducttimeoverbreak;


                //get undertime
                undertime = getUnderTime(txtMorningInDay6.Text, txtMorningOutDay6.Text,
                txtAftInDay6.Text, txtAftOutDay6.Text, companytimein, companytimeout,
                morninginday6, morningoutday6, afternooninday6, afternoonoutday6,
                chkDay6CoffeeBreak.Checked, chkOTDay6.Checked,
                txtOTInDay6.Text, txtOTOutDay6.Text);


                ListViewItem lvDraftData = new ListViewItem();
                lvDraftData.Text = "5";
                lvDraftData.SubItems.Add("6");
                lvDraftData.SubItems.Add(totalminuteday6.ToString());
                lvDraftData.SubItems.Add(totalotminuteday6.ToString());
                lvDraftData.SubItems.Add(late.ToString());
                lvDraftData.SubItems.Add(undertime.ToString());
                lvDraft.Items.Add(lvDraftData);

                //MessageBox.Show("Undertime: " + undertime);s
                //MessageBox.Show("Late Time: " + late);
                //MessageBox.Show("Days Duty " + daysduty);
                //MessageBox.Show("Total Minutes Day 1: " + totalminuteday1);
                //MessageBox.Show("Total OT Minutes Day 1: " + totalotminuteday1);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Input Day 6");
                //throw;
            }

            #endregion

            #region Day 7 Computation

            //Day 7 Computation
            try
            {

                int totalminuteday7 = 0;
                int totalotminuteday7 = 0;
                int deducttimeoverbreak = 0;

                var morninginday7 = DateTime.Parse(txtMorningInDay7.Text);
                var morningoutday7 = DateTime.Parse(txtMorningOutDay7.Text);

                var afternooninday7 = DateTime.Parse(txtAftInDay7.Text);
                var afternoonoutday7 = DateTime.Parse(txtAftOutDay7.Text);

                var otinday7 = DateTime.Parse(txtOTInDay7.Text);
                var otoutday7 = DateTime.Parse(txtOTOutDay7.Text);

                var ambreaktime = Convert.ToInt32(cmbBreakAMDay7.Text);
                var pmbreaktime = Convert.ToInt32(cmbBreakPMDay7.Text);


                //check if emplyoee is earlier if earlier set time in to company time in
                if (morninginday7 <= DateTime.Parse(companytimein))
                {
                    morninginday7 = DateTime.Parse(lblTimeIn.Text);
                }

                //afternoon out time
                if (afternoonoutday7 >= DateTime.Parse(companytimeout))
                {
                    afternoonoutday7 = DateTime.Parse(lblTimeOut.Text);
                }


                if (cmbBreakAMDay7.Text != "0")
                {
                    //morning break time
                    var ambreaktimets = afternooninday7.Subtract(morningoutday7);
                    var ambreaktimeint = Convert.ToInt32(ambreaktimets.TotalMinutes);

                    if (ambreaktime < ambreaktimeint)
                    {
                        int breaktime = ambreaktimeint - ambreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }


                if (cmbBreakPMDay7.Text != "0")
                {
                    //afternoon break time
                    var pmbreaktimets = otinday7.Subtract(afternoonoutday7);
                    var pmbreaktimeint = Convert.ToInt32(pmbreaktimets.TotalMinutes);

                    if (pmbreaktime < pmbreaktimeint)
                    {
                        int breaktime = pmbreaktimeint - pmbreaktime;
                        deducttimeoverbreak = deducttimeoverbreak + breaktime;
                    }
                }

                //add late if breaktime is over 
                //late = late + deducttime;

                //get total minute of day
                if (chkDay7CoffeeBreak.Checked == true && chkOTDay7.Checked == true)
                {

                    if (otoutday7 > DateTime.Parse(companytimeout))
                    {
                        otoutday7 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday7 = getTotalMinuteCoffeeBreakOT(morninginday7, morningoutday7,
                    afternooninday7, afternoonoutday7, otinday7, otoutday7,
                    deducttimeoverbreak, txtMorningInDay7.Text, txtMorningOutDay7.Text,
                    txtAftInDay7.Text, txtAftOutDay7.Text, txtOTInDay7.Text, txtOTOutDay7.Text,
                    companytimeout);

                    TimeSpan tstotalotminuteday7;
                    tstotalotminuteday7 = DateTime.Parse(txtOTOutDay7.Text).Subtract(DateTime.Parse(companytimeout));

                    totalotminuteday7 = Convert.ToInt32(tstotalotminuteday7.TotalMinutes);

                }
                else if (chkDay7CoffeeBreak.Checked == true && chkOTDay7.Checked == false)
                {

                    if (otoutday7 > DateTime.Parse(companytimeout))
                    {
                        otoutday7 = DateTime.Parse(companytimeout);
                    }

                    totalminuteday7 = getTotalMinuteCoffeeBreakOT(morninginday7, morningoutday7,
                    afternooninday7, afternoonoutday7, otinday7, otoutday7,
                    deducttimeoverbreak, txtMorningInDay7.Text, txtMorningOutDay7.Text,
                    txtAftInDay7.Text, txtAftOutDay7.Text, txtOTInDay7.Text, txtOTOutDay7.Text,
                    companytimeout);

                    totalotminuteday7 = 0;
                }
                else if (chkDay7CoffeeBreak.Checked == false && chkOTDay7.Checked == true)
                {
                    totalminuteday7 = getTotalMinutes(morninginday7, morningoutday7,
                       afternooninday7, afternoonoutday7, deducttimeoverbreak,
                       txtMorningInDay7.Text, txtMorningOutDay7.Text,
                       txtAftInDay7.Text, txtAftOutDay7.Text);

                    totalotminuteday7 = getOTMinutes(otoutday7, otinday7, afternoonoutday7,
                        chkDay7CoffeeBreak.Checked, chkOTDay7.Checked);
                }
                else if (chkDay7CoffeeBreak.Checked == false && chkOTDay7.Checked == false)
                {
                    totalminuteday7 = getTotalMinutes(morninginday7, morningoutday7,
                       afternooninday7, afternoonoutday7, deducttimeoverbreak,
                       txtMorningInDay7.Text, txtMorningOutDay7.Text,
                       txtAftInDay7.Text, txtAftOutDay7.Text);

                    totalotminuteday7 = 0;
                }

                //get late
                late = getLate(txtMorningInDay7.Text, txtMorningOutDay7.Text,
                        companytimein, morninginday7) + deducttimeoverbreak;


                //get undertime
                undertime = getUnderTime(txtMorningInDay7.Text, txtMorningOutDay7.Text,
                txtAftInDay7.Text, txtAftOutDay7.Text, companytimein, companytimeout,
                morninginday7, morningoutday7, afternooninday7, afternoonoutday7,
                chkDay7CoffeeBreak.Checked, chkOTDay7.Checked,
                txtOTInDay7.Text, txtOTOutDay7.Text);


                ListViewItem lvDraftData = new ListViewItem();
                lvDraftData.Text = "6";
                lvDraftData.SubItems.Add("7");
                lvDraftData.SubItems.Add(totalminuteday7.ToString());
                lvDraftData.SubItems.Add(totalotminuteday7.ToString());
                lvDraftData.SubItems.Add(late.ToString());
                lvDraftData.SubItems.Add(undertime.ToString());
                lvDraft.Items.Add(lvDraftData);

                //MessageBox.Show("Undertime: " + undertime);s
                //MessageBox.Show("Late Time: " + late);
                //MessageBox.Show("Days Duty " + daysduty);
                //MessageBox.Show("Total Minutes Day 1: " + totalminuteday1);
                //MessageBox.Show("Total OT Minutes Day 1: " + totalotminuteday1);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Input Day 7");
                //throw;
            }

            #endregion


        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtName;


            //set combobox selected index
            cmbBreakAMDay1.SelectedIndex = 0;
            cmbBreakPMDay1.SelectedIndex = 0;

            cmbBreakAMDay2.SelectedIndex = 0;
            cmbBreakPMDay2.SelectedIndex = 0;

            cmbBreakAMDay3.SelectedIndex = 0;
            cmbBreakPMDay3.SelectedIndex = 0;

            cmbBreakAMDay4.SelectedIndex = 0;
            cmbBreakPMDay4.SelectedIndex = 0;

            cmbBreakAMDay5.SelectedIndex = 0;
            cmbBreakPMDay5.SelectedIndex = 0;

            cmbBreakAMDay6.SelectedIndex = 0;
            cmbBreakPMDay6.SelectedIndex = 0;

            cmbBreakAMDay7.SelectedIndex = 0;
            cmbBreakPMDay7.SelectedIndex = 0;

        }



        private int getTotalMinutes(DateTime morninginday,DateTime morningoutday,
            DateTime aftinday,DateTime aftoutday, int deducttime,
            string txtMorningInDay, string txtMorningOutDay,
            string txtAftInDay, string txtAftOutDay)
        {
            //get total minute of day
            TimeSpan tstotalminuteday;
            int totalminuteday = 0;

            if ((txtMorningInDay == "00:00" || txtMorningOutDay == "00:00") &&
                (txtAftInDay != "00:00" && txtAftOutDay != "00:00") ) 
            {
                tstotalminuteday = aftoutday.Subtract(aftinday);
                totalminuteday = Convert.ToInt32(tstotalminuteday.TotalMinutes) - deducttime;
            }
            else if ((txtMorningInDay != "00:00" && txtMorningOutDay != "00:00") &&
               (txtAftInDay == "00:00" || txtAftOutDay == "00:00")) 
            {
                tstotalminuteday = morningoutday.Subtract(morninginday);
                totalminuteday = Convert.ToInt32(tstotalminuteday.TotalMinutes) - deducttime;
            }else if ((txtMorningInDay != "00:00" && txtMorningOutDay == "00:00") &&
               (txtAftInDay == "00:00" && txtAftOutDay != "00:00") )
            {
                tstotalminuteday = aftoutday.Subtract(morninginday);
                totalminuteday = Convert.ToInt32(tstotalminuteday.TotalMinutes) - deducttime;
            }
            else if ((txtMorningInDay != "00:00" && txtMorningOutDay != "00:00") &&
              (txtAftInDay != "00:00" && txtAftOutDay != "00:00")) 
            {
                tstotalminuteday = aftoutday.Subtract(morninginday);
                totalminuteday = Convert.ToInt32(tstotalminuteday.TotalMinutes) - deducttime;
            }

            return totalminuteday;


        }


        private int getOTMinutes(DateTime otoutday,DateTime otinday,DateTime afternoonout,
            Boolean coffeebreak,Boolean overtime)
        {
            TimeSpan tstotalotminuteday;
            int totalotminuteday =0;

            if (coffeebreak == true && overtime == true)
            {
                tstotalotminuteday = otoutday.Subtract(afternoonout);
                totalotminuteday = Convert.ToInt32(tstotalotminuteday.TotalMinutes);
            }
            else if (coffeebreak == false && overtime == true)
            {
                tstotalotminuteday = otoutday.Subtract(otinday);
                totalotminuteday = Convert.ToInt32(tstotalotminuteday.TotalMinutes);
            }

            

            return totalotminuteday;
        }

        private int getLate(string txtMorningIn, string txtMorningOut, string companytimein,
            DateTime morningin)
        {
            TimeSpan tstotallate;
            int late = 0;

            if (txtMorningIn != "00:00")
            {
                tstotallate = morningin.Subtract(DateTime.Parse(companytimein));
                late = late + Convert.ToInt32(tstotallate.TotalMinutes);
            }
      

            return late;
        }

        private int getUnderTime(string txtMorningIn, string txtMorningOut,
            string txtAfternoonIn, string txtAfternoonOut, 
            string companytimein, string companytimeout,
            DateTime morningin, DateTime morningout,
            DateTime afternoonin, DateTime afternoonout,
            Boolean coffeebreak, Boolean overtime,
            string txtOtIn, string txtOtOut)
        {

            int undertime = 0 ;
            TimeSpan tstotalundertimeday;

            if (coffeebreak == true)
            {
                if (DateTime.Parse(txtOtOut) > DateTime.Parse(companytimeout))
                {
                    undertime = 0;
                }
                else    
                {
                    tstotalundertimeday = DateTime.Parse(companytimeout).Subtract(DateTime.Parse(txtOtOut));
                    undertime = Convert.ToInt32(tstotalundertimeday.TotalMinutes);
                }
  
            }
            else { 
            
                if ((txtMorningOut != "00:00" && txtMorningIn != "00:00") && 
                    (txtAfternoonIn == "00:00" || txtAfternoonOut == "00:00" ))
                {
                    tstotalundertimeday = DateTime.Parse(companytimeout).Subtract(morningout);
                    undertime =  Convert.ToInt32(tstotalundertimeday.TotalMinutes);
                }
                else if ((txtMorningOut == "00:00" || txtMorningIn == "00:00") &&
                   (txtAfternoonIn != "00:00" && txtAfternoonOut != "00:00")) 
                {
                    tstotalundertimeday = DateTime.Parse(companytimein).Subtract(afternoonin);
                    undertime = Convert.ToInt32(tstotalundertimeday.TotalMinutes) * -1;
                }
                else if ((txtMorningOut != "00:00" && txtMorningIn != "00:00") &&
                  (txtAfternoonIn != "00:00" && txtAfternoonOut != "00:00")) 
                {
                    tstotalundertimeday = DateTime.Parse(companytimeout).Subtract(afternoonout);
                    undertime = Convert.ToInt32(tstotalundertimeday.TotalMinutes);
                }
                else if ((txtMorningIn != "00:00" && txtMorningOut== "00:00") &&
                 (txtAfternoonIn == "00:00" && txtAfternoonOut != "00:00")) 
                {
                    tstotalundertimeday = DateTime.Parse(companytimeout).Subtract(afternoonout);
                    undertime = Convert.ToInt32(tstotalundertimeday.TotalMinutes);
                }

            }


            return undertime;
        }


        private int getBreakTime(string txtMorningIn, string txtMorningOut, string txtAftIn, string txtAftOut,
            DateTime morningout,DateTime aftin)
        {
            TimeSpan tsbreaktime;
            int deducttime = 0;

            if ((txtMorningOut != "00:00" && txtAftIn != "00:00") && (txtMorningIn != "00:00" && txtAftOut != "00:00"))
            {
                tsbreaktime = aftin.Subtract(morningout);

                //set deduct time
                if (Convert.ToInt32(tsbreaktime.TotalMinutes) > 90)
                {
                    deducttime = Convert.ToInt32(tsbreaktime.TotalMinutes) - 90;
                }
            }
            else
            {
                deducttime = 0;
            }

            return deducttime;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            lblName.Text = txtName.Text;
            lblOutlet.Text = txtOutlet.Text;
        }

        private void btnComputeOverall_Click(object sender, EventArgs e)
        {
            int days = 0;
            int totalminutes = 0;
            int overtime = 0;
            int late = 0;
            int undertime = 0;

            foreach (ListViewItem item in lvDraft.Items) 
            {   
                if (item.SubItems[2].Text != "0") 
                {
                    days = days + 1;
                }


                totalminutes = totalminutes + Convert.ToInt32(item.SubItems[2].Text);
                overtime = overtime + Convert.ToInt32(item.SubItems[3].Text);
                late = late + Convert.ToInt32(item.SubItems[4].Text);
                undertime = undertime + Convert.ToInt32(item.SubItems[5].Text);


            }


            ListViewItem lvSummaryData = new ListViewItem();


            lvSummaryData.Text = lblOutlet.Text + " - " + lblName.Text;
            lvSummaryData.SubItems.Add(days.ToString());
            lvSummaryData.SubItems.Add(totalminutes.ToString());
            lvSummaryData.SubItems.Add(overtime.ToString());
            lvSummaryData.SubItems.Add(late.ToString());
            lvSummaryData.SubItems.Add(undertime.ToString());
            lvSummary.Items.Add(lvSummaryData);


        }

        private void btnClearLVSummary_Click(object sender, EventArgs e)
        {
            lvSummary.Items.Clear();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            txtOutlet.Text = "";
            lblTimeIn.Text = "";
            lblTimeOut.Text = "";

            clearHoursTextBox();

            //lvSummary.Items.Clear();
            lvDraft.Items.Clear();

            this.ActiveControl = txtName;

        }

        private void ClearTextBoxes()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
        }

        private void btnClearDraft_Click(object sender, EventArgs e)
        {
            lvDraft.Items.Clear();
        }

        private void btnResetCompute_Click(object sender, EventArgs e)
        {
            clearHoursTextBox();
        }


        private void clearHoursTextBox()
        {
            txtMorningInDay1.Text = "00:00";
            txtMorningInDay2.Text = "00:00";
            txtMorningInDay3.Text = "00:00";
            txtMorningInDay4.Text = "00:00";
            txtMorningInDay5.Text = "00:00";
            txtMorningInDay6.Text = "00:00";
            txtMorningInDay7.Text = "00:00";

            txtMorningOutDay1.Text = "00:00";
            txtMorningOutDay2.Text = "00:00";
            txtMorningOutDay3.Text = "00:00";
            txtMorningOutDay4.Text = "00:00";
            txtMorningOutDay5.Text = "00:00";
            txtMorningOutDay6.Text = "00:00";
            txtMorningOutDay7.Text = "00:00";

            txtAftInDay1.Text = "00:00";
            txtAftInDay2.Text = "00:00";
            txtAftInDay3.Text = "00:00";
            txtAftInDay4.Text = "00:00";
            txtAftInDay5.Text = "00:00";
            txtAftInDay6.Text = "00:00";
            txtAftInDay7.Text = "00:00";

            txtAftOutDay1.Text = "00:00";
            txtAftOutDay2.Text = "00:00";
            txtAftOutDay3.Text = "00:00";
            txtAftOutDay4.Text = "00:00";
            txtAftOutDay5.Text = "00:00";
            txtAftOutDay6.Text = "00:00";
            txtAftOutDay7.Text = "00:00";

            txtOTInDay1.Text = "00:00";
            txtOTInDay2.Text = "00:00";
            txtOTInDay3.Text = "00:00";
            txtOTInDay4.Text = "00:00";
            txtOTInDay5.Text = "00:00";
            txtOTInDay6.Text = "00:00";
            txtOTInDay7.Text = "00:00";

            txtOTOutDay1.Text = "00:00";
            txtOTOutDay2.Text = "00:00";
            txtOTOutDay3.Text = "00:00";
            txtOTOutDay4.Text = "00:00";
            txtOTOutDay5.Text = "00:00";
            txtOTOutDay6.Text = "00:00";
            txtOTOutDay7.Text = "00:00";

            cmbBreakAMDay1.SelectedIndex = 0;
            cmbBreakPMDay1.SelectedIndex = 0;

            cmbBreakAMDay2.SelectedIndex = 0;
            cmbBreakPMDay2.SelectedIndex = 0;

            cmbBreakAMDay3.SelectedIndex = 0;
            cmbBreakPMDay3.SelectedIndex = 0;

            cmbBreakAMDay4.SelectedIndex = 0;
            cmbBreakPMDay4.SelectedIndex = 0;

            cmbBreakAMDay5.SelectedIndex = 0;
            cmbBreakPMDay5.SelectedIndex = 0;

            cmbBreakAMDay6.SelectedIndex = 0;
            cmbBreakPMDay6.SelectedIndex = 0;

            cmbBreakAMDay7.SelectedIndex = 0;
            cmbBreakPMDay7.SelectedIndex = 0;

            chkDay1CoffeeBreak.Checked = false;
            chkDay2CoffeeBreak.Checked = false;
            chkDay3CoffeeBreak.Checked = false;
            chkDay4CoffeeBreak.Checked = false;
            chkDay5CoffeeBreak.Checked = false;
            chkDay6CoffeeBreak.Checked = false;
            chkDay7CoffeeBreak.Checked = false;

            chkOTDay1.Checked = false;
            chkOTDay2.Checked = false;
            chkOTDay3.Checked = false;
            chkOTDay4.Checked = false;
            chkOTDay5.Checked = false;
            chkOTDay6.Checked = false;
            chkOTDay7.Checked = false;


        }


        private void lvSummary_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Delete == e.KeyCode)
            {
                foreach (ListViewItem eachItem in lvSummary.SelectedItems)
                {
                    lvSummary.Items.Remove(eachItem);
                }
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyListBox(lvSummary);
            }


         
        }

        public void CopyListBox(ListView list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in list.SelectedItems)
            {
                ListViewItem l = item as ListViewItem;
                if (l != null)
                    foreach (ListViewItem.ListViewSubItem sub in l.SubItems)
                        sb.Append(sub.Text + "\t");
                sb.AppendLine();
            }
            Clipboard.SetDataObject(sb.ToString());

        }


        public int getTotalMinuteCoffeeBreakOT(DateTime morninginday, DateTime morningoutday,
            DateTime aftinday, DateTime aftoutday, DateTime otinday, DateTime otoutday,
            int deducttime,
            string txtMorningInDay, string txtMorningOutDay,
            string txtAftInDay, string txtAftOutDay,
            string txtOTInDay, string txtOTOutDay,
            string companytimeout)
        {

            //get total minute of day
            TimeSpan tstotalminuteday;
            
            int totalminuteday = 0;
           
            if ((txtMorningInDay != "00:00" || txtMorningOutDay != "00:00") &&
                (txtAftInDay != "00:00" && txtAftOutDay != "00:00") && 
                (txtOTInDay != "00:00" && txtOTOutDay != "00:00"))
            {

                tstotalminuteday = otoutday.Subtract(morninginday);
                totalminuteday = Convert.ToInt32(tstotalminuteday.TotalMinutes) - deducttime;
               
            }

           
            return totalminuteday;
        }

        

        private void lvSummary_SelectedIndexChanged(object sender, EventArgs e)
        {
                
        }


        
    }
}
