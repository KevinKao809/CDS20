using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class WidgetCatalogModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public int? MessageCatalogId { get; set; }
            public string Name { get; set; }
            public string Level { get; set; }
            public string MessageCatalogName { get; set; }            
            public int WidgetClassKey { get; set; }
            public string Title { get; set; }
            public string TitleBgColor { get; set; }
            public JObject Content { get; set; }
            public string ContentBgColor { get; set; }
        }
        public class Format_Create
        {
            public int? MessageCatalogId { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public string Level { get; set; }
            [Required]
            public int WidgetClassKey { get; set; }
            [Required]
            public string Title { get; set; }
            public string TitleBgColor { get; set; }
            public string Content { get; set; }
            public string ContentBgColor { get; set; }
        }

        public class Format_Update
        {
            public int? MessageCatalogId { get; set; }
            public string Name { get; set; }
            public string Level { get; set; }
            public int? WidgetClassKey { get; set; }
            public string Title { get; set; }
            public string TitleBgColor { get; set; }
            public string Content { get; set; }
            public string ContentBgColor { get; set; }
        }
        public List<Format_Detail> getAllWidgetCatalogByCompanyId(int companyId, string level)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.CompanyID == companyId && c.Level == level
                             select c;

                List<WidgetCatalog> widgetClassList = (from c in dbEntity.WidgetCatalog.AsNoTracking()
                                                     where c.CompanyID == companyId && c.Level == level
                                                     select c).ToList();

                List<Format_Detail> returnData = new List<Format_Detail>();
                foreach (var widgetClass in widgetClassList)
                {
                    returnData.Add(new Format_Detail()
                    {
                        Id = widgetClass.Id,
                        MessageCatalogId = widgetClass.MessageCatalogID,
                        MessageCatalogName = widgetClass.MessageCatalog == null ? "" : widgetClass.MessageCatalog.Name,
                        Name = widgetClass.Name,
                        Level = widgetClass.Level,
                        WidgetClassKey = widgetClass.WidgetClassKey,
                        Title = widgetClass.Title,
                        TitleBgColor = widgetClass.TitleBgColor,
                        Content = JObject.Parse(widgetClass.Content),
                        ContentBgColor = widgetClass.ContentBgColor
                    });
                }
                return returnData;
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                WidgetCatalog existingData = (from c in dbEntity.WidgetCatalog.AsNoTracking()
                                               where c.Id == id
                                               select c).SingleOrDefault<WidgetCatalog>();
                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    MessageCatalogId = existingData.MessageCatalogID,
                    MessageCatalogName = existingData.MessageCatalog == null ? "" : existingData.MessageCatalog.Name,
                    Name = existingData.Name,
                    Level = existingData.Level,
                    WidgetClassKey = existingData.WidgetClassKey,
                    Title = existingData.Title,
                    TitleBgColor = existingData.TitleBgColor,
                    Content = JObject.Parse(existingData.Content),
                    ContentBgColor = existingData.ContentBgColor
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            try
            {
                JObject.Parse(parseData.Content);
            }
            catch
            {
                throw new CDSException(12303);
            }

            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                WidgetCatalog newData = new WidgetCatalog();
                newData.CompanyID = companyId;
                newData.MessageCatalogID = parseData.MessageCatalogId;
                newData.Name = parseData.Name;
                newData.Level = parseData.Level;
                newData.WidgetClassKey = parseData.WidgetClassKey;
                newData.Title = parseData.Title;
                newData.TitleBgColor = parseData.TitleBgColor ?? "";
                newData.Content = parseData.Content;
                newData.ContentBgColor = parseData.ContentBgColor ?? "";

                dbEntity.WidgetCatalog.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.WidgetCatalog.Find(id);
                if (existingData == null)
                    throw new CDSException(12302);

                if (parseData.Content != null)
                {
                    try
                    {
                        JObject.Parse(parseData.Content);
                    }
                    catch
                    {
                        throw new CDSException(12303);
                    }
                    existingData.Content = parseData.Content;
                }

                if (parseData.Name != null)
                    existingData.Name = parseData.Name;

                if (parseData.MessageCatalogId.HasValue)
                    existingData.MessageCatalogID = parseData.MessageCatalogId;

                if (parseData.Level != null)
                    existingData.Level = parseData.Level;

                if (parseData.WidgetClassKey.HasValue)
                    existingData.WidgetClassKey = (int)parseData.WidgetClassKey;

                if (parseData.Title != null)
                    existingData.Title = parseData.Title;

                if (parseData.TitleBgColor != null)
                    existingData.TitleBgColor = parseData.TitleBgColor;
                                            
                if (parseData.ContentBgColor != null)
                    existingData.ContentBgColor = parseData.ContentBgColor;

                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                WidgetCatalog existingData = dbEntity.WidgetCatalog.Find(id);
                if (existingData == null)
                    throw new CDSException(12302);

                dbEntity.WidgetCatalog.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}