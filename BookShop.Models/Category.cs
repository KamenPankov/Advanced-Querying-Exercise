using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public ICollection<BookCategory> CategoryBooks { get; set; } = new HashSet<BookCategory>();
    }
}
