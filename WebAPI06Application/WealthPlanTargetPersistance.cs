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
            OleDbConnection conn = null;
            OleDbCommand command = null;
            OleDbDataReader mySQLReader = null;
            
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
                conn = new OleDbConnection(myConnectionString);

                conn.Open();

                command = new OleDbCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                //  ทำการเก็บ log ด้วย token
                if (authHeader != null && authHeader.StartsWith("Bearer"))
                {
                    authHeader = authHeader.Substring("Bearer ".Length).Trim();
                    /*
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
                    */
                }

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

                wealthPlanTargetResponse.Plot = proceedsArrayTmp;
                
                wealthPlanTargetResponse.Message = "Success";
                wealthPlanTargetResponse.Status = "OK";

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
 