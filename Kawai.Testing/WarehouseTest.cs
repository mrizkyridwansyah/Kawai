using Shouldly;
using System.Net;
using Kawai.Testing.Factory;
using Kawai.Domain.Shared;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Kawai.Testing;

public class WarehouseTest : BaseIntegrationTest
{
    public WarehouseTest(AuthenticatedFactory authFactory, UnauthenticatedFactory noAuthFactory) : base(authFactory, noAuthFactory)
    {
    }

    [Fact]
    public async Task Warehouse_Unauthorized_Test()
    {
        var parameter = new RequestParameter
        {
            Page = 1,
            Length = 10,
            Sorts = new()
            {
                { "WH_Name", "asc" }
            },
            Filters = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "Keyword", "" },
                    { "FactoryCode", "00000" },
                }
            }
        };

        // Serialize ke JSON
        var json = JsonSerializer.Serialize(parameter);

        // Buat content untuk request
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await ClientNoAuth.PostAsync("/api/warehouse/list", content);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Warehouse_List_Test()
    {
        var parameter = new RequestParameter
        {
            Page = 1,
            Length = 10,
            Sorts = new()
            {
                { "WH_Name", "asc" }
            },
            Filters = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "Keyword", "" },
                    { "FactoryCode", "00000" },
                }
            }
        };

        // Serialize ke JSON
        var json = JsonSerializer.Serialize(parameter);

        // Buat content untuk request
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await Client.PostAsync("/api/warehouse/list", content);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Deserialize minimal DTO
        var result = JsonSerializer.Deserialize<WarehouseListResponse>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.ShouldNotBeNull();
        result!.Data.ShouldNotBeNull();
        result.Data!.Items.ShouldNotBeNull();
        result.Data.Items!.Count.ShouldBeGreaterThan(0); // <-- assert: Items length > 0

    }

    [Fact]
    public async Task Warehouse_ListEmpty_Test()
    {
        var parameter = new RequestParameter
        {
            Page = 1,
            Length = 10,
            Sorts = new()
            {
                { "WH_Name", "asc" }
            },
            Filters = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    { "Keyword", "" },
                    { "FactoryCode", "-" },
                }
            }
        };

        // Serialize ke JSON
        var json = JsonSerializer.Serialize(parameter);

        // Buat content untuk request
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await Client.PostAsync("/api/warehouse/list", content);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Deserialize minimal DTO
        var result = JsonSerializer.Deserialize<WarehouseListResponse>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.ShouldNotBeNull();
        result!.Data.ShouldNotBeNull();
        result.Data!.Items.ShouldNotBeNull();
        result.Data.Items!.Count.ShouldBe(0);

    }
}

public class WarehouseListResponse
{
    public ResponseDto<WarehouseListDto>? Data { get; set; }
}

public class WarehouseListDto
{
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
}