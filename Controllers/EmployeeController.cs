using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RouteG01.BLL.DTOS.Employee;
using RouteG01.BLL.Services.Interfaces;
using RouteG01.Pl.ViewModels.EmployeeViewModels;
namespace RouteG01.Pl.Controllers
{
   
    public class EmployeeController (IEmployeeService employeeService,IWebHostEnvironment environment,ILogger<EmployeeController> logger,IDepartmentService departmentService): Controller
    {
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly ILogger _logger = logger;
        private readonly IDepartmentService _departmentService = departmentService;
        #region Index
        public IActionResult Index(string? EmployeeSearchName)
        {
            var Employees = _employeeService.GetAllEmployees(false);
            if(!string.IsNullOrWhiteSpace(EmployeeSearchName))
            {
                Employees = Employees.Where(E => E.Name.Contains(EmployeeSearchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return View(Employees);
        }
        #endregion
        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new EmployeeViewModel
            {
                Departments = _departmentService.GetAllDepartments()
                    .Select(d => new SelectListItem { Value = d.DeptId.ToString(), Text = d.Name })
            };

            return View(model);
        }


        [HttpPost]
public IActionResult Create(EmployeeViewModel employeeVM)
{
    if (ModelState.IsValid)
    {
        try
        {
            // تحويل ViewModel إلى DTO
            var employeeDto = new CreatedEmployeeDto()
            {
                Name = employeeVM.Name,
                Age = employeeVM.Age,
                Address = employeeVM.Address,
                Email = employeeVM.Email,
                EmployeeType = employeeVM.EmployeeType,
                Gender = employeeVM.Gender,
                HiringDate = employeeVM.HiringDate,
                IsActive = employeeVM.IsActive,
                PhoneNumber = employeeVM.PhoneNumber,
                Salary = employeeVM.Salary,
                DepartmentId = employeeVM.DepartmentId
            };
if (employeeVM.Image == null)
{
    throw new Exception("Image is NULL from View");
}

            // حفظ الصورة في wwwroot/Files/Images
           // حفظ الصورة
if (employeeVM.Image != null && employeeVM.Image.Length > 0)
{
    var fileName = Guid.NewGuid().ToString() + 
                   Path.GetExtension(employeeVM.Image.FileName);

    var folderPath = Path.Combine(_environment.WebRootPath, "Files", "Images");

    if (!Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath);

    var filePath = Path.Combine(folderPath, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        employeeVM.Image.CopyTo(stream);
    }

    employeeDto.ImageName = fileName;
}




            int result = _employeeService.CreateEmployee(employeeDto);

            if (result > 0)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Employee could not be created.");
        }
        catch (Exception ex)
        {
            if (_environment.IsDevelopment())
                ModelState.AddModelError(string.Empty, ex.Message);
            else
                _logger.LogError(ex.Message);
        }
    }

    // إعادة تحميل قائمة الأقسام لو حصل خطأ
    employeeVM.Departments = _departmentService.GetAllDepartments()
        .Select(d => new SelectListItem { Value = d.DeptId.ToString(), Text = d.Name });

    return View(employeeVM);
}


        #endregion
        #region Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var employee = _employeeService.GetEmployeeId(id.Value);
            if (employee is null) return NotFound();

            var employeeVM = new EmployeeViewModel()
            {Id = employee.Id,

                Name = employee.Name,
                Salary = employee.Salary,
                Address = employee.Address,
                Age = employee.Age,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                IsActive = employee.IsActive,
                HiringDate = employee.HiringDate,
                Gender = employee.Gender,
                EmployeeType = employee.EmployeeType,  // assuming EmpTypeEnum من نوع EmployeeTypes
    // لو EmpType عندك string
                DepartmentId = employee.DepartmentId  // هنا هيتخزن الـ ID بتاع الـ Dept الحالي
            };

            // جِيب كل الـ Departments واعمل Select على الحالي
            employeeVM.Departments = _departmentService.GetAllDepartments()
                .Select(d => new SelectListItem
                {
                    Value = d.DeptId.ToString(),
                    Text = d.Name,
                    Selected = d.DeptId == employee.DepartmentId
                });

            return View(employeeVM);
        }


  [HttpPost]
public IActionResult Edit(int id, EmployeeViewModel employeeVm)
{
    if (!ModelState.IsValid)
    {
        employeeVm.Departments = _departmentService.GetAllDepartments()
            .Select(d => new SelectListItem
            {
                Value = d.DeptId.ToString(),
                Text = d.Name,
                Selected = d.DeptId == employeeVm.DepartmentId
            });

        return View(employeeVm);
    }

    var existingEmployee = _employeeService.GetEmployeeId(id);
    if (existingEmployee is null) return NotFound();

    var employeeDto = new UpdatedEmployeeDto()
    {
        Id = id,
        Name = employeeVm.Name,
        Address = employeeVm.Address,
        Age = employeeVm.Age,
        Email = employeeVm.Email,
        EmployeeType = employeeVm.EmployeeType,
        Gender = employeeVm.Gender,
        HiringDate = employeeVm.HiringDate,
        IsActive = employeeVm.IsActive,
        PhoneNumber = employeeVm.PhoneNumber,
        Salary = employeeVm.Salary,
        DepartmentId = employeeVm.DepartmentId,
        ImageName = existingEmployee.Image // نحافظ على القديمة
    };

    // لو المستخدم اختار صورة جديدة
    if (employeeVm.Image != null && employeeVm.Image.Length > 0)
    {
        var fileName = Guid.NewGuid().ToString() +
                       Path.GetExtension(employeeVm.Image.FileName);

        var folderPath = Path.Combine(_environment.WebRootPath, "Files", "Images");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            employeeVm.Image.CopyTo(stream);
        }

        employeeDto.ImageName = fileName;
    }

    var result = _employeeService.UpdateEmployee(employeeDto);

    if (result > 0)
        return RedirectToAction(nameof(Index));

    return View(employeeVm);
}

        #endregion
        #region Details
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue) return BadRequest();//400
            var Department = _employeeService.GetEmployeeId(id.Value);
            if (Department is null) return NotFound();
            return View(Department);

        }
        #endregion
        #region Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id == 0) return BadRequest();
            try
            {
                var Deleted = _employeeService.DeleteEmployee(id);
                if (Deleted)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Employee Is Not Deleted");
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                //1- Developemnt =>
                //Log Error In Console  And  Return Same View With Error Message
                if (_environment.IsDevelopment())
                {
                    return RedirectToAction(nameof(Index));

                }
                //2- Deployment => Log Error In File / Table  in Database And Return  Error View
                else
                {
                    _logger.LogError(ex.Message);
                    return View("ErrorView", ex);

                }
            }
        }
        #endregion
    }
}
