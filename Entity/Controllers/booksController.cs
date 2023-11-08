﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Entity.Data;
using Entity.Models;

namespace Entity.Controllers
{
    public class booksController : Controller
    {
        private readonly EntityContext _context;

        public booksController(EntityContext context)
        {
            _context = context;
        }
        


        // GET: books
        public async Task<IActionResult> Index()
        {
              return _context.book != null ? 
                          View(await _context.book.ToListAsync()) :
                          Problem("Entity set 'EntityContext.book'  is null.");
        }
        public async Task<IActionResult> search()
        {

            List<book> brItems = new List<book>();

            return View(brItems);

        }
        [HttpPost]
        public async Task<IActionResult> search(string s)
        {
            var brItems = await _context.book.FromSqlRaw("select * from book where title LIKE '%" + s + "%' ").ToListAsync();
            return View(brItems);
        }
        // GET: books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.book == null)
            {
                return NotFound();
            }

            var book = await _context.book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: books/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,title,info,price,discount,pubdate,category,bookquantity")] book book)
        {


            



            if (file != null)
            {
                string filename = file.FileName;
                //  string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                book.imgfile = filename;
            }

            _context.Add(book);
                await _context.SaveChangesAsync();
            var li = await _context.categories.ToListAsync();
            ViewBag.Catag = li;

            return RedirectToAction(nameof(Index));
            
            
        }

        // GET: books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.book == null)
            {
                return NotFound();
            }

            var book = await _context.book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile file, int id, [Bind("Id,title,info,price,discount,pubdate,category,bookquantity,imgfile")] book book)
        {
            if (id != book.Id)
            { return NotFound(); }

            if (file != null)
            {
                string filename = file.FileName;
                //  string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                book.imgfile = filename;
            }
            _context.Update(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            return View(book);
        }


        // GET: books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.book == null)
            {
                return NotFound();
            }

            var book = await _context.book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.book == null)
            {
                return Problem("Entity set 'EntityContext.book'  is null.");
            }
            var book = await _context.book.FindAsync(id);
            if (book != null)
            {
                _context.book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool bookExists(int id)
        {
          return (_context.book?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> searchall()
        {

            {
                book brItem = new book();

                return View(brItem);
            }
        }

        [HttpPost]

        public async Task<IActionResult> searchall(string tit)
        {
            var bkItems = await _context.book.FromSqlRaw("select * from book where title = '" + tit + "' ").FirstOrDefaultAsync();

            return View(bkItems);
        }






    }




}

