// using CargoHub.Api.Dtos;
// using CargoHub.Api.Models;
// using CargoHub.Api.Services;
// using CargoHub.Tests.Unit.Fakes;

// namespace CargoHub.Tests.Services;

// public class OrderServiceTests
// {
//     private readonly FakeRepository<Order> _orders = new();
//     private readonly FakeRepository<OrderItem> _orderItems = new();
//     private readonly FakeRepository<Client> _clients = new();
//     private readonly FakeRepository<Warehouse> _warehouses = new();
//     private readonly FakeRepository<Item> _items = new();
//     private readonly FakeRepository<Inventory> _inventories = new();
//     private readonly FakeRepository<Shipment> _shipments = new();
//     private readonly OrderService _service;

//     public OrderServiceTests()
//     {
//         // Clients 1-2, warehouses 1-2 and items 1-3 exist.
//         for (var i = 0; i < 2; i++)
//         {
//             _clients.Add(new Client());
//             _warehouses.Add(new Warehouse());
//         }
//         for (var i = 0; i < 3; i++) _items.Add(new Item());

//         _service = new OrderService(_orders, _orderItems, _clients, _warehouses, _items, _inventories, _shipments);
//     }

//     private Order AddOrder(string reference = "ORD001")
//     {
//         var order = new Order
//         {
//             ClientId = 1,
//             OrderDate = new DateTime(2026, 11, 1),
//             RequestDate = new DateTime(2026, 11, 5),
//             Reference = reference,
//             OrderStatus = "Pending",
//             WarehouseId = 1,
//             ShipToClientId = 1,
//             BillToClientId = 2,
//             CreatedAt = new DateTime(2026, 11, 2),
//             UpdatedAt = new DateTime(2026, 11, 3)
//         };
//         _orders.Add(order);
//         return order;
//     }

//     private static CreateOrderDto ValidCreate() => new()
//     {
//         ClientId = 1,
//         OrderDate = new DateTime(2026, 11, 1),
//         RequestDate = new DateTime(2026, 11, 5),
//         Reference = "ORD-NEW",
//         CustomerPoNumber = "PO-1",
//         OrderStatus = "Pending",
//         ShippingNotes = "Handle with care",
//         WarehouseId = 1,
//         ShipToClientId = 1,
//         BillToClientId = 2
//     };

//     private static UpdateOrderDto ValidUpdate() => new()
//     {
//         ClientId = 2,
//         OrderDate = new DateTime(2026, 11, 1),
//         RequestDate = new DateTime(2026, 11, 9),
//         Reference = "ORD-UPDATED",
//         CustomerPoNumber = "PO-2",
//         OrderStatus = "Shipped",
//         ShippingNotes = "Updated",
//         WarehouseId = 2,
//         ShipToClientId = 2,
//         BillToClientId = 2
//     };

//     // ---------- read ----------

//     [Fact]
//     public async Task GetByIdAsync_ReturnsOrder_WithItemsAndTimestamps()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2, UnitPrice = 9.99m });
//         _orderItems.Add(new OrderItem { OrderId = 999, ItemId = 2, Amount = 5, UnitPrice = 1m }); // other order

//         var result = await _service.GetByIdAsync(order.Id);

//         Assert.NotNull(result);
//         Assert.Equal("ORD001", result.Reference);
//         Assert.Equal(new DateTime(2026, 11, 2), result.CreatedAt);
//         Assert.Equal(new DateTime(2026, 11, 3), result.UpdatedAt);
//         var item = Assert.Single(result.Items);
//         Assert.Equal(1, item.ItemId);
//         Assert.Equal(2, item.Amount);
//         Assert.Equal(9.99m, item.UnitPrice);
//     }

//     [Fact]
//     public async Task GetByIdAsync_ReturnsNull_WhenOrderDoesNotExist()
//     {
//         Assert.Null(await _service.GetByIdAsync(99));
//     }

//     [Fact]
//     public async Task GetAllAsync_ReturnsEveryOrder_EachWithItsOwnItems()
//     {
//         var first = AddOrder("ORD001");
//         var second = AddOrder("ORD002");
//         _orderItems.Add(new OrderItem { OrderId = first.Id, ItemId = 1, Amount = 1 });
//         _orderItems.Add(new OrderItem { OrderId = first.Id, ItemId = 2, Amount = 1 });
//         _orderItems.Add(new OrderItem { OrderId = second.Id, ItemId = 3, Amount = 4 });

