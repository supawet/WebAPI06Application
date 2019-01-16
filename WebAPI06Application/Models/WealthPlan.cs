using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI06Application.Models
{
    public class WealthPlan
    {
        public string AccessToken { get; set; }
        public decimal Balance_Sheet_Asset_Liquidity_Cash { get; set; }
        public decimal Balance_Sheet_Asset_Liquidity_Deposit { get; set; }
        public decimal Balance_Sheet_Asset_Liquidity_Fixed_Income_Fund { get; set; }
        public decimal Balance_Sheet_Asset_Invest_Mutual_Fund { get; set; }
        public decimal Balance_Sheet_Asset_Invest_Common_Stock { get; set; }
        public decimal Balance_Sheet_Asset_Invest_Bond { get; set; }
        public decimal Balance_Sheet_Asset_Invest_Property { get; set; }
        public decimal Balance_Sheet_Asset_Invest_Other { get; set; }
        public decimal Balance_Sheet_Asset_Personal_Home { get; set; }
        public decimal Balance_Sheet_Asset_Personal_Car { get; set; }
        public decimal Balance_Sheet_Asset_Personal_Other { get; set; }
        public decimal Balance_Sheet_Liability_Short_Term_Credit_Card { get; set; }
        public decimal Balance_Sheet_Liability_Short_Term_Cash_Card { get; set; }
        public decimal Balance_Sheet_Liability_Long_Term_Home { get; set; }
        public decimal Balance_Sheet_Liability_Long_Term_Car { get; set; }
        public decimal Balance_Sheet_Liability_Long_Term_Other { get; set; }
        public decimal Income_Salary_Month { get; set; }
        public decimal Income_Salary_Year { get; set; }
        public decimal Income_Bonus_Month { get; set; }
        public decimal Income_Bonus_Year { get; set; }
        public decimal Income_Other_Month { get; set; }
        public decimal Income_Other_Year { get; set; }
        public decimal Expense_Fix_Insurance_Premium_Month { get; set; }
        public decimal Expense_Fix_Insurance_Premium_Year { get; set; }
        public decimal Expense_Fix_Home_Month { get; set; }
        public decimal Expense_Fix_Home_Year { get; set; }
        public decimal Expense_Fix_Car_Month { get; set; }
        public decimal Expense_Fix_Car_Year { get; set; }
        public decimal Expense_Fix_Other_Month { get; set; }
        public decimal Expense_Fix_Other_Year { get; set; }
        public decimal Expense_Vary_Four_Requisites_Month { get; set; }
        public decimal Expense_Vary_Four_Requisites_Year { get; set; }
        public decimal Expense_Vary_Telephone_Charge_Month { get; set; }
        public decimal Expense_Vary_Telephone_Charge_Year { get; set; }
        public decimal Expense_Vary_Travelling_Expense_Month { get; set; }
        public decimal Expense_Vary_Travelling_Expense_Year { get; set; }
        public decimal Expense_Vary_Living_Allowance_Month { get; set; }
        public decimal Expense_Vary_Living_Allowance_Year { get; set; }
        public decimal Expense_Vary_Donation_Month { get; set; }
        public decimal Expense_Vary_Donation_Year { get; set; }
        public decimal Expense_Vary_Other_Month { get; set; }
        public decimal Expense_Vary_Other_Year { get; set; }
        public decimal Expense_Saving_and_Investing_Saving { get; set; }
        public decimal Expense_Saving_and_Investing_Investing_RMF { get; set; }
        public decimal Expense_Saving_and_Investing_Investing_LTF { get; set; }
        public decimal Expense_Saving_and_Investing_Investing_PVF { get; set; }
        public decimal Expense_Saving_and_Investing_Investing_Other { get; set; }
    }
    public class WealthPlanResponse
    {
        public decimal Total_Asset { get; set; }
        public decimal Total_Liability { get; set; }
        public decimal Total_Wealth { get; set; }
        public decimal Total_Income { get; set; }
        public decimal Total_Expense { get; set; }
        public decimal Liquidity_Analysis_Result { get; set; }
        public decimal Ability_To_Pay_Short_Term_Debt { get; set; }
        public decimal Ability_To_Pay_Mid_Term_Debt { get; set; }
        public decimal Payment_Of_Debt_From_Income { get; set; }
        public decimal Saving_Analysis_Result { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}