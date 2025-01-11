using BertScout2025.Models;
using BertScout2025.PageModels;

namespace BertScout2025.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}