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
    
    public partial class MetaDataDefination
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MetaDataDefination()
        {
            this.MetaDataValue = new HashSet<MetaDataValue>();
        }
    
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string EntityType { get; set; }
        public string ObjectName { get; set; }
    
        public virtual Company Company { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MetaDataValue> MetaDataValue { get; set; }
    }
}
