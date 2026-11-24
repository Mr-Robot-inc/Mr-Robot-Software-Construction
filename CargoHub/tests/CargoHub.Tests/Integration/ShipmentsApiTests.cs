// using System.Net;
// using System.Net.Http.Json;
// using CargoHub.Api.Data;
// using CargoHub.Api.Dtos;
// using CargoHub.Api.Models;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;

// namespace CargoHub.Tests.Integration;

// public class ShipmentsApiTests : IClassFixture<CargohubApiFactory>
// {
//     private readonly CargohubApiFactory _factory;
//     private readonly HttpClient _client;

//     public ShipmentsApiTests(CargohubApiFactory factory)
//     {
//         _factory = factory;
//         _client = factory.CreateClient();
//     }

//     private async Task<int> SeedOrderAsync()
//     {
//         using var scope = _factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         return await TestSeed.OrderAsync(db);
//     }

//     private static CreateShipmentDto ValidShipment(int orderId) => new()
//     {
//         Reference = "SHIP-TEST",
//         OrderId = orderId,
//         ShipmentDate = new DateTime(2026, 11, 12, 0, 0, 0, DateTimeKind.Utc),
//         ShipmentType = "Fast",
//         ShipmentStatus = "Pending",
//         CarrierName = "VintedGo",
//         ShippingMethod = "Air",
//         PaymentType = "Manual"
//     };

//     private static UpdateShipmentDto ValidUpdate(int orderId) => new()
//     {
//         Reference = "SHIP-UPDATED",
//         OrderId = orderId,
//         ShipmentDate = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc),
//         ShipmentType = "Slow",
//         ShipmentStatus = "Shipped",
//         CarrierName = "TinkerBell",
//         ShippingMethod = "Sea",
//         PaymentType = "Automatic"
//     };

//     private async Task<ShipmentDto> CreateShipmentAsync(int? orderId = null)
//     {
//         var oid = orderId ?? await SeedOrderAsync();
//         var post = await _client.PostAsJsonAsync("/api/shipments", ValidShipment(oid));
//         Assert.Equal(HttpStatusCode.Created, post.StatusCode);
//         var created = await post.Content.ReadFromJsonAsync<ShipmentDto>();
//         Assert.NotNull(created);
//         return created;
//     }

//     private async Task<int> SeedInventoryAsync(int quantityOnHand, int quantityOrdered)
//     {
//         using var scope = _factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//         var item = new Item
//         {
//             Code = "P01",
//             Description = "test",
//             BarCode = "9812743897124",
//             ModeNumber = "PJ-123",
//             CommodityCode = 1234,
//             UnitWeight = 12.5f,
//             MinPurchaseQty = 1,
//             CaseSize = 10,
//             PackagingType = "Box",
//             OrderMultiple = 1,
//             SupplierSku = "SUP-01",
//             SupplierId = 1,
//             ItemGroup = new ItemGroup { Name = "Tools", Description = "Hand tools", ItemGroupcol = "p-231" },
//             ItemLine = new ItemLine { Name = "line", Description = "line descr" },
//             ItemType = new ItemType { Name = "type", Description = "type descr" }
//         };
//         var location = new Location
//         {
//             Code = "LOC1",
//             Name = "Location 1",
//             Warehouse = new Warehouse
//             {
//                 Code = "WH1",
//                 Name = "Warehouse 1",
//                 Address = "Street 1",
//                 City = "City",
//                 ZipCode = "1234AB",
//                 Province = "Province",
//                 Country = "Country",
//                 ContactName = "Bob",
//                 ContactPhone = "0612345678",
//                 ContactEmail = "bob@example.com"
//             }
//         };
//         db.Add(item);
//         db.Add(location);
//         await db.SaveChangesAsync();

//         db.Add(new Inventory
//         {
//             ItemId = item.Id,
//             LocationId = location.Id,
//             QuantityOnHand = quantityOnHand,
//             QuantityOrdered = quantityOrdered
//         });
//         await db.SaveChangesAsync();

//         return item.Id;
//     }

//     private async Task<int> GetQuantityOrderedAsync(int itemId)
//     {
//         using var scope = _factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         return (await db.Inventories.AsNoTracking().SingleAsync(i => i.ItemId == itemId)).QuantityOrdered;
//     }

//     [Fact]
//     public async Task Create_ValidShipment_Returns201_AndCanBeRetrieved()
//     {
//         var orderId = await SeedOrderAsync();

//         var post = await _client.PostAsJsonAsync("/api/shipments", ValidShipment(orderId));
//         Assert.Equal(HttpStatusCode.Created, post.StatusCode);

