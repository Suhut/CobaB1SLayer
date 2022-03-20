// See https://aka.ms/new-console-template for more information
using B1SLayer;

var serviceLayer = new SLConnection("https://myserver:50000/b1s/v1", "mydb", "manager", "mypass");


serviceLayer.AfterCall(async call =>
{
    Console.WriteLine($"Request: {call.HttpRequestMessage.Method} {call.HttpRequestMessage.RequestUri}");
    Console.WriteLine($"Body sent: {call.RequestBody}");
    Console.WriteLine($"Response: {call.HttpResponseMessage?.StatusCode}");
    var res = await call.HttpResponseMessage?.Content?.ReadAsStringAsync();
    Console.WriteLine(res);
    Console.WriteLine($"Call duration: {call?.Duration.Value.TotalSeconds} seconds");
}); 

//01 Add New Item
var itemReq01 = new B1SLayer01.ItemModel { ItemCode = "Test01", ItemName = "Test01 Name" };
var itemRes01 = await serviceLayer.Request("Items").PostAsync<B1SLayer01.ItemModel>(itemReq01);

//02 Get Item
var itemRes02 = await serviceLayer.Request("Items", "Test01").GetAsync<B1SLayer01.ItemModel>();

//03 Update Item
await serviceLayer.Request("Items", "Test01").PatchAsync(new { ItemName = "Test01 Name update 01" });

//04 Delete Item
await serviceLayer.Request("Items", "Test01").DeleteAsync();

//05 Add new item dengan script engine
var itemReq05 = new B1SLayer01.ItemModel { ItemCode = "Test02", ItemName = "Test03 Name" };
var itemRes05 = await serviceLayer.Request("script/test/test_items").PostAsync<B1SLayer01.ItemModel>(itemReq05);

//06 Delete Item
await serviceLayer.Request("Items", "Test02").DeleteAsync();

//07 Add-Update-Delete item with SLBatchRequest
var batchRequests = new SLBatchRequest[]
{
        new SLBatchRequest(HttpMethod.Post,
            "Items",
            new  B1SLayer01.ItemModel { ItemCode = "Test01", ItemName = "Test01 Name" }
        ),
        new SLBatchRequest(HttpMethod.Patch,
            "Items('Test01')",
            new { ItemName = "Test01 Name (1)" }
        ),
        new SLBatchRequest(HttpMethod.Delete,
            "Items('Test01')"
        )
};

HttpResponseMessage[] batchResult = await serviceLayer.PostBatchAsync(batchRequests);

await serviceLayer.LogoutAsync();



