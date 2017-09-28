using AntMe.Deutsch;
using System.Collections.Generic;

namespace AntMe.Player.ParaponeraClavata
{
    /// <summary>
    /// Diese Datei enthält die Beschreibung für unsere Ameise. Die einzelnen Code-Blöcke 
    /// (Beginnend mit "public override void") fassen zusammen, wie unsere Ameise in den 
    /// entsprechenden Situationen reagieren soll.
    /// </summary>


    [Spieler(
        Volkname = "Paraponera Clavata",        // Namen des Volkes festlegen
        Vorname = "Marvin und Mikel",           // Vornamen der Schöpfer des Volks
        Nachname = "Hollmann und Bomball"       // Nachnamen der Schöpfer des Volks
    )]

    /// Kasten stellen "Berufsgruppen" innerhalb deines Ameisenvolkes dar. Hier haben wir mit
    /// den Fähigkeiten einzelner Ameisen gearbeitet. Wie genau das funktioniert kann der 
    /// Lektion zur Spezialisierung von Ameisen entnommen werden (http://wiki.antme.net/de/Lektion7).
    [Kaste(
        Name = "Krieger",                     // Name der Berufsgruppe
        AngriffModifikator = 2,               // Angriffsstärke einer Ameise
        DrehgeschwindigkeitModifikator = -1,  // Drehgeschwindigkeit einer Ameise
        EnergieModifikator = 2,               // Lebensenergie einer Ameise
        GeschwindigkeitModifikator = 0,       // Laufgeschwindigkeit einer Ameise
        LastModifikator = -1,                 // Tragkraft einer Ameise
        ReichweiteModifikator = -1,           // Ausdauer einer Ameise
        SichtweiteModifikator = -1            // Sichtweite einer Ameise
        )]
    [Kaste(
        Name = "Apfelsammler",                // Name der Berufsgruppe
        AngriffModifikator = -1,              // Angriffsstärke einer Ameise
        DrehgeschwindigkeitModifikator = -1,  // Drehgeschwindigkeit einer Ameise
        EnergieModifikator = -1,              // Lebensenergie einer Ameise
        GeschwindigkeitModifikator = 2,       // Laufgeschwindigkeit einer Ameise
        LastModifikator = 2,                  // Tragkraft einer Ameise
        ReichweiteModifikator = -1,           // Ausdauer einer Ameise
        SichtweiteModifikator = 0             // Sichtweite einer Ameise
        )]
    public class ParaponeraClavata : Basisameise
    {
        private Bau myBau = null;
        public Bau MyBau
        {
            get
            {
                //einmal pro Ameise ist myBau null
                if (myBau == null)
                {
                    //zwischenspeichern
                    Spielobjekt tempZiel = Ziel;
                    //um an den Bau zu kommen, müssen wir den Bau kurzzeitig als Ziel markieren.
                    GeheZuBau();
                    //Bau speichern
                    myBau = Ziel as Bau;
                    //Ziel auf zwischengespeichertes Ziel setzen
                    if (tempZiel != null)
                    {
                        if (tempZiel is Markierung || tempZiel is Wanze || tempZiel is Ameise)
                        {
                            GeheZuZielOptimized(tempZiel);
                        }
                        else
                        {
                            GeheZuZiel(tempZiel);
                        }
                    }
                }
                //den Bau der Ameise zurückgeben
                return myBau;
            }
            set
            {
                myBau = value;
            }
        }

        private Spielobjekt zielOptimized = null;

        /// <summary>
        /// Alternative zur Funktion namens GeheZuZiel, mit der die Ameise optimiert zum Ziel geht, also ohne Zick-Zack-Lauf
        /// </summary>
        /// <param name="zielOptimized">Das Ziel zu dem optimiert gelaufen werden soll</param>
        private void GeheZuZielOptimized(Spielobjekt zielOptimized)
        {
            //Entfernung zwischen der Ameise und dem übergebenen Ziel
            int distance = Koordinate.BestimmeEntfernung(this, zielOptimized);
            //Winkel zwischen mir und dem Ziel
            int angle = Koordinate.BestimmeRichtung(this, zielOptimized);
            //In Richtung des übergebenen Ziels drehen
            DreheInRichtung(angle);
            //Um die soeben berechnete Entfernung geradeaus gehen, um zum übergebenen Ziel zu gehen.
            GeheGeradeaus(distance);
            //Das zielOptimized der Ameise auf das übergebene Ziel setzen.
            this.zielOptimized = zielOptimized;
        }

