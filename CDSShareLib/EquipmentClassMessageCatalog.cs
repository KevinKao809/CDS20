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
    
    public partial class EquipmentClassMessageCatalog
    {
        public int Id { get; set; }
        public int EquipmentClassID { get; set; }
        public int MessageCatalogID { get; set; }
    
        public virtual EquipmentClass EquipmentClass { get; set; }
        public virtual MessageCatalog MessageCatalog { get; set; }
    }
}
