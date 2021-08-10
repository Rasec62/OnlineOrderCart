using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.DataBase;

namespace OnlineOrderCart.Web.Controllers
{
    public class TypeofPaymentsController : Controller
    {
        private readonly DataContext _context;

        public TypeofPaymentsController(DataContext context)
        {
            _context = context;
        }

        // GET: TypeofPayments
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypeofPayments.ToListAsync());
        }

        // GET: TypeofPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeofPayments = await _context.TypeofPayments
                .FirstOrDefaultAsync(m => m.TypeofPaymentId == id);
            if (typeofPayments == null)
            {
                return NotFound();
            }

            return View(typeofPayments);
        }

        // GET: TypeofPayments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeofPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeofPaymentId,Description,CodeKey,IsDeleted,RegistrationDate")] TypeofPayments typeofPayments)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typeofPayments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeofPayments);
        }

        // GET: TypeofPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeofPayments = await _context.TypeofPayments.FindAsync(id);
            if (typeofPayments == null)
            {
                return NotFound();
            }
            return View(typeofPayments);
        }

        // POST: TypeofPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeofPaymentId,Description,CodeKey,IsDeleted,RegistrationDate")] TypeofPayments typeofPayments)
        {
            if (id != typeofPayments.TypeofPaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeofPayments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeofPaymentsExists(typeofPayments.TypeofPaymentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(typeofPayments);
        }

        // GET: TypeofPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeofPayments = await _context.TypeofPayments
                .FirstOrDefaultAsync(m => m.TypeofPaymentId == id);
            if (typeofPayments == null)
            {
                return NotFound();
            }

            return View(typeofPayments);
        }

        // POST: TypeofPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeofPayments = await _context.TypeofPayments.FindAsync(id);
            _context.TypeofPayments.Remove(typeofPayments);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeofPaymentsExists(int id)
        {
            return _context.TypeofPayments.Any(e => e.TypeofPaymentId == id);
        }
    }
}
