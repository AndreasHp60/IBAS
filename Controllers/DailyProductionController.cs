using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure;
using DailyProduction.Model;
 
namespace IbasAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyProductionController : ControllerBase
    {
        private List<DailyProductionDTO> _productionRepo;
        private readonly ILogger<DailyProductionController> _logger;
 
        private TableClient _tableClient;
 
        public DailyProductionController(ILogger<DailyProductionController> logger)
        {
            _logger = logger;
            var serviceUri = "https://csb10032000f00e7492.table.core.windows.net/";
            var tableName = "IBASProduction2020";
 
            var accountName = "csb10032000f00e7492";
            var storageAccountKey = "6hrOCg3N0XjI4CACq/NgOl2FZHnb9elmIeU0NINsCxv36SXB3BAZE/VPptn0A0o1zs264ypN73y1RsFHir0BDQ==";
 
            this._tableClient = new TableClient(
                new Uri(serviceUri),
                tableName,
                new TableSharedKeyCredential(accountName, storageAccountKey));
        }
        
        [HttpGet]
        public IEnumerable<DailyProductionDTO> Get()
        {
            var production = new List<DailyProductionDTO>();
            Pageable<TableEntity> entities = this._tableClient.Query<TableEntity>();
 
            foreach (TableEntity entity in entities)
            {
                var dto = new DailyProductionDTO {
                    Date = DateTime.Parse(entity.RowKey),
                    Model = (BikeModel)Enum.ToObject(typeof(BikeModel), Int32.Parse(entity.PartitionKey)),
                    ItemsProduced = (int)entity.GetInt32("itemsProduced")
                };
                production.Add(dto);
            }
 
            return production;
        }
    }
}