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

        internal BlogModel GetBlogByID(long ID)
        {
            BlogModel singleBlog = new BlogModel();
            DataTable td = new DataTable();
            CommonUtil commonUtil = new CommonUtil();

            string sqlquery = "SELECT * FROM blog WHERE blogid = @blogid";

            try
            {
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("blogid", ID));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                Conn.Open();
                adp.Fill(td);

                double UserID = Convert.ToDouble(td.Rows[0]["userID"]);
                int BlogID = Convert.ToInt32(td.Rows[0]["blogid"]);

                singleBlog.BlogID = Convert.ToInt32(td.Rows[0]["blogid"]);
                singleBlog.Title = Convert.ToString(td.Rows[0]["title"]);
                singleBlog.Content = Convert.ToString(td.Rows[0]["blogContent"]);
                singleBlog.CatID = Convert.ToInt32(td.Rows[0]["catid"]);

                singleBlog.DateTime = Convert.ToDateTime(td.Rows[0]["datetime"]);

                if (!DBNull.Value.Equals(td.Rows[0]["updatedate"]))
                {
                    singleBlog.UpdateDateTime  = Convert.ToDateTime(td.Rows[0]["updatedate"]);
                }

                singleBlog.Likes = commonUtil.CountByArgs("likes", "blogid = " + BlogID);
                singleBlog.ViewCount = Convert.ToInt64(td.Rows[0]["viewcount"]);
                singleBlog.ViewTime = Convert.ToInt64(td.Rows[0]["viewtime"]);
                singleBlog.Comments = commonUtil.CountByArgs("comments", "blogid = " + BlogID);
                singleBlog.URL = Convert.ToString(td.Rows[0]["url"]);

                Conn.Close();
            }
            catch
            { }
            return singleBlog;
        }

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

        internal List<AdsModel> GetAllAds()
        {
            DataTable td = new DataTable();
            List<AdsModel> list = new List<AdsModel>();
            try
            {
                long UserID = Convert.ToInt64(HttpContext.Current.Session["userID"]);
                string sqlquery = "SELECT * FROM ads WHERE userID = @userID ORDER BY id DESC";

                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("userID", UserID));

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                Conn.Open();
                adp.Fill(td);
                foreach (DataRow row in td.Rows)
                {
                    AdsModel obj = new AdsModel();
                    obj.ID = Convert.ToDouble(row["id"]);
                    obj.UserID = Convert.ToDouble(row["userID"]);
                    obj.ImgUrl = Convert.ToString(row["img"]);
                    obj.Target = Convert.ToString(row["target"]);
                    obj.FromDate = Convert.ToDateTime(row["fromDate"]);
                    obj.ToDate = Convert.ToDateTime(row["toDate"]);
                    obj.CatID = Convert.ToDouble(row["catid"]);

                    if (!DBNull.Value.Equals(row["views"]))
                        obj.Views = Convert.ToInt64(row["views"]); 
                    
                    if (!DBNull.Value.Equals(row["clicks"]))
                        obj.Clicks = Convert.ToInt64(row["clicks"]);

                    obj.FormatedViews = GeneralFns.FormatNumber(obj.Views);
                    obj.FormatedClicks = GeneralFns.FormatNumber(obj.Clicks);

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

        internal bool InsertAd(AdsAndPaymentBundle model)
        {
            bool status = false;
            try
            {
                /*
                 * Creating a SQL prepared statement
                 */
                string query = "INSERT INTO ads (id, img, target, fromDate, toDate, catid, userID, views, clicks)" +
                        " VALUES(@id, @img, @target, @fromDate, @toDate, @catid, @userID, @views, @clicks)";
                
                string query1 = "INSERT INTO payment_details (userID, razorpay_payment_id, payment_for, adID, datetime)" +
                        " VALUES(@userID, @razorpay_payment_id, @payment_for, @adID, @datetime)";


                SqlCommand cmd = new SqlCommand(query, Conn);

                SqlCommand cmd1 = new SqlCommand(query1, Conn);

                /*
                 * Binding the SQL prepared statement with values
                 */
                cmd.Parameters.Add(new SqlParameter("id", model.Ads.ID));
                cmd.Parameters.Add(new SqlParameter("img", model.Ads.ImgUrl));
                cmd.Parameters.Add(new SqlParameter("target", model.Ads.Target));
                cmd.Parameters.Add(new SqlParameter("fromDate", model.Ads.FromDate));
                cmd.Parameters.Add(new SqlParameter("toDate", model.Ads.ToDate));
                cmd.Parameters.Add(new SqlParameter("catid", model.Ads.CatID));
                cmd.Parameters.Add(new SqlParameter("userID", model.Ads.UserID));
                cmd.Parameters.Add(new SqlParameter("views", model.Ads.Views));
                cmd.Parameters.Add(new SqlParameter("clicks", model.Ads.Clicks));

                cmd1.Parameters.Add(new SqlParameter("userID", model.Ads.UserID));
                cmd1.Parameters.Add(new SqlParameter("razorpay_payment_id", model.Pay.Razor_payment_id));
                cmd1.Parameters.Add(new SqlParameter("payment_for", 1));
                cmd1.Parameters.Add(new SqlParameter("adID", model.Ads.ID));
                cmd1.Parameters.Add(new SqlParameter("datetime", DateTime.Now));

                /*
                 * Opening sql connection
                 */
                Conn.Open();

                /*
                 * @return rows = number of rows affected
                 */
                int rows = cmd.ExecuteNonQuery();

                int rows1 = cmd1.ExecuteNonQuery();

                if (rows > 0 && rows1 > 0)
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch(Exception ex)
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