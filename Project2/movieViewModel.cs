using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project2
{
    public class movieViewModel:INotifyPropertyChanged
    {
        public string UserName {  get; set; }
        private HttpClient _httpClient = new HttpClient();
        private List<Movie> allMovies = new List<Movie>();
        public event PropertyChangedEventHandler? PropertyChanged;


        //it adds it to the collectionView
        public ObservableCollection<Movie> Movies { get; set; } = new ObservableCollection<Movie>();
        public ObservableCollection<Movie> ViewMovies { get; set; } = new ObservableCollection<Movie>();
        public ObservableCollection<Movie> FavoriteMovies { get; set; } = new ObservableCollection<Movie>();
        public ObservableCollection<MovieHistory> History { get; set; } = new ObservableCollection<MovieHistory>();




        protected virtual void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string searchMovie;
        public string SearchMovie
        {
            get => searchMovie;
            set
            {
                searchMovie = value;
                OnPropertyChanged(nameof(SearchMovie));
                FilterMovie();
            }
        }
        public Command<string> SearchCommand { get; set; }

        public movieViewModel(string userName)
        {
            UserName = userName;
            SearchCommand = new Command<string>((text) =>
            {
                SearchMovie = text;
            });
            Movies = new ObservableCollection<Movie>();
            History = new ObservableCollection<MovieHistory>(HistorySandL.Load(UserName));
            foreach (var movie in Movies)
            {
                movie.PropertyChanged += MoviePropertyChanged;
            }
        }
        private Movie _selectedMovie;
        public Movie SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                OnPropertyChanged(nameof(SelectedMovie));
                if (_selectedMovie != value && value != null)
                {
                    _selectedMovie = value;
                    value.IsViewed = true;
                    if(!ViewMovies.Contains(value))
                    {
                            ViewMovies.Add(value);
                    }
                }
            }
        }
        //this stores the name of the user
        private string _Name = "Your name";
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }




        public async void downloadFile()
        {
            //if this is the first time the application is opened

            //gets the link into the response variable
            var response = await _httpClient.GetAsync("https://raw.githubusercontent.com/DonH-ITS/jsonfiles/refs/heads/main/moviesemoji.json");



            if (response == null)
            {

            }
            //if the response is successful
            if (response.IsSuccessStatusCode)
            {
                //reads the content of the json into the text variable
                String text = await response.Content.ReadAsStringAsync();
                //deserializes the content into the texts variable
                var texts = JsonSerializer.Deserialize<List<Movie>>(text);
                Movies.Clear();
                EnterMovies(texts);
                //each film in the texts list and call it film
                foreach (var film in texts)
                    // Console.WriteLine(film.title);
                    Movies.Add(new Movie
                    {
                        title = film.title,
                        year = film.year,
                        genre = film.genre,
                        director = film.director,
                        rating = film.rating,
                        emoji = film.emoji
                    });

            }
        }



        public void EnterMovies(List<Movie> movieList)
        {
            allMovies = movieList;
            Movies.Clear();
            foreach (var movie in allMovies)
                Movies.Add(movie);
        }
        private void FilterMovie()
        {

            if (string.IsNullOrWhiteSpace(SearchMovie))
            {
                Movies.Clear();
                foreach (var movie in allMovies)
                    Movies.Add(movie);
                return;
            }
            var filtered = allMovies
 .Where(m =>
m.title.Contains(SearchMovie, StringComparison.OrdinalIgnoreCase) ||
             (m.genre != null && m.genre.Any(g => g.Contains(SearchMovie, StringComparison.OrdinalIgnoreCase))) ||
               m.year.ToString().Contains(SearchMovie)
               )
               .ToList();

            Movies.Clear();
            foreach (var movie in filtered)
                Movies.Add(movie);

        }
        private void _PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(sender is Movie movie && e.PropertyName == nameof(Movie.IsFavorite))
            {
                if(movie.IsFavorite && !FavoriteMovies.Contains(movie))
                     FavoriteMovies.Add(movie);
                else if(!movie.IsFavorite && FavoriteMovies.Contains(movie))
                    FavoriteMovies.Remove(movie);
            }
        }
        public IEnumerable<Movie> ViewedRecently =>
            Movies
            .Where(m => m.ViewedAt != null)
            .OrderByDescending(m => m.ViewedAt);

        public IEnumerable<Movie> FavoritedRecently =>
  Movies
  .Where(m => m.FavoritedAt != null)
  .OrderByDescending(m => m.FavoritedAt);
        // public event PropertyChangedEventHandler PropertyChanged;
        //  protected void OnPropertyChanged([CallerMemberName] string name = null)
        //  => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

       /* public movieViewModel()
        {
            Movies = new ObservableCollection<Movie>();
            History = new ObservableCollection<MovieHistory>(HistorySandL.Load());
            foreach (var movie in Movies)
            {
                movie.PropertyChanged += MoviePropertyChanged;
            }
        }*/
        private void MoviePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is not Movie movie)
            {
                return;
            }
            if (e.PropertyName == nameof(Movie.IsFavorite) && movie.IsFavorite)
            {
                AddHistory(movie, "Favorited");
            }
        }

        public void MarkAsViewed(Movie movie)
        {
            if (!movie.IsViewed)
            {
                movie.IsViewed = true;
                AddHistory(movie, "Viewed");
            }
        }
        private void AddHistory(Movie movie, string action)
        {
            History.Add(new MovieHistory
            {
                title = movie.title,
                year = movie.year,
                genre = movie.genre,
                emoji = movie.emoji,
                Action = action,    
                Timestamp = DateTime.Now,

            });
                HistorySandL.Save(UserName, History.ToList());
        }
    }
}