        /// <summary>
        /// Lässt die Ameise ohne Zick-Zack-Lauf zum Bau gehen
        /// </summary>
        private void GeheZuBauOptimized()
        {
            GeheZuZielOptimized(MyBau);
        }

        #region Kasten

        /// <summary>
        /// Jedes mal, wenn eine neue Ameise geboren wird, muss ihre Berufsgruppe
        /// bestimmt werden. Das kannst du mit Hilfe dieses Rückgabewertes dieser 
        /// Methode steuern.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:BestimmeKaste
        /// </summary>
        /// <param name="anzahl">Anzahl Ameisen pro Kaste</param>
        /// <returns>Name der Kaste zu der die geborene Ameise gehören soll</returns>
        public override string BestimmeKaste(Dictionary<string, int> anzahl)
        {
            //1. Priorität: 12 Krieger erstellen, damit die Sammler nicht schutzlos rumlaufen.
            if (anzahl["Krieger"] < 12)
                return "Krieger";
            //wenn es mehr als 11 Krieger gibt und noch weniger als 8 Apfelsammler, wird ein Apfelsammler erstellt.
            if (anzahl["Apfelsammler"] < 8)
                return "Apfelsammler";
            //wenn es bereits mehr als 11 Krieger gibt und mehr als 7 Apfelsammler, wird ein weiterer Krieger erstellt.
            return "Krieger";
        }

        #endregion

        #region Fortbewegung

        /// <summary>
        /// Wenn die Ameise keinerlei Aufträge hat, wartet sie auf neue Aufgaben. Um dir das 
        /// mitzuteilen, wird diese Methode hier aufgerufen.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:Wartet
        /// </summary>
        public override void Wartet()
        {
            this.GeheGeradeaus(40);
            this.DreheUmWinkel(Zufall.Zahl(-10, 10));
        }

        /// <summary>
        /// Erreicht eine Ameise ein drittel ihrer Laufreichweite, wird diese Methode aufgerufen.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:WirdM%C3%BCde
        /// </summary>
        public override void WirdMüde()
        {
           
        }

        /// <summary>
        /// Wenn eine Ameise stirbt, wird diese Methode aufgerufen. Man erfährt dadurch, wie 
        /// die Ameise gestorben ist. Die Ameise kann zu diesem Zeitpunkt aber keinerlei Aktion 
        /// mehr ausführen.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:IstGestorben
        /// </summary>
        /// <param name="todesart">Art des Todes</param>
        public override void IstGestorben(Todesart todesart)
        {

        }

        /// <summary>
        /// Diese Methode wird in jeder Simulationsrunde aufgerufen - ungeachtet von zusätzlichen 
        /// Bedingungen. Dies eignet sich für Aktionen, die unter Bedingungen ausgeführt werden 
        /// sollen, die von den anderen Methoden nicht behandelt werden.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:Tick
        /// </summary>
        public override void Tick()
        {
            if (this.Reichweite - this.ZurückgelegteStrecke - 9 < this.EntfernungZuBau)
            {
                //Die Anzahl an Ameisenschritten, die die Ameise noch zurücklegen kann - 9, ist kleiner als die Entfernung zum Bau
                //Also gehen wir zum Bau, damit die Ameise nicht verhungert
                this.GeheZuBauOptimized();
            }
            
            if (zielOptimized != null)
            {
                int distance = Koordinate.BestimmeEntfernung(this, zielOptimized);
                if (distance < Sichtweite)
                {
                    //Das Ziel befindet sich in Sichtweite. Nun wird von GeheZuZielOptimized wieder die GeheZuZiel-Methode gewechselt.
                    GeheZuZiel(zielOptimized);
                    //Hat unsere Ameise nur noch ein Ziel und das zielOptimized wird verworfen
                    zielOptimized = null;
                }
                else
                {
                    //Das Ziel befindet sich noch nicht in Sichtweite. Erneut GeheZuZielOptimized aufrufen, damit sich die Ameise erneut auf das Ziel ausrichtet.
                    //Wenn unsere Ameise einen Apfel trägt und eine gegnerische Ameise ebenfalls an diesem Apfel zieht, kommen wir von unserem Weg ab 
                    //und laufen gegen den Rand des Spielfeldes
                    GeheZuZielOptimized(zielOptimized);
                }
            }

        }

