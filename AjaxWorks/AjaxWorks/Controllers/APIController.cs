using AjaxWorks.Models;
using Microsoft.AspNetCore.Mvc;
using MSIT155Site.Models.DTO;
using System.Text;

namespace AjaxWorks.Controllers
{
    public class APIController : Controller
    {
        private readonly MyDBContext _context;
        public readonly IWebHostEnvironment _environment;

        public APIController(MyDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
                
        public IActionResult checkAccount(string name)
        {
            if(!string.IsNullOrEmpty(name))
            {
                var existUser = _context.Members.FirstOrDefault(m => m.Name == name);
                if(existUser != null)
                {
                    return Content("帳號已存在", "text/plain", Encoding.UTF8);
                }
                else
                {
                    return Content("帳號可使用", "text/plain", Encoding.UTF8);
                }
            }
            return Content("請輸入內容", "text/plain", Encoding.UTF8);
        }

        public IActionResult Avatar(int id = 1)
        {
            Member? member = _context.Members.Find(id);
            if(member!= null) {
                byte[] img = member.FileData;
                if(img != null)
                {
                    return File(img, "image/jpeg");
                }
            }
            return NotFound();
        }

        //public IActionResult Register(string name, int age = 28)
        [HttpPost]
        public IActionResult Register(Member _user, IFormFile avatar)
        {
            if(string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }
            string fileName = "noimage.png";
            if (avatar != null)
            {
                fileName = avatar.FileName;
            }
            string uploadPath = Path.Combine(_environment.WebRootPath, "images", fileName);
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                avatar?.CopyTo(fileStream);
            }

            //return Content($"Hello {_user.Name}, {_user.Age}歲了, 電子郵件是 {_user.Email}","text/plain", Encoding.UTF8);
            // return Content($"{_user.Avatar?.FileName} - {_user.Avatar?.Length} - {_user.Avatar?.ContentType}");
            //新增到資料庫
            _user.FileName = fileName;
            //轉成二進位
            byte[]? imgByte = null;
            using (var memoryStream = new MemoryStream())
            {
                avatar?.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }
            _user.FileData = imgByte;
            _context.Members.Add(_user);
            _context.SaveChanges();

            return Content(uploadPath);
        }
        public IActionResult Index()
        {
            Thread.Sleep(50000);
            //int x = 5;
            //int y = 0;
            //int z = x / y; 
            //輸入文字內容 ("純文字","資料格式",編碼方式) 若有非英文字必須加上編碼，否則會亂碼
            return Content("<h2>你好 Content!</h2>", "text/html", Encoding.UTF8);
        }

        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(a => a.City).Distinct();
            return Json(cities);
        }

        //根據城市名稱讀取鄉鎮區
        public IActionResult District(string city)
        {
            var districts = _context.Addresses.Where(c=>c.City==city).Select(a=>a.SiteId).Distinct();
            return Json(districts);
        }

        public IActionResult Road(string district)
        {
            var roads = _context.Addresses.Where(c => c.SiteId == district).Select(a => a.Road).Distinct();
            return Json(roads);
        }

        //利用標題查詢關鍵字
        public IActionResult SpotTitle(string title)
        {
            var titles = _context.Spots.Where(s => s.SpotTitle.Contains(title)).Select(s => s.SpotTitle).Take(8);
            return Json(titles);
        }


        //傳遞Json格式必須加上FromBody=>可以自動幫忙跟Class屬性做Binding
        [HttpPost]
        public IActionResult Spots([FromBody]SearchDTO searchDTO)
        {
            //分類編號搜尋
            var spots = searchDTO.categoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == searchDTO.categoryId);
            //關鍵字搜尋
            if (!string.IsNullOrEmpty(searchDTO.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(searchDTO.keyword) || s.SpotDescription.Contains(searchDTO.keyword));
            }
            //排序
            switch (searchDTO.sortBy)
            {
                case "spotTitle":
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);

                    break;
                default: //spotId
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }
            //總共幾筆
            int totalCount = spots.Count();
            //一頁幾筆資料
            int pageSize = searchDTO.pageSize ?? 9; //預設值
            //計算共幾頁
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //目前第幾頁
            int page = searchDTO.page ?? 1;

            //分頁
            spots= spots.Skip((page-1)*pageSize).Take(pageSize);
            //套用進類別中
            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalPages = totalPages;
            spotsPaging.SpotsResult = spots.ToList();

            return Json(spotsPaging);
        }
    }
}
