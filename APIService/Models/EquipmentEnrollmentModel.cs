//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;

//using sfShareLib;
//using System.ComponentModel.DataAnnotations;
//using System.Data.Entity;

//namespace sfAPIService.Models
//{
//    public class EquipmentEnrollmentModel
//    {
//        public class Format_Detail
//        {
//            public int Id { get; set; }
//            public int Ordering { get; set; }
//            public string Name { get; set; }
//            public string URL { get; set; }
//        }
//        public class Format_Create
//        {
//            [Required]
//            public int FactoryId { get; set; }
//            [Required]
//            public int EquipmentClassId { get; set; }
//            [Required]
//            public string EquipmentId { get; set; }
//            [Required]
//            public string ActivationKey { get; set; }
//            [Required]
//            public DateTime EnrollDate { get; set; }
//            [Required]
//            public int SuperAdminId { get; set; }
//            [Required]
//            public bool IsActivated { get; set; }
//            [Required]
//            public DateTime ActivationExpiredAt { get; set; }
//        }
//        public class Format_Update
//        {
//            public int FactoryId { get; set; }
//            public int EquipmentClassId { get; set; }
//            public string EquipmentId { get; set; }
//            public string ActivationKey { get; set; }
//            public DateTime EnrollDate { get; set; }
//            [Required]
//            public int SuperAdminId { get; set; }
//            public bool IsActivated { get; set; }
//            public DateTime ActivationExpiredAt { get; set; }
//        }
//        public List<Format_Detail> GetAllByCompanyId(int companyId)
//        {
//            using (CDStudioEntities dbEntity = new CDStudioEntities())
//            {
//                var L2Enty = from c in dbEntity.EquipmentEnrollment.AsNoTracking()
//                             where c.CompanyId == companyId
//                             orderby c.Ordering
//                             select c;

//                return L2Enty.Select(s => new Format_Detail
//                {
//                    Id = s.Id,
//                    Name = s.Name,
//                    Ordering = s.Ordering,
//                    URL = s.URL
//                }).ToList<Format_Detail>();
//            }
//        }

//        public Format_Detail GetById(int id, int companyId)
//        {
//            using (CDStudioEntities dbEntity = new CDStudioEntities())
//            {
//                EquipmentEnrollment existingData = (from c in dbEntity.EquipmentEnrollment.AsNoTracking()
//                                                  where c.Id == id && c.CompanyId == companyId
//                                                  select c).SingleOrDefault<EquipmentEnrollment>();

//                if (existingData == null)
//                    throw new CDSException(10502);

//                return new Format_Detail()
//                {
//                    Id = existingData.Id,
//                    Name = existingData.Name,
//                    Ordering = existingData.Ordering,
//                    URL = existingData.URL
//                };
//            }
//        }

//        public int Create(int companyId, Format_Create parseData)
//        {
//            using (CDStudioEntities dbEntity = new CDStudioEntities())
//            {
//                EquipmentEnrollment newData = new EquipmentEnrollment()
//                {
//                    CompanyId = companyId,
//                    Name = parseData.Name,
//                    Ordering = parseData.Ordering,
//                    URL = parseData.URL
//                };
//                dbEntity.EquipmentEnrollment.Add(newData);
//                dbEntity.SaveChanges();
//                return newData.Id;
//            }
//        }

//        public void Update(int id, Format_Update parseData, int companyId)
//        {
//            using (CDStudioEntities dbEntity = new CDStudioEntities())
//            {
//                //EquipmentEnrollment existingData = dbEntity.EquipmentEnrollment.Find(id);
//                EquipmentEnrollment existingData = (from c in dbEntity.EquipmentEnrollment
//                                                  where c.Id == id && c.CompanyId == companyId
//                                                  select c).SingleOrDefault<EquipmentEnrollment>();
//                if (existingData == null)
//                    throw new CDSException(10502);

//                if (parseData.Name != null)
//                    existingData.Name = parseData.Name;

//                if (parseData.Ordering > 0)
//                    existingData.Ordering = parseData.Ordering;

//                if (parseData.URL != null)
//                    existingData.URL = parseData.URL;

//                dbEntity.Entry(existingData).State = EntityState.Modified;
//                dbEntity.SaveChanges();
//            }
//        }

//        public void DeleteById(int id, int companyId)
//        {
//            using (CDStudioEntities dbEntity = new CDStudioEntities())
//            {
//                EquipmentEnrollment existingData = (from c in dbEntity.EquipmentEnrollment
//                                                  where c.Id == id && c.CompanyId == companyId
//                                                  select c).SingleOrDefault<EquipmentEnrollment>();
//                if (existingData == null)
//                    throw new CDSException(10502);

//                dbEntity.Entry(existingData).State = EntityState.Deleted;
//                dbEntity.SaveChanges();
//            }
//        }
//    }
//}