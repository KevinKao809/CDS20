using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using sfShareLib;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class ApplicationModel
    {

        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public JObject MessageTemplate { get; set; }
            public string TargetType { get; set; }
            public string Method { get; set; }
            public string ServiceURL { get; set; }
            public string AuthType { get; set; }
            public string AuthID { get; set; }
            public string AuthPW { get; set; }
            public string TokenURL { get; set; }
            public string HeaderValues { get; set; }            
        }

        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            /// <summary>
            /// Should be Json Format
            /// </summary>
            public string MessageTemplate { get; set; }
            [Required]
            public string TargetType { get; set; }
            [Required]
            public string Method { get; set; }
            [Required]
            public string ServiceURL { get; set; }
            [Required]
            public string AuthType { get; set; }
            public string AuthID { get; set; }
            public string AuthPW { get; set; }
            public string TokenURL { get; set; }
            public string HeaderValues { get; set; }
        }

        public class Format_Update
        {
            public string Name { get; set; }
            public string Description { get; set; }
            /// <summary>
            /// Should be Json Format
            /// </summary>
            public string MessageTemplate { get; set; }
            public string TargetType { get; set; }
            public string Method { get; set; }
            public string ServiceURL { get; set; }
            public string AuthType { get; set; }
            public string AuthID { get; set; }
            public string AuthPW { get; set; }
            public string TokenURL { get; set; }
            public string HeaderValues { get; set; }
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                List<Application> applicationList = (from c in dbEntity.Application.AsNoTracking()
                                                     where c.CompanyId == companyId
                                                     select c).ToList<Application>();

                List<Format_Detail> returnDataList = new List<Format_Detail>();
                foreach (var application in applicationList)
                {
                    returnDataList.Add(new Format_Detail()
                    {
                        Id = application.Id,
                        Name = application.Name,
                        Description = application.Description,
                        MessageTemplate = JObject.Parse(application.MessageTemplate),
                        TargetType = application.TargetType,
                        Method = application.Method,
                        ServiceURL = application.ServiceURL,
                        AuthType = application.AuthType,
                        AuthID = application.AuthID,
                        AuthPW = application.AuthPW,
                        TokenURL = application.TokenURL,
                        HeaderValues = application.HeaderValues
                    });
                }

                return returnDataList;
            }
        }
        
        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Application application = (from c in dbEntity.Application.AsNoTracking()
                                   where c.Id == id
                                   select c).SingleOrDefault<Application>();

                if (application == null)
                    throw new CDSException(10102);

                return new Format_Detail()
                {
                    Id = application.Id,
                    Name = application.Name,
                    Description = application.Description,
                    MessageTemplate = JObject.Parse(application.MessageTemplate),
                    TargetType = application.TargetType,
                    Method = application.Method,
                    ServiceURL = application.ServiceURL,
                    AuthType = application.AuthType,
                    AuthID = application.AuthID,
                    AuthPW = application.AuthPW,
                    TokenURL = application.TokenURL,
                    HeaderValues = application.HeaderValues
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            try
            {
                JObject.Parse(parseData.MessageTemplate);
            }
            catch
            {
                throw new CDSException(10103);
            }

            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Application newApplication = new Application()
                {
                    Name = parseData.Name,
                    Description = parseData.Description ?? "",
                    CompanyId = companyId,
                    MessageTemplate = parseData.MessageTemplate ?? "{}",
                    TargetType = parseData.TargetType,
                    Method = parseData.Method,
                    ServiceURL = parseData.ServiceURL,
                    AuthType = parseData.AuthType,
                    AuthID = parseData.AuthID ?? "",
                    AuthPW = parseData.AuthPW ?? "",                    
                    TokenURL = parseData.TokenURL ?? "",
                    HeaderValues = parseData.HeaderValues ?? ""
                };
                dbEntity.Application.Add(newApplication);
                dbEntity.SaveChanges();
                return newApplication.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Application existingApplication = dbEntity.Application.Find(id);
                if (existingApplication == null)
                    throw new CDSException(10102);

                if (parseData.MessageTemplate != null)
                {
                    try
                    {
                        JObject.Parse(parseData.MessageTemplate);
                    }
                    catch
                    {
                        throw new CDSException(10103);
                    }
                    existingApplication.MessageTemplate = parseData.MessageTemplate;
                }

                if (parseData.Name != null)
                    existingApplication.Name = parseData.Name;

                if (parseData.Description != null)
                    existingApplication.Description = parseData.Description;

                if(parseData.TargetType != null)
                    existingApplication.TargetType = parseData.TargetType;

                if (parseData.Method != null)
                    existingApplication.Method = parseData.Method;

                if (parseData.ServiceURL != null)
                    existingApplication.ServiceURL = parseData.ServiceURL;

                if (parseData.AuthType != null)
                    existingApplication.AuthType = parseData.AuthType;

                if (parseData.AuthID != null)
                    existingApplication.AuthID = parseData.AuthID;

                if (parseData.AuthPW != null)
                    existingApplication.AuthPW = parseData.AuthPW;

                if (parseData.TokenURL != null)
                    existingApplication.TokenURL = parseData.TokenURL;

                if (parseData.HeaderValues != null)
                    existingApplication.HeaderValues = parseData.HeaderValues;

                dbEntity.Entry(existingApplication).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Application existingApplication = dbEntity.Application.Find(id);
                if (existingApplication == null)
                    throw new CDSException(10102);
                
                dbEntity.Entry(existingApplication).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
    }
}