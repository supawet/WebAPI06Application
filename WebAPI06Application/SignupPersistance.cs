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

using Twilio;
using Twilio.Rest.Api.V2010.Account;

using System.Security.Cryptography;
using System.Web.Security;

namespace WebAPI06Application
{
    public class SignupPersistance
    {
        //public ArrayList getSignup(Signup signup)
        public SignupResponse GetSignup(Signup signup)
        {
            OleDbConnection conn = null;
            OleDbCommand command = null;
            OleDbDataReader mySQLReader = null;

            var hash = System.Security.Cryptography.SHA512.Create();

            SignupResponse signupResponse = new SignupResponse();
            signupResponse.Message = "Not Found";
            signupResponse.Status = "Fail";

            bool hasRows = false;
            bool hasRowsOTP = false;

            try
            {
                string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString; ;
                conn = new OleDbConnection(myConnectionString);

                conn.Open();

                command = new OleDbCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                //-------------Return
                command.CommandType = CommandType.Text;
                //command.CommandText = "select CONVERT(nvarchar(max),ID_Card,2) as ID_Card , Mobile_No, Pin from SrvA_PIN where Flag = 1";
                if (signup.Type.Equals("1"))
                {
                    command.CommandText = "select UnitHolder, ID_Card from SrvA_Customer_Cloud where UnitHolder = ? and ID_Card = ? and Mobile_No = ? and Flag = 1";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UnitHolder", signup.UnitHolder == null ? "" : signup.UnitHolder.Trim());
                    command.Parameters.AddWithValue("@ID_Card", signup.ID_Card == null ? new byte[0] : hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signup.ID_Card.Trim())));
                    command.Parameters.AddWithValue("@Mobile_No", signup.Mobile_No == null ? new byte[0] : hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signup.Mobile_No.Trim())));
                }
                else
                {
                    command.CommandText = "select UnitHolder, ID_Card from SrvA_Customer_Cloud where Username = ? and Password = ? and Mobile_No = ? and Flag = 1";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Username", signup.Username == null ? "" : signup.Username.Trim());
                    command.Parameters.AddWithValue("@Password", signup.Password == null ? "" : signup.Password.Trim());
                    command.Parameters.AddWithValue("@Mobile_No", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signup.Mobile_No.Trim())));
                }

                mySQLReader = command.ExecuteReader();

                while (mySQLReader.Read())
                {
                    //mySQLReader.GetString(mySQLReader.GetOrdinal("UnitHolder"));
                    signupResponse.Message = "Waiting for OTP";
                    signupResponse.Status = "OK";
                }

                if (mySQLReader.HasRows) hasRows = true;
                mySQLReader.Close();

                if (hasRows)
                {
                    char[] separators = new char[] { ' ', ';', ',', '\r', '\t', '\n', '-' };

                    string mobile = "";
                    string[] temp = signup.Mobile_No.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    mobile = String.Join("\n", temp);
                    mobile = "+66" + mobile.Substring(1, mobile.Length - 1);
                    signupResponse.Status = mobile;

                    //--------------------------------  OTP Generate   --------------------------
                    string OTP = "";
                    byte[] secretkey = new Byte[64];

                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                        // The array is now filled with cryptographically strong random bytes.
                        rng.GetBytes(secretkey);

                        // Use the secret key to sign the message file.
                        //SignFile(secretkey, dataFile, signedFile);

                        // Verify the signed file
                        //VerifyFile(secretkey, signedFile);
                    }

                    OtpAuthenticator otpAuthenticator = new OtpAuthenticator(0, OtpAlgorithm.SHA512, secretkey);
                    OTP = otpAuthenticator.GetOtp();
                    signupResponse.Status = OTP;
                    //--------------------------------  /OTP Generate  --------------------------

                    //signupResponse.Status = Helper.base64Decode("aT4Zr2ziKVZf4a+hGvyZfWHgYcU=");

                    //signupResponse.Status = Helper.base64Encode("bblam");
                    //signupResponse.Status = Convert.ToBase64String(Base32.Decode("g+6JjGHD75cSeRBQOvkyXQ"));

                    //signupResponse.Status = Helper.EncodePassword("bblam", "123456");

                    //MembershipProvider membershipProvider = new MembershipProvider();

                    //signupResponse.Status = FormsAuthentication.HashPasswordForStoringInConfigFile("CHULAT3pit6T+AnxAEWFslVd5Lw==","sha1");
                    //CryptoStream c = new CryptoStream();
                    byte[] bytes = System.Text.Encoding.Unicode.GetBytes("Password7");  //password
                    byte[] src = Convert.FromBase64String("g+6JjGHD75cSeRBQOvkyXQ==");//salt
                    //byte[] src = System.Text.Encoding.UTF8.GetBytes("g+6JjGHD75cSeRBQOvkyXQ==");//salt
                    byte[] dst = new byte[src.Length + bytes.Length];

                    Buffer.BlockCopy(src, 0, dst, 0, src.Length);
                    Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
                    HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
                    byte[] inArray = algorithm.ComputeHash(dst);
                    signupResponse.Status = Convert.ToBase64String(inArray);

                    //var sha1 = System.Security.Cryptography.SHA1.Create();
                    //signupResponse.Status = Convert.ToBase64String(sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes("g+6JjGHD75cSeRBQOvkyXQ==")));
                    //Crypto.HashPassword(password)

                    //--------------------------------  Ref. Generate  --------------------------
                    Random rand = new Random();
                    int size = 6;
                    //const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    char[] chars = new char[size];
                    for (int i = 0; i < size; i++)
                    {
                        chars[i] = Alphabet[rand.Next(Alphabet.Length)];
                    }
                    var refNo = new string(chars);
                    //--------------------------------  /Ref. Generate  --------------------------

                    //--------------------------------  Check before sent OTP   ----------------
                    //  อย่าลืมผูกเงื่อนไขเพื่อตรวจสอบจากตาราง SrvA_PIN_Cloud เพื่อตรวจคนที่เคยลงทะเบียนแล้ว

                    command.CommandType = CommandType.Text;
                    //command.CommandText = "select * from SrvA_OTP_Cloud where (Mobile_No = ? and DATEDIFF(minute, Dt_Gen, GETDATE()) <= 3 and flag = 1) or (select count(OTP_No) from SrvA_OTP_Cloud where Mobile_No = ? and DATEDIFF(day, Dt_Gen, GETDATE()) = 0) > 3";
                    command.CommandText = "select * from SrvA_OTP_Cloud where (Mobile_No = ? and DATEDIFF(minute, Dt_Gen, GETDATE()) <= 10 and flag = 1) or (select count(OTP_No) from SrvA_OTP_Cloud where Mobile_No = ? and DATEDIFF(day, Dt_Gen, GETDATE()) = 0) > 3";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Mobile_No", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signup.Mobile_No.Trim())));
                    command.Parameters.AddWithValue("@Mobile_No", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signup.Mobile_No.Trim())));
                    mySQLReader = command.ExecuteReader();

                    if (mySQLReader.HasRows) hasRowsOTP = true;
                    mySQLReader.Close();

                    if (hasRowsOTP)
                    {
                        //--------------------------------  Insert OTP to Database   ----------------
                        command.CommandType = CommandType.Text;
                    command.CommandText = "insert into SrvA_OTP_Cloud(Mobile_No,OTP_No,Ref_No,Dt_Gen,Flag) values(?,?,?,GETDATE(),1)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Mobile_No", hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signup.Mobile_No.Trim())));
                    command.Parameters.AddWithValue("@OTP_No", OTP);
                    command.Parameters.AddWithValue("@Ref_No", refNo);
                    command.ExecuteNonQuery();
                    //--------------------------------  /Insert OTP to Database  ----------------

                    //--------------------------------  twilio  --------------------------
                    
                    // Find your Account Sid and Token at twilio.com/console
                    const string accountSid = "AC15698d45616d4e4a93f1dea51c1818f3";
                    const string authToken = "f7a0c28c34ede98e3879381065a24f45";

                    TwilioClient.Init(accountSid, authToken);

                    var message = MessageResource.Create(
                        //body: "OTP = " + OTP,
                        body: "OTP ของท่านคือ " + OTP + " (หมายเลขอ้างอิง " + refNo + ") OTP นี้หมดอายุใน 3 นาที",
                    from: new Twilio.Types.PhoneNumber("+13343199559"),
                        to: new Twilio.Types.PhoneNumber(mobile)
                    );

                    Console.WriteLine(message.Sid);
                    
                    //--------------------------------  /twilio   --------------------------
                //}


                /*
                // Find your Account Sid and Token at twilio.com/console
                const string accountSid = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
                const string authToken = "your_auth_token";

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: "This is the ship that made the Kessel Run in fourteen parsecs?",
                    from: new Twilio.Types.PhoneNumber("+15017122661"),
                    to: new Twilio.Types.PhoneNumber("+15558675310")
                );

                Console.WriteLine(message.Sid);
                 */

                //return SignupArrayList;
                //return signupResponse;
                }   //-------  /hasRowsOTP
                else
                {
                    signupResponse.Message = "Limited of OTP or old OTP still active";
                    signupResponse.Status = "Fail";
                }   //-------  /hasRowsOTP else

                //--------------------------------  /Check before sent OTP  ----------------

            }
                else
                {
                signupResponse.Message = "Username or Mobile No. not correct";
                signupResponse.Status = "Fail";
            }

            //return forgotArrayList;
            return signupResponse;
        }
            /*catch (SqlException ex)
            {
                throw ex;
            }
            */
            catch (Exception ex)
            {
                signupResponse.Message = ex.ToString();
                signupResponse.Status = "Fail";
                return signupResponse;
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

        public SignupVerifyResponse GetSignupVerify(SignupVerify signupverify)
        {
            OleDbConnection conn = null;
            OleDbCommand command = null;
            OleDbDataReader mySQLReader = null;

            var hash = System.Security.Cryptography.SHA512.Create();

            SignupVerifyResponse signupVerifyResponse = new SignupVerifyResponse();
            signupVerifyResponse.Message = "Not Found";
            signupVerifyResponse.Status = "Fail";

            bool hasRows = false;

            try
            {
                string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString; ;
                conn = new OleDbConnection(myConnectionString);

                conn.Open();

                command = new OleDbCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                command.CommandType = CommandType.Text;
                command.CommandText = "select otp.Mobile_No, otp.OTP_No, otp.Ref_No, otp.Dt_Gen,customer.UnitHolder from SrvA_OTP_Cloud  otp left join SrvA_Customer_Cloud customer on otp.Mobile_No = customer.Mobile_No where (otp.Mobile_No = ? and otp.OTP_No = ? and DATEDIFF(minute, otp.Dt_Gen, GETDATE()) <= 3 and otp.flag = 1)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Mobile_No", signupverify.Mobile_No == null ? new byte[0] : hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signupverify.Mobile_No.Trim())));
                command.Parameters.AddWithValue("@OTP", signupverify.OTP == null ? "" : signupverify.OTP.Trim());
                mySQLReader = command.ExecuteReader();
                /*
                while (mySQLReader.Read())
                {
                    signupVerifyResponse.UnitHolder = mySQLReader.GetString(mySQLReader.GetOrdinal("UnitHolder"));
                }
                */
                if (mySQLReader.HasRows) hasRows = true;
                mySQLReader.Close();

                if (hasRows)
                {
                    /*
                    //--------------------------------  Hash Password  --------------------------
                    byte[] bytes = System.Text.Encoding.Unicode.GetBytes(signupverify.Password.Trim());  //password
                    byte[] src = Convert.FromBase64String("g+6JjGHD75cSeRBQOvkyXQ==");//salt
                                                                                      //byte[] src = System.Text.Encoding.UTF8.GetBytes("g+6JjGHD75cSeRBQOvkyXQ==");//salt
                    byte[] dst = new byte[src.Length + bytes.Length];

                    Buffer.BlockCopy(src, 0, dst, 0, src.Length);
                    Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
                    HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
                    byte[] inArray = algorithm.ComputeHash(dst);
                    signupVerifyResponse.Password = Convert.ToBase64String(inArray);
                    signupVerifyResponse.Message = "Success";
                    signupVerifyResponse.Status = "OK";
                    //--------------------------------  /Hash Password --------------------------
                    */

                    command.CommandType = CommandType.Text;
                    command.CommandText = "Insert Into SrvA_PIN_Cloud(Mobile_No,PIN,Dt_Gen,Flag)VALUES(?,?,GETDATE(),1)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Mobile_No", signupverify.Mobile_No == null ? new byte[0] : hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signupverify.Mobile_No.Trim())));
                    command.Parameters.AddWithValue("@PIN", signupverify.PIN == null ? new byte[0] : hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signupverify.PIN.Trim())));
                    command.ExecuteNonQuery();

                    TokenService tokenService = new TokenService();
                    var token = tokenService.GetToken(signupverify.Mobile_No.Trim().ToString(), signupverify.PIN.Trim().ToString());
                    //var token = GetToken("0813963651", "315709");

                    signupVerifyResponse.AccessToken = token;
                    signupVerifyResponse.Message = "Success";
                    signupVerifyResponse.Status = "OK";
                }
                else
                {
                    signupVerifyResponse.Message = "มีการลงทะเบียนเรียบร้อยแล้ว";
                }

                //return signupArrayList;
                return signupVerifyResponse;
            }
            /*catch (SqlException ex)
            {
                throw ex;
            }
            */
            catch (Exception ex)
            {
                signupVerifyResponse.Message = ex.ToString();
                signupVerifyResponse.Status = "Fail";
                return signupVerifyResponse;
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