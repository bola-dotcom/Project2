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
        //stores the current users name
        public string UserName {  get; set; }

        //for downloading data from API
        private HttpClient _httpClient = new HttpClient();

        //list of all movies
        private List<Movie> allMovies = new List<Movie>();

        //for when properties change
        public event PropertyChangedEventHandler? PropertyChanged;


        //it adds it to the collectionView
        public ObservableCollection<Movie> Movies { get; set; } = new ObservableCollection<Movie>();
        //movies viewed
        public ObservableCollection<Movie> ViewMovies { get; set; } = new ObservableCollection<Movie>();
        //movies favorited
        public ObservableCollection<Movie> FavoriteMovies { get; set; } = new ObservableCollection<Movie>();
        //history
        public ObservableCollection<MovieHistory> History { get; set; } = new ObservableCollection<MovieHistory>();



        //notifies that property has changed
        protected virtual void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //when users enter a search

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

        //runs when user searchees
        public Command<string> SearchCommand { get; set; }

        
        public movieViewModel(string userName)
        {
            UserName = userName;
            Name = userName;    
            //updates SearchMovie
            SearchCommand = new Command<string>((text) =>
            {
                SearchMovie = text;
            });
            
            //load history
            var loadedHistory = HistorySandL.Load(UserName);
            foreach(var item in loadedHistory)
            {
                History.Add(item);
            }
           
        }

        //currently selected movie
        private Movie _selectedMovie;
        public Movie SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                if (_selectedMovie != value && value != null)
                {
                    OnPropertyChanged(nameof(SelectedMovie));

                    _selectedMovie = value;
                   /* if (!value.IsViewed) 
                    {
                        value.IsViewed = true;
                        if (!ViewMovies.Contains(value))
                        {
                            ViewMovies.Add(value);
                        }*/
                        MarkAsViewed(value);
                    }
                }
            }
        

        //this stores the name of the user
        private string _Name = "";
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }



        //downloads movie from JSON
        public async void downloadFile()
        {
            //if this is the first time the application is opened
            if (allMovies.Count > 0)
                return;
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
               /* foreach (var film in texts)
                    // Console.WriteLine(film.title);
                    Movies.Add(new Movie
                    {
                        title = film.title,
                        year = film.year,
                        genre = film.genre,
                        director = film.director,
                        rating = film.rating,
                        emoji = film.emoji
                    })*/

            }
        }


        //ads movie to collection
        public void EnterMovies(List<Movie> movieList)
        {
            allMovies = movieList;
            Movies.Clear();
            foreach (var movie in allMovies)
            {

                movie.PropertyChanged += MoviePropertyChanged;
                Movies.Add(movie);
            }
        }
        private void FilterMovie()
        {
            //show movies if user hasnt typed anything
            if (string.IsNullOrWhiteSpace(SearchMovie))
            {
                Movies.Clear();
                foreach (var movie in allMovies)
                    Movies.Add(movie);
                return;
            }
            //filter movies based on title, genre, year and other
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
      
        //recently viewed movies and when it was viewed
        public IEnumerable<Movie> ViewedRecently =>
            Movies
            .Where(m => m.ViewedAt != null)
            .OrderByDescending(m => m.ViewedAt);


        //same thing but with favorited
        public IEnumerable<Movie> FavoritedRecently =>
  Movies
  .Where(m => m.FavoritedAt != null)
  .OrderByDescending(m => m.FavoritedAt);
        
        private void MoviePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Movie movie)
                if (e.PropertyName == nameof(Movie.IsFavorite))
                {
                    {
                        if (movie.IsFavorite && !FavoriteMovies.Contains(movie))
                            FavoriteMovies.Add(movie);
                        else if (!movie.IsFavorite && FavoriteMovies.Contains(movie))
                            FavoriteMovies.Remove(movie);
                    }
                }
                else if (e.PropertyName == nameof(Movie.IsViewed))
                {
                    if (movie.IsViewed && !ViewMovies.Contains(movie))
                    {
                        ViewMovies.Add(movie);
                    }
                }
                }
                

        public void MarkAsViewed(Movie movie)
        {
            if (!movie.IsViewed)
            {
                movie.IsViewed = true;

                if(!ViewMovies.Contains(movie))
                {
                    ViewMovies.Add(movie);
                }
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
