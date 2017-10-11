using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FurnitureFactory.DBAgent;
using FurnitureFactory.Models;

namespace FurnitureFactory.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.Products = agent.GetProducts(null, null, null, null);
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Admin()
        {
            getAdmin();
            return View();
        }

        public ActionResult Orders()
        {
            getAdmin();
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                List<Order> orders = agent.GetOrders(null, null, null, null, null);
                List<Product> products = agent.GetProducts(null, null, null, null);
                List<Purchaser> purchasers = agent.getPurchasers();
                ViewBag.Orders = orders;
                ViewBag.products = products;
                ViewBag.purchasers = purchasers;
                ViewBag.workgroups = agent.GetWorkgroups();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Orders(int? productID, int? purchaser, DateTime? startDate, DateTime? endDate)
        {
            getAdmin();
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                List<Order> orders = agent.GetOrders(productID, purchaser, startDate, endDate, null);
                List<Product> products = agent.GetProducts(null, null, null, null);
                List<Purchaser> purchasers = agent.getPurchasers();
                ViewBag.Orders = orders;
                ViewBag.products = products;
                ViewBag.purchasers = purchasers;
                ViewBag.workgroups = agent.GetWorkgroups();
            }
            return View();
        }

        public ActionResult NewOrder()
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.products = agent.GetProducts(null, null, null, null);
                ViewBag.purchasers = agent.getPurchasers();
            }
            return View();
        }

        [HttpPost]
        public ActionResult NewOrder(int? productID, int? purchaserID, int? number)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                bool result = agent.NewOrder(productID, purchaserID, number);
                if (result)
                    return Redirect("/Home/Orders");
                else
                    return View("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult NewProduct()
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.categories = agent.GetProdCategories();
            }
            return View();
        }

        [HttpPost]
        public ActionResult NewProduct(int? price, string name, int? categoryID, int? salary)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                bool result = agent.NewProduct(name, price, categoryID, salary);
                if (result)
                    return Redirect("/Home/Index");
                else
                    return View("~/Views/Shared/Error.cshtml");
            }
        }

        [HttpGet]
        public ActionResult DeleteProduct(int ID)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.DeleteProduct(ID);
            }
            if (res)
                return Redirect("/Home/Index");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        [HttpGet]
        public ActionResult EditProduct(string name, int ID, int? price, int? categoryID, int? salary)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.categories = agent.GetProdCategories();
                ViewBag.ID = ID;
                ViewBag.product = new Product
                {
                    ID = ID,
                    name = name,
                    price = price,
                    categoryID = categoryID,
                    salary = salary
                };
                ViewBag.method = "EditProduct";
            }
            return View("NewProduct");
        }

        [HttpPost]
        public ActionResult EditProduct(int ID, string name, int? price, int? categoryID, int? salary)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {

                res = agent.EditProduct(ID, name, price, categoryID, salary);
            }
            if (res)
                return Redirect("/Home/Index");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        public ActionResult Purchasers()
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                List<Purchaser> purchasers = agent.getPurchasers();
                ViewBag.purchasers = purchasers;
            }
            return View();
        }

        public ActionResult NewPurchaser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewPurchaser(Purchaser purchaser)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.NewPurchaser(purchaser);
            }
            if (res)
                return Redirect("/Home/Purchasers");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        public ActionResult Employees()
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.employees = agent.GetEmployees(null, null, null, null);
                ViewBag.workgroups = agent.GetWorkgroups();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Employees(int? workgroup)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.employees = agent.GetEmployees(null, workgroup, null, null);
                ViewBag.workgroups = agent.GetWorkgroups();
            }
            return View();
        }

        public ActionResult Wages()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Wages(DateTime? startDate, DateTime? endDate)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.wages = agent.GetWageReport(startDate, endDate);
            }
            return View();
        }

        public ActionResult Materials()
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.materials = agent.GetMaterials(null);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Materials(string template)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.materials = agent.GetMaterials(template);
            }
            return View();
        }

        public ActionResult DeleteMaterial(int ID)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.DeleteMaterial(ID);
            }
            if (res)
                return Redirect("/Home/Index");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        [HttpPost]
        public ActionResult SetWorkgroup(int orderID,int workgroupID)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.SetWorkgroupOnOrder(orderID, workgroupID);
            }
            if (res)
                return Redirect("/Home/Orders");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        [HttpGet]
        public ActionResult CompleteOrder(int ID)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.CompleteOrder(ID);
            }
            if (res)
                return Redirect("/Home/Orders");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        public ActionResult Money()
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.money = agent.GetFinancialReport(null, null);
                ViewBag.amount = agent.GetAmountOfMoney();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Money(DateTime? startDate, DateTime? endDate)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                if (startDate == null)
                    startDate = agent.FirstMoneyEntry().AddMinutes(-1);
                if (endDate == null)
                    endDate = DateTime.Now.AddMinutes(1);
                ViewBag.money = agent.GetFinancialReport(startDate, endDate);
                ViewBag.amount = agent.GetAmountOfMoney();
            }
            return View();
        }

        [HttpPost]
        public ActionResult NewMoneyEntry(float? change, string note)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.NewMoneyEntry(change, note);
            }
            if(res)
                return Redirect("/Home/Money");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        [HttpPost]
        public ActionResult PurchaseMaterial(int materialID, float price_of_one, float addNumber)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.PurchaseMaterial(materialID, price_of_one, addNumber);
            }
            if (res)
                return Redirect("/Home/Materials");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        [HttpGet]
        public ActionResult ProductMaterials(string template)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.Products = agent.GetProducts(null, null, null, template);
                ViewBag.materials = agent.GetMaterials("");
            }
            return View();
        }

        [HttpGet]
        public ActionResult DeleteProductMaterial(int materialID, int productID)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.DeleteProductMaterial(materialID,productID);
            }
            if (res)
                return Redirect("/Home/ProductMaterials");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        [HttpPost]
        public ActionResult NewProductMaterial(int productID, int materialID, int number)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.NewProductMaterial(productID, materialID, number);
            }
            if (res)
                return Redirect("/Home/productMaterials");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        [HttpGet]
        public ActionResult MaterialReg(string template)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.material_reg = agent.GetMaterialReg(template);
            }
            return View();
        }

        [HttpPost]
        public ActionResult NewEmployee(string name, int? workgroup, string modificator)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                float mod;
                try
                {
                    mod = float.Parse(modificator);
                }
                catch(Exception ex)
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
                res = agent.NewWorker(name, workgroup, mod);
            }
            if (res)
                return Redirect("/Home/Employees");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        [HttpGet]
        public ActionResult EditEmployee(int ID)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.employee = agent.getEmployee(ID);
                ViewBag.workgroups = agent.GetWorkgroups();
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditEmployee(Employee employee)
        {
            bool res;
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                res = agent.EditEmployee(employee);
            }
            if (res)
                return Redirect("/Home/Employees");
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        public ActionResult GetWageReportAdvance()
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.wages = agent.GetWageReportAdvance(null, null);
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetWageReportAdvance(DateTime? startDate, DateTime? endDate)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                ViewBag.wages = agent.GetWageReportAdvance(startDate, endDate);
            }
            return View();
        }

        [HttpPost]
        public ActionResult setAdmin(string username, string passwd)
        {
            getAdmin();
            if (username.Equals(adm.username) && passwd.Equals(adm.passwd))
            {
                HttpContext.Response.Cookies["admin"].Value = true.ToString();
                return Redirect("/Home/Index");
            }
            else
                return View("~/Views/Shared/Error.cshtml");
        }

        public void getAdmin()
        {
            if (HttpContext.Request.Cookies["admin"] != null)
            {
                string ifAuth = HttpContext.Request.Cookies["admin"].Value;
                try
                {
                    ViewBag.adm = Convert.ToBoolean(ifAuth);
                }
                catch (Exception ex)
                {
                    ViewBag.adm = false;
                }
            }
            else
            {
                ViewBag.adm = false;
                HttpContext.Response.Cookies["admin"].Value = false.ToString();
            }
        }

        private void setAuth(DBAgent.DBAgent agent)
        {
            string username, passwd;
            if (HttpContext.Request.Cookies["username"] != null && HttpContext.Request.Cookies["passwd"] != null)
            {
                username = HttpContext.Request.Cookies["username"].Value;
                passwd = HttpContext.Request.Cookies["passwd"].Value;
                ViewBag.username = Authoriz.getAuthoriz(username, passwd, agent);
            }
        }
    }
}