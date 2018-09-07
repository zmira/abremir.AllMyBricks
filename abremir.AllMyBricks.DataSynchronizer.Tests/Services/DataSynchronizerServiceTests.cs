﻿using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Services;
using abremir.AllMyBricks.Onboarding.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Services
{
    [TestClass]
    public class DataSynchronizerServiceTests
    {
        private DataSynchronizationService _dataSynchronizationService;
        private IThemeSynchronizer _themeSynchronizer;
        private ISubthemeSynchronizer _subthemeSynchronizer;
        private ISetSynchronizer _setSynchronizer;
        private IInsightsRepository _insightsRepository;
        private IOnboardingService _onboardingService;

        [TestInitialize]
        public void TestInitialize()
        {
            _themeSynchronizer = Substitute.For<IThemeSynchronizer>();
            _subthemeSynchronizer = Substitute.For<ISubthemeSynchronizer>();
            _setSynchronizer = Substitute.For<ISetSynchronizer>();
            _insightsRepository = Substitute.For<IInsightsRepository>();
            _onboardingService = Substitute.For<IOnboardingService>();

            _dataSynchronizationService = new DataSynchronizationService(_themeSynchronizer, _subthemeSynchronizer, _setSynchronizer, _insightsRepository, _onboardingService);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void SynchronizeAllSetData_InvalidApiKey_GetDataSynchronizationTimestampNotInvoked(string apiKey)
        {
            _onboardingService.GetBricksetApiKey().Returns(apiKey);

            _dataSynchronizationService.SynchronizeAllSetData();

            _insightsRepository.DidNotReceive().GetDataSynchronizationTimestamp();
        }

        [TestMethod]
        public void SynchronizeAllSetData_WithoutDataSynchronizationTimestamp_SynchronizeAllSetsAndUpdateDataSynchronizationTimestampInvoked()
        {
            _onboardingService.GetBricksetApiKey().Returns("APIKEY");
            _insightsRepository.GetDataSynchronizationTimestamp().Returns((DateTimeOffset?)null);
            _themeSynchronizer.Synchronize(Arg.Any<string>()).Returns(new List<Theme> { new Theme() });
            _subthemeSynchronizer.Synchronize(Arg.Any<string>(), Arg.Any<Theme>()).Returns(new List<Subtheme> { new Subtheme() });

            _dataSynchronizationService.SynchronizeAllSetData();

            _themeSynchronizer.Received(1).Synchronize(Arg.Any<string>());
            _subthemeSynchronizer.Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>());
            _setSynchronizer.Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>(), Arg.Any<Subtheme>());
            _setSynchronizer.DidNotReceive().Synchronize(Arg.Any<string>(), Arg.Any<DateTimeOffset>());
            _insightsRepository.Received(1).UpdateDataSynchronizationTimestamp(Arg.Any<DateTimeOffset>());
        }

        [TestMethod]
        public void SynchronizeAllSetData_WithDataSynchronizationTimestamp_SynchronizeRecentlyUpdatedSetsAndUpdateDataSynchronizationTimestampInvoked()
        {
            _onboardingService.GetBricksetApiKey().Returns("APIKEY");
            _insightsRepository.GetDataSynchronizationTimestamp().Returns(DateTimeOffset.Now);
            _themeSynchronizer.Synchronize(Arg.Any<string>()).Returns(new List<Theme> { new Theme() });
            _subthemeSynchronizer.Synchronize(Arg.Any<string>(), Arg.Any<Theme>()).Returns(new List<Subtheme> { new Subtheme() });

            _dataSynchronizationService.SynchronizeAllSetData();

            _themeSynchronizer.Received(1).Synchronize(Arg.Any<string>());
            _subthemeSynchronizer.Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>());
            _setSynchronizer.DidNotReceive().Synchronize(Arg.Any<string>(), Arg.Any<Theme>(), Arg.Any<Subtheme>());
            _setSynchronizer.Received(1).Synchronize(Arg.Any<string>(), Arg.Any<DateTimeOffset>());
            _insightsRepository.Received(1).UpdateDataSynchronizationTimestamp(Arg.Any<DateTimeOffset>());
        }
    }
}