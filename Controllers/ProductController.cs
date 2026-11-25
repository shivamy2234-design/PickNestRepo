using Microsoft.AspNetCore.Mvc;

namespace PickNest.Controllers
{
    [Route("Admin/{controller}/{action=Index}/{id?}")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int PageSize = 2;
        public ProductController(ApplicationDbContext context,IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index(int PageIndex,string? Search,string? column,string? orderBy)
        {
            IQueryable<Products> query = context.Product;

            //add search code
            if(Search!=null)
            {
                query = query.Where(p => p.Name.Contains(Search) || p.Brand.Contains(Search));
            }

            //sort functionality
            string[] validColumn = { "Id", "Name", "Brand", "Category", "Price", "CreatedAt" };
            string[] validOrderBy = { "desc", "asc" };

            if(!validColumn.Contains(column))
            {
                column = "Id";
            }
            if(!validOrderBy.Contains(orderBy))
            {
                column = "desc";
            }

            //sorting
            if(column=="Id")
            {
                if(orderBy=="asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }
            else if (column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }
            else if (column == "Brand")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (column == "Category")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (column == "CreatedAt")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            //query = query.OrderByDescending(p => p.Id);
            //Paging functionality
            if (PageIndex < 1)
            {
                PageIndex = 1;
            }
            decimal count = query.Count();
            int TotalPages = (int)Math.Ceiling(count/PageSize);
            query = query.Skip((PageIndex - 1) * PageSize).Take(PageSize);

            //for display data on pages
            var products = query.ToList();
            ViewData["PageIndex"] = PageIndex;
            ViewData["TotalPages"] = TotalPages;
            ViewData["Search"] = Search ?? "";
            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;

            //it work before paging
            //var products = context.Product.OrderByDescending(p=>p.Id).ToList();  
            return View(products);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productdto)
        {
            //error check
            if(productdto.ImageFileName==null)
            {
                ModelState.AddModelError("ImageFile", "Image file is required");
            }
            if(!ModelState.IsValid)
            {
                return View(productdto);
            }

            //save images
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productdto.ImageFileName!.FileName);
            string ImageFilePath = environment.WebRootPath + "/Products/" + newFileName;

            using(var stream=System.IO.File.Create(ImageFilePath))
            {
                productdto.ImageFileName.CopyTo(stream);
            };
            //save record
            Products products = new Products
            {
                Name = productdto.Name,
                Brand = productdto.Brand,
                Category = productdto.Category,
                Price = productdto.Price,
                Description = productdto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };

            context.Product.Add(products);
            context.SaveChanges();
            return RedirectToAction("index","Product");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = context.Product.Find(id);
            if(product==null)
            {
                return RedirectToAction("Index", "Product");
            }

            var productdto = new ProductDto()
            {
                Name=product.Name,
                Brand=product.Brand,
                Price=product.Price,
                Description=product.Description
            };

            ViewData["productId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productdto);

        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Product.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            if (!ModelState.IsValid)
            {
                ViewData["productId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            // Keep the existing image if no new image is uploaded
            string newFileName = product.ImageFileName;

            if (productDto.ImageFileName != null)
            {
                // Save the new image
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFileName.FileName);

                string newImagePath = Path.Combine(
                    environment.WebRootPath,
                    "Products",
                    newFileName
                );

                using (var stream = System.IO.File.Create(newImagePath))
                {
                    productDto.ImageFileName.CopyTo(stream);
                }

                // Delete the OLD image
                if (!string.IsNullOrEmpty(product.ImageFileName))
                {
                    string oldImagePath = Path.Combine(
                        environment.WebRootPath,
                        "Products",
                        Path.GetFileName(product.ImageFileName)
                    );

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }

            // Update the existing product record
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }


        public IActionResult Delete(int id)
        {
            var product = context.Product.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            

            string imageFilePath = Path.Combine(
                environment.WebRootPath,
                "Products",
                Path.GetFileName(product.ImageFileName)
            );

            if (System.IO.File.Exists(imageFilePath))
            {
                System.IO.File.Delete(imageFilePath);
            }

            context.Product.Remove(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Product");

        }

    }
}
