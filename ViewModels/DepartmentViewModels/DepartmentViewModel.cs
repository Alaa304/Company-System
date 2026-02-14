namespace RouteG01.Pl.ViewModels.DepartmentViewModels
{
    public class DepartmentViewModel
    {
        
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DateOfCreation { get; set; }
    }
}
