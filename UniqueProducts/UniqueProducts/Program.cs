using UniqueProducts.Models;
using UniqueProducts.Services;
using UniqueProducts.Data;
using UniqueProducts.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FuelStationT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var services = builder.Services;
            // внедрение зависимости для доступа к БД с использованием EF
            string connection = builder.Configuration.GetConnectionString("SqlServerConnection");
            services.AddDbContext<UniqueProductsContext>(options => options.UseSqlServer(connection));

            // добавление кэширования
            services.AddMemoryCache();

            // добавление поддержки сессии
            services.AddDistributedMemoryCache();
            services.AddSession();

            // внедрение зависимости CachedMaterialsService
            services.AddScoped<ICachedProductsService,CachedProductsService>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var app = builder.Build();

            // добавляем поддержку сессий
            app.UseSession();

            //Запоминание в Сookies значений, введенных в форме
            app.Map("/searchform1", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var product = new Product();
                    ICachedProductsService cachedProducts = context.RequestServices.GetService<ICachedProductsService>();
                    IEnumerable<Product> products = cachedProducts.GetProductsFromCache("products20");
                    IEnumerable<string> colors = cachedProducts.GetColors(products);

                    if (context.Request.Method == "POST")
                    {
                        product.ProductPrice = decimal.Parse(context.Request.Form["priceLimit"]);
                        product.ProductColor=context.Request.Form["color"];

                        context.Response.Cookies.Append("product", JsonConvert.SerializeObject(product));

                        if (product.ProductColor != "all")
                        {
                            products = products.Where(p => p.ProductPrice <= product.ProductPrice && p.ProductColor == product.ProductColor);
                        }
                        else
                        {
                            products = products.Where(p => p.ProductPrice <= product.ProductPrice);
                        }
                    }
                    else if (context.Request.Cookies.ContainsKey("product"))
                    {
                        product = JsonConvert.DeserializeObject<Product>(context.Request.Cookies["product"]);
                    }

                    string htmlString = "<html><head><title>Изделия</title></head>" +
                        "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                        "<body>" +
                        "<form method='post' action='/searchform1'>" +
                            "<label>Ваш бюджет:</label>" +
                            $"<input type='text' name='priceLimit' value='{product.ProductPrice}' placeholder='Максимальная цена'><br><br>" +
                            "<label>Выберите цвет:</label>" +
                            "<select name='color'>" +
                            "<option value='all'>Все</option>";

                    foreach (var color in colors)
                    {
                        htmlString += $"<option value='{color}' {(color == product.ProductColor ? "selected" : "")}>{color}</option>";
                    }

                    htmlString += "</select><br><br>" +
                        "<input type='submit' value='Поиск'>" +
                        "</form>";

                    htmlString += "<h1>Список изделий</h1>" +
                        "<table border='1'>" +
                        "<tr>" +
                            "<th>Код</th>" +
                            "<th>Название изделия</th>" +
                            "<th>Описание изделия</th>" +
                            "<th>Вес изделия</th>" +
                            "<th>Диаметр изделия</th>" +
                            "<th>Цвет изделия</th>" +
                            "<th>Цена изделия</th>" +
                        "</tr>";

                    foreach (var pr in products)
                    {
                        htmlString += "<tr>" +
                            $"<td>{pr.ProductId}</td>" +
                            $"<td>{pr.ProductName}</td>" +
                            $"<td>{pr.ProductDescript}</td>" +
                            $"<td>{pr.ProductWeight}</td>" +
                            $"<td>{pr.ProductDiameter}</td>" +
                            $"<td>{pr.ProductColor}</td>" +
                            $"<td>{pr.ProductPrice}</td>" +
                        "</tr>";
                    }

                    htmlString += "</table><br><a href='/'>Главная</a></br></body></html>";
                    await context.Response.WriteAsync(htmlString);
                });
            });

            //Запоминание в Session значений, введенных в форме
            app.Map("/searchform2", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    Product product = context.Session.Get<Product>("product") ?? new Product();
                    ICachedProductsService cachedProducts = context.RequestServices.GetService<ICachedProductsService>();
                    IEnumerable<Product> products = cachedProducts.GetProductsFromCache("products20");

                    if (context.Request.Method == "POST")
                    {
                        product.ProductName = context.Request.Form["ProductName"];
                        context.Session.Set("product", product);
                        products = products.Where(p => p.ProductName == product.ProductName);
                    }

                    string htmlString = "<html><head><title>Изделия</title></head>" +
                        "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                        "<body>" +
                        "<form method='post' action='/searchform2'>" +
                            "<label>Название изделия:</label>" +
                            $"<input type='text' name='ProductName' value='{product.ProductName}'><br><br>" +
                            "<input type='submit' value='Поиск'>" +
                        "</form>" +
                        "<h1>Список изделий</h1>" +
                        "<table border='1'>" +
                        "<tr>" +
                            "<th>Код</th>" +
                            "<th>Название изделия</th>" +
                            "<th>Описание изделия</th>" +
                            "<th>Вес изделия</th>" +
                            "<th>Диаметр изделия</th>" +
                            "<th>Цвет изделия</th>" +
                            "<th>Цена изделия</th>" +
                        "</tr>";

                    foreach (var pr in products)
                    {
                        htmlString += "<tr>" +
                            $"<td>{pr.ProductId}</td>" +
                            $"<td>{pr.ProductName}</td>" +
                            $"<td>{pr.ProductDescript}</td>" +
                            $"<td>{pr.ProductWeight}</td>" +
                            $"<td>{pr.ProductDiameter}</td>" +
                            $"<td>{pr.ProductColor}</td>" +
                            $"<td>{pr.ProductPrice}</td>" +
                        "</tr>";
                    }
                    htmlString += "</table><br><a href='/'>Главная</a></br></body></html>";
                    await context.Response.WriteAsync(htmlString);
                });
            });

            // Вывод информации о клиенте
            app.Map("/info", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Формирование строки для вывода 
                    string htmlString = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Информация:</H1>"
                    + "<BR> Сервер: " + context.Request.Host
                    + "<BR> Путь: " + context.Request.PathBase
                    + "<BR> Протокол: " + context.Request.Protocol
                    + "<BR><A href='/'>Главная</A></BODY></HTML>";
                    // Вывод данных
                    await context.Response.WriteAsync(htmlString);
                });
            });

            // Вывод кэшированной информации из таблицы базы данных
            app.Map("/products", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedProductsService cachedProductsService = context.RequestServices.GetService<ICachedProductsService>();
                    IEnumerable<Product> products = cachedProductsService.GetProductsFromCache("products20");
                    string htmlString = "<HTML><HEAD><TITLE>Изделия</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список изделий</H1>" +
                    "<TABLE BORDER=1>";
                    htmlString += "<TR>";
                    htmlString += "<TH>Код</TH>";
                    htmlString += "<TH>Название изделия</TH>";
                    htmlString += "<TH>Описание изделия</TH>";
                    htmlString += "<TH>Вес изделия</TH>";
                    htmlString += "<TH>Диаметр изделия</TH>";
                    htmlString += "<TH>Цвет изделия</TH>";
                    htmlString += "<TH>Цена изделия</TH>";
                    htmlString += "</TR>";
                    foreach (var pr in products)
                    {
                        htmlString += "<tr>" +
                            $"<td>{pr.ProductId}</td>" +
                            $"<td>{pr.ProductName}</td>" +
                            $"<td>{pr.ProductDescript}</td>" +
                            $"<td>{pr.ProductWeight}</td>" +
                            $"<td>{pr.ProductDiameter}</td>" +
                            $"<td>{pr.ProductColor}</td>" +
                            $"<td>{pr.ProductPrice}</td>" +
                        "</tr>";
                    }
                    htmlString += "</TABLE>";
                    htmlString += "<BR><A href='/'>Главная</A></BR>";
                    htmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(htmlString);
                });
            });

            // Стартовая страница и кэширование данных таблицы на web-сервере
            app.Run((context) =>
            {
                //обращение к сервису
                ICachedProductsService cachedMaterials = context.RequestServices.GetService<ICachedProductsService>();
                cachedMaterials.AddProducts("products20");

                string htmlString = "<HTML><HEAD><TITLE>Материалы</TITLE></HEAD>" +
                "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                "<BODY><H1>Главная</H1>"
                +"<BR><A href='/'>Главная</A></BR>"
                +"<BR><A href='/info'>Информация о клиенте</A></BR>"
                +"<BR><A href='/products'>Изделия</A></BR>"
                +"<BR><A href='/searchform2'>Поиск изделий по названию</A></BR>"
                +"<BR><A href='/searchform1'>Поиск изделий по характеристикам</A></BR>"
                +"</BODY></HTML>";

                return context.Response.WriteAsync(htmlString);

            });

            app.Run();
        }
    }
}