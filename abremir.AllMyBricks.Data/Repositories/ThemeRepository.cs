﻿using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        public ThemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Theme AddOrUpdateTheme(Theme theme)
        {
            if(theme == null
                || string.IsNullOrWhiteSpace(theme.Name)
                || theme.YearFrom < Constants.MinimumSetYear)
            {
                return null;
            }

            var existingTheme = GetTheme(theme.Name);

            using (var repository = _repositoryService.GetRepository())
            {
                if(existingTheme == null)
                {
                    repository.Insert(theme);
                }
                else
                {
                    repository.Update(theme);
                }
            }

            return theme;
        }

        public IEnumerable<Theme> GetAllThemes()
        {
            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<Theme>()
                    .ToEnumerable();
            }
        }

        public IEnumerable<Theme> GetAllThemesForYear(ushort year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return new Theme[] { };
            }

            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<Theme>()
                    .Where(theme => theme.YearFrom <= year
                        && year <= theme.YearTo)
                    .ToEnumerable();
            }
        }

        public Theme GetTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return null;
            }

            using(var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<Theme>()
                    .Where(theme => theme.Name.Equals(themeName, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();
            }
        }
    }
}