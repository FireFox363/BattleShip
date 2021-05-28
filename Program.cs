using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

//Semestralni Project po programovani
//Stanislav Zimirovkiy PEF 2021
//Hra lode

namespace BattleShip1
{
    class Battleship
    {        
        //Promenny
        static public string rule;
        static public string name;
        static public int pchit;
        static public int playerhit;
        static public int pcshoot;
        static public int playershoot;

        //Zadavam tri pole(jeden z cisel, a jeste dva s malymi a velkymi pismeny)
        static public string[] numbers = new string[8] { "0", "1", "2", "3", "4", "5", "6", "7"};
        static public string[] upper = new string[7] { "A", "B", "C", "D", "E", "F", "G" };
        static public string[] lowwer = new string[7] { "a", "b", "c", "d", "e", "f", "g"};

        //Vodorovny steh pro obrazovku
        public string horizontal = "A B C D E F G";

        //Velikost bojiste po ose x a y
        public const int mapSize = 7;

        //Zadavam dva pole(bojisye Hraca a bojiste Bota)
        static public int[,] myMap = new int[mapSize, mapSize];
        static public int[,] enemyMap = new int[mapSize, mapSize];

        //Booleovsky datovy typ
        static public bool nl = false;

        //----------------------------------------------------------------MAIN------------------------------------------------------------

        //Trida main v kterem je vola hlavni funkce Main
        class main
        {
            static void Main(string[] args)
            {
                
                //Pravidla hry a pozdrav
                Console.WriteLine("Ahoj,\nDnes budete hrat v hru lode");
                Console.WriteLine("Jestli si chcete precist pravidla, napiste 'ano', jestli ne tak stiskněte ENTER");
                rule = Console.ReadLine();
                Console.Clear();
                if (rule == "ano" || rule == "Ano")
                {
                    Console.WriteLine("Pravidla hry jsou jednoduchá. Oba hráči si na své bojište velikost(7X7) " +
                        "musí dát: \njednu ponorku velikost(1X1), dva torpédoborce velikost(2X1), dva křížníky velikost(3X1), jednu bitevní lod velikost(4X1)\nBudete dávat lodě střídavě" +
                        "\nPři hře se hráči vždy střídají. Hra končí když jeden z hráčů přijde o celou flotilu. " +
                        "\nBudete hrat proti AI\nAI vždy chodí prvni");
                }

                Console.WriteLine("Zadejte své jméno");
                //String vaseho jmeno
                name = Console.ReadLine();
                
                //XML soubor
                XmlWriterSettings Battleship = new XmlWriterSettings();
                using (XmlWriter xw = XmlWriter.Create("souborSP.xml", Battleship))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("Hrac");                    
                    xw.WriteStartElement("Bojiste");                    
                    xw.WriteStartElement("Pravidla");
                    xw.WriteAttributeString("Pravidla", rule);
                    xw.WriteEndElement();
                    xw.WriteElementString("Jmeno", name);
                    xw.WriteEndElement();
                    xw.WriteStartElement("Lodi");                    
                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                    xw.Flush();
                }
                
                //Vytvorim instance tridy s (new) klicovem slovem pro dalsi volani
                bot t = new bot();
                //Volame do funkce, Bot stavi sve lode, ktere nevidite
                t.BotsShips();

                //Vytvorim instance tridy s (new) klicovem slovem pro dalsi volani
                Battleship b = new Battleship();
                //Volame do funkce, zobrazuje bojiste
                b.display();

                //Vytvorim instance tridy s (new) klicovem slovem pro dalsi volani 
                player p = new player();
                //Volame do funkce, abyste mohli dat sve lode 
                p.ShipsP();

                //Booleovsky datovy typ
                bool winner = false;

                

