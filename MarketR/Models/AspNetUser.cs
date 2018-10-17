//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarketR.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            this.Deal_View = new HashSet<Deal_View>();
            this.PermissionForCreatingUsers = new HashSet<PermissionForCreatingUser>();
            this.AspNetRoles = new HashSet<AspNetRole>();
        }
    
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public string F_Name { get; set; }
        public string L_Name { get; set; }
        public string AD_ID { get; set; }
    
        public virtual ICollection<Deal_View> Deal_View { get; set; }
        public virtual ICollection<PermissionForCreatingUser> PermissionForCreatingUsers { get; set; }
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
    }
}
