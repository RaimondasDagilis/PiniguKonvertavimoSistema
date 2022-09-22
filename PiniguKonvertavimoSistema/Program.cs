using System.Xml;

Init();

void Init()
{
    List<Valiuta> valiutos = new List<Valiuta>();
    valiutos.Add(new Valiuta("EUR", "1"));
    string url = @"https://www.lb.lt/webservices/FxRates/FxRates.asmx/getCurrentFxRates?tp=EU";
    XmlDocument duomenys = new XmlDocument();
    duomenys.Load(url);
    XmlNodeList nodes = duomenys.GetElementsByTagName("CcyAmt");
    string pavadinimas = "", kaina = "";
    for (int i = 1; i < nodes.Count; i += 2)
    {
        pavadinimas = nodes[i].FirstChild.InnerText;
        kaina = nodes[i].LastChild.InnerText;
        valiutos.Add(new Valiuta(pavadinimas, kaina));
    }
    MenuIrSkaiciavimai menu = new MenuIrSkaiciavimai();
    menu.PiestiMeniu(valiutos);
}

class MenuIrSkaiciavimai
{
    bool rakinamMenu = false;

    public void PiestiMeniu(List<Valiuta> v)
    {
        Console.Clear();
        for (int i = 0; i < v.Count; i++)
        {
            Console.Write(String.Format($"[{i,2}] {v[i].Pavadinimas()}  "));
            if ((i + 1) % 10 == 0)
            {
                Console.WriteLine("");
            }
        }
        Console.WriteLine("");
        MenuHandleris(v);
    }

    void MenuHandleris(List<Valiuta> v)
    {
        rakinamMenu = false;
        Console.WriteLine("Noredami iseiti is programos iveskite [Q]");
        Console.WriteLine("Noredami rikiuoti pagal :");
        Console.WriteLine("\tpavadinima iveskite [P]");
        Console.WriteLine("\tpagal kaina iveskite [K]");
        Console.Write("Iveskite valiutos numeri arba pavadinima is kokios norite keisti. : ");
        int keistiIs = ArMenuPunktas(v);
        rakinamMenu = true;
        Console.Write($"Iveskite {v[keistiIs].Pavadinimas()} suma kuria norite issikeisti : ");
        decimal keiciamaSuma = ArPiniguKiekis();
        Console.Write("Iveskite valiutos numeri arba pavadinima i kuria norite keisti (Iseiti iveskite [Q]) : ");
        int keistiI = ArMenuPunktas(v);
        decimal suma = Skaiciuoti(v[keistiIs].Kaina(), v[keistiI].Kaina(), keiciamaSuma);
        Console.Write("Jums priklauso : {0} {1}\nPaspauskite bet kuri mygtuka.", suma.ToString("0.00"), v[keistiI].Pavadinimas());
        Console.ReadKey();
        PiestiMeniu(v);
    }

    decimal Skaiciuoti(decimal keistiIs, decimal keistiI, decimal keiciamaSuma)
    {
        decimal suma = keiciamaSuma / keistiIs * keistiI;
        return suma;
    }

    decimal ArPiniguKiekis()
    {
        decimal suma = -1;
        string userInput = Console.ReadLine();
        if (userInput.ToLower().CompareTo("q") == 0)
        {
            BaigtiDarba();
        }
        if (decimal.TryParse(userInput, out suma) && suma >= 0)
        {
            return suma;
        }
        else
        {
            Console.Write("Ivedete neteisinga keiciamu pinigu suma. Pakartokite. (Iseiti iveskite [Q]) : ");
            this.ArPiniguKiekis();
        }
        return 0;
    }

    int ArMenuPunktas(List<Valiuta> v)
    {
        int menu = -1;
        while (menu == -1)
        {
            string userInput = Console.ReadLine();
            if (userInput.ToLower().CompareTo("q") == 0)
            {
                BaigtiDarba();
            }

            if (userInput.ToLower().CompareTo("p") == 0 && !rakinamMenu)
            {
                v.Sort((v, vNext) => v.Pavadinimas().CompareTo(vNext.Pavadinimas()));
                PiestiMeniu(v);
            }

            if (userInput.ToLower().CompareTo("k") == 0 && !rakinamMenu)
            {
                v.Sort((v, vNext) =>
                {
                    if (v.Kaina() > vNext.Kaina())
                    {
                        return 1;
                    }
                    else if (v.Kaina() > vNext.Kaina())
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                });
                PiestiMeniu(v);
            }

            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].Pavadinimas().CompareTo(userInput.ToUpper()) == 0)
                {
                    menu = i;
                }
            }

            if (int.TryParse(userInput, out int test))
            {
                menu = test;
            }
            if (menu == -1 || menu >= v.Count())
            {
                Console.Write("Ivedete neteisinga menu numeri. Pakartokite. (Iseiti iveskite [Q]) : ");                
            }
        }
        return menu;
    }

    void BaigtiDarba()
    {
        Console.WriteLine("Programa baige darba. Prasome paspausti bet kuri mygtuka");
        Console.ReadKey();
        Environment.Exit(0);
    }
}

class Valiuta
{
    string pavadinimas;
    decimal kaina;

    public Valiuta(string pavadinimas, string kaina)
    {
        this.pavadinimas = pavadinimas;
        this.kaina = TikrintiKaina(kaina);
    }

    public string Pavadinimas()
    {
        return this.pavadinimas;
    }

    public decimal Kaina()
    {
        return kaina;
    }

    decimal TikrintiKaina(string kaina)
    {
        decimal result = 0;
        if (decimal.TryParse(kaina, out result) && result >= 0)
        {
            return result;
        }
        else
        {
            return -1;
        }
    }
}