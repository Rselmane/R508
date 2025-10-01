using BlazorApp.Models;

namespace BlazorApp.Service;

public class WebService : IService<Product>
{
    private readonly HttpClient httpClient = new() 
    { 
        BaseAddress = new Uri("https://localhost:7008/api/") 
    };

    public async Task AddAsync(Product product)
    {
        await httpClient.PostAsJsonAsync<Product>("product/create", product);
    }

    public async Task DeleteAsync(int id)
    {
        await httpClient.DeleteAsync($"product/remove/{id}");
    }

    public async Task<List<Product>?> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Product>?>("product/all");
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<Product?>($"product/details/{id}");
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        var response = await httpClient.PostAsJsonAsync("products/search", name);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task UpdateAsync(Product updatedProduct)
    {
        await httpClient.PutAsJsonAsync<Product>($"products/update/{updatedProduct.Id}", updatedProduct);
    }
}