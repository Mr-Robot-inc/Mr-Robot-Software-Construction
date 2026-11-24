// using System.Net;
// using System.Net.Http.Json;
// using CargoHub.Api.Data;
// using CargoHub.Api.Dtos;
// using CargoHub.Api.Models;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;

// namespace CargoHub.Tests.Integration;

// public class OrdersApiTests : IClassFixture<CargohubApiFactory>
// {
//     private readonly CargohubApiFactory _factory;
//     private readonly HttpClient _client;

//     public OrdersApiTests(CargohubApiFactory factory)
//     {
//         _factory = factory;
//         _client = factory.CreateClient();
//     }

//     private async Task<T> WithDbAsync<T>(Func<AppDbContext, Task<T>> action)
//     {
//         using var scope = _factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         return await action(db);
//     }

//     private static CreateOrderDto ValidOrder(int clientId, int warehouseId) => new()
//     {
//         ClientId = clientId,
//         OrderDate = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc),
//         RequestDate = new DateTime(2026, 11, 5, 0, 0, 0, DateTimeKind.Utc),
//         Reference = "ORD-API",
//         CustomerPoNumber = "PO-1",
//         OrderStatus = "Pending",
//         ShippingNotes = "Handle with care",
//         WarehouseId = warehouseId,
//         ShipToClientId = clientId,
//         BillToClientId = clientId
//     };

//     private static UpdateOrderDto ValidUpdate(int clientId, int warehouseId) => new()
//     {
//         ClientId = clientId,
//         OrderDate = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc),
//         RequestDate = new DateTime(2026, 11, 9, 0, 0, 0, DateTimeKind.Utc),
//         Reference = "ORD-UPDATED",
//         CustomerPoNumber = "PO-2",
//         OrderStatus = "Shipped",
//         ShippingNotes = "Updated",
//         WarehouseId = warehouseId,
//         ShipToClientId = clientId,
//         BillToClientId = clientId
//     };

//     private async Task<(OrderDto Order, int ClientId, int WarehouseId)> CreateOrderAsync()
//     {
//         var clientId = await WithDbAsync(TestSeed.ClientAsync);
//         var warehouseId = await WithDbAsync(TestSeed.WarehouseAsync);

//         var post = await _client.PostAsJsonAsync("/api/orders", ValidOrder(clientId, warehouseId));
//         Assert.Equal(HttpStatusCode.Created, post.StatusCode);

//         var created = await post.Content.ReadFromJsonAsync<OrderDto>();
//         Assert.NotNull(created);

//         return (created, clientId, warehouseId);
//     }

//     [Fact]
//     public async Task Create_Valid_Returns201_AndCanBeRetrieved()
//     {
//         var (created, clientId, warehouseId) = await CreateOrderAsync();

//         Assert.True(created.Id > 0);
//         Assert.Equal("ORD-API", created.Reference);
//         Assert.Equal(clientId, created.ClientId);
//         Assert.Equal(warehouseId, created.WarehouseId);
//         Assert.NotEqual(default, created.CreatedAt);

//         var fetched = await _client.GetFromJsonAsync<OrderDto>($"/api/orders/{created.Id}");
//         Assert.Equal("PO-1", fetched!.CustomerPoNumber);
//     }

//     [Fact]
//     public async Task Create_UnknownClient_Returns400()
//     {
//         var warehouseId = await WithDbAsync(TestSeed.WarehouseAsync);

//         var post = await _client.PostAsJsonAsync("/api/orders", ValidOrder(999999, warehouseId));

//         Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
//     }

//     [Fact]
//     public async Task Create_UnknownWarehouse_Returns400()
//     {
//         var clientId = await WithDbAsync(TestSeed.ClientAsync);

//         var post = await _client.PostAsJsonAsync("/api/orders", ValidOrder(clientId, 999999));

//         Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
//     }

//     [Fact]
//     public async Task Create_MissingReference_Returns400()
//     {
//         var clientId = await WithDbAsync(TestSeed.ClientAsync);
//         var warehouseId = await WithDbAsync(TestSeed.WarehouseAsync);

//         var post = await _client.PostAsJsonAsync("/api/orders", ValidOrder(clientId, warehouseId) with { Reference = null });

//         Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
//     }

//     [Fact]
//     public async Task Create_ClientIdZero_Returns400()
//     {
//         var warehouseId = await WithDbAsync(TestSeed.WarehouseAsync);

//         var post = await _client.PostAsJsonAsync("/api/orders", ValidOrder(0, warehouseId));

