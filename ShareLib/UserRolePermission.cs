//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace sfShareLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRolePermission
    {
        public int Id { get; set; }
        public int UserRoleID { get; set; }
        public int PermissionCatalogCode { get; set; }
    
        public virtual PermissionCatalog PermissionCatalog { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}
