using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Services.Core.Controllers
{
    using Logic.DataAccess.TableStorage;

    [ApiController]
    [Route("[controller]")]
    public class TelemetryController : ControllerBase
    {

        private readonly ILogger<TelemetryController> _logger;

        private readonly ITableStorageAdapter<TelemeryTableEntity> _adapter;

        public TelemetryController(ILogger<TelemetryController> logger, ITableStorageAdapter<TelemeryTableEntity> adapter)
        {
            _logger = logger;
            _adapter = adapter;
        }

        [HttpGet]
        public async Task<IEnumerable<TelemeryTableEntity>> Get()
        {
            var result = await _adapter.GetAllAsync();
            return result;
        }
    }
}
