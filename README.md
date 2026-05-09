# BCSH1 - Semestrální práce: Arena Shooter: Zombie Apocalypse

Jedná se o 2D top-down střílečku, kde se hráč musí bránit nekonečným vlnám zombíků a nahrát co nejvyšší skóre. 

## 🎮 Ovládání

* **W, A, S, D** – Pohyb postavy
* **Pohyb myší** – Míření (rotace postavy za kurzorem)
* **Levé tlačítko myši** – Střelba (tlačítko lze držet pro plynulou palbu)

---

## 🚀 Klíčové vlastnosti a implementované funkce

* **Tři úrovně obtížnosti (Easy, Medium, Hard):** Každá obtížnost nabízí vizuálně odlišnou mapu (Les, Město, Hřbitov) s jinými překážkami, rozdílnou rychlostí spawnování nepřátel a odlišným bodovým ohodnocením za zabití.
* **Procedurální generování mapy:** Překážky jsou na mapě rozmísťovány dynamicky pomocí mřížkového systému (Grid), s ochranou středu (tzv. "Safe Zone") pro spawn hráče.
* **Pokročilé kolize a pohyb (Steering Behaviors):**
  * *Sliding kolize:* Hráč i nepřátelé při nárazu do zdi nezastaví, ale plynule "kloužou" po jejím okraji.
  * *Odstrkování (Repel Force):* Zombíci využívají odpudivé síly, díky kterým dokážou elegantně obcházet překážky a plynule se vyhýbají sobě navzájem.
* **Správa dat a perzistence:** Ukládání nejvyššího dosaženého skóre (High Score) pro každou obtížnost samostatně do souboru `.json`.
* **Architektura a Clean Code:** Projekt striktně dodržuje C# jmenné konvence a pravidla čistého kódu (Single Responsibility Principle, nahrazení "magic numbers" za konstanty) a je logicky rozdělen do jmenných prostorů (`Entities`, `Data`, `GameDialogs`).

---

## 🎨 Použité assety

* **Zdroj:** balíček *Top-down Shooter* (https://kenney.nl/assets/top-down-shooter)
* **Autor:** Kenney (kenney.nl)
* **Licence:** Creative Commons CC0 (Public Domain)

**Seznam použitých souborů (v projektu byly některé přejmenovány pro lepší orientaci):**
* **Postavy:** `soldier1_machine.png`, `zoimbie1_hold.png`
* **Pozadí map:** `tile_3.png`, `tile_6.png`, `tile_169.png` -> `tile_10.png` (`medium_bg.png`)
* **Překážky a objekty:** `tile_156.png`, `tile_183.png`, `tile_186.png`, `tile_210.png`, `tile_237.png`, `tile_238.png`, `tile_243.png`, `tile_292.png`, `tile_318.png`
<img width="638" height="140" alt="Snímek obrazovky 2026-05-08 204844" src="https://github.com/user-attachments/assets/de8caef5-b4dc-44a3-9bb7-dfd0f9ee14bf" />


---

## 💻 Použité technologie a frameworky

* **.NET (verze 8.0/6.0):** Základní vývojová platforma pro běh aplikace. [Odkaz](https://learn.microsoft.com/cs-cz/dotnet/)
* **Windows Forms:** Framework použitý pro tvorbu uživatelského rozhraní a herního okna. [Odkaz](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)
* **System.Text.Json:** Knihovna použitá pro perzistenci dat (ukládání stavu a skóre hry do souboru JSON). [Odkaz](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)

---

## 🤖 Umělá inteligence (AI)

**Gemini (Google)** – Využito jako AI asistent pro:
* Konzultace při návrhu architektury a Clean Code postupů.
* Ladění chyb (debugging) a optimalizaci kódu.
* Vysvětlení a implementaci matematických principů (vektorová normalizace, výpočty vzdáleností, steering behaviors a klouzavé kolize).
