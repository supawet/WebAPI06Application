using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebAPI06Application.Models;

using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace WebAPI06Application.Services
{
    public class TokenService
    {
        public string GetToken(string username, string password, string tenancyName = null)
        {
            try
            {
                string postString = string.Format("username={0}&amp;password={1}&amp;grant_type=password", HttpUtility.HtmlEncode(username), HttpUtility.HtmlEncode(password));

                string encodedUserCredentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("user:password"));
                string userData = "username=" + username + "&password=" + password + "&grant_type=password";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:57533/api/token");
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.Headers.Add("Authorization", "Basic " + encodedUserCredentials);

                StreamWriter requestWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                requestWriter.Write(userData);
                requestWriter.Close();

                var httpResponse = httpWebRequest.GetResponse() as HttpWebResponse;

                string json;
                using (Stream responseStream = httpResponse.GetResponseStream())
                {
                    json = new StreamReader(responseStream).ReadToEnd();
                }
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
                return tokenResponse.AccessToken;

                /*
                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                return readStream.ReadToEnd();
                //return response.ToString();

                
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var input = "{\"grant_type\":\"" + "password" + "\"," +
                                "\"username\":\"" + username + "\"," +
                                "\"password\":\"" + password + "\"}";

                    if (tenancyName != null)
                    {
                        input = input.TrimEnd('}') + "," +
                                "\"tenancyName\":\"" + tenancyName + "\"}";
                    }

                    streamWriter.Write(input);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                */
                /*
                byte[] bytes = Encoding.UTF8.GetBytes(postString);
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {

                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
                string json;
                using (Stream responseStream = httpResponse.GetResponseStream())
                {
                    json = new StreamReader(responseStream).ReadToEnd();
                }
                TokenResponseModel tokenResponse = (TokenResponseModel)JsonConvert.DeserializeObject(json);
                return tokenResponse.AccessToken;
                */
                /*
                // Set credentials to use for this request.
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                // Get the stream associated with the response.
                Stream receiveStream = httpResponse.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                return readStream.ReadToEnd();
                */
                /*
                Console.Write(httpResponse.ToString());
                string response;

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                // Crude way
                var entries = response.TrimStart('{').TrimEnd('}').Replace("\"", String.Empty).Split(',');

                foreach (var entry in entries)
                {
                    if (entry.Split(':')[0] == "result")
                    {
                        return entry.Split(':')[1];
                    }
                }
                */
                //return null;
            }
            catch (Exception ex)
            {
                return "SignupPersistance error";// ex.ToString();
            }
        }
    }
}