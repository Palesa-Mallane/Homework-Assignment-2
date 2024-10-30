using _22686640_Homework_Assignment_2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace _22686640_Homework_Assignment_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {


            // Disable the navigation tabs on page load
            ViewBag.CanSell = false;
            ViewBag.CanBuy = false;
            ViewBag.CanViewMyBikes = false;

            // Check if the user is logged in
            if (Session["UserEmail"] != null)
            {
                var role = Session["UserRole"]?.ToString();  // 'Seller', 'Buyer', or 'Both'

                // Enable the correct options based on the user's role
                if (role == "Seller" || role == "Both")
                {
                    ViewBag.CanSell = true;
                    ViewBag.CanViewMyBikes = true;
                }
                if (role == "Buyer" || role == "Both")
                {
                    ViewBag.CanBuy = true;
                    ViewBag.CanViewMyBikes = true;
                }
            }
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult RegisterUser(User model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert the user into the users table
                    string insertUserSql = "INSERT INTO sales.users (user_name, profile_image) OUTPUT INSERTED.user_id VALUES (@Email, @Image_URL);";
                    SqlCommand insertUserCmd = new SqlCommand(insertUserSql, connection);
                    insertUserCmd.Parameters.AddWithValue("@Email", model.Email);
                    insertUserCmd.Parameters.AddWithValue("@Image_URL", model.ProfilePhoto);

                    // Get the new user_id
                    int userId = (int)insertUserCmd.ExecuteScalar(); // Capture the inserted user_id

                    // Insert into user_roles table based on roles from the model
                    if (model.IsBuyer)
                    {
                        // Insert buyer role
                        string insertUserRoleSql = "INSERT INTO sales.user_roles (user_id, role_id) VALUES (@UserId, (SELECT role_id FROM sales.roles WHERE role_name = 'Customer'))";
                        using (SqlCommand insertUserRoleCmd = new SqlCommand(insertUserRoleSql, connection))
                        {
                            insertUserRoleCmd.Parameters.AddWithValue("@UserId", userId);
                            insertUserRoleCmd.ExecuteNonQuery();
                        }

                        // Insert into customers table
                        string insertCustomerSql = "INSERT INTO sales.customers (user_id, first_name, last_name, phone, email, street, city, state, zip_code) VALUES (@UserId, @FirstName, @LastName, @Phone, @Email, @Street, @City, @State, @ZipCode)";
                        using (SqlCommand insertCustomerCmd = new SqlCommand(insertCustomerSql, connection))
                        {
                            insertCustomerCmd.Parameters.AddWithValue("@UserId", userId); // Include user_id in customer record
                            insertCustomerCmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                            insertCustomerCmd.Parameters.AddWithValue("@LastName", model.LastName);
                            insertCustomerCmd.Parameters.AddWithValue("@Phone", model.Phone);
                            insertCustomerCmd.Parameters.AddWithValue("@Email", model.Email);
                            insertCustomerCmd.Parameters.AddWithValue("@Street", model.Street);
                            insertCustomerCmd.Parameters.AddWithValue("@City", model.City);
                            insertCustomerCmd.Parameters.AddWithValue("@State", model.State);
                            insertCustomerCmd.Parameters.AddWithValue("@ZipCode", model.ZipCode);

                            insertCustomerCmd.ExecuteNonQuery();
                        }
                    }

                    if (model.IsSeller)
                    {
                        // Insert seller role
                        string insertUserRoleSql = "INSERT INTO sales.user_roles (user_id, role_id) VALUES (@UserId, (SELECT role_id FROM sales.roles WHERE role_name = 'Seller'))";
                        using (SqlCommand insertUserRoleCmd = new SqlCommand(insertUserRoleSql, connection))
                        {
                            insertUserRoleCmd.Parameters.AddWithValue("@UserId", userId);
                            insertUserRoleCmd.ExecuteNonQuery();
                        }

                        // Insert into sellers table
                        string insertSellerSql = "INSERT INTO sales.sellers (user_id, first_name, last_name, phone, email, street, city, state, zip_code) VALUES (@UserId, @FirstName, @LastName, @Phone, @Email, @Street, @City, @State, @ZipCode)";
                        using (SqlCommand insertSellerCmd = new SqlCommand(insertSellerSql, connection))
                        {
                            insertSellerCmd.Parameters.AddWithValue("@UserId", userId); // Include user_id in seller record
                            insertSellerCmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                            insertSellerCmd.Parameters.AddWithValue("@LastName", model.LastName);
                            insertSellerCmd.Parameters.AddWithValue("@Phone", model.Phone);
                            insertSellerCmd.Parameters.AddWithValue("@Email", model.Email);
                            insertSellerCmd.Parameters.AddWithValue("@Street", model.Street);
                            insertSellerCmd.Parameters.AddWithValue("@City", model.City);
                            insertSellerCmd.Parameters.AddWithValue("@State", model.State);
                            insertSellerCmd.Parameters.AddWithValue("@ZipCode", model.ZipCode);

                            insertSellerCmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // You might want to log the error message for debugging purposes
                return View("Error", new { Message = ex.Message });
            }
        }

        public ActionResult SellSearchResults(string searchTerm)
        {
            List<ProductViewModel> results = new List<ProductViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                            p.product_id, 
                            p.product_name, 
                            b.brand_name, 
                            c.category_name, 
                            p.model_year, 
                            p.list_price,
                            p.image_url
                        FROM 
                            production.products p
                        JOIN 
                            production.brands b ON p.brand_id = b.brand_id
                        JOIN 
                            production.categories c ON p.category_id = c.category_id
                        WHERE 
                            p.product_name LIKE @SearchTerm";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductViewModel
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                BrandName = reader.GetString(2),
                                CategoryName = reader.GetString(3),
                                ModelYear = reader.GetInt16(4),
                                ListPrice = reader.GetDecimal(5),
                                ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url"))
                            };
                            results.Add(product);
                        }
                    }
                }
            }

            return View("Sell", results); // Return the results to the view
        }



        public ActionResult SearchResults(string searchTerm)
        {
            List<ProductViewModel> products = new List<ProductViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                            p.product_id, 
                            p.product_name, 
                            b.brand_name, 
                            c.category_name, 
                            p.model_year, 
                            p.list_price,
                            p.image_url
                        FROM 
                            production.products p
                        JOIN 
                            production.brands b ON p.brand_id = b.brand_id
                        JOIN 
                            production.categories c ON p.category_id = c.category_id
                        WHERE 
                            p.product_name LIKE @SearchTerm";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductViewModel
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                BrandName = reader.GetString(2),
                                CategoryName = reader.GetString(3), 
                                ModelYear = reader.GetInt16(4),
                                ListPrice = reader.GetDecimal(5),
                                ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url"))
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return View(products);
        }

        public ActionResult ProductsByLocation(int countryId)
        {
            // Retrieve the list of products by the specified countryId
            var products = GetProductsByLocation(countryId);
            ViewBag.Location = GetCountryNameById(countryId); 

            // Check if any products were found
            if (products == null || !products.Any())
            {
                ViewBag.Message = "No products found for the selected location.";
                return View(new List<ProductViewModel>()); // Return an empty list if no products found
            }

            //ViewBag.Location = Location;
            return View(products);
        }

        public List<ProductViewModel> GetProductsByLocation(int countryId)
        {
            var products = new List<ProductViewModel>(); // List to hold multiple products
            string query = @"
                SELECT p.product_id, p.product_name, b.brand_name, c.category_name, p.model_year, p.list_price, p.image_url, st.store_name
                FROM [production].[products] p
                INNER JOIN [production].[brands] b ON b.brand_id = p.brand_id
                INNER JOIN [production].[categories] c ON c.category_id = p.category_id
                INNER JOIN [sales].[stores] st ON st.store_id = p.store_id
                WHERE st.country_id = @CountryId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryId", countryId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a new product instance for each row
                                ProductViewModel product = new ProductViewModel
                                {
                                    ProductId = reader.GetInt32(reader.GetOrdinal("product_id")), // Fetch ProductId
                                    ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                    BrandName = reader.GetString(reader.GetOrdinal("brand_name")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                                    ModelYear = reader.GetInt16(reader.GetOrdinal("model_year")),
                                    ListPrice = reader.GetDecimal(reader.GetOrdinal("list_price")),
                                    ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url"))
                                };
                                products.Add(product);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving products: " + ex.Message);
                    }
                }
            }

            return products; // Return the list of products
        }

        public string GetCountryNameById(int countryId)
        {
            string countryName = string.Empty; // Default value in case the country isn't found
            string query = "SELECT country_name FROM sales.countries WHERE country_id = @CountryId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryId", countryId);

                    try
                    {
                        conn.Open();
                        var result = cmd.ExecuteScalar();
                        countryName = result != null ? result.ToString() : "Unknown Location"; // Fallback if country not found
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving country name: " + ex.Message);
                    }
                }
            }

            return countryName; // Return the country name
        }

        public ActionResult Bikes()
        {
            if (Session["UserRole"] != null)
            {
                return View();
            }

            return RedirectToAction("Login", "Home");
        }

        public ActionResult Buy()
        {
            if (Session["UserRole"] != null && (Session["UserRole"].ToString() == "Buyer" || Session["UserRole"].ToString() == "Both"))
            {
                var products = GetAllProducts();
                ViewBag.Categories = GetCategories();
                ViewBag.Brands = GetBrands();
                ViewBag.Locations = GetStoreLocations();

                return View(products);
            }

            return RedirectToAction("Login", "Home");
        }

        public ActionResult SearchProduct(decimal? minPrice, decimal? maxPrice, string brands, string categories, string locations)
        {
            List<ProductViewModel> products = new List<ProductViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                            p.product_id, 
                            p.product_name, 
                            b.brand_name, 
                            c.category_name, 
                            p.model_year, 
                            p.list_price,
                            p.image_url
                        FROM 
                            production.products p
                        JOIN 
                            production.brands b ON p.brand_id = b.brand_id
                        JOIN 
                            production.categories c ON p.category_id = c.category_id
                        JOIN production.seller_product sp ON sp.product_id = p.product_id
                        JOIN
                            sales.stores s ON sp.store_id = s.store_id
                        WHERE 
                            (@Brands IS NULL OR b.brand_id = @Brands)
                            AND (@Categories IS NULL OR c.category_id = @Categories)
                            AND (@Locations IS NULL OR s.store_id = @Locations)
                            AND (@minPrice IS NULL OR p.list_price >= @minPrice)
                            AND (@maxPrice IS NULL OR p.list_price <= @maxPrice)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@minPrice", minPrice ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@maxPrice", maxPrice ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Brands", string.IsNullOrEmpty(brands) ? (object)DBNull.Value : brands);
                    command.Parameters.AddWithValue("@Categories", string.IsNullOrEmpty(categories) ? (object)DBNull.Value : categories);
                    command.Parameters.AddWithValue("@Locations", string.IsNullOrEmpty(locations) ? (object)DBNull.Value : locations);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductViewModel
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                BrandName = reader.GetString(2),
                                CategoryName = reader.GetString(3),
                                ModelYear = reader.GetInt16(4),
                                ListPrice = reader.GetDecimal(5),
                                ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url"))
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return View(products);
        }

        public void AddToCart(int productId)
        {
            int? customerId = HttpContext.Session["BuyerId"] as int?;

            // SQL query to insert a product into the cart
            string insertQuery = @"
                    INSERT INTO [sales].[cart] ([product_id], [customer_id]) VALUES (@Product, @CustomerId)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    // Pass the parameters to the SQL query
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    command.Parameters.AddWithValue("@CustomerId", customerId ?? (object)DBNull.Value);
                    command.ExecuteNonQuery(); // Execute the insert command
                }
            }
        }

        // Update the Order method to use AddToCart with multiple product IDs
        public ActionResult Order(int productId)
        {
            
             AddToCart(productId); // Add the product to the cart

            var cartViewModel = new CartViewModel
            {
                CartProducts = GetCartProducts() // Fetch products from the cart
            };

            return View(cartViewModel); // Pass cartViewModel to the view
        }

        private List<ProductViewModel> GetCartProducts()
        {
            var cartProducts = new List<ProductViewModel>();
            int? customerId = HttpContext.Session["BuyerId"] as int?;

            // SQL query to fetch products from the cart
            string selectQuery = @"
                    SELECT c.product_id, p.product_name, b.brand_name, cat.category_name, p.list_price, p.image_url
                    FROM sales.cart c
                    JOIN production.products p ON c.product_id = p.product_id
                    JOIN production.brands b ON b.brand_id = p.brand_id
                    JOIN production.categories cat ON cat.category_id = p.category_id
                    WHERE c.customer_id = @CustomerId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customerId ?? (object)DBNull.Value);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create a ProductViewModel from the query result
                            var product = new ProductViewModel
                            {
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                ProductName = reader["product_name"].ToString(),
                                BrandName = reader["brand_name"].ToString(),
                                CategoryName = reader["category_name"].ToString(),
                                ListPrice = Convert.ToDecimal(reader["list_price"]),
                                ImageURL = reader["image_url"].ToString(),
                            };
                            // Add the product to the cart list
                            cartProducts.Add(product);
                        }
                    }
                }
            }
            return cartProducts;
        }
        public ActionResult SubmitOrder(OrderViewModel orderViewModel)
        {
            int? customerId = HttpContext.Session["BuyerId"] as int?;

            

            // Start a new order transaction
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into orders table
                    string insertOrderQuery = @"
                            INSERT INTO sales.orders (customer_id, order_status, order_date, required_date, store_id, staff_id) 
                            VALUES (@CustomerId, @OrderStatus, @OrderDate, @RequiredDate, @StoreId, @StaffId);
                            SELECT SCOPE_IDENTITY();"; // Get the new order ID

                    int orderId;

                    using (SqlCommand command = new SqlCommand(insertOrderQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CustomerId", customerId);
                        command.Parameters.AddWithValue("@OrderStatus", 1); // Assuming 1 is for a new order
                        command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        command.Parameters.AddWithValue("@RequiredDate", DateTime.Now.AddDays(7)); // Example: 7 days from now
                        command.Parameters.AddWithValue("@StoreId", orderViewModel.StoreId);
                        command.Parameters.AddWithValue("@StaffId", orderViewModel.StaffId);
                        orderId = Convert.ToInt32(command.ExecuteScalar()); // Get the order ID
                    }

                    // Insert into order_items table
                    string insertOrderItemsQuery = @"
                            INSERT INTO sales.order_items (order_id, item_id, product_id, quantity, list_price, discount) 
                            VALUES (@OrderId, @ItemId, @ProductId, @Quantity, @ListPrice, @Discount)";

                    foreach (var item in orderViewModel.OrderItems)
                    {
                        using (SqlCommand command = new SqlCommand(insertOrderItemsQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@OrderId", orderId);
                            command.Parameters.AddWithValue("@ItemId", item.ItemId); // You can increment this ID or manage as necessary
                            command.Parameters.AddWithValue("@ProductId", item.ProductId);
                            command.Parameters.AddWithValue("@Quantity", item.Quantity);
                            command.Parameters.AddWithValue("@ListPrice", item.ListPrice);
                            command.Parameters.AddWithValue("@Discount", item.Discount); // Assuming you have a discount field
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit(); // Commit the transaction
                }
                catch
                {
                    transaction.Rollback(); // Rollback on error
                    ModelState.AddModelError("", "An error occurred while processing your order.");
                    return View(orderViewModel);
                }
            }
            return RedirectToAction("OrderConfirmation"); // Redirect to confirmation page
        }

        public ActionResult Sell(int? userId)
        {
            var soldProducts = GetProductsSoldByUser();

            // Check if the user is logged in and has the correct role
            var userRole = Session["UserRole"]?.ToString();
            var sellerId = Session["SellerId"] as int?;

            if (userRole == "Seller" || userRole == "Both")
            {
                if (!userId.HasValue)
                {
                    // If userId is not provided, fall back to the logged-in user
                    userId = Session["UserId"] as int?;
                }

                if (!userId.HasValue)
                {
                    // Handle case where user is not logged in or userId is missing
                    return RedirectToAction("Login", "Home");
                }

                // Fetch sold products by the user
                ViewBag.Categories = GetCategories();
                ViewBag.Brands = GetBrands();
                ViewBag.Locations = GetStoreLocations();

                return View(soldProducts);
            }

            return RedirectToAction("Login", "Home");
        }

        public ActionResult UpdateProduct(int productId, string productName, int brandId, int categoryId, int modelYear, decimal listPrice)
        {         
            ViewBag.Brands = GetBrands();
            ViewBag.Categories = GetCategories();
            ViewBag.Locations = GetStoreLocations();

            // Open a new connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE production.products " +
                               "SET product_name = @ProductName, brand_id = @BrandId, category_id = @CategoryId, model_year = @ModelYear, list_price = @ListPrice " +
                               "WHERE product_id = @ProductId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@BrandId", brandId);
                    command.Parameters.AddWithValue("@CategoryId", categoryId);
                    command.Parameters.AddWithValue("@ModelYear", modelYear);
                    command.Parameters.AddWithValue("@ListPrice", listPrice);
                    command.Parameters.AddWithValue("@ProductId", productId);

                    try
                    {
                        // Open the connection
                        connection.Open();

                        // Execute the update command
                        int rowsAffected = command.ExecuteNonQuery();

                        // check if any rows were updated
                        if (rowsAffected == 0)
                        {
                            // Handle the case where no rows were affected (e.g., product not found)
                            ModelState.AddModelError("", "Update failed. Product not found.");
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Handle any SQL exceptions
                        ModelState.AddModelError("", $"Database error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        // Handle any other exceptions
                        ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    }
                }
            }
            return RedirectToAction("Sell"); // Redirect to the same view after the update
        }

        public ActionResult DeleteProduct(int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Use parameterized query to prevent SQL injection
                    SqlCommand myDeleteCommand = new SqlCommand("DELETE FROM production.products WHERE product_id = @ProductId", connection);
                    myDeleteCommand.Parameters.AddWithValue("@ProductId", productId);

                    // Execute the delete command
                    int rowsAffected = myDeleteCommand.ExecuteNonQuery();

                    // Provide user feedback on how many rows were deleted
                    if (rowsAffected > 0)
                    {
                        ViewBag.Message = "Success: " + rowsAffected + " rows deleted.";
                    }
                    else
                    {
                        ViewBag.Message = "No rows were deleted. The product might not exist.";
                    }
                }
            }
            catch (Exception err)
            {
                ViewBag.Message = "Error: " + err.Message;
            }

            return RedirectToAction("Sell");
        }
        public ActionResult ViewSimilarBikes(int productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT p.product_id, p.product_name, p.list_price, p.model_year, p.listing_date, c.category_name, b.brand_name, p.image_url
                    FROM production.products p
                    INNER JOIN production.brands b on b.brand_id = p.brand_id
                    INNER JOIN production.categories c on c.category_id = p.category_id
                    WHERE (p.brand_id = (SELECT p.brand_id FROM production.products p WHERE product_id = @ProductId)
                           OR p.category_id = (SELECT p.category_id FROM production.products p WHERE product_id = @ProductId)
                           OR ABS(p.list_price - (SELECT p.list_price FROM production.products p WHERE product_id = @ProductId)) <= 500)
                    AND product_id != @ProductId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    List<ProductViewModel> similarProducts = new List<ProductViewModel>();

                    while (reader.Read())
                    {
                        var similarProduct = new ProductViewModel
                        {
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            ProductName = reader["product_name"].ToString(),
                            BrandName = reader["brand_name"].ToString(),
                            CategoryName = reader["category_name"].ToString(),
                            ListPrice = Convert.ToDecimal(reader["list_price"]),
                            ModelYear = Convert.ToInt16(reader["model_year"]),
                            ImageURL = reader["image_url"].ToString(),
                            
                        };
                        similarProducts.Add(similarProduct);
                    }
                    return View(similarProducts);
                }           
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult DoLogin(User model)
        {
            // Validate input
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT u.user_id, u.user_name, ur.role_id 
                FROM sales.users u
                INNER JOIN sales.user_roles ur ON ur.user_id = u.user_id
                WHERE u.user_name = @Email";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@Email", SqlDbType.VarChar).Value = model.Email; // Explicitly set parameter type

                    SqlDataReader reader = command.ExecuteReader();

                    List<string> roles = new List<string>();
                    string userEmail = null;
                    int? userId = null;

                    // If user exists, retrieve roles, email, and user ID
                    if (reader.Read())
                    {
                        userId = reader.GetInt32(reader.GetOrdinal("user_id"));
                        userEmail = reader["user_name"].ToString();

                        do
                        {
                            roles.Add(reader["role_id"].ToString());
                        } while (reader.Read());
                    }
                    reader.Close();

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        // Set session variables for email and userId
                        Session["UserEmail"] = userEmail;
                        Session["UserId"] = userId;

                        SetUserRoleInSession(roles);

                        // Retrieve and store seller and customer IDs
                        if (roles.Contains("2")) // Seller or Both
                        {
                            var sellerId = GetSellerId(userEmail, connection);
                            if (sellerId.HasValue)
                            {
                                Session["SellerId"] = sellerId.Value;
                            }
                        }

                        if (roles.Contains("1")) // Buyer or Both
                        {
                            var customerId = GetCustomerId(userEmail, connection);
                            if (customerId.HasValue)
                            {
                                Session["BuyerId"] = customerId.Value; // Make sure this is set correctly
                            }
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }
                }
            }
            return View(model);
        }


        private void SetUserRoleInSession(List<string> roles)
        {
            if (roles.Contains("2") && roles.Contains("1"))
            {
                Session["UserRole"] = "Both";  // Both Seller and Buyer
            }
            else if (roles.Contains("2"))
            {
                Session["UserRole"] = "Seller";  // Only a Seller
            }
            else if (roles.Contains("1"))
            {
                Session["UserRole"] = "Buyer";  // Only a Buyer
            }
        }

        public int? GetUserIdFromSession()
        {
            return Session["UserId"] as int?;
        }


        private int? GetSellerId(string email, SqlConnection connection)
        {
            string query = @"
                SELECT s.seller_id 
                FROM sales.sellers s
                INNER JOIN sales.users u ON u.user_id = s.user_id
                WHERE u.user_name = @Email";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            var sellerId = command.ExecuteScalar();
            if (sellerId != null)
            {
                // Log the seller ID retrieved
                System.Diagnostics.Debug.WriteLine($"Seller ID for {email}: {sellerId}");
            }
            else
            {
                // Log if no seller ID found
                System.Diagnostics.Debug.WriteLine($"No seller ID found for {email}");
            }
            return sellerId != null ? (int?)sellerId : null;
        }

        private int? GetCustomerId(string email, SqlConnection connection)
        {
            string query = @"
                SELECT c.customer_id 
                FROM sales.customers c
                INNER JOIN sales.users u ON u.user_id = c.user_id
                WHERE u.user_name = @Email";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            var customerId = command.ExecuteScalar();
            return customerId != null ? (int?)customerId : null;
        }

        public ActionResult StoreBikes()
        {
            return View();
        }

        public ActionResult DashBoard()
        {
            var userId = GetUserIdFromSession();
            var boughtProducts = GetProductsBoughtByUser();
            var soldProducts = GetProductsSoldByUser();

            var viewModel = new DashboardViewModel
            {
                BoughtProducts = boughtProducts,
                SoldProducts = soldProducts
            };

            return View(viewModel);
        }

        public List<ProductViewModel> GetProductsBoughtByUser()
        {
            int? customerId = HttpContext.Session["BuyerId"] as int?;

            var boughtProducts = new List<ProductViewModel>();

            string query = @"
                    SELECT DISTINCT
	                cu.customer_id,
                    p.product_id, 
                    p.product_name, 
                    b.brand_name, 
                    c.category_name, 
                    p.model_year, 
                    p.list_price, 
                    o.order_date,
                    p.image_url
                    FROM [production].[products] p
                    INNER JOIN [production].[brands] b on b.brand_id = p.brand_id
                    INNER JOIN [production].[categories] c on c.category_id = p.category_id
                    INNER JOIN [sales].[order_items] oi ON oi.product_id = p.product_id
                    INNER JOIN [sales].[orders] o ON o.order_id = oi.order_id
                    INNER JOIN [sales].[customers] cu ON cu.customer_id = o.customer_id
                    WHERE cu.customer_id = @CustomerId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new ProductViewModel
                                {
                                    ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                                    ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                    BrandName = reader.IsDBNull(reader.GetOrdinal("brand_name")) ? null : reader.GetString(reader.GetOrdinal("brand_name")),
                                    CategoryName = reader.IsDBNull(reader.GetOrdinal("category_name")) ? null : reader.GetString(reader.GetOrdinal("category_name")),
                                    ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url")),
                                    TransactionDate = reader.GetDateTime(reader.GetOrdinal("order_date")) // Ensure this field is in the SELECT statement
                                };
                                boughtProducts.Add(product);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving bought products: " + ex.Message);
                    }
                }
            }
            return boughtProducts;
        }

        public List<ProductViewModel> GetProductsSoldByUser()
        {
            int? sellerId = HttpContext.Session["sellerId"] as int?;

            var soldProducts = new List<ProductViewModel>();

            if (!sellerId.HasValue)
            {
                // Handle case when userId is not available
                return soldProducts; // or throw an exception, or return null
            }

            string query = @"
                SELECT DISTINCT
                p.seller_id,
                p.product_id, 
                p.product_name, 
                b.brand_name, 
                c.category_name, 
                p.model_year, 
                p.list_price, 
                p.image_url, 
                s.store_name,
                p.listing_date
                FROM [production].[products] p
                INNER JOIN [production].[brands] b on b.brand_id = p.brand_id
                INNER JOIN [production].[categories] c on c.category_id = p.category_id               
                INNER JOIN [sales].[stores] s ON s.store_id = p.store_id                  
                WHERE p.seller_id = @SellerId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SellerId", sellerId.Value); // Ensure value is used

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var soldProduct = new ProductViewModel
                                {
                                    ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                                    ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                    BrandName = reader.IsDBNull(reader.GetOrdinal("brand_name")) ? null : reader.GetString(reader.GetOrdinal("brand_name")),
                                    CategoryName = reader.IsDBNull(reader.GetOrdinal("category_name")) ? null : reader.GetString(reader.GetOrdinal("category_name")),
                                    ModelYear = reader.GetInt16(reader.GetOrdinal("model_year")),
                                    ListPrice = reader.GetDecimal(reader.GetOrdinal("list_price")),
                                    ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url")),
                                    StoreName = reader.IsDBNull(reader.GetOrdinal("store_name")) ? null : reader.GetString(reader.GetOrdinal("store_name"))
                                };
                                soldProducts.Add(soldProduct);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving sold products: " + ex.Message);
                    }
                }
            }

            return soldProducts;
        }
        public ActionResult Price(decimal? minPrice, decimal? maxPrice)
        {
            // When no prices are provided, get all products
            if (minPrice == null && maxPrice == null)
            {
                var allProducts = GetAllProducts();
                return View(allProducts);
            }

            // When prices are provided, get filtered products
            var products = GetProductsByPrice(minPrice, maxPrice);
            return View(products);
        }
        private List<ProductViewModel> GetAllProducts()
        {
            var products = new List<ProductViewModel>();

            string query = @"SELECT p.product_id, p.product_name, b.brand_name, c.category_name, 
                    p.model_year, p.list_price, p.image_url
                     FROM [production].[products] p
                     INNER JOIN [production].[brands] b ON b.brand_id = p.brand_id
                     INNER JOIN [production].[categories] c ON c.category_id = p.category_id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProductViewModel product = new ProductViewModel
                                {
                                    ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                                    ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                    BrandName = reader.GetString(reader.GetOrdinal("brand_name")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                                    ModelYear = reader.GetInt16(reader.GetOrdinal("model_year")),
                                    ListPrice = reader.GetDecimal(reader.GetOrdinal("list_price")),
                                    ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url"))
                                };
                                products.Add(product);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving products: " + ex.Message);
                    }
                }
            }
            return products;
        }

        public List<ProductViewModel> GetProductsByPrice(decimal? minPrice = null, decimal? maxPrice = null)
        {
            var products = new List<ProductViewModel>();

            string query = @"SELECT p.product_id, p.product_name, b.brand_name, c.category_name, 
                            p.model_year, p.list_price, p.image_url, st.store_name
                             FROM [production].[products] p
                             INNER JOIN [production].[brands] b ON b.brand_id = p.brand_id
                             INNER JOIN [production].[categories] c ON c.category_id = p.category_id
							 INNER JOIN [sales].[stores] st on st.store_id = p.store_id
                             WHERE p.list_price >= @MinPrice 
                             AND p.list_price <= @MaxPrice";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Set parameters, allowing null values for min and max prices
                    cmd.Parameters.AddWithValue("@MinPrice", (object)minPrice ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaxPrice", (object)maxPrice ?? DBNull.Value);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProductViewModel product = new ProductViewModel
                                {
                                    ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                                    ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                    BrandName = reader.GetString(reader.GetOrdinal("brand_name")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                                    ModelYear = reader.GetInt16(reader.GetOrdinal("model_year")),
                                    ListPrice = reader.GetDecimal(reader.GetOrdinal("list_price")),
                                    ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url"))
                                };
                                products.Add(product); // Add product to the list
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving products: " + ex.Message);
                    }
                }
            }

            return products; // Return the list of products
        }

        public ActionResult Stores()
        {
            var allProducts = GetAllProducts();

            return View(allProducts);
        }

        public ActionResult Brands()
        {

            return View();
        }

        public ActionResult ProductDetails(int productId)
        {
            // Retrieve the product details for the given productId
            var product = GetProductById(productId);

            // Check if the product is null
            if (product == null)
            {
                // Display an appropriate message or redirect to a different page
                ViewBag.Message = "Product not found.";
                return RedirectToAction("Index", "Home"); //Redirect to home
            }

            

            // Return the view with the ProductViewModel
            return View(product);
        }
       
        public ActionResult OrderProductDetails(int productId)
        {
            // Retrieve the product details for the given productId
            var product = GetProductById(productId);

            // Check if the product is null
            if (product == null)
            {
                // Display an appropriate message or redirect to a different page
                ViewBag.Message = "Product not found.";
                return RedirectToAction("Buy", "Home"); // Redirect to home or a different view
            }


            return View(product);
        }

        public ProductViewModel GetProductById(int productId)
        {
            ProductViewModel product = null; // To store the product data
            string query = @"SELECT 
                        p.product_id, 
                        p.product_name, 
                        b.brand_name, 
                        c.category_name, 
                        p.model_year, 
                        p.list_price, 
                        p.image_url, 
                        st.store_name,
                        s.first_name + ' ' + s.last_name AS Seller,  
                        s.phone,
                        s.email,
                        u.profile_image
                        FROM [production].[products] p
                        INNER JOIN [production].[brands] b ON b.brand_id = p.brand_id
                        INNER JOIN [production].[categories] c ON c.category_id = p.category_id
                        INNER JOIN [sales].[stores] st ON st.store_id = p.store_id
                        INNER JOIN [sales].[sellers] s ON s.seller_id = p.seller_id  
                        INNER JOIN [sales].[users] u ON u.user_name = s.email
                        WHERE p.product_id = @ProductId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Map the data to the ProductViewModel
                                product = new ProductViewModel
                                {
                                    ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                    BrandName = reader.GetString(reader.GetOrdinal("brand_name")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                                    ModelYear = reader.GetInt16(reader.GetOrdinal("model_year")), 
                                    ListPrice = reader.GetDecimal(reader.GetOrdinal("list_price")),
                                    ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url")),
                                    StoreName = reader.GetString(reader.GetOrdinal("store_name")),
                                    SellerName = reader.GetString(reader.GetOrdinal("Seller")),
                                    SellerEmail = reader.GetString(reader.GetOrdinal("email")),
                                    SellerPhone = reader.GetString(reader.GetOrdinal("phone")),
                                    SellerImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : reader.GetString(reader.GetOrdinal("profile_image")),
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving product details: " + ex.Message);
                    }
                }
            }
            return product; // Return the product details (or null if not found)
        }

        public ActionResult BrandBikes(int brandId)
        {
            // Retrieve the list of products by the specified brandId
            var products = GetProductsByBrand(brandId);

            // Check if any products were found
            if (products == null || !products.Any())
            {
                ViewBag.Message = "No products found for the selected brand.";
                return View(new List<ProductViewModel>()); // Return an empty list if no products found
            }

            // Set the brand name in ViewBag for displaying it in the view, using the first product's brand name
            ViewBag.BrandName = products.First().BrandName;


            return View(products);
        }

        public List<ProductViewModel> GetProductsByBrand(int brandId)
        {
            var products = new List<ProductViewModel>(); // List to hold multiple products
            string query = @"
                SELECT p.product_id, p.product_name, b.brand_name, c.category_name, p.model_year, p.list_price, p.image_url
                FROM [production].[products] p
                INNER JOIN [production].[brands] b ON b.brand_id = p.brand_id
                INNER JOIN [production].[categories] c ON c.category_id = p.category_id
                WHERE p.brand_id = @BrandId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BrandId", brandId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a new product instance for each row
                                ProductViewModel product = new ProductViewModel
                                {
                                    ProductId = reader.GetInt32(reader.GetOrdinal("product_id")), // Fetch ProductId
                                    ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                    BrandName = reader.GetString(reader.GetOrdinal("brand_name")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                                    ModelYear = reader.GetInt16(reader.GetOrdinal("model_year")),
                                    ListPrice = reader.GetDecimal(reader.GetOrdinal("list_price")),
                                    ImageURL = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url"))
                                };
                                products.Add(product); // Add product to the list
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving products: " + ex.Message);
                    }
                }
            }

            return products; // Return the list of products
        }

        public ActionResult Buyers(string storeLocation = null, DateTime? orderDate = null)
        {
            ViewBag.Stores = GetStoreLocations();

            List<CustomerViewModel> buyers;

            // When no filters are applied, get all buyers
            if (string.IsNullOrEmpty(storeLocation) && !orderDate.HasValue)
            {
                buyers = GetAllBuyers();
            }
            else if (!string.IsNullOrEmpty(storeLocation) && orderDate.HasValue)
            {
                // If both filters are applied
                var buyersByLocation = FilterBuyersByStore(storeLocation);
                var buyersByDate = FilterBuyersByDate(orderDate);

                // Combine the results from both filters 
                buyers = buyersByLocation.Intersect(buyersByDate).ToList();
            }
            else if (!string.IsNullOrEmpty(storeLocation))
            {
                buyers = FilterBuyersByStore(storeLocation);
            }
            else 
            {
                buyers = FilterBuyersByDate(orderDate);
            }

            return View(buyers);
        }
        public List<CustomerViewModel> GetAllBuyers()
        {
            var buyers = new List<CustomerViewModel>();

            string query = @"SELECT u.profile_image, c.first_name + ' ' + c.last_name AS Name, 
                    s.store_name AS StoreLocation, 
                    o.order_date AS OrderDate, 
                    st.first_name + ' ' + st.last_name AS StaffName
                    FROM sales.orders o
                    JOIN sales.customers c ON o.customer_id = c.customer_id
					JOIN sales.users u ON u.user_id = c.user_id
                    JOIN sales.staffs st ON o.staff_id = st.staff_id
                    JOIN sales.stores s ON o.store_id = s.store_id";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CustomerViewModel buyer = new CustomerViewModel
                                {
                                    CustomerName = reader["Name"].ToString(),
                                    StoreLocation = reader["StoreLocation"].ToString(),
                                    OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                    StaffName = reader["StaffName"].ToString(),
                                    ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : reader.GetString(reader.GetOrdinal("profile_image"))
                                };

                                buyers.Add(buyer);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving products: " + ex.Message);
                    }
                }
            }
            return buyers;
        }
        
        public List<CustomerViewModel> FilterBuyersByStore(string storeLocation)
        {
            var buyers = new List<CustomerViewModel>();

            string query = @"
                    SELECT 
                        u.profile_image,
                        c.first_name + ' ' + c.last_name AS Name, 
                        s.store_name AS StoreLocation, 
                        o.order_date, 
                        st.first_name + ' ' + st.last_name AS StaffName
                    FROM sales.orders o
                    JOIN sales.customers c ON o.customer_id = c.customer_id
                    JOIN sales.staffs st ON o.staff_id = st.staff_id
                    JOIN sales.stores s ON o.store_id = s.store_id
                    JOIN sales.users u ON u.user_id =  c.user_id
                    WHERE (@StoreLocation IS NULL OR s.store_id = @StoreLocation)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StoreLocation", (object)storeLocation ?? DBNull.Value);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buyers.Add(new CustomerViewModel
                            {
                                CustomerName = reader["Name"].ToString(),
                                StoreLocation = reader["StoreLocation"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["order_date"]),
                                StaffName = reader["StaffName"].ToString(),
                                ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : reader.GetString(reader.GetOrdinal("profile_image"))
                            });
                        }
                    }
                }
            }
            return buyers;
        }
        public List<CustomerViewModel> FilterBuyersByDate(DateTime? orderDate)
        {
            var buyers = new List<CustomerViewModel>();

            string query = @"
                    SELECT 
                        u.profile_image,
                        c.first_name + ' ' + c.last_name AS Name, 
                        s.store_name AS StoreLocation, 
                        o.order_date, 
                        st.first_name + ' ' + st.last_name AS StaffName
                    FROM sales.orders o
                    JOIN sales.customers c ON o.customer_id = c.customer_id
                    JOIN sales.staffs st ON o.staff_id = st.staff_id
                    JOIN sales.stores s ON o.store_id = s.store_id
                    JOIN sales.users u ON u.user_id =  c.user_id
                    WHERE (@OrderDate IS NULL OR CAST(o.order_date AS DATE) = CAST(@OrderDate AS DATE))";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderDate", orderDate.HasValue ? (object)orderDate.Value : DBNull.Value);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buyers.Add(new CustomerViewModel
                            {
                                CustomerName = reader["Name"].ToString(),
                                StoreLocation = reader["StoreLocation"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["order_date"]),
                                StaffName = reader["StaffName"].ToString(),
                                ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : reader.GetString(reader.GetOrdinal("profile_image"))
                            });
                        }
                    }
                }
            }
            return buyers;
        }

        private List<string> GetStores()
        {
            var stores = new List<string>();

            string query = "SELECT DISTINCT store_name FROM sales.stores";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stores.Add(reader["store_name"].ToString());
                        }
                    }
                }
            }
            return stores;
        }


        public ActionResult Sellers(string storeLocation = null, DateTime? orderDate = null)
        {
            ViewBag.Stores = GetStoreLocations();

            List<SellerViewModel> sellers;

            // When no filters are applied, get all sellers
            if (string.IsNullOrEmpty(storeLocation) && !orderDate.HasValue)
            {
                sellers = GetAllSellers();
            }
            else if (!string.IsNullOrEmpty(storeLocation) && orderDate.HasValue)
            {
                // If both filters are applied, you can decide to either combine them or choose one
                var sellersByLocation = GetSellersByStoreLocation(storeLocation);
                var sellersByDate = GetSellersByOrderDate(orderDate);

                // Combine the results from both filters if needed (this could be a union or intersection based on your requirements)
                sellers = sellersByLocation.Intersect(sellersByDate).ToList(); // Example: get common sellers
            }
            else if (!string.IsNullOrEmpty(storeLocation))
            {
                sellers = GetSellersByStoreLocation(storeLocation);
            }
            else // orderDate.HasValue
            {
                sellers = GetSellersByOrderDate(orderDate);
            }

            return View(sellers);
        }

        public List<SellerViewModel> GetAllSellers()
        {
            var sellers = new List<SellerViewModel>();
            var sellerDict = new Dictionary<int, SellerViewModel>();

            string query = @"
                    SELECT DISTINCT
                    s.seller_id,
                    s.first_name + ' ' + s.last_name AS SellerName, 
                    s.email,
                    s.phone,
                    u.profile_image,
                    p.product_id,
                    p.product_name,
                    s.street + ', ' + s.city + ', ' + s.state + ' ' + s.zip_code AS Location
                    FROM sales.sellers s
                    INNER JOIN sales.users u ON s.user_id = u.user_id                 
                    INNER JOIN production.products p ON p.seller_id = s.seller_id
                    INNER JOIN sales.order_items oi ON oi.product_id = p.product_id
                    INNER JOIN sales.orders o ON o.order_id = oi.order_id
                    ORDER BY s.seller_id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int sellerId = Convert.ToInt32(reader["seller_id"]);

                            if (!sellerDict.TryGetValue(sellerId, out var seller))
                            {
                                seller = new SellerViewModel
                                {
                                    SellerId = sellerId,
                                    SellerName = reader["SellerName"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    ProfilePhoto = reader["profile_image"] != DBNull.Value ? reader["profile_image"].ToString() : null,
                                    ProductsListed = new List<Product>()
                                };

                                sellerDict[sellerId] = seller; // Add to dictionary
                            }

                            if (reader["product_id"] != DBNull.Value)
                            {
                                var product = new Product
                                {
                                    ProductId = Convert.ToInt32(reader["product_id"]),
                                    ProductName = reader["product_name"].ToString()
                                };

                                seller.ProductsListed.Add(product); // Add product to seller
                            }
                        }
                    }
                }
            }

            return sellerDict.Values.ToList(); // Return unique sellers
        }

        public List<SellerViewModel> GetSellersByStoreLocation(string storeLocation)
        {
            var sellerDict = new Dictionary<int, SellerViewModel>();
            var sellers = new List<SellerViewModel>();

            string query = @"
        SELECT DISTINCT
            s.seller_id,
            s.first_name + ' ' + s.last_name AS SellerName, 
            s.email,
            s.phone,
            u.profile_image,
            p.product_id,
            p.product_name,
            s.street + ', ' + s.city + ', ' + s.state + ' ' + s.zip_code AS Location
            FROM sales.sellers s
            INNER JOIN sales.users u ON s.user_id = u.user_id      
            INNER JOIN production.products p ON p.seller_id = s.seller_id
            INNER JOIN sales.order_items oi ON oi.product_id = p.product_id
            INNER JOIN sales.orders o ON o.order_id = oi.order_id
            WHERE (@StoreLocation IS NULL OR p.store_id = @StoreLocation)
            ORDER BY s.seller_id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StoreLocation", (object)storeLocation ?? DBNull.Value);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int sellerId = Convert.ToInt32(reader["seller_id"]);

                            // Check if seller already exists in the dictionary
                            if (!sellerDict.TryGetValue(sellerId, out var seller))
                            {
                                seller = new SellerViewModel
                                {
                                    SellerId = sellerId,
                                    SellerName = reader["SellerName"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    ProfilePhoto = reader["profile_image"] != DBNull.Value ? reader["profile_image"].ToString() : null,
                                    ProductsListed = new List<Product>()
                                };

                                sellerDict[sellerId] = seller; // Add to dictionary
                            }

                            if (reader["product_id"] != DBNull.Value)
                            {
                                var product = new Product
                                {
                                    ProductId = Convert.ToInt32(reader["product_id"]),
                                    ProductName = reader["product_name"].ToString()
                                };

                                seller.ProductsListed.Add(product); // Add product to seller
                            }
                        }
                    }
                }
            }

            return sellerDict.Values.ToList(); // Return unique sellers
        }

        public List<SellerViewModel> GetSellersByOrderDate(DateTime? orderDate)
        {
            var sellerDict = new Dictionary<int, SellerViewModel>();
            var sellers = new List<SellerViewModel>();

            string query = @"
        SELECT DISTINCT
            s.seller_id,
            s.first_name + ' ' + s.last_name AS SellerName, 
            s.email,
            s.phone,
            u.profile_image,
            p.product_id,
            p.product_name,
            s.street + ', ' + s.city + ', ' + s.state + ' ' + s.zip_code AS Location
        FROM sales.sellers s
        INNER JOIN sales.users u ON s.user_id = u.user_id       
        INNER JOIN production.products p ON p.seller_id = s.seller_id
        INNER JOIN sales.order_items oi ON oi.product_id = p.product_id
        INNER JOIN sales.orders o ON o.order_id = oi.order_id
        WHERE (@OrderDate IS NULL OR CAST(o.order_date AS DATE) = CAST(@OrderDate AS DATE))
        ORDER BY s.seller_id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderDate", orderDate.HasValue ? (object)orderDate.Value : DBNull.Value);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int sellerId = Convert.ToInt32(reader["seller_id"]);

                            // Check if seller already exists in the dictionary
                            if (!sellerDict.TryGetValue(sellerId, out var seller))
                            {
                                seller = new SellerViewModel
                                {
                                    SellerId = sellerId,
                                    SellerName = reader["SellerName"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    ProfilePhoto = reader["profile_image"] != DBNull.Value ? reader["profile_image"].ToString() : null,
                                    ProductsListed = new List<Product>()
                                };

                                sellerDict[sellerId] = seller; // Add to dictionary
                            }

                            if (reader["product_id"] != DBNull.Value)
                            {
                                var product = new Product
                                {
                                    ProductId = Convert.ToInt32(reader["product_id"]),
                                    ProductName = reader["product_name"].ToString()
                                };

                                seller.ProductsListed.Add(product); // Add product to seller
                            }
                        }
                    }
                }
            }

            return sellerDict.Values.ToList(); // Return unique sellers
        }


        private SelectList GetStoreLocations()
        {
            var locations = new List<SelectListItem>();

            string query = "SELECT store_id, store_name FROM sales.stores";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            locations.Add(new SelectListItem
                            {
                                Value = reader["store_id"].ToString(),
                                Text = reader["store_name"].ToString()
                            });
                        }
                    }
                }
            }

            return new SelectList(locations, "Value", "Text");
        }

        private SelectList GetBrands()
        {
            var brands = new List<SelectListItem>();

            string query = "SELECT brand_id, brand_name FROM production.brands";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            brands.Add(new SelectListItem
                            {
                                Value = reader["brand_id"].ToString(),
                                Text = reader["brand_name"].ToString()
                            });
                        }
                    }
                }
            }

            return new SelectList(brands, "Value", "Text");
        }

        private SelectList GetCategories()
        {
            var categories = new List<SelectListItem>();

            string query = "SELECT category_id, category_name FROM production.categories";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new SelectListItem
                            {
                                Value = reader["category_id"].ToString(),
                                Text = reader["category_name"].ToString()
                            });
                        }
                    }
                }
            }
            return new SelectList(categories, "Value", "Text");
        }

        public ActionResult UploadProductForSale(string ProductName, decimal ListPrice, int ModelYear, string BrandId, int CategoryId, int StoreId, HttpPostedFileBase ImageURL)
        {
            try
            {
                // Get the seller ID from session
                int sellerId = (int)Session["SellerId"];
                DateTime listingDate = DateTime.Now;
                byte[] imageData = null; // Initialize byte array for image data

                // Handle file upload
                if (ImageURL != null && ImageURL.ContentLength > 0)
                {
                    // Convert image to byte array
                    using (var binaryReader = new BinaryReader(ImageURL.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(ImageURL.ContentLength);
                    }
                }

                // Save product details to database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // SQL query to insert product details into the database
                    string insertProductQuery = @"
                INSERT INTO production.products
                (product_name, list_price, brand_id, category_id, model_year, image_data, store_id, seller_id, listing_date)
                VALUES (@ProductName, @ListPrice, @BrandId, @CategoryId, @ModelYear, @ImageData, @StoreId, @SellerId, @ListingDate)";

                    using (SqlCommand cmd = new SqlCommand(insertProductQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", ProductName);
                        cmd.Parameters.AddWithValue("@ListPrice", ListPrice);
                        cmd.Parameters.AddWithValue("@BrandId", BrandId);
                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@ModelYear", ModelYear);
                        cmd.Parameters.AddWithValue("@ImageData", imageData); 
                        cmd.Parameters.AddWithValue("@StoreId", StoreId);
                        cmd.Parameters.AddWithValue("@SellerId", sellerId);
                        cmd.Parameters.AddWithValue("@ListingDate", listingDate);

                        cmd.ExecuteNonQuery();
                    }

                    ViewBag.Message = "Success: The product has been added for sale!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
                return View("Error"); // Handle error view appropriately
            }

            return RedirectToAction("Sell"); // Redirect to the manage products page
        }

        private List<ProductViewModel> GetSellerProducts()
        {
            int sellerId = (int)Session["SellerId"];

            var products = new List<ProductViewModel>();

            string query = @"SELECT 
                    s.seller_id,
                    p.product_id, 
                    p.product_name, 
                    p.list_price, 
                    p.model_year,                   
                    b.brand_name, 
                    c.category_name, 
                    p.image_url,
                    p.listing_date
                 FROM 
                    production.products p
                 
                 INNER JOIN 
                    production.brands b ON p.brand_id = b.brand_id
                 INNER JOIN 
                    production.categories c ON p.category_id = c.category_id
                 INNER JOIN 
                    sales.sellers s ON s.seller_id= p.seller_id
                 WHERE 
                    s.seller_id = @SellerId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SellerId", sellerId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductViewModel
                            {
                                
                                ProductName = reader["product_name"].ToString(),
                                ListPrice = Convert.ToDecimal(reader["list_price"]),
                                ModelYear = Convert.ToInt16(reader["model_year"]),
                                BrandName = reader["brand_name"].ToString(),
                                CategoryName = reader["category_name"].ToString(),
                                ImageURL = reader["image_url"].ToString()
                                
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        ///SUMMARIES
        ////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult NewStocks()
        {
            int newStockCount;

            string query = @"
                    SELECT COUNT(*) 
                    FROM production.products 
                    WHERE listing_date >= DATEADD(DAY, -30, GETDATE())"; // Filter for the last 30 days

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    newStockCount = (int)cmd.ExecuteScalar();
                }
            }

            // Display the result as a message
            TempData["Message"] = $"Total new stocks in the last 30 days: {newStockCount}";
            return RedirectToAction("Index"); // Redirect back to the Index view
        }
        public ActionResult ListedForSale()
        {
            int listedCount;

            string query = @"
                SELECT COUNT(*) 
                FROM production.stocks"; 

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    listedCount = (int)cmd.ExecuteScalar();
                }
            }

            TempData["MessageTitle"] = "Listed for Sale";
            TempData["Message"] = $"Total number of bicycles available for sale: {listedCount}.";
            return RedirectToAction("Index");
        }

        public ActionResult TotalSold()
        {
            int totalSoldCount;

            string query = @"
                SELECT COUNT(*) AS TotalSold
                FROM sales.order_items
                WHERE order_id IN (
                        SELECT order_id
                        FROM sales.orders)
            " ;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    totalSoldCount = (int)cmd.ExecuteScalar();
                }
            }

            TempData["MessageTitle"] = "Total Sold";
            TempData["Message"] = $"Total number of bicycles sold: {totalSoldCount}.";
            return RedirectToAction("Index");
        }

        public ActionResult SalesPerBrand()
        {
            var salesPerBrand = new Dictionary<string, int>();

            string query = @"
                    SELECT b.brand_name, COUNT(o.order_id) AS SoldCount
                    FROM sales.order_items o
                    JOIN production.products p ON o.product_id = p.product_id
                    JOIN production.brands b ON p.brand_id = b.brand_id
                    GROUP BY b.brand_name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            salesPerBrand[reader["brand_name"].ToString()] = (int)reader["SoldCount"];
                        }
                    }
                }
            }

            TempData["MessageTitle"] = "Sales per Brand";
            TempData["Message"] = string.Join("<br />", salesPerBrand.Select(kv => $"{kv.Key}: {kv.Value} sold"));
            return RedirectToAction("Index");
        }

        public ActionResult ListingsPerBrand()
        {
            var listingsPerBrand = new Dictionary<string, int>();

            string query = @"
                SELECT b.brand_name, COUNT(p.product_id) AS ListingsCount
                FROM production.products p
                JOIN production.brands b ON p.brand_id = b.brand_id
                GROUP BY b.brand_name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listingsPerBrand[reader["brand_name"].ToString()] = (int)reader["ListingsCount"];
                        }
                    }
                }
            }

            TempData["MessageTitle"] = "Listings per Brand";
            TempData["Message"] = string.Join("<br />", listingsPerBrand.Select(kv => $"{kv.Key}: {kv.Value} listings"));
            return RedirectToAction("Index");
        }

        public ActionResult AvgSalesPerBrand()
        {
            var avgSalesPerBrand = new Dictionary<string, decimal>();

            string query = @"
                SELECT b.brand_name, AVG(p.list_price) AS AvgPrice
                FROM sales.order_items oi
                JOIN production.products p ON oi.product_id = p.product_id
                JOIN production.brands b ON p.brand_id = b.brand_id
                GROUP BY b.brand_name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            avgSalesPerBrand[reader["brand_name"].ToString()] = (decimal)reader["AvgPrice"];
                        }
                    }
                }
            }

            TempData["MessageTitle"] = "Average Sales per Brand";
            TempData["Message"] = string.Join("<br />", avgSalesPerBrand.Select(kv => $"{kv.Key}: ${kv.Value:F2}"));
            return RedirectToAction("Index");
        }

        public ActionResult TotalsPerBrandCategory()
        {
            var totalsPerBrandCategory = new Dictionary<string, int>();

            string query = @"
                SELECT b.brand_name, c.category_name, COUNT(p.product_id) AS Count
                FROM production.products p
                JOIN production.categories c ON p.category_id = c.category_id
                JOIN production.brands b ON b.brand_id = p.brand_id
                GROUP BY b.brand_name, c.category_name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string brandCategory = $"{reader["brand_name"]} - {reader["category_name"]}";
                            totalsPerBrandCategory[brandCategory] = (int)reader["Count"];
                        }
                    }
                }
            }

            TempData["MessageTitle"] = "Totals per Brand Category";
            TempData["Message"] = string.Join("<br />", totalsPerBrandCategory.Select(kv => $"{kv.Key}: {kv.Value} bicycles"));

            return RedirectToAction("Index");
        }


        public ActionResult TotalsPerStore()
        {
            var totalsPerStore = new Dictionary<string, int>();

            string query = @"
                SELECT s.store_name, COUNT(p.product_id) AS Count
                FROM production.products p
                JOIN sales.stores s ON p.store_id = s.store_id                
                GROUP BY s.store_name";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totalsPerStore[reader["store_name"].ToString()] = (int)reader["Count"];
                        }
                    }
                }
            }

            TempData["MessageTitle"] = "Totals per Store";
            TempData["Message"] = string.Join("<br />", totalsPerStore.Select(kv => $"{kv.Key}: {kv.Value} bicycles available"));
            return RedirectToAction("Index");
        }

    }
}