                //Hlavni cyklicka funkce
                //Bude to delat, dokud nekdo nevyhraje
                do
                {
                    //Vola funkce, ktery nuti Bota strilet do vaseho pole
                    b.pc_shoot();
                    //Vola funkce, ktery nuti vas vybrat pozice pro strelbu v pole Bota
                    b.player_shoot();

                    //Cyklus, kdo prvni znici vsechny lode
                    if ((pchit == 15) || (playerhit == 15))
                    {
                        winner = true;
                    }
                } while (winner == false);

                //Cyklus, pokud Hrac znici vsehcny lode to vyhraje
                //Jestli jinak, to vyhraje Bot
                if (playerhit == 15)
                {
                    Console.WriteLine("Vyhrál " + name); 
                }
                else
                {
                    Console.WriteLine("Vyhrál Bot");
                }
                //Konec
                Console.WriteLine("Díky za hraní");
                Console.ReadLine();
            }
        }


        //---------------------------------------------------------------------BOJISTE-------------------------------------------------------------

        //Funkce, kde se dela bojiste 
        public void display()
        {
            //Vytvoreni bojiste Hrace a Bota s ruznymi barvami(ConsoleColor) pro lepsi pochopeni bojiste
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            //Vorovna cara
            Console.WriteLine("      " + horizontal + "                       " + horizontal);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("     " + "---------------                     ---------------");
            //Cyklus pro sestaveni bojiste 7X7
            for (int i = 0; i < mapSize; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(i + 1);
                Console.Write("|");
                Console.Write("   ");

                //Cyklus pro bojiste Hraca, kde bude ho lode a kudy bude strilit bot
                //Vratime se k ni, kdy bude strilit Bot
                for (int j = 0; j < mapSize; j++)
                {
                    //Bunky bojiste
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("|");  

                    //Pri vraceni, nic neobsahuje 
                    if (myMap[i, j] == 0)
                    {
                        Console.Write(" ");
                    }
                    //Pri vraceni, obsahuje lod Hraca(srdc)
                    else if (myMap[i, j] == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(Convert.ToChar(3));
                    }
                    //Pri vraceni, obsahuje zasah Bota
                    else if (myMap[i, j] == 8)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(Convert.ToChar(6));
                    }
                    //Pri vraceni, obsahuje chybu Bota
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(Convert.ToChar(30));
                    }
                }
                //Bunky bojiste
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("|");
                Console.Write("              ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(i + 1);
                Console.Write("|");
                Console.Write("     ");

                //Cyklus pro bojiste Bota, kde bude ho lode a kudy bude strilit Hrac
                for (int j = 0; j < mapSize; j++)
                {
                    //Bunky bojiste
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    //Pri vraceni, nic neobsahuje 
                    if (enemyMap[i, j] == 0)
                    {                        
                       Console.Write(" ");
                    }
                    //Pri vraceni, taky nic neobsahuje
                    else if (enemyMap[i, j] == 1)
                    {
                       Console.Write(Convert.ToChar(" "));
                    }
                    //Pri vraceni, obsahuje zasah Hraca
                    else if (enemyMap[i, j] == 8)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(Convert.ToChar(6)); 
                    }
                    //Pri vraceni, obsahuje chybu Hraca
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(Convert.ToChar(30)); 
                    }
                }
                //Bunky bojiste
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("|");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n");
                //Vorovna cara mezi bunkami
                Console.WriteLine("     ---------------                     ---------------");
            }
            //Konec obrazovky, kde zobrrazuje zasah, name, a kolik vystrelu
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name + "\t\t\t\t Bot");
            Console.WriteLine("Zásah: " + playerhit + "\t\t\t Zásah: " + pchit);
            Console.WriteLine("Výstřel: " + playershoot + "\t\t\t Výstřel: " + pcshoot + "\n\n");
        }


        //----------------------------------------------------------HRAC----------------------------------------------------------------

        //Trida player, kdyz vy stavite sve lode
        class player
        {
            //Funkce pro lode      
            public void ShipsP()
            {
                Battleship b = new Battleship();
                //Promenny
                int position1, position2, direction;
                string getx, gety, getd;
                //Booleovsky datovy typ
                bool flag1 = false;

                //Jedna ponorka
                Console.WriteLine("Máte jednu ponorku");
                //Cyklus, ktera dela jen jednu ponorku
                for (int s = 0; s < 1; s++)
                {
                    //Cyklus, ktery skonci, kdyz zadate spravne hodnoty
                    do
                    {
                        do
                        {
                            //Hrac musi zadat vodorovnou pozici
                            Console.WriteLine("\nZadejte vodorovnou pozici:");
                            getx = Console.ReadLine();
                            //Vrati k funkce Letters
                            getx = Letters(getx);
                        } while (nl != false);
                        //Vrati k funkce hposition
                        position2 = hposition(getx);
                        nl = false;
                        do
                        {
                            //Hrac musi zadat svislou pozici
                            Console.WriteLine("\nZadejte svislou pozici:");
                            gety = Console.ReadLine();
                            //Vrati k funkce Numbers
                            gety = Numbers(gety);
                        } while (nl != false);
                        //Prevod vstupu do int16
                        position1 = Convert.ToInt16(gety);
                        //Musime snizit hodnotu, aby delka lodi nemohla dostat mimo bojiste(pole)
                        position1--;

                        nl = false;
                        //Cyklus kontraluje, aby vase pozice byla v poli a byla prazdna
                        if ((position1 > -1) && (position1 < 7) && (position2 > -1) && (position2 < 7) && (myMap[position1, position2] == 0))
                        {
                            //Vrati k bojiste(myMap) a stavi ponorku
                            myMap[position1, position2] = 1;
                            flag1 = true;
                        }
                        //Jinak vyjde chyba
                        else
                        {
                            Console.WriteLine("Špatne zadaná data, prosím zkuste to znovu");
                            flag1 = false;
                        }
                    }
                    //Bude to delat, dokud flag1 negativni
                    while (!flag1);
                    flag1 = false;

                    //Znovu zobrazuje bojiste
                    b.display();
                }
                
                //Cely kod v teto casti bude podobny tomu predchozimu, zvyraznim hlavni rozdily

                //Dva torpédoborce
                Console.WriteLine("Máte dva torpédoborce");
                //Cyklus, ktera dela jen dva torpédoborce
                for (int s = 0; s < 2; s++)
                {
                    //Cyklus, ktery skonci, kdyz zadate spravne hodnoty
                    do
                    {
                        do
                        {
                            do
                            {
                                //Hrac musi zadat v jakem smeru bude lod stat
                                Console.WriteLine("Klikněte na 1 aby vaša lod byla vodorovnym smerem\nNebo klikněte 0 aby byla svislem smerem");
                                getd = Console.ReadLine();
                                getd = Numbers(getd);
                            } while (nl != false);
                            direction = Convert.ToInt16(getd);
                            do
                            {
                                Console.WriteLine("\nZadejte vodorovnou pozici:");
                                getx = Console.ReadLine();
                                getx = Letters(getx);
                            } while (nl != false);
                            position2 = hposition(getx);
                            nl = false;
                            do
                            {
                                //Hrac musi zadat svislou pozici
                                Console.WriteLine("\nZadejte svislou pozici:");
                                gety = Console.ReadLine();
                                gety = Numbers(gety);
                            } while (nl != false);
                            position1 = Convert.ToInt16(gety);
                            position1--;
                            nl = false;

                            //Cyklus, ktery ted kontroluje smer a aby vase pozice byla v poli a byla prazdna
                            if ((direction == 0) && (position1 > -1) && (position1 < 6) && (position2 > -1) && (position2 < 7)
                                || (direction == 1) && (position2 > -1) && (position2 < 6) && (position1 > -1) && (position1 < 7))
                            {
                                flag1 = true;
                            }
                            else
                            {
                                Console.WriteLine("Špatne zadaná data, prosím zkuste to znovu");
                            }

                        } while (!flag1);

                        flag1 = false;
                        //Pri vybore smera 0
                        //Vrati k bojiste(myMap)
                        if (direction == 0)
                        {
                            //Cyklus, kontroluje aby cela lod mohla vejit spravnym smerem
                            if ((myMap[position1, position2] == 0) && (myMap[position1 + 1, position2] == 0))
                            {
                                for (int fill = 0; fill < 2; fill++)
                                {
                                    //Stavi torpedoborec
                                    myMap[position1 + fill, position2] = 1;
                                }
                                //Konec cyklu
                                flag1 = true;
                            }
                            else Console.WriteLine("Pozice už obsahuje lod");
                        }
                        //Pri vybore smera 1
                        //Vrati k bojiste(myMap)
                        if (direction == 1)
                        {
                            //Cyklus, kontroluje aby cela lod mohla vejit spravnym smerem
                            if ((myMap[position1, position2] == 0) && (myMap[position1, position2 + 1] == 0))
                            {
                                for (int fill = 0; fill < 2; fill++)
                                {
                                    //Stavi torpedoborec
                                    myMap[position1, position2 + fill] = 1;
                                }
                                //Konec cyklu
                                flag1 = true;
                            }
                            else Console.WriteLine("Pozice už obsahuje lod");
                        }
                    } while (!flag1);
                    b.display();
                    flag1 = false;
                }

                //Cely kod v teto casti bude podobny tomu predchozimu, zvyraznim hlavni rozdily

                //dva křížníky
                Console.WriteLine("Máte dva křížníky");
                for (int s = 0; s < 2; s++)
                {
                    do
                    {
                        do
                        {
                            do
                            {
                                Console.WriteLine("Klikněte na 1 aby vaša lod byla vodorovnym smerem\nNebo klikněte 0 aby byla svislem smerem");
                                getd = Console.ReadLine();
                                getd = Numbers(getd);
                            } while (nl != false);
                            direction = Convert.ToInt16(getd);
                            do
                            {
                                Console.WriteLine("\nZadejte vodorovnou pozici:");
                                getx = Console.ReadLine();
                                getx = Letters(getx);
                            } while (nl != false);
                            position2 = hposition(getx);
                            nl = false;
                            do
                            {
                                Console.WriteLine("\nZadejte svislou pozici:");
                                gety = Console.ReadLine();
                                gety = Numbers(gety);
                            } while (nl != false);
                            position1 = Convert.ToInt16(gety);
                            position1--;
                            nl = false;
                            //Cyklus, ktery ted kontroluje smer a aby vase pozice byla v poli a byla prazdna
                            if ((direction == 0) && (position1 > -1) && (position1 < 5) && (position2 > -1) && (position2 < 7)
                                || (direction == 1) && (position2 > -1) && (position2 < 5) && (position1 > -1) && (position1 < 7))
                            {
                                flag1 = true;
                            }
                            else
                            {
                                Console.WriteLine("Špatne zadaná data, prosím zkuste to znovu");
                            }

                        } while (!flag1);

                        flag1 = false;
                        
                        if (direction == 0)
                        {
                            if ((myMap[position1, position2] == 0) && (myMap[position1 + 1, position2] == 0) && (myMap[position1 + 2, position2] == 0))
                            {
                                for (int fill = 0; fill < 3; fill++)
                                {
                                    myMap[position1 + fill, position2] = 1;
                                }
                                flag1 = true;
                            }
                            else Console.WriteLine("Pozice už obsahuje lod");
                        }
                        
                        if (direction == 1)
                        {
                            if ((myMap[position1, position2] == 0) && (myMap[position1, position2 + 1] == 0) && (myMap[position1 + 2, position2] == 0))
                            {
                                for (int fill = 0; fill < 3; fill++)
                                {
                                    myMap[position1, position2 + fill] = 1;
                                }
                                flag1 = true;
                            }
                            else Console.WriteLine("Pozice už obsahuje lod");
                        }
                    } while (!flag1);
                    b.display();
                    flag1 = false;
                }

                //Cely kod v teto casti bude podobny tomu predchozimu, zvyraznim hlavni rozdily

                //Jedna bitevní lod
                Console.WriteLine("Máte jeden bitevní lod");
                 do
                 {
                      do
                      {
                            do
                            {
                                Console.WriteLine("Klikněte na 1 aby vaša lod byla vodorovnym smerem\nNebo klikněte 0 aby byla svislem smerem");
                                getd = Console.ReadLine();
                                getd = Numbers(getd);
                            } while (nl != false);
                            direction = Convert.ToInt16(getd);
                            do
                            {
                                Console.WriteLine("\nZadejte vodorovnou pozici:");
                                getx = Console.ReadLine();
                                getx = Letters(getx);
                            } while (nl != false);
                            position2 = hposition(getx);
                            nl = false;
                            do
                            {
                                Console.WriteLine("\nZadejte svislou pozici:");
                                gety = Console.ReadLine();
                                gety = Numbers(gety);
                            } while (nl != false);
                            position1 = Convert.ToInt16(gety);
                            position1--;
                            nl = false;
                            //
                            if ((direction == 0) && (position1 > -1) && (position1 < 4) && (position2 > -1) && (position2 < 7)
                                || (direction == 1) && (position2 > -1) && (position2 < 4) && (position1 > -1) && (position1 < 7))
                            {
                                flag1 = true;
                            }
                            else
                            {
                                Console.WriteLine("Špatne zadaná data, prosím zkuste to znovu");
                            }

                      } while (!flag1);

                        flag1 = false;
                        //
                        if (direction == 0)
                        {
                            if ((myMap[position1, position2] == 0) && (myMap[position1 + 1, position2] == 0))
                            {
                                for (int fill = 0; fill < 4; fill++)
                                {
                                    myMap[position1 + fill, position2] = 1;
                                }
                                flag1 = true;
                            }
                            else Console.WriteLine("Pozice už obsahuje lod");
                        }
                        //
                        if (direction == 1)
                        {
                            if ((myMap[position1, position2] == 0) && (myMap[position1, position2 + 1] == 0))
                            {
                                for (int fill = 0; fill < 4; fill++)
                                {
                                    myMap[position1, position2 + fill] = 1;
                                }
                                flag1 = true;
                            }
                            else Console.WriteLine("Pozice už obsahuje lod");
                        }
                 } while (!flag1);
                   b.display();
                   flag1 = false;                
            }
        }

        //Funkce, pri volani kontroluje je STRING velke nebo male pismeno
        //Jestli funkce pochopila ze tohle pismeno, vrati hodnotu
        static string Letters(string key)
        {           
            for (int z = 0; z < 7; z++)
            {
                //Jestli male pismeno z pole
                if (key == lowwer[z])
                {
                    return key;
                    nl = true;
                    break;
                }
                //Jestli velke pismeno z pole
                if (key == upper[z])
                {
                    return key;
                    nl = true;
                    break;
                }
            }
            nl = false;
            return "Z";
        }

        //Funkce, pri volani kontroluje je STRING cislo
        //Jestli to nepochopila, tak nic nevrati a musite zadat to znovu 
        static string Numbers(string key)
        {
            for (int z = 0; z < 8; z++)
            {
                if (key == numbers[z])
                {
                    return key;
                    nl = true;
                    break;
                }
            }
            nl = false;
            return "200";
        }
        
        //Fenkce(switch), pri volani pochopi male nebo velke pismeno a vrati jednu hodnotu pro nasledujici pouziti
        //Udelano to pro zjednoduseni vstupu od uzivatele
        static int hposition(string hpo)
        {
            int hpos;  
            //Vstup prevadi na int16
            int x = Convert.ToInt16(hpo[0]);
            switch (x)
            {
                case 65:
                case 97:
                    hpos = 0;
                    break;
                case 66:
                case 98:
                    hpos = 1;
                    break;
                case 67:
                case 99:
                    hpos = 2;
                    break;
                case 68:
                case 100:
                    hpos = 3;
                    break;
                case 69:
                case 101:
                    hpos = 4;
                    break;
                case 70:
                case 102:
                    hpos = 5;
                    break;
                case 71:
                case 103:
                    hpos = 6;
                    break;
                case 72:
                case 104:
                    hpos = 7;
                    break;
                default:
                    hpos = 20;
                    break;
            }
            return hpos;
        }


        //---------------------------------------------------------------------BOT--------------------------------------------------------------------------------------------

        //Trida bot, kdyz Bot stavi sve lode
        class bot
        {
            public void BotsShips()
            {
                
                Battleship b = new Battleship();

                //Delame random pro zadani pozice u Bota
                Random r = new Random();
                //Novy promenny s nahodnym cislem 
                int co1 = r.Next(7);
                int co2 = r.Next(7);
                //Jedna ponorka
                //Jestli pozice nic necobsahuje
                if (enemyMap[co1, co2] == 0)
                {
                   //Stavi ponorku
                   enemyMap[co1, co2] = 1;
                }

                //Dva torpédoborce a dva křížníky
                //Cyklus, ktera dela jen dva torpédoborce a dva křížníky
                for (int s = 0; s< 2; s++) 
                {
                    //Cyklus, aby delka byla 2 a 3, proto count stoji do 4
                    for (int count = 2; count < 4; count++)
                    {
                        //Dve promenny s nahodnymi cisly pro dve pozice                        
                        co1 = r.Next(7 - count);
                        co2 = r.Next(7 - count);
                        //Smer 1 nebo 2
                        int directionBot = r.Next(2);
                        bool flag2 = false;
                        //Cyklus, jestli Bot vybral 0 smer(svisle)
                        if (directionBot == 0)
                        {
                            //Musime znovu zadat druhou pozice
                            //Protoze Bot zvolil svisly smer, uz 
                            co2 = r.Next(7);
                            do
                            {
                                //Pri count 2 kontroluje aby aby cela lod mohla vejit spravnym smerem a nevychazela z bojiste
                                //Pri count 3 kontroluje aby aby cela lod mohla vejit spravnym smerem a nevychazela z bojiste
                                if ((count == 2) && ((enemyMap[co1, co2] == 0) && (enemyMap[co1 + 1, co2] == 0))
                                    || (count == 3) && ((enemyMap[co1, co2] == 0) && (enemyMap[co1 + 1, co2] == 0) && (enemyMap[co1 + 2, co2] == 0)))                                    
                                {
                                    //Jestli vsecno v pohode, dela ten cyklus
                                    for (int p = 0; p < count; p++)
                                    {
                                        //Vrati k (enemyMap)
                                        //Stavi jeden torpedoborec a  křížník
                                        enemyMap[co1 + p, co2] = 1;
                                    }
                                    flag2 = true;
                                }
                                //Jestli ne tak musi to udelat jeste
                                else
                                {
                                    co1 = r.Next(7-count);
                                    co2 = r.Next(7-count);
                                    flag2 = false;
                                }
                                //Bude to delat, dokud flag2 negativni
                            } while (!flag2);
                        }
                        //Cyklus, jestli Bot vybral 0 smer(vodorovne)
                        if (directionBot == 1)
                        {
                            //Musime znovu zadat druhou pozice
                            //Protoze Bot zvolil vodorovny smer, uz
                            co1 = r.Next(7);
                            do
                            {
                                //Pri count 2 kontroluje aby aby cela lod mohla vejit spravnym smerem a nevychazela z bojiste
                                //Pri count 3 kontroluje aby aby cela lod mohla vejit spravnym smerem a nevychazela z bojiste
                                if ((count == 2) && ((enemyMap[co1, co2] == 0) && (enemyMap[co1, co2 + 1] == 0))
                                    || (count == 3) && ((enemyMap[co1, co2] == 0) && (enemyMap[co1, co2 + 1] == 0) && (enemyMap[co1, co2 + 2] == 0)))                                    
                                {
                                    //Jestli vsecno v pohode, dela ten cyklus
                                    for (int p = 0; p < count; p++)
                                    {
                                        //Vrati k (enemyMap)
                                        //Stavi jeste jeden torpedoborec a křížník
                                        enemyMap[co1, co2 + p] = 1;
                                    }
                                    flag2 = true;
                                }
                                else
                                {
                                    //Jestli ne tak musi to udelat jeste
                                    co1 = r.Next(7-count);
                                    co2 = r.Next(7-count);
                                    flag2 = false;
                                }
                                //Bude to delat, dokud flag2 negativni
                            } while (!flag2);
                        }
                    }
                }


                // Cely kod v teto casti bude podobny tomu predchozimu(Dva torpédoborce)           

                //Jedna bitevní lod
                //Cyklus, ktera dela jen jeden bitevní lod
                for (int s = 0; s < 1; s++)
                {
                    //Cyklus, aby delka byla jen 4
                    for (int count = 4; count <= 4; count++)
                    {
                        co1 = r.Next(7 - count);
                        co2 = r.Next(7 - count);
                        int directionBot = r.Next(2);
                        bool flag2 = false;
                        if (directionBot == 0)
                        {
                            co2 = r.Next(7);
                            do
                            {
                                //Pri count 4 kontroluje aby aby cela lod mohla vejit spravnym smerem a nevychazela z bojiste
                                if ((count == 4) && ((enemyMap[co1, co2] == 0) && (enemyMap[co1 + 1, co2] == 0) && (enemyMap[co1 + 2, co2] == 0) && (enemyMap[co1 + 3, co2] == 0)))
                                {
                                    for (int p = 0; p < count; p++)
                                    {
                                        enemyMap[co1 + p, co2] = 1;
                                    }
                                    flag2 = true;
                                }
                                else
                                {
                                    co1 = r.Next(7 - count);
                                    co2 = r.Next(7 - count);
                                    flag2 = false;
                                }
                                //Bude to delat, dokud flag2 negativni
                            } while (!flag2);
                        }

                        if (directionBot == 1)
                        {
                            co1 = r.Next(7);
                            do
                            {
                                //Pri count 4 kontroluje aby aby cela lod mohla vejit spravnym smerem a nevychazela z bojiste
                                if ((count == 4) && ((enemyMap[co1, co2] == 0) && (enemyMap[co1, co2 + 1] == 0) && (enemyMap[co1, co2 + 2] == 0) && (enemyMap[co1, co2 + 3] == 0)))
                                {
                                    for (int p = 0; p < count; p++)
                                    {
                                        enemyMap[co1, co2 + p] = 1;
                                    }
                                    flag2 = true;
                                }
                                else
                                {
                                    co1 = r.Next(7 - count);
                                    co2 = r.Next(7 - count);
                                    flag2 = false;
                                }
                            } while (!flag2);
                        }
                    }
                }
            }
        }


        //--------------------------------------------------------------------------------PALBA----------------------------------------------------------------

        //Funkce, kdyz strili Bot
        public void pc_shoot()
        {
            Battleship b = new Battleship();
            //promenny
            int xshoot, yshoot;
            //Booleovsky datovy typ
            bool hit = false;
            //Delame random pro zadani pozice u Bota
            Random r = new Random();
            //Cyklus pri kterem, Bot bude strilit dokud vystrel nezobrazuje na bojiste
            do
            {
                //Nahodne vybira dve pozici po ose x a y
                xshoot = r.Next(7);
                yshoot = r.Next(7);

                //Cyklus v kterem Bot se snazi zasahnout v lod Hraca
                //Vraci k bojiste(myMap)
                //Kontroluje jestli pozice neobsahuje chybu nebo zasah
                if ((myMap[xshoot, yshoot] == 0) || (myMap[xshoot, yshoot] == 1))
                {
                    //Vraci k bojiste(myMap), ale nic neobsahuje
                    if (myMap[xshoot, yshoot] == 0)
                    {
                        //Menime na 10(chyba)
                        myMap[xshoot, yshoot] = 10;
                    }
                    //Vraci k bojiste(myMap), a obsahuje lod
                    else
                    {
                        //Menime na 8(zasah)
                        myMap[xshoot, yshoot] = 8;
                        //Zapocitan zasah
                        pchit++;                        
                    }
                    hit = true;
                }
            } while (!hit);

            //Zapocitan vystrel
            pcshoot++;

            //Znovu zobrazuje bojiste
            b.display();
        }

        //Funkce, kdyz strili Hrac
        public void player_shoot()
        {
            Battleship b = new Battleship();
            //promenny
            int xshoot, yshoot;
            string getx, gety;
            //Booleovsky datovy typ
            bool hit = false;
            //Booleovsky datovy typ po ose x a y
            bool xflag = false;
            bool yflag = false;
            //Cyklus, dokud nebudou zadana spravne data, neskonci
            do
            {
                //Cyklus pro vodorovnou pozici
                do
                {
                    do
                    {
                        //Hrac musi zadat vodorovnou pozici spravne, jinak musi to zadat znovu
                        Console.Write("\nZadejte vodorovnou pozici:");
                        getx = Console.ReadLine();
                        //Vrati k funkce Letters
                        getx = Letters(getx);
                    } while (nl != false);
                    //Vrati k funkce hposition
                    yshoot = hposition(getx);  

                    //Kontroluje, ze vsechno je v mezich pole
                    if ((yshoot > -1) && (yshoot < 7))
                    {
                        xflag = true;
                    }
                    else Console.Write("Špatne zadaná data, prosím zkuste to znovu");
                } while (!xflag);

                //Cyklus pro svislou pozici
                do
                {
                    do
                    {
                        Console.WriteLine("\nZadejte svislou pozici:");
                        gety = Console.ReadLine();
                        //Vrati k funkce Numbers
                        gety = Numbers(gety);
                    } while (nl != false);
                    //Prevede v int32
                    xshoot = Convert.ToInt32(gety);
                    xshoot--;

                    //Kontroluje, ze vsechno je v mezich pole
                    if ((xshoot > -1) && (xshoot < 7))
                    {                        
                        yflag = true;
                    }
                    else Console.Write("Špatne zadaná data, prosím zkuste to znovu");
                } while (!yflag);

                //Cyklus v kterem Hrac se snazi zasahnout v lod Hraca
                //Vraci k bojiste(enemyMap)
                //Kontroluje jestli pozice neobsahuje zasah
                if ((enemyMap[xshoot, yshoot] == 0) || (enemyMap[xshoot, yshoot] == 1))
                {
                    //Vraci k bojiste(enemyMap), ale nic neobsahuje
                    if (enemyMap[xshoot, yshoot] == 0)
                    {
                        //Menime na 10(chyba)
                        enemyMap[xshoot, yshoot] = 10;
                    }
                    //Vraci k bojiste(enemyMap), a obsahuje lod
                    else
                    {
                        //Menime na 8(zasah)
                        enemyMap[xshoot, yshoot] = 8;
                        //Zapocitan zasah
                        playerhit++;                   
                    }
                    hit = true;
                }
                else Console.Write("Pozice už obsahuje zásah");
            } while (!hit);

            //Zapocitan vystrel
            playershoot++;

            //Znovu zobrazuje bojiste
            b.display();
        }
    }  
}