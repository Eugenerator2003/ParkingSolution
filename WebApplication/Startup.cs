using Arch.EntityFrameworkCore.Metadata.Internal;
using Parking.Application;
using Parking.Domain;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Reflection;
using WebApplication.Services;

namespace WebApplication
{
    public class Startup
    {  
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ParkingContext>();
            services.AddTransient<CachedService<CarMark>>();
            services.AddTransient<CachedService<Owner>>();
            services.AddTransient<CachedService<Car>>();
            services.AddTransient<CachedService<Emploeyee>>();
            services.AddTransient<CachedService<WorkShift>>();
            services.AddTransient<CachedService<ParkingType>>();
            services.AddTransient<CachedService<PaymentTariff>>();
            services.AddTransient<CachedService<ParkingRecord>>();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();
            app.Map("/info", Info);
            app.Map("/CarMarks", CarMarksTable);
            app.Map("/Owners", OwnersTable);
            app.Map("/Cars", CarsTable);
            app.Map("/ParkingTypes", ParkingTypesTable);
            app.Map("/PaymentsTariffs", PaymentTariffsTable);
            app.Map("/ParkingRecords", ParkingRecordsTable);
            app.Map("/Employees", EmployeesTable);
            app.Map("/WorkShifts", WorkShiftsTable);
            app.Map("/searchform1", SearchForm1);
            app.Map("/searchform2", SearchForm2);
            app.Run(async context =>
            {
                await context.Response.WriteAsync("<html><title>Parking web application</title>" +
                                                  "<a href='/info'>Info</a><br>" +
                                                  "<a href='/CarMarks'>Car marks</a><br>" +
                                                  "<a href='/Owners'>Owners</a><br>" +
                                                  "<a href='/Cars'>Cars</a><br>" +
                                                  "<a href='/ParkingTypes'>Parking types</a><br>" +
                                                  "<a href='/PaymentsTariffs'>Payment tariffs<br>" +
                                                  "<a href='/ParkingRecords'>Parking records</a><br>" +
                                                  "<a href='/Employees'>Employees</a><br>" +
                                                  "<a href='/WorkShifts'>Work shifts</a><br>" +
                                                  "<a href='/searchform1'>First search form</a><br>" +
                                                  "<a href='/searchform2'>Second search form</a></html>");
            });
        }

