using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ExcelDataReader.Core;

namespace ExcelParserApp
{
    public partial class ExcelParserForm : Form
    {
        private string selectedFilePath;
        public ExcelParserForm()
        {
            InitializeComponent();            
        }

        private async void startParsingButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Could not load the selected file", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            startParsingButton.Text = "Parsing data...";
            startParsingButton.Enabled = false;
            browseFileButton.Enabled = false;
            List<PatientSurveyDetail> surveyDetails = new List<PatientSurveyDetail>();
            try
            {
                string filePath = selectedFilePath;
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                    {
                        var conf = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        };
                        var dataSet = reader.AsDataSet(conf);
                        if (dataSet != null)
                        {
                            if (dataSet.Tables != null)
                            {
                                var dataTable = dataSet.Tables[0];

                                for (int i = 0; i < dataTable.Rows.Count; i++)
                                {
                                    PatientSurveyDetail detail = new PatientSurveyDetail();
                                    detail.PatientSurveyDetailId = Guid.NewGuid();
                                    detail.PatientName = dataTable.Rows[i][0].ToString();
                                    detail.MRN = dataTable.Rows[i][1].ToString();
                                    detail.MRN = detail.MRN.TrimStart('0');
                                    detail.PatientAge = int.Parse(dataTable.Rows[i][2].ToString());
                                    detail.ProfessionalName = dataTable.Rows[i][3].ToString();
                                    detail.SurveyDate = DateTime.Parse(dataTable.Rows[i][4].ToString());
                                    detail.SurveyQuestionText = dataTable.Rows[i][6].ToString();
                                    detail.SurveyQuestionAnswerText = dataTable.Rows[i][7].ToString();
                                    detail.CreatedOn = DateTime.Now;
                                    surveyDetails.Add(detail);
                                }
                            }
                        }

                    }
                }

                //Write data into Database
                SQLBulkInsert bulkInsert = new SQLBulkInsert();
                startParsingButton.Text = "Inserting data...";
                var rowsAffected =  await bulkInsert.InsertBulkRecord(surveyDetails);
                string message = $"Records Provided: {surveyDetails.Count}. \nRecords Inserted: {rowsAffected}";
                startParsingButton.Text = "Start Parsing";
                startParsingButton.Enabled = true;
                browseFileButton.Enabled = true;
                MessageBox.Show(message,"Message",MessageBoxButtons.OK,MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                startParsingButton.Text = "Start Parsing";
                startParsingButton.Enabled = true;
                browseFileButton.Enabled = true;
                MessageBox.Show(ex.Message.ToString(),"Message",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void browseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse CSV Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "CSV files (*.csv)|*.csv",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog1.FileName;
                filePathLabel.Text = selectedFilePath;
                if (string.IsNullOrEmpty(selectedFilePath))
                {
                    MessageBox.Show("Could not load the selected file", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

        }
        
    }
}
