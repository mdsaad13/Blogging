using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blogging.Models
{
    public class CategoryModel
    {
        public long CatID { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public int Count { get; set; }
    }
}