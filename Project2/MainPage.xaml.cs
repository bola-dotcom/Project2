namespace Project2
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private HttpClient _httpClient;
        private movieViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();

            viewModel = new movieViewModel();
            BindingContext = viewModel;
            _httpClient = new HttpClient();
            if (count == 0)
            {
                viewModel.downloadFile();
            }
        }
        private async void selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Movie selectedMovie)
            {
                var parameters = new Dictionary<string, object>
            {
                {"Movie" , selectedMovie}
        };
                await Shell.Current.GoToAsync(nameof(movieDetailPage), parameters);
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}