using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;

namespace ExcelParserApp
{
    public class SQLBulkInsert
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["EMRPatientDB"].ToString();
        
        public async Task<int> InsertBulkRecord(List<PatientSurveyDetail> patientSurveyDetails)
        {
            int bulkInsertRowAtOnce = 10000;
            int rowsAffected = 0;
            int InsertToRowCount = 0;
            try
            {
                StringBuilder insertCommand = new StringBuilder();
                foreach (var item in patientSurveyDetails)
                {                    
                    var data = string.Format("INSERT INTO PatientSurveyDetail([PatientName],[MRN],[PatientAge],[ProfessionalName],[SurveyDate],[SurveyQuestionText],[SurveyQuestionAnswerText],[CreatedOn]) VALUES ('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}');", item.PatientName, item.MRN, item.PatientAge, item.ProfessionalName, item.SurveyDate, item.SurveyQuestionText, item.SurveyQuestionAnswerText,item.CreatedOn);
                    insertCommand.Append(data);
                    InsertToRowCount++;
                    if (InsertToRowCount % bulkInsertRowAtOnce == 0)
                    {
                        rowsAffected += await SplitInsert(insertCommand);
                        Console.WriteLine("bulkInsertRowAtOnce: " + bulkInsertRowAtOnce);
                        Console.WriteLine("rowsAffected: " + rowsAffected);
                        insertCommand = new StringBuilder();                        
                    }                    
                }
                if ((patientSurveyDetails.Count - rowsAffected) < bulkInsertRowAtOnce)
                {
                    rowsAffected += await SplitInsert(insertCommand);
                    Console.WriteLine("bulkInsertRowAtOnce: " + bulkInsertRowAtOnce);
                    Console.WriteLine("rowsAffected: " + rowsAffected);
                }
                return rowsAffected;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> SplitInsert(StringBuilder insertCommand)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    using (SqlCommand myCmd = new SqlCommand(insertCommand.ToString(), connection))
                    {
                        myCmd.CommandType = System.Data.CommandType.Text;
                        myCmd.Transaction = transaction;
                        try
                        {
                            rowsAffected = await myCmd.ExecuteNonQueryAsync();
                            myCmd.Transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            myCmd.Transaction.Rollback();
                        }

                    }
                    connection.Close();
                }
                return rowsAffected;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