//         var created = await post.Content.ReadFromJsonAsync<ShipmentDto>();
//         Assert.NotNull(created);
//         Assert.True(created.Id > 0);
//         Assert.Equal("SHIP-TEST", created.Reference);
//         Assert.Equal(orderId, created.OrderId);
//         Assert.NotEqual(default, created.CreatedAt);
//         Assert.NotEqual(default, created.UpdatedAt);

//         var get = await _client.GetAsync($"/api/shipments/{created.Id}");
//         Assert.Equal(HttpStatusCode.OK, get.StatusCode);
//         var fetched = await get.Content.ReadFromJsonAsync<ShipmentDto>();
//         Assert.Equal("VintedGo", fetched!.CarrierName);
//     }

//     [Fact]
//     public async Task Create_UnknownOrder_Returns400_AndNothingIsSaved()
//     {
//         var post = await _client.PostAsJsonAsync("/api/shipments", ValidShipment(999999) with { Reference = "SHIP-NO-ORDER" });

//         Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);

//         using var scope = _factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         Assert.False(await db.Shipments.AnyAsync(s => s.Reference == "SHIP-NO-ORDER"));
//     }

//     [Fact]
//     public async Task Create_MissingReference_Returns400()
//     {
//         var dto = new
//         {
//             OrderId = 1,
//             ShipmentDate = new DateTime(2026, 11, 12, 0, 0, 0, DateTimeKind.Utc),
//             ShipmentType = "Fast",
//             ShipmentStatus = "Pending",
//             CarrierName = "VintedGo",
//             ShippingMethod = "Air",
//             PaymentType = "Manual"
//         };

//         var post = await _client.PostAsJsonAsync("/api/shipments", dto);

//         Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
//     }

//     [Fact]
//     public async Task Create_OrderIdZero_Returns400()
//     {
//         var post = await _client.PostAsJsonAsync("/api/shipments", ValidShipment(0));

//         Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
//     }

//     [Fact]
//     public async Task GetAll_ContainsCreatedShipment()
//     {
//         var created = await CreateShipmentAsync();

//         var list = await _client.GetFromJsonAsync<List<ShipmentDto>>("/api/shipments");

//         Assert.NotNull(list);
//         Assert.Contains(list, s => s.Id == created.Id);
//     }

//     [Fact]
//     public async Task GetById_UnknownId_Returns404()
//     {
//         var response = await _client.GetAsync("/api/shipments/999999");

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task GetOrderIds_Existing_ReturnsOrderId()
//     {
//         var orderId = await SeedOrderAsync();
//         var created = await CreateShipmentAsync(orderId);

//         var response = await _client.GetAsync($"/api/shipments/{created.Id}/orders");

//         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//         var ids = await response.Content.ReadFromJsonAsync<List<int>>();
//         Assert.Equal(new List<int> { orderId }, ids);
//     }

//     [Fact]
//     public async Task GetOrderIds_UnknownShipment_Returns404()
//     {
//         var response = await _client.GetAsync("/api/shipments/999999/orders");

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task Update_Existing_Returns200_AndDataChanged()
//     {
//         var created = await CreateShipmentAsync();
//         var otherOrderId = await SeedOrderAsync();

//         var put = await _client.PutAsJsonAsync($"/api/shipments/{created.Id}", ValidUpdate(otherOrderId));

//         Assert.Equal(HttpStatusCode.OK, put.StatusCode);
//         var updated = await put.Content.ReadFromJsonAsync<ShipmentDto>();
//         Assert.Equal("SHIP-UPDATED", updated!.Reference);
//         Assert.Equal(otherOrderId, updated.OrderId);

//         var fetched = await _client.GetFromJsonAsync<ShipmentDto>($"/api/shipments/{created.Id}");
//         Assert.Equal("TinkerBell", fetched!.CarrierName);
//         Assert.Equal("Shipped", fetched.ShipmentStatus);
//     }

//     [Fact]
//     public async Task Update_UnknownId_Returns404()
//     {
//         var orderId = await SeedOrderAsync();

//         var put = await _client.PutAsJsonAsync("/api/shipments/999999", ValidUpdate(orderId));

//         Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
//     }

//     [Fact]
//     public async Task Update_UnknownOrder_Returns400_AndShipmentIsUnchanged()
//     {
//         var created = await CreateShipmentAsync();

//         var put = await _client.PutAsJsonAsync($"/api/shipments/{created.Id}", ValidUpdate(999999));

//         Assert.Equal(HttpStatusCode.BadRequest, put.StatusCode);
//         var fetched = await _client.GetFromJsonAsync<ShipmentDto>($"/api/shipments/{created.Id}");
//         Assert.Equal("SHIP-TEST", fetched!.Reference);
//     }

//     [Fact]
//     public async Task Delete_Existing_Returns200_AndIsRemovedFromDatabase()
//     {
//         var created = await CreateShipmentAsync();

