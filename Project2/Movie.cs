
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    [QueryProperty(nameof(Movie), "Movie")]
    public class Movie :INotifyPropertyChanged
    {
        public Movie MovieProperty { get; set; }

        public string title { get; set; }
        public int year {  get; set; }   
        public List<string> genre { get; set; }
        public string Genre => string.Join(",", genre);
        public string director {  get; set; }
        public double rating { get; set; }
        public string emoji { get; set; }
        private DateTime? viewedAt;
        private DateTime? favoritedAt;


        private bool isFavorite;
        public bool IsFavorite { get => isFavorite;
            set
            {
                if (isFavorite != value)
                {
                    isFavorite = value;
                    FavoritedAt = value ? DateTime.Now : (DateTime?)null;

                    OnPropertyChanged();

                }
            }
            }

        private bool isViewed;
        public bool IsViewed
        {
            get => isViewed;
            set
            {
                if (isViewed != value)
                {
                    isViewed = value;
                    ViewedAt = value ? DateTime.Now : (DateTime?)null;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? ViewedAt { get => favoritedAt;
            set
            {
                if (viewedAt != value)
                {
                    viewedAt = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime? FavoritedAt
        {
            get => favoritedAt;
            set
            {
                if (favoritedAt != value)
                {
                    favoritedAt = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name =null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
