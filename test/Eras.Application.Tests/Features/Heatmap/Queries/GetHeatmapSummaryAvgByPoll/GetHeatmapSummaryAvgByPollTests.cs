﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters;
using Eras.Application.Models.Response.HeatMap;

using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Heatmap.Queries.GetHeatmapSummaryAvgByPoll;
public class GetHeatMapSummaryAvgByPollTests
{
    private readonly Mock<IHeatMapRepository> _mockHeatMapRepository;
    private readonly Mock<ILogger<GetHeatMapSummaryHandler>> _mockLogger;
    private readonly GetHeatMapSummaryHandler _handler;

    public GetHeatMapSummaryAvgByPollTests()
    {
        // Inicialización de los mocks
        _mockHeatMapRepository = new Mock<IHeatMapRepository>();
        _mockLogger = new Mock<ILogger<GetHeatMapSummaryHandler>>();

        // Inicialización del handler con los mocks
        _handler = new GetHeatMapSummaryHandler(_mockHeatMapRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task HandleShouldReturnSuccessResponseWhenAverageDataIsFoundAsync()
    {        
        var query = new GetHeatMapSummaryQuery("3dbce0ff-21a1-484c-ab1c-e2e4fe78eca5");

        _mockHeatMapRepository.Setup(Repo => Repo.GetHeatMapAnswersPercentageByVariableAsync(query.PollInstanceUUID))
            .ReturnsAsync(new List<GetHeatMapAnswersPercentageByVariableQueryResponse>
            {
                new GetHeatMapAnswersPercentageByVariableQueryResponse { 
                    ComponentName = "Component1",
                    PollVariableId = 1,
                    Name = "student name",
                    AnswerText = "Name",
                    VariableAverageRisk = 15,
                    Percentage = 1
                }
            });

        var result = await _handler.Handle(query, CancellationToken.None);
        
        Assert.True(result.Success);
        Assert.NotNull(result.Body);
        Assert.NotEmpty(result.Body.Components);

        _mockHeatMapRepository.Verify(Repo => Repo.GetHeatMapAnswersPercentageByVariableAsync(query.PollInstanceUUID), Times.Once);
    }
}