//         Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
//     }

//     // ---------- read ----------

//     [Fact]
//     public async Task GetAll_ContainsCreatedOrder()
//     {
//         var (created, _, _) = await CreateOrderAsync();

//         var list = await _client.GetFromJsonAsync<List<OrderDto>>("/api/orders");

//         Assert.Contains(list!, o => o.Id == created.Id);
//     }

//     [Fact]
//     public async Task GetById_Unknown_Returns404()
//     {
//         var response = await _client.GetAsync("/api/orders/999999");

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task GetByClient_ReturnsOrdersOfThatClient()
//     {
//         var (created, clientId, _) = await CreateOrderAsync();

//         var response = await _client.GetAsync($"/api/orders/client/{clientId}");

//         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//         var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
//         Assert.Contains(orders!, o => o.Id == created.Id);
//     }

//     [Fact]
//     public async Task GetByClient_ClientWithoutOrders_Returns404()
//     {
//         var response = await _client.GetAsync("/api/orders/client/999999");

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     // ---------- update ----------

//     [Fact]
//     public async Task Update_Existing_Returns200_AndDataChanged()
//     {
//         var (created, clientId, warehouseId) = await CreateOrderAsync();

//         var put = await _client.PutAsJsonAsync($"/api/orders/{created.Id}", ValidUpdate(clientId, warehouseId));

//         Assert.Equal(HttpStatusCode.OK, put.StatusCode);
//         var fetched = await _client.GetFromJsonAsync<OrderDto>($"/api/orders/{created.Id}");
//         Assert.Equal("ORD-UPDATED", fetched!.Reference);
//         Assert.Equal("Shipped", fetched.OrderStatus);
//     }

//     [Fact]
//     public async Task Update_Unknown_Returns404()
//     {
//         var clientId = await WithDbAsync(TestSeed.ClientAsync);
//         var warehouseId = await WithDbAsync(TestSeed.WarehouseAsync);

//         var put = await _client.PutAsJsonAsync("/api/orders/999999", ValidUpdate(clientId, warehouseId));

//         Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
//     }

//     [Fact]
//     public async Task Update_UnknownClient_Returns400_AndOrderUnchanged()
//     {
//         var (created, _, warehouseId) = await CreateOrderAsync();

//         var put = await _client.PutAsJsonAsync($"/api/orders/{created.Id}", ValidUpdate(999999, warehouseId));

//         Assert.Equal(HttpStatusCode.BadRequest, put.StatusCode);
//         var fetched = await _client.GetFromJsonAsync<OrderDto>($"/api/orders/{created.Id}");
//         Assert.Equal("ORD-API", fetched!.Reference);
//     }

//     [Fact]
//     public async Task Create_WithItems_ReturnsThemWithTheOrder()
//     {
//         var clientId = await WithDbAsync(TestSeed.ClientAsync);
//         var warehouseId = await WithDbAsync(TestSeed.WarehouseAsync);
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);

//         var post = await _client.PostAsJsonAsync("/api/orders", ValidOrder(clientId, warehouseId) with
//         {
//             Items = new List<OrderItemDto> { new() { ItemId = itemId, Amount = 3 } }
//         });

//         Assert.Equal(HttpStatusCode.Created, post.StatusCode);
//         var created = await post.Content.ReadFromJsonAsync<OrderDto>();
//         var item = Assert.Single(created!.Items);
//         Assert.Equal(3, item.Amount);
//         Assert.Equal(4.5m, item.UnitPrice);

//         var fetched = await _client.GetFromJsonAsync<OrderDto>($"/api/orders/{created.Id}");
//         Assert.Single(fetched!.Items);
//     }

//     [Fact]
//     public async Task Update_WithItems_ReplacesItems_AndWithoutItemsKeepsThem()
//     {
//         var (created, clientId, warehouseId) = await CreateOrderAsync();
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);

//         var withItems = await _client.PutAsJsonAsync($"/api/orders/{created.Id}", ValidUpdate(clientId, warehouseId) with
//         {
//             Items = new List<OrderItemDto> { new() { ItemId = itemId, Amount = 2 } }
//         });
//         Assert.Equal(HttpStatusCode.OK, withItems.StatusCode);
//         Assert.Single((await _client.GetFromJsonAsync<OrderDto>($"/api/orders/{created.Id}"))!.Items);

