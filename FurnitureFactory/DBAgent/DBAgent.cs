using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;
using FurnitureFactory.Models;

namespace FurnitureFactory.DBAgent
{
    public class DBAgent:IDisposable
    {
        SqlConnection connection;

        public DBAgent()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FurFacDBConnection"].ConnectionString);
            connection.Open();
        }
        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
        public List<Product> GetProducts(int? categoryID, int? minPrice, int? maxPrice, string template)
        {
            if (template == null)
                template = "";

            List<Product> products = new List<Product>();
            string cmdText = "dbo.ProductInfo";//строка запроса (имя процедуры)
            
            SqlCommand command = new SqlCommand(cmdText,connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@template", template));
            SqlDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    Product product = new Product
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1),
                        price = result.GetInt32(2),
                        category = result.GetString(3),
                        categoryID = result.GetInt32(4),
                        salary=result.GetInt32(5)
                    };
                    products.Add(product);
                }
                result.Close();

                if (categoryID != null)
                    products.RemoveAll((x) => x.categoryID != categoryID);
                if (minPrice != null)
                    products.RemoveAll(x => x.price < minPrice);
                if (maxPrice != null)
                    products.RemoveAll(x => x.price > maxPrice);

                return products;
            }
            result.Close();
            return products;
        }
        public List<prodCategories> GetProdCategories()
        {
            List<prodCategories> categories = new List<prodCategories>();
            string cmdText = "select * from prodCategories";

            SqlCommand command = new SqlCommand(cmdText, connection);
            SqlDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    prodCategories category = new prodCategories
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1)
                    };
                    categories.Add(category);
                }
                result.Close();
            }
            return categories;
        }
        public List<Order> GetOrders(int? productID,int? purchaser,DateTime? startDate,DateTime? endDate,bool? state)
        {
            List<Order> orders = new List<Order>();
            string cmdText = "dbo.OrdersInfo";

            SqlCommand command = new SqlCommand(cmdText, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    Order order = new Order
                    {
                        ID = result.GetInt32(0),
                        purchaserID = result.GetInt32(1),
                        purchaser = result.GetString(2),
                        productID = result.GetInt32(3),
                        product = result.GetString(4),
                        price = result.GetInt32(5),
                        state = result.GetBoolean(6),
                        date = result.GetDateTime(7),
                        number = result.GetInt32(8)
                    };
                    orders.Add(order);
                }
                result.Close();

                if (productID != null)
                    orders.RemoveAll(x => x.productID != productID);
                if (purchaser != null)
                    orders.RemoveAll(x => x.purchaserID != purchaser);
                if (startDate != null)
                    orders.RemoveAll(x => x.date.CompareTo(startDate) <= 0);
                if (endDate != null)
                    orders.RemoveAll(x => x.date.CompareTo(endDate) > 0);
                if (state != null)
                    orders.RemoveAll(x => x.state != state);

                
            }
            result.Close();
            return orders;
        }
        public bool NewOrder(int? productID, int? purchaserID, int? number)
        {
            if (productID == null || purchaserID == null || number==null)
                return false;

            try
            {
                string cmdText = "dbo.NewUnitOrder";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@productID", productID));
                command.Parameters.Add(new SqlParameter("@purchaserID", purchaserID));
                command.Parameters.Add(new SqlParameter("@number", number));
                int result = command.ExecuteNonQuery();
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch(SqlException ex)
            {
                return false;
            }
        }
        public List<Material> GetMaterials(string template)
        {
            if (template == null)
                template = "";

            List<Material> materials = new List<Material>();
            string cmd = "select * from materials where name like @template+'%' order by name asc";
            SqlCommand command = new SqlCommand(cmd, connection);
            command.Parameters.Add(new SqlParameter("@template", template));
            SqlDataReader result = command.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    Material material = new Material
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1),
                        reserve = result.GetFloat(2)
                    };
                    materials.Add(material);
                }
                result.Close();
                return materials;
            }
            else
            {
                result.Close();
                return null;
            }
        }
        public List<Material> GetMaterials(int productID)
        {
            List<Material> materials = new List<Material>();
            string cmd = "dbo.ProductMaterials";
            SqlCommand command = new SqlCommand(cmd, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@productID", productID));
            SqlDataReader result = command.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    Material material = new Material
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1),
                        reserve = result.GetFloat(2)
                    };
                    materials.Add(material);
                }
                result.Close();
                return materials;
            }
            else
            {
                result.Close();
                return null;
            }
        }
        public bool DeleteMaterial(int ID)
        {
            try
            {
                string cmdText = "delete from materials where ID=@id";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@id", ID));
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public bool CheckAuthoriz(string username,string passwd)
        {
            List<Purchaser> materials = new List<Purchaser>();
            string cmd = "select username, passwd from purchasers";
            SqlCommand command = new SqlCommand(cmd, connection);
            SqlDataReader result = command.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    Purchaser purchaser = new Purchaser
                    {
                        username = result.GetString(0),
                        passwd = result.GetString(1)
                    };
                    materials.Add(purchaser);
                }
                result.Close();

                if (materials.FindAll(x => x.username == username && x.passwd == passwd).Count > 0)
                {
                    return true;
                }
                else return false;
            }
            else
            {
                result.Close();
                return false;
            }
        }
        public bool NewPurchaser(Purchaser purchaser)
        {
            try
            {
                string cmdText = "dbo.NewPurchaser";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@name", purchaser.name));
                command.Parameters.Add(new SqlParameter("@email", purchaser.email));
                command.Parameters.Add(new SqlParameter("@tel_number", purchaser.tel_number));
                command.Parameters.Add(new SqlParameter("@username", purchaser.username));
                command.Parameters.Add(new SqlParameter("@passwd", purchaser.passwd));
                object result = command.ExecuteScalar();
                return true;
            }
            catch(SqlException ex)
            {
                return false;
            }
            
        }
        public bool EditPurchaser(int ID, string name, string email, string tel_number)
        {
            try
            {
                string cmdText = "update purchasers set name=@name, email=@email, tel_number=@tel_number where ID=@id";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@name", name));
                command.Parameters.Add(new SqlParameter("@email", email));
                command.Parameters.Add(new SqlParameter("@tel_number", tel_number));
                int count = command.ExecuteNonQuery();
                if (count == 0)
                    return false;
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public List<Employee> GetEmployees(string name,int? workgroupID, float? minMod, float? maxMod)
        {
            List<Employee> employees = new List<Employee>();
            string cmdText = "dbo.EmployeesInfo";

            SqlCommand command = new SqlCommand(cmdText, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    Employee employee = new Employee
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1),
                        workgroupID = result.GetInt32(2),
                        workgroup = result.GetString(3),
                        modificator = result.GetFloat(4)
                    };
                    employees.Add(employee);
                }
                result.Close();

                if (name != null)
                    employees.RemoveAll((x) => !x.name.Equals(name));
                if (minMod != null)
                    employees.RemoveAll(x => x.modificator < minMod);
                if (maxMod != null)
                    employees.RemoveAll(x => x.modificator > maxMod);
                if (workgroupID != null)
                    employees.RemoveAll(x => x.workgroupID != workgroupID);
            }
            result.Close();

            return employees;
        }
        public Purchaser getPurchaser(string username, string passwd)
        {
            string cmd = "dbo.PurhaserInfo";
            SqlCommand command = new SqlCommand(cmd, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@username",username));
            command.Parameters.Add(new SqlParameter("@password", passwd));
            SqlDataReader result = command.ExecuteReader();
            Purchaser purchaser = null;

            if (result.HasRows)
            {
                while (result.Read())
                {
                    purchaser = new Purchaser
                    {
                        username = result.GetString(0),
                        passwd = result.GetString(1)
                    };
                }
                return purchaser;
            }
            else
            {
                result.Close();
                return null;
            }
        }
        public List<Purchaser> getPurchasers()
        {
            List<Purchaser> purchasers = new List<Purchaser>();
            string cmd = "dbo.PurchaserInfo";
            SqlCommand command = new SqlCommand(cmd, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader result = command.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {

                    Purchaser purchaser = new Purchaser
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1),
                        email = result.GetString(2),
                        //tel_number = result.GetString(3),
                        OrderCount = result.GetInt32(5)
                    };
                    purchasers.Add(purchaser);
                }
            }

            result.Close();
            return purchasers;
            
        }
        public bool NewProduct(string name, int? price,int? categoryID, int? salary)
        {
            if(name==null || price==null || categoryID==null || salary == null)
            {
                return false;
            }

            try
            {
                string cmdText = "dbo.NewProduct";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@name", name));
                command.Parameters.Add(new SqlParameter("@price", price));
                command.Parameters.Add(new SqlParameter("@categoryID", categoryID));
                command.Parameters.Add(new SqlParameter("@salary", salary));
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public bool DeleteProduct(int ID)
        {
            try
            {
                string cmdText = "delete from products where ID=@id";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@id", ID));
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public bool EditProduct(int? ID, string name, int? price, int? categoryID, int? salary)
        {
            if(ID==null || name == null || price == null || categoryID == null || salary == null)
            {
                return false;
            }

            try
            {
                string cmdText = "update products set name=@name, price=@price,categoryID=@categoryID,salary_on_workgroup=@salary where ID=@id";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@id", ID));
                command.Parameters.Add(new SqlParameter("@name", name));
                command.Parameters.Add(new SqlParameter("@categoryID", categoryID));
                command.Parameters.Add(new SqlParameter("@salary", salary));
                command.Parameters.Add(new SqlParameter("@price", price));
                int c = command.ExecuteNonQuery();
                if (c == 0)
                    return false;
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public bool PurchaseMaterial(int? materialID,float? price_of_one,float? addNumber)
        {
            try
            {
                string cmdText = "dbo.PurchaseMaterials";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@materialID", materialID));
                command.Parameters.Add(new SqlParameter("@price_of_one", price_of_one));
                command.Parameters.Add(new SqlParameter("@addNumber", addNumber));
                int res = command.ExecuteNonQuery();
                if (res > 0)
                    return true;
                else return false;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public Employee getEmployee(int id)
        {
            string cmd = "select * from employees where ID=@id";
            SqlCommand command = new SqlCommand(cmd, connection);
            command.Parameters.Add(new SqlParameter("@id", id));
            SqlDataReader result = command.ExecuteReader();
            Employee employee = null;

            if (result.HasRows)
            {
                while (result.Read())
                {
                    employee = new Employee
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1),
                        workgroupID = result.GetInt32(2),
                        modificator = result.GetFloat(3)
                    };
                }
                result.Close();
                return employee;
            }
            else
            {
                result.Close();
                return null;
            }
        }
        public bool EditEmployee(Employee employee)
        {
            try
            {
                string cmdText = "update employees set name=@name,рабочая_группа=@workgroupID,modificator=@modificator where ID=@id";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@id", employee.ID));
                command.Parameters.Add(new SqlParameter("@name", employee.name));
                command.Parameters.Add(new SqlParameter("@workgroupID", employee.workgroupID));
                command.Parameters.Add(new SqlParameter("@modificator", employee.modificator));
                int c = command.ExecuteNonQuery();
                if (c == 0)
                    return false;
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public Workgroup getWorkgroup(int id)
        {
            string cmd = "select * from рабочие_группы where ID=@id";
            SqlCommand command = new SqlCommand(cmd, connection);
            command.Parameters.Add(new SqlParameter("@id", id));
            SqlDataReader result = command.ExecuteReader();
            Workgroup workgroup = null;

            if (result.HasRows)
            {
                while (result.Read())
                {
                    workgroup = new Workgroup
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1)
                    };
                }
                return workgroup;
            }
            else return null;
        }
        public List<Workgroup> GetWorkgroups()
        {
            List<Workgroup> workgroups = new List<Workgroup>();
            string cmd = "select * from рабочие_группы";
            SqlCommand command = new SqlCommand(cmd, connection);
            SqlDataReader result = command.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    Workgroup group = new Workgroup
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1)
                    };
                    workgroups.Add(group);
                }
            }

            result.Close();
            return workgroups;
        }
        public List<Wage> GetWageReport(DateTime? startDate, DateTime? endDate)
        {
            List<Wage> wages = new List<Wage>();
            string cmdText = "dbo.WageReport";

            SqlCommand command = new SqlCommand(cmdText, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (startDate == null)
                startDate = FirstMoneyEntry();
            if (endDate == null)
                endDate = DateTime.Now.AddDays(1);
            else
                endDate.Value.AddDays(1);

            command.Parameters.Add(new SqlParameter("@startDate", startDate));
            command.Parameters.Add(new SqlParameter("@endDate", endDate));
            SqlDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    float w;
                    if (result.IsDBNull(2))
                        w = 0;
                    else
                        w = (float)result.GetDouble(2);
                    Wage wage = new Wage(result.GetInt32(0), result.GetInt32(1), w);
                    wages.Add(wage);
                }
                result.Close();
            }
            result.Close();

            return wages;
        }
        public List<Wage> GetWageReportAdvance(DateTime? startDate, DateTime? endDate)
        {
            List<Wage> wages = new List<Wage>();
            string cmdText = "select employeeID, [date], wage from wages order by [date] DESC";

            SqlCommand command = new SqlCommand(cmdText, connection);

            SqlDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    float w = result.GetFloat(2);
                    Wage wage = new Wage(result.GetInt32(0), null, w);
                    wage.date = result.GetDateTime(1);
                    wages.Add(wage);
                }
            }
            result.Close();

            if (startDate != null)
                wages.RemoveAll(x => x.date < startDate);
            if (endDate != null)
                wages.RemoveAll(x => x.date > endDate);

            return wages;
        }

        public bool SetWorkgroupOnOrder(int orderID, int workgroupID)
        {
            try
            {
                string cmdText = "insert into OrderTreatment (orderID,рабочая_группа)" +
                " values (@orderID, @workgroupID)";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@orderID", orderID));
                command.Parameters.Add(new SqlParameter("@workgroupID", workgroupID));
                int result = command.ExecuteNonQuery();
                if (result == 0)
                    return false;
                else
                    return true;
            }
            catch(SqlException ex)
            {
                return false;
            }

        }
        public bool CheckWorkgroupOnOrder(int orderID)
        {
            try
            {
                string cmdText = "SELECT        orders.ID "+
                                  "FROM orders INNER JOIN "+
                         "OrderTreatment ON orders.ID = OrderTreatment.orderID "+
                                   "WHERE(orders.ID = @orderID)";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@orderID", orderID));
                SqlDataReader result = command.ExecuteReader();
                if (result.HasRows)
                    return true;
                else
                    return false;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public bool CompleteOrder(int ID)
        {
            try
            {
                string cmdText = "update orders set state=1 where ID=@id";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@id", ID));
                int result = command.ExecuteNonQuery();
                if (result>0)
                    return true;
                else
                    return false;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public List<Money> GetFinancialReport(DateTime? startDate, DateTime? endDate)
        {
            List<Money> money = new List<Money>();
            string cmdText = "select * from [money] order by [date] DESC";

            SqlCommand command = new SqlCommand(cmdText, connection);
            SqlDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    Money entry = new Money();
                    entry.ID = result.GetInt32(0);
                    entry.change = result.GetFloat(1);
                    entry.date = result.GetDateTime(2);
                    if (result.IsDBNull(3))
                        entry.note = null;
                    else
                        entry.note = result.GetString(3);

                    money.Add(entry);
                }
                result.Close();

                if (startDate != null)
                    money.RemoveAll(x => x.date.CompareTo(startDate) <= 0);
                if (endDate != null)
                    money.RemoveAll(x => x.date.CompareTo(endDate) > 0);
            }
            result.Close();
            return money;
        }
        public float? GetAmountOfMoney()
        {
            float? amount = null;
            try
            {
                string cmdText = "select dbo.CountBill()";
                SqlCommand command = new SqlCommand(cmdText, connection);
                object result = command.ExecuteScalar();
                if (result.ToString() != "")
                    amount = float.Parse(result.ToString());
                else amount = 0;
                return amount;
            }
            catch (SqlException ex)
            {
                return amount;
            }
        }
        public bool NewMoneyEntry(float? change, string note)
        {
            try
            {
                string cmdText = "insert into [money] (change,note) values (@change, @note)";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@change", change));
                command.Parameters.Add(new SqlParameter("@note", note));
                int result = command.ExecuteNonQuery();
                if (result > 0)
                    return true;
                else return false;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public bool DeleteProductMaterial(int materialID,int productID)
        {
            try
            {
                string cmdText = "delete from product_material where material_ID=@materialid and product_ID=@productid";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@materialid", materialID));
                command.Parameters.Add(new SqlParameter("@productid", productID));
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public bool NewProductMaterial(int productID, int materialID, float number)
        {
            try
            {
                string cmdText = "insert into product_material (product_ID,material_ID,number)" +
                    " values (@productID,@materialID,@number)";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@productID", productID));
                command.Parameters.Add(new SqlParameter("@materialID", materialID));
                command.Parameters.Add(new SqlParameter("@number", number));
                int res = command.ExecuteNonQuery();
                if (res > 0)
                    return true;
                else return false;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public List<MaterialReg> GetMaterialReg(string template)
        {
            if (template == null)
                template = "";

            List<MaterialReg> regs = new List<MaterialReg>();
            string cmdText = "dbo.Material_regInfo";

            SqlCommand command = new SqlCommand(cmdText, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@template", template));
            SqlDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    float? money;
                    if (result.IsDBNull(4))
                        money = null;
                    else
                        money = result.GetFloat(4);
                    MaterialReg reg = new MaterialReg
                    {
                        ID = result.GetInt32(0),
                        name = result.GetString(1),
                        number = result.GetFloat(2),
                        date = result.GetDateTime(3),
                        money = money
                    };
                    regs.Add(reg);
                }
            }
            result.Close();

            return regs;
        }
        public bool NewWorker(string name, int? workgroupID, float modificator)
        {
            try
            {
                string cmdText = "insert into employees (name, рабочая_группа, modificator)" +
                    " values (@name, @workgroupID, @modificator)";
                SqlCommand command = new SqlCommand(cmdText, connection);
                command.Parameters.Add(new SqlParameter("@name", name));
                command.Parameters.Add(new SqlParameter("@workgroupID", workgroupID));
                command.Parameters.Add(new SqlParameter("@modificator", modificator));
                int result = command.ExecuteNonQuery();
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public float GetMatCustPrice(int materialID)
        {
            string cmdText = "select dbo.MatCustPrice(@materialID)";

            SqlCommand command = new SqlCommand(cmdText, connection);
            command.Parameters.Add(new SqlParameter("@materialID", materialID));
            object result = command.ExecuteScalar();
            string num = result.ToString();
            if (num != "")
                return float.Parse(num);
            else return 0;
        }
        public float GetProdCust(int productID)
        {
            string cmdText = "select dbo.ProdCustPrice(@productID)";

            SqlCommand command = new SqlCommand(cmdText, connection);
            command.Parameters.Add(new SqlParameter("@productID", productID));
            object result = command.ExecuteScalar();
            string num = result.ToString();
            if (num != "")
                return float.Parse(num);
            else return 0;
        }


        public DateTime FirstMoneyEntry()
        {
            string cmdText = "select top 1 [date] from [money]";

            SqlCommand command = new SqlCommand(cmdText, connection);
            object result = command.ExecuteScalar();
            return (DateTime)result;
        }
    }
}