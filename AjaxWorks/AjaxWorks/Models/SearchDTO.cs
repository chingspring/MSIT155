namespace AjaxWorks.Models
{
    public class SearchDTO
    {
        public string? keyword {  get; set; }
        public int? categoryId { get; set; }
        public string? sortBy { get; set; }
        public string? sortType { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }

    }
}
