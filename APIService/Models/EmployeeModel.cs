using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using System.Data.Entity;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class EmployeeModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string EmployeeNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhotoURL { get; set; }
            public bool AdminFlag { get; set; }
            public string Lang { get; set; }
        }

        public class Format_Create
        {
            public string EmployeeNumber { get; set; }
            [Required]
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public bool AdminFlag { get; set; }
            public string Lang { get; set; }
        }

        public class Format_Update
        {
            public string EmployeeNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool? AdminFlag { get; set; }
            public string Lang { get; set; }
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    EmployeeNumber = s.EmployeeNumber,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    PhotoURL = s.PhotoURL,
                    AdminFlag = s.AdminFlag,
                    Lang = s.Lang
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Employee employee = (from c in dbEntity.Employee.AsNoTracking()
                                     where c.Id == id
                                     select c).SingleOrDefault<Employee>();
                if (employee == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = employee.Id,
                    EmployeeNumber = employee.EmployeeNumber,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    PhotoURL = employee.PhotoURL,
                    AdminFlag = employee.AdminFlag,
                };
            }
        }

        public Format_Detail GetByEmail(string email)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Employee employee = (from c in dbEntity.Employee.AsNoTracking()
                                     where c.Email == email
                                     select c).SingleOrDefault<Employee>();
                if (employee == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = employee.Id,
                    EmployeeNumber = employee.EmployeeNumber,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    PhotoURL = employee.PhotoURL,
                    AdminFlag = employee.AdminFlag,
                };
            }
        }

        public int GetCompanyId(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Employee employee = (from c in dbEntity.Employee.AsNoTracking()
                                     where c.Id == id
                                     select c).SingleOrDefault<Employee>();
                if (employee == null)
                    throw new CDSException(10701);

                return employee.CompanyId;
            }
        }
           
        public int Create(int companyId, Format_Create employee)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Employee newEmployee = new Employee();
                newEmployee.CompanyId = companyId;
                newEmployee.EmployeeNumber = employee.EmployeeNumber ?? "";
                newEmployee.FirstName = employee.FirstName ?? "";
                newEmployee.LastName = employee.LastName ?? "";
                newEmployee.Email = employee.Email;
                newEmployee.PhotoURL = "";
                newEmployee.Password = Crypto.HashPassword(employee.Password);
                newEmployee.AdminFlag = employee.AdminFlag;
                newEmployee.Lang = employee.Lang;

                dbEntity.Employee.Add(newEmployee);
                dbEntity.SaveChanges();
                return newEmployee.Id;
            }
        }

        public void Update(int id, Format_Update dataModel)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingEmployee = dbEntity.Employee.Find(id);
                if (existingEmployee == null)
                    throw new CDSException(10701);

                if (dataModel.EmployeeNumber != null)
                    existingEmployee.EmployeeNumber = dataModel.EmployeeNumber;

                if (dataModel.FirstName != null)
                    existingEmployee.FirstName = dataModel.FirstName;

                if (dataModel.LastName != null)
                    existingEmployee.LastName = dataModel.LastName;

                if (dataModel.Lang != null)
                    existingEmployee.Lang = dataModel.Lang;

                if (dataModel.AdminFlag.HasValue)
                    existingEmployee.AdminFlag = (bool)dataModel.AdminFlag;

                dbEntity.SaveChanges();
            }
        }
        
        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Employee existingData = dbEntity.Employee.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }

        public void UpdatePhotoURL(int id, string photoURL, string iconURL)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingEmployee = dbEntity.Employee.Find(id);
                if (existingEmployee == null)
                    throw new CDSException(10701);

                existingEmployee.PhotoURL = photoURL;
                existingEmployee.IconURL = iconURL;
                dbEntity.SaveChanges();
            }
        }

        public int VerifyPassword(string email, string password)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Employee employee = (from c in dbEntity.Employee.AsNoTracking()
                                     where c.Email == email
                                     select c).SingleOrDefault<Employee>();

                if (employee != null && Crypto.VerifyHashedPassword(employee.Password, password))
                    return employee.CompanyId;
                else
                    return -1;
            }
        }

        public void ResetPassword(int id, PasswordModel.Format_Reset model)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingEmployee = dbEntity.Employee.Find(id);
                if (existingEmployee == null)
                    throw new CDSException(10701);

                existingEmployee.Password = Crypto.HashPassword(model.NewPassword);
                dbEntity.SaveChanges();
            }
        }
        public void ChangePassword(int id, PasswordModel.Format_Change model)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingEmployee = dbEntity.Employee.Find(id);
                if (existingEmployee == null)
                    throw new CDSException(10701);

                if (Crypto.VerifyHashedPassword(existingEmployee.Password, model.OldPassword))
                {
                    existingEmployee.Password = Crypto.HashPassword(model.NewPassword);
                    dbEntity.SaveChanges();
                }
                else
                    throw new CDSException(10101);
            }
        }
    }
    
}