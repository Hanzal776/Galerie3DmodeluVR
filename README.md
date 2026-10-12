# VR 3D Model Gallery (Meta Quest 3)

Půlroční školní projekt zaměřený na vytvoření interaktivní 3D galerie ve virtuální realitě pro **Meta Quest 3**. Projekt je vyvíjen v prostředí **Unity** s využitím **Universal Render Pipeline (URP)** a standardu **OpenXR**.

---

## O projektu

Cílem projektu je vytvořit plynulé a vizuálně atraktivní prostředí 3D galerie, ve které si uživatel může prohlížet různé 3D modely a exponáty, komunikovat s nimi (chytat do ruky, otáčet) a pohybovat se po virtuálním prostoru.

### Klíčové vlastnosti:
* **Cílová platforma:** Meta Quest 3 (Stand-alone VR).
* **Vývojové prostředí:** Unity (URP - Universal Render Pipeline).
* **Interakce:** XR Interaction Toolkit (pohyb, uchopování objektů, paprsková interakce).
* **Standard:** OpenXR pro plnou kompatibilitu s Meta Quest ovladači.

---

## Plán vývoje (Harmonogram do 1. 12.)

* [x] **1. Fáze: Nastavení projektu & VR prostředí**
  * Konfigurace Unity URP a OpenXR pro Meta Quest 3.
  * Nastavení základní scény (`XR Origin`, podlaha, testovací interaktivní objekty).
  * Optimalizace vykreslování pro mobilní čip Quest 3 (vypnutí náročného SSAO).
  * Inicializace repozitáře na GitHubu se správným `.gitignore`.

* [ ] **2. Fáze: Import modelů & Návrh galerie**
  * Výběr a import 3D budovy galerie (Sketchfab / Poly Pizza).
  * Rozmístění podstavců a exponátů.
  * Nastavení materiálu a optimalizovaného osvětlení (Light baking).

* [ ] **3. Fáze: Interakce & UI**
  * Odladění uchopování modelů (`XR Grab Interactable`).
  * Přidání informačních tabulí s popisem exponátů.
  * Implementace plynulého pohybu (Continuous Move) a teleportace.

* [ ] **4. Fáze: Testování & Finální Build**
  * Testování plynulosti na Meta Quest 3 hardware.
  * Finální úpravy, dokumentace a příprava prezentace.

---

## Jak spustit projekt v Unity

1. **Požadavky:**
   * **Unity Editor** (verze s podporou Android Build Support).
   * **Git** nebo **GitHub Desktop** pro klonování repozitáře.

2. **Otevření projektu:**
   * Stáhni/naklonuj tento repozitář na svůj počítač.
   * Otevři **Unity Hub**, klikni na **Add** a vyber složku projektu.
   * Otevři projekt v odpovídající verzi Unity.

3. **Spuštění ve scéně:**
   * V panelu `Project` otevři složku `Assets/Scenes/`.
   * Dvakrát klikni na hlavní scénu (např. `SampleScene.unity`).
   * Stiskni tlačítko **Play** nahoře v Unity Editoru pro otestování scény.

---

## Propojení s Meta Quest 3 & Užitečné tutoriály

### Jak spustit aplikaci přímo v brýlích Meta Quest 3:
1. **Přes kabel / Oculus Link:** Propoj Quest 3 USB-C kabelem s PC, zapni v brýlích *Quest Link* a v Unity zmáčkni **Play**.
2. **Nativní Build (APK):**
   * V Unity jdi do **File** -> **Build Settings**.
   * Přepni platformu na **Android** a klikni na **Switch Platform**.
   * Připoj Quest 3 přes USB a vyber ho v seznamu *Run Device*.
   * Klikni na **Build and Run**.

### Doporučené návody na začátek:
*  [Unity XR Interaction Toolkit Tutorial (YouTube)](https://www.youtube.com/results?search_query=Unity+XR+Interaction+Toolkit+tutorial) — Základy uchopování objektů a VR pohybu.
*  [Meta Quest 3 Unity Setup (YouTube)](https://www.youtube.com/results?search_query=Unity+Meta+Quest+3+setup+OpenXR) — Kompaktní návody na nastavení OpenXR.
*  [Poly Pizza](https://poly.pizza/) & [Sketchfab](https://sketchfab.com/) — Bezplatné 3D modely a exponáty do galerie.
