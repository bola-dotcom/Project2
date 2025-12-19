using System.Threading.Tasks;

namespace Project2
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private HttpClient _httpClient;
        private movieViewModel viewModel;
        //movies dont load after you click button
        public MainPage()
        {
            InitializeComponent();

            viewModel = new movieViewModel("Guest");
            _httpClient = new HttpClient();
            BindingContext = viewModel;
            Task.Run(async () => viewModel.downloadFile());

        }
        private async void OnStartClicked(object sender, EventArgs e)
        {
            var userName = NameBox.Text;
            if (string.IsNullOrEmpty(userName))
            {
                await DisplayAlert("Name error", "Please put in your name", "Ok");
                return;
            }
            //creating a new viewModel wasnt working so i did this instead
            viewModel.UserName = userName;
            viewModel.Name = userName;

            var history = viewModel.History.ToList();

            OnPropertyChanged(nameof(viewModel.Name));    
            if (count == 0)
            {
                viewModel.downloadFile();
            }
            BindingContext = viewModel;
        }



        private bool _isNavigating = false;
        private async void selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isNavigating)
            {
                return;
            }
            if (e.CurrentSelection.FirstOrDefault() is Movie selectedMovie)
            {
                try

                {
                    ((CollectionView)sender).SelectedItem = null;

                    _isNavigating = true;
                    selectedMovie.IsViewed = true;
                    viewModel.MarkAsViewed(selectedMovie);
                    var parameters = new Dictionary<string, object>
            {
                {"Movie" , selectedMovie}
        };
                    await Shell.Current.GoToAsync(nameof(movieDetailPage), parameters);
                }
                finally
                {
                    ((CollectionView)sender).SelectedItem = null;

                    _isNavigating = false;

                }
            }
        }
    }
}