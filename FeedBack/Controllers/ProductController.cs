
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FeedBack.Data.Models;

namespace FeedBack.Controllers
{
    [Route("feedback-api/v1/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly FeedBackContext _context;

        public ProductController(FeedBackContext context)
        {
            _context = context;
        }

        private string GetProductCodeNew(ReqAddProduct req)
        {
            int superCategoryId = req.SuperCategoryId;

            int categoryId = req.CategoryId;


            var superCategory = _context.supercategories
                .Where(sc => sc.Id == superCategoryId)
                .Select(sc => sc.Name)
                .FirstOrDefault();

            if (superCategory == null)
                throw new ArgumentException("SuperCategory ID not found.");

            string prefix = superCategory switch
            {
                "Laptop Parts" => "LP",
                "Accessories" => "AC",
                "Monitors" => "MN",
                "PC Parts" => "PC",
                _ => throw new ArgumentException("Invalid SuperCategory name.")
            };

            var categoryExists = _context.categories.Any(c => c.Id == categoryId && c.DeletedAt.Equals(null));
            if (!categoryExists)
                throw new ArgumentException("Category ID not found.");

            string baseCode = $"{prefix}{categoryId:D2}";

            var lastCode = _context.products
                .Where(p => p.ProductCode.StartsWith(baseCode))
                .OrderByDescending(p => p.ProductCode)
                .Select(p => p.ProductCode)
                .FirstOrDefault();

            int nextIndex = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                var suffix = lastCode.Substring(baseCode.Length);
                if (int.TryParse(suffix, out int parsed))
                {
                    nextIndex = parsed + 1;
                }
            }

            return $"{baseCode}{nextIndex:D3}";
        }

        //[Authorize]
        [HttpGet]
        public ActionResult GetAll(string title = "", int page = 1, int size = 10)
        {
            var query = _context.products
                .Where(p => string.IsNullOrEmpty(title) || p.Name.Contains(title));

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);
            var response = query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(m => new
                {
                    id = m.Id,
                    product_code = m.ProductCode,
                    name = m.Name,
                    price = m.Price,
                    created_at = m.CreatedAt,
                    updated_at = m.UpdatedAt,
                    delete_id = m.DeletedAt,
                    category = _context.productcategories
                        .Where(mg => mg.ProductId == m.Id)
                        .Join(_context.categories,
                              mg => mg.CategoryId,
                              g => g.Id,
                              (mg, g) => g.Name)
                        .OrderBy(name => name)
                        .ToList()
                })
                .ToList();

            return Ok(new { 
                data = response.Take(size),
                pagination = new
                {
                    currentPage = page,
                    pageSize = size,
                    totalItems,
                    totalPages
                }
            });
        }

