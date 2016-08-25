This solution for sample application on UWP app to connect to Bridge the solution will include following Project

- UWPJs ( Javascript Hosted Web App )
This is main Universal Window Platform Project , it will be Javascript Hosted Web app.
- WRC ( C# UWP Runtime Component )
Windows Runtime Component for UWPJs
- Win32Bridge
Classic Console Application that can do thing that UWP Cannot do

To run this Solution.
- open solution and change CPU to x86
- Click on Rebuild Solution , you can see on output windows that when it build Win32Bridge it will copy file to APPX folder of UWPJs
- Set startup project to UWPJs then hit F5
- Click on Start Win32Bridge , it should open console app. if not please make sure that Win32Bride.exe had been copy to AppX folder
- Click on other button , it will call to console app to run code and send result back to Display in UWP


