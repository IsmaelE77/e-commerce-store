using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace e_commerce_store.Models
{

    public class Category
    {
        [Key]
        [Display(Name = "ID")]
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        [Required]
        public string? CategoryName { get; set; }
        
        [Display(Name = "Parent Category ID")]
        [ForeignKey("ParentCategory")]
        public int? ParentCategoryId { get; set; }

        [Display(Name = "Parent Category")]
        public Category? ParentCategory { get; set; }
        public List<Category>? ChildCategories { get; set; }
    }
}