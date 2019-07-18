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
    public class WealthPlanPersistance
    {
        //public ArrayList GetWealthPlan(WealthPlan wealthPlan)
        public WealthPlanResponse GetWealthPlan(WealthPlan wealthPlan)
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

            string[] actions = {"A","D","U","I"};

            var hash = System.Security.Cryptography.SHA512.Create();

            WealthPlanResponse wealthPlanResponse = new WealthPlanResponse();
            wealthPlanResponse.Message = "Not Found";
            wealthPlanResponse.Status = "Fail";

            decimal Balance_Sheet_Asset_Liquidity = 0;
            decimal Balance_Sheet_Asset_Personal = 0;
            decimal Balance_Sheet_Asset_Invest = 0;
            decimal Balance_Sheet_Liability_Short_Term = 0;
            decimal Balance_Sheet_Liability_Long_Term = 0;
            decimal Expense_Fix = 0;
            decimal Expense_Fix_Sub = 0;
            decimal Expense_Saving_and_Investing = 0;
            decimal Expense_Vary = 0;

            decimal Total_Asset = 0;
            decimal Total_Liability = 0;
            decimal Total_Wealth = 0;
            decimal Total_Income = 0;
            decimal Total_Expense = 0;
            decimal Liquidity_Analysis_Result = 0;
            decimal Ability_To_Pay_Short_Term_Debt = 0;
            decimal Ability_To_Pay_Mid_Term_Debt = 0;
            decimal Payment_Of_Debt_From_Income = 0;
            decimal Saving_Analysis_Result = 0;

            decimal Ratio_Liability_Per_Asset = 0;

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

                //wealthPlan.AccessToken = "test2";    //for test only
                
                if (wealthPlan.AccessToken == null || wealthPlan.AccessToken.Trim() == "")
                {
                    //wealthPlan.AccessToken = "test2";    //for test only
                    //wealthPlan.WealthPlanName = "WealthPlan 1";    //for test only

                    wealthPlanResponse.Message = "AccessToken can not be null or empty";
                    wealthPlanResponse.Status = "Fail";
                    return wealthPlanResponse;
                }   //  end if wealthPlan.AccessToken == null
                
                if(wealthPlan.WealthPlanName == null || wealthPlan.WealthPlanName.Trim() == "")
                {
                    wealthPlanResponse.Message = "WealthPlan Name can not be null or empty";
                    wealthPlanResponse.Status = "Fail";
                    return wealthPlanResponse;
                }   //  end if wealthPlan.WealthPlanName == null

                if (wealthPlan.Action == null || wealthPlan.Action.Trim() == "")
                {
                    wealthPlanResponse.Message = "Action can not be null or empty";
                    wealthPlanResponse.Status = "Fail";
                    return wealthPlanResponse;
                }   //  end if wealthPlan.Action == null

                if (!actions.Contains(wealthPlan.Action.ToUpper()))
                {
                    wealthPlanResponse.Message = "Action can not be null or empty or invalid";
                    wealthPlanResponse.Status = "Fail";
                    return wealthPlanResponse;
                }   //  end if wealthPlan.Action not in A,D,U,I

                //  start Main
                //--------------------------------  Check Access Token   ----------------

                //--------------------------------  Check Access Token   ----------------

                //--------------------------------  Insert Log   ----------------
                command.CommandType = CommandType.Text;
                command.CommandText = "insert into SrvA_Log_Cloud(AccessToken,AccessModule,Dt_Gen,Flag) values(@AccessToken,'WealthPlan',GETDATE(),1)";

                command.Parameters.Clear();
                param = null;

                param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                param.Value = wealthPlan.AccessToken == null ? "" : wealthPlan.AccessToken.Trim();
                param.Direction = ParameterDirection.Input;
                command.Parameters.Add(param);

                command.ExecuteNonQuery();
                //--------------------------------  /Insert Log  ----------------

                //--------------------------------  Check WealthPlanName  ----------------
                command.CommandType = CommandType.Text;
                command.CommandText = "select wp.WealthPlanName, li.Mobile_No from SrvA_WealthPlan_Cloud wp left join SrvA_Login_Cloud li on wp.AccessToken = li.AccessToken and wp.Flag = 1 where Mobile_No = (select top 1 Mobile_No from SrvA_Login_Cloud where Flag = 1 and AccessToken = @AccessToken) and wp.WealthPlanName = @WealthPlanName";

                command.Parameters.Clear();
                param = null;

                param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                param.Value = wealthPlan.AccessToken == null ? "" : wealthPlan.AccessToken.Trim();
                param.Direction = ParameterDirection.Input;
                command.Parameters.Add(param);

                param = new SqlParameter("@WealthPlanName", System.Data.SqlDbType.NVarChar, 200);
                param.Value = wealthPlan.WealthPlanName == null ? "" : wealthPlan.WealthPlanName.Trim();
                param.Direction = ParameterDirection.Input;
                command.Parameters.Add(param);

                mySQLReader = command.ExecuteReader();

                if (mySQLReader.HasRows) hasRows = true;
                    
                while (mySQLReader.Read())
                {
                    //mySQLReader.GetString(mySQLReader.GetOrdinal("UnitHolder"));
                    //forgotResponse.Message = mySQLReader.GetDataTypeName(mySQLReader.GetOrdinal("Mobile_No"));
                    //forgotResponse.Message = mySQLReader.GetValue(mySQLReader.GetOrdinal("Mobile_No")).ToString();
                    wealthPlan.Mobile_No = mySQLReader.GetString(mySQLReader.GetOrdinal("Mobile_No"));
                    //forgotResponse.Message = "Waiting for OTP";
                }
                    
                mySQLReader.Close();
                //--------------------------------  /Check WealthPlanName  ----------------

                switch (wealthPlan.Action.ToUpper()) {
                    case "A":
                        if (hasRows)
                        {
                            wealthPlanResponse.Message = "WealthPlanName can not be duplicate";
                            wealthPlanResponse.Status = "Fail";
                        }
                        else
                        {
                            //--------------------------------  Insert WealthPlan   ----------------
                            command.CommandType = CommandType.Text;
                            command.CommandText = "insert into SrvA_WealthPlan_Cloud(AccessToken," +
                                "WealthPlanName," +
                                "Balance_Sheet_Asset_Liquidity_Cash," +
                                "Balance_Sheet_Asset_Liquidity_Deposit," +
                                "Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund," +
                                "Balance_Sheet_Asset_Invest_Mutual_Fund," +
                                "Balance_Sheet_Asset_Invest_Common_Stock," +
                                "Balance_Sheet_Asset_Invest_Bond," +
                                "Balance_Sheet_Asset_Invest_Property," +
                                "Balance_Sheet_Asset_Invest_Other," +
                                "Balance_Sheet_Asset_Personal_Home," +
                                "Balance_Sheet_Asset_Personal_Car," +
                                "Balance_Sheet_Asset_Personal_Other," +
                                "Balance_Sheet_Liability_Short_Term_Credit_Card," +
                                "Balance_Sheet_Liability_Short_Term_Cash_Card," +
                                "Balance_Sheet_Liability_Long_Term_Home," +
                                "Balance_Sheet_Liability_Long_Term_Car," +
                                "Balance_Sheet_Liability_Long_Term_Other," +
                                "Income_Salary_Month," +
                                "Income_Salary_Year," +
                                "Income_Bonus_Month," +
                                "Income_Bonus_Year," +
                                "Income_Other_Month," +
                                "Income_Other_Year," +
                                "Expense_Fix_Insurance_Premium_Month," +
                                "Expense_Fix_Insurance_Premium_Year," +
                                "Expense_Fix_Home_Month," +
                                "Expense_Fix_Home_Year," +
                                "Expense_Fix_Car_Month," +
                                "Expense_Fix_Car_Year," +
                                "Expense_Fix_Credit_Card_Month," +
                                "Expense_Fix_Credit_Card_Year," +
                                "Expense_Fix_Car_Insurance_Premium_Month," +
                                "Expense_Fix_Car_Insurance_Premium_Year," +
                                "Expense_Fix_Social_Security_Month," +
                                "Expense_Fix_Social_Security_Year," +
                                "Expense_Fix_Other_Month," +
                                "Expense_Fix_Other_Year," +
                                "Expense_Vary_Four_Requisites_Month," +
                                "Expense_Vary_Four_Requisites_Year," +
                                "Expense_Vary_Telephone_Charge_Month," +
                                "Expense_Vary_Telephone_Charge_Year," +
                                "Expense_Vary_Travelling_Expense_Month," +
                                "Expense_Vary_Travelling_Expense_Year," +
                                "Expense_Vary_Living_Allowance_Month," +
                                "Expense_Vary_Living_Allowance_Year," +
                                "Expense_Vary_Donation_Month," +
                                "Expense_Vary_Donation_Year," +
                                "Expense_Vary_Other_Month," +
                                "Expense_Vary_Other_Year," +
                                "Expense_Saving_and_Investing_Saving_Month," +
                                "Expense_Saving_and_Investing_Saving_Year," +
                                "Expense_Saving_and_Investing_Investing_RMF_Month," +
                                "Expense_Saving_and_Investing_Investing_RMF_Year," +
                                "Expense_Saving_and_Investing_Investing_LTF_Month," +
                                "Expense_Saving_and_Investing_Investing_LTF_Year," +
                                "Expense_Saving_and_Investing_Investing_PVF_Month," +
                                "Expense_Saving_and_Investing_Investing_PVF_Year," +
                                "Expense_Saving_and_Investing_Investing_Mutual_Fund_Month," +
                                "Expense_Saving_and_Investing_Investing_Mutual_Fund_Year," +
                                "Expense_Saving_and_Investing_Investing_Common_Stock_Month," +
                                "Expense_Saving_and_Investing_Investing_Common_Stock_Year," +
                                "Expense_Saving_and_Investing_Investing_Other_Month," +
                                "Expense_Saving_and_Investing_Investing_Other_Year," +
                                "Dt_Gen,Flag) values(@AccessToken,@WealthPlanName,@Balance_Sheet_Asset_Liquidity_Cash,@Balance_Sheet_Asset_Liquidity_Deposit,@Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund,@Balance_Sheet_Asset_Invest_Mutual_Fund,@Balance_Sheet_Asset_Invest_Common_Stock,@Balance_Sheet_Asset_Invest_Bond,@Balance_Sheet_Asset_Invest_Property,@Balance_Sheet_Asset_Invest_Other,@Balance_Sheet_Asset_Personal_Home,@Balance_Sheet_Asset_Personal_Car,@Balance_Sheet_Asset_Personal_Other,@Balance_Sheet_Liability_Short_Term_Credit_Card,@Balance_Sheet_Liability_Short_Term_Cash_Card,@Balance_Sheet_Liability_Long_Term_Home,@Balance_Sheet_Liability_Long_Term_Car,@Balance_Sheet_Liability_Long_Term_Other,@Income_Salary_Month,@Income_Salary_Year,@Income_Bonus_Month,@Income_Bonus_Year,@Income_Other_Month,@Income_Other_Year,@Expense_Fix_Insurance_Premium_Month,@Expense_Fix_Insurance_Premium_Year,@Expense_Fix_Home_Month,@Expense_Fix_Home_Year,@Expense_Fix_Car_Month,@Expense_Fix_Car_Year,@Expense_Fix_Credit_Card_Month,@Expense_Fix_Credit_Card_Year,@Expense_Fix_Car_Insurance_Premium_Month,@Expense_Fix_Car_Insurance_Premium_Year,@Expense_Fix_Social_Security_Month,@Expense_Fix_Social_Security_Year,@Expense_Fix_Other_Month,@Expense_Fix_Other_Year,@Expense_Vary_Four_Requisites_Month,@Expense_Vary_Four_Requisites_Year,@Expense_Vary_Telephone_Charge_Month,@Expense_Vary_Telephone_Charge_Year,@Expense_Vary_Travelling_Expense_Month,@Expense_Vary_Travelling_Expense_Year,@Expense_Vary_Living_Allowance_Month,@Expense_Vary_Living_Allowance_Year,@Expense_Vary_Donation_Month,@Expense_Vary_Donation_Year,@Expense_Vary_Other_Month,@Expense_Vary_Other_Year,@Expense_Saving_and_Investing_Saving_Month,@Expense_Saving_and_Investing_Saving_Year,@Expense_Saving_and_Investing_Investing_RMF_Month,@Expense_Saving_and_Investing_Investing_RMF_Year,@Expense_Saving_and_Investing_Investing_LTF_Month,@Expense_Saving_and_Investing_Investing_LTF_Year,@Expense_Saving_and_Investing_Investing_PVF_Month,@Expense_Saving_and_Investing_Investing_PVF_Year,@Expense_Saving_and_Investing_Investing_Mutual_Fund_Month,@Expense_Saving_and_Investing_Investing_Mutual_Fund_Year,@Expense_Saving_and_Investing_Investing_Common_Stock_Month,@Expense_Saving_and_Investing_Investing_Common_Stock_Year,@Expense_Saving_and_Investing_Investing_Other_Month,@Expense_Saving_and_Investing_Investing_Other_Year,GETDATE(),1)";

                            command.Parameters.Clear();
                            param = null;

                            param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                            param.Value = wealthPlan.AccessToken == null ? "" : wealthPlan.AccessToken.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@WealthPlanName", System.Data.SqlDbType.NVarChar, 200);
                            param.Value = wealthPlan.WealthPlanName == null ? "" : wealthPlan.WealthPlanName.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Liquidity_Cash", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Liquidity_Cash == null ? 0 : wealthPlan.Balance_Sheet_Asset_Liquidity_Cash;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Liquidity_Deposit", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit == null ? 0 : wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund == null ? 0 : wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Invest_Mutual_Fund", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Invest_Common_Stock", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Invest_Bond", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Bond == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Bond;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Invest_Property", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Property == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Property;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Invest_Other", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Other == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Other;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Personal_Home", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Personal_Home == null ? 0 : wealthPlan.Balance_Sheet_Asset_Personal_Home;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Personal_Car", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Personal_Car == null ? 0 : wealthPlan.Balance_Sheet_Asset_Personal_Car;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Asset_Personal_Other", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Asset_Personal_Other == null ? 0 : wealthPlan.Balance_Sheet_Asset_Personal_Other;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Liability_Short_Term_Credit_Card", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card == null ? 0 : wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Liability_Short_Term_Cash_Card", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card == null ? 0 : wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Liability_Long_Term_Home", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Liability_Long_Term_Home == null ? 0 : wealthPlan.Balance_Sheet_Liability_Long_Term_Home;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Liability_Long_Term_Car", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Liability_Long_Term_Car == null ? 0 : wealthPlan.Balance_Sheet_Liability_Long_Term_Car;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Balance_Sheet_Liability_Long_Term_Other", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Balance_Sheet_Liability_Long_Term_Other == null ? 0 : wealthPlan.Balance_Sheet_Liability_Long_Term_Other;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Income_Salary_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Income_Salary_Month == null ? 0 : wealthPlan.Income_Salary_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Income_Salary_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Income_Salary_Year == null ? 0 : wealthPlan.Income_Salary_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Income_Bonus_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Income_Bonus_Month == null ? 0 : wealthPlan.Income_Bonus_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Income_Bonus_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Income_Bonus_Year == null ? 0 : wealthPlan.Income_Bonus_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Income_Other_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Income_Other_Month == null ? 0 : wealthPlan.Income_Other_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Income_Other_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Income_Other_Year == null ? 0 : wealthPlan.Income_Other_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Insurance_Premium_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Insurance_Premium_Month == null ? 0 : wealthPlan.Expense_Fix_Insurance_Premium_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Insurance_Premium_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Insurance_Premium_Year == null ? 0 : wealthPlan.Expense_Fix_Insurance_Premium_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Home_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Home_Month == null ? 0 : wealthPlan.Expense_Fix_Home_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Home_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Home_Year == null ? 0 : wealthPlan.Expense_Fix_Home_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Car_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Car_Month == null ? 0 : wealthPlan.Expense_Fix_Car_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Car_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Car_Year == null ? 0 : wealthPlan.Expense_Fix_Car_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Credit_Card_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Credit_Card_Month == null ? 0 : wealthPlan.Expense_Fix_Credit_Card_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Credit_Card_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Credit_Card_Year == null ? 0 : wealthPlan.Expense_Fix_Credit_Card_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Car_Insurance_Premium_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Car_Insurance_Premium_Month == null ? 0 : wealthPlan.Expense_Fix_Car_Insurance_Premium_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Car_Insurance_Premium_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Car_Insurance_Premium_Year == null ? 0 : wealthPlan.Expense_Fix_Car_Insurance_Premium_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Social_Security_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Social_Security_Month == null ? 0 : wealthPlan.Expense_Fix_Social_Security_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Social_Security_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Social_Security_Year == null ? 0 : wealthPlan.Expense_Fix_Social_Security_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Other_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Other_Month == null ? 0 : wealthPlan.Expense_Fix_Other_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Fix_Other_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Fix_Other_Year == null ? 0 : wealthPlan.Expense_Fix_Other_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Four_Requisites_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Four_Requisites_Month == null ? 0 : wealthPlan.Expense_Vary_Four_Requisites_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Four_Requisites_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Four_Requisites_Year == null ? 0 : wealthPlan.Expense_Vary_Four_Requisites_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Telephone_Charge_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Telephone_Charge_Month == null ? 0 : wealthPlan.Expense_Vary_Telephone_Charge_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Telephone_Charge_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Telephone_Charge_Year == null ? 0 : wealthPlan.Expense_Vary_Telephone_Charge_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Travelling_Expense_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Travelling_Expense_Month == null ? 0 : wealthPlan.Expense_Vary_Travelling_Expense_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Travelling_Expense_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Travelling_Expense_Year == null ? 0 : wealthPlan.Expense_Vary_Travelling_Expense_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Living_Allowance_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Living_Allowance_Month == null ? 0 : wealthPlan.Expense_Vary_Living_Allowance_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Living_Allowance_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Living_Allowance_Year == null ? 0 : wealthPlan.Expense_Vary_Living_Allowance_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Donation_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Donation_Month == null ? 0 : wealthPlan.Expense_Vary_Donation_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Donation_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Donation_Year == null ? 0 : wealthPlan.Expense_Vary_Donation_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Other_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Other_Month == null ? 0 : wealthPlan.Expense_Vary_Other_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Vary_Other_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Vary_Other_Year == null ? 0 : wealthPlan.Expense_Vary_Other_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Saving_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Saving_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Saving_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Saving_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Saving_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Saving_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_RMF_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_RMF_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_LTF_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_LTF_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_PVF_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_PVF_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Mutual_Fund_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Mutual_Fund_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Common_Stock_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Common_Stock_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Other_Month", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Other_Year", System.Data.SqlDbType.Decimal);
                            param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year;
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            command.ExecuteNonQuery();
                            //--------------------------------  /Insert WealthPlan  ----------------
                            wealthPlanResponse.Message = "Success";
                            wealthPlanResponse.Status = "OK";
                        }
                        break;
                    case "U":
                        if (!hasRows)
                        {
                            wealthPlanResponse.Message = "Can not find this name";
                            wealthPlanResponse.Status = "Fail";
                        }
                        else
                        {
                            //--------------------------------  Update WealthPlan   ----------------
                            command.CommandType = CommandType.Text;
                        command.CommandText = "update SrvA_WealthPlan_Cloud set Flag = 0 where WealthPlanName = @WealthPlanName and AccessToken in (select AccessToken from SrvA_Login_Cloud where Mobile_No = @Mobile_No)";

                        command.Parameters.Clear();
                        param = null;

                        param = new SqlParameter("@WealthPlanName", System.Data.SqlDbType.NVarChar, 200);
                        param.Value = wealthPlan.WealthPlanName == null ? "" : wealthPlan.WealthPlanName.Trim();
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Mobile_No", System.Data.SqlDbType.NVarChar, 20);
                        param.Value = wealthPlan.Mobile_No == null ? "" : wealthPlan.Mobile_No.Trim();
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        command.ExecuteNonQuery();
                        //--------------------------------  Update WealthPlan   ----------------

                        //--------------------------------  Insert WealthPlan   ----------------
                        command.CommandType = CommandType.Text;
                        command.CommandText = "insert into SrvA_WealthPlan_Cloud(AccessToken," +
                            "WealthPlanName," +
                            "Balance_Sheet_Asset_Liquidity_Cash," +
                            "Balance_Sheet_Asset_Liquidity_Deposit," +
                            "Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund," +
                            "Balance_Sheet_Asset_Invest_Mutual_Fund," +
                            "Balance_Sheet_Asset_Invest_Common_Stock," +
                            "Balance_Sheet_Asset_Invest_Bond," +
                            "Balance_Sheet_Asset_Invest_Property," +
                            "Balance_Sheet_Asset_Invest_Other," +
                            "Balance_Sheet_Asset_Personal_Home," +
                            "Balance_Sheet_Asset_Personal_Car," +
                            "Balance_Sheet_Asset_Personal_Other," +
                            "Balance_Sheet_Liability_Short_Term_Credit_Card," +
                            "Balance_Sheet_Liability_Short_Term_Cash_Card," +
                            "Balance_Sheet_Liability_Long_Term_Home," +
                            "Balance_Sheet_Liability_Long_Term_Car," +
                            "Balance_Sheet_Liability_Long_Term_Other," +
                            "Income_Salary_Month," +
                            "Income_Salary_Year," +
                            "Income_Bonus_Month," +
                            "Income_Bonus_Year," +
                            "Income_Other_Month," +
                            "Income_Other_Year," +
                            "Expense_Fix_Insurance_Premium_Month," +
                            "Expense_Fix_Insurance_Premium_Year," +
                            "Expense_Fix_Home_Month," +
                            "Expense_Fix_Home_Year," +
                            "Expense_Fix_Car_Month," +
                            "Expense_Fix_Car_Year," +
                            "Expense_Fix_Credit_Card_Month," +
                            "Expense_Fix_Credit_Card_Year," +
                            "Expense_Fix_Car_Insurance_Premium_Month," +
                            "Expense_Fix_Car_Insurance_Premium_Year," +
                            "Expense_Fix_Social_Security_Month," +
                            "Expense_Fix_Social_Security_Year," +
                            "Expense_Fix_Other_Month," +
                            "Expense_Fix_Other_Year," +
                            "Expense_Vary_Four_Requisites_Month," +
                            "Expense_Vary_Four_Requisites_Year," +
                            "Expense_Vary_Telephone_Charge_Month," +
                            "Expense_Vary_Telephone_Charge_Year," +
                            "Expense_Vary_Travelling_Expense_Month," +
                            "Expense_Vary_Travelling_Expense_Year," +
                            "Expense_Vary_Living_Allowance_Month," +
                            "Expense_Vary_Living_Allowance_Year," +
                            "Expense_Vary_Donation_Month," +
                            "Expense_Vary_Donation_Year," +
                            "Expense_Vary_Other_Month," +
                            "Expense_Vary_Other_Year," +
                            "Expense_Saving_and_Investing_Saving_Month," +
                            "Expense_Saving_and_Investing_Saving_Year," +
                            "Expense_Saving_and_Investing_Investing_RMF_Month," +
                            "Expense_Saving_and_Investing_Investing_RMF_Year," +
                            "Expense_Saving_and_Investing_Investing_LTF_Month," +
                            "Expense_Saving_and_Investing_Investing_LTF_Year," +
                            "Expense_Saving_and_Investing_Investing_PVF_Month," +
                            "Expense_Saving_and_Investing_Investing_PVF_Year," +
                            "Expense_Saving_and_Investing_Investing_Mutual_Fund_Month," +
                            "Expense_Saving_and_Investing_Investing_Mutual_Fund_Year," +
                            "Expense_Saving_and_Investing_Investing_Common_Stock_Month," +
                            "Expense_Saving_and_Investing_Investing_Common_Stock_Year," +
                            "Expense_Saving_and_Investing_Investing_Other_Month," +
                            "Expense_Saving_and_Investing_Investing_Other_Year," +
                            "Dt_Gen,Flag) values(@AccessToken,@WealthPlanName,@Balance_Sheet_Asset_Liquidity_Cash,@Balance_Sheet_Asset_Liquidity_Deposit,@Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund,@Balance_Sheet_Asset_Invest_Mutual_Fund,@Balance_Sheet_Asset_Invest_Common_Stock,@Balance_Sheet_Asset_Invest_Bond,@Balance_Sheet_Asset_Invest_Property,@Balance_Sheet_Asset_Invest_Other,@Balance_Sheet_Asset_Personal_Home,@Balance_Sheet_Asset_Personal_Car,@Balance_Sheet_Asset_Personal_Other,@Balance_Sheet_Liability_Short_Term_Credit_Card,@Balance_Sheet_Liability_Short_Term_Cash_Card,@Balance_Sheet_Liability_Long_Term_Home,@Balance_Sheet_Liability_Long_Term_Car,@Balance_Sheet_Liability_Long_Term_Other,@Income_Salary_Month,@Income_Salary_Year,@Income_Bonus_Month,@Income_Bonus_Year,@Income_Other_Month,@Income_Other_Year,@Expense_Fix_Insurance_Premium_Month,@Expense_Fix_Insurance_Premium_Year,@Expense_Fix_Home_Month,@Expense_Fix_Home_Year,@Expense_Fix_Car_Month,@Expense_Fix_Car_Year,@Expense_Fix_Credit_Card_Month,@Expense_Fix_Credit_Card_Year,@Expense_Fix_Car_Insurance_Premium_Month,@Expense_Fix_Car_Insurance_Premium_Year,@Expense_Fix_Social_Security_Month,@Expense_Fix_Social_Security_Year,@Expense_Fix_Other_Month,@Expense_Fix_Other_Year,@Expense_Vary_Four_Requisites_Month,@Expense_Vary_Four_Requisites_Year,@Expense_Vary_Telephone_Charge_Month,@Expense_Vary_Telephone_Charge_Year,@Expense_Vary_Travelling_Expense_Month,@Expense_Vary_Travelling_Expense_Year,@Expense_Vary_Living_Allowance_Month,@Expense_Vary_Living_Allowance_Year,@Expense_Vary_Donation_Month,@Expense_Vary_Donation_Year,@Expense_Vary_Other_Month,@Expense_Vary_Other_Year,@Expense_Saving_and_Investing_Saving_Month,@Expense_Saving_and_Investing_Saving_Year,@Expense_Saving_and_Investing_Investing_RMF_Month,@Expense_Saving_and_Investing_Investing_RMF_Year,@Expense_Saving_and_Investing_Investing_LTF_Month,@Expense_Saving_and_Investing_Investing_LTF_Year,@Expense_Saving_and_Investing_Investing_PVF_Month,@Expense_Saving_and_Investing_Investing_PVF_Year,@Expense_Saving_and_Investing_Investing_Mutual_Fund_Month,@Expense_Saving_and_Investing_Investing_Mutual_Fund_Year,@Expense_Saving_and_Investing_Investing_Common_Stock_Month,@Expense_Saving_and_Investing_Investing_Common_Stock_Year,@Expense_Saving_and_Investing_Investing_Other_Month,@Expense_Saving_and_Investing_Investing_Other_Year,GETDATE(),1)";

                        command.Parameters.Clear();
                        param = null;

                        param = new SqlParameter("@AccessToken", System.Data.SqlDbType.NVarChar, -1);   //nvarchar(max)
                        param.Value = wealthPlan.AccessToken == null ? "" : wealthPlan.AccessToken.Trim();
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@WealthPlanName", System.Data.SqlDbType.NVarChar, 200);
                        param.Value = wealthPlan.WealthPlanName == null ? "" : wealthPlan.WealthPlanName.Trim();
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Liquidity_Cash", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Liquidity_Cash == null ? 0 : wealthPlan.Balance_Sheet_Asset_Liquidity_Cash;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Liquidity_Deposit", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit == null ? 0 : wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund == null ? 0 : wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Invest_Mutual_Fund", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Invest_Common_Stock", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Invest_Bond", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Bond == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Bond;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Invest_Property", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Property == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Property;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Invest_Other", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Invest_Other == null ? 0 : wealthPlan.Balance_Sheet_Asset_Invest_Other;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Personal_Home", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Personal_Home == null ? 0 : wealthPlan.Balance_Sheet_Asset_Personal_Home;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Personal_Car", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Personal_Car == null ? 0 : wealthPlan.Balance_Sheet_Asset_Personal_Car;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Asset_Personal_Other", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Asset_Personal_Other == null ? 0 : wealthPlan.Balance_Sheet_Asset_Personal_Other;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Liability_Short_Term_Credit_Card", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card == null ? 0 : wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Liability_Short_Term_Cash_Card", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card == null ? 0 : wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Liability_Long_Term_Home", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Liability_Long_Term_Home == null ? 0 : wealthPlan.Balance_Sheet_Liability_Long_Term_Home;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Liability_Long_Term_Car", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Liability_Long_Term_Car == null ? 0 : wealthPlan.Balance_Sheet_Liability_Long_Term_Car;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Balance_Sheet_Liability_Long_Term_Other", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Balance_Sheet_Liability_Long_Term_Other == null ? 0 : wealthPlan.Balance_Sheet_Liability_Long_Term_Other;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Income_Salary_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Income_Salary_Month == null ? 0 : wealthPlan.Income_Salary_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Income_Salary_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Income_Salary_Year == null ? 0 : wealthPlan.Income_Salary_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Income_Bonus_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Income_Bonus_Month == null ? 0 : wealthPlan.Income_Bonus_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Income_Bonus_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Income_Bonus_Year == null ? 0 : wealthPlan.Income_Bonus_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Income_Other_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Income_Other_Month == null ? 0 : wealthPlan.Income_Other_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Income_Other_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Income_Other_Year == null ? 0 : wealthPlan.Income_Other_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Insurance_Premium_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Insurance_Premium_Month == null ? 0 : wealthPlan.Expense_Fix_Insurance_Premium_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Insurance_Premium_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Insurance_Premium_Year == null ? 0 : wealthPlan.Expense_Fix_Insurance_Premium_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Home_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Home_Month == null ? 0 : wealthPlan.Expense_Fix_Home_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Home_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Home_Year == null ? 0 : wealthPlan.Expense_Fix_Home_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Car_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Car_Month == null ? 0 : wealthPlan.Expense_Fix_Car_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Car_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Car_Year == null ? 0 : wealthPlan.Expense_Fix_Car_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Credit_Card_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Credit_Card_Month == null ? 0 : wealthPlan.Expense_Fix_Credit_Card_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Credit_Card_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Credit_Card_Year == null ? 0 : wealthPlan.Expense_Fix_Credit_Card_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Car_Insurance_Premium_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Car_Insurance_Premium_Month == null ? 0 : wealthPlan.Expense_Fix_Car_Insurance_Premium_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Car_Insurance_Premium_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Car_Insurance_Premium_Year == null ? 0 : wealthPlan.Expense_Fix_Car_Insurance_Premium_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Social_Security_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Social_Security_Month == null ? 0 : wealthPlan.Expense_Fix_Social_Security_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Social_Security_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Social_Security_Year == null ? 0 : wealthPlan.Expense_Fix_Social_Security_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Other_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Other_Month == null ? 0 : wealthPlan.Expense_Fix_Other_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Fix_Other_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Fix_Other_Year == null ? 0 : wealthPlan.Expense_Fix_Other_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Four_Requisites_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Four_Requisites_Month == null ? 0 : wealthPlan.Expense_Vary_Four_Requisites_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Four_Requisites_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Four_Requisites_Year == null ? 0 : wealthPlan.Expense_Vary_Four_Requisites_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Telephone_Charge_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Telephone_Charge_Month == null ? 0 : wealthPlan.Expense_Vary_Telephone_Charge_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Telephone_Charge_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Telephone_Charge_Year == null ? 0 : wealthPlan.Expense_Vary_Telephone_Charge_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Travelling_Expense_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Travelling_Expense_Month == null ? 0 : wealthPlan.Expense_Vary_Travelling_Expense_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Travelling_Expense_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Travelling_Expense_Year == null ? 0 : wealthPlan.Expense_Vary_Travelling_Expense_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Living_Allowance_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Living_Allowance_Month == null ? 0 : wealthPlan.Expense_Vary_Living_Allowance_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Living_Allowance_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Living_Allowance_Year == null ? 0 : wealthPlan.Expense_Vary_Living_Allowance_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Donation_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Donation_Month == null ? 0 : wealthPlan.Expense_Vary_Donation_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Donation_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Donation_Year == null ? 0 : wealthPlan.Expense_Vary_Donation_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Other_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Other_Month == null ? 0 : wealthPlan.Expense_Vary_Other_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Vary_Other_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Vary_Other_Year == null ? 0 : wealthPlan.Expense_Vary_Other_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Saving_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Saving_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Saving_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Saving_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Saving_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Saving_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_RMF_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_RMF_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_LTF_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_LTF_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_PVF_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_PVF_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Mutual_Fund_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Mutual_Fund_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Common_Stock_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Common_Stock_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Other_Month", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        param = new SqlParameter("@Expense_Saving_and_Investing_Investing_Other_Year", System.Data.SqlDbType.Decimal);
                        param.Value = wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year == null ? 0 : wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year;
                        param.Direction = ParameterDirection.Input;
                        command.Parameters.Add(param);

                        command.ExecuteNonQuery();
                        //--------------------------------  /Insert WealthPlan  ----------------
                        wealthPlanResponse.Message = "Success";
                        wealthPlanResponse.Status = "OK";
                }
                        break;
                    case "D":
                        if (!hasRows)
                        {
                            wealthPlanResponse.Message = "Can not find this name";
                            wealthPlanResponse.Status = "Fail";
                        }
                        else
                        {
                            //--------------------------------  Delete WealthPlan   ----------------
                            command.CommandType = CommandType.Text;
                            command.CommandText = "update SrvA_WealthPlan_Cloud set Flag = 0 where WealthPlanName = @WealthPlanName and AccessToken in (select AccessToken from SrvA_Login_Cloud where Mobile_No = @Mobile_No)";

                            command.Parameters.Clear();
                            param = null;

                            param = new SqlParameter("@WealthPlanName", System.Data.SqlDbType.NVarChar, 200);
                            param.Value = wealthPlan.WealthPlanName == null ? "" : wealthPlan.WealthPlanName.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Mobile_No", System.Data.SqlDbType.NVarChar, 20);
                            param.Value = wealthPlan.Mobile_No == null ? "" : wealthPlan.Mobile_No.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            command.ExecuteNonQuery();
                            //--------------------------------  Delete WealthPlan   ----------------
                            wealthPlanResponse.Message = "Success";
                            wealthPlanResponse.Status = "OK";
                        }
                        break;
                    case "I":
                        if (!hasRows)
                        {
                            wealthPlanResponse.Message = "Data not found";
                            wealthPlanResponse.Status = "Fail";
                        }
                        else
                        {
                            List<WealthPlanInfo> WealthPlaninfo = new List<WealthPlanInfo>();

                            WealthPlanInfo wpi = null;
                            //--------------------------------  Info WealthPlan   ----------------
                            command.CommandType = CommandType.Text;
                            command.CommandText = "select * from SrvA_WealthPlan_Cloud where Flag = 1 and WealthPlanName = @WealthPlanName and AccessToken in (select AccessToken from SrvA_Login_Cloud where Mobile_No = @Mobile_No)";

                            command.Parameters.Clear();
                            param = null;

                            param = new SqlParameter("@WealthPlanName", System.Data.SqlDbType.NVarChar, 200);
                            param.Value = wealthPlan.WealthPlanName == null ? "" : wealthPlan.WealthPlanName.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            param = new SqlParameter("@Mobile_No", System.Data.SqlDbType.NVarChar, 20);
                            param.Value = wealthPlan.Mobile_No == null ? "" : wealthPlan.Mobile_No.Trim();
                            param.Direction = ParameterDirection.Input;
                            command.Parameters.Add(param);

                            mySQLReader = command.ExecuteReader();

                            while (mySQLReader.Read())
                            {
                                wpi = new WealthPlanInfo();

                                wealthPlan.Balance_Sheet_Asset_Liquidity_Cash = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Liquidity_Cash"));
                                wpi.Balance_Sheet_Asset_Liquidity_Cash = wealthPlan.Balance_Sheet_Asset_Liquidity_Cash;

                                wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Liquidity_Deposit"));
                                wpi.Balance_Sheet_Asset_Liquidity_Deposit = wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit;

                                wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund"));
                                wpi.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund = wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund;

                                wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Invest_Mutual_Fund"));
                                wpi.Balance_Sheet_Asset_Invest_Mutual_Fund = wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund;

                                wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Invest_Common_Stock"));
                                wpi.Balance_Sheet_Asset_Invest_Common_Stock = wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock;

                                wealthPlan.Balance_Sheet_Asset_Invest_Bond = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Invest_Bond"));
                                wpi.Balance_Sheet_Asset_Invest_Bond = wealthPlan.Balance_Sheet_Asset_Invest_Bond;

                                wealthPlan.Balance_Sheet_Asset_Invest_Property = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Invest_Property"));
                                wpi.Balance_Sheet_Asset_Invest_Property = wealthPlan.Balance_Sheet_Asset_Invest_Property;

                                wealthPlan.Balance_Sheet_Asset_Invest_Other = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Invest_Other"));
                                wpi.Balance_Sheet_Asset_Invest_Other = wealthPlan.Balance_Sheet_Asset_Invest_Other;

                                wealthPlan.Balance_Sheet_Asset_Personal_Home = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Personal_Home"));
                                wpi.Balance_Sheet_Asset_Personal_Home = wealthPlan.Balance_Sheet_Asset_Personal_Home;

                                wealthPlan.Balance_Sheet_Asset_Personal_Car = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Personal_Car"));
                                wpi.Balance_Sheet_Asset_Personal_Car = wealthPlan.Balance_Sheet_Asset_Personal_Car;

                                wealthPlan.Balance_Sheet_Asset_Personal_Other = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Asset_Personal_Other"));
                                wpi.Balance_Sheet_Asset_Personal_Other = wealthPlan.Balance_Sheet_Asset_Personal_Other;

                                wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Liability_Short_Term_Credit_Card"));
                                wpi.Balance_Sheet_Liability_Short_Term_Credit_Card = wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card;

                                wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Liability_Short_Term_Cash_Card"));
                                wpi.Balance_Sheet_Liability_Short_Term_Cash_Card = wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card;

                                wealthPlan.Balance_Sheet_Liability_Long_Term_Home = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Liability_Long_Term_Home"));
                                wpi.Balance_Sheet_Liability_Long_Term_Home = wealthPlan.Balance_Sheet_Liability_Long_Term_Home;

                                wealthPlan.Balance_Sheet_Liability_Long_Term_Car = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Liability_Long_Term_Car"));
                                wpi.Balance_Sheet_Liability_Long_Term_Car = wealthPlan.Balance_Sheet_Liability_Long_Term_Car;

                                wealthPlan.Balance_Sheet_Liability_Long_Term_Other = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Balance_Sheet_Liability_Long_Term_Other"));
                                wpi.Balance_Sheet_Liability_Long_Term_Other = wealthPlan.Balance_Sheet_Liability_Long_Term_Other;

                                wealthPlan.Income_Salary_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Income_Salary_Month"));
                                wpi.Income_Salary_Month = wealthPlan.Income_Salary_Month;

                                wealthPlan.Income_Salary_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Income_Salary_Year"));
                                wpi.Income_Salary_Year = wealthPlan.Income_Salary_Year;

                                wealthPlan.Income_Bonus_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Income_Bonus_Month"));
                                wpi.Income_Bonus_Month = wealthPlan.Income_Bonus_Month;

                                wealthPlan.Income_Bonus_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Income_Bonus_Year"));
                                wpi.Income_Bonus_Year = wealthPlan.Income_Bonus_Year;

                                wealthPlan.Income_Other_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Income_Other_Month"));
                                wpi.Income_Other_Month = wealthPlan.Income_Other_Month;

                                wealthPlan.Income_Other_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Income_Other_Year"));
                                wpi.Income_Other_Year = wealthPlan.Income_Other_Year;

                                wealthPlan.Expense_Fix_Insurance_Premium_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Insurance_Premium_Month"));
                                wpi.Expense_Fix_Insurance_Premium_Month = wealthPlan.Expense_Fix_Insurance_Premium_Month;

                                wealthPlan.Expense_Fix_Insurance_Premium_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Insurance_Premium_Year"));
                                wpi.Expense_Fix_Insurance_Premium_Year = wealthPlan.Expense_Fix_Insurance_Premium_Year;

                                wealthPlan.Expense_Fix_Home_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Home_Month"));
                                wpi.Expense_Fix_Home_Month = wealthPlan.Expense_Fix_Home_Month;

                                wealthPlan.Expense_Fix_Home_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Home_Year"));
                                wpi.Expense_Fix_Home_Year = wealthPlan.Expense_Fix_Home_Year;

                                wealthPlan.Expense_Fix_Car_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Car_Month"));
                                wpi.Expense_Fix_Car_Month = wealthPlan.Expense_Fix_Car_Month;

                                wealthPlan.Expense_Fix_Car_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Car_Year"));
                                wpi.Expense_Fix_Car_Year = wealthPlan.Expense_Fix_Car_Year;

                                wealthPlan.Expense_Fix_Credit_Card_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Credit_Card_Month"));
                                wpi.Expense_Fix_Credit_Card_Month = wealthPlan.Expense_Fix_Credit_Card_Month;

                                wealthPlan.Expense_Fix_Credit_Card_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Credit_Card_Year"));
                                wpi.Expense_Fix_Credit_Card_Year = wealthPlan.Expense_Fix_Credit_Card_Year;

                                wealthPlan.Expense_Fix_Car_Insurance_Premium_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Car_Insurance_Premium_Month"));
                                wpi.Expense_Fix_Car_Insurance_Premium_Month = wealthPlan.Expense_Fix_Car_Insurance_Premium_Month;

                                wealthPlan.Expense_Fix_Car_Insurance_Premium_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Car_Insurance_Premium_Year"));
                                wpi.Expense_Fix_Car_Insurance_Premium_Year = wealthPlan.Expense_Fix_Car_Insurance_Premium_Year;

                                wealthPlan.Expense_Fix_Social_Security_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Social_Security_Month"));
                                wpi.Expense_Fix_Social_Security_Month = wealthPlan.Expense_Fix_Social_Security_Month;

                                wealthPlan.Expense_Fix_Social_Security_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Social_Security_Year"));
                                wpi.Expense_Fix_Social_Security_Year = wealthPlan.Expense_Fix_Social_Security_Year;

                                wealthPlan.Expense_Fix_Other_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Other_Month"));
                                wpi.Expense_Fix_Other_Month = wealthPlan.Expense_Fix_Other_Month;

                                wealthPlan.Expense_Fix_Other_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Fix_Other_Year"));
                                wpi.Expense_Fix_Other_Year = wealthPlan.Expense_Fix_Other_Year;

                                wealthPlan.Expense_Vary_Four_Requisites_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Four_Requisites_Month"));
                                wpi.Expense_Vary_Four_Requisites_Month = wealthPlan.Expense_Vary_Four_Requisites_Month;

                                wealthPlan.Expense_Vary_Four_Requisites_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Four_Requisites_Year"));
                                wpi.Expense_Vary_Four_Requisites_Year = wealthPlan.Expense_Vary_Four_Requisites_Year;

                                wealthPlan.Expense_Vary_Telephone_Charge_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Telephone_Charge_Month"));
                                wpi.Expense_Vary_Telephone_Charge_Month = wealthPlan.Expense_Vary_Telephone_Charge_Month;

                                wealthPlan.Expense_Vary_Telephone_Charge_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Telephone_Charge_Year"));
                                wpi.Expense_Vary_Telephone_Charge_Year = wealthPlan.Expense_Vary_Telephone_Charge_Year;

                                wealthPlan.Expense_Vary_Travelling_Expense_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Travelling_Expense_Month"));
                                wpi.Expense_Vary_Travelling_Expense_Month = wealthPlan.Expense_Vary_Travelling_Expense_Month;

                                wealthPlan.Expense_Vary_Travelling_Expense_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Travelling_Expense_Year"));
                                wpi.Expense_Vary_Travelling_Expense_Year = wealthPlan.Expense_Vary_Travelling_Expense_Year;

                                wealthPlan.Expense_Vary_Living_Allowance_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Living_Allowance_Month"));
                                wpi.Expense_Vary_Living_Allowance_Month = wealthPlan.Expense_Vary_Living_Allowance_Month;

                                wealthPlan.Expense_Vary_Living_Allowance_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Living_Allowance_Year"));
                                wpi.Expense_Vary_Living_Allowance_Year = wealthPlan.Expense_Vary_Living_Allowance_Year;

                                wealthPlan.Expense_Vary_Donation_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Donation_Month"));
                                wpi.Expense_Vary_Donation_Month = wealthPlan.Expense_Vary_Donation_Month;

                                wealthPlan.Expense_Vary_Donation_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Donation_Year"));
                                wpi.Expense_Vary_Donation_Year = wealthPlan.Expense_Vary_Donation_Year;

                                wealthPlan.Expense_Vary_Other_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Other_Month"));
                                wpi.Expense_Vary_Other_Month = wealthPlan.Expense_Vary_Other_Month;

                                wealthPlan.Expense_Vary_Other_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Vary_Other_Year"));
                                wpi.Expense_Vary_Other_Year = wealthPlan.Expense_Vary_Other_Year;

                                wealthPlan.Expense_Saving_and_Investing_Saving_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Saving_Month"));
                                wpi.Expense_Saving_and_Investing_Saving_Month = wealthPlan.Expense_Saving_and_Investing_Saving_Month;

                                wealthPlan.Expense_Saving_and_Investing_Saving_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Saving_Year"));
                                wpi.Expense_Saving_and_Investing_Saving_Year = wealthPlan.Expense_Saving_and_Investing_Saving_Year;

                                wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_RMF_Month"));
                                wpi.Expense_Saving_and_Investing_Investing_RMF_Month = wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month;

                                wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_RMF_Year"));
                                wpi.Expense_Saving_and_Investing_Investing_RMF_Year = wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year;

                                wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_LTF_Month"));
                                wpi.Expense_Saving_and_Investing_Investing_LTF_Month = wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month;

                                wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_LTF_Year"));
                                wpi.Expense_Saving_and_Investing_Investing_LTF_Year = wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year;

                                wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_PVF_Month"));
                                wpi.Expense_Saving_and_Investing_Investing_PVF_Month = wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month;

                                wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_PVF_Year"));
                                wpi.Expense_Saving_and_Investing_Investing_PVF_Year = wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year;

                                wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_Mutual_Fund_Month"));
                                wpi.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month = wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month;

                                wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_Mutual_Fund_Year"));
                                wpi.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year = wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year;

                                wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_Common_Stock_Month"));
                                wpi.Expense_Saving_and_Investing_Investing_Common_Stock_Month = wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month;

                                wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_Common_Stock_Year"));
                                wpi.Expense_Saving_and_Investing_Investing_Common_Stock_Year = wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year;

                                wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_Other_Month"));
                                wpi.Expense_Saving_and_Investing_Investing_Other_Month = wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month;

                                wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year = mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Expense_Saving_and_Investing_Investing_Other_Year"));
                                wpi.Expense_Saving_and_Investing_Investing_Other_Year = wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year;

                                WealthPlaninfo.Add(wpi);
                            }
                            mySQLReader.Close();
                            //--------------------------------  Info WealthPlan   ----------------
                            wealthPlanResponse.Data = WealthPlaninfo;
                            wealthPlanResponse.Message = "Success";
                            wealthPlanResponse.Status = "OK";
                        }
                        break;
                    default:break;
                }   //  end switch


                //--------------------------- Calculate  --------------------------------
                Balance_Sheet_Asset_Liquidity = wealthPlan.Balance_Sheet_Asset_Liquidity_Cash + wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit + wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund;

                Balance_Sheet_Asset_Personal = wealthPlan.Balance_Sheet_Asset_Personal_Home + wealthPlan.Balance_Sheet_Asset_Personal_Car + wealthPlan.Balance_Sheet_Asset_Personal_Other;

                Balance_Sheet_Asset_Invest = wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund + wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock + wealthPlan.Balance_Sheet_Asset_Invest_Bond + wealthPlan.Balance_Sheet_Asset_Invest_Property + wealthPlan.Balance_Sheet_Asset_Invest_Other;

                Balance_Sheet_Liability_Short_Term = wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card + wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card;

                Balance_Sheet_Liability_Long_Term = wealthPlan.Balance_Sheet_Liability_Long_Term_Home + wealthPlan.Balance_Sheet_Liability_Long_Term_Car + wealthPlan.Balance_Sheet_Liability_Long_Term_Other;

                Total_Asset = Balance_Sheet_Asset_Liquidity + Balance_Sheet_Asset_Personal + Balance_Sheet_Asset_Invest;
                Total_Liability = Balance_Sheet_Liability_Short_Term + Balance_Sheet_Liability_Long_Term;
                Total_Wealth = Total_Asset - Total_Liability;

                Total_Income = (wealthPlan.Income_Salary_Month * 12) + wealthPlan.Income_Salary_Year + (wealthPlan.Income_Bonus_Month * 12) + wealthPlan.Income_Bonus_Year + (wealthPlan.Income_Other_Month * 12) + wealthPlan.Income_Other_Year;

                Expense_Fix = (wealthPlan.Expense_Fix_Insurance_Premium_Month * 12) + wealthPlan.Expense_Fix_Insurance_Premium_Year + (wealthPlan.Expense_Fix_Home_Month * 12) + wealthPlan.Expense_Fix_Home_Year + (wealthPlan.Expense_Fix_Car_Month * 12) + wealthPlan.Expense_Fix_Car_Year + (wealthPlan.Expense_Fix_Credit_Card_Month * 12) + wealthPlan.Expense_Fix_Credit_Card_Year + (wealthPlan.Expense_Fix_Car_Insurance_Premium_Month * 12) + wealthPlan.Expense_Fix_Car_Insurance_Premium_Year + (wealthPlan.Expense_Fix_Social_Security_Month * 12) + wealthPlan.Expense_Fix_Social_Security_Year + (wealthPlan.Expense_Fix_Other_Month * 12) + wealthPlan.Expense_Fix_Other_Year;

                Expense_Fix_Sub = (wealthPlan.Expense_Fix_Home_Month * 12) + wealthPlan.Expense_Fix_Home_Year + (wealthPlan.Expense_Fix_Car_Month * 12) + wealthPlan.Expense_Fix_Car_Year + (wealthPlan.Expense_Fix_Credit_Card_Month * 12) + wealthPlan.Expense_Fix_Credit_Card_Year;


                Expense_Saving_and_Investing = (wealthPlan.Expense_Saving_and_Investing_Saving_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Saving_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year;

                Expense_Vary = (wealthPlan.Expense_Vary_Four_Requisites_Month * 12) + wealthPlan.Expense_Vary_Four_Requisites_Year + (wealthPlan.Expense_Vary_Telephone_Charge_Month * 12) + wealthPlan.Expense_Vary_Telephone_Charge_Year + (wealthPlan.Expense_Vary_Travelling_Expense_Month * 12) + wealthPlan.Expense_Vary_Travelling_Expense_Year + (wealthPlan.Expense_Vary_Living_Allowance_Month * 12) + wealthPlan.Expense_Vary_Living_Allowance_Year + (wealthPlan.Expense_Vary_Donation_Month * 12) + wealthPlan.Expense_Vary_Donation_Year + (wealthPlan.Expense_Vary_Other_Month * 12) + wealthPlan.Expense_Vary_Other_Year;

                Total_Expense = Expense_Fix + Expense_Saving_and_Investing + Expense_Vary;

                Liquidity_Analysis_Result = Balance_Sheet_Asset_Liquidity / ((Expense_Fix + Expense_Saving_and_Investing + Expense_Vary) / 12);

                Ability_To_Pay_Short_Term_Debt = Balance_Sheet_Liability_Short_Term == 0 ? 0 : Balance_Sheet_Asset_Liquidity / Balance_Sheet_Liability_Short_Term; //0;// P81 / P91;

                Ability_To_Pay_Mid_Term_Debt = Total_Income == 0 ? 0 : Expense_Fix_Sub / Total_Income;

                Payment_Of_Debt_From_Income = Total_Asset == 0 ? 0 : Balance_Sheet_Asset_Invest / Total_Asset;

                Ratio_Liability_Per_Asset = Total_Asset == 0 ? 0 : Total_Liability / Total_Asset;

                Saving_Analysis_Result = Total_Income == 0 ? 0 : Expense_Saving_and_Investing / Total_Income;

                wealthPlanResponse.Total_Asset = Total_Asset;
                wealthPlanResponse.Total_Liability = Total_Liability;
                wealthPlanResponse.Total_Wealth = Total_Wealth;
                wealthPlanResponse.Total_Income = Total_Income;
                wealthPlanResponse.Total_Expense = Total_Expense;
                wealthPlanResponse.Liquidity_Analysis_Result = Math.Round(Liquidity_Analysis_Result, 4);
                wealthPlanResponse.Ability_To_Pay_Short_Term_Debt = Math.Round(Ability_To_Pay_Short_Term_Debt, 4);
                wealthPlanResponse.Ratio_Liability_Per_Asset = Math.Round(Ratio_Liability_Per_Asset, 4);
                wealthPlanResponse.Ability_To_Pay_Mid_Term_Debt = Math.Round(Ability_To_Pay_Mid_Term_Debt, 4);
                wealthPlanResponse.Payment_Of_Debt_From_Income = Math.Round(Payment_Of_Debt_From_Income, 4);
                wealthPlanResponse.Saving_Analysis_Result = Math.Round(Saving_Analysis_Result, 4);
                //--------------------------- /Calculate  --------------------------------

                //  end Main

                return wealthPlanResponse;
            }

            catch (Exception ex)
            {
                wealthPlanResponse.Message = ex.ToString();
                wealthPlanResponse.Status = "Fail";
                return wealthPlanResponse;
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