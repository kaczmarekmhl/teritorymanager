using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AddressSearch.AdressProvider.Filters.PersonFilter.Helpers
{
    /// <summary>
    /// Recognizes polish surnames.
    /// </summary>
    public class PolishSurnameRecognizer
    {
        public PolishSurnameRecognizer()
        {
            LoadSurnameList("Resources/PolishSurnameList.txt");
        }

        /// <summary>
        ///     Checks if sting contains polish surname
        /// </summary>
        public bool ContainsPolishSurname(string text, bool skipFirstPart = false)
        {
            foreach (var textPart in text.Split(new char[] { ' ', '-' }))
            {
                if (skipFirstPart)
                {
                    skipFirstPart = false;
                    continue;
                }

                if (polishSurnameList.Contains(textPart.ToLower()))
                {
                    return true;
                };

                if (polishSurnameSuffix.Any(suffix => textPart.EndsWith(suffix)))
                {
                    return true;
                }
            }

            return false;
        }

        protected HashSet<string> polishSurnameList;

        public static List<string> polishSurnameSuffix = new List<string>{
            "ski",
            "cki",
            "dzki",
            "cki",
            "ak",
            "ek",
            "ik",
            "yk",
            "ka",
            "owicz",
            "ewicz",
            "cz",

            "ska",
            "cka",
            "dzka",
            "cka"
        };

        private void LoadSurnameList(string filePath)
        {
            polishSurnameList = new HashSet<string>();

            foreach (string line in File.ReadAllLines(filePath))
            {
                if (!String.IsNullOrEmpty(line))
                {
                    string surname = line.Trim().ToLower();

                    polishSurnameList.Add(surname);

                    // Add surname with female ending
                    if (surname.EndsWith("ki"))
                    {
                        var femaleSurname = surname.Remove(surname.Length - 2) + "ka";

                        polishSurnameList.Add(femaleSurname);
                    }
                }
            }
        }
    }
}
