using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using sfShareLib;

namespace sfAPIService.Models
{
    public class ApplicationModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int CompanyId { get; set; }
            public JObject MessageTemplate { get; set; }
            public string Method { get; set; }
            public string ServiceURL { get; set; }
            public string AuthType { get; set; }
            public string AuthID { get; set; }
            public string AuthPW { get; set; }
            public string TokenURL { get; set; }
            public string HeaderValues { get; set; }
            public string TargetType { get; set; }
        }

        public class Add
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            [Required]
            public int CompanyId { get; set; }
            public string MessageTemplate { get; set; }
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
            [Required]
            public string TargetType { get; set; }
        }

        public class Update
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            public string MessageTemplate { get; set; }
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
            [Required]
            public string TargetType { get; set; }
        }


        public List<Detail> GetAllApplicationByCompanyId(int companyId)
        {
            DBHelper._Application dbhelp = new DBHelper._Application();
            List<Application> externalApps = dbhelp.GetAllByCompanyId(companyId);
            List<Detail> returnList = new List<Detail>();

            foreach (var externalApp in externalApps)
            {
                try
                {
                    returnList.Add(new Detail()
                    {
                        Id = externalApp.Id,
                        Name = externalApp.Name,
                        Description = externalApp.Description,
                        CompanyId = externalApp.CompanyId,
                        MessageTemplate = (externalApp.MessageTemplate == null) ? null : JObject.Parse(externalApp.MessageTemplate),
                        Method = externalApp.Method,
                        ServiceURL = externalApp.ServiceURL,
                        AuthType = externalApp.AuthType,
                        AuthID = externalApp.AuthID,
                        AuthPW = externalApp.AuthPW,
                        TokenURL = externalApp.TokenURL,
                        HeaderValues = externalApp.HeaderValues,
                        TargetType = externalApp.TargetType
                    });
                }
                catch { }
            }

            return returnList;
        }

        public Detail getApplicationById(int id)
        {
            DBHelper._Application dbhelp = new DBHelper._Application();
            Application application = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = application.Id,
                Name = application.Name,
                Description = application.Description,
                CompanyId = application.CompanyId,
                MessageTemplate = (application.MessageTemplate == null) ? null : JObject.Parse(application.MessageTemplate),
                Method = application.Method,
                ServiceURL = application.ServiceURL,
                AuthType = application.AuthType,
                AuthID = application.AuthID,
                AuthPW = application.AuthPW,
                TokenURL = application.TokenURL,
                HeaderValues = application.HeaderValues,
                TargetType = application.TargetType
            };
        }

        public void addApplication(Add application)
        {            
            try
            {
                if (application.MessageTemplate != null)
                {
                    JObject tmp = JObject.Parse(application.MessageTemplate);
                }
            }
            catch
            {
                throw new Exception("MessageTemplate must be in Json fromat");
            }

            DBHelper._Application dbhelp = new DBHelper._Application();
            var newApplication = new Application()
            {
                Name = application.Name,
                Description = application.Description,
                CompanyId = application.CompanyId,
                MessageTemplate = application.MessageTemplate,
                Method = application.Method,
                ServiceURL = application.ServiceURL,
                AuthType = application.AuthType,
                AuthID = application.AuthID,
                AuthPW = application.AuthPW,
                TokenURL = application.TokenURL,
                HeaderValues = application.HeaderValues,
                TargetType = application.TargetType
            };
            dbhelp.Add(newApplication);
        }

        public void updateApplication(int id, Update application)
        {
            try
            {
                if (application.MessageTemplate != null)
                {
                    JObject tmp = JObject.Parse(application.MessageTemplate);
                }
            }
            catch
            {
                throw new Exception("MessageTemplate must be in Json fromat");
            }

            DBHelper._Application dbhelp = new DBHelper._Application();
            Application existingApplication = dbhelp.GetByid(id);
            existingApplication.Name = application.Name;
            existingApplication.Description = application.Description;
            existingApplication.MessageTemplate = application.MessageTemplate;
            existingApplication.Method = application.Method;
            existingApplication.ServiceURL = application.ServiceURL;
            existingApplication.AuthType = application.AuthType;
            existingApplication.AuthID = application.AuthID;
            existingApplication.AuthPW = application.AuthPW;
            existingApplication.TokenURL = application.TokenURL;
            existingApplication.HeaderValues = application.HeaderValues;
            existingApplication.TargetType = application.TargetType;

            dbhelp.Update(existingApplication);
        }

        public void deleteApplication(int id)
        {
            DBHelper._Application dbhelp = new DBHelper._Application();
            Application existingApplication = dbhelp.GetByid(id);

            dbhelp.Delete(existingApplication);
        }
    }
}