//         var result = await _service.GetAllAsync();

//         Assert.Equal(2, result.Count);
//         Assert.Equal(2, result.Single(o => o.Id == first.Id).Items.Count);
//         Assert.Single(result.Single(o => o.Id == second.Id).Items);
//     }

//     [Fact]
//     public async Task GetOrderItemsInOrderAsync_ReturnsOnlyItemsOfThatOrder()
//     {
//         _orderItems.Add(new OrderItem { OrderId = 1, ItemId = 1, Amount = 1 });
//         _orderItems.Add(new OrderItem { OrderId = 2, ItemId = 2, Amount = 1 });

//         var result = await _service.GetOrderItemsInOrderAsync(1);

//         var item = Assert.Single(result);
//         Assert.Equal(1, item.OrderId);
//     }

//     [Fact]
//     public async Task GetOrdersByClientIdAsync_MatchesClientShipToAndBillTo()
//     {
//         AddOrder("A"); // client 1, shipTo 1, billTo 2
//         var other = AddOrder("B");
//         other.ClientId = 2;
//         other.ShipToClientId = 2;
//         other.BillToClientId = 2;

//         var forClient1 = await _service.GetOrdersByClientIdAsync(1);
//         var forClient2 = await _service.GetOrdersByClientIdAsync(2);

//         Assert.Single(forClient1);
//         Assert.Equal(2, forClient2.Count);
//         Assert.Empty(await _service.GetOrdersByClientIdAsync(99));
//     }

//     [Fact]
//     public async Task CreateAsync_CreatesOrder()
//     {
//         var result = await _service.CreateAsync(ValidCreate());

//         Assert.Equal(1, result.Id);
//         Assert.Equal("ORD-NEW", result.Reference);
//         Assert.Equal("PO-1", result.CustomerPoNumber);
//         Assert.Empty(result.Items);
//         Assert.Single(_orders.Items);
//         Assert.Equal(1, _orders.SaveCount);
//     }

//     [Theory]
//     [InlineData("client")]
//     [InlineData("shipTo")]
//     [InlineData("billTo")]
//     [InlineData("warehouse")]
//     public async Task CreateAsync_Throws_WhenReferenceDoesNotExist(string which)
//     {
//         var dto = which switch
//         {
//             "client" => ValidCreate() with { ClientId = 99 },
//             "shipTo" => ValidCreate() with { ShipToClientId = 99 },
//             "billTo" => ValidCreate() with { BillToClientId = 99 },
//             _ => ValidCreate() with { WarehouseId = 99 }
//         };

//         await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));

//         Assert.Empty(_orders.Items);
//         Assert.Equal(0, _orders.SaveCount);
//     }

//     [Fact]
//     public async Task UpdateAsync_UpdatesExistingOrder()
//     {
//         var order = AddOrder();

//         var result = await _service.UpdateAsync(order.Id, ValidUpdate());

//         Assert.NotNull(result);
//         Assert.Equal("ORD-UPDATED", result.Reference);
//         Assert.Equal(2, result.ClientId);
//         Assert.Equal(2, result.WarehouseId);
//         Assert.Equal(1, _orders.SaveCount);
//     }

//     [Fact]
//     public async Task UpdateAsync_ReturnsNull_WhenOrderDoesNotExist()
//     {
//         Assert.Null(await _service.UpdateAsync(99, ValidUpdate()));
//         Assert.Equal(0, _orders.SaveCount);
//     }

//     [Fact]
//     public async Task UpdateAsync_Throws_WhenReferenceDoesNotExist_AndLeavesOrderUnchanged()
//     {
//         var order = AddOrder();

//         await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(order.Id, ValidUpdate() with { ClientId = 99 }));

//         Assert.Equal("ORD001", order.Reference);
//         Assert.Equal(0, _orders.SaveCount);
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_ReplacesItems_AndKeepsUnitPrice()
//     {
//         var order = AddOrder();
//         var other = AddOrder("ORD002");
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2, UnitPrice = 1m });
//         _orderItems.Add(new OrderItem { OrderId = other.Id, ItemId = 1, Amount = 7, UnitPrice = 1m });