//         var response = await _client.DeleteAsync($"/api/shipments/{created.Id}");

//         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//         using var scope = _factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         Assert.Null(await db.Shipments.FindAsync(created.Id));
//     }

//     [Fact]
//     public async Task Delete_UnknownId_Returns404()
//     {
//         var response = await _client.DeleteAsync("/api/shipments/999999");

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task Delete_Shipment_AlsoRemovesItsItems()
//     {
//         var created = await CreateShipmentAsync();
//         var other = await CreateShipmentAsync();
//         await _client.PutAsJsonAsync($"/api/shipments/{created.Id}/items",
//             new List<ShipmentItemDto> { new() { ItemId = 7001, Amount = 2 } });
//         await _client.PutAsJsonAsync($"/api/shipments/{other.Id}/items",
//             new List<ShipmentItemDto> { new() { ItemId = 7002, Amount = 5 } });

//         var response = await _client.DeleteAsync($"/api/shipments/{created.Id}");

//         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//         using var scope = _factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         Assert.False(await db.ShipmentItems.AnyAsync(i => i.ShipmentId == created.Id));
//         Assert.True(await db.ShipmentItems.AnyAsync(i => i.ShipmentId == other.Id));
//     }

//     [Fact]
//     public async Task GetItems_NewShipment_ReturnsEmptyList()
//     {
//         var created = await CreateShipmentAsync();

//         var response = await _client.GetAsync($"/api/shipments/{created.Id}/items");

//         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//         var items = await response.Content.ReadFromJsonAsync<List<ShipmentItemDto>>();
//         Assert.Empty(items!);
//     }

//     [Fact]
//     public async Task GetItems_UnknownShipment_Returns404()
//     {
//         var response = await _client.GetAsync("/api/shipments/999999/items");

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task PutItems_UnknownShipment_Returns404()
//     {
//         var response = await _client.PutAsJsonAsync("/api/shipments/999999/items",
//             new List<ShipmentItemDto> { new() { ItemId = 1, Amount = 1 } });

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task PutItems_ReplacesItems_WithoutDuplicates()
//     {
//         var created = await CreateShipmentAsync();
//         var other = await CreateShipmentAsync();
//         await _client.PutAsJsonAsync($"/api/shipments/{other.Id}/items",
//             new List<ShipmentItemDto> { new() { ItemId = 8001, Amount = 9 } });

//         var first = await _client.PutAsJsonAsync($"/api/shipments/{created.Id}/items",
//             new List<ShipmentItemDto>
//             {
//                 new() { ItemId = 8001, Amount = 2 },
//                 new() { ItemId = 8002, Amount = 1 }
//             });
//         Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);

//         var afterFirst = await _client.GetFromJsonAsync<List<ShipmentItemDto>>($"/api/shipments/{created.Id}/items");
//         Assert.Equal(2, afterFirst!.Count);

//         var second = await _client.PutAsJsonAsync($"/api/shipments/{created.Id}/items",
//             new List<ShipmentItemDto> { new() { ItemId = 8001, Amount = 3 } });
//         Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);

//         var afterSecond = await _client.GetFromJsonAsync<List<ShipmentItemDto>>($"/api/shipments/{created.Id}/items");
//         var only = Assert.Single(afterSecond!);
//         Assert.Equal(8001, only.ItemId);
//         Assert.Equal(3, only.Amount);
//         Assert.All(afterSecond!, i => Assert.Equal(created.Id, i.ShipmentId));

//         var otherItems = await _client.GetFromJsonAsync<List<ShipmentItemDto>>($"/api/shipments/{other.Id}/items");
//         var otherOnly = Assert.Single(otherItems!);
//         Assert.Equal(9, otherOnly.Amount);
//     }

//     [Fact]
//     public async Task PutItems_AdjustsInventoryOrderedQuantity()
//     {
//         var itemId = await SeedInventoryAsync(quantityOnHand: 50, quantityOrdered: 10);
//         var created = await CreateShipmentAsync();

//         await _client.PutAsJsonAsync($"/api/shipments/{created.Id}/items",
//             new List<ShipmentItemDto> { new() { ItemId = itemId, Amount = 4 } });
//         Assert.Equal(14, await GetQuantityOrderedAsync(itemId));   // +4

//         await _client.PutAsJsonAsync($"/api/shipments/{created.Id}/items",
//             new List<ShipmentItemDto> { new() { ItemId = itemId, Amount = 1 } });
//         Assert.Equal(11, await GetQuantityOrderedAsync(itemId));   // -3

//         await _client.PutAsJsonAsync($"/api/shipments/{created.Id}/items", new List<ShipmentItemDto>());
//         Assert.Equal(10, await GetQuantityOrderedAsync(itemId));   // -1, back to start
//     }
// }
