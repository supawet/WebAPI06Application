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

using Excel.FinancialFunctions;

using System.Net.Http;
using System.Net.Http.Headers;

namespace WebAPI06Application
{
    public class WealthPlanTargetPersistance
    {
        //public ArrayList GetWealthPlanTarget(WealthPlanTarget wealthPlanTarget)
        public WealthPlanTargetResponse GetWealthPlanTarget(WealthPlanTarget wealthPlanTarget)
        {
            /*
            OdbcConnection conn = null;
            OdbcCommand command = null;
            OdbcDataReader mySQLReader = null;
            */

            SqlConnection conn = null;
            SqlCommand command = null;
            SqlDataReader mySQLReader = null;

            bool hasRows = false;

            string[] actions = { "A", "D", "U", "I" };

            var hash = System.Security.Cryptography.SHA512.Create();

            HttpContext httpContext = HttpContext.Current;
            string authHeader = httpContext.Request.Headers["Authorization"];

            //ArrayList proceedsArray = new ArrayList();
            List<Proceeds> proceedsArray = new List<Proceeds>();
            List<Proceeds> proceedsArrayTmp = new List<Proceeds>();

            WealthPlanTargetResponse wealthPlanTargetResponse = new WealthPlanTargetResponse();
            wealthPlanTargetResponse.Message = "Not Found";
            wealthPlanTargetResponse.Status = "Fail";

            wealthPlanTargetResponse.Amount_Needed = wealthPlanTarget.Amount_Needed;
            wealthPlanTargetResponse.Investment_Period = wealthPlanTarget.Investment_Period;
            wealthPlanTargetResponse.Initial_Investment = wealthPlanTarget.Initial_Investment;
            wealthPlanTargetResponse.Investment_Per_Month = wealthPlanTarget.Investment_Per_Month;
            wealthPlanTargetResponse.Investment_Risk = wealthPlanTarget.Investment_Risk;

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

                //  ทำการเก็บ log ด้วย token   
                //  ตอนนี้ยังไม่ต้องมี
                /*
                if (authHeader != null && authHeader.StartsWith("Bearer"))
                {
                    authHeader = authHeader.Substring("Bearer ".Length).Trim();
                    
                    //--------------------------------  Insert Log   ----------------
                    command.CommandType = CommandType.Text;
                    command.CommandText = "insert into SrvA_Log_Cloud(AccessToken,AccessModule,Dt_Gen,Flag) values(?,'WealthPlanTarget',GETDATE(),1)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@AccessToken", authHeader);
                    command.ExecuteNonQuery();

                    command.CommandType = CommandType.Text;
                    command.CommandText = "insert into SrvA_WealthPlanTarget_Log_Cloud(AccessToken,Amount_Needed,Investment_Period,Initial_Investment,Investment_Per_Month,Investment_Risk,Dt_Gen,Flag) values(?,'WealthPlanTarget',GETDATE(),1)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@AccessToken", authHeader);
                    command.Parameters.AddWithValue("@Amount_Needed", wealthPlanTarget.Amount_Needed);
                    command.Parameters.AddWithValue("@Investment_Period", wealthPlanTarget.Investment_Period);
                    command.Parameters.AddWithValue("@Initial_Investment", wealthPlanTarget.Initial_Investment);
                    command.Parameters.AddWithValue("@Investment_Per_Month", wealthPlanTarget.Investment_Per_Month);
                    command.Parameters.AddWithValue("@Investment_Risk", wealthPlanTarget.Investment_Risk);
                    command.ExecuteNonQuery();
                    //--------------------------------  /Insert Log  ----------------
                    
                }
                */

                SqlParameter param = null;

                if (wealthPlanTarget.AccessToken == null || wealthPlanTarget.AccessToken.Trim() == "")
                {
                    //wealthPlan.AccessToken = "test2";    //for test only
                    //wealthPlan.WealthPlanName = "WealthPlan 1";    //for test only

                    wealthPlanTargetResponse.Message = "AccessToken can not be null or empty";
                    wealthPlanTargetResponse.Status = "Fail";
                    return wealthPlanTargetResponse;
                }   //  end if wealthPlanTarget.AccessToken == null

                if (wealthPlanTarget.WealthPlanTargetName == null || wealthPlanTarget.WealthPlanTargetName.Trim() == "")
                {
                    wealthPlanTargetResponse.Message = "WealthPlanTarget Name can not be null or empty";
                    wealthPlanTargetResponse.Status = "Fail";
                    return wealthPlanTargetResponse;
                }   //  end if wealthPlanTarget.WealthPlanName == null

                if (wealthPlanTarget.Action == null || wealthPlanTarget.Action.Trim() == "")
                {
                    wealthPlanTargetResponse.Message = "Action can not be null or empty";
                    wealthPlanTargetResponse.Status = "Fail";
                    return wealthPlanTargetResponse;
                }   //  end if wealthPlanTarget.Action == null

                if (!actions.Contains(wealthPlanTarget.Action.ToUpper()))
                {
                    wealthPlanTargetResponse.Message = "Action can not be null or empty or invalid";
                    wealthPlanTargetResponse.Status = "Fail";
                    return wealthPlanTargetResponse;
                }   //  end if wealthPlan.Action not in A,D,U,I

                //  start Main
                //--------------------------------  Check Access Token   ----------------

                //--------------------------------  Check Access Token   ----------------

                //--------------------------------  Insert Log   ----------------
                command.CommandType = CommandType.Text;
                command.CommandText = "insert into SrvA_Log_Cloud(AccessToken,AccessModule,Dt_Gen,Flag) values(@AccessToken,'WealthPlanTarget',GETDATE(),1)";

                command.Parameters.Clear();
                param = null;

                param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                param.Value = wealthPlanTarget.AccessToken == null ? "" : wealthPlanTarget.AccessToken.Trim();
                param.Direction = ParameterDirection.Input;
                command.Parameters.Add(param);

                command.ExecuteNonQuery();
                //--------------------------------  /Insert Log  ----------------

                //--------------------------------  Check WealthPlanTargetName  ----------------
                command.CommandType = CommandType.Text;
                command.CommandText = "select wpt.WealthPlanTargetName, li.Mobile_No from SrvA_WealthPlanTarget_Cloud wpt left join SrvA_Login_Cloud li on wpt.AccessToken = li.AccessToken and wpt.Flag = 1 where Mobile_No = (select top 1 Mobile_No from SrvA_Login_Cloud where Flag = 1 and AccessToken = @AccessToken) and wpt.WealthPlanTargetName = @WealthPlanTargetName";

                command.Parameters.Clear();
                param = null;

                param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                param.Value = wealthPlanTarget.AccessToken == null ? "" : wealthPlanTarget.AccessToken.Trim();
                param.Direction = ParameterDirection.Input;
                command.Parameters.Add(param);

                param = new SqlParameter("@WealthPlanTargetName", System.Data.SqlDbType.NVarChar, 200);
                param.Value = wealthPlanTarget.WealthPlanTargetName == null ? "" : wealthPlanTarget.WealthPlanTargetName.Trim();
                param.Direction = ParameterDirection.Input;
                command.Parameters.Add(param);

                mySQLReader = command.ExecuteReader();

                if (mySQLReader.HasRows) hasRows = true;

                while (mySQLReader.Read())
                {
                    //mySQLReader.GetString(mySQLReader.GetOrdinal("UnitHolder"));
                    //forgotResponse.Message = mySQLReader.GetDataTypeName(mySQLReader.GetOrdinal("Mobile_No"));
                    //forgotResponse.Message = mySQLReader.GetValue(mySQLReader.GetOrdinal("Mobile_No")).ToString();
                    wealthPlanTarget.Mobile_No = mySQLReader.GetString(mySQLReader.GetOrdinal("Mobile_No"));
                    //forgotResponse.Message = "Waiting for OTP";
                }

                mySQLReader.Close();
                //--------------------------------  /Check WealthPlanTargetName  ----------------

                switch (wealthPlanTarget.Action.ToUpper())
                {
                    case "A":
                        if (hasRows)
                        {
                            wealthPlanTargetResponse.Message = "WealthPlanTargetName can not be duplicate";
                            wealthPlanTargetResponse.Status = "Fail";
                        }
                        else
                        {
                            //--------------------------------  Insert WealthPlanTarget   ----------------
                            command.CommandType = CommandType.Text;
                            command.CommandText = "insert into SrvA_WealthPlanTarget_Cloud(AccessToken,WealthPlanTargetName,Amount_Needed,Investment_Period,Initial_Investment,Investment_Per_Month,Investment_Risk,Dt_Gen,Flag) values(@AccessToken,@WealthPlanTargetName,@Amount_Needed,@Investment_Period,@Initial_Investment,@Investment_Per_Month,@Investment_Risk,GETDATE(),1)";

                            command.Parameters.Clear();
                            param = null;

                            param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                            param.Value = wealthPlanTarget.AccessToken == null ? "" : wealthPlanTarget.AccessToken.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@WealthPlanTargetName", System.Data.SqlDbType.NVarChar, 200);
                            param.Value = wealthPlanTarget.WealthPlanTargetName == null ? "" : wealthPlanTarget.WealthPlanTargetName.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Amount_Needed", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Amount_Needed == null ? 0 : wealthPlanTarget.Amount_Needed;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Investment_Period", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Investment_Period == null ? 0 : wealthPlanTarget.Investment_Period;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Initial_Investment", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Initial_Investment == null ? 0 : wealthPlanTarget.Initial_Investment;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Investment_Per_Month", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Investment_Per_Month == null ? 0 : wealthPlanTarget.Investment_Per_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Investment_Risk", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Investment_Risk == null ? 0 : wealthPlanTarget.Investment_Risk;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            command.ExecuteNonQuery();
                            //--------------------------------  /Insert WealthPlanTarget  ----------------
                            wealthPlanTargetResponse.Message = "Success";
                            wealthPlanTargetResponse.Status = "OK";
                        }
                        break;
                    case "U":
                        if (!hasRows)
                        {
                            wealthPlanTargetResponse.Message = "Can not find this name";
                            wealthPlanTargetResponse.Status = "Fail";
                        }
                        else
                        {
                            //--------------------------------  Update WealthPlanTarget   ----------------
                            command.CommandType = CommandType.Text;
                            command.CommandText = "update SrvA_WealthPlanTarget_Cloud set Flag = 0 where WealthPlanTargetName = @WealthPlanTargetName and AccessToken in (select AccessToken from SrvA_Login_Cloud where Mobile_No = @Mobile_No)";

                            command.Parameters.Clear();
                            param = null;

                            param = new SqlParameter("@WealthPlanTargetName", System.Data.SqlDbType.NVarChar, 200);
                            param.Value = wealthPlanTarget.WealthPlanTargetName == null ? "" : wealthPlanTarget.WealthPlanTargetName.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Mobile_No", System.Data.SqlDbType.NVarChar, 20);
                            param.Value = wealthPlanTarget.Mobile_No == null ? "" : wealthPlanTarget.Mobile_No.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            command.ExecuteNonQuery();
                            //--------------------------------  Update WealthPlanTarget   ----------------

                            //--------------------------------  Insert WealthPlanTarget   ----------------
                            command.CommandType = CommandType.Text;
                            command.CommandText = "insert into SrvA_WealthPlanTarget_Cloud(AccessToken,WealthPlanTargetName,Amount_Needed,Investment_Period,Initial_Investment,Investment_Per_Month,Investment_Risk,Dt_Gen,Flag) values(@AccessToken,@WealthPlanTargetName,@Amount_Needed,@Investment_Period,@Initial_Investment,@Investment_Per_Month,@Investment_Risk,GETDATE(),1)";

                            command.Parameters.Clear();
                            param = null;

                            param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                            param.Value = wealthPlanTarget.AccessToken == null ? "" : wealthPlanTarget.AccessToken.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@WealthPlanTargetName", System.Data.SqlDbType.NVarChar, 200);
                            param.Value = wealthPlanTarget.WealthPlanTargetName == null ? "" : wealthPlanTarget.WealthPlanTargetName.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Amount_Needed", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Amount_Needed == null ? 0 : wealthPlanTarget.Amount_Needed;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Investment_Period", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Investment_Period == null ? 0 : wealthPlanTarget.Investment_Period;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Initial_Investment", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Initial_Investment == null ? 0 : wealthPlanTarget.Initial_Investment;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Investment_Per_Month", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Investment_Per_Month == null ? 0 : wealthPlanTarget.Investment_Per_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Investment_Risk", System.Data.SqlDbType.Float);
                            param.Value = wealthPlanTarget.Investment_Risk == null ? 0 : wealthPlanTarget.Investment_Risk;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            command.ExecuteNonQuery();
                            //--------------------------------  /Insert WealthPlanTarget  ----------------
                            wealthPlanTargetResponse.Message = "Success";
                            wealthPlanTargetResponse.Status = "OK";
                        }
                        break;
                    case "D":
                        if (!hasRows)
                        {
                            wealthPlanTargetResponse.Message = "Can not find this name";
                            wealthPlanTargetResponse.Status = "Fail";
                        }
                        else
                        {
                            //--------------------------------  Delete WealthPlanTarget   ----------------
                            command.CommandType = CommandType.Text;
                            command.CommandText = "update SrvA_WealthPlanTarget_Cloud set Flag = 0 where WealthPlanTargetName = @WealthPlanTargetName and AccessToken in (select AccessToken from SrvA_Login_Cloud where Mobile_No = @Mobile_No)";

                            command.Parameters.Clear();
                            param = null;

                            param = new SqlParameter("@WealthPlanTargetName", System.Data.SqlDbType.NVarChar, 200);
                            param.Value = wealthPlanTarget.WealthPlanTargetName == null ? "" : wealthPlanTarget.WealthPlanTargetName.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Mobile_No", System.Data.SqlDbType.NVarChar, 20);
                            param.Value = wealthPlanTarget.Mobile_No == null ? "" : wealthPlanTarget.Mobile_No.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            command.ExecuteNonQuery();
                            //--------------------------------  Delete WealthPlanTarget   ----------------
                            wealthPlanTargetResponse.Message = "Success";
                            wealthPlanTargetResponse.Status = "OK";
                        }
                        break;
                    case "I":

                        wealthPlanTargetResponse.Amount_Needed = 0;
                        wealthPlanTargetResponse.Investment_Period = 0;
                        wealthPlanTargetResponse.Initial_Investment = 0;
                        wealthPlanTargetResponse.Investment_Per_Month = 0;
                        wealthPlanTargetResponse.Investment_Risk = 0;

                        if (!hasRows)
                        {
                            wealthPlanTargetResponse.Message = "Data not found";
                            wealthPlanTargetResponse.Status = "Fail";
                        }
                        else
                        {
                            /*
                            List<WealthPlanTargetInfo> WealthPlanTargetinfo = new List<WealthPlanTargetInfo>();

                            WealthPlanTargetInfo wpti = null;
                            */

                            //--------------------------------  Info WealthPlan   ----------------
                            command.CommandType = CommandType.Text;
                            command.CommandText = "select * from SrvA_WealthPlanTarget_Cloud where Flag = 1 and WealthPlanTargetName = @WealthPlanTargetName and AccessToken in (select AccessToken from SrvA_Login_Cloud where Mobile_No = @Mobile_No)";

                            command.Parameters.Clear();
                            param = null;

                            param = new SqlParameter("@WealthPlanTargetName", System.Data.SqlDbType.NVarChar, 200);
                            param.Value = wealthPlanTarget.WealthPlanTargetName == null ? "" : wealthPlanTarget.WealthPlanTargetName.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Mobile_No", System.Data.SqlDbType.NVarChar, 20);
                            param.Value = wealthPlanTarget.Mobile_No == null ? "" : wealthPlanTarget.Mobile_No.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            mySQLReader = command.ExecuteReader();

                            while (mySQLReader.Read())
                            {   /*
                                wpti = new WealthPlanTargetInfo();

                                wealthPlanTarget.Amount_Needed = mySQLReader.GetDouble(mySQLReader.GetOrdinal("Amount_Needed"));
                                wpti.Amount_Needed = wealthPlanTarget.Amount_Needed;
                                */

                                wealthPlanTargetResponse.Amount_Needed = mySQLReader.GetDouble(mySQLReader.GetOrdinal("Amount_Needed"));
                                wealthPlanTargetResponse.Investment_Period = mySQLReader.GetDouble(mySQLReader.GetOrdinal("Investment_Period"));
                                wealthPlanTargetResponse.Initial_Investment = mySQLReader.GetDouble(mySQLReader.GetOrdinal("Initial_Investment"));
                                wealthPlanTargetResponse.Investment_Per_Month = mySQLReader.GetDouble(mySQLReader.GetOrdinal("Investment_Per_Month"));
                                wealthPlanTargetResponse.Investment_Risk = mySQLReader.GetInt32(mySQLReader.GetOrdinal("Investment_Risk"));

                                //WealthPlanTargetinfo.Add(wpti);
                            }
                            mySQLReader.Close();
                            //--------------------------------  Info WealthPlan   ----------------
                            //wealthPlanTargetResponse.Data = WealthPlanTargetinfo;
                            wealthPlanTargetResponse.Message = "Success";
                            wealthPlanTargetResponse.Status = "OK";
                        }
                        break;
                    default: break;
                }   //  end switch

                /*  -------------   คำอธิบาย  -------------
                    lowest = ต่ำสุด
                    downtrend = ขาลง
                    normL= ปกติ L
                    normH = ปกติ H
                    uptrend = ขาขึ้น
                */
                //  -------------   ค่า z-Score  -------------
                Dictionary<string, double> zScore = new Dictionary<string, double>(){
                    { "lowest", -1.9600},
                    { "downtrend", -1.1503},
                    { "normL", 0.0000},
                    { "normH", 1.1503},
                    { "uptrend", 1.9600}
                };
                //  -------------   ผลตอบแทนตามความเสี่ยง  -------------
                Dictionary<int, ExpectedReturn> expectedReturn = new Dictionary<int, ExpectedReturn>(){
                    { 1, new ExpectedReturn { SD= 0.0176, RET= 0.0464 }},
                    { 2, new ExpectedReturn { SD= 0.0335, RET= 0.0609 }},
                    { 3, new ExpectedReturn { SD= 0.0335, RET= 0.0609 }},
                    { 4, new ExpectedReturn { SD= 0.0335, RET= 0.0609 }},
                    { 5, new ExpectedReturn { SD= 0.0485, RET= 0.0713 }},
                    { 6, new ExpectedReturn { SD= 0.0667, RET= 0.0863 }},
                    { 7, new ExpectedReturn { SD= 0.0667, RET= 0.0863 }},
                    { 8, new ExpectedReturn { SD= 0.0990, RET= 0.1600 }}
                };

                Proceeds proceeds = null;
                //Suggest an alternative

                double futureValue = 0;
                double endingWealth = 0;
                bool endingWealthBool = true;
                double separateGraph = 0;

                //int periodLoop = (int)wealthPlanTarget.Investment_Period * 12 * 5;
                int periodLoop = 180;
                double endingWealthTarget = 0;  //เป้าหมายที่ได้

                for (int i = 1; i <= periodLoop; i++) {
                    proceeds = new Proceeds();
                    futureValue = wealthPlanTarget.Investment_Per_Month * ((Math.Pow(1 + (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0), i)) - 1) / (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0) + wealthPlanTarget.Initial_Investment * (Math.Pow(1 + (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0), i));

                    proceeds.Month = i;
                    proceeds.Lowest = Math.Round(futureValue + zScore["lowest"] * (expectedReturn[wealthPlanTarget.Investment_Risk].SD * Math.Sqrt(i / 12.0)) * futureValue, 4);
                    proceeds.Downtrend = Math.Round(futureValue + zScore["downtrend"] * (expectedReturn[wealthPlanTarget.Investment_Risk].SD * Math.Sqrt(i / 12.0)) * futureValue, 4);
                    proceeds.NormL = Math.Round(futureValue + zScore["normL"] * (expectedReturn[wealthPlanTarget.Investment_Risk].SD * Math.Sqrt(i / 12.0)) * futureValue, 4);
                    proceeds.NormH = Math.Round(futureValue + zScore["normH"] * (expectedReturn[wealthPlanTarget.Investment_Risk].SD * Math.Sqrt(i / 12.0)) * futureValue, 4);
                    proceeds.Uptrend = Math.Round(futureValue + zScore["uptrend"] * (expectedReturn[wealthPlanTarget.Investment_Risk].SD * Math.Sqrt(i / 12.0)) * futureValue,4);

                    proceedsArray.Add(proceeds);
                    /*
                    if (i == (int)wealthPlanTarget.Investment_Period * 12)
                    {
                        endingWealthTarget = futureValue;
                    }
                    */

                    if (endingWealthBool && wealthPlanTarget.Amount_Needed <= futureValue)
                    {
                        endingWealth = i;
                        endingWealthTarget = futureValue;
                        endingWealthBool = false;
                    }
                }

                separateGraph = wealthPlanTarget.Investment_Period * 12 > endingWealth ? wealthPlanTarget.Investment_Period * 12 : endingWealth;
                if(separateGraph <= 60)
                {
                    periodLoop = 60;
                }else if(separateGraph <= 120){
                    periodLoop = 120;
                }else{
                    periodLoop = 180;
                }
                for (int i = 1; i <= periodLoop; i++)
                {
                    proceedsArrayTmp.Add(proceedsArray[i-1]);
                }


                if (wealthPlanTarget.Amount_Needed < endingWealthTarget && endingWealth <= wealthPlanTarget.Investment_Period * 12)   //  ถึงเป้า
                {
                    wealthPlanTargetResponse.Target = "Y";
                    wealthPlanTargetResponse.Target_Month = Math.Round(endingWealth, MidpointRounding.AwayFromZero);
                    wealthPlanTargetResponse.Target_Amount = Math.Round(endingWealthTarget, MidpointRounding.AwayFromZero);
                }
                else
                {
                    wealthPlanTargetResponse.Target = "N";

                    //----------------- ยืดระยะเวลาลงทุน(ปี)  --------------------------
                    wealthPlanTargetResponse.Recommended_Choice1 = endingWealth;
                    //----------------- /ยืดระยะเวลาลงทุน(ปี)  --------------------------

                    periodLoop = (int)wealthPlanTarget.Investment_Period * 12;

                    //----------------- เพิ่มเงินลงทุนตั้งต้น(บาท) --------------------------
                    //for (int i = 10000; i <= 1000000; i++)
                    //{
                    endingWealth = (wealthPlanTarget.Amount_Needed - (wealthPlanTarget.Investment_Per_Month * ((Math.Pow(1 + (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0), periodLoop)) - 1) / (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0))) / (Math.Pow(1 +
    (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0), periodLoop));

                      //  if (endingWealth >= wealthPlanTarget.Amount_Needed) break;
                    //}

                    wealthPlanTargetResponse.Recommended_Choice2 = Math.Round(endingWealth, MidpointRounding.AwayFromZero);
                    //----------------- /เพิ่มเงินลงทุนตั้งต้น(บาท) --------------------------

                    //----------------- เพิ่มเงินลงทุนต่อเดือน(บาท) --------------------------
                    for (int i = 1000; i <= 100000; i++)
                    {
                        endingWealth = (wealthPlanTarget.Amount_Needed - i * (Math.Pow(1 + (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0), periodLoop))) / ((Math.Pow(1 + (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0), periodLoop)) - 1) * (expectedReturn[wealthPlanTarget.Investment_Risk].RET / 12.0);

                        if (endingWealth >= wealthPlanTarget.Amount_Needed) break;
                    }

                    wealthPlanTargetResponse.Recommended_Choice3 = Math.Round(endingWealth, MidpointRounding.AwayFromZero);
                    //----------------- เพิ่มเงินลงทุนต่อเดือน(บาท) --------------------------
                }

                if (wealthPlanTargetResponse.Status == "OK")
                {
                    wealthPlanTargetResponse.Plot = proceedsArrayTmp;
                }
                
                
                //wealthPlanTargetResponse.Message = "Success";
                //wealthPlanTargetResponse.Status = "OK";

                // P is the periodic deposit
                // N is number of periods per year
                // R is the effective annual rate
                // r is the periodic rate
                // T is the total number of periods
                // A is the future value

                /*
                double P = wealthPlanTarget.Investment_Per_Month;   //ลงทุนต่อเดือน
                int N = 12;
                double R = wealthPlanTarget.Interest / 100;  //อัตราผลตอบแทน
                double r = Math.Pow(1 + R, 1 / N) - 1;
                int T = wealthPlanTarget.Investment_Period; //จำนวนเดือน

                double A = 0;
                for (int i = 1; i <= T; i++)
                    A = A + P * Math.Pow(1 + r, i);
                double Interest = A - P;
                */

                //decimal futureValues = this.CalculateFutureValue(wealthPlanTarget.Investment_Per_Month, wealthPlanTarget.Interest / 100 / 12, wealthPlanTarget.Investment_Period);

                //double futureValue = Financial.Fv(wealthPlanTarget.Interest, wealthPlanTarget.Investment_Period, -wealthPlanTarget.Investment_Per_Month,0, 0);

                /*
                FV = MonthlyDeposit * (((1 + MonthlyInterest)^Months - 1 ) 
 / MonthlyInterest ) + StartingBalance * ( 1 + MonthlyInterest )^Months
 */
                /*
                               futureValue = wealthPlanTarget.Investment_Per_Month * ((Math.Pow(1+((wealthPlanTarget.Interest/100) / 12),(wealthPlanTarget.Investment_Period*12))) - 1) / ((wealthPlanTarget.Interest/100) / 12) + wealthPlanTarget.Initial_Investment * (Math.Pow(1+
                                   ((wealthPlanTarget.Interest / 100) / 12),(wealthPlanTarget.Investment_Period*12)));
                                   */
                //wealthPlanTargetResponse.Test = futureValue;

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

        private decimal CalculateFutureValue(decimal monthlyInvestment,decimal monthlyInterestRate, int months)
        {
            decimal futureValue = 0m;
            for (int i = 0; i < months; i++)
            {
                futureValue = (futureValue + monthlyInvestment) * (1 + monthlyInterestRate);
            }

            return futureValue;
        }
    }
}
 