using System.Collections.Generic;

namespace Services.DTO
{
    public class CategoryRequestDto
    {
        public string CategoryName { get; set; } = null!;
        public string CategoryDescription { get; set; } = null!;
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CategoryResponseDto
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CategoryDescription { get; set; } = null!;
        public short? ParentCategoryId { get; set; }
        public bool? IsActive { get; set; }

        public ParentCategoryDto? ParentCategory { get; set; }
        public List<ChildCategoryDto> ChildCategories { get; set; } = new();
    }

    public class ParentCategoryDto
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class ChildCategoryDto
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
