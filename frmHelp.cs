using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pShell
{
    public partial class frmHelp : Form
    {
        public frmHelp()
        {
            InitializeComponent();
        }

        private void frmHelp_Load(object sender, EventArgs e)
        {
            string txt = "This is a customized utility has been created using C# and intended to run on Microsft Windows OS" + Environment.NewLine + Environment.NewLine;
            txt = txt + "The utility uses a specially formated CSV file having following three columns" + Environment.NewLine + Environment.NewLine;
            txt = txt + "COMPANY|HOST|IPADDRESS" + Environment.NewLine + Environment.NewLine;
            txt = txt + "In the above file first line/row contains columns headers which are not included while creating PowerShell Commands." + Environment.NewLine + Environment.NewLine;
            txt = txt + "After Opening file, the CSV file will be read and the grid will get populated and you will see all three columns. " + Environment.NewLine + Environment.NewLine;
            txt = txt + "If you feel satisifed with information in the grid, click Create Commands. This will populate the list and create list of required commands which can be saved from the menu as text file." + Environment.NewLine + Environment.NewLine;
            //txt = txt + "After thoroughly checking the created list of commands, you can click Run PowerShell Commands to proceed. This will run all the commands with a top to bottom approach, one by one and show the progress bar. A message will appear that the work has been completed and the list at the bottom right will get populated, showing results. This list or processing progress can also be saved from the menu as text file for convenience and analyizing errors." + Environment.NewLine + Environment.NewLine;
            //txt = txt + "Note: Running PowerShell Commands is a sensitive issue. Please be careful and use only if you know, what you are doing. The developer of this software shall not be responsible for any wrongful use of the provided utility." + Environment.NewLine + Environment.NewLine;
            txt = txt + "Regards" + Environment.NewLine;
            txt = txt + "Khalid M." + Environment.NewLine;
            textBox1.Text = txt;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
