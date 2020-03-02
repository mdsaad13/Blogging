using Blogging.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Blogging.DAL
{
    public class CreatorUtil
    {
        private readonly SqlConnection Conn = new SqlConnection("Data Source=localhost;Initial Catalog=Blogging;Integrated Security=True");
        internal bool CreatBlog(BlogModel blogModel)
        {
            bool status;
            try
            {
                /*
                 * Creating a SQL prepared statement
                 */
                string query = "INSERT INTO blog (blogid, userID, title, blogContent, datetime, status, viewcount, viewtime, catid, url)" +
                        " VALUES(@blogid, @userID, @title, @blogContent, @datetime, @status, @viewcount, @viewtime, @catid, @url)";


                SqlCommand cmd = new SqlCommand(query, Conn);

                /*
                 * Binding the SQL prepared statement with values
                 */
                cmd.Parameters.Add(new SqlParameter("blogid", blogModel.BlogID));
                cmd.Parameters.Add(new SqlParameter("userID", blogModel.UserID));
                cmd.Parameters.Add(new SqlParameter("title", blogModel.Title));
                cmd.Parameters.Add(new SqlParameter("blogContent", blogModel.Content));
                cmd.Parameters.Add(new SqlParameter("datetime", blogModel.DateTime));
                cmd.Parameters.Add(new SqlParameter("status", blogModel.Status));
                cmd.Parameters.Add(new SqlParameter("viewcount", blogModel.ViewCount));
                cmd.Parameters.Add(new SqlParameter("viewtime", blogModel.ViewTime));
                cmd.Parameters.Add(new SqlParameter("catid", blogModel.CatID));
                cmd.Parameters.Add(new SqlParameter("url", blogModel.URL));

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

        internal bool AttachImgToBlog(string ImgURL, int blogID)
        {
            bool result = false;
            try
            {
                string query = "INSERT INTO blogimg (blogid, url, status)" +
                        " VALUES(@blogid, @url, @status)";
                SqlCommand cmd = new SqlCommand(query, Conn);
                cmd.Parameters.Add(new SqlParameter("blogid", blogID));
                cmd.Parameters.Add(new SqlParameter("url", ImgURL));
                cmd.Parameters.Add(new SqlParameter("status", 1));

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
                    result = true;
                }
                else
                {
                    result = false;
                }

            }
            catch (Exception ex)
            {
                result = false;
            }
            
            return result;
        }

        internal List<BlogImages> GetImgsByBlog(int BlogID)
        {
            DataTable td = new DataTable();
            List<BlogImages> list = new List<BlogImages>();

            try
            {
                string sqlquery = "SELECT * FROM blogimg WHERE blogid = @BlogID";
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("BlogID", BlogID));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                Conn.Open();
                adp.Fill(td);
                foreach (DataRow row in td.Rows)
                {
                    BlogImages blogImages = new BlogImages();

                    blogImages.ID = Convert.ToInt32(row["id"]);
                    blogImages.URL = Convert.ToString(row["url"]);

                    list.Add(blogImages);
                }
            }
            catch (Exception)
            { }
            finally
            {
                Conn.Close();
            }
            return list;
        }

    }
}