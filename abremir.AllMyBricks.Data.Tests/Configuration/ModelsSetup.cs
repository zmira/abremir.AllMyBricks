﻿using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Tests.Configuration
{
    public static class ModelsSetup
    {
        public const string StringEmpty = "";
        public const string ThemeUnderTestName = "test theme";
        public const string SubthemeUnderTestName = "test subtheme";
        public const string NonExistentThemeName = "unknown theme";
        public const string NonExistentSubthemeName = "unknown subtheme";
        public const ushort FirstThemeYearFrom = 1990;
        public const ushort FirstThemeYearTo = 2019;
        public const ushort SecondThemeYearTo = 2020;
        public const ushort FirstSubthemeYearFrom = FirstThemeYearFrom + 10;
        public const ushort SecondSubthemeYearFrom = FirstThemeYearFrom + 11;
        public const string CategoryReferenceDataValue = "New Category";
        public const string PackagingTypeReferenceDataValue = "New Packaging Type";
        public const string TagReferenceDataValue = "New Tag";
        public const string ThemeGroupReferenceDataValue = "New Theme Group";

        public static Theme ThemeUnderTest => new Theme
        {
            Name = ThemeUnderTestName,
            SetCount = 9,
            SubthemeCount = 99,
            YearFrom = FirstThemeYearFrom,
            YearTo = FirstThemeYearTo
        };

        public static Theme[] ListOfThemesUnderTest => new[]
        {
            ThemeUnderTest,
            new Theme
            {
                Name = "test theme 2",
                SetCount = 10,
                SubthemeCount = 100,
                YearFrom = 1991,
                YearTo = SecondThemeYearTo
            }
        };

        public static Subtheme SubthemeUnderTest => new Subtheme
        {
            Name = SubthemeUnderTestName,
            SetCount = 8,
            Theme = ThemeUnderTest,
            YearFrom = FirstSubthemeYearFrom,
            YearTo = ThemeUnderTest.YearTo
        };

        public static Subtheme[] ListOfSubthemesUnderTest => new[]
        {
            SubthemeUnderTest,
            new Subtheme
            {
                Name = "test subtheme 2",
                SetCount = 7,
                Theme = ThemeUnderTest,
                YearFrom = SecondSubthemeYearFrom,
                YearTo = ThemeUnderTest.YearTo
            }
        };

        public static Category CategoryReferenceData => new Category
        {
            Value = CategoryReferenceDataValue
        };

        public static ThemeGroup ThemeGroupReferenceData => new ThemeGroup
        {
            Value = ThemeGroupReferenceDataValue
        };

        public static Tag TagReferenceData => new Tag
        {
            Value = TagReferenceDataValue
        };

        public static PackagingType PackagingTypeReferenceData => new PackagingType
        {
            Value = PackagingTypeReferenceDataValue
        };

        public static ThemeYearCount ThemeYearCountUnderTest => new ThemeYearCount
        {
            Key = new ThemeYear
            {
                Theme = ThemeUnderTest,
                Year = ThemeUnderTest.YearFrom
            },
            Count = 55
        };

        public static ThemeYearCount[] ListOfThemeYearCountUnderTest => new[]
        {
            ThemeYearCountUnderTest,
            new ThemeYearCount
            {
                Key = new ThemeYear
                {
                    Theme = ListOfThemesUnderTest[1],
                    Year = ListOfThemesUnderTest[1].YearFrom
                },
                Count = 11
            }
        };
    }
}