using BlazorApp.Models;

namespace BlazorApp.Service;

public class WSService : IService<Product>
{
    private readonly HttpClient httpClient = new() 
    { 
        BaseAddress = new Uri("http://localhost:5109/api/") 
    };

    public async Task AddAsync(Product product)
    {
        await httpClient.PostAsJsonAsync<Product>("produits", product);
    }

    public async Task DeleteAsync(int id)
    {
        await httpClient.DeleteAsync($"produits/{id}");
    }

    public async Task<List<Product>?> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Product>?>("produits");
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<Product?>($"produits/{id}");
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        var response = await httpClient.PostAsJsonAsync("produits/search", name);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task UpdateAsync(Product updatedProduct)
    {
        await httpClient.PutAsJsonAsync<Product>($"produits/{updatedProduct.IdProduct}", updatedProduct);
    }
}