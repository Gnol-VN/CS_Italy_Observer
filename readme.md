# Azure Function application - Cron
This is an example application to pool Xbox series X stock in Smyth Ireland for every hour.   
When the stock is avaiable, it will send a push notification to my phone.


# Step
1. From Azure portal, create a new resource type Function App
2. Add a new function app to your resource group
    1. On Basics tab, give it a name, version, region...
    2. On Hosting tab, choose a *Storage Account*, and choose OS: Windows
    3. On Monitoring tab, select **No***
    4. Click **Review + create**
3. Wait for a minute, and go to Resource group > newly created Function App
4. In left pane, select Functions, add a new Function
    1. Choose **Timer trigger**
    2. Give it a name and cron expression, for example: 0 */5 * * * * (Every 5 minutes, replace 5 by 10 or 60 up to you)
5. Click in the newly created function name, select **Code + Test**. Write your code here **inside Run()** method
    1. Click save
6. **(Most important)** If you import any Nuget package:
    1. Back to Function App blade, in left pane, scroll down, select Advanced Tools, click Go, It will lead you to Kudu dashboard
    2. In top nav, select Debug Console -> Powershell
    3. A directory viewer appear, go to \site\wwwroot\{functionname}
    4. You will see some files: run.csx, readme.md,...
    5. Click on Plus button to create a new file, name it exactly: **function.proj**
    6. Edit the content like below, and click save
    
    ```
    <Project Sdk="Microsoft.NET.Sdk">
        <PropertyGroup>
            <TargetFramework>netstandard2.0</TargetFramework>
        </PropertyGroup>
        <ItemGroup>
            <PackageReference Include="HtmlAgilityPack" Version="1.11.24" />
            <PackageReference Include="HtmlAgilityPack.CssSelectors.NetCore" Version="1.2.1" />
            <PackageReference Include="Expo.Server.SDK" Version="1.0.2" />
        </ItemGroup>
    </Project>
    ```
7. Come back to **Code + Test** blade on step 5, click *Test / Run* button 
    