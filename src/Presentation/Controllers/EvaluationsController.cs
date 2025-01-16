﻿using ERAS.Application.Services;
using ERAS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ERAS.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly ICosmicLatteAPIService _cosmicLatteService;

        public EvaluationsController(ICosmicLatteAPIService cosmicLatteService)
        {
            _cosmicLatteService = cosmicLatteService;
        }

        [HttpOptions("/cosmic-latte/status")]
        public async Task<ActionResult<CosmicLatteStatus>> CosmicApiIsHealthy()
        {
            return Ok(await _cosmicLatteService.CosmicApiIsHealthy());
        }
    }
}

