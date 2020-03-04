using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blogging.Models
{
    public class BlogBundle
    {
        public BlogModel Blog { get; set; }
        public BlogImages BlogImages { get; set; }
        public CommentsModel BlogComments { get; set; }
        public RepliesModel CommentsReplies { get; set; }
    }
}