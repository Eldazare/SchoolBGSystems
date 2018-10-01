using System;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;

namespace wapi_client
{


    class Program
    {
        static int polltime = 1000; // in milliseconds

        static List<string> commandList = new List<string>{"New", "Join", "Resume", "Find", "Help", "Exit", "Spectate"};
        static List<string> descriptionList = new List<string>{"Create a new game", 
                                                                "Join a game", 
                                                                "Resume a game with specific playerID.",
                                                                "Find games that aren't over, join/spectate with GameNumber afterwards.", 
                                                                "Get descriptions of commands.",
                                                                "Exit the application",
                                                                "Get latest state of a game and get updates when moves are made."};
        static HttpClient client = new HttpClient();
        static List<Guid> gameGList;
        static Guid latestPlayerID;
        static bool escapePressed = false;
        static void Main(string[] args)
        {
            Thread exitThread = new Thread(new ThreadStart(ReadEsc));
            exitThread.Start();
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync(){
            client.BaseAddress = new Uri("http://localhost:5000/api/Game/");
            client.DefaultRequestHeaders.Accept.Clear();
            /* 
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            */
            try{
                HttpResponseMessage responseInfo = await client.GetAsync($"Info");
                if (responseInfo.IsSuccessStatusCode){
                    string info = await responseInfo.Content.ReadAsStringAsync();
                    Console.Write(info);
                } else{
                    Console.WriteLine("GetInfo failed");
                }
            } catch (Exception e){
                Console.WriteLine("WebApi not detected, with message:"+Environment.NewLine+ e.Message);
            }
            while (true){
                int command = -1;
                Console.Write(Environment.NewLine);
                while (command == -1){
                    command = GetCommand();
                }
                switch(command){
                case 0: // new
                    HttpResponseMessage responseCreate = await client.GetAsync($"New");
                    if (responseCreate.IsSuccessStatusCode){
                        Guid gameID = await responseCreate.Content.ReadAsAsync<Guid>();
                        Console.WriteLine("New room created with ID: "+gameID);
                        Console.WriteLine("Do you want to join the created room? (Y/N)");
                        string res = "";
                        while (true){
                            res = Console.ReadLine();
                            if (res == "Y" || res == "N"){
                                break;
                            }
                        }
                        if (res == "Y"){
                            HttpResponseMessage responseJoin = await client.GetAsync($"Join/{gameID}");
                            if (responseJoin.IsSuccessStatusCode){
                                Guid playerID = await responseJoin.Content.ReadAsAsync<Guid>();
                                latestPlayerID = playerID;
                                Console.WriteLine("Joined the game with playerID "+playerID);
                                await PlayMode();
                            } else {
                                Console.WriteLine("Join response didn't succeed..."+responseJoin.Content.ReadAsStringAsync());
                            }
                        }
                    }
                    break;
                case 1: // join
                    string line = "";
                    while (line == ""){
                        Console.WriteLine("Enter a game Guid or GameNumber to join or Exit to stop inputting");
                        line = GetGuidOrCancel();
                    }
                    int tryInt1;
                    if (int.TryParse(line, out tryInt1)){
                        line = gameGList[tryInt1].ToString();
                    }
                    if (line != "Exit"){
                        Guid gameID = Guid.Parse(line);
                        Console.WriteLine("Joining to game "+gameID);
                        HttpResponseMessage responseJoin = await client.GetAsync($"Join/{gameID}");
                        if (responseJoin.IsSuccessStatusCode){
                            Guid playerID = await responseJoin.Content.ReadAsAsync<Guid>();
                            latestPlayerID = playerID;
                            Console.WriteLine("Joined the game with playerID "+playerID);
                            await PlayMode();
                        } else {
                            string joinResp = await responseJoin.Content.ReadAsStringAsync();
                            Console.WriteLine("Join response didn't succeed...:"+Environment.NewLine+joinResp);
                        }
                    }
                    break;
                case 2: // Resume
                    Console.WriteLine("Give playerID to resume a game");
                    string input = "";
                    while (input == ""){
                        input = GetGuidOrCancel();
                        int tryInt2;
                        if (int.TryParse(input, out tryInt2)){
                            input = "";
                        }
                    }
                    if (input != "Exit"){
                        HttpResponseMessage responseResume = await client.GetAsync($"Play/{input}");
                        if (responseResume.IsSuccessStatusCode){
                            string respResume = await responseResume.Content.ReadAsStringAsync();
                            if (respResume.Split(':')[0] != "OVER"){
                                latestPlayerID = Guid.Parse(input);
                                await PlayMode();
                            } else {
                                Console.Write(respResume);
                            }
                        }
                    }
                    break;
                case 3: // find
                    HttpResponseMessage responseFind = await client.GetAsync($"");
                    if (responseFind.IsSuccessStatusCode){
                        string respFind = await responseFind.Content.ReadAsStringAsync();
                        Console.Write(respFind);
                        gameGList = new List<Guid>();
                        string[] arr = respFind.Split(Environment.NewLine);
                        foreach(string str in arr){
                            string[] arrSplt = str.Split('|');
                            if (arrSplt.Length>1){
                                Guid tryGuid;
                                if (Guid.TryParse(arrSplt[1], out tryGuid)){
                                    gameGList.Add(tryGuid);
                                }
                            }
                        }
                    } else {
                        string respFind = await responseFind.Content.ReadAsStringAsync();
                        Console.Write(respFind);
                    }
                    break;
                case 4: // help 
                for (int i = 0; i<commandList.Count;i++){
                    Console.WriteLine(commandList[i]+": "+descriptionList[i]);
                }
                    break;
                case 5: // exit
                    Environment.Exit(0);
                    break;
                case 6: // spectate
                    string specGuid = "";
                    Console.WriteLine("Give game Guid or GameNumber to spectate: ");
                    while (specGuid == ""){
                        specGuid = GetGuidOrCancel();
                    }
                    int tryInt6;
                    if (int.TryParse(specGuid, out tryInt6)){
                        specGuid = gameGList[tryInt6].ToString();
                    }
                    if (specGuid != "Exit"){
                        escapePressed = false;
                        int lastPlayer = -1;
                        Console.WriteLine("Attempting to spectate game "+specGuid);
                        while (!escapePressed){
                            HttpResponseMessage response = await client.GetAsync($"{specGuid}");
                            if (response.IsSuccessStatusCode){
                                string respResume = await response.Content.ReadAsStringAsync();
                                string identifier = respResume.Split(':')[0];
                                int identInt;
                                if (int.TryParse(identifier, out identInt)){
                                    if (identInt != lastPlayer){
                                        lastPlayer = identInt;
                                        Console.Write(respResume);
                                    }
                                } else if (identifier == "OVER"){
                                    Console.Write(respResume);
                                    break;
                                }
                            } else {
                                string respResume = await response.Content.ReadAsStringAsync();
                                Console.Write(respResume);
                                break;
                            }
                            System.Threading.Thread.Sleep(polltime*2);
                        }
                    }
                    break;
                default:
                Console.WriteLine("Command not registered: "+command);
                    break;
                }
            }
        }

        static async Task PlayMode(){
            bool firstReturn = false;
            escapePressed = false;
            while (!escapePressed){
                HttpResponseMessage response = await client.GetAsync($"Play/{latestPlayerID}");
                if (response.IsSuccessStatusCode){
                    string resp = await response.Content.ReadAsStringAsync();
                    string status = resp.Split(':')[0];
                    if (status == "TRUE"){
                        Console.Write(resp);
                        int move = -1;
                        while (move == -1){
                            Console.WriteLine("Give a move: (1-9)");
                            move = GetMove();
                        }
                        HttpResponseMessage response2 = await client.GetAsync($"Play/{latestPlayerID}?move={move}");
                        if (response2.IsSuccessStatusCode){
                            string resp2 = await response2.Content.ReadAsStringAsync();
                            Console.Write(resp2);
                            if (resp2.Split(':')[0] == "OVER"){
                                return;
                            }
                            System.Threading.Thread.Sleep(polltime*2);
                        } else {
                            string resp2 = await response2.Content.ReadAsStringAsync();
                            Console.Write("Move giving failed.");
                            Console.Write(resp2);
                        }
                    } else if (status == "FALSE"){
                        if (!firstReturn){
                            Console.Write(resp);
                        }
                        System.Threading.Thread.Sleep(polltime);
                    } else if (status == "OVER"){
                        Console.Write(resp);
                        return;
                    }
                    firstReturn = true;
                } else {
                    Console.WriteLine("Unsuccessful query");
                    Console.WriteLine(response.Content.ReadAsStringAsync());
                    return;
                }
            }
        }
        static int GetCommand(){
            Console.WriteLine("Give command: (Command Help for list of commands)");
            string line = Console.ReadLine();
            if (commandList.Contains(line)){
                return commandList.IndexOf(line);
            } else {
                return -1;
            }
        }

        static string GetGuidOrCancel(){
            string line = Console.ReadLine();
            if (line == "Exit"){
                return line;
            } else {
                int fluff;
                if (int.TryParse(line, out fluff)){
                    if (gameGList != null){
                        if (gameGList.Count>=fluff && fluff > 0){
                            return (fluff-1).ToString();
                        }
                    }
                }
                try{
                    Guid.Parse(line);
                    return line;
                } catch{
                    Console.WriteLine("Guid parse unsuccessful.");
                    return "";
                }
            }
        }

        static int GetMove(){
            string line = Console.ReadLine();
            int o;
            if (int.TryParse(line, out o)){
                if (o >0 && o<10){
                    return o;
                }
            }
            Console.WriteLine("Invalid input clientside.");
            return -1;
        
        }
        static void ReadEsc(){
            while(true){
                if (Console.KeyAvailable){
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape){
                        Console.WriteLine("ESC PRESSED!");
                        escapePressed = true;
                    }
                }
            }
        }
    }
}
