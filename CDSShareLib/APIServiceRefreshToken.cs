//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CDSShareLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class APIServiceRefreshToken
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
        public string ProtectedTicket { get; set; }
        public System.DateTime IssuedAt { get; set; }
        public System.DateTime ExpiredAt { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
