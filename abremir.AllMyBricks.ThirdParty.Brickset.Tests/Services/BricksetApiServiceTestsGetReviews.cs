﻿using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderGetReviews)]
    public class BricksetApiServiceTestsGetReviews : BricksetApiServiceTestsBase
    {
        [TestMethod]
        public void Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var service = new BricksetApiService();

            var result = service.GetReviews(new ParameterSetId());

            result.Should().NotBeEmpty();
        }

        [TestMethod]
        public void NoReviews()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoReviews)));

            var service = new BricksetApiService();

            var result = service.GetReviews(new ParameterSetId());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidSetId()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidSetId)));

            var service = new BricksetApiService();

            var result = service.GetReviews(new ParameterSetId());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var service = new BricksetApiService();

            var result = service.GetReviews(new ParameterSetId());

            result.Should().BeEmpty();
        }
    }
}