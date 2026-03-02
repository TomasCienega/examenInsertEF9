using examenInsertEF9.Context;
using examenInsertEF9.Models;
using examenInsertEF9.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace examenInsertEF9.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly ExamenInsertEf9Context _context;
        public EmpleadoController(ExamenInsertEf9Context context)
        {
            _context=context;
        }
        public async Task<IActionResult> Index(int idDep)
        {
            var vm = new EmpleadoVM();
            ViewBag.IdSeleccionado = idDep;
            try
            {
                vm.ListaDepartamentos = await _context.Departamentos.ToListAsync();
                if (idDep>0)
                {
                    vm.ListaEmpleados = await _context.Empleados.FromSqlRaw("exec sp_ListarEmpleadoPorIdDep {0}",idDep).ToListAsync();
                }
                else
                {
                    vm.ListaEmpleados = await _context.Empleados.Include(tD => tD.IdDepartamentoNavigation).ToListAsync();
                }
                return View(vm);

            }catch (Exception ex)
            {
                vm.ListaEmpleados = new List<Empleado>();
                vm.ListaDepartamentos = new List<Departamento>();
                Console.WriteLine(ex.Message);
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Guardar(EmpleadoVM vm)
        {
            try
            {
                await _context.Empleados.AddAsync(vm.EmpleadoModelReference);
                await _context.SaveChangesAsync();

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");
        }
    }
}