        #endregion

        #region Nahrung

        /// <summary>
        /// Sobald eine Ameise innerhalb ihres Sichtradius einen Apfel erspäht wird 
        /// diese Methode aufgerufen. Als Parameter kommt das betroffene Stück Obst.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:Sieht(Obst)"
        /// </summary>
        /// <param name="obst">Das gesichtete Stück Obst</param>
        public override void Sieht(Obst obst)
        {
            if (Kaste == "Krieger")
            {
                int distance = Koordinate.BestimmeEntfernung(this, obst);
                if (distance <= Sichtweite - 2)
                {
                    //Der Krieger sieht den Apfel nicht nur an der Grenze der Sichtweite, sondern ist sogar noch 2 Schritte näher dran.
                    //Ohne die Abfrage kam es vor, dass unsere Apfelsammler den Apfel gar nicht gefunden haben, weil den Apfel genau 
                    //an der Grenze ihrer Sichtweite war und sie ihn deshalb nicht gesehen haben.
                    Denke("Kommt! hier ist Obst");
                    //Schicke die Apfelsammler hin (Die Information enthält bei uns den Empfänger. int.Max richtet sich dabei an Apfelsammler)
                    SprüheMarkierung(int.MaxValue, 2000);
                }
            }
            else
            {
                //Die Markierungsgröße ist beim Apfelsammler kleiner, weil diese Ameise selbst auch schon hingeht. 
                //Durch die kleinere Markierung wollen wir dafür sorgen, dass möglichst eine Ameise weniger kommt.
                //Schicke die Apfelsammler hin (Die Information enthält bei uns den Empfänger. int.Max richtet sich dabei an Apfelsammler)
                SprüheMarkierung(int.MaxValue, 1600);
                GeheZuZiel(obst);
            }
        }

        /// <summary>
        /// Sobald eine Ameise innerhalb ihres Sichtradius einen Zuckerhügel erspäht wird 
        /// diese Methode aufgerufen. Als Parameter kommt der betroffene Zuckerghügel.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:Sieht(Zucker)"
        /// </summary>
        /// <param name="zucker">Der gesichtete Zuckerhügel</param>
        public override void Sieht(Zucker zucker)
        {
            
        }

        /// <summary>
        /// Hat die Ameise ein Stück Obst als Ziel festgelegt, wird diese Methode aufgerufen, 
        /// sobald die Ameise ihr Ziel erreicht hat. Ab jetzt ist die Ameise nahe genug um mit 
        /// dem Ziel zu interagieren.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:ZielErreicht(Obst)"
        /// </summary>
        /// <param name="obst">Das erreichte Stück Obst</param>
        public override void ZielErreicht(Obst obst)
        {
            if (Kaste == "Apfelsammler")
            {
                Nimm(obst);
                GeheZuBauOptimized();
                //Schicke die Apfelsammler hin (Die Information enthält bei uns den Empfänger. int.Max richtet sich dabei an Apfelsammler)
                SprüheMarkierung(int.MaxValue,2000);
            }
        }

        /// <summary>
        /// Hat die Ameise eine Zuckerhügel als Ziel festgelegt, wird diese Methode aufgerufen, 
        /// sobald die Ameise ihr Ziel erreicht hat. Ab jetzt ist die Ameise nahe genug um mit 
        /// dem Ziel zu interagieren.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:ZielErreicht(Zucker)"
        /// </summary>
        /// <param name="zucker">Der erreichte Zuckerhügel</param>
        public override void ZielErreicht(Zucker zucker)
        {
            
        }

