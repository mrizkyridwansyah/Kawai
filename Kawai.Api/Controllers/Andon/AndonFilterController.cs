using Kawai.Data.Repositories;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Andon;

[Route("api/andon/filter")]
[ApiController]
public class AndonFilterController : HahaController
{
    private readonly IAndonFilterRepository _andonFilter;
    private readonly IWorkStationSettingRepository _workstationsettingRepository;

    public AndonFilterController(IAndonFilterRepository repo, IWorkStationSettingRepository workstationsettingRepository)
    {
        _andonFilter = repo;
        _workstationsettingRepository = workstationsettingRepository;   
    }

    //get ddl area
    [HttpGet("ddlarea")]
    public async Task<IActionResult> DDLArea(string keyword, string ids, string warehouseCode)
    {
        var results = await _andonFilter.DDLArea(keyword, warehouseCode);

        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.AreaCode)).ToList();
        }
        return Success(results);
    }

    [HttpGet("ddl-linecompany-search")]
    public async Task<IActionResult> DDLLineSearch(string keyword, string companyCode, string manufacture, string ids)
    {
        var results = await _workstationsettingRepository.GetLineCompanyDDL(keyword, companyCode, manufacture);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }

    //get ddl area
    [HttpGet("ddlline")]
    public async Task<IActionResult> DDLLine(string keyword, string ids)
    {
        var results = await _andonFilter.DDLLine(keyword);

        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }
        return Success(results);
    }
    //get ddl area
    [HttpGet("ddlmodel")]
    public async Task<IActionResult> DDLModel(string keyword, string ids)
    {
        var results = await _andonFilter.DDLModel(keyword);

        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ModelCls)).ToList();
        }
        return Success(results);
    }

    //get ddl area
    [HttpGet("ddlsupplier")]
    public async Task<IActionResult> DDLSupplier(string keyword, string ids)
    {
        var results = await _andonFilter.DDLSupplier(keyword);

        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.SupplierCode)).ToList();
        }
        return Success(results);
    }



}
