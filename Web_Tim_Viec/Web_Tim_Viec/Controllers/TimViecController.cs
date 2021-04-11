using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Web_Tim_Viec.Models;

namespace Web_Tim_Viec.Controllers
{
    public class TimViecController : Controller
    {
        // GET: TimViec
        public ActionResult Index()
        {

            List<dataRecord> list = querySelection("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?q=");
            ViewBag.Quantity = getQuantityResult("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?q=");

            return View(list);
        }
        [HttpPost]
        public ActionResult Index(string inputNameWork, string optionWork, string optionCity)
        {
            List<dataRecord> list = new List<dataRecord>();
            if (inputNameWork == "")
            {

                if (optionWork == "0" && optionCity == "0")
                {
                    list = querySelection("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids&province_ids&q=");
                    ViewBag.Quantity = getQuantityResult("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids&province_ids&q=");
                }
                else if (optionWork == "0")
                {
                    list = querySelection("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids&province_ids[]=" + optionCity + "&q=");
                    ViewBag.Quantity = getQuantityResult("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids&province_ids[]=" + optionCity + "&q=");
                }
                else if (optionCity == "0")
                {
                    list = querySelection("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids[]=" + optionWork + "&province_ids&q=");
                    ViewBag.Quantity = getQuantityResult("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids[]=" + optionWork + "&province_ids&q=");
                }
            }
            else
            {
                string input = inputNameWork.Replace(" ", "%");
                if (optionWork == "0" && optionCity == "0")
                {
                    list = querySelection("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids&province_ids&q=" + input);
                    ViewBag.Quantity = getQuantityResult("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids&province_ids&q=" + input);
                    ViewBag.tuKhoaTimKiem = "Từ Khóa Tìm Kiếm: " + inputNameWork;
                }
                else if (optionWork == "0")
                {
                    list = querySelection("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids&province_ids[]=" + optionCity + "&q=" + input);
                    ViewBag.Quantity = getQuantityResult("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids&province_ids[]=" + optionCity + "&q=" + input);
                    ViewBag.tuKhoaTimKiem = "Từ Khóa Tìm Kiếm: " + inputNameWork;
                }
                else if (optionCity == "0")
                {
                    list = querySelection("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids[]=" + optionWork + "&province_ids&q=" + input);
                    ViewBag.Quantity = getQuantityResult("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids[]=" + optionWork + "&province_ids&q=" + input);
                    ViewBag.tuKhoaTimKiem = "Từ Khóa Tìm Kiếm: " + inputNameWork;
                }
                else
                {
                    list = querySelection("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids[]=" + optionWork + "&province_ids[]=" + optionCity + "&q=" + input);
                    ViewBag.Quantity = getQuantityResult("https://vieclam24h.vn/tim-kiem-viec-lam-nhanh?field_ids[]=" + optionWork + "&province_ids[]=" + optionCity + "&q=" + input);
                    ViewBag.tuKhoaTimKiem = "Từ Khóa Tìm Kiếm: " + inputNameWork;
                }
            }
            return View(list);
        }

        public List<dataRecord> querySelection(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument document = web.Load(url);
            HtmlNode[] nodeNames = document.DocumentNode.SelectNodes("//div[@class='box-list-job']//ul//li[contains(@class, 'jsx-896248193')]//a[@href]").ToArray();

            ArrayList arr = new ArrayList();

            HtmlNode[] nodeSalary = document.DocumentNode.SelectNodes("//div[@class='box-list-job']//div[@title='Mức lương']").ToArray();
            HtmlNode[] nodeDate = document.DocumentNode.SelectNodes("//div[@class='box-list-job']//div[@title='Hạn nộp hồ sơ']").ToArray();

            HtmlNode[] nodeCountry = document.DocumentNode.SelectNodes("//div[@class='box-list-job']//div[@class='jsx-896248193 job-desc truncate-ellipsis text-center']").ToArray();


            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//div[@class='jsx-896248193 job-ttl truncate-ellipsis']//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                string a = att.Value;
                arr.Add(a);
            }

            ArrayList arrayRecord = new ArrayList();
            for (int i = 0; i < arr.Count; i++)
            {
                string href = "https://vieclam24h.vn" + arr[i].ToString();
                string salary = nodeSalary[i].InnerText.ToString();
                string name = "";
                string nameCompany = "";
                string country = "";
                string date = nodeDate[i].InnerText.ToString();
                if (i == 0)
                {
                    name = nodeNames[i].InnerText;
                    nameCompany = nodeNames[i + 1].InnerText;
                }
                else
                {
                    name = nodeNames[i * 2].InnerText;
                    nameCompany = nodeNames[i * 2 + 1].InnerText;
                }
                if (i == 0)
                {
                    country = nodeCountry[1].InnerText.ToString();
                }
                else
                {
                    country = nodeCountry[i * 3 + 1].InnerText.ToString();
                }

                arrayRecord.Add(new Web_Tim_Viec.Models.dataRecord(href, name, nameCompany, salary, country, date));

            }

            List<dataRecord> list = arrayRecord.Cast<dataRecord>().ToList();
            return list;
        }

        public string getQuantityResult(string url)
        {
            string i = "";
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument document = web.Load(url);
            HtmlNode[] nodeQuantity = document.DocumentNode.SelectNodes("//div[@class='ttl-line-left font700 my-1 case-unset-mb']").ToArray();
            i = nodeQuantity[0].InnerText;
            return i;
        }

        [HttpGet]
        public ActionResult getAndSaveData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getAndSaveData(string inputURL)
        {
            viecLamEntities db = new viecLamEntities();
            if (inputURL == "")
            {
                ViewBag.ThongBao = "Bạn cần phải nhập URL để lấy dữ liệu";
            }
            else
            {
                


                try
                {
                    List<dataRecord> list = new List<dataRecord>();
                    list = querySelection(inputURL);


                    for (int i = 0; i < list.Count; i++)
                    {

                        infomationWork work = new infomationWork();

                        work.nameWork = list[i].name.ToString();
                        work.nameCompany = list[i].nameCompany.ToString();
                        work.href = list[i].href.ToString();
                        work.salary = list[i].salary.ToString();
                        work.country = list[i].country.ToString();
                        work.date = list[i].date.ToString();




                        db.infomationWork.Add(work);

                        db.SaveChanges();

                    }

                    ViewBag.ThongBao = "Thanh Cong";
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }



            }


            return View();
        }

        public ActionResult showData(String inputSearch)
        {
            viecLamEntities db = new viecLamEntities();


            if (inputSearch != null)
            {
                ViewBag.tuKhoaTimKiem = "Từ Khóa Tìm Kiếm: " +inputSearch;
                return View(db.infomationWork.Where(x => x.nameWork.StartsWith(inputSearch)));
            }

            List<infomationWork> list = db.infomationWork.ToList();
            return View(list);



        }

    }
}