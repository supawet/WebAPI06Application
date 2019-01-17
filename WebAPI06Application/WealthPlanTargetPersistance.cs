using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

using System.Collections;
using System.Globalization;
using WebAPI06Application.Models;
using WebAPI06Application.Services;
using System.Configuration;

using System.Security.Cryptography;
using System.Web.Security;
namespace WebAPI06Application
{
    public class WealthPlanTargetPersistance
    {
        //public ArrayList GetWealthPlanTarget(WealthPlanTarget wealthPlanTarget)
        public WealthPlanTargetResponse GetWealthPlanTarget(WealthPlanTarget wealthPlanTarget)
        {
            OleDbConnection conn = null;
            OleDbCommand command = null;
            OleDbDataReader mySQLReader = null;

            var hash = System.Security.Cryptography.SHA512.Create();

            WealthPlanTargetResponse wealthPlanTargetResponse = new WealthPlanTargetResponse();
            wealthPlanTargetResponse.Message = "Not Found";
            wealthPlanTargetResponse.Status = "Fail";

            try
            {
                string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString; ;
                conn = new OleDbConnection(myConnectionString);

                conn.Open();

                command = new OleDbCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                //--------------------------------  Insert Log   ----------------
                command.CommandType = CommandType.Text;
                command.CommandText = "insert into SrvA_Log_Cloud(AccessToken,AccessModule,Dt_Gen,Flag) values(?,'WealthPlan',GETDATE(),1)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@AccessToken", wealthPlanTarget.AccessToken);
                command.ExecuteNonQuery();
                //--------------------------------  /Insert Log  ----------------

                //wealthPlanTargetResponse.Saving_Analysis_Result = Math.Round(Saving_Analysis_Result, 2);
                wealthPlanTargetResponse.Message = "Success";
                wealthPlanTargetResponse.Status = "OK";
                return wealthPlanTargetResponse;
            }

            catch (Exception ex)
            {
                wealthPlanTargetResponse.Message = ex.ToString();
                wealthPlanTargetResponse.Status = "Fail";
                return wealthPlanTargetResponse;
            }
            finally
            {
                if (mySQLReader != null)
                {
                    mySQLReader.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}