        #endregion

        #region Kommunikation

        /// <summary>
        /// Markierungen, die von anderen Ameisen platziert werden, können von befreundeten Ameisen 
        /// gewittert werden. Diese Methode wird aufgerufen, wenn eine Ameise zum ersten Mal eine 
        /// befreundete Markierung riecht.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:RiechtFreund(Markierung)"
        /// </summary>
        /// <param name="markierung">Die gerochene Markierung</param>
        public override void RiechtFreund(Markierung markierung)
        {
            if (this.Ziel != null || zielOptimized != null)
                return;
            //Die Information enthält bei uns den Empfänger. 
            //int.Max richtet sich dabei an Apfelsammler
            //-1 richtet sich dabei an Krieger
            if ((markierung.Information == -1 && Kaste == "Krieger") || (markierung.Information == int.MaxValue && Kaste == "Apfelsammler"))
            {
                this.GeheZuZielOptimized(markierung);
            }

        }

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus dem eigenen Volk, so 
        /// wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFreund(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte befreundete Ameise</param>
        public override void SiehtFreund(Ameise ameise)
        {
            
        }

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus einem befreundeten Volk 
        /// (Völker im selben Team), so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtVerb%C3%BCndeten(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte verbündete Ameise</param>
        public override void SiehtVerbündeten(Ameise ameise)
        {

        }

        #endregion

        #region Kampf

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus einem feindlichen Volk, 
        /// so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFeind(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte feindliche Ameise</param>
        public override void SiehtFeind(Ameise ameise)
        {
            if (ameise.AktuelleGeschwindigkeit < ameise.MaximaleGeschwindigkeit)
            {
                //Feind ist gerade am sammeln
                Denke("Feind sammelt");
                if (this.Ziel is Wanze || zielOptimized is Wanze || Kaste == "Apfelsammler")
                {
                    //Schicke die Krieger hin (Die Information enthält bei uns den Empfänger. -1 richtet sich dabei an Krieger)
                    SprüheMarkierung(-1, 200);
                }
                else
                {
                    //Schicke die Krieger hin (Die Information enthält bei uns den Empfänger. -1 richtet sich dabei an Krieger)
                    //Eine Ameise geht bereits hin, also müssen wir eine weniger anfordern
                    SprüheMarkierung(-1, 150);
                    GreifeAn(ameise);
                }
            }

        }

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Wanze, so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFeind(Wanze)"
        /// </summary>
        /// <param name="wanze">Erspähte Wanze</param>
        public override void SiehtFeind(Wanze wanze)
        {
            if (Kaste == "Krieger")
            {
                //Schicke die Krieger hin (Die Information enthält bei uns den Empfänger. -1 richtet sich dabei an Krieger)
                this.SprüheMarkierung(-1, 150);
                Denke("W");
                GreifeAn(wanze);
            }
        }

        /// <summary>
        /// Es kann vorkommen, dass feindliche Lebewesen eine Ameise aktiv angreifen. Sollte 
        /// eine feindliche Ameise angreifen, wird diese Methode hier aufgerufen und die 
        /// Ameise kann entscheiden, wie sie darauf reagieren möchte.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:WirdAngegriffen(Ameise)"
        /// </summary>
        /// <param name="ameise">Angreifende Ameise</param>
        public override void WirdAngegriffen(Ameise ameise)
        {

        }

        /// <summary>
        /// Es kann vorkommen, dass feindliche Lebewesen eine Ameise aktiv angreifen. Sollte 
        /// eine Wanze angreifen, wird diese Methode hier aufgerufen und die Ameise kann 
        /// entscheiden, wie sie darauf reagieren möchte.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:WirdAngegriffen(Wanze)"
        /// </summary>
        /// <param name="wanze">Angreifende Wanze</param>
        public override void WirdAngegriffen(Wanze wanze)
        {
            //Wenn wir angegriffen werden, lassen wir alles stehen und liegen und hauen vor ihr ab
            LasseNahrungFallen();
            GeheWegVon(wanze);
        }

        #endregion
    }
}