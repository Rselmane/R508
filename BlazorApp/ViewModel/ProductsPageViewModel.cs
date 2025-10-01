using BlazorApp.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlazorApp.ViewModel
{
    internal sealed partial class ProductsPageViewModel : ObservableObject
    {
        private readonly WebService _service;
        public ProductsPageViewModel(WebService service) 
        {
            _service = service;
        }
    }
}
