using System.Text.Json;

namespace MinAPISeparateFile;

public static class UserEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/", async context =>
        {
            // Get all todo items
            await context.Response.WriteAsJsonAsync(new { Message = "All todo items" });
        });

        app.MapGet("/{id}", async context => {
          if (context.Request.RouteValues["id"] == null){
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { Message = "Id is required" });        
          }
          try {
            int id = int.Parse(context.Request.RouteValues["id"].ToString());
            String? address = await GetAddressAsync(id);
            
            if (address == null) {
              context.Response.StatusCode = 404;
              await context.Response.WriteAsJsonAsync(new { Message = $"User {id} not found" });   
              return;     
            }

            await context.Response.WriteAsJsonAsync(new { Message = $"Address for user {id} is {address}" });
            return;
          }
          catch (FormatException)
          {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { Message = "Id must be a number" });        
            return;
          }
        });
    }

  private static async Task<string?> GetAddressAsync(int id)
  {
    string url = $"https://jsonplaceholder.typicode.com/users/{id}";
    var client = new HttpClient();
    var response = await client.GetAsync(url);
    

    if (response.IsSuccessStatusCode)
    {
      var content = await response.Content.ReadAsStringAsync();
      var user = JsonSerializer.Deserialize<User>(content);
      return $"{user.address.street}, {user.address.suite}, {user.address.city}, {user.address.zipcode}, ( {user.address.geo.lat}, {user.address.geo.lng} )"; 
    }
      return null;
  }
}

public struct Geo {
  public string lat { get; set; }
  public string lng { get; set; }
}

public struct Address {
  public string street { get; set; }
  public string city { get; set; }
  public string zipcode { get; set; }
  public string suite { get; set; }
  public Geo geo { get; set; }
}

public struct User 
{
  public int id { get; set; }
  public string name { get; set; }
  public string email { get; set; }
  public Address address { get; set; }
}