        private void Info(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync($"<div>Host: {context.Request.Host}</div>" +
                                                  $"<div>Path: {context.Request.PathBase}</div>" +
                                                  $"<div>Protocol: {context.Request.Protocol}</div>" +
                                                  $"<div><a href=\"\\\">Back</a></div>");
            });
        }

        private IEnumerable<T> GetEntities<T>(HttpContext context) where T : class
        {
            var cachedService = context.RequestServices.GetService<CachedService<T>>();
            var entities = cachedService.GetItems(typeof(T).Name + "s20");
            return entities;
        }

        private string MakeHtml<T>(IEnumerable<T> entities) where T : class 
        {
            var isSuitable = (PropertyInfo prop) =>
            {
                var type = prop.PropertyType;
                return type.IsPrimitive ||
                       type == typeof(string) ||
                       type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            };

            var type = typeof(T);
            var builder = new StringBuilder($"<html><title>{type.Name}s</title><body><table border=2>" +
                                            $"<caption>{type.Name}s</caption><tr>");
            var props = type.GetProperties();
            props = props.Where(p => isSuitable(p)).ToArray();
            foreach(var prop in props)
            {
                builder.Append($"<th>{prop.Name}</th>");
            }
            builder.Append("</tr>");

            foreach(var item in entities)
            {
                builder.Append("<tr>");
                foreach(var prop in props)
                {
                    builder.Append($"<td>{prop.GetValue(item)}</td>");
                }
                builder.Append("</tr>");
            }

            return builder.Append("<div><a href=\"\\\">Back</a></div></table></body></html>").ToString();
        }
        
        private void CarMarksTable(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var cars = GetEntities<CarMark>(context);
                await context.Response.WriteAsync(MakeHtml(cars));
            });
        }

        private void OwnersTable(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var owners = GetEntities<Owner>(context);
                await context.Response.WriteAsync(MakeHtml(owners));
            });
        }

        private void CarsTable(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var Cars = GetEntities<Car>(context);
                await context.Response.WriteAsync(MakeHtml(Cars));
            });
        }
        private void PaymentTariffsTable(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var tariffs = GetEntities<PaymentTariff>(context);
                await context.Response.WriteAsync(MakeHtml(tariffs));
            });
        }

        private void ParkingRecordsTable(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var records = GetEntities<ParkingRecord>(context);
                await context.Response.WriteAsync(MakeHtml(records));
            });
        }

        private void WorkShiftsTable(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var workShifts = GetEntities<WorkShift>(context);
                await context.Response.WriteAsync(MakeHtml(workShifts));
            });
        }

        private void EmployeesTable(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var employees = GetEntities<Emploeyee>(context);
                await context.Response.WriteAsync(MakeHtml(employees));
            });
        }

        private void ParkingTypesTable(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var types = GetEntities<ParkingType>(context);
                await context.Response.WriteAsync(MakeHtml(types));
            });
        }
        private void SearchForm1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var marks = GetEntities<CarMark>(context);
                string keyInput = "CarMark";
                string keySelect = "CarMarkSelect";
                string keyRadio = "CarMarkRadio";

                var builder = new StringBuilder(
                              $"<html><head><title>Search form 1</title></head><body>" +
                              $"<form><div><input type='submit' value='Save data to session'></div><br>" +
                              $"<div>Enter mark name:<br><input type='text' name={keyInput} " +
                              $"value='{context.Session.GetString(keyInput)}'></div>" +
                              $"<br><div>Select mark name:<br><select name='{keySelect}'>");

                foreach(var mark in marks)
                {
                    builder.Append($"<option");
                    if (mark.Name == context.Session.GetString(keySelect))
                    {
                        builder.Append(" selected");
                    }
                    builder.Append('>');
                    builder.Append(mark.Name);
                    builder.Append("</option>");
                }

                builder.Append("</select></div><br><div>Select mark name:<br>");

                foreach(var mark in marks)
                {
                    builder.Append($"<input type='radio' name='{keyRadio}'");
                    if (mark.Name == context.Session.GetString(keyRadio))
                    {
                        builder.Append("checked ");
                    }
                    builder.Append($"value='{mark.Name}' name='{keyRadio}'>{mark.Name}");
                    builder.Append("<br>");
                }

                string markField = context.Request.Query[keyInput];
                string markSelect = context.Request.Query[keySelect];
                string markRadio = context.Request.Query[keyRadio];

                if (markField is not null)
                {
                    context.Session.SetString(keyInput, markField);
                }

                if (markSelect is not null)
                {
                    context.Session.SetString(keySelect, markSelect);
                }

                if (markRadio is not null)
                {
                    context.Session.SetString(keyRadio, markRadio);
                }

                builder.Append("</div></form><div><a href=\"\\\">Back</a></div></body></html>");

                await context.Response.WriteAsync(builder.ToString());
            });
        }

        private void SearchForm2(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var marks = GetEntities<CarMark>(context);
                string keyInput = "CarMark";
                string keySelect = "CarMarkSelect";
                string keyRadio = "CarMarkRadio";

                var builder = new StringBuilder(
                              $"<html><head><title>Search form 2</title></head><body>" +
                              $"<form><div><input type='submit' value='Save data to cookies'></div><br>" +
                              $"<div>Enter mark name:<br><input type='text' name={keyInput} " +
                              $"value='{context.Request.Cookies[keyInput]}'></div>" +
                              $"<br><div>Select mark name:<br><select name='{keySelect}'>");

                foreach (var mark in marks)
                {
                    builder.Append($"<option");
                    if (mark.Name == context.Request.Cookies[keySelect])
                    {
                        builder.Append(" selected");
                    }
                    builder.Append('>');
                    builder.Append(mark.Name);
                    builder.Append("</option>");
                }

                builder.Append("</select></div><br><div>Select mark name:<br>");

                foreach (var mark in marks)
                {
                    builder.Append($"<input type='radio' name='{keyRadio}'");
                    if (mark.Name == context.Request.Cookies[keyRadio])
                    {
                        builder.Append("checked ");
                    }
                    builder.Append($"value='{mark.Name}' name='{keyRadio}'>{mark.Name}");
                    builder.Append("<br>");
                }

                string markField = context.Request.Query[keyInput];
                string markSelect = context.Request.Query[keySelect];
                string markRadio = context.Request.Query[keyRadio];

                if (markField is not null)
                {
                    context.Response.Cookies.Append(keyInput, markField);
                }

                if (markSelect is not null)
                {
                    context.Response.Cookies.Append(keySelect, markSelect);
                }

                if (markRadio is not null)
                {
                    context.Response.Cookies.Append(keyRadio, markRadio);
                }

                builder.Append("</div></form><div><a href=\"\\\">Back</a></div></body></html>");

                await context.Response.WriteAsync(builder.ToString());
            });
        }

    }
}