//         var withoutItems = await _client.PutAsJsonAsync($"/api/orders/{created.Id}", ValidUpdate(clientId, warehouseId));
//         Assert.Equal(HttpStatusCode.OK, withoutItems.StatusCode);
//         Assert.Single((await _client.GetFromJsonAsync<OrderDto>($"/api/orders/{created.Id}"))!.Items);
//     }

//     // ---------- items ----------

//     [Fact]
//     public async Task GetItems_UnknownOrder_Returns404()
//     {
//         var response = await _client.GetAsync("/api/orders/999999/items");

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task PutItems_UnknownOrder_Returns404()
//     {
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);

//         var response = await _client.PutAsJsonAsync("/api/orders/999999/items",
//             new List<OrderItemDto> { new() { ItemId = itemId, Amount = 1 } });

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task PutItems_ReplacesItems_AndRoundTripsUnitPrice()
//     {
//         var (created, _, _) = await CreateOrderAsync();
//         var item1 = await WithDbAsync(TestSeed.ItemAsync);
//         var item2 = await WithDbAsync(TestSeed.ItemAsync);

//         var first = await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items", new List<OrderItemDto>
//         {
//             new() { ItemId = item1, Amount = 2 },
//             new() { ItemId = item2, Amount = 1 }
//         });
//         Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);

//         // the order itself now includes its items, with prices
//         var order = await _client.GetFromJsonAsync<OrderDto>($"/api/orders/{created.Id}");
//         Assert.Equal(2, order!.Items.Count);
//         Assert.Contains(order.Items, i => i.ItemId == item1 && i.Amount == 2 && i.UnitPrice == 19.99m);

//         var second = await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items",
//             new List<OrderItemDto> { new() { ItemId = item1, Amount = 3 } });
//         Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);

//         var items = await _client.GetFromJsonAsync<List<OrderItemDto>>($"/api/orders/{created.Id}/items");
//         var only = Assert.Single(items!);
//         Assert.Equal(3, only.Amount);
//         Assert.Equal(20m, only.UnitPrice);
//     }

//     [Fact]
//     public async Task PutItems_AdjustsInventoryAllocatedQuantity()
//     {
//         var (created, _, _) = await CreateOrderAsync();
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);
//         await WithDbAsync(async db => { await TestSeed.InventoryAsync(db, itemId, 50, 10); return 0; });

//         async Task<int> QuantityAllocated() => await WithDbAsync(db =>
//             db.Inventories.AsNoTracking().Where(i => i.ItemId == itemId).Select(i => i.QuantityAllocated).SingleAsync());

//         await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items",
//             new List<OrderItemDto> { new() { ItemId = itemId, Amount = 4 } });
//         Assert.Equal(14, await QuantityAllocated());   // +4

//         await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items",
//             new List<OrderItemDto> { new() { ItemId = itemId, Amount = 1 } });
//         Assert.Equal(11, await QuantityAllocated());   // -3

//         await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items", new List<OrderItemDto>());
//         Assert.Equal(10, await QuantityAllocated());   // -1, back to the start
//     }

//     [Fact]
//     public async Task PutItems_UnknownItem_Returns400_AndKeepsOldItems()
//     {
//         var (created, _, _) = await CreateOrderAsync();
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);
//         await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items",
//             new List<OrderItemDto> { new() { ItemId = itemId, Amount = 2 } });

//         var response = await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items",
//             new List<OrderItemDto> { new() { ItemId = 999999, Amount = 1 } });

//         Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//         var items = await _client.GetFromJsonAsync<List<OrderItemDto>>($"/api/orders/{created.Id}/items");
//         var kept = Assert.Single(items!);
//         Assert.Equal(itemId, kept.ItemId);
//     }

//     [Fact]
//     public async Task PutItems_AmountZero_Returns400()
//     {
//         var (created, _, _) = await CreateOrderAsync();
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);

//         var response = await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items",
//             new List<OrderItemDto> { new() { ItemId = itemId, Amount = 0 } });

//         Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//     }

//     // ---------- delete ----------

//     [Fact]
//     public async Task Delete_Existing_Returns200_AndRemovesOrderAndItems()
//     {
//         var (created, _, _) = await CreateOrderAsync();
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);
//         await _client.PutAsJsonAsync($"/api/orders/{created.Id}/items",
//             new List<OrderItemDto> { new() { ItemId = itemId, Amount = 2 } });

//         var response = await _client.DeleteAsync($"/api/orders/{created.Id}");

//         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//         Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/orders/{created.Id}")).StatusCode);
//         Assert.False(await WithDbAsync(db => db.OrderItems.AnyAsync(i => i.OrderId == created.Id)));
//     }

