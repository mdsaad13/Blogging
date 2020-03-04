﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blogging.Models
{
    public class AllBlogsModel
    {
        public int BlogID { get; set; }
        public bool Freatured { get; set; }
        public bool HasImages { get; set; }
        public string BlogImg { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PublistDate { get; set; }
        public string Likes { get; set; }
        public string Views { get; set; }
        public string Comments { get; set; }
        public string URL { get; set; }

        public string CatName { get; set; }

        public string UserName { get; set; }
        public string UserImg { get; set; }
        public string UniqueUserName { get; set; }
    }
}