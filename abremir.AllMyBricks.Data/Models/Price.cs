﻿using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Extensions;
using System;
using System.Globalization;
using System.Linq;

namespace abremir.AllMyBricks.Data.Models
{
    public class Price
    {
        public PriceRegionEnum Region { get; set; }
        public float Value { get; set; }

        public override string ToString()
        {
            var culture = (from specificCulture in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                           let region = new RegionInfo(specificCulture.LCID)
                           where region?.ISOCurrencySymbol
                            .Equals(Region.GetDescription(), StringComparison.InvariantCultureIgnoreCase) == true
                           select specificCulture).FirstOrDefault();

            if (culture == null)
            {
                return Value.ToString("0.00");
            }

            return string.Format(culture, "{0:C}", Value);
        }
    }
}
