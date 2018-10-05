namespace RedactApplication.Models
{
    /// <summary>
    /// Classe implémentant un objet Singleton.
    /// </summary>
    public class StatePageSingleton
    {
        /// <summary>
        /// Attribut désignant un numéro de page
        /// </summary>
        public int? Numpage { get; set; }
        /// <summary>
        /// Attribut désignant le nombre de ligne d'une page
        /// </summary>
        public int? Nbrow { get; set; }
        /// <summary>
        /// Attribut désignant le nombre total d'index
        /// </summary>
        public double? Count { get; set; }
        /// <summary>
        /// Attribut désignant l'index précédent
        /// </summary>
        public int? Before { get; set; }
        /// <summary>
        /// Attribut désignant l'index suivant
        /// </summary>
        public int? After { get; set; }
        /// <summary>
        /// Attribut désignant l'index de début
        /// </summary>
        public int IdBegin { get; set; }
        /// <summary>
        /// Attribut désignant l'index de fin
        /// </summary>
        public int IdEnd { get; set; }
        /// <summary>
        /// Attribut désignant la limite d'index
        /// </summary>
        public int Limite { get; set; }

        /// <summary>
        /// Attribut unique de la class Singleton.
        /// </summary>
        private static StatePageSingleton Instance = null;
        /// <summary>
        /// Constructeur de la classe Singleton.
        /// </summary>
        public StatePageSingleton()
        {
            this.Numpage = 1;
            this.Nbrow = 10;
        }
        /// <summary>
        /// Constructeur de la classe Singleton.
        /// </summary>
        /// <param name="numpage">numéro de page</param>
        /// <param name="nbrow">nombre de ligne d'une page</param>
        public StatePageSingleton(int? numpage, int? nbrow)
        {
            this.Numpage = numpage;
            this.Nbrow = nbrow;
        }
        /// <summary>
        /// Crée une instance de la classe Singleton.
        /// </summary>
        private static void Construction()
        {
            //Si l'instance est null création d'un objet Singleton.
            if (nullInstance())
            {
                Instance = new StatePageSingleton();
            }
        }
        /// <summary>
        /// Crée une instance de la classe Singleton.
        /// </summary>
        /// <param name="numpage">numéro de page</param>
        /// <param name="nbrow">nombre de ligne d'une page</param>
        private static void Construction(int? numpage, int? nbrow)
        {
            //Si l'instance est null création d'un objet Singleton.
            if (nullInstance())
            {
                Instance = new StatePageSingleton(numpage, nbrow);
            }
        }

        private void InitNumpageValue() {
            Numpage = 1;
        }
        private void InitNbrowValue()
        {
            Nbrow = 10;
        }

        /// <summary>
        /// Retourne un objet Singleton.
        /// </summary>
        /// <returns>StatePageSingleton</returns>
        public static StatePageSingleton getInstance()
        {
            Construction();
            return Instance;
        }
        /// <summary>
        /// Retourne un objet Singleton.
        /// </summary>
        /// <param name="numpage">numéro de page</param>
        /// <param name="nbrow">nombre de ligne d'une page</param>
        /// <returns>StatePageSingleton</returns>
        public static StatePageSingleton getInstance(int? numpage, int? nbrow)
        {
            Construction(numpage, nbrow);
            return Instance;
        }
        /// <summary>
        /// Teste si la classe Singleton n'est pas encore Instancié.
        /// </summary>
        /// <returns>bool</returns>
        public static bool nullInstance()
        {
            if (Instance == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gère la pagination des pages
        /// </summary>
        /// <param name="count">nombre total d'enregistrement dans une table d'une base de données</param>
        public void PaginationPage(double? count)
        {
            Before = 1;
            After = 1;
            IdBegin = 1;
            IdEnd = 1;
            Limite = 6;

            if (Numpage==null)
            {
                InitNumpageValue();
            }
            if (Nbrow==null)
            {
                InitNbrowValue();
            }

            Count = count;
            if (count == null)
            {
                Count = 0;
            }
            double? temp = ((double)count / Nbrow);
            if (temp == null || temp <= 0)
            {
                temp = 1;
            }
            double? tempreste = ((double)count % Nbrow);
            if (temp != 0 && tempreste != 0)
            {
                temp++;
            }
            if ((Numpage - 1) <= 0)
            {
                Before = 1;
            }
            else
            {
                Before = Numpage-1;
            }
            if ((Numpage + 1) > temp)
            {
                After = (int)temp;
            }
            else
            {
                After = Numpage + 1;
            }
            IdEnd = (int)temp;

            if (Numpage > Limite)
            {
                IdBegin = (int)Numpage;
                Before = Numpage - 1;
                After = Numpage + 1;
                if (After > temp)
                {
                    After = (int)temp;
                }
            }
            if (tempreste == 0 && (Numpage + 1) < IdEnd)
            {
                After = Numpage + 1;
            }
        }

        /// <summary>
        /// Parse une chaine suivant l'encodage Unicode.
        /// </summary>
        /// <param name="chaine">chaine à parser</param>
        /// <returns>string</returns>
        public static string SanitizeString(string chaine)
        {
            string result = chaine;
            if (chaine!=null && chaine!="")
            {
                string temp = "";
                bool between = false;
                char[] val = chaine.ToCharArray();
                for (int i = 0; i < chaine.Length; i++)
                {
                    if (val[i] == '&' || between == true)
                    {
                        between = true;
                        temp += val[i];
                        if (val[i] == ';')
                        {
                            result = result.Replace(temp, "");
                            temp = "";
                            between = false;
                        }
                    }
                }
            }
            return result;
        }

       
    }


    /// <summary>
    /// Classe implémentant un objet Singleton.
    /// </summary>
    public class StateLogErrorPageSingleton
    {
        /// <summary>
        /// Attribut désignant un numéro de page
        /// </summary>
        public int? Numpage { get; set; }
        /// <summary>
        /// Attribut désignant le nombre de ligne d'une page
        /// </summary>
        public int? Nbrow { get; set; }
        /// <summary>
        /// Attribut désignant le nombre total d'index
        /// </summary>
        public double? Count { get; set; }
        /// <summary>
        /// Attribut désignant l'index précédent
        /// </summary>
        public int? Before { get; set; }
        /// <summary>
        /// Attribut désignant l'index suivant
        /// </summary>
        public int? After { get; set; }
        /// <summary>
        /// Attribut désignant l'index de début
        /// </summary>
        public int IdBegin { get; set; }
        /// <summary>
        /// Attribut désignant l'index de fin
        /// </summary>
        public int IdEnd { get; set; }
        /// <summary>
        /// Attribut désignant la limite d'index
        /// </summary>
        public int Limite { get; set; }

        /// <summary>
        /// Attribut unique de la class Singleton.
        /// </summary>
        private static StateLogErrorPageSingleton Instance = null;
        /// <summary>
        /// Constructeur de la classe Singleton.
        /// </summary>
        public StateLogErrorPageSingleton()
        {
            this.Numpage = 1;
            this.Nbrow = 10;
        }
        /// <summary>
        /// Constructeur de la classe Singleton.
        /// </summary>
        /// <param name="numpage">numéro de page</param>
        /// <param name="nbrow">nombre de ligne d'une page</param>
        public StateLogErrorPageSingleton(int? numpage, int? nbrow)
        {
            this.Numpage = numpage;
            this.Nbrow = nbrow;
        }
        /// <summary>
        /// Crée une instance de la classe Singleton.
        /// </summary>
        private static void Construction()
        {
            //Si l'instance est null création d'un objet Singleton.
            if (nullInstance())
            {
                Instance = new StateLogErrorPageSingleton();
            }
        }
        /// <summary>
        /// Crée une instance de la classe Singleton.
        /// </summary>
        /// <param name="numpage">numéro de page</param>
        /// <param name="nbrow">nombre de ligne d'une page</param>
        private static void Construction(int? numpage, int? nbrow)
        {
            //Si l'instance est null création d'un objet Singleton.
            if (nullInstance())
            {
                Instance = new StateLogErrorPageSingleton(numpage, nbrow);
            }
        }
        /// <summary>
        /// Retourne un objet Singleton.
        /// </summary>
        /// <returns>StatePageSingleton</returns>
        public static StateLogErrorPageSingleton getInstance()
        {
            Construction();
            return Instance;
        }
        /// <summary>
        /// Retourne un objet Singleton.
        /// </summary>
        /// <param name="numpage">numéro de page</param>
        /// <param name="nbrow">nombre de ligne d'une page</param>
        /// <returns>StatePageSingleton</returns>
        public static StateLogErrorPageSingleton getInstance(int? numpage, int? nbrow)
        {
            Construction(numpage, nbrow);
            return Instance;
        }
        /// <summary>
        /// Teste si la classe Singleton n'est pas encore Instancié.
        /// </summary>
        /// <returns>bool</returns>
        public static bool nullInstance()
        {
            if (Instance == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gère la pagination des pages
        /// </summary>
        /// <param name="count">nombre total d'enregistrement dans une table d'une base de données</param>
        public void PaginationPage(double? count)
        {
            Before = 1;
            After = 1;
            IdBegin = 1;
            IdEnd = 1;
            Limite = 6;

            Count = count;
            if (count == null)
            {
                Count = 0;
            }
            double? temp = ((double)count / Nbrow);
            if (temp == null || temp <= 0)
            {
                temp = 1;
            }
            double? tempreste = ((double)count % Nbrow);
            if (temp != 0 && tempreste != 0)
            {
                temp++;
            }
            if ((Numpage - 1) <= 0)
            {
                Before = 1;
            }
            else
            {
                Before = Numpage - 1;
            }
            if ((Numpage + 1) > temp)
            {
                After = (int)temp;
            }
            else
            {
                After = Numpage + 1;
            }
            IdEnd = (int)temp;

            if (Numpage > Limite)
            {
                IdBegin = (int)Numpage;
                Before = Numpage - 1;
                After = Numpage + 1;
                if (After > temp)
                {
                    After = (int)temp;
                }
            }
            if (tempreste == 0 && (Numpage + 1) < IdEnd)
            {
                After = Numpage + 1;
            }
        }      
    }
}