using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using sfShareLib;
using System.Data.Entity;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class FactoryModel
    {

        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string PhotoURL { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int TimeZone { get; set; }
            public string CultureInfoId { get; set; }
            public string CultureInfoName { get; set; }
        }

        public class Format_DetailForExternal
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string PhotoURL { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int TimeZone { get; set; }
            public string CultureInfoName { get; set; }
        }

        public class Format_DetailForInternal
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int CompanyId { get; set; }
            public string Description { get; set; }
            public string PhotoURL { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int TimeZone { get; set; }
            public string CultureInfoId { get; set; }
            public string CultureInfoName { get; set; }
        }

        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            [Required]
            public int TimeZone { get; set; }
            public string CultureInfoId { get; set; }
        }

        public class Format_Update
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public float? Latitude { get; set; }
            public float? Longitude { get; set; }
            public int? TimeZone { get; set; }
            public string CultureInfoId { get; set; }
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Factory.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                    Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                    PhotoURL = s.PhotoURL,
                    TimeZone = s.TimeZone,
                    CultureInfoId = s.CultureInfo,
                    CultureInfoName = s.RefCultureInfo.Name
                }).ToList<Format_Detail>();
            }
        }

        public List<Format_DetailForExternal> External_GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Factory.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;

                return L2Enty.Select(s => new Format_DetailForExternal
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                    Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                    PhotoURL = s.PhotoURL,
                    TimeZone = s.TimeZone,
                    CultureInfoName = s.RefCultureInfo.Name
                }).ToList<Format_DetailForExternal>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Factory factory = (from c in dbEntity.Factory.AsNoTracking()
                                   where c.Id == id
                                   select c).SingleOrDefault<Factory>();

                if (factory == null)
                    throw new CDSException(10601);

                return new Format_Detail()
                {
                    Id = factory.Id,
                    Name = factory.Name,
                    Description = factory.Description,
                    Latitude = (factory.Latitude == null) ? "" : factory.Latitude.ToString(),
                    Longitude = (factory.Longitude == null) ? "" : factory.Longitude.ToString(),
                    PhotoURL = factory.PhotoURL,
                    TimeZone = factory.TimeZone,
                    CultureInfoId = factory.CultureInfo,
                    CultureInfoName = factory.RefCultureInfo.Name
                };
            }
        }

        public Format_DetailForExternal External_GetById(int id, int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Factory factory = (from c in dbEntity.Factory.AsNoTracking()
                                   where c.Id == id && c.CompanyId == companyId
                                   select c).SingleOrDefault<Factory>();

                if (factory == null)
                    throw new CDSException(10601);

                return new Format_DetailForExternal()
                {
                    Id = factory.Id,
                    Name = factory.Name,
                    Description = factory.Description,
                    Latitude = (factory.Latitude == null) ? "" : factory.Latitude.ToString(),
                    Longitude = (factory.Longitude == null) ? "" : factory.Longitude.ToString(),
                    PhotoURL = factory.PhotoURL,
                    TimeZone = factory.TimeZone,
                    CultureInfoName = factory.RefCultureInfo.Name
                };
            }
        }

        public Format_DetailForInternal GetByIdForInternal(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Factory factory = (from c in dbEntity.Factory.AsNoTracking()
                                   where c.Id == id
                                   select c).SingleOrDefault<Factory>();

                if (factory == null)
                    throw new CDSException(10601);

                return new Format_DetailForInternal()
                {
                    Id = factory.Id,
                    CompanyId = factory.CompanyId,
                    Name = factory.Name,
                    Description = factory.Description,
                    Latitude = (factory.Latitude == null) ? "" : factory.Latitude.ToString(),
                    Longitude = (factory.Longitude == null) ? "" : factory.Longitude.ToString(),
                    PhotoURL = factory.PhotoURL,
                    TimeZone = factory.TimeZone,
                    CultureInfoId = factory.CultureInfo,
                    CultureInfoName = factory.RefCultureInfo.Name
                };
            }
        }

        public int Create(int companyId, Format_Create factory)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Factory newFactory = new Factory()
                {
                    Name = factory.Name,
                    Description = factory.Description ?? "",
                    CompanyId = companyId,
                    Latitude = factory.Latitude,
                    Longitude = factory.Longitude,
                    PhotoURL = "",
                    TimeZone = factory.TimeZone,
                    CultureInfo = factory.CultureInfoId
                };
                dbEntity.Factory.Add(newFactory);
                dbEntity.SaveChanges();
                return newFactory.Id;
            }
        }

        public void Update(int id, Format_Update factory)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Factory existingFactory = dbEntity.Factory.Find(id);
                if (existingFactory == null)
                    throw new CDSException(10601);

                if (factory.Name != null)
                    existingFactory.Name = factory.Name;

                if (factory.Description != null)
                    existingFactory.Description = factory.Description;

                if (factory.Latitude.HasValue)
                    existingFactory.Latitude = factory.Latitude;

                if (factory.Longitude.HasValue)
                    existingFactory.Longitude = factory.Longitude;

                if (factory.TimeZone.HasValue)
                    existingFactory.TimeZone = Convert.ToInt32(factory.TimeZone);

                if (factory.CultureInfoId != null)
                    existingFactory.CultureInfo = factory.CultureInfoId;

                dbEntity.Entry(existingFactory).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Factory existingFactory = dbEntity.Factory.Find(id);
                if (existingFactory == null)
                    throw new CDSException(10601);
                
                dbEntity.Entry(existingFactory).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }

        public void UpdatePhotoURL(int id, string url)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingFactory = dbEntity.Factory.Find(id);
                if (existingFactory == null)
                    throw new CDSException(10601);

                existingFactory.PhotoURL = url;
                dbEntity.SaveChanges();
            }
        }
    }
}