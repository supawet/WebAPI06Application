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
    public class LoginPersistance
    {
        //public ArrayList GetLogin(Login login)
        public LoginResponse GetLogin(Login login)
        {
            OleDbConnection conn = null;
            OleDbCommand command = null;
            OleDbDataReader mySQLReader = null;

            var hash = System.Security.Cryptography.SHA512.Create();

            LoginResponse loginResponse = new LoginResponse();
            loginResponse.Message = "Not Found";
            loginResponse.Status = "Fail";

            bool hasRows = false;

            try
            {
                string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString; ;
                conn = new OleDbConnection(myConnectionString);

                conn.Open();

                command = new OleDbCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                //--------------------------------  Check before Insert   ----------------
                //อย่าลืมเพิ่มเงื่อนไขการดูเวลาหมดอายุด้วย
                command.CommandText = "select PIN from SrvA_PIN_Cloud where Mobile_No = ? and PIN = ? and Flag = 1";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Mobile_No", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(login.Mobile_No.Trim())));
                command.Parameters.AddWithValue("@PIN", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(login.PIN.Trim())));
                mySQLReader = command.ExecuteReader();

                if (mySQLReader.HasRows) hasRows = true;
                mySQLReader.Close();

                if (hasRows)
                {
                    TokenService tokenService = new TokenService();
                    var token = tokenService.GetToken(login.Mobile_No.Trim().ToString(), login.PIN.Trim().ToString());
                    //var token = GetToken("0813963651", "315709");

                    //--------------------------------  Insert Login   ----------------
                    command.CommandType = CommandType.Text;
                    command.CommandText = "insert into SrvA_Login_Cloud(Mobile_No,AccessToken,Dt_Gen,Flag) values(?,?,GETDATE(),1)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Mobile_No", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(login.Mobile_No.Trim())));
                    command.Parameters.AddWithValue("@AccessToken", token);
                    command.ExecuteNonQuery();
                    //--------------------------------  /Insert Login  ----------------

                    loginResponse.AccessToken = token;
                    loginResponse.Message = "Success";
                    loginResponse.Status = "OK";
                }
                return loginResponse;
            }

            catch (Exception ex)
            {
                loginResponse.Message = ex.ToString();
                loginResponse.Status = "Fail";
                return loginResponse;
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