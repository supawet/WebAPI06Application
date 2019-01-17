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
    public class WealthPlanPersistance
    {
        //public ArrayList GetWealthPlan(WealthPlan wealthPlan)
        public WealthPlanResponse GetWealthPlan(WealthPlan wealthPlan)
        {
            OleDbConnection conn = null;
            OleDbCommand command = null;
            OleDbDataReader mySQLReader = null;

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
                conn = new OleDbConnection(myConnectionString);

                conn.Open();

                command = new OleDbCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                //--------------------------------  Insert Log   ----------------
                command.CommandType = CommandType.Text;
                command.CommandText = "insert into SrvA_Log_Cloud(AccessToken,AccessModule,Dt_Gen,Flag) values(?,'WealthPlan',GETDATE(),1)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@AccessToken", wealthPlan.AccessToken);
                command.ExecuteNonQuery();
                //--------------------------------  /Insert Log  ----------------

                //--------------------------------  Insert WealthPlan   ----------------
                command.CommandType = CommandType.Text;
                command.CommandText = "insert into SrvA_WealthPlan_Cloud(AccessToken," +
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
                    "Dt_Gen,Flag) values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,GETDATE(),1)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@AccessToken", wealthPlan.AccessToken);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Liquidity_Cash", wealthPlan.Balance_Sheet_Asset_Liquidity_Cash);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Liquidity_Deposit", wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund", wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Invest_Mutual_Fund", wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Invest_Common_Stock", wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Invest_Bond", wealthPlan.Balance_Sheet_Asset_Invest_Bond);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Invest_Property", wealthPlan.Balance_Sheet_Asset_Invest_Property);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Invest_Other", wealthPlan.Balance_Sheet_Asset_Invest_Other);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Personal_Home", wealthPlan.Balance_Sheet_Asset_Personal_Home);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Personal_Car", wealthPlan.Balance_Sheet_Asset_Personal_Car);
                command.Parameters.AddWithValue("@Balance_Sheet_Asset_Personal_Other", wealthPlan.Balance_Sheet_Asset_Personal_Other);
                command.Parameters.AddWithValue("@Balance_Sheet_Liability_Short_Term_Credit_Card", wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card);
                command.Parameters.AddWithValue("@Balance_Sheet_Liability_Short_Term_Cash_Card", wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card);
                command.Parameters.AddWithValue("@Balance_Sheet_Liability_Long_Term_Home", wealthPlan.Balance_Sheet_Liability_Long_Term_Home);
                command.Parameters.AddWithValue("@Balance_Sheet_Liability_Long_Term_Car", wealthPlan.Balance_Sheet_Liability_Long_Term_Car);
                command.Parameters.AddWithValue("@Balance_Sheet_Liability_Long_Term_Other", wealthPlan.Balance_Sheet_Liability_Long_Term_Other);
                command.Parameters.AddWithValue("@Income_Salary_Month", wealthPlan.Income_Salary_Month);
                command.Parameters.AddWithValue("@Income_Salary_Year", wealthPlan.Income_Salary_Year);
                command.Parameters.AddWithValue("@Income_Bonus_Month", wealthPlan.Income_Bonus_Month);
                command.Parameters.AddWithValue("@Income_Bonus_Year", wealthPlan.Income_Bonus_Year);
                command.Parameters.AddWithValue("@Income_Other_Month", wealthPlan.Income_Other_Month);
                command.Parameters.AddWithValue("@Income_Other_Year", wealthPlan.Income_Other_Year);
                command.Parameters.AddWithValue("@Expense_Fix_Insurance_Premium_Month", wealthPlan.Expense_Fix_Insurance_Premium_Month);
                command.Parameters.AddWithValue("@Expense_Fix_Insurance_Premium_Year", wealthPlan.Expense_Fix_Insurance_Premium_Year);
                command.Parameters.AddWithValue("@Expense_Fix_Home_Month", wealthPlan.Expense_Fix_Home_Month);
                command.Parameters.AddWithValue("@Expense_Fix_Home_Year", wealthPlan.Expense_Fix_Home_Year);
                command.Parameters.AddWithValue("@Expense_Fix_Car_Month", wealthPlan.Expense_Fix_Car_Month);
                command.Parameters.AddWithValue("@Expense_Fix_Car_Year", wealthPlan.Expense_Fix_Car_Year);
                command.Parameters.AddWithValue("@Expense_Fix_Credit_Card_Month", wealthPlan.Expense_Fix_Credit_Card_Month);
                command.Parameters.AddWithValue("@Expense_Fix_Credit_Card_Year", wealthPlan.Expense_Fix_Credit_Card_Year);
                command.Parameters.AddWithValue("@Expense_Fix_Car_Insurance_Premium_Month", wealthPlan.Expense_Fix_Car_Insurance_Premium_Month);
                command.Parameters.AddWithValue("@Expense_Fix_Car_Insurance_Premium_Year", wealthPlan.Expense_Fix_Car_Insurance_Premium_Year);
                command.Parameters.AddWithValue("@Expense_Fix_Social_Security_Month", wealthPlan.Expense_Fix_Social_Security_Month);
                command.Parameters.AddWithValue("@Expense_Fix_Social_Security_Year", wealthPlan.Expense_Fix_Social_Security_Year);
                command.Parameters.AddWithValue("@Expense_Fix_Other_Month", wealthPlan.Expense_Fix_Other_Month);
                command.Parameters.AddWithValue("@Expense_Fix_Other_Year", wealthPlan.Expense_Fix_Other_Year);
                command.Parameters.AddWithValue("@Expense_Vary_Four_Requisites_Month", wealthPlan.Expense_Vary_Four_Requisites_Month);
                command.Parameters.AddWithValue("@Expense_Vary_Four_Requisites_Year", wealthPlan.Expense_Vary_Four_Requisites_Year);
                command.Parameters.AddWithValue("@Expense_Vary_Telephone_Charge_Month", wealthPlan.Expense_Vary_Telephone_Charge_Month);
                command.Parameters.AddWithValue("@Expense_Vary_Telephone_Charge_Year", wealthPlan.Expense_Vary_Telephone_Charge_Year);
                command.Parameters.AddWithValue("@Expense_Vary_Travelling_Expense_Month", wealthPlan.Expense_Vary_Travelling_Expense_Month);
                command.Parameters.AddWithValue("@Expense_Vary_Travelling_Expense_Year", wealthPlan.Expense_Vary_Travelling_Expense_Year);
                command.Parameters.AddWithValue("@Expense_Vary_Living_Allowance_Month", wealthPlan.Expense_Vary_Living_Allowance_Month);
                command.Parameters.AddWithValue("@Expense_Vary_Living_Allowance_Year", wealthPlan.Expense_Vary_Living_Allowance_Year);
                command.Parameters.AddWithValue("@Expense_Vary_Donation_Month", wealthPlan.Expense_Vary_Donation_Month);
                command.Parameters.AddWithValue("@Expense_Vary_Donation_Year", wealthPlan.Expense_Vary_Donation_Year);
                command.Parameters.AddWithValue("@Expense_Vary_Other_Month", wealthPlan.Expense_Vary_Other_Month);
                command.Parameters.AddWithValue("@Expense_Vary_Other_Year", wealthPlan.Expense_Vary_Other_Year);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Saving_Month", wealthPlan.Expense_Saving_and_Investing_Saving_Month);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Saving_Year", wealthPlan.Expense_Saving_and_Investing_Saving_Year);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_RMF_Month", wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_RMF_Year", wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_LTF_Month", wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_LTF_Year", wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_PVF_Month", wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_PVF_Year", wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_Mutual_Fund_Month", wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_Mutual_Fund_Year", wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_Common_Stock_Month", wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_Common_Stock_Year", wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_Other_Month", wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_Other_Year", wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year);
                command.ExecuteNonQuery();
                //--------------------------------  /Insert WealthPlan  ----------------

                Balance_Sheet_Asset_Liquidity = wealthPlan.Balance_Sheet_Asset_Liquidity_Cash + wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit + wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund;

                Balance_Sheet_Asset_Personal = wealthPlan.Balance_Sheet_Asset_Personal_Home + wealthPlan.Balance_Sheet_Asset_Personal_Car + wealthPlan.Balance_Sheet_Asset_Personal_Other;

                Balance_Sheet_Asset_Invest = wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund + wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock + wealthPlan.Balance_Sheet_Asset_Invest_Bond + wealthPlan.Balance_Sheet_Asset_Invest_Property + wealthPlan.Balance_Sheet_Asset_Invest_Other;

                Balance_Sheet_Liability_Short_Term = wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card + wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card;

                Balance_Sheet_Liability_Long_Term = wealthPlan.Balance_Sheet_Liability_Long_Term_Home + wealthPlan.Balance_Sheet_Liability_Long_Term_Car + wealthPlan.Balance_Sheet_Liability_Long_Term_Other;

                Total_Asset = Balance_Sheet_Asset_Liquidity + Balance_Sheet_Asset_Personal + Balance_Sheet_Asset_Invest;
                Total_Liability = Balance_Sheet_Liability_Short_Term + Balance_Sheet_Liability_Long_Term;
                Total_Wealth = Total_Asset - Total_Liability;

                Total_Income = (wealthPlan.Income_Salary_Month * 12) + wealthPlan.Income_Salary_Year + (wealthPlan.Income_Bonus_Month * 12) + wealthPlan.Income_Bonus_Year + (wealthPlan.Income_Other_Month * 12) + wealthPlan.Income_Other_Year;

                Expense_Fix = (wealthPlan.Expense_Fix_Insurance_Premium_Month * 12) + wealthPlan.Expense_Fix_Insurance_Premium_Year + (wealthPlan.Expense_Fix_Home_Month*12) + wealthPlan.Expense_Fix_Home_Year +(wealthPlan.Expense_Fix_Car_Month*12) + wealthPlan.Expense_Fix_Car_Year + (wealthPlan.Expense_Fix_Credit_Card_Month * 12) + wealthPlan.Expense_Fix_Credit_Card_Year + (wealthPlan.Expense_Fix_Car_Insurance_Premium_Month * 12) + wealthPlan.Expense_Fix_Car_Insurance_Premium_Year + (wealthPlan.Expense_Fix_Social_Security_Month * 12) + wealthPlan.Expense_Fix_Social_Security_Year + (wealthPlan.Expense_Fix_Other_Month*12) + wealthPlan.Expense_Fix_Other_Year;

                Expense_Fix_Sub = (wealthPlan.Expense_Fix_Home_Month * 12) + wealthPlan.Expense_Fix_Home_Year + (wealthPlan.Expense_Fix_Car_Month * 12) + wealthPlan.Expense_Fix_Car_Year + (wealthPlan.Expense_Fix_Credit_Card_Month * 12) + wealthPlan.Expense_Fix_Credit_Card_Year;


                Expense_Saving_and_Investing = (wealthPlan.Expense_Saving_and_Investing_Saving_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Saving_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_RMF_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_LTF_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_PVF_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_Mutual_Fund_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_Common_Stock_Year + (wealthPlan.Expense_Saving_and_Investing_Investing_Other_Month * 12) + wealthPlan.Expense_Saving_and_Investing_Investing_Other_Year;

                Expense_Vary = (wealthPlan.Expense_Vary_Four_Requisites_Month * 12) + wealthPlan.Expense_Vary_Four_Requisites_Year + (wealthPlan.Expense_Vary_Telephone_Charge_Month * 12) + wealthPlan.Expense_Vary_Telephone_Charge_Year +  (wealthPlan.Expense_Vary_Travelling_Expense_Month *12) + wealthPlan.Expense_Vary_Travelling_Expense_Year + (wealthPlan.Expense_Vary_Living_Allowance_Month * 12) + wealthPlan.Expense_Vary_Living_Allowance_Year +(wealthPlan.Expense_Vary_Donation_Month * 12) +  wealthPlan.Expense_Vary_Donation_Year + (wealthPlan.Expense_Vary_Other_Month * 12) + wealthPlan.Expense_Vary_Other_Year;

                Total_Expense = Expense_Fix + Expense_Saving_and_Investing + Expense_Vary;

                Liquidity_Analysis_Result = Balance_Sheet_Asset_Liquidity / ((Expense_Fix + Expense_Saving_and_Investing + Expense_Vary) / 12 );

                Ability_To_Pay_Short_Term_Debt = Balance_Sheet_Liability_Short_Term == 0? 0 : Balance_Sheet_Asset_Liquidity / Balance_Sheet_Liability_Short_Term; //0;// P81 / P91;

                Ability_To_Pay_Mid_Term_Debt = Total_Income == 0? 0 : Expense_Fix_Sub / Total_Income;

                Payment_Of_Debt_From_Income = Total_Asset == 0? 0 : Balance_Sheet_Asset_Invest / Total_Asset;

                Ratio_Liability_Per_Asset = Total_Asset == 0 ? 0 : Total_Liability / Total_Asset;

                Saving_Analysis_Result = Total_Income == 0 ? 0 : Expense_Saving_and_Investing / Total_Income;

                wealthPlanResponse.Total_Asset = Total_Asset;
                wealthPlanResponse.Total_Liability = Total_Liability;
                wealthPlanResponse.Total_Wealth = Total_Wealth;
                wealthPlanResponse.Total_Income = Total_Income;
                wealthPlanResponse.Total_Expense = Total_Expense;
                wealthPlanResponse.Liquidity_Analysis_Result = Math.Round(Liquidity_Analysis_Result,4);
                wealthPlanResponse.Ability_To_Pay_Short_Term_Debt = Math.Round(Ability_To_Pay_Short_Term_Debt, 4);
                wealthPlanResponse.Ratio_Liability_Per_Asset = Math.Round(Ratio_Liability_Per_Asset, 4);
                wealthPlanResponse.Ability_To_Pay_Mid_Term_Debt = Math.Round(Ability_To_Pay_Mid_Term_Debt, 4);
                wealthPlanResponse.Payment_Of_Debt_From_Income = Math.Round(Payment_Of_Debt_From_Income, 4);
                wealthPlanResponse.Saving_Analysis_Result = Math.Round(Saving_Analysis_Result, 4);
                wealthPlanResponse.Message = "Success";
                wealthPlanResponse.Status = "OK";
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