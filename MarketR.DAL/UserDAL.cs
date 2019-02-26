
using MarketR.Common.Service.CsvParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MarketR.Common
{
    public class UserDAL
    {
        
        //public User SearchUser(string search)
        //{
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        var users = (from user in context.AspNetUsers
        //                     where user.UserName.Contains(search) || user.Email.Contains(search)
        //                     select new User
        //                     {
        //                         Id = user.Id,
        //                         UserName = user.UserName,
        //                         Email = user.Email,
        //                         Role = (string)user.AspNetRoles.Select(x => x.Name).FirstOrDefault()
        //                     }).ToList();
        //        return Json(users, JsonRequestBehavior.AllowGet);
        //    }
        //    //var result = new { Success = false };
        //    return Json(new HttpStatusCodeResult(400, "Ajax error test"), JsonRequestBehavior.AllowGet);
        //}
    }
}
