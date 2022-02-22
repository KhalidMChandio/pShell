using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace pShell
{
    public partial class frmMain : Form
    {
        private ListBox lstP = new ListBox();
        public frmMain()
        {
            InitializeComponent();
        }
        //Open file dialog to load CSV File
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            openFileDialog.ShowDialog();

            txtFileName.Text = openFileDialog.FileName;
            if (openFileDialog.FileName != "")
            {
                //load file into the Grid
                DataGridView dg = new DataGridView();
                dg.DataSource = openFileDialog.FileName;
                BindData(txtFileName.Text);
            }
        }
        //procedure to find data from CSV file to a DataGridView
        private void BindData(string filePath)
        {
            DataTable dt = new DataTable();
            string[] lines = System.IO.File.ReadAllLines(filePath);
            if (lines.Length > 0)
            {
                //first line to create header
                string firstLine = lines[0];
                string[] headerLabels = firstLine.Split(',');
                //take care of headers that are included in the file
                foreach (string headerWord in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }
                //Iterate through CSV file's lines one by one
                for (int i = 1; i < lines.Length; i++)
                {
                    //split on comma
                    string[] dataWords = lines[i].Split(',');
                    DataRow dr = dt.NewRow();
                    int columnIndex = 0;
                    foreach (string headerWord in headerLabels)
                    {
                        dr[headerWord] = dataWords[columnIndex++];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
            }
        }
        //Create Commands and aadd to listbox
        private void button2_Click(object sender, EventArgs e)
        {
            //If grid is not empty
            if (dataGridView1.Rows.Count > 0)
            {
                //iterate through grid rows
                foreach (DataGridViewRow dr in dataGridView1.Rows)
                {
                    //Create commands against each row of datagridview
                    lstCommands.Items.Add("config firewall address");
                    lstCommands.Items.Add("edit " + dr.Cells[0].Value.ToString() + "." + dr.Cells[1].Value.ToString());
                    lstCommands.Items.Add("set subnet " + dr.Cells[2].Value.ToString() + " 255.255.255.255");
                    lstCommands.Items.Add("end");
                    lstCommands.Items.Add("");
                    lstCommands.Items.Add("config firewall addrgrp");
                    lstCommands.Items.Add("edit PermittedSubnets");
                    lstCommands.Items.Add("append member " + dr.Cells[0].Value.ToString() + "." + dr.Cells[1].Value.ToString());
                    lstCommands.Items.Add("end");
                    lstCommands.Items.Add("");
                }
            }
            //if grid is empty
            else
            {
                MessageBox.Show("Please Open file to proceed.");
            }
        }
        //Close button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Rum Commands in PowerShell ------------------------- Not required by the Client
        private void button3_Click(object sender, EventArgs e)
        {
            RunPSCommands();
        }
        //button code to clear all
        private void button5_Click(object sender, EventArgs e)
        {
            txtFileName.Text = "";
            dataGridView1.DataSource = null;
            lstCommands.Items.Clear();
            lstProgress.Items.Clear();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Load event should empty
        }
        private void RunPSCommands()
        {
            //progressBar1.Maximum = lstCommands.Items.Count;
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            backgroundWorker1.RunWorkerAsync();
        }
        // backgroundworker to run PS Commands behind the Screen --------------- Not required by Client
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //Create Powershell Object
            using (PowerShell powerShell = PowerShell.Create())
            {
                int iterationCounter = 0;
                //Iterate through list of commands
                foreach (String lstItem in lstCommands.Items)
                {
                    if (lstItem.Trim().Length > 0)
                    {
                        // Source function to add commands
                        powerShell.AddScript("python --version");
                        lstP.Items.Add(lstItem.Trim());
                        // invoke execution on the pipeline (collecting output)
                        Collection<PSObject> PSOutput = powerShell.Invoke();
                        Collection<ErrorRecord> Errors = powerShell.Streams.Error.ReadAll();
                        // loop through each output object item
                        foreach (PSObject outputItem in PSOutput)
                        {
                            // if null object was dumped to the pipeline during the script then a null object may be present here
                            if (outputItem != null)
                            {
                                lstP.Items.Add($"Output line: [{outputItem.ToString()}]");
                            }
                            else if (outputItem is null)
                            {
                                lstP.Items.Add($"Done...");
                            }
                        }

                        // check the other output streams (for example, the error stream)
                        //if (powerShell.Streams.Error.Count > 0)
                        {
                            foreach (ErrorRecord er in Errors)
                            {
                                MessageBox.Show($"Error line: [{ er.ToString()}]");
                                // error records were written to the error stream.
                                lstP.Items.Add($"Error line: [{ er.ToString()}]");
                            }
                        }
                    }
                    iterationCounter += 1;
                    backgroundWorker1.ReportProgress((iterationCounter * 100)/lstCommands.Items.Count);
                }
            }
        }
        // Shows progress on Progress Bar while running PS Commands ------------- Not required by the client
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            for (int i=0;i<lstP.Items.Count-1;i++)
            {
                lstProgress.Items.Add(lstP.Items[i]);
            }
            MessageBox.Show("Task Completed");
        }

        private void saveCommandsCreatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 1)
            {
                if (lstCommands.Items.Count > 1)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Text files (*.txt)|*.txt";
                    saveFileDialog.ShowDialog();
                    if (saveFileDialog.FileName != "")
                    {
                        using (TextWriter tw = new StreamWriter(saveFileDialog.FileName))
                        {
                            foreach (String s in lstCommands.Items)
                                tw.WriteLine(s);
                            tw.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Commands List is empty, please click create commands button to create commands list.");
                }
            }
            else
            {
                MessageBox.Show("Grid is empty, please open CSV file, create commands and then proceed to save.");
            }
        }
        // Save Powershell Progress if desired -------------------------- Not required by the client
        private void savePowershellProgressReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstProgress.Items.Count > 1)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text files (*.txt)|*.txt";
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    using (TextWriter tw = new StreamWriter(saveFileDialog.FileName))
                    {
                        foreach (String s in lstProgress.Items)
                            tw.WriteLine(s);
                        tw.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Commands List is empty, please click create commands button to create commands list.");
            }
        }
        //Exit from toolstrip menu
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Show about box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frm = new frmAbout();
            frm.Show();
        }
        //show help & Info
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmHelp frm = new frmHelp();
            frm.Show();
        }
    }
}
