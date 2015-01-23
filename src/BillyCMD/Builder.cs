using BillyCMD.Data;
using Newtonsoft.Json;
using RazorEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BillyCMD
{
    static class Builder
    {
        public static void BuildDocs(Options.BuildSubOptions options)
        {
            var log = options.Verbose
                ? m => { Console.WriteLine("[INFO] " + m); }
                : (Action<string>)(m => { });

            Action<string> error = m => Console.WriteLine("[ERROR] " + m);


            Action<bool, string> assert = (exp, m) =>
            {
                if (exp)
                    return;

                error("Assertion failed: " + m);
                Env.Exit();
            };

            if (Directory.Exists(options.OutputPath))
                Directory.Delete(options.OutputPath, true);

            Directory.CreateDirectory(options.OutputPath);


            log("Looking for templates at " + options.InputPath);

            if (!Directory.Exists(options.InputPath))
            {
                error("Could not find input directory " + options.InputPath);
                Env.Exit();
            }

            var dirs = Directory.GetDirectories(options.InputPath);

            foreach (var dir in dirs)
            {
                log(dir);

                var contractPath = Path.Combine(dir, "contract.json");

                if (!File.Exists(contractPath))
                {
                    error("Could not find contract description file " + contractPath);
                    Env.Exit();
                }

                var contract = JsonConvert.DeserializeObject<Contract>(File.ReadAllText(contractPath));

                assert(string.IsNullOrWhiteSpace(contract.Number) == false, "contract number is undefined");
                assert(contract.Date != DateTime.MinValue, "contract date is undefined");

                log("Loaded contract #" + contract.Number + " from " + contract.Date.ToShortDateString());

                var invoices = Directory.GetFiles(dir, "invoice*.json");

                var invoiceTmpl = LoadTemplate(dir, "Invoice");
                var actTmpl = LoadTemplate(dir, "Acceptance");
                var notificationTmpl = LoadTemplate(dir, "BankNotification");

                foreach (var invoicePath in invoices)
                {
                    var invoice = JsonConvert.DeserializeObject<Invoice>(File.ReadAllText(invoicePath));

                    assert(string.IsNullOrWhiteSpace(invoice.Number) == false, "contract number is undefined");
                    assert(invoice.Date != DateTime.MinValue, "contract date is undefined");
                    assert(invoice.Period != null, "period is undefined");
                    assert(invoice.Period.From != DateTime.MinValue, "period FROM date is undefined");
                    assert(invoice.Period.From != DateTime.MinValue, "period TO date is undefined");

                    log("Loaded invoice " + invoice.Number + " from " + invoice.Date.ToShortDateString() + " for period from " + invoice.Period.From.ToShortDateString() + " to " + invoice.Period.To.ToShortDateString());

                    BuildDoc("Invoice", options, invoiceTmpl, invoice, contract, log, error);
                    BuildDoc("Act", options, actTmpl, invoice, contract, log, error);
                    BuildDoc("Letter to the Bank", options, notificationTmpl, invoice, contract, log, error);
                }
            }
        }

        static string LoadTemplate(string dir, string file)
        {
            var path = Path.Combine(dir, file + ".cshtml");
            return File.Exists(path) ? File.ReadAllText(path) : String.Empty;
        }

        static void BuildDoc(string name, Options.BuildSubOptions options, string tmpl, Invoice invoice, Contract contract, Action<string> log, Action<string> error)
        {
            if (string.IsNullOrEmpty(tmpl))
                return;

            var html = Razor.Parse(tmpl, new InvoiceModel
            {

                Contract = contract,
                Invoice = invoice

            }, name + contract.GetHashCode());

            var htmlPath = Path.Combine(options.OutputPath, name + " #" + invoice.Number + ".html");

            log("Built " + htmlPath);
            File.WriteAllText(htmlPath, html);

            ConvertToPdf(htmlPath, log, error);
        }

        static void ConvertToPdf(string inputPath, Action<string> log, Action<string> error)
        {
            var pdfPath = Path.ChangeExtension(inputPath, "pdf");
            log("Convert to pdf " + pdfPath);

            var args = new StringBuilder();

            args.Append(" -q ");
            args.Append(" --orientation Portrait");
            args.Append(" --minimum-font-size 22");
            args.Append(" --image-dpi 300");
            args.Append(" --margin-top 10mm --margin-bottom 10mm --margin-right 15mm --margin-left 15mm ");


            args.Append(" \"" + inputPath + "\" ");
            args.Append(" \"" + pdfPath + "\" ");

            var start = new ProcessStartInfo
            {
                FileName = ".\\wkhtmltopdf.exe",
                Arguments = args.ToString(),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };

            log(start.FileName + " " + start.Arguments);

            using (var p = Process.Start(start))
            {
                if (p == null)
                {
                    error("Couldn't start the converter process: " + start.FileName);
                    Env.Exit();
                }

                // var o = p.StandardOutput;
                var e = p.StandardError;

                p.WaitForExit(300000);

                using (var reader = e)
                {
                    var result = reader.ReadToEnd();
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        log(result);
                    }
                }

                if (!p.HasExited)
                {
                    error("Printing failed. Wkhtmltopdf had not exited on time basis");
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {
                        error("Could not kill the converter process");
                    }
                }

                // Delete the input html
            }
        }
    }
}