//         await _service.UpdateOrderItemsInOrderAsync(order.Id, new List<OrderItemDto>
//         {
//             new() { ItemId = 2, Amount = 3, UnitPrice = 19.99m },
//             new() { ItemId = 3, Amount = 1, UnitPrice = 5m }
//         });

//         var mine = _orderItems.Items.Where(i => i.OrderId == order.Id).ToList();
//         Assert.Equal(2, mine.Count);
//         Assert.Contains(mine, i => i.ItemId == 2 && i.Amount == 3 && i.UnitPrice == 19.99m);
//         Assert.Contains(mine, i => i.ItemId == 3 && i.Amount == 1 && i.UnitPrice == 5m);
//         Assert.Single(_orderItems.Items, i => i.OrderId == other.Id);
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_Throws_WhenItemDoesNotExist_AndKeepsOldItems()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2 });

//         await Assert.ThrowsAsync<ArgumentException>(() =>
//             _service.UpdateOrderItemsInOrderAsync(order.Id, new List<OrderItemDto>
//             {
//                 new() { ItemId = 2, Amount = 1 },
//                 new() { ItemId = 99, Amount = 1 }
//             }));

//         var remaining = Assert.Single(_orderItems.Items);
//         Assert.Equal(1, remaining.ItemId);
//         Assert.Equal(0, _orderItems.SaveCount);
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_AdjustsAllocatedQuantityByTheDifference()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2 });
//         _orderItems.Add(new OrderItem { OrderId = 999, ItemId = 1, Amount = 9 }); // other order, ignored
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 50, QuantityAllocated = 10 });
//         _inventories.Add(new Inventory { ItemId = 2, QuantityOnHand = 50, QuantityAllocated = 0 });

//         await _service.UpdateOrderItemsInOrderAsync(order.Id, new List<OrderItemDto>
//         {
//             new() { ItemId = 1, Amount = 3 },
//             new() { ItemId = 2, Amount = 1 }
//         });

//         Assert.Equal(11, _inventories.Items.Single(i => i.ItemId == 1).QuantityAllocated); // 3 - 2 = +1
//         Assert.Equal(1, _inventories.Items.Single(i => i.ItemId == 2).QuantityAllocated);  // new item, +1
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_RemovedItem_LowersAllocatedQuantity()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 4 });
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 50, QuantityAllocated = 10 });

//         await _service.UpdateOrderItemsInOrderAsync(order.Id, new List<OrderItemDto>());

//         Assert.Empty(_orderItems.Items);
//         Assert.Equal(6, _inventories.Items.Single().QuantityAllocated);
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_UsesInventoryWithMostStock()
//     {
//         var order = AddOrder();
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 5, QuantityAllocated = 0 });
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 80, QuantityAllocated = 0 });

//         await _service.UpdateOrderItemsInOrderAsync(order.Id, new List<OrderItemDto> { new() { ItemId = 1, Amount = 2 } });

//         Assert.Equal(0, _inventories.Items[0].QuantityAllocated);
//         Assert.Equal(2, _inventories.Items[1].QuantityAllocated);
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_ItemWithoutInventory_DoesNotFail()
//     {
//         var order = AddOrder();

//         await _service.UpdateOrderItemsInOrderAsync(order.Id, new List<OrderItemDto> { new() { ItemId = 1, Amount = 2 } });

//         Assert.Single(_orderItems.Items);
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_BadItem_LeavesInventoryUntouched()
//     {
//         var order = AddOrder();
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 50, QuantityAllocated = 10 });

//         await Assert.ThrowsAsync<ArgumentException>(() =>
//             _service.UpdateOrderItemsInOrderAsync(order.Id, new List<OrderItemDto>
//             {
//                 new() { ItemId = 1, Amount = 5 },
//                 new() { ItemId = 99, Amount = 1 }
//             }));

//         Assert.Equal(10, _inventories.Items.Single().QuantityAllocated);
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_Throws_WhenOrderDoesNotExist()
//     {
//         await Assert.ThrowsAsync<KeyNotFoundException>(() =>
//             _service.UpdateOrderItemsInOrderAsync(99, new List<OrderItemDto>()));
//     }

//     [Fact]
//     public async Task CreateAsync_WithItems_SavesThem_WithoutChangingTheInventory()
//     {
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 50, QuantityAllocated = 10 });

