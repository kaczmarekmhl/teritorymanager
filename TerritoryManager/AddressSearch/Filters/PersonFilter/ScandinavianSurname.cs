﻿using System.Collections.Generic;
using System.Linq;
using AddressSearch.Data;

namespace AddressSearch.Filters.PersonFilter
{
    /// <summary>
    ///     Satisfies criteria if person is a male and has scandinavian surname
    /// </summary>
    public class ScandinavianSurname : IPersonFilter
    {
        public bool SatisfiesCriteria(Person person)
        {
            if (person.SearchName.IsMale == true || ContainsScandinavianUniqueFemaleName(person))
            {
                if (ContainsScandinavianSurname(person.Lastname))
                {
                    return true;
                }

                // Sometimes it happens that surename is in name
                if (ContainsScandinavianSurname(person.Name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Checks if sting contains unique scandinavian female name that will not be polish.
        /// </summary>
        public bool ContainsScandinavianUniqueFemaleName(Person person)
        {
            foreach (var textPart in person.Name.Split(new char[] { ' ', '-' }))
            {
                if (scandinavianUniqueFemaleList.Contains(textPart))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Checks if sting contains scandinacian surname
        /// </summary>
        public bool ContainsScandinavianSurname(string text)
        {
            foreach (var textPart in text.Split(new char[] { ' ', '-' }))
            {
                if (danishSurnameList.Contains(textPart))
                {
                    return true;
                }

                if (scandinavianSurnameSuffix.Any(suffix => textPart.EndsWith(suffix)))
                {
                    return true;
                }
            }

            return false;
        }

        protected static HashSet<string> scandinavianUniqueFemaleList = new HashSet<string>
        {
            "Eva",
            "Camilla",
            "Nina",
            "Monica"
        };

        public static List<string> scandinavianSurnameSuffix = new List<string>{
            "sen", 
            "son", 
            "gaard",
            "gaart",
            "gård",
            "berg",
            "borg",
            "drup",
            "trup",
            "holm",
            "skov",
            "lund",
            "bjorn",
            "vik",
            "sund",
            "stad",
            "land"
        }; 

        public static HashSet<string> danishSurnameList = new HashSet<string> {
            "Jensen",
            "Nielsen",
            "Hansen",
            "Pedersen",
            "Andersen",
            "Christensen",
            "Larsen",
            "Sørensen",
            "Rasmussen",
            "Jørgensen",
            "Petersen",
            "Madsen",
            "Kristensen",
            "Olsen",
            "Thomsen",
            "Christiansen",
            "Poulsen",
            "Johansen",
            "Knudsen",
            "Mortensen",
            "Møller",
            "Jakobsen",
            "Jacobsen",
            "Olesen",
            "Mikkelsen",
            "Frederiksen",
            "Laursen",
            "Henriksen",
            "Lund",
            "Schmidt",
            "Eriksen",
            "Holm",
            "Kristiansen",
            "Clausen",
            "Simonsen",
            "Svendsen",
            "Andreasen",
            "Iversen",
            "Jeppesen",
            "Østergaard",
            "Mogensen",
            "Lauridsen",
            "Nissen",
            "Jespersen",
            "Jepsen",
            "Frandsen",
            "Vestergaard",
            "Kjær",
            "Nørgaard",
            "Jessen",
            "Carlsen",
            "Søndergaard",
            "Dahl",
            "Skov",
            "Bertelsen",
            "Christoffersen",
            "Bruun",
            "Lassen",
            "Gregersen",
            "Bach",
            "Friis",
            "Johnsen",
            "Kjeldsen",
            "Steffensen",
            "Krogh",
            "Bech",
            "Lauritsen",
            "Danielsen",
            "Andresen",
            "Mathiesen",
            "Winther",
            "Toft",
            "Ravn",
            "Brandt",
            "Dam",
            "Holst",
            "Lind",
            "Berg",
            "Mathiasen",
            "Overgaard",
            "Bak",
            "Nilsson",
            "Klausen",
            "Schultz",
            "Schou",
            "Koch",
            "Thorsen",
            "Kristoffersen",
            "Paulsen",
            "Thygesen",
            "Hermansen",
            "Bang",
            "Nygaard",
            "Juhl",
            "Kruse",
            "Karlsen",
            "Villadsen",
            "Hedegaard",
            "Damgaard",
            "Lorenzen",
            "Bjerregaard",
            "Nguyen",
            "Søgaard",
            "Hald",
            "Kjærgaard",
            "Lorentzen",
            "Juul",
            "Munk",
            "Davidsen",
            "Lauritzen",
            "Ali",
            "Bendtsen",
            "Riis",
            "Beck",
            "Justesen",
            "Bonde",
            "Bundgaard",
            "Johannsen",
            "Lange",
            "Svensson",
            "Kofoed",
            "Meyer",
            "Aagaard",
            "Carstensen",
            "Ibsen",
            "Fischer",
            "Eskildsen",
            "Johannesen",
            "Andersson",
            "Hjorth",
            "Enevoldsen",
            "Vinther",
            "Hemmingsen",
            "Schrøder",
            "Andreassen",
            "Dalsgaard",
            "Thomassen",
            "Berthelsen",
            "Bjerre",
            "Persson",
            "Gade",
            "Asmussen",
            "Michelsen",
            "Henningsen",
            "Thøgersen",
            "Laustsen",
            "Kragh",
            "Kirkegaard",
            "Hougaard",
            "Jønsson",
            "Johansson",
            "Olsson",
            "Buch",
            "Ahmed",
            "Markussen",
            "Krog",
            "Müller",
            "Hoffmann",
            "Nikolajsen",
            "Brodersen",
            "Ipsen",
            "Sommer",
            "Leth",
            "Dalgaard",
            "Mølgaard",
            "Graversen",
            "Marcussen",
            "Clemmensen",
            "Ludvigsen",
            "Frost",
            "Laugesen",
            "Ottosen",
            "Therkildsen",
            "Sloth",
            "Ebbesen",
            "Albertsen",
            "Due",
            "Michaelsen",
            "Svenningsen",
            "Munch",
            "Bisgaard",
            "Fisker",
            "Thorup",
            "Axelsen",
            "Ottesen",
            "Westergaard",
            "Kjærsgaard",
            "Bentsen",
            "Høj",
            "Thuesen",
            "Kofod",
            "Erichsen",
            "Povlsen",
            "Albrechtsen",
            "Vilhelmsen",
            "Bjerg",
            "Buhl",
            "Bendixen",
            "Johannessen",
            "Storm",
            "Ovesen",
            "Matthiesen",
            "Lindberg",
            "Mohamed",
            "Nedergaard",
            "Villumsen",
            "Nicolaisen",
            "Smith",
            "Borup",
            "Wagner",
            "Korsgaard",
            "Thomasen",
            "Bentzen",
            "Greve",
            "Mouritsen",
            "Gram",
            "Bruhn",
            "Christophersen",
            "Haugaard",
            "Rask",
            "Dinesen",
            "Gravesen",
            "Pallesen",
            "Boesen",
            "Magnussen",
            "Isaksen",
            "Wulff",
            "Frank",
            "Hassan",
            "Bæk",
            "Bay",
            "Damsgaard",
            "Thrane",
            "Espersen",
            "Kirk",
            "Torp",
            "Rohde",
            "Rahbek",
            "Sonne",
            "Høgh",
            "Fabricius",
            "Hammer",
            "Abrahamsen",
            "Ahmad",
            "Fuglsang",
            "Pihl",
            "Daugaard",
            "Boysen",
            "Birch",
            "Antonsen",
            "Callesen",
            "Khan",
            "Dall",
            "Storgaard",
            "Larsson",
            "Ladefoged",
            "Høyer",
            "Skaarup",
            "Smidt",
            "Hussain",
            "TRUE",
            "Skou",
            "Steen",
            "Skovgaard",
            "Bjørn",
            "Hjort",
            "Truelsen",
            "Tran",
            "Busk",
            "Hartmann",
            "Karlsson",
            "Bloch",
            "Duus",
            "Hviid",
            "Borg",
            "Caspersen",
            "Philipsen",
            "Damm",
            "Balle",
            "Kvist",
            "Troelsen",
            "Mørch",
            "Martinsen",
            "Brøndum",
            "Brink",
            "Brix",
            "Ditlevsen",
            "Therkelsen",
            "Elkjær",
            "Birk",
            "Vester",
            "Degn",
            "Meldgaard",
            "Abildgaard",
            "Jansen",
            "Svane",
            "Haagensen",
            "Mørk",
            "Stokholm",
            "Adamsen",
            "Rømer",
            "Lykke",
            "Nørregaard",
            "Wind",
            "Hvid",
            "Pilgaard",
            "Grøn",
            "Skytte",
            "Højgaard",
            "Willumsen",
            "Bager",
            "Skriver",
            "Fogh",
            "Kock",
            "Lynge",
            "Gundersen",
            "Klitgaard",
            "Bengtsson",
            "Severinsen",
            "Rosenberg",
            "Brogaard",
            "Bagger",
            "Boisen",
            "Holmgaard",
            "Joensen",
            "Falk",
            "Green",
            "Bødker",
            "Groth",
            "Strøm",
            "Voss",
            "Krarup",
            "Odgaard",
            "Carlsson",
            "Hauge",
            "Gammelgaard",
            "Lundberg",
            "Wolff",
            "Dideriksen",
            "Würtz",
            "Lundsgaard",
            "Jürgensen",
            "Astrup",
            "Jæger",
            "Kramer",
            "Lehmann",
            "Le",
            "Josefsen",
            "Jonassen",
            "Skøtt",
            "Blom",
            "Drejer",
            "Schwartz",
            "Bro",
            "Martinussen",
            "Vang",
            "Qvist",
            "Bøgh",
            "Terkelsen",
            "Weber",
            "Hansson",
            "Nicolajsen",
            "Vad",
            "Kragelund",
            "Sandberg",
            "Dupont",
            "Krogsgaard",
            "Husted",
            "Meier",
            "Nørskov",
            "Keller",
            "Schneider",
            "Tolstrup",
            "Hussein",
            "Kjeldgaard",
            "Bjerrum",
            "Holt",
            "Tønnesen",
            "Koefoed",
            "Krag",
            "Kilic",
            "Lorentsen",
            "Bidstrup",
            "Bergmann",
            "Kaas",
            "Yilmaz",
            "Mark",
            "Smed",
            "Hein",
            "Yde",
            "Bagge",
            "Skjødt",
            "Buus",
            "Lundgaard",
            "Nyborg",
            "Junker",
            "Ernst",
            "Roed",
            "Mathiassen",
            "Rytter",
            "Celik",
            "Mouritzen",
            "Hermann",
            "Eliasen",
            "Holmberg",
            "Terp",
            "Pham",
            "Lindholm",
            "Lindegaard",
            "Westh",
            "Bitsch",
            "Boye",
            "Gotfredsen",
            "Borch",
            "Foged",
            "Hendriksen",
            "Busch",
            "Kronborg",
            "Kirkeby",
            "Kloster",
            "Matzen",
            "Nøhr",
            "Samuelsen",
            "Månsson",
            "Geertsen",
            "Kaspersen",
            "Raun",
            "Just",
            "Sejersen",
            "Worm",
            "Bülow",
            "Hinrichsen",
            "Stage",
            "Krabbe",
            "Børgesen",
            "Rosenkilde",
            "Ellegaard",
            "Esbensen",
            "Hertz",
            "Feddersen",
            "Slot",
            "From",
            "Espensen",
            "Hove",
            "Dissing",
            "Petersson",
            "Sander",
            "Norup",
            "Rosendahl",
            "Kure",
            "Rossen",
            "Vedel",
            "Jochumsen",
            "Abdi",
            "Mohammad",
            "Damkjær",
            "Dyhr",
            "Kjærulff",
            "Ibrahim",
            "Beyer",
            "Warming",
            "Yildirim",
            "Rose",
            "Tang",
            "Kjøller",
            "Haahr",
            "Klemmensen",
            "Rønne",
            "Stephansen",
            "Harder",
            "Ploug",
            "Eskesen",
            "Salomonsen",
            "Salling",
            "Vind",
            "Lang",
            "Pagh",
            "Sivertsen",
            "Smedegaard",
            "Kromann",
            "Bork",
            "Neumann",
            "Schulz",
            "Sahin",
            "Lausen",
            "Bille",
            "Dreyer",
            "Krüger",
            "Kastrup",
            "Lindstrøm",
            "Kaya",
            "Lerche",
            "Brøgger",
            "Steensen",
            "Steenberg",
            "Hede",
            "Lindhardt",
            "Stæhr",
            "Fog",
            "Funch",
            "Klein",
            "Flindt",
            "Have",
            "Holgersen",
            "Ladegaard",
            "Stougaard",
            "Linde",
            "Lynggaard",
            "Grønning",
            "Enemark",
            "Conradsen",
            "Bengtsen",
            "Post",
            "Refsgaard",
            "Staal",
            "Horn",
            "Osman",
            "Cramer",
            "Gertsen",
            "Bossen",
            "Risager",
            "Marker",
            "Munck",
            "Singh",
            "Mose",
            "Paaske",
            "Strand",
            "Døssing",
            "Munkholm",
            "Stampe",
            "Anderson",
            "Elgaard",
            "Høeg",
            "Kanstrup",
            "Ørum",
            "Høegh",
            "Skaaning",
            "Alstrup",
            "Bækgaard",
            "Gottlieb",
            "Deleuran",
            "Ehlers",
            "Wolf",
            "Dahlgaard",
            "Bojsen",
            "Bengtson",
            "Bjerring",
            "Lohse",
            "Rasch",
            "Melgaard",
            "Rosendal",
            "Vistisen",
            "Lausten",
            "Vangsgaard",
            "Wang",
            "Carstens",
            "Serup",
            "Kusk",
            "Sønderby",
            "Falkenberg",
            "Høy",
            "Aarup",
            "Linnet",
            "Lyhne",
            "Kaae",
            "Guldager",
            "Kold",
            "Aaen",
            "Dueholm",
            "Grønbæk",
            "Aagesen",
            "Marcher",
            "Kolding",
            "Højer",
            "Lillelund",
            "Fenger",
            "Malling",
            "Vinding",
            "Hamann",
            "Mollerup",
            "Lyngsø",
            "Brun",
            "Zachariassen",
            "Engel",
            "Skipper",
            "Tranberg",
            "Hjortshøj",
            "Witt",
            "Gormsen",
            "Søby",
            "Lau",
            "Schack",
            "Boel",
            "Agger",
            "Bugge",
            "Farah",
            "Bachmann",
            "Harboe",
            "Nørby",
            "West",
            "Mohr",
            "Aggerholm",
            "Fink",
            "Iqbal",
            "Voigt",
            "Agerskov",
            "Egholm",
            "Lindgaard",
            "Lohmann",
            "Egeberg",
            "Harbo",
            "Mygind",
            "Blaabjerg",
            "Stilling",
            "Didriksen",
            "Halkjær",
            "Karstensen",
            "Ulrich",
            "Albrektsen",
            "Noer",
            "Mørup",
            "Otte",
            "Ejlersen",
            "Gustavsen",
            "Mohamad",
            "Berntsen",
            "Søe",
            "Soelberg",
            "Breum",
            "Becker",
            "Langhoff",
            "Vittrup",
            "Vendelbo",
            "Byskov",
            "Valentin",
            "Akhtar",
            "Bank",
            "Dalby",
            "Rafn",
            "Walther",
            "Riber",
            "Kryger",
            "Bramsen",
            "Dogan",
            "Broberg",
            "Høst",
            "Appel",
            "Overby",
            "Corneliussen",
            "Tarp",
            "Moesgaard",
            "Strange",
            "Palm",
            "Neergaard",
            "Secher",
            "Thiesen",
            "Josephsen",
            "Korsholm",
            "Boll",
            "Richter",
            "Faber",
            "Hagen",
            "Hartvig",
            "Krøyer",
            "Kyed",
            "Niemann",
            "Zacho",
            "Banke",
            "Holck",
            "Straarup",
            "Køhler",
            "Martens",
            "Demir",
            "Grønlund",
            "Gaarde",
            "Hall",
            "Ismail",
            "Outzen",
            "Hovgaard",
            "Sønnichsen",
            "Ulriksen",
            "Ebsen",
            "Grønkjær",
            "Otto",
            "Begum",
            "Wichmann",
            "Anker",
            "Krebs",
            "Yildiz",
            "Rønn",
            "Tønder",
            "Suhr",
            "Tange",
            "Fallesen",
            "Bladt",
            "Jonsson",
            "Lambertsen",
            "Mejer",
            "Trolle",
            "Kappel",
            "Reimer",
            "Grønbech",
            "Højbjerg",
            "Lundgren",
            "Wiese",
            "Hoff",
            "Højlund",
            "Polat",
            "Handberg",
            "Ohlsen",
            "Stender",
            "Kudsk",
            "Bennedsen",
            "Engberg",
            "Hartvigsen",
            "Broe",
            "Hvass",
            "Mosegaard",
            "Pettersson",
            "Lam",
            "Thaysen",
            "Bock",
            "Kokholm",
            "Lomholt",
            "Emborg",
            "Nyholm",
            "Svenstrup",
            "Omar",
            "Peters",
            "Konradsen",
            "Pape",
            "Balslev",
            "Huynh",
            "Hyldgaard",
            "Moos",
            "Petterson",
            "Pontoppidan",
            "Nymann",
            "Sunesen",
            "Eriksson",
            "Franck",
            "Saleh",
            "Foldager",
            "Støvring",
            "Werner",
            "Halberg",
            "Thisted",
            "Mønster",
            "Bendsen",
            "Brødsgaard",
            "Hvidberg",
            "Østergård",
            "Martensen",
            "Balling",
            "Lykkegaard",
            "Honore",
            "Højland",
            "Kondrup",
            "Midtgaard",
            "Bredahl",
            "Viborg",
            "Amstrup",
            "Haslund",
            "Stensgaard",
            "Engstrøm",
            "Bauer",
            "Beier",
            "Lyng",
            "Randrup",
            "Bering",
            "Grønborg",
            "Halvorsen",
            "Skafte",
            "Stefansen",
            "Thyssen",
            "Turan",
            "Wiberg",
            "Husum",
            "Vestergård",
            "Hejlesen",
            "Holdt",
            "Guldbrandsen",
            "Munksgaard",
            "Alexandersen",
            "Langkjær",
            "Gadegaard",
            "Gravgaard",
            "Haastrup",
            "Lauesen",
            "Schjødt",
            "Sønderskov",
            "Østerby",
            "Juel",
            "Willadsen",
            "Thestrup",
            "Helbo",
            "Brask",
            "Lysgaard",
            "Shah",
            "Vest",
            "Borregaard",
            "Lunde",
            "Chen",
            "Hegelund",
            "Mouridsen",
            "Bilde",
            "Drachmann",
            "Honoré",
            "Ryberg",
            "Zimmermann",
            "Wilhelmsen",
            "Bennetsen",
            "Weinreich",
            "Baun",
            "Grøndahl",
            "Vinter",
            "Weiss",
            "Malmberg",
            "Hessellund",
            "Kok",
            "Topp",
            "Jokumsen",
            "Klit",
            "Bek",
            "Melchiorsen",
            "Glud",
            "Rindom",
            "Dreier",
            "Erlandsen",
            "Hjelm",
            "Hyllested",
            "Jama",
            "Brinch",
            "Bojesen",
            "Funder",
            "Kamp",
            "Kejser",
            "Højberg",
            "Lundgreen",
            "Grønhøj",
            "Gustafsson",
            "Høier",
            "Loft",
            "Kjer",
            "Skovbjerg",
            "Velling",
            "Hahn",
            "Lundqvist",
            "Timm",
            "List",
            "Magnusson",
            "Ringgaard",
            "Filtenborg",
            "Pilegaard",
            "Lentz",
            "Nymand",
            "Terkildsen",
            "Westermann",
            "Basse",
            "Brandstrup",
            "Kamper",
            "Tobiasen",
            "Hovmand",
            "Johnson",
            "Offersen",
            "Lindgreen",
            "Timmermann",
            "Edvardsen",
            "Heide",
            "Lodberg",
            "Morsing",
            "Thorhauge",
            "Duelund",
            "Riise",
            "Grau",
            "Højrup",
            "Vognsen",
            "Lunding",
            "Niebuhr",
            "Winkler",
            "Boje",
            "Frisk",
            "Glerup",
            "Kring",
            "Damborg",
            "Dehn",
            "Gissel",
            "Hesselberg",
            "Holme",
            "Houmann",
            "Kara",
            "Stryhn",
            "Sønderup",
            "Thybo",
            "Bondesen",
            "Yusuf",
            "Friedrichsen",
            "Hoppe",
            "Kiilerich",
            "Li",
            "Tøttrup",
            "Bærentsen",
            "Foss",
            "Panduro",
            "Pehrson",
            "Præst",
            "Bekker",
            "Butt",
            "Herskind",
            "Als",
            "Berger",
            "Bigum",
            "Ziegler",
            "Klavsen",
            "Børsting",
            "Carlson",
            "Dencker",
            "Langballe",
            "Lindahl",
            "Mahmood",
            "Puggaard",
            "Ring",
            "Tonnesen",
            "Clemensen",
            "Demant",
            "Egelund",
            "Gylling",
            "Jonasson",
            "Skovsgaard",
            "Binderup",
            "Djurhuus",
            "Sehested",
            "Bruus",
            "Mønsted",
            "Snedker",
            "Arildsen",
            "Geisler",
            "Klint",
            "Frølund",
            "Hildebrandt",
            "Mørck",
            "Håkansson",
            "Miller",
            "Wollesen",
            "Borre",
            "Broch",
            "Ankersen",
            "Fyhn",
            "Nordentoft",
            "Rode",
            "Futtrup",
            "Krause",
            "Meincke",
            "Rye",
            "Kjellerup",
            "Lemming",
            "Lundstrøm",
            "Mahmoud",
            "Henneberg",
            "Jantzen",
            "Roth",
            "Winkel",
            "Abel",
            "Elbæk",
            "Monrad",
            "Seerup",
            "Udsen",
            "Vilstrup",
            "Hassing",
            "Hollænder",
            "Mahler",
            "Abildtrup",
            "Bertram",
            "Haaning",
            "Termansen",
            "Dohn",
            "Kornum",
            "Sauer",
            "Øster",
            "Bendix",
            "Nørgård",
            "Sylvest",
            "Klinge",
            "Sahl",
            "Stentoft",
            "Frantzen",
            "Schütt",
            "Tychsen",
            "Behrens",
            "Rokkjær",
            "Ernstsen",
            "Jensby",
            "Ritter",
            "Raahauge",
            "Buur",
            "Guldborg",
            "Lindgren",
            "Oddershede",
            "Guldbæk",
            "Vik"
        };


    }
}
