using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/rack-dashboard")]
[ApiController]
public class RackDashboardController : HahaController
{
    private readonly IRackDashboardRepository _rackDashboardRepository;

    public RackDashboardController(IRackDashboardRepository rackDashboardRepository)
    {
        _rackDashboardRepository = rackDashboardRepository;
    }

    /// <summary>
    /// Mengambil daftar warehouse beserta jumlah lokasi dan stok rak
    /// </summary>
    [HttpGet("warehouses")]
    public async Task<IActionResult> GetWarehouses()
    {
        try
        {
            var warehouses = await _rackDashboardRepository.GetWarehouses();
            return Success(warehouses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
    }

    /// <summary>
    /// Mengambil daftar area berdasarkan kode warehouse
    /// </summary>
    [HttpGet("areas")]
    public async Task<IActionResult> GetAreas([FromQuery] string? warehouse)
    {
        try
        {
            var areas = await _rackDashboardRepository.GetAreas(warehouse);
            return Success(areas);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
    }

    /// <summary>
    /// Mengambil data grid lokasi rak dengan filter warehouse, area, status, pencarian, dan item
    /// </summary>
    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations(
        [FromQuery] string? warehouse,
        [FromQuery] string? area,
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] string? item)
    {
        try
        {
            var locations = await _rackDashboardRepository.GetLocations(warehouse, area, status, search, item);
            return Success(locations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
    }

    /// <summary>
    /// Mengambil detail stok barang pada satu slot address rak
    /// </summary>
    [HttpGet("detail")]
    public async Task<IActionResult> GetDetail(
        [FromQuery] string warehouse,
        [FromQuery] string area,
        [FromQuery] string address)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(warehouse) || string.IsNullOrWhiteSpace(area) || string.IsNullOrWhiteSpace(address))
            {
                return Invalid("warehouse, area, dan address wajib diisi.");
            }

            var detail = await _rackDashboardRepository.GetAddressDetail(warehouse, area, address);
            if (detail == null)
            {
                return NotFound(new { Status = "NotFound", Message = "Lokasi rak tidak ditemukan." });
            }

            return Success(detail);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
    }

    /// <summary>
    /// Menghitung ringkasan KPI dan tingkat okupansi rak
    /// </summary>
    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis(
        [FromQuery] string? warehouse,
        [FromQuery] string? area,
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] string? item)
    {
        try
        {
            var kpis = await _rackDashboardRepository.GetKpi(warehouse, area, status, search, item);
            return Success(kpis);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
    }
}
