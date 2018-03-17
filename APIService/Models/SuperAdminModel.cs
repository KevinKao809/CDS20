using CDSShareLib.Helper;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace sfAPIService.Models
{
    public class SuperAdminModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool DeletedFlag { get; set; }
        }
        public class Format_Create
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
        }
        public class Format_Update
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool? DeletedFlag { get; set; }
        }

        public List<Format_Detail> GetAll()
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.SuperAdmin.AsNoTracking()
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    CreatedAt = s.CreatedAt,
                    DeletedFlag = s.DeletedFlag
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SuperAdmin SuperAdmin = (from c in dbEntity.SuperAdmin.AsNoTracking()
                                         where c.Id == id
                                         select c).SingleOrDefault<SuperAdmin>();
                if (SuperAdmin == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = SuperAdmin.Id,
                    FirstName = SuperAdmin.FirstName,
                    LastName = SuperAdmin.LastName,
                    Email = SuperAdmin.Email,
                    CreatedAt = SuperAdmin.CreatedAt,
                    DeletedFlag = SuperAdmin.DeletedFlag
                };
            }
        }

        public Format_Detail GetByEmail(string email)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SuperAdmin SuperAdmin = (from c in dbEntity.SuperAdmin.AsNoTracking()
                                         where c.Email == email
                                         select c).SingleOrDefault<SuperAdmin>();
                if (SuperAdmin == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = SuperAdmin.Id,
                    FirstName = SuperAdmin.FirstName,
                    LastName = SuperAdmin.LastName,
                    Email = SuperAdmin.Email,
                    CreatedAt = SuperAdmin.CreatedAt,
                    DeletedFlag = SuperAdmin.DeletedFlag
                };
            }
        }

        public int Create(Format_Create SuperAdmin)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SuperAdmin newSuperAdmin = new SuperAdmin();
                newSuperAdmin.FirstName = SuperAdmin.FirstName ?? "";
                newSuperAdmin.LastName = SuperAdmin.LastName ?? "";
                newSuperAdmin.Email = SuperAdmin.Email;
                newSuperAdmin.Password = Crypto.HashPassword(SuperAdmin.Password);
                newSuperAdmin.DeletedFlag = false;
                newSuperAdmin.CreatedAt = DateTime.UtcNow;

                dbEntity.SuperAdmin.Add(newSuperAdmin);
                dbEntity.SaveChanges();
                return newSuperAdmin.Id;
            }
        }

        public void Update(int id, Format_Update dataModel)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingSuperAdmin = dbEntity.SuperAdmin.Find(id);
                if (existingSuperAdmin == null)
                    throw new CDSException(10701);                

                if (dataModel.FirstName != null)
                    existingSuperAdmin.FirstName = dataModel.FirstName;

                if (dataModel.LastName != null)
                    existingSuperAdmin.LastName = dataModel.LastName;

                if (dataModel.DeletedFlag.HasValue)
                    existingSuperAdmin.DeletedFlag = (bool)dataModel.DeletedFlag;

                existingSuperAdmin.UpdatedAt = DateTime.UtcNow;
                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SuperAdmin existingData = dbEntity.SuperAdmin.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                existingData.DeletedFlag = true;

                dbEntity.Entry(existingData).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        public void ChangePassword(int id, PasswordModel.Format_Change model)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.SuperAdmin.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                if (Crypto.VerifyHashedPassword(existingData.Password, model.OldPassword))
                {
                    existingData.Password = Crypto.HashPassword(model.NewPassword);
                    dbEntity.SaveChanges();
                }
                else
                    throw new CDSException(10101);
            }
        }

        public bool VerifyPassword(string email, string password)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SuperAdmin superAdmin = (from c in dbEntity.SuperAdmin.AsNoTracking()
                                         where c.Email == email
                                         select c).SingleOrDefault<SuperAdmin>();

                if (superAdmin != null && Crypto.VerifyHashedPassword(superAdmin.Password, password))
                    return true;

                return false;
            }
        }
    }
}