## Användning av AI

Jag har använt AI (Claude) som stöd under projektet. Delar där hjälpen
varit omfattande är markerade med kommentaren "AI-assisterad" i koden.

### Varför
- Grundstrukturen: jag fick hjälp att dela upp projektet enligt onion-
  arkitekturen med repositories, services, DTO:er och API-controllers.
- Trädstrukturen: jag hade inte byggt rekursiva komponenter tidigare
  och ville förstå hur man gör en lista till ett träd.
- Felsökning: ex. när appen inte startade på Azure, där jag fick
  hjälp att hitta felet i Application Event Logs.
- Tidsbrist: för att hinna med VG-delarna. Detta på grund av att jag
  har haft mycket annat att göra privat under kursen och inte
  kunnat lägga lika mycket tid som jag velat på projektet.
- Styling: jag har använt AI för att styla komponenter med CSS, för
  att spara tid.

### Hur
AI har fungerat som en handledare: förklarat koncept, gett kodexempel
och hjälpt till vid fel. Jag har själv byggt in koden i projektet,
anpassat den, testat, felsökt och publicerat på Azure.

### Vad jag lärt mig
- Hur DTO:er skyddar känslig data, ex. att lösenords-hashen aldrig
  skickas ut via API:t.
- Varför Restrict behövs för trädet: ett inlägg kan inte raderas så
  länge det har svar som pekar på det.
- Skillnaden mellan EventCallback och Func: med EventCallback ritas
  föräldern om automatiskt, med Func måste man anropa StateHasChanged.