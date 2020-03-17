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
                        PublishDate = PublishDate.ToLongDateString(),
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

        /// <summary>
        /// <b>param: </b><c>double UserID</c><br></br>
        /// <b>param: </b><c>int type</c><br></br>
        /// <c>
        /// if type = 1 // Recents 5 blogs<br></br>
        /// else if type = 2 // All blogs sorted by asc<br></br>
        /// else if type = 3 // All blogs sorted by desc<br></br>
        /// else if type = 4 // All blogs sorted by most viewed
        /// </c>
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal List<AllBlogsModel> AllBlogs (double userID, int type)
        {
            DataTable td = new DataTable();
            List<AllBlogsModel> list = new List<AllBlogsModel>();
            CommonUtil commonUtil = new CommonUtil();

            string sqlquery = string.Empty;

            if (type == 1)
            {
                // Recents 5 blogs
                sqlquery = "SELECT TOP 5 * FROM blog WHERE userID = @userID ORDER BY datetime DESC";
            }
            else if(type == 2)
            {
                // All blogs sorted by asc
                sqlquery = "SELECT * FROM blog WHERE userID = @userID ORDER BY datetime ASC";
            }
            else if(type == 3)
            {
                // All blogs sorted by desc
                sqlquery = "SELECT * FROM blog WHERE userID = @userID ORDER BY datetime DESC";
            }
            else if(type == 4)
            {
                // All blogs sorted by most viewed
                sqlquery = "SELECT * FROM blog WHERE userID = @userID ORDER BY viewcount DESC";
            }

            try
            {
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("userID", userID));
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
                        PublishDate = PublishDate.ToLongDateString(),
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

        internal bool InsertComment(CommentsModel commentsModel)
        {
            bool result = false;
            try
            {
                string query = "INSERT INTO comments (commentID, blogid, userID, text, datetime)" +
                        " VALUES(@commentID, @blogid, @userID, @text, @datetime)";
                SqlCommand cmd = new SqlCommand(query, Conn);

                cmd.Parameters.Add(new SqlParameter("commentID", commentsModel.CommentID));
                cmd.Parameters.Add(new SqlParameter("blogid", commentsModel.BlogID));
                cmd.Parameters.Add(new SqlParameter("userID", commentsModel.UserID));
                cmd.Parameters.Add(new SqlParameter("text", commentsModel.Text));
                cmd.Parameters.Add(new SqlParameter("datetime", commentsModel.DateTime));

                Conn.Open();

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
            catch
            {
                result = false;
            }
            finally
            {
                Conn.Close();
            }
            return result;
        }

        internal SingleBlog FetchSingleBlog(string URL)
        {
            SingleBlog singleBlog = new SingleBlog();
            DataTable td = new DataTable();
            CommonUtil commonUtil = new CommonUtil();
            CreatorUtil creatorUtil = new CreatorUtil();
            AccountUtil accountUtil = new AccountUtil();

            string sqlquery = "SELECT * FROM blog WHERE url = @URL";

            try
            {
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                cmd.Parameters.Add(new SqlParameter("URL", URL));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                Conn.Open();
                adp.Fill(td);
                
                double UserID = Convert.ToDouble(td.Rows[0]["userID"]);
                int BlogID = Convert.ToInt32(td.Rows[0]["blogid"]);

                singleBlog.Blog = new AllBlogsModel();

                singleBlog.Blog.BlogID = Convert.ToInt32(td.Rows[0]["blogid"]);
                singleBlog.Blog.Freatured = IsFeatured(BlogID);
                singleBlog.Blog.Title = Convert.ToString(td.Rows[0]["title"]);
                singleBlog.Blog.Content = Convert.ToString(td.Rows[0]["blogContent"]);

                DateTime PublishDate = Convert.ToDateTime(td.Rows[0]["datetime"]);
                singleBlog.Blog.PublishDate = PublishDate.ToLongDateString();

                if (!DBNull.Value.Equals(td.Rows[0]["updatedate"]))
                {
                    DateTime UpdateDate = Convert.ToDateTime(td.Rows[0]["updatedate"]);
                    singleBlog.Blog.UpdateDate = UpdateDate.ToLongDateString();
                } 
                else
                    singleBlog.Blog.UpdateDate = string.Empty;

                singleBlog.Blog.Likes = GeneralFns.FormatNumber(commonUtil.CountByArgs("likes", "blogid = " + BlogID));
                singleBlog.Blog.Views = GeneralFns.FormatNumber(Convert.ToInt64(td.Rows[0]["viewcount"]));
                singleBlog.Blog.Comments = GeneralFns.FormatNumber(commonUtil.CountByArgs("comments", "blogid = " + BlogID));
                singleBlog.Blog.URL = Convert.ToString(td.Rows[0]["url"]);

                singleBlog.Profile = accountUtil.GetUserDetailsByID(UserID);

                if (commonUtil.CountByArgs("blogimg", "blogid = " + BlogID) > 0)
                {
                    singleBlog.Blog.HasImages = true;
                    singleBlog.BlogImages = creatorUtil.GetImgsByBlog(BlogID);
                }
                else
                {
                    singleBlog.Blog.HasImages = false;
                }

                singleBlog.CatList = PersonalizedCats(Convert.ToInt64(td.Rows[0]["catid"]));
                singleBlog.AdsList = PersonalizedAds(Convert.ToInt64(td.Rows[0]["catid"]));

                long CommentsCount = commonUtil.CountByArgs("comments", "blogid = " + BlogID);
                singleBlog.CommentsCount = CommentsCount;
                singleBlog.FormatedCommentsCount = GeneralFns.FormatNumber(CommentsCount);
                Conn.Close();
                singleBlog.CommentsList = BlogComments(0, BlogID);
            }
            catch
            { }

            return singleBlog;
        }

        private string RandBlogImg(int BlogID)
        {
            DataTable td = new DataTable();
            string Url;
            try
            {
                string sqlquery = "SELECT TOP 1 * FROM blogimg WHERE blogid = @blogid ORDER BY NEWID()";
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

        private List<CategoryModel> PersonalizedCats(long CatID)
        {
            List<CategoryModel> categoryList = new List<CategoryModel>();
            DataTable dataTable = new DataTable();
            CommonUtil commonUtil = new CommonUtil();

            try
            {
                string query = "SELECT * FROM categories WHERE catid = @catid";
                SqlCommand cmd = new SqlCommand(query, Conn);
                cmd.Parameters.Add(new SqlParameter("catid", CatID));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dataTable);

                CategoryModel Firstcategory = new CategoryModel()
                {
                    CatID = Convert.ToInt64(dataTable.Rows[0]["catid"]),
                    Name = Convert.ToString(dataTable.Rows[0]["name"]),
                    Count = commonUtil.CountByArgs("blog", "catid = "+ CatID),
                };
                categoryList.Add(Firstcategory);
                dataTable.Reset();

                string query1 = "SELECT TOP 5 * FROM categories WHERE catid <> @catid ORDER BY NEWID()";
                SqlCommand cmd1 = new SqlCommand(query1, Conn);
                cmd1.Parameters.Add(new SqlParameter("catid", CatID));
                SqlDataAdapter adp1 = new SqlDataAdapter(cmd1);
                adp1.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    CategoryModel Othercategory = new CategoryModel();
                    //int BlogID = Convert.ToInt32(row["blogid"]);
                    Othercategory.CatID = Convert.ToInt64(row["catid"]);
                    Othercategory.Name = Convert.ToString(row["name"]);
                    Othercategory.Count = commonUtil.CountByArgs("blog", "catid = " + Othercategory.CatID);

                    categoryList.Add(Othercategory);
                }
            }
            catch
            { }

            return categoryList;
        }
        
        private List<AdsModel> PersonalizedAds(long CatID)
        {
            List<AdsModel> AdsList = new List<AdsModel>();
            DataTable dataTable = new DataTable();
            var dateTime = DateTime.Now;
            try
            {
                string query = "SELECT TOP 2 * FROM ads WHERE catid = @catid AND fromDate > @fromDate AND toDate > @toDate ORDER BY NEWID()";
                SqlCommand cmd = new SqlCommand(query, Conn);
                cmd.Parameters.Add(new SqlParameter("catid", CatID));
                cmd.Parameters.Add(new SqlParameter("fromDate", dateTime));
                cmd.Parameters.Add(new SqlParameter("toDate", dateTime));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                {
                    string query1 = "SELECT TOP 2 * FROM ads WHERE fromDate > @fromDate AND toDate > @toDate ORDER BY NEWID()";
                    SqlCommand cmd1 = new SqlCommand(query1, Conn);
                    cmd1.Parameters.Add(new SqlParameter("catid", CatID));
                    cmd1.Parameters.Add(new SqlParameter("fromDate", dateTime));
                    cmd1.Parameters.Add(new SqlParameter("toDate", dateTime));
                    SqlDataAdapter adp1 = new SqlDataAdapter(cmd1);
                    adp1.Fill(dataTable);
                }
                else if (dataTable.Rows.Count == 1)
                {
                    string query2 = "SELECT TOP 1 * FROM ads WHERE fromDate > @fromDate AND toDate > @toDate ORDER BY NEWID()";
                    SqlCommand cmd2 = new SqlCommand(query2, Conn);
                    cmd2.Parameters.Add(new SqlParameter("fromDate", dateTime));
                    cmd2.Parameters.Add(new SqlParameter("toDate", dateTime));
                    SqlDataAdapter adp2 = new SqlDataAdapter(cmd);
                    adp2.Fill(dataTable);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    AdsModel adsModel = new AdsModel()
                    {
                        ImgUrl = Convert.ToString(row["img"]),
                        Target = Convert.ToString(row["target"]),
                    };
                    AdsList.Add(adsModel);
                }
            }
            catch
            { }
            return AdsList;
        }

        internal List<CommentsModel> BlogComments(long Offset, long BlogID)
        {
            List<CommentsModel> List = new List<CommentsModel>();
            DataTable dataTable = new DataTable();
            CommonUtil commonUtil = new CommonUtil();

            try
            {
                string query = "SELECT * FROM comments WHERE blogid = @BlogID ORDER BY datetime DESC OFFSET "+ Offset + "ROWS FETCH NEXT 5 ROWS ONLY";
                //string query = "SELECT * FROM comments";
                SqlCommand cmd = new SqlCommand(query, Conn);
                cmd.Parameters.Add(new SqlParameter("BlogID", BlogID));
                
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                Conn.Open();
                adp.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        CommentsModel commentsModel = new CommentsModel();

                        commentsModel.UserID = Convert.ToInt64(row["userID"]);
                        commentsModel.CommentID = Convert.ToInt64(row["commentID"]);
                        commentsModel.BlogID = Convert.ToInt64(row["blogid"]);
                        commentsModel.Text = Convert.ToString(row["text"]);
                        commentsModel.DateTime = Convert.ToDateTime(row["datetime"]);

                        string[] UserDetails = BasicUserDetails(commentsModel.UserID);

                        commentsModel.Name = UserDetails[1];
                        commentsModel.UserName = UserDetails[0];
                        commentsModel.ImgURL = UserDetails[2];

                        commentsModel.FormatDateTime = Convert.ToString(GeneralFns.TimeAgo(commentsModel.DateTime));
                        commentsModel.RepliesCount = commonUtil.CountByArgs("replies", "commentID = "+ commentsModel.CommentID);
                        
                        List.Add(commentsModel);
                    }
            }
            catch
            { }
            finally
            {
                Conn.Close();
            }
            return List;
        }
    }
}