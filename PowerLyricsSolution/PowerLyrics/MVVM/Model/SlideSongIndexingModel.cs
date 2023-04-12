using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerLyrics.MVVM.Model
{
    /**
     * Slúži na indexovanie modelov v playlist kolekcií slide
     */
    internal class SlideSongIndexingModel
    {
        public int indexOfFirstSlide { get; set; }
        public int indexOfLastSlide { get; set; }
    }
}