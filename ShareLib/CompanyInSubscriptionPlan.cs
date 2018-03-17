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
    
    public partial class CompanyInSubscriptionPlan
    {
        public int Id { get; set; }
        public int CompanyID { get; set; }
        public int SubscriptionPlanID { get; set; }
        public string SubscriptionName { get; set; }
        public Nullable<double> RatePer1KMessageIngestion { get; set; }
        public Nullable<double> RatePer1KMessageHotStore { get; set; }
        public Nullable<double> RatePer1KMessageColdStore { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime ExpiredDate { get; set; }
        public int MaxMessageQuotaPerDay { get; set; }
        public bool StoreHotMessage { get; set; }
        public bool StoreColdMessage { get; set; }
        public string CosmosDBConnectionString { get; set; }
        public string CosmosDBName { get; set; }
        public string CosmosDBCollectionID { get; set; }
        public Nullable<int> CosmosDBCollectionTTL { get; set; }
        public Nullable<int> CosmosDBCollectionReservedUnits { get; set; }
        public string IoTHubConnectionString { get; set; }
        public string IoTHubConsumerGroup { get; set; }
        public string StorageConnectionString { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    }
}
