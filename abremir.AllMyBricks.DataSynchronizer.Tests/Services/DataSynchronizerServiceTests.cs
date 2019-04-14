﻿using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Services;
using abremir.AllMyBricks.Onboarding.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstituteAutoMocker.Standard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Services
{
    [TestClass]
    public class DataSynchronizerServiceTests
    {
        private NSubstituteAutoMocker<SetSynchronizationService> _dataSynchronizationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _dataSynchronizationService = new NSubstituteAutoMocker<SetSynchronizationService>();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task SynchronizeAllSetData_InvalidApiKey_GetDataSynchronizationTimestampNotInvoked(string apiKey)
        {
            _dataSynchronizationService.Get<IOnboardingService>()
                .GetBricksetApiKey()
                .Returns(apiKey);

            await _dataSynchronizationService.ClassUnderTest.SynchronizeAllSets();

            _dataSynchronizationService.Get<IInsightsRepository>().DidNotReceive().GetDataSynchronizationTimestamp();
        }

        [TestMethod]
        public async Task SynchronizeAllSetData_WithoutDataSynchronizationTimestamp_SynchronizeAllSetsAndUpdateDataSynchronizationTimestampInvoked()
        {
            _dataSynchronizationService.Get<IOnboardingService>()
                .GetBricksetApiKey()
                .Returns("APIKEY");
            _dataSynchronizationService.Get<IInsightsRepository>()
                .GetDataSynchronizationTimestamp().
                Returns((DateTimeOffset?)null);
            _dataSynchronizationService.Get<IThemeSynchronizer>()
                .Synchronize(Arg.Any<string>())
                .Returns(new List<Theme> { new Theme() });
            _dataSynchronizationService.Get<ISubthemeSynchronizer>()
                .Synchronize(Arg.Any<string>(), Arg.Any<Theme>())
                .Returns(new List<Subtheme> { new Subtheme() });

            await _dataSynchronizationService.ClassUnderTest.SynchronizeAllSets();

            await _dataSynchronizationService.Get<IThemeSynchronizer>().Received(1).Synchronize(Arg.Any<string>());
            await _dataSynchronizationService.Get<ISubthemeSynchronizer>().Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>());
            await _dataSynchronizationService.Get<ISetSynchronizer>().Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>(), Arg.Any<Subtheme>());
            await _dataSynchronizationService.Get<ISetSynchronizer>().DidNotReceive().Synchronize(Arg.Any<string>(), Arg.Any<DateTimeOffset>());
            _dataSynchronizationService.Get<IInsightsRepository>().Received(1).UpdateDataSynchronizationTimestamp(Arg.Any<DateTimeOffset>());
        }

        [TestMethod]
        public async Task SynchronizeAllSetData_WithDataSynchronizationTimestamp_SynchronizeRecentlyUpdatedSetsAndUpdateDataSynchronizationTimestampInvoked()
        {
            _dataSynchronizationService.Get<IOnboardingService>()
                .GetBricksetApiKey()
                .Returns("APIKEY");
            _dataSynchronizationService.Get<IInsightsRepository>()
                .GetDataSynchronizationTimestamp()
                .Returns(DateTimeOffset.Now);
            _dataSynchronizationService.Get<IThemeSynchronizer>()
                .Synchronize(Arg.Any<string>())
                .Returns(new List<Theme> { new Theme() });
            _dataSynchronizationService.Get<ISubthemeSynchronizer>()
                .Synchronize(Arg.Any<string>(), Arg.Any<Theme>())
                .Returns(new List<Subtheme> { new Subtheme() });

            await _dataSynchronizationService.ClassUnderTest.SynchronizeAllSets();

            await _dataSynchronizationService.Get<IThemeSynchronizer>().Received(1).Synchronize(Arg.Any<string>());
            await _dataSynchronizationService.Get<ISubthemeSynchronizer>().Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>());
            await _dataSynchronizationService.Get<ISetSynchronizer>().DidNotReceive().Synchronize(Arg.Any<string>(), Arg.Any<Theme>(), Arg.Any<Subtheme>());
            await _dataSynchronizationService.Get<ISetSynchronizer>().Received(1).Synchronize(Arg.Any<string>(), Arg.Any<DateTimeOffset>());
            _dataSynchronizationService.Get<IInsightsRepository>().Received(1).UpdateDataSynchronizationTimestamp(Arg.Any<DateTimeOffset>());
        }
    }
}