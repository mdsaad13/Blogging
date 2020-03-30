using Blogging.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Blogging.DAL
{
    public class CommonUtil
    {
        /*
         * Conn = connection string of database
         */
        private readonly SqlConnection Conn = new SqlConnection("Data Source=localhost;Initial Catalog=Blogging;Integrated Security=True");

        internal int Count(string TableName)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM " + TableName;
                SqlCommand cmd = new SqlCommand(query, Conn);
                Conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                Conn.Close();
                return count;
            }
            catch
            {
                return 0;
            }
        }

        internal int CountByArgs(string TableName, string args)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM " + TableName + " WHERE " + args;
                SqlCommand cmd = new SqlCommand(query, Conn);
                Conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                Conn.Close();
                return count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        ///     <b>param</b> <c>table-name</c> <br></br>
        ///     <b>param</b> <c>column-name</c> <br></br>
        ///     <b>param</b> <c>field</c> to validate <br></br><br></br>
        ///     <b>return</b> true if found <br></br>
        ///     <b>return</b> false if not found <br></br>
        /// </summary>
        internal bool Validate(string TableName, string column, string field)
        {
            try
            {
                string query = "SELECT * FROM " + TableName + " WHERE " + column + " = @field";

                SqlCommand cmd = new SqlCommand(query, Conn);

                cmd.Parameters.Add(new SqlParameter("field", field));

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    // Found in database
                    return true;
                }
                else
                {
                    // not found
                    return false;
                }
                
            }
            catch
            {
                return false;
            }
           
        }
        
        internal bool RawValidate(string TableName, string fields)
        {
            try
            {
                string query = "SELECT * FROM " + TableName + " WHERE " + fields;

                SqlCommand cmd = new SqlCommand(query, Conn);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    // Found in database
                    return true;
                }
                else
                {
                    // not found
                    return false;
                }
                
            }
            catch
            {
                return false;
            }
           
        }

        internal List<CategoryModel> GetAllCat()
        {
            DataTable td = new DataTable();
            List<CategoryModel> list = new List<CategoryModel>();
            try
            {
                string sqlquery = "SELECT * FROM categories ORDER BY name ASC";
                SqlCommand cmd = new SqlCommand(sqlquery, Conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                Conn.Open();
                adp.Fill(td);
                foreach (DataRow row in td.Rows)
                {
                    CategoryModel obj = new CategoryModel();
                    obj.CatID = Convert.ToInt64(row["catid"]);
                    obj.Name = Convert.ToString(row["name"]);
                    obj.Icon = Convert.ToString(row["icon"]);
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

    }
}