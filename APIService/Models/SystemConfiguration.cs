using sfShareLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sfAPIService.Models
{
    public class SystemConfigurationModels
    {
        public class Detail
        {
            public string Group { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Edit
        {
            public string Group { get; set; }
            [Required]
            public string Key { get; set; }
            [Required]
            public string Value { get; set; }
        }
        public List<Detail> GetAllSystemConfiguration()
        {
            DBHelper.APIService dbhelp = new DBHelper.APIService();

            return dbhelp.GetAllSystemConfiguration().Select(s => new Detail()
            {
                Group = s.Group,
                Key = s.Key,
                Value = s.Value
            }).ToList<Detail>();
        }

        public void addSystemConfiguration(Edit systemConfiguration)
        {
            DBHelper.APIService dbhelp = new DBHelper.APIService();
            var newSystemConfiguration = new SystemConfiguration()
            {
                Group = systemConfiguration.Group,
                Key = systemConfiguration.Key,
                Value = systemConfiguration.Value
            };
            dbhelp.AddSystemConfiguration(newSystemConfiguration);
        }

        public void updateSystemConfiguration(int id, Edit systemConfiguration)
        {
            DBHelper.APIService dbhelp = new DBHelper.APIService();
            SystemConfiguration existingSystemConfiguration = dbhelp.GetSystemConfigurationById(id);
            existingSystemConfiguration.Group = systemConfiguration.Group;
            existingSystemConfiguration.Key = systemConfiguration.Key;
            existingSystemConfiguration.Value = systemConfiguration.Value;

            dbhelp.UpdateSystemConfiguration(existingSystemConfiguration);
        }

        public void deleteSystemConfiguration(int id)
        {
            DBHelper.APIService dbhelp = new DBHelper.APIService();
            SystemConfiguration existingSystemConfiguration = dbhelp.GetSystemConfigurationById(id);

            dbhelp.DeleteSystemConfiguration(existingSystemConfiguration);
        }
    }
}