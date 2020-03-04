using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Blogging.Controllers;
using Blogging.Models;
using HtmlAgilityPack;

namespace Blogging.DAL
{
    public class IndexUtil
    {
        /*
         * Conn = connection string of database
         */
        private readonly SqlConnection Conn = new SqlConnection("Data Source=localhost;Initial Catalog=Blogging;Integrated Security=True");

        internal List<AllBlogsModel> AllBlogs ()
        {
            DataTable td = new DataTable();
            List<AllBlogsModel> list = new List<AllBlogsModel>();
            CommonUtil commonUtil = new CommonUtil();

            string sqlquery = "SELECT * FROM blog";

            try
            {
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                Conn.Open();
                adp.Fill(td);

                foreach (DataRow row in td.Rows)
                {
                    int BlogID = Convert.ToInt32(row["blogid"]);
                    long UserID = Convert.ToInt64(row["userID"]);
                    string Content = Convert.ToString(row["blogContent"]);
                    DateTime PublishDate = Convert.ToDateTime(row["datetime"]);

                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(Content);
                    // Removing all html tags
                    Content = htmlDoc.DocumentNode.InnerText;

                    string[] UserDetails = BasicUserDetails(UserID);

                    bool HasImages = false;
                    string BlogImg = String.Empty;
                    if (commonUtil.CountByArgs("blogimg", "blogid = "+ BlogID) > 0)
                    {
                        HasImages = true;
                        BlogImg = RandBlogImg(BlogID);
                    }

                    AllBlogsModel SingleBlogData = new AllBlogsModel()
                    {
                        BlogID = BlogID,
                        Freatured = IsFeatured(BlogID),
                        HasImages = HasImages,
                        BlogImg = BlogImg,
                        Title = Convert.ToString(row["title"]),
                        Content = Content.Substring(0, 220),
                        PublistDate = PublishDate.ToLongDateString(),
                        Likes = GeneralFns.FormatNumber(commonUtil.CountByArgs("likes", "blogid = " + BlogID)),
                        Views = GeneralFns.FormatNumber(Convert.ToInt64(row["viewcount"])),
                        Comments = GeneralFns.FormatNumber(commonUtil.CountByArgs("comments", "blogid = " + BlogID)),
                        URL = Convert.ToString(row["url"]),

                        CatName = CategoryName(Convert.ToInt32(row["catid"])),
                        UniqueUserName = UserDetails[0],
                        UserName = UserDetails[1],
                        UserImg = UserDetails[2],
                    };
                    list.Add(SingleBlogData);
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

        private string RandBlogImg(int BlogID)
        {
            DataTable td = new DataTable();
            string Url;
            try
            {
                string sqlquery = "SELECT * FROM blogimg WHERE blogid = @blogid ORDER BY RAND()";
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("blogid", BlogID));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(td);

                Url = Convert.ToString(td.Rows[0]["url"]);
            }
            catch (Exception)
            {
                throw;
            }
            return Url;
        }

        private bool IsFeatured(int BlogID)
        {
            bool result = true;

            return result;
        }

        private string[] BasicUserDetails(long UserID)
        {
            string[] data = new string[3];
            DataTable td = new DataTable();
            try
            {
                string sqlquery = "SELECT * FROM users WHERE userID = @userID";
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("userID", UserID));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                
                string sqlquery1 = "SELECT * FROM userDetails WHERE userID = @userID";
                SqlCommand cmd1 = new SqlCommand(sqlquery1, Conn);
                cmd1.Parameters.Add(new SqlParameter("userID", UserID));
                SqlDataAdapter adp1 = new SqlDataAdapter(cmd1);

                adp.Fill(td);
                adp1.Fill(td);

                data[0] = Convert.ToString(td.Rows[0]["username"]);

                data[1] = Convert.ToString(td.Rows[0]["name"]);

                if (!DBNull.Value.Equals(td.Rows[1]["imgURL"]))
                    data[2] = Convert.ToString(td.Rows[1]["imgURL"]);
                else
                    data[2] = String.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }

            return data;
        }

        private string CategoryName(int CatID)
        {
            string Name;
            DataTable td = new DataTable();
            try
            {
                string sqlquery = "SELECT * FROM categories WHERE catid = @catid";
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("catid", CatID));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(td);

                Name = Convert.ToString(td.Rows[0]["name"]);
            }
            catch (Exception)
            {
                throw;
            }

            return Name;
        }

    }
}