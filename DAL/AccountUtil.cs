using Blogging.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Blogging.DAL
{
    public class AccountUtil
    {
        private readonly SqlConnection Conn = new SqlConnection("Data Source=localhost;Initial Catalog=Blogging;Integrated Security=True");

        internal bool CreatAcc(AccountModel accountModel)
        {
            bool status;
            try
            {
                /*
                 * Creating a SQL prepared statement
                 */
                string query = "INSERT INTO users (userID, username, email, password, phone, name, status)" +
                        " VALUES(@userID, @username, @email, @password, @phone, @name, @status)";
                
                string query1 = "INSERT INTO userDetails (userID, imgURL, dob, gender)" +
                        " VALUES(@userID, @imgURL, @dob, @gender)";

                SqlCommand cmd = new SqlCommand(query, Conn);

                SqlCommand cmd1 = new SqlCommand(query1, Conn);

                /*
                 * Binding the SQL prepared statement with values
                 */
                cmd.Parameters.Add(new SqlParameter("userID", accountModel.ID));
                cmd.Parameters.Add(new SqlParameter("username", accountModel.UserName));
                cmd.Parameters.Add(new SqlParameter("email", accountModel.Email));
                cmd.Parameters.Add(new SqlParameter("password", accountModel.Password));
                cmd.Parameters.Add(new SqlParameter("phone", accountModel.Mobile));
                cmd.Parameters.Add(new SqlParameter("name", accountModel.Name));
                cmd.Parameters.Add(new SqlParameter("status", 1));

                cmd1.Parameters.Add(new SqlParameter("userID", accountModel.ID));
                cmd1.Parameters.Add(new SqlParameter("imgURL", accountModel.ImgUrl));
                cmd1.Parameters.Add(new SqlParameter("dob", accountModel.DOB));
                cmd1.Parameters.Add(new SqlParameter("gender", accountModel.Gender));

                /*
                 * Opening sql connection
                 */
                Conn.Open();

                /*
                 * @return rows = number of rows affected
                 */
                int rows = cmd.ExecuteNonQuery();

                if(rows > 0)
                {
                    cmd1.ExecuteNonQuery();
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch
            {
                status = false;
            }
            finally
            {
                /*
                 * Closing sql connection
                 */
                Conn.Close();
            }
            return status;
        }

        internal int Login(string unameORmobile, string Password)
        {
            int ret = 0;
            try
            {
                /*
                 * Creating a SQL prepared statement
                 */
                string query = "SELECT * FROM users WHERE username = @unameORmobile OR phone = @unameORmobile";
                SqlCommand cmd = new SqlCommand(query, Conn);

                /*
                 * Binding the SQL prepared statement with values
                 */
                cmd.Parameters.Add(new SqlParameter("unameORmobile", unameORmobile));

                /*
                 * Creating object of SqlDataAdapter
                 * Used to retrieve data
                 */
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                /*
                 * Creating object of DataTable
                 * Used to store retrieved data in tabular format
                 */
                DataTable dt = new DataTable();

                /*
                 * Storing the `SqlDataAdapter` data into `DataTable`
                 */
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string newPassword = Convert.ToString(dt.Rows[0]["password"]);
                    if (Password == newPassword)
                    {
                        //HttpSessionStateBase.Session["a"] = 1;
                        HttpContext.Current.Session["userID"] = dt.Rows[0]["userID"];
                        HttpContext.Current.Session["username"] = dt.Rows[0]["username"];
                        ret = 1;
                    }
                    else
                    {
                        ret = 2;
                    }
                }
                else
                {
                    // Invalid username
                    ret = 0;
                }
            }
            catch
            { }
            return ret;
        }

        /// <summary>
        /// <b>param</b> <c>long userID</c> <br></br>
        /// <b>Return</b> object of <c>`AccountModel`</c> <br></br>
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        internal AccountModel GetUserById(long userID)
        {
            AccountModel accountModel = new AccountModel();
            try
            {
                /*
                 * Creating a SQL prepared statement
                 */
                string query = "SELECT * FROM users WHERE userID = @userID";
                string query1 = "SELECT * FROM userDetails WHERE userID = @userID";

                SqlCommand cmd = new SqlCommand(query, Conn);
                SqlCommand cmd1 = new SqlCommand(query1, Conn);

                /*
                 * Binding the SQL prepared statement with values
                 */
                cmd.Parameters.Add(new SqlParameter("userID", userID));
                cmd1.Parameters.Add(new SqlParameter("userID", userID));

                /*
                 * Creating object of SqlDataAdapter
                 * Used to retrieve data
                 */
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                SqlDataAdapter adp1 = new SqlDataAdapter(cmd1);

                /*
                 * Creating object of DataTable
                 * Used to store retrieved data in tabular format
                 */
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();

                /*
                 * Storing the `SqlDataAdapter` data into `DataTable`
                 */
                adp.Fill(dt);
                adp1.Fill(dt1);
                if (dt.Rows.Count > 0 && dt1.Rows.Count > 0)
                {
                    accountModel.ID = Convert.ToInt64(dt.Rows[0]["userID"]);
                    accountModel.UserName = Convert.ToString(dt.Rows[0]["username"]);
                    accountModel.Name = Convert.ToString(dt.Rows[0]["name"]);
                    accountModel.Email = Convert.ToString(dt.Rows[0]["email"]);
                    accountModel.Mobile = Convert.ToString(dt.Rows[0]["phone"]);

                    accountModel.Gender = Convert.ToString(dt1.Rows[0]["gender"]);
                    accountModel.DOB = Convert.ToDateTime(dt1.Rows[0]["dob"]);
                    accountModel.ImgUrl = Convert.ToString(dt1.Rows[0]["imgURL"]);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return accountModel;
        }
        
        internal AccountModel GetUserByUname(string username)
        {
            AccountModel accountModel = new AccountModel();
            try
            {
                /*
                 * Creating a SQL prepared statement
                 */
                string query = "SELECT * FROM users WHERE username = @username";

                SqlCommand cmd = new SqlCommand(query, Conn);

                /*
                 * Binding the SQL prepared statement with values
                 */
                cmd.Parameters.Add(new SqlParameter("username", username));

                /*
                 * Creating object of SqlDataAdapter
                 * Used to retrieve data
                 */
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                /*
                 * Creating object of DataTable
                 * Used to store retrieved data in tabular format
                 */
                DataTable dt = new DataTable();

                /*
                 * Storing the `SqlDataAdapter` data into `DataTable`
                 */
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    accountModel.ID = Convert.ToInt64(dt.Rows[0]["userID"]);
                    accountModel.Name = Convert.ToString(dt.Rows[0]["name"]);
                    accountModel.Email = Convert.ToString(dt.Rows[0]["email"]);
                    accountModel.Mobile = Convert.ToString(dt.Rows[0]["phone"]);

                    string query1 = "SELECT * FROM userDetails WHERE userID = @userID";
                    SqlCommand cmd1 = new SqlCommand(query1, Conn);
                    cmd1.Parameters.Add(new SqlParameter("userID", accountModel.ID));
                    SqlDataAdapter adp1 = new SqlDataAdapter(cmd1);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);

                    if (dt1.Rows.Count > 0)
                    {
                        accountModel.Gender = Convert.ToString(dt1.Rows[0]["gender"]);
                        accountModel.DOB = Convert.ToDateTime(dt1.Rows[0]["dob"]);
                        accountModel.ImgUrl = Convert.ToString(dt1.Rows[0]["imgURL"]);
                    }    
                }
            }
            catch (Exception)
            {
                throw;
            }

            return accountModel;
        }
    }
}