//         var result = await _service.CreateAsync(ValidCreate() with
//         {
//             Items = new List<OrderItemDto> { new() { ItemId = 1, Amount = 4, UnitPrice = 2.5m } }
//         });

//         var item = Assert.Single(result.Items);
//         Assert.Equal(result.Id, item.OrderId);
//         Assert.Equal(4, item.Amount);
//         Assert.Equal(2.5m, item.UnitPrice);
//         Assert.Equal(10, _inventories.Items.Single().QuantityAllocated); // like the python add_order
//     }

//     [Fact]
//     public async Task CreateAsync_WithUnknownItem_Throws_AndSavesNothing()
//     {
//         await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(ValidCreate() with
//         {
//             Items = new List<OrderItemDto> { new() { ItemId = 99, Amount = 1 } }
//         }));

//         Assert.Empty(_orders.Items);
//         Assert.Empty(_orderItems.Items);
//     }

//     [Fact]
//     public async Task UpdateAsync_WithItems_ReplacesItems_AndAdjustsAllocation()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2 });
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 50, QuantityAllocated = 10 });

//         var result = await _service.UpdateAsync(order.Id, ValidUpdate() with
//         {
//             Items = new List<OrderItemDto> { new() { ItemId = 1, Amount = 5 } }
//         });

//         var item = Assert.Single(result!.Items);
//         Assert.Equal(5, item.Amount);
//         Assert.Equal(13, _inventories.Items.Single().QuantityAllocated); // 5 - 2 = +3
//     }

//     [Fact]
//     public async Task UpdateAsync_WithoutItems_LeavesItemsAndInventoryAlone()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2 });
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 50, QuantityAllocated = 10 });

//         var result = await _service.UpdateAsync(order.Id, ValidUpdate()); // Items = null

//         Assert.Single(result!.Items);
//         Assert.Equal(2, _orderItems.Items.Single().Amount);
//         Assert.Equal(10, _inventories.Items.Single().QuantityAllocated);
//     }

//     [Fact]
//     public async Task UpdateAsync_WithUnknownItem_Throws_AndChangesNothing()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2 });
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 50, QuantityAllocated = 10 });

//         await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(order.Id, ValidUpdate() with
//         {
//             Items = new List<OrderItemDto> { new() { ItemId = 99, Amount = 1 } }
//         }));

//         Assert.Equal("ORD001", order.Reference);
//         Assert.Equal(2, _orderItems.Items.Single().Amount);
//         Assert.Equal(10, _inventories.Items.Single().QuantityAllocated);
//     }

//     [Fact]
//     public async Task UpdateOrderItemsInOrderAsync_NeverLetsAllocatedQuantityGoBelowZero()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 8 });
//         _inventories.Add(new Inventory { ItemId = 1, QuantityOnHand = 50, QuantityAllocated = 3 });

//         await _service.UpdateOrderItemsInOrderAsync(order.Id, new List<OrderItemDto>());

//         Assert.Equal(0, _inventories.Items.Single().QuantityAllocated); // 3 - 8 clamped at 0
//     }

//     // ---------- delete ----------

//     [Fact]
//     public async Task DeleteAsync_DeletesOrderAndItsItems()
//     {
//         var order = AddOrder();
//         var other = AddOrder("ORD002");
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2 });
//         _orderItems.Add(new OrderItem { OrderId = other.Id, ItemId = 1, Amount = 4 });

//         var result = await _service.DeleteAsync(order.Id);

//         Assert.True(result);
//         Assert.Single(_orders.Items);
//         Assert.Equal(other.Id, _orders.Items[0].Id);
//         Assert.Single(_orderItems.Items);
//         Assert.Equal(other.Id, _orderItems.Items[0].OrderId);
//     }

//     [Fact]
//     public async Task DeleteAsync_ReturnsNull_WhenOrderDoesNotExist()
//     {
//         Assert.Null(await _service.DeleteAsync(99));
//     }

//     [Fact]
//     public async Task DeleteAsync_Throws_WhenOrderHasShipments_AndDeletesNothing()
//     {
//         var order = AddOrder();
//         _orderItems.Add(new OrderItem { OrderId = order.Id, ItemId = 1, Amount = 2 });
//         _shipments.Add(new Shipment { OrderId = order.Id });

//         await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(order.Id));

//         Assert.Single(_orders.Items);
//         Assert.Single(_orderItems.Items);
//     }
// }
