using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Entity.Data;
using Entity.Models;
using Microsoft.Data.SqlClient;

namespace Entity.Controllers
{
    public class ordersController : Controller
    {
        private readonly EntityContext _context;

        public ordersController(EntityContext context)
        {
            _context = context;
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
              return _context.orders != null ? 
                          View(await _context.orders.ToListAsync()) :
                          Problem("Entity set 'EntityContext.orders'  is null.");
        }

        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: orders/Create
        

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
       // public async Task<IActionResult> Create([Bind("Id,bookid,custid,quantity,buydate")] orders orders)
        //{
          //  if (ModelState.IsValid)
            //{
              //  _context.Add(orders);
                //await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
           // return View(orders);
        //}

        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,bookid,custid,quantity,buydate")] orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.Id))
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
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.orders == null)
            {
                return Problem("Entity set 'EntityContext.orders'  is null.");
            }
            var orders = await _context.orders.FindAsync(id);
            if (orders != null)
            {
                _context.orders.Remove(orders);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
          return (_context.orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(int bookId, int quantity)
        {
            orders order = new orders();
            order.bookid = bookId;
            order.quantity = quantity;
            order.custid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            order.buydate = DateTime.Today;
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("EntityContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql;
            int qt = 0;
            sql = "select * from book where (id ='" + order.bookid + "' )";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                qt = (int)reader["bookquantity"]; // store quantity
            }
            reader.Close();
            conn.Close();
            if (order.quantity > qt)
            {
                ViewData["message"] = "maxiumam order quantity should be " + qt;
                var book = await _context.book.FindAsync(bookId);
                return View(book);
            }
            else
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                sql = "UPDATE book  SET bookquantity  = bookquantity   - '" + order.quantity + "'  where (id ='" + order.bookid + "' )";
                comm = new SqlCommand(sql, conn);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
                return RedirectToAction(nameof(index2));
            }
        }



        public async Task <IActionResult> buy(int? id) {

            var book = await _context.book.FindAsync(id);
            string ss = HttpContext.Session.GetString("Role");

            if (ss == "customer")
                return View(book);
            else
                return RedirectToAction("login", "Home");
        }

        [HttpPost]

        public IActionResult buy(int quantity, int bookid ) {
            List<orders> orders = new List<orders>();
            string id = HttpContext.Session.GetString("userid");
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=ziad2;Integrated Security=True;Pooling=False");
            string sql = "select * from orders where Id='"+id+"'";
            SqlCommand comm=new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read()) {
                orders.Add(new orders
                {
                    Id = (int)reader["Id"],
                    bookid=bookid,
                    custid =(int) reader["custid"],
                    quantity=quantity,
                    buydate = (DateTime)reader["buydate"]
                });
            
            
            }
            return View("index2", orders);
                
                }

        


        
        public IActionResult index2(int Id ,[Bind("int bookid,int custid,int quantity, DateTime buydate")]orders order) {
            
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=ziad2;Integrated Security=True;Pooling=False");
            string sql = "SELECT * FROM orders where Id='" + HttpContext.Session.GetString("userid") + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read()) {
                order.bookid = (int)reader["bookid"];
                order.quantity = (int)reader["quantity"];
                order.custid = (int)reader["custid"];
                order.buydate = (DateTime)reader["buydate"];
                     }
            return View(order);
        }
    
    }
}
