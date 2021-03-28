using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HackAnalysis.Data;
using Microsoft.AspNetCore.Mvc;

namespace HackAnalysis.Helpers
{
    public class UsersCsvResult : FileResult
    {
        private readonly IEnumerable<User> _employeeData;

        public UsersCsvResult(IEnumerable<User> employeeData, string fileDownloadName) : base("text/csv")
        {
            _employeeData = employeeData;
            FileDownloadName = fileDownloadName;
        }

        public async override Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            context.HttpContext.Response.Headers.Add("Content-Disposition",
                new[] {"attachment; filename=" + FileDownloadName});
            using (var streamWriter = new StreamWriter(response.Body))
            {
                await streamWriter.WriteLineAsync(
                    $"Name, Salary, Age"
                );
                foreach (var p in _employeeData)
                {
                    await streamWriter.WriteLineAsync(
                        $"{p.name}, {p.sex}, {p.date_birth}"
                    );
                    await streamWriter.FlushAsync();
                }

                await streamWriter.FlushAsync();
            }
        }
    }
}

