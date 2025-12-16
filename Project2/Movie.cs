
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    [QueryProperty(nameof(Movie), "Movie")]
    public partial class movieDetailPage : ContentPage;
    public class Movie
    {
        public Movie MovieProperty { get; set; }

        public string title { get; set; }
        public int year {  get; set; }   
        public List<string> genre { get; set; }
        public string Genre => string.Join(",", genre);
        public string director {  get; set; }
        public double rating { get; set; }
        public string emoji { get; set; }
    }
}
