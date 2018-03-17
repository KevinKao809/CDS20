using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class WidgetClassModel
    {
        public class Format_Detail
        {
            public int Key { get; set; }
            public string Name { get; set; }
            public string Level { get; set; }
            public string PhotoURL { get; set; }
            public bool AllowMultipleAppearOnBoard { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public int Key { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public string Level { get; set; }
            public string PhotoURL { get; set; }
            [Required]
            public bool AllowMultipleAppearOnBoard { get; set; }
        }

        public class Format_Update
        {
            public string Name { get; set; }
            public string Level { get; set; }
            public string PhotoURL { get; set; }
            public bool? AllowMultipleAppearOnBoard { get; set; }
        }
        public List<Format_Detail> GetAll(string level)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                List<Format_Detail> returnDataList = new List<Format_Detail>();
                if (string.IsNullOrEmpty(level))
                {
                    var L2Enty = from c in dbEntity.WidgetClass.AsNoTracking()
                                 select c;

                    return L2Enty.Select(s => new Format_Detail()
                    {
                        Key = s.Key,
                        Name = s.Name,
                        Level = s.Level,
                        PhotoURL = s.PhotoURL,
                        AllowMultipleAppearOnBoard = s.AllowMultipleAppearOnBoard
                    }).ToList<Format_Detail>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.WidgetClass.AsNoTracking()
                                 where c.Level == level.ToLower()
                                 select c;

                    return L2Enty.Select(s => new Format_Detail()
                    {
                        Key = s.Key,
                        Name = s.Name,
                        Level = s.Level,
                        PhotoURL = s.PhotoURL,
                        AllowMultipleAppearOnBoard = s.AllowMultipleAppearOnBoard
                    }).ToList<Format_Detail>();
                }
            }
        }

        public Format_Detail GetByKey(int key)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                WidgetClass WidgetClass = (from c in dbEntity.WidgetClass.AsNoTracking()
                                           where c.Key == key
                                           select c).SingleOrDefault<WidgetClass>();
                if (WidgetClass == null)
                    throw new CDSException(12301);

                return new Format_Detail()
                {
                    Key = WidgetClass.Key,
                    Name = WidgetClass.Name,
                    Level = WidgetClass.Level,
                    PhotoURL = WidgetClass.PhotoURL,
                    AllowMultipleAppearOnBoard = WidgetClass.AllowMultipleAppearOnBoard
                };
            }
        }

        public int Create(Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                WidgetClass newWidgetClass = new WidgetClass();
                newWidgetClass.Name = parseData.Name;
                newWidgetClass.Key = parseData.Key;
                newWidgetClass.Level = parseData.Level;
                newWidgetClass.PhotoURL = parseData.PhotoURL ?? "";
                newWidgetClass.AllowMultipleAppearOnBoard = parseData.AllowMultipleAppearOnBoard;

                dbEntity.WidgetClass.Add(newWidgetClass);
                dbEntity.SaveChanges();
                return newWidgetClass.Id;
            }
        }

        public void UpdateByKey(int key, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.WidgetClass.Find(key);
                if (existingData == null)
                    throw new CDSException(12301);

                if (parseData.Name != null)
                    existingData.Name = parseData.Name;

                if (parseData.Level != null)
                    existingData.Level = parseData.Level;

                if (parseData.PhotoURL != null)
                    existingData.PhotoURL = parseData.PhotoURL;

                if (parseData.AllowMultipleAppearOnBoard.HasValue)
                    existingData.AllowMultipleAppearOnBoard = (bool)parseData.AllowMultipleAppearOnBoard;
                dbEntity.SaveChanges();
            }
        }

        public void DeleteByKey(int key)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                WidgetClass existingData = dbEntity.WidgetClass.Find(key);
                if (existingData == null)
                    throw new CDSException(12301);

                dbEntity.WidgetClass.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}