using System;


namespace Domain.Model
{
    public class PaginatedList<T>where T : class
    {
        public List<T> Items { get; set; }
        public PageData PageData { set; get; }

        public PaginatedList(List<T> items, PageData pageData)
        {
            Items = items;
            PageData = pageData;
        }
    }
}
