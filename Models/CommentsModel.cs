using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blogging.Models
{
    public class CommentsModel
    {
        public long UserID { get; set; }
        public long CommentID { get; set; }
        public long BlogID { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
    public class RepliesModel
    {
        public long ReplyID { get; set; }
        public long UserID { get; set; }
        public long CommentID { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}