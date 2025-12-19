
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

        private bool isFavorite;
        public bool IsFavorite { get => isFavorite;
            set
            {
                if (isFavorite != value)
                {
                    isFavorite = value;
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
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? ViewedAt { get; set; }
        public DateTime? FavoritedAt { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name =null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
