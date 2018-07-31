﻿using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using ExpressMapper.Extensions;
using Realms;
using System.Collections.Generic;
using System.Linq;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class SetRepository : ISetRepository
    {
        private readonly IRepositoryService _repositoryService;

        private IEnumerable<Set> EmptyEnumerable => new Set[] { };

        public SetRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Set AddOrUpdate(Set set)
        {
            if (set == null
                || set.SetId == 0)
            {
                return null;
            }

            var existingSet = Get(set.SetId);

            var repository = _repositoryService.GetRepository();

            var managedSet = GetManagedSet(set);

            repository.Write(() => repository.Add(managedSet, existingSet != null));

            return managedSet.Map<Managed.Set, Set>();
        }

        private Managed.Set GetManagedSet(Set set)
        {
            var managedSet = set.Map<Set, Managed.Set>();

            var repository = _repositoryService.GetRepository();

            managedSet.Theme = set.Theme == null
                ? null
                : repository.All<Managed.Theme>()
                    .Filter($"Name ==[c] '{set.Theme.Name}'")
                    .FirstOrDefault();
            managedSet.Subtheme = set.Subtheme == null
                ? null
                : repository.All<Managed.Subtheme>()
                    .Filter($"Name ==[c] '{set.Subtheme.Name}' && Theme.Name ==[c] '{set.Theme.Name}'")
                    .FirstOrDefault();
            managedSet.ThemeGroup = set.ThemeGroup == null
                ? null
                : repository.All<Managed.ThemeGroup>()
                    .Filter($"Value ==[c] '{set.ThemeGroup.Value}'")
                    .FirstOrDefault();
            managedSet.Category = set.Category == null
                ? null
                : repository.All<Managed.Category>()
                    .Filter($"Value ==[c] '{set.Category.Value}'")
                    .FirstOrDefault();
            managedSet.PackagingType = set.PackagingType == null
                ? null
                : repository.All<Managed.PackagingType>()
                    .Filter($"Value ==[c] '{set.PackagingType.Value}'")
                    .FirstOrDefault();

            managedSet.Tags.Clear();

            foreach (var tag in (set.Tags ?? new List<Tag>())
                .Where(tag => tag != null))
            {
                var managedTag = repository.All<Managed.Tag>()
                        .Filter($"Value ==[c] '{tag.Value}'")
                        .FirstOrDefault();
                if (managedTag != null)
                {
                    managedSet.Tags.Add(managedTag);
                }
            }

            return managedSet;
        }

        public IEnumerable<Set> All()
        {
            return GetQueryable().Map<IQueryable<Managed.Set>, IEnumerable<Set>>();
        }

        public Set Get(long setId)
        {
            if (setId == 0)
            {
                return null;
            }

            return GetQueryable()
                .FirstOrDefault(set => set.SetId == setId)
                ?.Map<Managed.Set, Set>();
        }

        public IEnumerable<Set> AllForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"Theme.Name ==[c] '{themeName}'")
                .Map<IQueryable<Managed.Set>, IEnumerable<Set>>();
        }

        public IEnumerable<Set> AllForSubtheme(string themeName, string subthemeName)
        {
            if (string.IsNullOrWhiteSpace(themeName) || string.IsNullOrWhiteSpace(subthemeName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"Theme.Name ==[c] '{themeName}' && Subtheme.Name ==[c] '{subthemeName}'")
                .Map<IQueryable<Managed.Set>, IEnumerable<Set>>();
        }

        public IEnumerable<Set> AllForThemeGroup(string themeGroupName)
        {
            if (string.IsNullOrWhiteSpace(themeGroupName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"ThemeGroup.Value ==[c] '{themeGroupName}'")
                .Map<IQueryable<Managed.Set>, IEnumerable<Set>>();
        }

        public IEnumerable<Set> AllForCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"Category.Value ==[c] '{categoryName}'")
                .Map<IQueryable<Managed.Set>, IEnumerable<Set>>();

        }

        public IEnumerable<Set> AllForTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"Tags.Value ==[c] '{tagName}'")
                .Map<IQueryable<Managed.Set>, IEnumerable<Set>>();
        }

        public IEnumerable<Set> AllForYear(short year)
        {
            if (year < Constants.MinimumSetYear)
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Where(set => set.Year == year)
                .Map<IQueryable<Managed.Set>, IEnumerable<Set>>();
        }

        public IEnumerable<Set> AllForPriceRange(PriceRegionEnum priceRegion, float minimumPrice, float maximumPrice)
        {
            if (minimumPrice < 0 || maximumPrice < 0)
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"Prices.RegionRaw == {(int)priceRegion} && Prices.Value >= {minimumPrice} && Prices.Value <= {maximumPrice}")
                .Map<IQueryable<Managed.Set>, IEnumerable<Set>>();
        }

        public IEnumerable<Set> SearchBy(string searchQuery)
        {
            var realmQuery = BuildRealmQueryFromSearchQuery(searchQuery);

            if (realmQuery == null)
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter(realmQuery)
                .Map<IQueryable<Managed.Set>, IEnumerable<Set>>();
        }

        private string BuildRealmQueryFromSearchQuery(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return null;
            }

            var queryList = new List<string>();

            foreach (var searchTerm in searchQuery
                .Split(' ', '-')
                .Where(term => (term?.Trim().Length ?? 0) >= Constants.MinimumSearchQuerySize)
                .Distinct())
            {
                queryList.Add($"Number CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"Name CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"Ean CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"Upc CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"Description CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"Theme.Name CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"Subtheme.Name CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"ThemeGroup.Value CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"Category.Value CONTAINS[c] '{searchTerm.Trim()}'");
                queryList.Add($"Tags.Value CONTAINS[c] '{searchTerm.Trim()}'");
            }

            return queryList.Count == 0
                ? null
                : string.Join(" OR ", queryList.ToArray());
        }

        private IQueryable<Managed.Set> GetQueryable()
        {
            return _repositoryService
                .GetRepository()
                .All<Managed.Set>();
        }
    }
}