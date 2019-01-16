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
using System.Configuration;

namespace WebAPI06Application.Services
{
    public class UserService
    {
        public User_PIN ValidateUser(string Mobile_No, string PIN_No)
        {
            // Here you can write the code to validate
            // User from database and return accordingly
            // To test we use dummy list here
            var userList = GetUserList(Mobile_No, PIN_No);
            var user = userList.FirstOrDefault(x => x.PIN == PIN_No);
            //var user = userList.FirstOrDefault(x => x.Mobile_No == Mobile_No && x.OTP_No == OTP_No);
            return user;
        }

        public List<User_PIN> GetUserList(string Mobile_No, string PIN_No)
        {
            // Create the list of user and return
            /*
            var u = new User();
            u.Id = 1;
            u.Email = "supawet.muennoie@gmail.com";
            u.Username = "supawet";
            u.Password = "123456";
            var l = new List<User>();
            l.Add(u);
            */
            //-------------DB Connect
            OleDbConnection conn = null;
            OleDbCommand command = null;
            OleDbDataReader mySQLReader = null;

            var u = new User_PIN();
            var l = new List<User_PIN>();

            var hash = System.Security.Cryptography.SHA512.Create();
            try
            {
                string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString; ;
                conn = new OleDbConnection(myConnectionString);

                conn.Open();

                command = new OleDbCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                command.CommandType = CommandType.Text;
                command.CommandText = "select PIN from SrvA_PIN_Cloud where Mobile_No = ? and PIN = ? and Flag = 1";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Mobile_No", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Mobile_No.Trim())));
                command.Parameters.AddWithValue("@PIN", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PIN_No.Trim())));
                //command.Parameters.AddWithValue("@PIN_No", PIN_No.Trim());
                //command.Parameters.AddWithValue("@Mobile_No", "HASHBYTES('SHA2_512', " + Mobile_No.Trim() + ")");
                mySQLReader = command.ExecuteReader();

                ArrayList rbDateArray = new ArrayList();
                Byte[] buffer = new Byte[128];

                while (mySQLReader.Read())
                {
                    //rbDateArray.Add(mySQLReader.GetDateTime(mySQLReader.GetOrdinal("RBDATE")).ToString("yyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US")));
                    //u.Id = mySQLReader.GetBytes(mySQLReader.GetOrdinal("ID_Card"));
                    //mySQLReader.GetBytes(mySQLReader.GetOrdinal("ID_Card"), 0, buffer, 0, 128);
                    //u.Id = System.Text.Encoding.UTF8.GetString(buffer);
                    u.PIN = PIN_No.Trim();// mySQLReader.GetString(mySQLReader.GetOrdinal("PIN"));
                    //u.ID_Card = mySQLReader.GetString(mySQLReader.GetOrdinal("ID_Card"));
                    //u.Password = mySQLReader.GetString(mySQLReader.GetOrdinal("PIN"));
                    l.Add(u);
                }
                mySQLReader.Close();
                //-------------DB Connect
            }
            catch (SqlException ex)
            {
                throw ex;
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

            return l;
        }
    }
}