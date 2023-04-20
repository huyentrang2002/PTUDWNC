using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.DTO;

public class PostQuery
{
    public int Id { get; set; }
    public int? AuthorId { get; set; }
    public int? CategoryId { get; set; }
    public string Slug { get; set; }
    public string PostedDate { get; set; }
    public int PostYear { get; set; }
    public int PostMonth { get; set; }
    public string Tags { get; set; }
    public IList<string> SelectedTag { get; set; }
    public IEnumerable<string> SelectedAuthor { get; set; }
    public IEnumerable<string> SelectedCategory { get; set; }
    public bool PublishedOnly { get; set; }
    public bool NotPublished { get; set; }
    public string CategorySlug { get; set; }
    public string AuthorSlug { get; set; }
    public string TagSlug { get; set; }
    public string TitleSlug { get; set; }
    public string Keyword { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
}

