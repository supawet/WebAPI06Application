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
    public class WealthPlanTargetListPersistance
    {
        //public ArrayList GetWealthPlanTarget(WealthPlanTarget wealthPlanTarget)
        public WealthPlanTargetListResponse GetWealthPlanTargetList(WealthPlanTargetList wealthPlanTargetList)
        {
            /*
            OdbcConnection conn = null;
            OdbcCommand command = null;
            OdbcDataReader mySQLReader = null;
            */

            SqlConnection conn = null;
            SqlCommand command = null;
            SqlDataReader mySQLReader = null;

            List<WealthPlanTargetListName> wealthPlanTargetListName = new List<WealthPlanTargetListName>();

            WealthPlanTargetListName wptln = null;

            WealthPlanTargetListResponse wealthPlanTargetListResponse = new WealthPlanTargetListResponse();
            wealthPlanTargetListResponse.Message = "Not Found";
            wealthPlanTargetListResponse.Status = "Fail";

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
                command.CommandText = "select wpt.WealthPlanTargetName, li.Mobile_No from SrvA_WealthPlanTarget_Cloud wpt left join SrvA_Login_Cloud li on wpt.AccessToken = li.AccessToken and wpt.Flag = 1 where Mobile_No = (select top 1 Mobile_No from SrvA_Login_Cloud where Flag = 1 and AccessToken = @AccessToken)";

                command.Parameters.Clear();
                param = null;

                param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                param.Value = wealthPlanTargetList.AccessToken == null ? "" : wealthPlanTargetList.AccessToken.Trim();
                param.Direction = ParameterDirection.Input;
                command.Parameters.Add(param);

                mySQLReader = command.ExecuteReader();

                while (mySQLReader.Read())
                {
                    wptln = new WealthPlanTargetListName();

                    wptln.WealthPlanTargetName = mySQLReader.GetString(mySQLReader.GetOrdinal("WealthPlanTargetName"));
                    wealthPlanTargetListName.Add(wptln);
                    //wealthPlanTargetListName.WealthPlanTargetName = mySQLReader.GetString(mySQLReader.GetOrdinal("WealthPlanTargetName"));
                    //mySQLReader.GetString(mySQLReader.GetOrdinal("UnitHolder"));
                    //forgotResponse.Message = mySQLReader.GetDataTypeName(mySQLReader.GetOrdinal("Mobile_No"));
                    //forgotResponse.Message = mySQLReader.GetValue(mySQLReader.GetOrdinal("Mobile_No")).ToString();
                    //wealthPlanTarget.Mobile_No = mySQLReader.GetString(mySQLReader.GetOrdinal("Mobile_No"));
                    //forgotResponse.Message = "Waiting for OTP";
                }
                mySQLReader.Close();
                //--------------------------------  /Check WealthPlanName  ----------------
                wealthPlanTargetListResponse.Data = wealthPlanTargetListName;
                wealthPlanTargetListResponse.Message = "Success";
                wealthPlanTargetListResponse.Status = "OK";

                return wealthPlanTargetListResponse;
            }

            catch (Exception ex)
            {
                wealthPlanTargetListResponse.Message = ex.ToString();
                wealthPlanTargetListResponse.Status = "Fail";
                return wealthPlanTargetListResponse;
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