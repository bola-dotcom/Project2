using System.Threading.Tasks;

namespace Project2
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private HttpClient _httpClient;
        private movieViewModel viewModel;
        private bool userNameSet = false;
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
            if (viewModel.UserName != userName)
            {
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


                var newHistory = HistorySandL.Load(userName);
                viewModel.History.Clear();
                foreach (var item in newHistory)
                {
                    viewModel.History.Add(item);
                }
                UserSection.IsVisible = true;
                userNameSet = true;

                await DisplayAlert("Success", $"Welcome, {userName}!", "OK");
            }
        }


        private async void OnFavoriteClicked(object sender, EventArgs e)
        {
            if (viewModel == null || string.IsNullOrEmpty(viewModel.UserName) || viewModel.UserName == "Guest")
            {
                await DisplayAlert("message", "Enter your name first", "Thank you");

                return;
            }
            if (viewModel.FavoriteMovies.Count == 0)
            {
                await DisplayAlert("Favorite", "You havent favorited anything", "Alright");
                return;
            }
            var favorite = string.Join("\n", viewModel.FavoriteMovies.Select(m => $"{m.title}({m.year})"));
            await DisplayAlert($"{viewModel.UserName} Favorites", favorite, "Alright");
        }

        private async void OnViewedClicked(object sender, EventArgs e)
        {
            if (viewModel == null || string.IsNullOrEmpty(viewModel.UserName) || viewModel.UserName == "Guest")
            {
                await DisplayAlert("message", "Enter your name first", "Thank you");

                return;
            }
            if (viewModel.History.Count == 0)
            {
                await DisplayAlert("History", "You havent viewed anything", "Alright");
                return;
            }
            var history = string.Join("\n", viewModel.History.OrderByDescending(h => h.Timestamp).Select(h => $"{h.Timestamp:MM/dd HH:mm} - {h.Action}: {h.title}"));
            await DisplayAlert($"{viewModel.UserName}'s History", history, "Alright");
        }

        private async Task showFavorite()
        {
            if (viewModel.FavoriteMovies.Count == 0)
            {
                await DisplayAlert("Favorite", "You havent favorited anything", "Alright");
                return;
            }
            var favorite = string.Join("\n", viewModel.FavoriteMovies.Select(m => $"{m.title}({m.year})"));
            await DisplayAlert("Your Favorites", favorite, "Alright");
        }
        private async Task showHistory()
        {
            if (viewModel.History.Count == 0)
            {
                await DisplayAlert("History", "You havent viewed anything", "Alright");
                return;
            }
            var history = string.Join("\n", viewModel.History.OrderByDescending(h => h.Timestamp).Select(h => $"{h.Timestamp:MM/dd HH:mm} - {h.Action}: {h.title}"));
            await DisplayAlert("Your History", history, "Alright");
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