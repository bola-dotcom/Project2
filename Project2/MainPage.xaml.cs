using System.Threading.Tasks;

namespace Project2
{
    public partial class MainPage : ContentPage
    {
        //To track if movies are already downloaded
        int count = 0;
        
        //Http for request on the web
        private HttpClient _httpClient;

        //view Model object 
        private movieViewModel viewModel;

        //track if user has input their name
        private bool userNameSet = false;
        //movies dont load after you click button
        public MainPage()
        {
            InitializeComponent();

            //this is the defaul username 
            viewModel = new movieViewModel("Guest");

            //initialization
            _httpClient = new HttpClient();

            //connect vieModel to bindings
            BindingContext = viewModel;

            //downloads movies on the background
            Task.Run(async () => viewModel.downloadFile());

        }
        //when save button is clicked
        private async void OnStartClicked(object sender, EventArgs e)
        {
            var userName = NameBox.Text;
            //checks if name is entered
            if (string.IsNullOrEmpty(userName))
            {
                await DisplayAlert("Name error", "Please put in your name", "Ok");
                return;
            }
            //only workd if the name has changed
            if (viewModel.UserName != userName)
            {
                //creating a new viewModel wasnt working so i did this instead
                viewModel.UserName = userName;
                viewModel.Name = userName;

                var history = viewModel.History.ToList();


                //updates the binding
                OnPropertyChanged(nameof(viewModel.Name));
                if (count == 0)
                {
                    viewModel.downloadFile();
                }
                BindingContext = viewModel;

                //Load history for the new name
                var newHistory = HistorySandL.Load(userName);
                viewModel.History.Clear();
                foreach (var item in newHistory)
                {
                    viewModel.History.Add(item);
                }

                //shows the buttons for favorites amd history
                UserSection.IsVisible = true;
                userNameSet = true;

                await DisplayAlert("Success", $"Welcome, {userName}!", "OK");
            }
        }

        //When favorites button is clicked
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
            //get favorites with timestamp
            var favorite =viewModel.FavoriteMovies
                .Where(m => m.FavoritedAt != null)//only movies with timstamp
                .OrderByDescending(m => m.FavoritedAt)//sort 
                .Select(m => $"{m.FavoritedAt:MM/dd HH:mm} - {m.title} ({m.year})")
                .ToList();

            //puts all favorites into one string
            var favoriteText = string.Join("\n\n", favorite);

            await DisplayAlert($"{viewModel.UserName} Favorites", favoriteText, "Alright");
        }

        //same thing but for history
        private async void OnViewedClicked(object sender, EventArgs e)
        {
            if (viewModel == null || string.IsNullOrEmpty(viewModel.UserName) || viewModel.UserName == "Guest")
            {
                await DisplayAlert("message", "Enter your name first", "Thank you");

                return;
            }

            if (viewModel.ViewMovies.Count == 0 && viewModel.History.Count(h => h.Action == "Viewed") == 0)
            {
                await DisplayAlert("History", "You havent viewed anything", "Alright");
                return;
            }

            var viewedMovies = string.Join("\n", viewModel.ViewMovies.Select(m => $"{m.title}({m.year})"));

            var viewedHistory = viewModel.History
                .Where(h => h.Action == "Viewed")
                .OrderByDescending(h => h.Timestamp)
                .Select(h => $"{h.Timestamp:MM/dd HH:mm} - {h.Action}: {h.title}")
                .ToList();

            var historyText = string.Join("\n", viewedHistory);

            var message = $"Viewed Movies:\n{viewedMovies}\n\nViewing History:\n{historyText}";
           
            await DisplayAlert($"{viewModel.UserName}'s Viewed Content", message, "Alright");
        }

 //prevents more than one navigation
        private bool _isNavigating = false;

        //when a movie is tapped
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
                    //doesnt allow selecting the same movie later
                    ((CollectionView)sender).SelectedItem = null;

                    _isNavigating = true;
                   
                    viewModel.SelectedMovie = selectedMovie;

                    var parameters = new Dictionary<string, object>
            {
                {"Movie" , selectedMovie}
        };
                    await Shell.Current.GoToAsync(nameof(movieDetailPage), parameters);
                }
                finally
                {

                    _isNavigating = false;

                }
            }
        }
    }
}