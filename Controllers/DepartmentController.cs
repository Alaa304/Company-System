using Microsoft.AspNetCore.Mvc;
using RouteG01.BLL.DTOS;
using RouteG01.BLL.DTOS.Department;
using RouteG01.BLL.Services.Interfaces;
using RouteG01.Pl.ViewModels.DepartmentViewModels;

namespace RouteG01.Pl.Controllers
{
    public class DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger,IWebHostEnvironment environment) : Controller
    {
        private readonly IDepartmentService _departmentService = departmentService;
        private readonly ILogger<DepartmentController> _logger = logger;
        private readonly IWebHostEnvironment _environment = environment;

        #region Index
        //BaseUrl/Department/Index
        public IActionResult Index()
        {
            //ViewData["Message"] = "Hello From View Data";
            //ViewBag.Message = "Hello From View Bag";
            //ViewData["Message"] = new DepartmentDto() { Name = "TestViewData" };
            //ViewBag.Message = new DepartmentDto() { Name = "TestViewData" };
            var Departments = _departmentService.GetAllDepartments();
            return View(Departments);
        }
        #endregion
        #region Create Department
        [HttpGet]
     
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(DepartmentViewModel departmentVM)
        {
            // Server Side Validation
            if(ModelState.IsValid)
            {
                try
                {
                    var departmentDto = new CreatedDepartmentDto()
                    {
                        Name = departmentVM.Name,
                        Code = departmentVM.Code,
                        DateOfCreation = departmentVM.DateOfCreation,
                        Description = departmentVM.Description

                    };
                    int Result = _departmentService.AddDepartment(departmentDto);
                    string Message;
                    if(Result>0)
                    {
                        Message = $"Department{departmentVM.Name} Has Been Created  Succssfully";
                    }
                    else
                    {
                        Message = $"Department{departmentVM.Name} Has Not Been Created  Succssfully";
                    }
                    TempData["Message"] = Message;
                    return RedirectToAction(nameof(Index));
                        

                }
                catch(Exception ex)
                {
                    //Log Exception
                    //1- Developemnt =>
                    //Log Error In Console  And  Return Same View With Error Message
                    if(_environment.IsDevelopment())
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                        

                    }
                    //2- Deployment => Log Error In File / Table  in Database And Return  Error View
                    else
                    {
                        _logger.LogError(ex.Message);
                        
                    }
                }
            }
            
            return View(departmentVM);

            


        }
        #endregion
        #region Details
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue) return BadRequest();//400
            var Department = _departmentService.GetDepartmentById(id.Value);
            if (Department is null) return NotFound();
            return View(Department);

        }
        #endregion
        #region Edit
        [HttpGet]
         public IActionResult Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var Department = _departmentService.GetDepartmentById(id.Value);
            if (Department is null) return NotFound();
            var DepartmentViewModel = new DepartmentViewModel()
            {
                Code= Department.Code,
                Name=Department.Name,
                Description= Department.Description,
                DateOfCreation= Department.DateOfCreation

            };
            return View(DepartmentViewModel);
        }
        [HttpPost]
        public IActionResult Edit( [FromRoute]int?id,DepartmentViewModel viewModel)
        {
            if (!id.HasValue) return BadRequest();
             if(ModelState.IsValid)
            {
                try
                {
                    var UpdatedDepartment = new UpdatedDepartmentDto()
                    {
                        Id = id.Value,
                        Code = viewModel.Code,
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        DateOfCreation = viewModel.DateOfCreation

                    };
                    int Result = _departmentService.UpdatedDepartment(UpdatedDepartment);
                    if(Result>0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Employee Can not Be Updated");
                       
                    }
                }
                catch(Exception ex)
                {
                    //Log Exception
                    //1- Developemnt =>
                    //Log Error In Console  And  Return Same View With Error Message
                    if (_environment.IsDevelopment())
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);


                    }
                    //2- Deployment => Log Error In File / Table  in Database And Return  Error View
                    else
                    {
                        _logger.LogError(ex.Message);
                        return View("ErrorView", ex);

                    }
                }
            }
            return View(viewModel);
        }
        #endregion
        #region Delete
        //[HttpGet]
        //public IActionResult Delete(int? id)
        //{
        //    if (!id.HasValue) return BadRequest();
        //    var Department = _departmentService.GetDepartmentById(id.Value);
        //    if (Department is null) return NotFound();
        //    return View(Department);
        //}

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id == 0) return BadRequest();
            try
            {
                bool Deleted = _departmentService.DeleteDepartment(id);
                if(Deleted)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Employee Is Not Deleted");
                    return RedirectToAction(nameof(Delete), new { id });
              
                    }
            }
            catch( Exception ex)
            {
                //Log Exception
                //1- Developemnt =>
                //Log Error In Console  And  Return Same View With Error Message
                if (_environment.IsDevelopment())
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                    return RedirectToAction(nameof(Index));


                }
                //2- Deployment => Log Error In File / Table  in Database And Return  Error View
                else
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction(nameof(Index));

                }
            }
        }
        #endregion
    }
}
