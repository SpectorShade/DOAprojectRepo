using _211426_FinalProjectDOA.Models;
using System.Collections.Generic;

namespace _211426_FinalProjectDOA.ViewModels
{
    public class SearchVM
    {
        public List<Character> theCharacters { get; set; } // List of all characters
        public string sChar { get; set; } // Search term for filtering by character name
    }
}
