﻿using AjaxWorks.Models;
using Microsoft.AspNetCore.Mvc;
using MSIT155Site.Models.DTO;
using System.Text;

namespace AjaxWorks.Controllers
{
    public class APIController : Controller
    {
        private readonly MyDBContext _context;
        public APIController(MyDBContext context)
        {
            _context = context;
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
        public IActionResult Register(UserDTO _user)
        {
            if(string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }
            return Content($"Hello {_user.Name}, {_user.Age}歲了, 電子郵件是 {_user.Email}","text/plain", Encoding.UTF8);
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


    }
}
