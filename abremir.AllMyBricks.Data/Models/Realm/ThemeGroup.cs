﻿using abremir.AllMyBricks.Data.Interfaces;
using Realms;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    public class ThemeGroup : RealmObject, IReferenceData
    {
        [PrimaryKey]
        public string Value { get; set; }
    }
}