//     [Fact]
//     public async Task Delete_Unknown_Returns404()
//     {
//         var response = await _client.DeleteAsync("/api/orders/999999");

//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }

//     [Fact]
//     public async Task Delete_OrderWithShipment_Returns409_AndOrderStays()
//     {
//         var (created, _, _) = await CreateOrderAsync();
//         await _client.PostAsJsonAsync("/api/shipments", new CreateShipmentDto
//         {
//             Reference = "SHIP-FOR-ORDER",
//             OrderId = created.Id,
//             ShipmentDate = new DateTime(2026, 11, 12, 0, 0, 0, DateTimeKind.Utc),
//             ShipmentType = "Fast",
//             ShipmentStatus = "Pending",
//             CarrierName = "VintedGo",
//             ShippingMethod = "Air",
//             PaymentType = "Manual"
//         });

//         var response = await _client.DeleteAsync($"/api/orders/{created.Id}");

//         Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync($"/api/orders/{created.Id}")).StatusCode);
//     }

//     // ---------- the database itself enforces the foreign keys ----------

//     [Fact]
//     public async Task Database_RejectsOrderWithUnknownClient()
//     {
//         var warehouseId = await WithDbAsync(TestSeed.WarehouseAsync);

//         await Assert.ThrowsAsync<DbUpdateException>(() => WithDbAsync(async db =>
//         {
//             db.Add(new Order
//             {
//                 ClientId = 999999,
//                 ShipToClientId = 999999,
//                 BillToClientId = 999999,
//                 WarehouseId = warehouseId,
//                 Reference = "BAD",
//                 OrderStatus = "Pending",
//                 OrderDate = DateTime.UtcNow,
//                 RequestDate = DateTime.UtcNow
//             });
//             return await db.SaveChangesAsync();
//         }));
//     }

//     [Fact]
//     public async Task Database_RejectsOrderWithUnknownWarehouse()
//     {
//         var clientId = await WithDbAsync(TestSeed.ClientAsync);

//         await Assert.ThrowsAsync<DbUpdateException>(() => WithDbAsync(async db =>
//         {
//             db.Add(new Order
//             {
//                 ClientId = clientId,
//                 ShipToClientId = clientId,
//                 BillToClientId = clientId,
//                 WarehouseId = 999999,
//                 Reference = "BAD",
//                 OrderStatus = "Pending",
//                 OrderDate = DateTime.UtcNow,
//                 RequestDate = DateTime.UtcNow
//             });
//             return await db.SaveChangesAsync();
//         }));
//     }

//     [Fact]
//     public async Task Database_RejectsOrderItemWithUnknownOrderOrItem()
//     {
//         var orderId = await WithDbAsync(db => TestSeed.OrderAsync(db));
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);

//         await Assert.ThrowsAsync<DbUpdateException>(() => WithDbAsync(async db =>
//         {
//             db.Add(new OrderItem { OrderId = 999999, ItemId = itemId, Amount = 1 });
//             return await db.SaveChangesAsync();
//         }));

//         await Assert.ThrowsAsync<DbUpdateException>(() => WithDbAsync(async db =>
//         {
//             db.Add(new OrderItem { OrderId = orderId, ItemId = 999999, Amount = 1 });
//             return await db.SaveChangesAsync();
//         }));
//     }

//     [Fact]
//     public async Task Database_RejectsShipmentWithUnknownOrder()
//     {
//         await Assert.ThrowsAsync<DbUpdateException>(() => WithDbAsync(async db =>
//         {
//             db.Add(new Shipment { Reference = "BAD", OrderId = 999999, ShipmentDate = DateTime.UtcNow });
//             return await db.SaveChangesAsync();
//         }));
//     }

//     [Fact]
//     public async Task Database_DeletingOrder_CascadesToItsOrderItems()
//     {
//         var orderId = await WithDbAsync(db => TestSeed.OrderAsync(db));
//         var itemId = await WithDbAsync(TestSeed.ItemAsync);
//         await WithDbAsync(async db =>
//         {
//             db.Add(new OrderItem { OrderId = orderId, ItemId = itemId, Amount = 1 });
//             return await db.SaveChangesAsync();
//         });

//         await WithDbAsync(db => db.Orders.Where(o => o.Id == orderId).ExecuteDeleteAsync());

//         Assert.False(await WithDbAsync(db => db.OrderItems.AnyAsync(i => i.OrderId == orderId)));
//     }
// }