        [Authorize]
        [HttpGet("{productId}")]
        public ActionResult getId(int productId, [FromQuery] bool getReview = false)
        {
            var data = _context.products.FirstOrDefault(i => i.Id.Equals(productId) && i.DeletedAt.Equals(null));

            if (data.Equals(null))
                return NotFound(new { message = "Product not found." });

            if (getReview)
            {
                var review = _context.productreviews.Where(i => i.ProductId.Equals(data.Id)).ToList();

                return Ok(new
                {
                    id = data.Id,
                    product_code = data.ProductCode,
                    name = data.Name,
                    price = data.Price,
                    created_at = data.CreatedAt,
                    updated_at = data.UpdatedAt,
                    delete_at = data.DeletedAt,
                    category = _context.productcategories
                        .Where(i => i.ProductId == data.Id)
                        .Join(_context.categories,
                              pc => pc.CategoryId,
                              c => c.Id,
                              (pc, c) => new
                              {
                                  CategoryName = c.Name,
                                  SuperCategoryName = _context.supercategories
                                      .Where(sc => sc.Id == pc.SuperCategoryId)
                                      .Select(sc => sc.Name)
                                      .FirstOrDefault()
                              })
                        .OrderBy(x => x.CategoryName)
                        .Select(x => new[] { x.SuperCategoryName, x.CategoryName })
                        .ToList(),
                    review = review.Select(i => new
                    {
                        id = i.Id,
                        review_at = i.ReviewedAt,
                        customer = i.TransactionId != null
                            ? _context.transactions
                                .Where(t => t.Id == i.TransactionId)
                                .Join(_context.customers,
                                      t => t.CustomerId,
                                      c => c.Id,
                                      (t, c) => new { c.Id, c.Name })
                                .FirstOrDefault()
                            : null,
                        rating = i.Rating,
                        review = i.ReviewText
                    })
                });

            }

            return Ok(new
            {
                id = data.Id,
                product_code = data.ProductCode,
                name = data.Name,
                price = data.Price,
                created_at = data.CreatedAt,
                updated_at = data.UpdatedAt,
                delete_id = data.DeletedAt,
                category = _context.productcategories
                        .Where(i => i.ProductId == data.Id)
                        .Join(_context.categories,
                              pc => pc.CategoryId,
                              c => c.Id,
                              (pc, c) => new
                              {
                                  CategoryName = c.Name,
                                  SuperCategoryName = _context.supercategories
                                      .Where(sc => sc.Id == pc.SuperCategoryId)
                                      .Select(sc => sc.Name)
                                      .FirstOrDefault()
                              })
                        .OrderBy(x => x.CategoryName)
                        .Select(x => new[] { x.SuperCategoryName, x.CategoryName })
                        .ToList(),
                review = new {}
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult post(ReqAddProduct req)
        {
            var data = _context.products.FirstOrDefault(i => i.Name.Equals(req.Name) && i.DeletedAt.Equals(null));

            if (data != null)
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Already a product with the same name.",
                    product_id = data.Id
                });

            var add = new Product
            {
                Name = req.Name,
                Price = req.Price,
                Description = req.Description,
                ProductCode = GetProductCodeNew(req),
            };

            _context.products.Add(add);
            _context.SaveChanges(); 

            _context.productcategories.Add(new ProductCategory
            {
                ProductId = add.Id,
                CategoryId = req.CategoryId,
                SuperCategoryId = req.SuperCategoryId
                    
            });

            _context.SaveChanges();

            var product = _context.products
                .Include(p => p.ProductCategories)
                .FirstOrDefault(p => p.Id == add.Id);

            return Ok(new
            {
                message = "Product successfully added.",
                created_at = DateTime.Now,
                data = new
                {
                    id = product.Id,
                    product_code = product.ProductCode,
                    name = product.Name,
                    price = product.Price,
                    created_at = product.CreatedAt,
                    updated_at = product.UpdatedAt,
                    delete_id = product.DeletedAt,
                    category = _context.productcategories
                        .Where(i => i.ProductId == product.Id)
                        .Join(_context.categories,
                              pc => pc.CategoryId,
                              c => c.Id,
                              (pc, c) => new
                              {
                                  CategoryName = c.Name,
                                  SuperCategoryName = _context.supercategories
                                      .Where(sc => sc.Id == pc.SuperCategoryId)
                                      .Select(sc => sc.Name)
                                      .FirstOrDefault()
                              })
                        .OrderBy(x => x.CategoryName)
                        .Select(x => new[] { x.SuperCategoryName, x.CategoryName })
                        .ToList(),
                },
            });

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{productId}")]
        public ActionResult update(int productId, ReqUpdateProduct req)
        {
            var data = _context.products.FirstOrDefault(i => i.Id.Equals(productId) && i.DeletedAt.Equals(null));

            if (data.Equals(null))
                return NotFound(new { message = "Product not found." });

            var name = _context.products.FirstOrDefault(i => i.Name.Equals(req.Name) && i.DeletedAt.Equals(null));

            if (name != null)
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Already a product with the same name.",
                    product_id = data.Id
                });

            data.Name = req.Name;
            data.Description = req.Description;
            data.Price = req.Price;
            data.UpdatedAt = DateTime.Now;

            _context.products.Update(data);
            _context.SaveChanges();

            return Ok(new {
                message = "Product successfully edited.",
                updated_at = data.UpdatedAt,
                data = new
                {
                    id = data.Id,
                    product_code = data.ProductCode,
                    name = data.Name,
                    price = data.Price,
                    created_at = data.CreatedAt,
                    updated_at = data.UpdatedAt,
                    delete_id = data.DeletedAt,
                    category = _context.productcategories
                        .Where(i => i.ProductId == data.Id)
                        .Join(_context.categories,
                              pc => pc.CategoryId,
                              c => c.Id,
                              (pc, c) => new
                              {
                                  CategoryName = c.Name,
                                  SuperCategoryName = _context.supercategories
                                      .Where(sc => sc.Id == pc.SuperCategoryId)
                                      .Select(sc => sc.Name)
                                      .FirstOrDefault()
                              })
                        .OrderBy(x => x.CategoryName)
                        .Select(x => new[] { x.SuperCategoryName, x.CategoryName })
                        .ToList(),
                },
            });
        }

        [Authorize(Roles = "Admin")]
        [Route("delete/{productId}")]
        [HttpPost]
        public ActionResult Delete(int productId)
        {
            var data = _context.users.FirstOrDefault(i => i.Id == productId && i.DeletedAt.Equals(null));

            if (data == null || data.DeletedAt != null)
                return NotFound(new { message = "User not found or already deleted." });

            data.DeletedAt = DateTime.Now;
            _context.users.Update(data);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User successfully deleted.",
                at = data.DeletedAt
            });
        }
    }
}
