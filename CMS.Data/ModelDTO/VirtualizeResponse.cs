using System.Collections.Generic;

namespace CMS.Data.ModelDTO
{
    public class VirtualizeResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalSize { get; set; }
    }
}