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

            decimal P81 = 0;
            decimal P82 = 0;
            decimal P83 = 0;
            decimal P91 = 0;
            decimal P92 = 0;
            decimal P101 = 0;
            decimal P104 = 0;
            decimal P105 = 0;

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
                    "Expense_Saving_and_Investing_Saving," +
                    "Expense_Saving_and_Investing_Investing_RMF," +
                    "Expense_Saving_and_Investing_Investing_LTF," +
                    "Expense_Saving_and_Investing_Investing_PVF," +
                    "Expense_Saving_and_Investing_Investing_Other," +
                    "Dt_Gen,Flag) values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,GETDATE(),1)";
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
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Saving", wealthPlan.Expense_Saving_and_Investing_Saving);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_RMF", wealthPlan.Expense_Saving_and_Investing_Investing_RMF);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_LTF", wealthPlan.Expense_Saving_and_Investing_Investing_LTF);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_PVF", wealthPlan.Expense_Saving_and_Investing_Investing_PVF);
                command.Parameters.AddWithValue("@Expense_Saving_and_Investing_Investing_Other", wealthPlan.Expense_Saving_and_Investing_Investing_Other);
                command.ExecuteNonQuery();
                //--------------------------------  /Insert WealthPlan  ----------------

                P81 = wealthPlan.Balance_Sheet_Asset_Liquidity_Cash + wealthPlan.Balance_Sheet_Asset_Liquidity_Deposit + wealthPlan.Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund;
                P82 = wealthPlan.Balance_Sheet_Asset_Personal_Home + wealthPlan.Balance_Sheet_Asset_Personal_Car + wealthPlan.Balance_Sheet_Asset_Personal_Other;
                P83 = wealthPlan.Balance_Sheet_Asset_Invest_Mutual_Fund + wealthPlan.Balance_Sheet_Asset_Invest_Common_Stock + wealthPlan.Balance_Sheet_Asset_Invest_Bond + wealthPlan.Balance_Sheet_Asset_Invest_Property + wealthPlan.Balance_Sheet_Asset_Invest_Other;
                P91 = wealthPlan.Balance_Sheet_Liability_Short_Term_Credit_Card + wealthPlan.Balance_Sheet_Liability_Short_Term_Cash_Card;
                P92 = wealthPlan.Balance_Sheet_Liability_Long_Term_Home + wealthPlan.Balance_Sheet_Liability_Long_Term_Car + wealthPlan.Balance_Sheet_Liability_Long_Term_Other;

                Total_Asset = P81 + P82 + P83;
                Total_Liability = P91 + P92;
                Total_Wealth = Total_Asset - Total_Liability;
                Total_Income = (wealthPlan.Income_Salary_Year == 0? wealthPlan.Income_Salary_Month * 12 : wealthPlan.Income_Salary_Year) + (wealthPlan.Income_Bonus_Year == 0? wealthPlan.Income_Bonus_Month * 12 : wealthPlan.Income_Bonus_Year) + (wealthPlan.Income_Other_Year == 0? wealthPlan.Income_Other_Month * 12 : wealthPlan.Income_Other_Year);

                P101 = (wealthPlan.Expense_Fix_Insurance_Premium_Year == 0 ? wealthPlan.Expense_Fix_Insurance_Premium_Month * 12 : wealthPlan.Expense_Fix_Insurance_Premium_Year) + (wealthPlan.Expense_Fix_Home_Year == 0? wealthPlan.Expense_Fix_Home_Month * 12 : wealthPlan.Expense_Fix_Home_Year) + (wealthPlan.Expense_Fix_Car_Year == 0? wealthPlan.Expense_Fix_Car_Month * 12: wealthPlan.Expense_Fix_Car_Year) + (wealthPlan.Expense_Fix_Other_Year == 0?  wealthPlan.Expense_Fix_Other_Month * 12 : wealthPlan.Expense_Fix_Other_Year);
                P104 = wealthPlan.Expense_Saving_and_Investing_Saving + wealthPlan.Expense_Saving_and_Investing_Investing_RMF + wealthPlan.Expense_Saving_and_Investing_Investing_LTF + wealthPlan.Expense_Saving_and_Investing_Investing_PVF + wealthPlan.Expense_Saving_and_Investing_Investing_Other;
                P105 = (wealthPlan.Expense_Vary_Four_Requisites_Year == 0 ? wealthPlan.Expense_Vary_Four_Requisites_Month * 12 : wealthPlan.Expense_Vary_Four_Requisites_Year) + (wealthPlan.Expense_Vary_Telephone_Charge_Year == 0? wealthPlan.Expense_Vary_Telephone_Charge_Month * 12 : wealthPlan.Expense_Vary_Telephone_Charge_Year) + (wealthPlan.Expense_Vary_Travelling_Expense_Year == 0? wealthPlan.Expense_Vary_Travelling_Expense_Month * 12 : wealthPlan.Expense_Vary_Travelling_Expense_Year) + (wealthPlan.Expense_Vary_Living_Allowance_Year == 0? wealthPlan.Expense_Vary_Living_Allowance_Month * 12 : wealthPlan.Expense_Vary_Living_Allowance_Year) + (wealthPlan.Expense_Vary_Donation_Year == 0? wealthPlan.Expense_Vary_Donation_Month * 12 : wealthPlan.Expense_Vary_Donation_Year) + (wealthPlan.Expense_Vary_Other_Year == 0? wealthPlan.Expense_Vary_Other_Month * 12 : wealthPlan.Expense_Vary_Other_Year);
                Total_Expense = P101 + P104 + P105;

                Liquidity_Analysis_Result = P81 / Total_Expense;
                Ability_To_Pay_Short_Term_Debt = P81 / P91;
                Ability_To_Pay_Mid_Term_Debt = Total_Expense / P82;
                Payment_Of_Debt_From_Income = P101 / Total_Income;
                //Saving_Analysis_Result = (P81 + P83) / P82;
                Saving_Analysis_Result = P104 / Total_Income;

                wealthPlanResponse.Total_Asset = Total_Asset;
                wealthPlanResponse.Total_Liability = Total_Liability;
                wealthPlanResponse.Total_Wealth = Total_Wealth;
                wealthPlanResponse.Total_Income = Total_Income;
                wealthPlanResponse.Total_Expense = Total_Expense;
                wealthPlanResponse.Liquidity_Analysis_Result = Liquidity_Analysis_Result;
                wealthPlanResponse.Ability_To_Pay_Short_Term_Debt = Ability_To_Pay_Short_Term_Debt;
                wealthPlanResponse.Ability_To_Pay_Mid_Term_Debt = Ability_To_Pay_Mid_Term_Debt;
                wealthPlanResponse.Payment_Of_Debt_From_Income = Payment_Of_Debt_From_Income;
                wealthPlanResponse.Saving_Analysis_Result = Saving_Analysis_Result;
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