using Blogging.Controllers;
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
                    BlogImages blogImages = new BlogImages
                    {
                        ID = Convert.ToInt32(row["id"]),
                        URL = Convert.ToString(row["url"])
                    };

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

        internal List<BlogModel> GetAllBlogs()
        {
            DataTable td = new DataTable();
            List<BlogModel> list = new List<BlogModel>();
            try
            {
                long UserID = Convert.ToInt64(HttpContext.Current.Session["userID"]);
                string sqlquery = "SELECT * FROM blog WHERE userID = @userID ORDER BY datetime DESC";

                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("userID", UserID));

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                Conn.Open();
                adp.Fill(td);
                foreach (DataRow row in td.Rows)
                {
                    BlogModel obj = new BlogModel();
                    obj.BlogID = Convert.ToInt32(row["blogid"]);
                    obj.Title = Convert.ToString(row["title"]);
                    obj.Content = Convert.ToString(row["blogContent"]);
                    obj.DateTime = Convert.ToDateTime(row["datetime"]);
                    obj.Status = Convert.ToInt32(row["status"]);
                    obj.ViewCount = Convert.ToInt64(row["viewcount"]);
                    obj.FormatViewCount = GeneralFns.FormatNumber(obj.ViewCount);
                    obj.ViewTime = Convert.ToInt64(row["viewtime"]);
                    obj.CatID = Convert.ToInt32(row["catid"]);
                    obj.URL = Convert.ToString(row["url"]);

                    if (!DBNull.Value.Equals(row["updatedate"]))
                        obj.UpdateDateTime = Convert.ToDateTime(row["updatedate"]);

                    list.Add(obj);
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

        internal bool DeleteBlogImg(int blogID, int imgID)
        {
            bool result = false;
            try
            {
                string query = "DELETE FROM blogimg WHERE blogid = @blogID AND id = @imgID";
                SqlCommand cmd = new SqlCommand(query, Conn);
                cmd.Parameters.Add(new SqlParameter("@blogID", blogID));
                cmd.Parameters.Add(new SqlParameter("@imgID", imgID));
                Conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows != 0)
                    result = true;
            }
            catch
            { }
            finally
            {
                Conn.Close();
            }
            return result;
        }
    }
}