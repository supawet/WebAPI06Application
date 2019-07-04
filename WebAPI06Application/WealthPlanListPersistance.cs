using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
//using System.Data.OleDb;
using System.Data.Odbc;
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
    public class WealthPlanListPersistance
    {
        //public ArrayList GetWealthPlan(WealthPlan wealthPlan)
        public WealthPlanListResponse GetWealthPlanList(WealthPlanList wealthPlanList)
        {
            /*
            OdbcConnection conn = null;
            OdbcCommand command = null;
            OdbcDataReader mySQLReader = null;
            */

            SqlConnection conn = null;
            SqlCommand command = null;
            SqlDataReader mySQLReader = null;

            List<WealthPlanListName> wealthPlanListName = new List<WealthPlanListName>();

            WealthPlanListName wpln = null;

            WealthPlanListResponse wealthPlanListResponse = new WealthPlanListResponse();
            wealthPlanListResponse.Message = "Not Found";
            wealthPlanListResponse.Status = "Fail";

            try
            {
                string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString; ;
                //conn = new OleDbConnection(myConnectionString);
                conn = new SqlConnection(myConnectionString);

                conn.Open();

                //command = new OleDbCommand();
                command = new SqlCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                SqlParameter param = null;

                //--------------------------------  Check WealthPlanName  ----------------
                command.CommandType = CommandType.Text;
                command.CommandText = "select wp.WealthPlanName, li.Mobile_No from SrvA_WealthPlan_Cloud wp left join SrvA_Login_Cloud li on wp.AccessToken = li.AccessToken and wp.Flag = 1 where Mobile_No = (select top 1 Mobile_No from SrvA_Login_Cloud where Flag = 1 and AccessToken = @AccessToken)";

                command.Parameters.Clear();
                param = null;

                param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                param.Value = wealthPlanList.AccessToken == null ? "" : wealthPlanList.AccessToken.Trim();
                param.Direction = ParameterDirection.Input;
                command.Parameters.Add(param);

                mySQLReader = command.ExecuteReader();

                while (mySQLReader.Read())
                {
                    wpln = new WealthPlanListName();

                    wpln.WealthPlanName = mySQLReader.GetString(mySQLReader.GetOrdinal("WealthPlanName"));
                    wealthPlanListName.Add(wpln);
                    //wealthPlanListName.WealthPlanName = mySQLReader.GetString(mySQLReader.GetOrdinal("WealthPlanName"));
                    //mySQLReader.GetString(mySQLReader.GetOrdinal("UnitHolder"));
                    //forgotResponse.Message = mySQLReader.GetDataTypeName(mySQLReader.GetOrdinal("Mobile_No"));
                    //forgotResponse.Message = mySQLReader.GetValue(mySQLReader.GetOrdinal("Mobile_No")).ToString();
                    //wealthPlan.Mobile_No = mySQLReader.GetString(mySQLReader.GetOrdinal("Mobile_No"));
                    //forgotResponse.Message = "Waiting for OTP";
                }
                mySQLReader.Close();
                //--------------------------------  /Check WealthPlanName  ----------------
                wealthPlanListResponse.Data = wealthPlanListName;
                wealthPlanListResponse.Message = "Success";
                wealthPlanListResponse.Status = "OK";

                return wealthPlanListResponse;
            }

            catch (Exception ex)
            {
                wealthPlanListResponse.Message = ex.ToString();
                wealthPlanListResponse.Status = "Fail";
                return wealthPlanListResponse;
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