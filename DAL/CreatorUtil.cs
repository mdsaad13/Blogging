using Blogging.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Blogging.DAL
{
    public class CreatorUtil
    {
        private readonly SqlConnection Conn = new SqlConnection("Data Source=localhost;Initial Catalog=Blogging;Integrated Security=True");
        internal bool CreatBlog(AccountModel accountModel)
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

                if (rows > 0)
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
    }
}