namespace AddressSearch.AdressProvider.Filters.PersonFilter.Helpers
{
    using AddressSearch.AdressProvider.Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Recognizes polish surnames.
    /// </summary>
    public class PolishSurnameRecogniser
    {
        public PolishSurnameRecogniser()
        {
            LoadSurnameList();
        }

        /// <summary>
        ///     Checks if person is polish.
        /// </summary>
        /// <param name="name">Person name.</param>
        /// <param name="surname">Person surname.</param>
        public bool IsPolish(string name, string surname)
        {
            return ContainsPolishSurname(surname) || ContainsUniquePolishName(name);
        }

        /// <summary>
        ///     Checks if sting contains unique polish name.
        /// </summary>
        public bool ContainsUniquePolishName(string text)
        {
            foreach (var textPart in text.Split(new char[] { ' ', '-' }))
            {
                if (polishUniqueNameList.Contains(textPart.ToLower()))
                {
                    return true;
                };
            }

            return false;
        }

        /// <summary>
        ///     Checks if sting contains polish surname.
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

        protected static HashSet<string> polishSurnameList;
        protected static HashSet<string> polishUniqueNameList = new HashSet<string>
        {
            "Andrzej",
            "Bartek",
            "Bartosz",
            "Bartlomiej",
            "Bogdan",
            "Boguslaw",
            "Boleslaw",
            "Czeslaw",
            "Edward",
            "Franciszek",
            "Grzegorz",
            "Jacek",
            "Janusz",
            "Jakub",
            "Jarek",
            "Jaroslaw",
            "Jerzy",
            "Juliusz",
            "Kazimierz",
            "Klaudiusz",
            "Krzysztof",
            "Lech",
            "Leszek",
            "Lukasz",
            "Marcin",
            "Marek",
            "Mariusz",
            "Mateusz",
            "Michal",
            "Pawel",
            "Piotr",
            "Przemyslaw",
            "Radoslaw",
            "Sebastian",
            "Slawomir",
            "Stanislaw",
            "Sylwek",
            "Szymon",
            "Tadeusz",
            "Tomasz",
            "Waldemar",
            "Wawrzyniec",
            "Wladyslaw",
            "Wlodzimierz",
            "Wojciech",
            "Zbigniew",
            "Zbyszek",
            "Zdzislaw",
            "Zygmunt",
            "Agnieszka",
            "Barbara",
            "Beata",
            "Boguslawa",
            "Bozena",
            "Czeslawa",
            "Elzbieta",
            "Eugenia",
            "Ewelina",
            "Genowefa",
            "Grazyna",
            "Jadwiga",
            "Janina",
            "Jolanta",
            "Justyna",
            "Karolina",
            "Katarzyna",
            "Krystyna",
            "Magdalena",
            "Malwina",
            "Malgorzata",
            "Marzena",
            "Miroslawa",
            "Stanislawa",
            "Stefania",
            "Urszula",
            "Zofia",
            "Zuzanna"
        };

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

        private static void LoadSurnameList()
        {
            // Load only once
            if (polishSurnameList != null)
            {
                return;
            }

            polishSurnameList = new HashSet<string>();

            using (var reader = new StringReader(Resources.TypicalPolishSurnames))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
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
}
