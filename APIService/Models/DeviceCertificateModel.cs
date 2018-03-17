using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class DeviceCertificateModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CertFile { get; set; }
            public string KeyFile { get; set; }
            public string Thumbprint { get; set; }
            public string Password { get; set; }
            public DateTime? ExpiredAt { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            public string CertFile { get; set; }
            public string KeyFile { get; set; }
            [Required]
            public string Thumbprint { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            public DateTime ExpiredAt { get; set; }
        }
        public class Format_Update
        {
            public string Name { get; set; }
            public string CertFile { get; set; }
            public string KeyFile { get; set; }
            public string Thumbprint { get; set; }
            public string Password { get; set; }
            public DateTime? ExpiredAt { get; set; }
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.DeviceCertificate.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    Name = s.Name,
                    CertFile = s.CertFile,
                    KeyFile = s.KeyFile,
                    Thumbprint = s.Thumbprint,
                    Password = s.Password,
                    ExpiredAt = s.ExpiredAt
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceCertificate existingData = (from c in dbEntity.DeviceCertificate.AsNoTracking()
                                                  where c.Id == id
                                                  select c).SingleOrDefault<DeviceCertificate>();

                if (existingData == null)
                    throw new CDSException(10401);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    Name = existingData.Name,
                    CertFile = existingData.CertFile,
                    KeyFile = existingData.KeyFile,
                    Thumbprint = existingData.Thumbprint,
                    Password = existingData.Password,
                    ExpiredAt = existingData.ExpiredAt
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceCertificate newData = new DeviceCertificate()
                {
                    CompanyID = companyId,
                    Name = parseData.Name,
                    CertFile = parseData.CertFile ?? "",
                    KeyFile = parseData.KeyFile ?? "",
                    Thumbprint = parseData.Thumbprint,
                    Password = parseData.Password,
                    ExpiredAt = parseData.ExpiredAt
                };
                dbEntity.DeviceCertificate.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceCertificate existingData = dbEntity.DeviceCertificate.Find(id);
                if (existingData == null)
                    throw new CDSException(10401);

                if (parseData.Name != null)
                    existingData.Name = parseData.Name;

                if (parseData.KeyFile != null)
                    existingData.KeyFile = parseData.KeyFile;

                if (parseData.CertFile != null)
                    existingData.CertFile = parseData.CertFile;

                if (parseData.Thumbprint != null)
                    existingData.Thumbprint = parseData.Thumbprint;

                if (parseData.Password != null)
                    existingData.Password = parseData.Password;

                if (parseData.ExpiredAt != null)
                    existingData.ExpiredAt = parseData.ExpiredAt;

                dbEntity.Entry(existingData).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceCertificate existingData = dbEntity.DeviceCertificate.Find(id);
                if (existingData == null)
                    throw new CDSException(10401);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
    }
}